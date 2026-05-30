#!/usr/bin/env python3
"""Convert a user-recorded runtime MP4 into a small validated portfolio GIF.

Default use from the Unity project root:

    python3 Docs/Captures/convert_runtime_clip.py /path/to/runtime_clip.mp4

The script is intentionally ffmpeg-based so Windows Game Bar/OBS recordings can
be converted from WSL without opening Unity. Raw MP4 files should stay outside
git; commit only the compressed GIF/preview after validation passes.
"""

from __future__ import annotations

import argparse
import json
import math
import shutil
import subprocess
import sys
import tempfile
from dataclasses import dataclass
from pathlib import Path
from typing import Any, NoReturn

CAPTURE_DIR = Path(__file__).resolve().parent
DEFAULT_GIF = CAPTURE_DIR / "codex_tactics_runtime_clip.gif"
DEFAULT_PREVIEW = CAPTURE_DIR / "codex_tactics_runtime_clip_preview.png"
DEFAULT_WIDTH = 960
DEFAULT_FPS = 12
DEFAULT_DURATION = 12.0
DEFAULT_MAX_SIZE_MB = 5.0
DEFAULT_PREVIEW_FRAMES = 12


@dataclass(frozen=True)
class VideoInfo:
    width: int
    height: int
    duration: float


@dataclass(frozen=True)
class GifInfo:
    width: int
    height: int
    frames: int
    size_bytes: int


def fail(message: str) -> NoReturn:
    raise SystemExit(f"ERROR: {message}")


def require_tool(name: str) -> str:
    path = shutil.which(name)
    if path is None:
        fail(
            f"{name} was not found on PATH. Install ffmpeg/ffprobe first "
            "(for WSL Ubuntu: sudo apt update && sudo apt install ffmpeg)."
        )
    return path


def run_command(command: list[str], *, capture_json: bool = False) -> Any:
    completed: subprocess.CompletedProcess[str] | None = None
    try:
        completed = subprocess.run(
            command,
            check=True,
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
        )
    except FileNotFoundError:
        fail(f"Command not found: {command[0]}")
    except subprocess.CalledProcessError as exc:
        detail = (exc.stderr or exc.stdout or "").strip()
        fail(f"Command failed: {' '.join(command)}\n{detail}")

    if completed is None:
        fail(f"Command did not complete: {' '.join(command)}")

    if capture_json:
        try:
            return json.loads(completed.stdout)
        except json.JSONDecodeError as exc:
            fail(f"Could not parse ffprobe JSON output: {exc}")
    return completed.stdout


def probe_video(input_path: Path) -> VideoInfo:
    data = run_command(
        [
            "ffprobe",
            "-v",
            "error",
            "-select_streams",
            "v:0",
            "-show_entries",
            "stream=width,height,duration:format=duration",
            "-of",
            "json",
            str(input_path),
        ],
        capture_json=True,
    )
    streams = data.get("streams") or []
    if not streams:
        fail(f"No video stream found in {input_path}")

    stream = streams[0]
    width = int(stream.get("width") or 0)
    height = int(stream.get("height") or 0)
    duration_raw = stream.get("duration") or (data.get("format") or {}).get("duration")
    try:
        duration = float(duration_raw)
    except (TypeError, ValueError):
        duration = 0.0

    if width <= 0 or height <= 0:
        fail(f"Invalid source dimensions: {width}x{height}")
    if duration <= 0:
        fail("Could not determine source duration; use a normal MP4 recording with a video duration.")

    return VideoInfo(width=width, height=height, duration=duration)


def probe_gif(gif_path: Path) -> GifInfo:
    data = run_command(
        [
            "ffprobe",
            "-v",
            "error",
            "-select_streams",
            "v:0",
            "-count_frames",
            "-show_entries",
            "stream=width,height,nb_read_frames",
            "-of",
            "json",
            str(gif_path),
        ],
        capture_json=True,
    )
    streams = data.get("streams") or []
    if not streams:
        fail(f"No video stream found in generated GIF: {gif_path}")
    stream = streams[0]
    return GifInfo(
        width=int(stream.get("width") or 0),
        height=int(stream.get("height") or 0),
        frames=int(stream.get("nb_read_frames") or 0),
        size_bytes=gif_path.stat().st_size,
    )


def effective_duration(source: VideoInfo, start: float, requested_duration: float) -> float:
    if start < 0:
        fail("--start must be 0 or greater.")
    if requested_duration <= 0:
        fail("--duration must be greater than 0.")
    remaining = source.duration - start
    if remaining <= 0:
        fail(f"--start {start:.2f}s is beyond source duration {source.duration:.2f}s.")
    return min(requested_duration, remaining)


def build_gif(input_path: Path, output_gif: Path, args: argparse.Namespace) -> None:
    output_gif.parent.mkdir(parents=True, exist_ok=True)
    vf_palette = f"fps={args.fps},scale={args.width}:-2:flags=lanczos,palettegen=max_colors={args.colors}"
    vf_gif = (
        f"fps={args.fps},scale={args.width}:-2:flags=lanczos[x];"
        f"[x][1:v]paletteuse=dither={args.dither}"
    )

    with tempfile.TemporaryDirectory(prefix="codex_tactics_clip_") as temp_dir:
        palette = Path(temp_dir) / "palette.png"
        run_command(
            [
                "ffmpeg",
                "-hide_banner",
                "-y",
                "-ss",
                f"{args.start:.3f}",
                "-t",
                f"{args.duration:.3f}",
                "-i",
                str(input_path),
                "-vf",
                vf_palette,
                str(palette),
            ]
        )
        run_command(
            [
                "ffmpeg",
                "-hide_banner",
                "-y",
                "-ss",
                f"{args.start:.3f}",
                "-t",
                f"{args.duration:.3f}",
                "-i",
                str(input_path),
                "-i",
                str(palette),
                "-filter_complex",
                vf_gif,
                "-loop",
                "0",
                str(output_gif),
            ]
        )


def build_preview(input_path: Path, preview_path: Path, args: argparse.Namespace, clip_duration: float) -> None:
    preview_path.parent.mkdir(parents=True, exist_ok=True)
    columns = 4
    rows = math.ceil(args.preview_frames / columns)
    sample_fps = max(args.preview_frames / max(clip_duration, 0.1), 0.1)
    vf = f"fps={sample_fps:.4f},scale=320:-2:flags=lanczos,tile={columns}x{rows}"
    run_command(
        [
            "ffmpeg",
            "-hide_banner",
            "-y",
            "-ss",
            f"{args.start:.3f}",
            "-t",
            f"{clip_duration:.3f}",
            "-i",
            str(input_path),
            "-vf",
            vf,
            "-frames:v",
            "1",
            str(preview_path),
        ]
    )


def validate_outputs(output_gif: Path, preview_path: Path, args: argparse.Namespace, clip_duration: float) -> GifInfo:
    if not output_gif.exists():
        fail(f"GIF was not written: {output_gif}")
    if not preview_path.exists():
        fail(f"Preview/contact sheet was not written: {preview_path}")

    gif = probe_gif(output_gif)
    if gif.width != args.width:
        fail(f"Unexpected GIF width: {gif.width}, expected {args.width}")
    if gif.height <= 0:
        fail(f"Invalid GIF height: {gif.height}")

    expected_min = max(1, math.floor(clip_duration * args.fps * 0.65))
    expected_max = max(expected_min, math.ceil(clip_duration * args.fps * 1.35) + 2)
    if gif.frames < expected_min or gif.frames > expected_max:
        fail(
            f"Unexpected GIF frame count: {gif.frames}, expected roughly "
            f"{expected_min}-{expected_max} for {clip_duration:.2f}s at {args.fps} fps"
        )

    max_bytes = int(args.max_size_mb * 1_000_000)
    if gif.size_bytes < 10_000:
        fail(f"GIF looks too small to be valid: {gif.size_bytes} bytes")
    if gif.size_bytes > max_bytes:
        fail(
            f"GIF is too large for README use: {gif.size_bytes} bytes "
            f"> {max_bytes} bytes. Try --duration 8, --fps 10, --width 720, or --colors 64."
        )

    preview_size = preview_path.stat().st_size
    if preview_size < 10_000:
        fail(f"Preview/contact sheet looks too small to be valid: {preview_size} bytes")

    return gif


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Trim, resize, fps-limit, and validate a portfolio GIF from a runtime MP4.",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter,
    )
    parser.add_argument("input_mp4", type=Path, help="User-recorded runtime MP4, preferably kept outside git.")
    parser.add_argument("--output", type=Path, default=DEFAULT_GIF, help="Output GIF path.")
    parser.add_argument("--preview", type=Path, default=DEFAULT_PREVIEW, help="Preview/contact-sheet PNG path.")
    parser.add_argument("--start", type=float, default=0.0, help="Trim start time in seconds.")
    parser.add_argument("--duration", type=float, default=DEFAULT_DURATION, help="Trim duration in seconds; 8-15s is recommended.")
    parser.add_argument("--width", type=int, default=DEFAULT_WIDTH, help="Output GIF width in pixels; height keeps source aspect ratio.")
    parser.add_argument("--fps", type=int, default=DEFAULT_FPS, help="Output GIF frame rate.")
    parser.add_argument("--colors", type=int, default=96, help="Palette size for GIF compression.")
    parser.add_argument("--dither", default="bayer", choices=["bayer", "floyd_steinberg", "sierra2_4a", "none"], help="GIF palette dithering mode.")
    parser.add_argument("--max-size-mb", type=float, default=DEFAULT_MAX_SIZE_MB, help="Validation cap for committed README GIF size.")
    parser.add_argument("--preview-frames", type=int, default=DEFAULT_PREVIEW_FRAMES, help="Number of preview samples for the contact sheet.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    require_tool("ffmpeg")
    require_tool("ffprobe")

    input_path = args.input_mp4.expanduser().resolve()
    if not input_path.exists():
        fail(f"Input MP4 not found: {input_path}")
    if input_path.suffix.lower() != ".mp4":
        fail(f"Expected an .mp4 input file, got: {input_path.name}")
    if args.width <= 0:
        fail("--width must be greater than 0.")
    if args.fps <= 0:
        fail("--fps must be greater than 0.")
    if args.colors < 2 or args.colors > 256:
        fail("--colors must be between 2 and 256.")
    if args.preview_frames <= 0:
        fail("--preview-frames must be greater than 0.")

    source = probe_video(input_path)
    clip_duration = effective_duration(source, args.start, args.duration)
    if clip_duration < 8.0 or clip_duration > 15.0:
        print(
            f"NOTE: clip duration after trimming is {clip_duration:.2f}s; "
            "8-15s is recommended for portfolio README GIFs.",
            file=sys.stderr,
        )

    output_gif = args.output.expanduser().resolve()
    preview_path = args.preview.expanduser().resolve()
    build_gif(input_path, output_gif, args)
    build_preview(input_path, preview_path, args, clip_duration)
    gif = validate_outputs(output_gif, preview_path, args, clip_duration)

    print("Runtime clip conversion: PASS")
    print(f"Source: {input_path}")
    print(f"Source video: {source.width}x{source.height}, {source.duration:.2f}s")
    print(f"Trim: start {args.start:.2f}s, duration {clip_duration:.2f}s")
    print(f"GIF: {output_gif}")
    print(f"GIF metrics: {gif.width}x{gif.height}, {gif.frames} frames, {gif.size_bytes} bytes")
    print(f"Preview/contact sheet: {preview_path} ({preview_path.stat().st_size} bytes)")
    print("Recommended git hygiene: do not commit the raw MP4; commit only the validated GIF/preview if they are portfolio-ready.")


if __name__ == "__main__":
    main()

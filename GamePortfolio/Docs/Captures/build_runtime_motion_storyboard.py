#!/usr/bin/env python3
"""Build a runtime-motion-focused storyboard GIF from capture PNGs.

Run from the Unity project root:

    python3 Docs/Captures/build_runtime_motion_storyboard.py

This does not replace true video capture. It creates a small, verified motion
preview by selecting action-heavy capture frames and adding pan/zoom beats that
make Fire, Guard, result, and retry moments easier to review in a portfolio.
"""

from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
from typing import Any

from PIL import Image, ImageDraw, ImageFont, ImageSequence

CAPTURE_DIR = Path(__file__).resolve().parent
OUTPUT_GIF = CAPTURE_DIR / "codex_tactics_runtime_motion_storyboard.gif"
PREVIEW_SHEET = CAPTURE_DIR / "codex_tactics_runtime_motion_storyboard_preview.png"
TARGET_SIZE = (960, 540)
MIN_SOURCE_SIZE = (960, 540)
FRAME_DURATION_MS = 120


@dataclass(frozen=True)
class Beat:
    filename: str
    caption: str
    frames: int
    start_zoom: float
    end_zoom: float
    pan_x: float
    pan_y: float


# Use the frames most likely to show motion/VFX/readable combat feedback.
BEATS = [
    Beat("01_battle_start.png", "Runtime setup: selected ally, tactical HUD, target lane", 5, 1.00, 1.03, -0.05, -0.02),
    Beat("02_fire_skill_burn.png", "Action beat: Fire Skill impact, projectile/VFX, Burn feedback", 6, 1.02, 1.10, 0.12, -0.03),
    Beat("03_ice_lance_stun.png", "Action beat: Ice Lance follow-up and Stun feedback", 6, 1.02, 1.09, 0.10, -0.02),
    Beat("03_guard_status.png", "Defense beat: Guard status and enemy-response readability", 6, 1.00, 1.07, -0.10, 0.04),
    Beat("04_result_summary_rank.png", "Resolution beat: result metrics, rank, reward, clear state", 6, 1.01, 1.05, 0.02, 0.08),
    Beat("05_retry_reset.png", "Loop beat: retry reset ready for another run", 5, 1.00, 1.04, -0.04, -0.05),
]


def load_font(size: int) -> Any:
    candidates = [
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/liberation2/LiberationSans-Bold.ttf",
    ]

    for candidate in candidates:
        path = Path(candidate)
        if path.exists():
            return ImageFont.truetype(str(path), size)

    return ImageFont.load_default()


def validate_sources() -> None:
    missing = [beat.filename for beat in BEATS if not (CAPTURE_DIR / beat.filename).exists()]
    if missing:
        raise SystemExit("Missing runtime-motion source PNGs: " + ", ".join(missing))

    for beat in BEATS:
        with Image.open(CAPTURE_DIR / beat.filename) as source:
            if source.width < MIN_SOURCE_SIZE[0] or source.height < MIN_SOURCE_SIZE[1]:
                raise SystemExit(
                    f"Source too small for motion storyboard: {beat.filename} "
                    f"is {source.width}x{source.height}, expected at least "
                    f"{MIN_SOURCE_SIZE[0]}x{MIN_SOURCE_SIZE[1]}"
                )


def fit_cover(image: Image.Image, zoom: float, pan_x: float, pan_y: float) -> Image.Image:
    image = image.convert("RGB")
    base_scale = max(TARGET_SIZE[0] / image.width, TARGET_SIZE[1] / image.height)
    scale = base_scale * zoom
    resized = image.resize((round(image.width * scale), round(image.height * scale)), Image.Resampling.LANCZOS)

    max_x = max(0, resized.width - TARGET_SIZE[0])
    max_y = max(0, resized.height - TARGET_SIZE[1])
    # pan values are normalized offsets around center: -1.0 left/up, +1.0 right/down.
    left = round(max_x * (0.5 + pan_x * 0.5))
    top = round(max_y * (0.5 + pan_y * 0.5))
    left = min(max(left, 0), max_x)
    top = min(max(top, 0), max_y)

    return resized.crop((left, top, left + TARGET_SIZE[0], top + TARGET_SIZE[1]))


def add_caption(image: Image.Image, text: str, beat_index: int, total_beats: int) -> Image.Image:
    draw = ImageDraw.Draw(image, "RGBA")
    title_font = load_font(24)
    meta_font = load_font(17)

    band_h = 70
    draw.rectangle((0, TARGET_SIZE[1] - band_h, TARGET_SIZE[0], TARGET_SIZE[1]), fill=(5, 7, 12, 205))
    draw.rectangle((0, TARGET_SIZE[1] - band_h, TARGET_SIZE[0], TARGET_SIZE[1] - band_h + 3), fill=(217, 166, 73, 235))
    draw.text((26, TARGET_SIZE[1] - 54), text, font=title_font, fill=(245, 238, 214, 255))
    draw.text(
        (TARGET_SIZE[0] - 110, TARGET_SIZE[1] - 28),
        f"{beat_index}/{total_beats}",
        font=meta_font,
        fill=(179, 198, 255, 255),
    )
    return image


def build_frames() -> list[Image.Image]:
    frames: list[Image.Image] = []
    total_beats = len(BEATS)

    for beat_index, beat in enumerate(BEATS, start=1):
        with Image.open(CAPTURE_DIR / beat.filename) as source:
            for step in range(beat.frames):
                t = 0.0 if beat.frames == 1 else step / (beat.frames - 1)
                eased = t * t * (3.0 - 2.0 * t)
                zoom = beat.start_zoom + (beat.end_zoom - beat.start_zoom) * eased
                pan_x = beat.pan_x * eased
                pan_y = beat.pan_y * eased
                frame = fit_cover(source, zoom, pan_x, pan_y)
                frame = add_caption(frame, beat.caption, beat_index, total_beats)
                frames.append(frame)

    return frames


def quantize_frames(frames: list[Image.Image]) -> list[Image.Image]:
    return [
        frame.quantize(colors=128, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.FLOYDSTEINBERG)
        for frame in frames
    ]


def build_preview(frames: list[Image.Image]) -> None:
    thumb_w, thumb_h = 240, 135
    columns = 5
    rows = (len(BEATS) + columns - 1) // columns
    sheet = Image.new("RGB", (columns * thumb_w, rows * thumb_h), (8, 9, 14))

    frame_cursor = 0
    for i, beat in enumerate(BEATS):
        sample_index = frame_cursor + beat.frames // 2
        thumb = frames[sample_index].convert("RGB").resize((thumb_w, thumb_h), Image.Resampling.LANCZOS)
        x = (i % columns) * thumb_w
        y = (i // columns) * thumb_h
        sheet.paste(thumb, (x, y))
        frame_cursor += beat.frames

    sheet.save(PREVIEW_SHEET, optimize=True)


def validate_output(expected_frames: int) -> None:
    with Image.open(OUTPUT_GIF) as gif:
        frame_count = sum(1 for _ in ImageSequence.Iterator(gif))
        if gif.size != TARGET_SIZE:
            raise SystemExit(f"Unexpected GIF size: {gif.size}, expected {TARGET_SIZE}")
        if frame_count != expected_frames:
            raise SystemExit(f"Unexpected GIF frame count: {frame_count}, expected {expected_frames}")

    size = OUTPUT_GIF.stat().st_size
    if size < 150_000:
        raise SystemExit(f"Motion storyboard GIF looks too small to be valid: {size} bytes")
    if size > 8_000_000:
        raise SystemExit(f"Motion storyboard GIF is too large for README use: {size} bytes")

    with Image.open(PREVIEW_SHEET) as preview:
        columns = 5
        rows = (len(BEATS) + columns - 1) // columns
        expected_preview_size = (240 * columns, 135 * rows)
        if preview.size != expected_preview_size:
            raise SystemExit(f"Unexpected preview sheet size: {preview.size}, expected {expected_preview_size}")


def main() -> None:
    validate_sources()
    frames = build_frames()
    gif_frames = quantize_frames(frames)
    gif_frames[0].save(
        OUTPUT_GIF,
        save_all=True,
        append_images=gif_frames[1:],
        duration=FRAME_DURATION_MS,
        loop=0,
        optimize=True,
        disposal=2,
    )
    build_preview(frames)
    validate_output(len(frames))

    print(f"Wrote {OUTPUT_GIF}")
    print(f"Wrote {PREVIEW_SHEET}")
    print(f"GIF: {TARGET_SIZE[0]}x{TARGET_SIZE[1]}, {len(frames)} frames, {OUTPUT_GIF.stat().st_size} bytes")


if __name__ == "__main__":
    main()

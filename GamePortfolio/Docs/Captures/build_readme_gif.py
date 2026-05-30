#!/usr/bin/env python3
"""Build the README gameplay GIF from the current portfolio capture PNGs.

Run from the Unity project root:

    python3 Docs/Captures/build_readme_gif.py

The script intentionally depends only on Pillow so it can run from WSL without
opening Unity, as long as the capture PNGs in Docs/Captures are already fresh.
"""

from __future__ import annotations

from pathlib import Path
from typing import Any, Iterable

from PIL import Image, ImageDraw, ImageFont, ImageSequence

CAPTURE_DIR = Path(__file__).resolve().parent
OUTPUT_GIF = CAPTURE_DIR / "codex_tactics_battle_loop.gif"
PREVIEW_SHEET = CAPTURE_DIR / "codex_tactics_battle_loop_preview.png"
TARGET_SIZE = (960, 540)

FRAMES = [
    ("00_title_scene.png", "Title -> Stage Select -> Battle vertical slice", 850),
    ("00_stage_select_scene.png", "Stage Select: modifier preview and start flow", 950),
    ("01_battle_start.png", "Battle start: tactical HUD, HP/AP bars, rosters", 950),
    ("02_fire_skill_burn.png", "Fire Skill: projectile, impact, Burn feedback", 1100),
    ("03_guard_status.png", "Guard: defensive status and enemy response", 1100),
    ("04_result_summary_rank.png", "Result: rank, reward, damage metrics", 1300),
    ("05_retry_reset.png", "Retry: loop resets cleanly for another run", 900),
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


def fit_frame(image: Image.Image) -> Image.Image:
    image = image.convert("RGB")
    image.thumbnail(TARGET_SIZE, Image.Resampling.LANCZOS)

    canvas = Image.new("RGB", TARGET_SIZE, (10, 12, 18))
    x = (TARGET_SIZE[0] - image.width) // 2
    y = (TARGET_SIZE[1] - image.height) // 2
    canvas.paste(image, (x, y))
    return canvas


def add_caption(image: Image.Image, text: str, index: int, total: int) -> Image.Image:
    draw = ImageDraw.Draw(image, "RGBA")
    title_font = load_font(26)
    meta_font = load_font(18)

    band_h = 74
    draw.rectangle((0, TARGET_SIZE[1] - band_h, TARGET_SIZE[0], TARGET_SIZE[1]), fill=(5, 7, 12, 210))
    draw.rectangle((0, TARGET_SIZE[1] - band_h, TARGET_SIZE[0], TARGET_SIZE[1] - band_h + 3), fill=(217, 166, 73, 230))
    draw.text((28, TARGET_SIZE[1] - 58), text, font=title_font, fill=(245, 238, 214, 255))
    draw.text(
        (TARGET_SIZE[0] - 138, TARGET_SIZE[1] - 31),
        f"{index}/{total}",
        font=meta_font,
        fill=(179, 198, 255, 255),
    )
    return image


def quantize_frames(frames: Iterable[Image.Image]) -> list[Image.Image]:
    return [frame.quantize(colors=128, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.FLOYDSTEINBERG) for frame in frames]


def build_preview(frames: list[Image.Image]) -> None:
    thumb_w, thumb_h = 320, 180
    columns = 2
    rows = (len(frames) + columns - 1) // columns
    sheet = Image.new("RGB", (columns * thumb_w, rows * thumb_h), (8, 9, 14))

    for i, frame in enumerate(frames):
        thumb = frame.convert("RGB").resize((thumb_w, thumb_h), Image.Resampling.LANCZOS)
        x = (i % columns) * thumb_w
        y = (i // columns) * thumb_h
        sheet.paste(thumb, (x, y))

    sheet.save(PREVIEW_SHEET, optimize=True)


def validate_gif() -> None:
    with Image.open(OUTPUT_GIF) as gif:
        frame_count = sum(1 for _ in ImageSequence.Iterator(gif))
        if gif.size != TARGET_SIZE:
            raise SystemExit(f"Unexpected GIF size: {gif.size}, expected {TARGET_SIZE}")
        if frame_count != len(FRAMES):
            raise SystemExit(f"Unexpected GIF frame count: {frame_count}, expected {len(FRAMES)}")

    if OUTPUT_GIF.stat().st_size < 100_000:
        raise SystemExit(f"GIF looks too small to be valid: {OUTPUT_GIF.stat().st_size} bytes")


def main() -> None:
    missing = [name for name, _, _ in FRAMES if not (CAPTURE_DIR / name).exists()]
    if missing:
        raise SystemExit("Missing capture PNGs: " + ", ".join(missing))

    frames: list[Image.Image] = []
    durations: list[int] = []
    total = len(FRAMES)

    for index, (name, caption, duration) in enumerate(FRAMES, start=1):
        with Image.open(CAPTURE_DIR / name) as source:
            frame = fit_frame(source)
            frame = add_caption(frame, caption, index, total)
            frames.append(frame)
            durations.append(duration)

    gif_frames = quantize_frames(frames)
    gif_frames[0].save(
        OUTPUT_GIF,
        save_all=True,
        append_images=gif_frames[1:],
        duration=durations,
        loop=0,
        optimize=True,
        disposal=2,
    )

    build_preview(frames)
    validate_gif()

    print(f"Wrote {OUTPUT_GIF}")
    print(f"Wrote {PREVIEW_SHEET}")
    print(f"GIF: {TARGET_SIZE[0]}x{TARGET_SIZE[1]}, {len(FRAMES)} frames, {OUTPUT_GIF.stat().st_size} bytes")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""Build the portfolio capture contact sheet from deterministic capture PNGs.

Run from the Unity project root:

    python3 Docs/Captures/build_capture_contact_sheet.py
"""

from __future__ import annotations

from pathlib import Path
from typing import Any

from PIL import Image, ImageDraw, ImageFont

CAPTURE_DIR = Path(__file__).resolve().parent
OUTPUT = CAPTURE_DIR / "capture_contact_sheet.png"
THUMB_SIZE = (480, 270)
PADDING = 18
LABEL_HEIGHT = 32
COLUMNS = 2
ITEMS = [
    ("1. Title", "00_title_scene.png"),
    ("2. Stage Select", "00_stage_select_scene.png"),
    ("3. Battle Start", "01_battle_start.png"),
    ("4. Fire Skill", "02_fire_skill_burn.png"),
    ("5. Ice Lance", "03_ice_lance_stun.png"),
    ("6. Guard", "03_guard_status.png"),
    ("7. Result", "04_result_summary_rank.png"),
    ("8. Retry Reset", "05_retry_reset.png"),
]


def load_font(size: int) -> Any:
    for candidate in [
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/liberation2/LiberationSans-Bold.ttf",
    ]:
        path = Path(candidate)
        if path.exists():
            return ImageFont.truetype(str(path), size)
    return ImageFont.load_default()


def main() -> None:
    missing = [filename for _, filename in ITEMS if not (CAPTURE_DIR / filename).exists()]
    if missing:
        raise SystemExit("Missing capture PNGs: " + ", ".join(missing))

    rows = (len(ITEMS) + COLUMNS - 1) // COLUMNS
    width = COLUMNS * (THUMB_SIZE[0] + PADDING) + PADDING
    height = rows * (THUMB_SIZE[1] + LABEL_HEIGHT + PADDING) + PADDING
    sheet = Image.new("RGB", (width, height), (8, 12, 20))
    draw = ImageDraw.Draw(sheet)
    font = load_font(20)

    for index, (label, filename) in enumerate(ITEMS):
        with Image.open(CAPTURE_DIR / filename) as source:
            thumb = source.convert("RGB").resize(THUMB_SIZE, Image.Resampling.LANCZOS)
        x = PADDING + (index % COLUMNS) * (THUMB_SIZE[0] + PADDING)
        y = PADDING + (index // COLUMNS) * (THUMB_SIZE[1] + LABEL_HEIGHT + PADDING)
        sheet.paste(thumb, (x, y))
        draw.rectangle(
            [x, y, x + THUMB_SIZE[0] - 1, y + THUMB_SIZE[1] - 1],
            outline=(214, 171, 83),
            width=2,
        )
        draw.text((x + 8, y + THUMB_SIZE[1] + 7), label, fill=(240, 224, 170), font=font)

    sheet.save(OUTPUT, optimize=True)
    print(f"Wrote {OUTPUT}")
    print(f"Contact sheet: {width}x{height}, {len(ITEMS)} captures, {OUTPUT.stat().st_size} bytes")


if __name__ == "__main__":
    main()

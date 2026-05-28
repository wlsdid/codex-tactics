from PIL import Image, ImageDraw
from pathlib import Path

OUT = Path(__file__).resolve().parent
SCALE = 3

# Compact SNES-like fantasy palettes with strong outline/readability.
INK = (18, 18, 28, 255)
INK2 = (33, 30, 45, 255)
SHADOW = (0, 0, 0, 80)
WHITE = (245, 244, 232, 255)
GOLD = (238, 178, 64, 255)
GOLD_L = (255, 222, 106, 255)
STEEL = (164, 198, 222, 255)
STEEL_L = (225, 242, 255, 255)
BLUE = (60, 110, 210, 255)
BLUE_L = (100, 170, 255, 255)
SKIN = (231, 178, 128, 255)
SKIN_L = (255, 210, 162, 255)
HAIR = (62, 46, 74, 255)
HAIR_L = (98, 74, 122, 255)
RED = (188, 50, 66, 255)
RED_L = (248, 90, 82, 255)
PURPLE = (116, 60, 172, 255)
PURPLE_L = (178, 90, 238, 255)
GREEN = (70, 154, 105, 255)
GREEN_L = (124, 222, 146, 255)


def rect(d, xy, c):
    d.rectangle(xy, fill=c)


def poly(d, pts, c):
    d.polygon(pts, fill=c)


def upscale(img):
    return img.resize((img.width * SCALE, img.height * SCALE), Image.Resampling.NEAREST)


def save(img, name):
    upscale(img).save(OUT / name)


def draw_eye(d, x, y, iris):
    rect(d, (x, y, x + 6, y + 7), WHITE)
    rect(d, (x + 2, y + 2, x + 5, y + 6), iris)
    rect(d, (x + 4, y + 2, x + 5, y + 3), WHITE)


def hero_main():
    img = Image.new("RGBA", (96, 128), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    # ground shadow / aura pixels
    d.ellipse((23, 112, 76, 124), fill=SHADOW)
    rect(d, (34, 97, 62, 111), INK)
    rect(d, (37, 84, 59, 103), BLUE)
    rect(d, (40, 86, 56, 96), BLUE_L)
    rect(d, (38, 88, 44, 108), STEEL)
    rect(d, (52, 88, 58, 108), STEEL)
    rect(d, (37, 106, 46, 116), INK)
    rect(d, (51, 106, 60, 116), INK)
    rect(d, (39, 106, 46, 112), STEEL_L)
    rect(d, (51, 106, 58, 112), STEEL)
    # cloak and sword silhouette
    poly(d, [(28, 82), (15, 108), (35, 102), (38, 86)], INK2)
    poly(d, [(30, 84), (22, 103), (36, 98)], BLUE)
    poly(d, [(62, 80), (81, 44), (86, 47), (67, 91)], INK)
    poly(d, [(66, 79), (81, 46), (83, 48), (69, 88)], STEEL_L)
    rect(d, (62, 77, 70, 83), GOLD)
    # oversized head outline
    d.ellipse((21, 28, 75, 82), fill=INK)
    d.ellipse((26, 32, 70, 76), fill=SKIN)
    rect(d, (30, 32, 66, 45), HAIR)
    poly(d, [(25, 38), (40, 24), (55, 31), (71, 38), (66, 47), (52, 39), (39, 47)], HAIR)
    rect(d, (35, 28, 45, 34), HAIR_L)
    rect(d, (55, 31, 64, 36), HAIR_L)
    draw_eye(d, 34, 51, BLUE)
    draw_eye(d, 56, 51, GREEN)
    rect(d, (45, 64, 53, 66), RED_L)
    rect(d, (29, 47, 33, 58), SKIN_L)
    rect(d, (65, 47, 68, 58), SKIN_L)
    # armor face highlights
    rect(d, (38, 78, 60, 84), GOLD)
    rect(d, (41, 79, 56, 81), GOLD_L)
    rect(d, (46, 83, 51, 97), GOLD_L)
    # outline sparkle pixels
    rect(d, (17, 56, 19, 58), BLUE_L)
    rect(d, (76, 31, 78, 33), GOLD_L)
    rect(d, (78, 69, 80, 71), BLUE_L)
    return img


def enemy_main():
    img = Image.new("RGBA", (104, 128), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((20, 111, 88, 124), fill=SHADOW)
    # tail / wings
    poly(d, [(20, 75), (5, 58), (17, 99), (31, 91)], INK)
    poly(d, [(84, 75), (101, 58), (87, 99), (73, 91)], INK)
    poly(d, [(20, 78), (10, 65), (19, 91), (29, 88)], PURPLE)
    poly(d, [(83, 78), (96, 65), (86, 91), (75, 88)], PURPLE)
    # body
    d.ellipse((31, 72, 75, 112), fill=INK)
    d.ellipse((36, 76, 70, 108), fill=PURPLE)
    rect(d, (44, 90, 61, 105), PURPLE_L)
    rect(d, (35, 101, 45, 114), INK)
    rect(d, (61, 101, 71, 114), INK)
    # head / horns
    d.ellipse((20, 27, 84, 87), fill=INK)
    d.ellipse((25, 32, 79, 82), fill=(104, 45, 132, 255))
    poly(d, [(18, 39), (8, 16), (30, 33)], INK)
    poly(d, [(86, 39), (98, 16), (75, 33)], INK)
    poly(d, [(18, 36), (11, 20), (28, 34)], GOLD)
    poly(d, [(86, 36), (94, 20), (76, 34)], GOLD)
    rect(d, (38, 25, 66, 33), INK)
    rect(d, (42, 22, 62, 30), GOLD)
    rect(d, (45, 20, 48, 25), GOLD_L)
    rect(d, (56, 20, 59, 25), GOLD_L)
    # face
    rect(d, (33, 50, 45, 59), RED_L)
    rect(d, (60, 50, 72, 59), RED_L)
    rect(d, (37, 52, 44, 56), WHITE)
    rect(d, (61, 52, 68, 56), WHITE)
    rect(d, (50, 65, 55, 67), INK)
    rect(d, (42, 73, 63, 76), INK)
    rect(d, (46, 76, 49, 80), WHITE)
    rect(d, (57, 76, 60, 80), WHITE)
    # armor/gems
    rect(d, (48, 86, 58, 96), RED)
    rect(d, (50, 88, 56, 92), RED_L)
    rect(d, (25, 48, 29, 58), PURPLE_L)
    rect(d, (76, 48, 80, 58), PURPLE_L)
    rect(d, (12, 80, 15, 84), RED_L)
    rect(d, (91, 80, 94, 84), RED_L)
    return img


def ally_variant():
    img = Image.new("RGBA", (72, 96), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((17, 84, 55, 93), fill=SHADOW)
    rect(d, (25, 63, 48, 82), INK)
    rect(d, (28, 65, 45, 78), GREEN)
    d.ellipse((15, 19, 57, 61), fill=INK)
    d.ellipse((20, 23, 52, 57), fill=SKIN)
    rect(d, (18, 20, 54, 34), (122, 74, 36, 255))
    rect(d, (25, 39, 31, 46), WHITE); rect(d, (42, 39, 48, 46), WHITE)
    rect(d, (27, 41, 30, 45), GREEN); rect(d, (43, 41, 46, 45), GREEN)
    rect(d, (31, 59, 41, 64), GOLD)
    poly(d, [(48, 65), (62, 42), (65, 44), (52, 75)], STEEL_L)
    return img


def enemy_variant():
    img = Image.new("RGBA", (72, 96), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((14, 84, 58, 93), fill=SHADOW)
    d.ellipse((20, 56, 52, 84), fill=INK)
    d.ellipse((23, 59, 49, 80), fill=RED)
    d.ellipse((10, 18, 62, 62), fill=INK)
    d.ellipse((15, 23, 57, 58), fill=(55, 82, 108, 255))
    poly(d, [(13, 30), (4, 16), (22, 25)], INK)
    poly(d, [(59, 30), (68, 16), (50, 25)], INK)
    rect(d, (24, 37, 32, 44), RED_L); rect(d, (41, 37, 49, 44), RED_L)
    rect(d, (30, 53, 43, 56), INK)
    rect(d, (34, 70, 39, 78), GOLD)
    return img


def main():
    save(hero_main(), "chibi_hero_original.png")
    save(enemy_main(), "chibi_enemy_original.png")
    save(ally_variant(), "chibi_ally_guardian.png")
    save(enemy_variant(), "chibi_enemy_raider.png")
    print("generated chibi pixel standees in", OUT)


if __name__ == "__main__":
    main()

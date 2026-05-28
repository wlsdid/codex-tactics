from PIL import Image, ImageDraw
from pathlib import Path

OUT = Path(__file__).resolve().parent
SCALE = 2  # higher native dot count, smaller final screen footprint than previous 3x assets

# Mature SNES/tactical-RPG palette: lower candy saturation, stronger metal/leather contrast.
INK = (12, 13, 20, 255)
INK2 = (28, 25, 36, 255)
SHADOW = (0, 0, 0, 82)
WHITE = (236, 234, 220, 255)
BONE = (206, 194, 166, 255)
GOLD = (190, 132, 45, 255)
GOLD_L = (242, 197, 87, 255)
STEEL_D = (64, 77, 94, 255)
STEEL = (126, 151, 174, 255)
STEEL_L = (213, 228, 236, 255)
BLUE_D = (31, 55, 92, 255)
BLUE = (52, 86, 145, 255)
BLUE_L = (94, 132, 190, 255)
SKIN_D = (154, 96, 70, 255)
SKIN = (205, 144, 103, 255)
SKIN_L = (236, 183, 132, 255)
HAIR = (45, 36, 54, 255)
HAIR_L = (83, 62, 96, 255)
RED_D = (100, 24, 35, 255)
RED = (162, 44, 52, 255)
RED_L = (229, 80, 70, 255)
PURPLE_D = (50, 31, 72, 255)
PURPLE = (89, 51, 118, 255)
PURPLE_L = (144, 79, 176, 255)
GREEN_D = (40, 80, 64, 255)
GREEN = (72, 130, 94, 255)
GREEN_L = (126, 190, 132, 255)
LEATHER = (94, 57, 38, 255)
LEATHER_L = (143, 91, 54, 255)


def rect(d, xy, c):
    d.rectangle(xy, fill=c)


def poly(d, pts, c):
    d.polygon(pts, fill=c)


def line(d, pts, c, width=1):
    d.line(pts, fill=c, width=width)


def upscale(img):
    return img.resize((img.width * SCALE, img.height * SCALE), Image.Resampling.NEAREST)


def save(img, name):
    upscale(img).save(OUT / name)


def eye_slit(d, x, y, iris):
    # Smaller, sharper eyes than the previous cute round-eye style.
    rect(d, (x, y, x + 8, y + 3), INK)
    rect(d, (x + 1, y + 1, x + 6, y + 2), WHITE)
    rect(d, (x + 4, y + 1, x + 6, y + 2), iris)


def hero_main():
    img = Image.new("RGBA", (128, 160), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((31, 143, 96, 155), fill=SHADOW)

    # Back cape and long sword: sharper silhouette, less plush-toy outline.
    poly(d, [(45, 67), (24, 116), (42, 137), (55, 101)], INK2)
    poly(d, [(47, 72), (32, 118), (44, 128), (55, 98)], BLUE_D)
    poly(d, [(89, 53), (111, 15), (116, 18), (95, 86)], INK)
    poly(d, [(92, 53), (111, 18), (113, 19), (96, 82)], STEEL_L)
    line(d, [(103, 34), (93, 71)], STEEL_D, 1)
    rect(d, (82, 78, 97, 84), GOLD)
    rect(d, (87, 71, 92, 91), LEATHER)

    # Longer tactical body/legs; head is still stylized but no longer toddler-proportioned.
    poly(d, [(50, 75), (79, 75), (89, 121), (40, 121)], INK)
    poly(d, [(54, 80), (75, 80), (82, 116), (47, 116)], BLUE)
    rect(d, (58, 82, 72, 116), STEEL_D)
    rect(d, (60, 84, 70, 101), STEEL)
    rect(d, (53, 92, 58, 121), STEEL)
    rect(d, (72, 92, 78, 121), STEEL)
    rect(d, (49, 117, 62, 141), INK)
    rect(d, (68, 117, 81, 141), INK)
    rect(d, (52, 119, 62, 134), STEEL_L)
    rect(d, (68, 119, 78, 134), STEEL)
    rect(d, (52, 138, 65, 144), INK2)
    rect(d, (68, 138, 84, 144), INK2)
    rect(d, (51, 86, 78, 91), GOLD)
    rect(d, (55, 87, 73, 88), GOLD_L)
    rect(d, (62, 91, 67, 116), GOLD_L)
    rect(d, (45, 83, 53, 111), BLUE_D)
    rect(d, (78, 83, 87, 111), BLUE_D)
    rect(d, (42, 108, 53, 116), STEEL_L)
    rect(d, (80, 108, 91, 116), STEEL)

    # Head with angular hair and slimmer face.
    d.ellipse((34, 23, 94, 78), fill=INK)
    d.ellipse((40, 30, 88, 75), fill=SKIN)
    rect(d, (42, 31, 86, 43), HAIR)
    poly(d, [(36, 40), (50, 20), (66, 29), (82, 24), (92, 39), (84, 48), (67, 40), (54, 50), (43, 46)], HAIR)
    rect(d, (49, 25, 59, 31), HAIR_L)
    rect(d, (72, 29, 83, 35), HAIR_L)
    rect(d, (39, 48, 44, 61), SKIN_D)
    rect(d, (84, 48, 88, 61), SKIN_D)
    eye_slit(d, 50, 53, BLUE_L)
    eye_slit(d, 72, 53, GREEN_L)
    rect(d, (63, 62, 67, 64), SKIN_D)
    rect(d, (59, 70, 72, 72), RED_D)
    rect(d, (45, 44, 83, 47), HAIR_L)
    rect(d, (46, 75, 82, 80), INK)
    rect(d, (55, 76, 74, 78), STEEL_L)

    # Small premium glints, kept controlled.
    rect(d, (25, 89, 27, 91), BLUE_L)
    rect(d, (101, 41, 103, 43), STEEL_L)
    rect(d, (78, 68, 80, 70), GOLD_L)
    return img


def enemy_main():
    img = Image.new("RGBA", (136, 160), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((28, 142, 108, 156), fill=SHADOW)

    # Wings and tail, angular rather than cute.
    poly(d, [(37, 72), (6, 47), (18, 119), (47, 105)], INK)
    poly(d, [(98, 72), (130, 47), (118, 119), (88, 105)], INK)
    poly(d, [(36, 76), (14, 56), (23, 103), (46, 96)], PURPLE_D)
    poly(d, [(99, 76), (121, 56), (112, 103), (89, 96)], PURPLE_D)
    line(d, [(19, 60), (42, 93)], PURPLE_L, 2)
    line(d, [(117, 60), (94, 93)], PURPLE_L, 2)
    poly(d, [(65, 113), (49, 139), (69, 129), (83, 145), (76, 117)], INK)
    poly(d, [(67, 116), (57, 133), (70, 126), (79, 137), (74, 119)], RED_D)

    # Taller armored demon body.
    d.ellipse((40, 78, 96, 128), fill=INK)
    d.ellipse((46, 84, 90, 122), fill=PURPLE)
    rect(d, (55, 93, 80, 119), PURPLE_L)
    rect(d, (60, 97, 75, 111), RED_D)
    rect(d, (62, 99, 73, 106), RED_L)
    rect(d, (42, 115, 55, 142), INK)
    rect(d, (80, 115, 93, 142), INK)
    rect(d, (45, 117, 55, 133), PURPLE_D)
    rect(d, (80, 117, 90, 133), PURPLE_D)
    rect(d, (38, 90, 47, 116), INK)
    rect(d, (90, 90, 99, 116), INK)
    rect(d, (33, 111, 47, 119), BONE)
    rect(d, (90, 111, 104, 119), BONE)

    # Crowned head: smaller relative to body, more menacing.
    d.ellipse((29, 20, 107, 88), fill=INK)
    d.ellipse((36, 29, 100, 84), fill=(82, 43, 110, 255))
    poly(d, [(30, 39), (16, 9), (47, 31)], INK)
    poly(d, [(106, 39), (121, 9), (89, 31)], INK)
    poly(d, [(31, 36), (20, 15), (46, 32)], GOLD)
    poly(d, [(105, 36), (116, 15), (90, 32)], GOLD)
    rect(d, (48, 24, 88, 33), INK)
    rect(d, (52, 20, 84, 30), GOLD)
    rect(d, (55, 16, 59, 22), GOLD_L)
    rect(d, (66, 14, 70, 22), GOLD_L)
    rect(d, (79, 16, 83, 22), GOLD_L)
    rect(d, (45, 48, 58, 55), RED_L)
    rect(d, (78, 48, 91, 55), RED_L)
    rect(d, (48, 50, 56, 53), WHITE)
    rect(d, (80, 50, 88, 53), WHITE)
    rect(d, (66, 61, 70, 64), INK)
    rect(d, (54, 73, 83, 77), INK)
    rect(d, (59, 77, 63, 83), BONE)
    rect(d, (74, 77, 78, 83), BONE)
    rect(d, (39, 50, 43, 64), PURPLE_L)
    rect(d, (94, 50, 98, 64), PURPLE_L)
    rect(d, (13, 86, 17, 91), RED_L)
    rect(d, (119, 86, 123, 91), RED_L)
    return img


def ally_variant():
    img = Image.new("RGBA", (104, 132), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((23, 118, 79, 129), fill=SHADOW)
    poly(d, [(41, 63), (66, 63), (75, 100), (32, 100)], INK)
    poly(d, [(45, 68), (62, 68), (68, 96), (38, 96)], GREEN_D)
    rect(d, (48, 70, 60, 96), STEEL_D)
    rect(d, (32, 92, 48, 116), INK)
    rect(d, (61, 92, 74, 116), INK)
    rect(d, (35, 96, 47, 111), STEEL_L)
    rect(d, (61, 96, 72, 111), STEEL)
    d.ellipse((26, 25, 80, 76), fill=INK)
    d.ellipse((33, 32, 74, 73), fill=SKIN)
    poly(d, [(29, 37), (45, 22), (59, 27), (78, 36), (70, 45), (55, 39), (44, 46)], LEATHER)
    rect(d, (36, 48, 44, 52), WHITE)
    rect(d, (60, 48, 68, 52), WHITE)
    rect(d, (40, 49, 43, 52), GREEN_L)
    rect(d, (63, 49, 66, 52), GREEN_L)
    rect(d, (48, 61, 59, 63), RED_D)
    rect(d, (43, 75, 65, 80), GOLD)
    # Guard shield as main identity.
    d.ellipse((68, 63, 95, 100), fill=INK)
    d.ellipse((72, 67, 91, 96), fill=STEEL)
    rect(d, (80, 68, 84, 96), GOLD)
    line(d, [(73, 82), (90, 82)], STEEL_L, 2)
    return img


def enemy_variant():
    img = Image.new("RGBA", (104, 132), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.ellipse((20, 118, 84, 129), fill=SHADOW)
    d.ellipse((34, 74, 72, 112), fill=INK)
    d.ellipse((39, 80, 67, 106), fill=RED_D)
    rect(d, (42, 94, 52, 119), INK)
    rect(d, (56, 94, 66, 119), INK)
    d.ellipse((20, 24, 84, 82), fill=INK)
    d.ellipse((28, 33, 76, 78), fill=(49, 72, 91, 255))
    poly(d, [(23, 37), (9, 16), (38, 29)], INK)
    poly(d, [(82, 37), (95, 16), (67, 29)], INK)
    rect(d, (35, 50, 45, 55), RED_L)
    rect(d, (60, 50, 70, 55), RED_L)
    rect(d, (44, 67, 61, 70), INK)
    rect(d, (49, 84, 58, 97), GOLD)
    # Raider axe silhouette.
    rect(d, (74, 56, 79, 107), LEATHER)
    poly(d, [(76, 50), (95, 56), (82, 67)], STEEL_L)
    poly(d, [(76, 50), (90, 45), (84, 61)], STEEL)
    return img


def main():
    save(hero_main(), "chibi_hero_original.png")
    save(enemy_main(), "chibi_enemy_original.png")
    save(ally_variant(), "chibi_ally_guardian.png")
    save(enemy_variant(), "chibi_enemy_raider.png")
    print("generated higher-density mature tactical pixel standees in", OUT)


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""
Generates high-contrast promotional assets for ECHO-ISM itch.io submission:
- logo.png (transparent background, horizontal)
- favicon.png (256x256)
- cover.png (630x500)
- wide_cover.png (1680x720, 21:9)
- social_preview.png (1280x720, 16:9)
"""

import math
from PIL import Image, ImageDraw, ImageFont

FONT_BOLD = "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf"
FONT_REGULAR = "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"
FONT_MONO = "/usr/share/fonts/truetype/liberation/LiberationMono-Bold.ttf"

# Color Palette (WCAG AAA compliant per Design System)
COLOR_BG_DARK = (9, 14, 26, 255)         # #090E1A
COLOR_BG_CARD = (14, 22, 38, 255)        # #0E1626
COLOR_CYAN = (0, 210, 255, 255)          # #00D2FF
COLOR_CYAN_DIM = (0, 140, 180, 255)
COLOR_WHITE = (248, 250, 252, 255)       # #F8FAFC
COLOR_SLATE = (148, 163, 184, 255)       # #94A3B8
COLOR_EMERALD = (16, 185, 129, 255)      # #10B981
COLOR_GOLD = (245, 158, 11, 255)         # #F59E0B
COLOR_VIOLET = (124, 58, 237, 255)       # #7C3AED
COLOR_BORDER = (51, 65, 85, 255)         # #334155

def draw_blob(draw, center, radius, fill_color, outline_color, eye_color=(11, 19, 43, 255), is_echo=False):
    cx, cy = center
    r = radius
    if is_echo:
        # Echo aura ring
        draw.ellipse([cx - r * 1.35, cy - r * 1.35, cx + r * 1.35, cy + r * 1.35], outline=outline_color, width=2)
    
    # Body
    draw.ellipse([cx - r, cy - r * 0.95, cx + r, cy + r * 0.95], fill=fill_color, outline=outline_color, width=3)
    
    # Eyes
    eye_offset_x = r * 0.28
    eye_offset_y = -r * 0.1
    eye_r = max(2, int(r * 0.18))
    # Left eye
    draw.ellipse([cx - eye_offset_x - eye_r, cy + eye_offset_y - eye_r, cx - eye_offset_x + eye_r, cy + eye_offset_y + eye_r], fill=eye_color)
    # Right eye
    draw.ellipse([cx + eye_offset_x - eye_r, cy + eye_offset_y - eye_r, cx + eye_offset_x + eye_r, cy + eye_offset_y + eye_r], fill=eye_color)
    # Specular highlights
    spec_r = max(1, int(eye_r * 0.35))
    draw.ellipse([cx - eye_offset_x - spec_r * 0.5, cy + eye_offset_y - spec_r * 0.8, cx - eye_offset_x + spec_r * 0.5, cy + eye_offset_y + spec_r * 0.2], fill=(255, 255, 255, 255))
    draw.ellipse([cx + eye_offset_x + spec_r * 0.5, cy + eye_offset_y - spec_r * 0.8, cx + eye_offset_x + spec_r * 1.5, cy + eye_offset_y + spec_r * 0.2], fill=(255, 255, 255, 255))

def generate_logo():
    # 800 x 220 transparent PNG
    img = Image.new("RGBA", (800, 220), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Left Icon: Greg + Echo
    draw_blob(draw, (85, 110), 45, (0, 210, 255, 120), (0, 210, 255, 180), is_echo=True)
    draw_blob(draw, (115, 110), 45, COLOR_CYAN, (8, 27, 51, 255), is_echo=False)

    # Title: ECHO-ISM
    font_title = ImageFont.truetype(FONT_BOLD, 72)
    font_sub = ImageFont.truetype(FONT_MONO, 16)

    # Text shadow
    draw.text((203, 53), "ECHO-ISM", font=font_title, fill=(0, 50, 80, 180))
    # Main text
    draw.text((200, 50), "ECHO-ISM", font=font_title, fill=COLOR_WHITE)

    # Subtitle badge line
    draw.line([(200, 142), (720, 142)], fill=COLOR_CYAN, width=2)
    draw.text((200, 155), "TEMPORAL CONSENSUS PROTOCOL", font=font_sub, fill=COLOR_CYAN)

    img.save("Promotional/logo.png")
    img.save("Builds/Promotional/logo.png")
    print("Generated logo.png")

def generate_favicon():
    # 256 x 256 square
    img = Image.new("RGBA", (256, 256), COLOR_BG_DARK)
    draw = ImageDraw.Draw(img)

    # Rounded boundary
    draw.rounded_rectangle([8, 8, 248, 248], radius=32, outline=COLOR_BORDER, width=4)
    draw.rounded_rectangle([14, 14, 242, 242], radius=28, fill=COLOR_BG_CARD)

    # Echo behind
    draw_blob(draw, (110, 128), 65, (0, 210, 255, 100), (0, 210, 255, 160), is_echo=True)
    # Greg in front
    draw_blob(draw, (146, 128), 65, COLOR_CYAN, (8, 27, 51, 255), is_echo=False)

    img.save("Promotional/favicon.png")
    img.save("Builds/Promotional/favicon.png")
    print("Generated favicon.png")

def generate_cover():
    # 630 x 500 itch.io cover
    img = Image.new("RGBA", (630, 500), COLOR_BG_DARK)
    draw = ImageDraw.Draw(img)

    # Border & circuit traces
    draw.rectangle([12, 12, 618, 488], outline=COLOR_BORDER, width=3)
    draw.line([(40, 320), (200, 320), (240, 360), (590, 360)], fill=COLOR_CYAN_DIM, width=2)
    draw.line([(80, 360), (140, 360)], fill=COLOR_CYAN, width=3)

    # Blobs in game scene
    # Live Greg
    draw_blob(draw, (460, 360), 38, COLOR_CYAN, (8, 27, 51, 255), is_echo=False)
    # Echo 1
    draw_blob(draw, (320, 360), 34, (0, 210, 255, 130), (0, 210, 255, 200), is_echo=True)
    # Echo 2
    draw_blob(draw, (180, 320), 30, (0, 210, 255, 80), (0, 210, 255, 150), is_echo=True)

    # Pressure plate & gate
    draw.rectangle([165, 310, 195, 330], outline=COLOR_GOLD, width=2)
    draw.rectangle([305, 345, 335, 375], outline=COLOR_EMERALD, width=2)

    # Header / Title Block
    font_title = ImageFont.truetype(FONT_BOLD, 54)
    font_sub = ImageFont.truetype(FONT_MONO, 14)
    font_desc = ImageFont.truetype(FONT_REGULAR, 15)
    font_badge = ImageFont.truetype(FONT_BOLD, 12)

    draw.text((42, 48), "ECHO-ISM", font=font_title, fill=COLOR_WHITE)
    draw.text((42, 116), "THE LAST SIGNAL // COOPERATE WITH YOUR PAST", font=font_sub, fill=COLOR_CYAN)

    draw.text((42, 150), "A puzzle adventure about restoring truth to NEXUS.", font=font_desc, fill=COLOR_SLATE)
    draw.text((42, 174), "Record loops, coordinate past echoes, unseal verified data.", font=font_desc, fill=COLOR_SLATE)

    # Feature badges
    badges = ["5 STORY SECTORS", "RECORD & REPLAY", "NARRATIVE ARCHIVE", "WCAG AAA"]
    bx = 42
    for b in badges:
        bw = len(b) * 8 + 18
        draw.rounded_rectangle([bx, 215, bx + bw, 240], radius=4, fill=COLOR_BG_CARD, outline=COLOR_BORDER, width=1)
        draw.text((bx + 9, 222), b, font=font_badge, fill=COLOR_CYAN)
        bx += bw + 10

    img.save("Promotional/cover.png")
    img.save("Builds/Promotional/cover.png")
    print("Generated cover.png")

def generate_social_preview():
    # 1280 x 720 (16:9)
    img = Image.new("RGBA", (1280, 720), COLOR_BG_DARK)
    draw = ImageDraw.Draw(img)

    # Chamber frame
    draw.rectangle([24, 24, 1256, 696], outline=COLOR_BORDER, width=4)
    draw.line([(80, 520), (450, 520), (520, 580), (1200, 580)], fill=COLOR_CYAN_DIM, width=3)
    draw.line([(200, 420), (600, 420), (650, 520)], fill=COLOR_CYAN_DIM, width=2)

    # NPC Silhouettes & Greg
    # Pip
    draw_blob(draw, (320, 520), 40, (16, 185, 129, 255), (6, 78, 59, 255))
    # Bolt
    draw_blob(draw, (550, 580), 45, (249, 115, 22, 255), (124, 45, 18, 255))
    # Mira
    draw_blob(draw, (780, 580), 42, (250, 204, 21, 255), (113, 63, 18, 255))
    # Live Greg & Echoes
    draw_blob(draw, (980, 580), 50, COLOR_CYAN, (8, 27, 51, 255), is_echo=False)
    draw_blob(draw, (900, 580), 46, (0, 210, 255, 120), (0, 210, 255, 180), is_echo=True)
    # NULL in background
    draw_blob(draw, (1120, 460), 44, (124, 58, 237, 255), (46, 16, 101, 255))

    # Title area
    font_title = ImageFont.truetype(FONT_BOLD, 84)
    font_sub = ImageFont.truetype(FONT_MONO, 20)
    font_desc = ImageFont.truetype(FONT_REGULAR, 22)
    font_badge = ImageFont.truetype(FONT_BOLD, 15)

    draw.text((70, 75), "ECHO-ISM", font=font_title, fill=COLOR_WHITE)
    draw.text((70, 180), "THE LAST SIGNAL // TEMPORAL CONSENSUS PROTOCOL", font=font_sub, fill=COLOR_CYAN)

    draw.text((70, 230), "When a distorted broadcast divides a digital civilization,", font=font_desc, fill=COLOR_WHITE)
    draw.text((70, 265), "cooperate across time loops to verify the truth.", font=font_desc, fill=COLOR_SLATE)

    # Feature Badges
    badges = ["5 HANDCRAFTED SECTORS", "MULTIPLE ECHOES", "NARRATIVE MYSTERY", "ZERO SLOP DESIGN"]
    bx = 70
    for b in badges:
        bw = len(b) * 10 + 24
        draw.rounded_rectangle([bx, 330, bx + bw, 365], radius=6, fill=COLOR_BG_CARD, outline=COLOR_BORDER, width=2)
        draw.text((bx + 12, 339), b, font=font_badge, fill=COLOR_CYAN)
        bx += bw + 16

    img.save("Promotional/social_preview.png")
    img.save("Builds/Promotional/social_preview.png")
    print("Generated social_preview.png")

def generate_wide_cover():
    # 1680 x 720 (21:9)
    img = Image.new("RGBA", (1680, 720), COLOR_BG_DARK)
    draw = ImageDraw.Draw(img)

    draw.rectangle([20, 20, 1660, 700], outline=COLOR_BORDER, width=4)

    # Digital wave / network lines across width
    for step in range(0, 1680, 40):
        y1 = 500 + int(math.sin(step * 0.01) * 60)
        y2 = 500 + int(math.sin((step + 40) * 0.01) * 60)
        draw.line([(step, y1), (step + 40, y2)], fill=COLOR_CYAN_DIM, width=2)

    # Characters traversing the network
    draw_blob(draw, (380, 520), 40, (16, 185, 129, 255), (6, 78, 59, 255))
    draw_blob(draw, (620, 480), 45, (249, 115, 22, 255), (124, 45, 18, 255))
    draw_blob(draw, (860, 540), 42, (250, 204, 21, 255), (113, 63, 18, 255))
    draw_blob(draw, (1100, 480), 52, COLOR_CYAN, (8, 27, 51, 255), is_echo=False)
    draw_blob(draw, (1020, 480), 48, (0, 210, 255, 120), (0, 210, 255, 180), is_echo=True)
    draw_blob(draw, (1400, 520), 46, (124, 58, 237, 255), (46, 16, 101, 255))

    font_title = ImageFont.truetype(FONT_BOLD, 92)
    font_sub = ImageFont.truetype(FONT_MONO, 22)
    font_desc = ImageFont.truetype(FONT_REGULAR, 24)

    draw.text((80, 90), "ECHO-ISM", font=font_title, fill=COLOR_WHITE)
    draw.text((80, 205), "THE LAST SIGNAL // A TEMPORAL RESILIENCE ADVENTURE", font=font_sub, fill=COLOR_CYAN)
    draw.text((80, 255), "Coordinate past loops. Unseal the archive. Restore truth to the network.", font=font_desc, fill=COLOR_SLATE)

    img.save("Promotional/wide_cover.png")
    img.save("Builds/Promotional/wide_cover.png")
    print("Generated wide_cover.png")

if __name__ == "__main__":
    generate_logo()
    generate_favicon()
    generate_cover()
    generate_social_preview()
    generate_wide_cover()
    print("All promotional assets generated successfully.")

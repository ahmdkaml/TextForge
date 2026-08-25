import io
import os
from PIL import Image
import cairosvg

def generate_assets():
    # Use the full-bleed transparent mark to fill the titlebar space
    source_svg = "icon-window.svg"

    # Destination output directories
    icons_dir = os.path.join("..", "icons")
    branding_dir = "."  # Current folder

    os.makedirs(icons_dir, exist_ok=True)

    if not os.path.exists(source_svg):
        raise FileNotFoundError(f"Could not find {source_svg} in current folder.")

    # 1. Rasterize master SVG to high-res base PNG in memory (1024x1024)
    png_data = cairosvg.svg2png(url=source_svg, output_width=1024, output_height=1024)
    master_image = Image.open(io.BytesIO(png_data)).convert("RGBA")

    # 2. Generate standalone PNGs
    png_targets = {
        os.path.join(branding_dir, "icon.png"): 256,
        os.path.join(icons_dir, "icon-192.png"): 192,
        os.path.join(icons_dir, "icon-512.png"): 512,
    }

    for path, size in png_targets.items():
        resized = master_image.resize((size, size), Image.Resampling.LANCZOS)
        resized.save(path, format="PNG")
        print(f"Generated: {path} ({size}x{size})")

    # 3. Generate Windows multi-resolution .ico files
    ico_sizes = [(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)]

    app_ico_path = os.path.join(icons_dir, "app-icon.ico")
    master_image.save(app_ico_path, format="ICO", sizes=ico_sizes)
    print(f"Generated: {app_ico_path}")

    fav_ico_path = os.path.join(icons_dir, "favicon.ico")
    master_image.save(fav_ico_path, format="ICO", sizes=[(16, 16), (32, 32), (48, 48)])
    print(f"Generated: {fav_ico_path}")

if __name__ == "__main__":
    generate_assets()

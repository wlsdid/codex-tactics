using UnityEngine;

/// <summary>
/// Generates placeholder colored sprite textures at runtime.
/// No external assets needed — creates textures procedurally.
/// </summary>
public static class PlaceholderSpriteGenerator
{
    /// <summary>
    /// Creates a simple sprite from a solid color with an outline.
    /// </summary>
    /// <param name="color">Fill color</param>
    /// <param name="size">Width and height in pixels (power of 2 recommended for legacy, but Unity handles non-PoT2)</param>
    /// <param name="outlineColor">Outline color (null = no outline)</param>
    /// <param name="outlineWidth">Outline thickness in pixels</param>
    public static Sprite CreateSprite(Color color, int size, Color? outlineColor = null, int outlineWidth = 2)
    {
        int texSize = Mathf.NextPowerOfTwo(size);
        var tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
        
        Color outline = outlineColor ?? Color.clear;
        bool hasOutline = outlineColor.HasValue;

        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                // Circle test: is the pixel inside the circle?
                float dx = (x - texSize / 2f) / (texSize / 2f);
                float dy = (y - texSize / 2f) / (texSize / 2f);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist > 1.0f)
                {
                    tex.SetPixel(x, y, Color.clear);
                }
                else if (hasOutline && dist > 1.0f - (outlineWidth * 2f / texSize))
                {
                    tex.SetPixel(x, y, outline);
                }
                else
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }

        tex.Apply();

        Rect rect = new Rect(0, 0, size, size);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        return Sprite.Create(tex, rect, pivot, 100f);
    }

    /// <summary>
    /// Creates a simple rounded rectangle sprite.
    /// </summary>
    public static Sprite CreateRoundedRect(Color color, int width, int height, int cornerRadius = 8)
    {
        int w = Mathf.NextPowerOfTwo(width);
        int h = Mathf.NextPowerOfTwo(height);
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool inside = IsInsideRoundedRect(x, y, w, h, cornerRadius);
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
        }

        tex.Apply();
        Rect rect = new Rect(0, 0, width, height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        return Sprite.Create(tex, rect, pivot, 100f);
    }

    /// <summary>
    /// Creates a hero-styled sprite (larger circle with accent border).
    /// </summary>
    public static Sprite CreateHeroSprite()
    {
        return CreateSprite(new Color(0.26f, 0.56f, 1.0f, 1f), 128, new Color(0.50f, 0.80f, 1.0f, 1f), 4);
    }

    /// <summary>
    /// Creates an enemy-styled sprite (red-tinted).
    /// </summary>
    public static Sprite CreateEnemySprite(bool isBoss = false)
    {
        int size = isBoss ? 160 : 128;
        Color fill = isBoss ? new Color(0.82f, 0.22f, 0.24f) : new Color(0.60f, 0.35f, 0.80f);
        Color outline = isBoss ? new Color(1.0f, 0.50f, 0.20f) : new Color(0.80f, 0.50f, 1.0f);
        return CreateSprite(fill, size, outline, isBoss ? 6 : 4);
    }

    private static bool IsInsideRoundedRect(int x, int y, int w, int h, int r)
    {
        // Check corner circles
        float cx = x - w / 2f;
        float cy = y - h / 2f;
        float hw = w / 2f - r;
        float hh = h / 2f - r;

        if (Mathf.Abs(cx) <= hw || Mathf.Abs(cy) <= hh)
            return true;

        // Corner circle test
        float dx = Mathf.Abs(cx) - hw;
        float dy = Mathf.Abs(cy) - hh;
        return dx * dx + dy * dy <= r * r;
    }
}

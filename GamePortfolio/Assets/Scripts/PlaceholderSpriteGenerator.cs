using UnityEngine;

/// <summary>
/// Generates procedural placeholder sprites for heroes, enemies, and UI icons.
/// Each element type gets a distinct color palette and silhouette.
/// </summary>
public static class PlaceholderSpriteGenerator
{
    private const int SpriteSize = 128;

    // ── Sprite cache ──
    private static Sprite cachedHeroSprite;
    private static readonly System.Collections.Generic.Dictionary<(ElementType, bool), Sprite> cachedEnemySprites = new();

    // ── Hero identity ──
    public static string HeroName => "Kaelen";
    public static string HeroTitle => "Crystal Tactician";
    public static string HeroFlavor => "A wandering tactician wielding ancient elemental crystals.";

    // ── Enemy identity per stage ──
    public static readonly string[] StageEnemyNames =
    {
        "Slime",           // Stage 1
        "Wolf",            // Stage 2
        "Golem",           // Stage 3
        "Storm Hawk",      // Stage 4
        "Shadow Wraith",   // Stage 5
        "Light Warden"     // Stage 6
    };

    public static readonly string[] StageBossNames =
    {
        "Slime King",
        "Alpha Wolf",
        "Ancient Golem",
        "Thunder Phoenix",
        "Shadow Lord",
        "Holy Sentinel"
    };

    public static readonly string[] StageEnemyFlavor =
    {
        "A pulsating blob of primordial ooze. Weak to Fire.",
        "A cunning predator with sharp instincts. Weak to Nature.",
        "A towering stone construct from ancient times. Weak to Earth.",
        "A majestic bird commanding the skies. Weak to Lightning.",
        "A void-touched spirit of darkness. Weak to Dark.",
        "A radiant being of pure light. Weak to Light."
    };

    public static readonly string[] StageBossFlavor =
    {
        "The ruler of slimes, swollen with power. Its royal slam crushes all.",
        "The pack leader, scarred from countless battles. Its howl signals death.",
        "A primordial titan of bedrock and fury. Cataclysm awaits the foolish.",
        "Legendary phoenix of the storm. Its skyfall brings oblivion.",
        "The void incarnate. Oblivion Strike erases all hope.",
        "The final sentinel of divine order. Heavenly Wrath judges all."
    };

    // ── Sprite generation ──

    /// <summary>Create a hero sprite with crystalline armor + sword motif.</summary>
    public static Sprite CreateHeroSprite()
    {
        if (cachedHeroSprite != null) return cachedHeroSprite;
        Texture2D tex = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
        Color clear = Color.clear;

        // Fill transparent
        for (int y = 0; y < SpriteSize; y++)
            for (int x = 0; x < SpriteSize; x++)
                tex.SetPixel(x, y, clear);

        Color primary = new Color(0.15f, 0.55f, 0.85f);   // Blue crystal
        Color accent = new Color(0.60f, 0.85f, 1.0f);     // Light blue glow
        Color crystal = new Color(0.35f, 0.70f, 1.0f);     // Crystal highlight

        // Body (armored torso)
        DrawRect(tex, 38, 35, 52, 65, primary);
        DrawRect(tex, 40, 37, 48, 63, accent);

        // Head
        DrawCircle(tex, 64, 22, 16, new Color(0.85f, 0.80f, 0.72f)); // Skin tone
        // Hair
        DrawRect(tex, 48, 6, 80, 16, new Color(0.20f, 0.25f, 0.35f)); // Dark hair

        // Crystal shoulder pads
        DrawTriangle(tex, 30, 40, 38, 55, 22, 40, crystal);
        DrawTriangle(tex, 90, 40, 98, 55, 106, 40, crystal);

        // Sword (right side)
        DrawRect(tex, 92, 60, 96, 100, new Color(0.80f, 0.80f, 0.85f)); // Blade
        DrawRect(tex, 90, 94, 98, 100, new Color(0.50f, 0.35f, 0.15f)); // Handle
        DrawCircle(tex, 94, 92, 4, new Color(0.70f, 0.20f, 0.20f));    // Gem

        // Legs
        DrawRect(tex, 42, 90, 54, 120, new Color(0.15f, 0.20f, 0.30f));
        DrawRect(tex, 56, 90, 68, 120, new Color(0.15f, 0.20f, 0.30f));

        // Boots
        DrawRect(tex, 40, 114, 56, 126, new Color(0.25f, 0.15f, 0.10f));
        DrawRect(tex, 58, 114, 74, 126, new Color(0.25f, 0.15f, 0.10f));

        tex.Apply();
        cachedHeroSprite = Sprite.Create(tex, new Rect(0, 0, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), 100f);
        return cachedHeroSprite;
    }

    /// <summary>Create an enemy sprite based on element type and boss flag.</summary>
    public static Sprite CreateEnemySprite(ElementType element, bool isBoss = false)
    {
        var key = (element, isBoss);
        if (cachedEnemySprites.TryGetValue(key, out Sprite cached)) return cached;
        Texture2D tex = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
        Color clear = Color.clear;
        for (int y = 0; y < SpriteSize; y++)
            for (int x = 0; x < SpriteSize; x++)
                tex.SetPixel(x, y, clear);

        Color bodyColor = GetElementColor(element);
        if (isBoss) bodyColor = Color.Lerp(bodyColor, Color.white, 0.2f);
        Color accentColor = GetElementAccent(element);
        int sizeMod = isBoss ? 2 : 0; // Bosses are slightly bigger

        switch (element)
        {
            case ElementType.Fire:
                DrawSlime(tex, bodyColor, accentColor, sizeMod);
                break;
            case ElementType.Nature:
                DrawWolf(tex, bodyColor, accentColor, sizeMod);
                break;
            case ElementType.Earth:
                DrawGolem(tex, bodyColor, accentColor, sizeMod);
                break;
            case ElementType.Lightning:
                DrawBird(tex, bodyColor, accentColor, sizeMod);
                break;
            case ElementType.Dark:
                DrawWraith(tex, bodyColor, accentColor, sizeMod);
                break;
            case ElementType.Light:
                DrawAngel(tex, bodyColor, accentColor, sizeMod);
                break;
            default:
                DrawSlime(tex, bodyColor, accentColor, sizeMod);
                break;
        }

        tex.Apply();
        var result = Sprite.Create(tex, new Rect(0, 0, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), 100f);
        cachedEnemySprites[key] = result;
        return result;
    }

    /// <summary>Overload for backward compatibility (defaults to Fire slime).</summary>
    public static Sprite CreateEnemySprite(bool isBoss = false)
    {
        return CreateEnemySprite(ElementType.Fire, isBoss);
    }

    /// <summary>Create a small element icon sprite for UI buttons.</summary>
    public static Sprite CreateElementIcon(ElementType element, int size = 32)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = Color.clear;

        int cx = size / 2;
        int cy = size / 2;
        int r = size / 2 - 2;

        // Transparent bg
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, clear);

        Color body = GetElementColor(element);
        Color accent = GetElementAccent(element);

        // Circle
        DrawCircle(tex, cx, cy, r, body);
        DrawCircle(tex, cx, cy, r - 2, accent);

        // Element symbol
        switch (element)
        {
            case ElementType.Fire:
                // Flame arrow
                DrawTriangle(tex, cx - 4, cy + 4, cx + 4, cy + 4, cx, cy - 6, body);
                break;
            case ElementType.Nature:
                // Leaf = diamond
                DrawRect(tex, cx - 3, cy - 3, cx + 3, cy + 3, body);
                break;
            case ElementType.Earth:
                // Rock = square
                DrawRect(tex, cx - 4, cy - 4, cx + 4, cy + 4, body);
                break;
            case ElementType.Lightning:
                // Lightning bolt shape
                tex.SetPixel(cx, cy - 5, body);
                tex.SetPixel(cx - 1, cy - 4, body);
                tex.SetPixel(cx, cy - 3, body);
                tex.SetPixel(cx + 1, cy - 2, body);
                tex.SetPixel(cx, cy - 1, body);
                tex.SetPixel(cx - 1, cy, body);
                tex.SetPixel(cx, cy + 1, body);
                tex.SetPixel(cx + 1, cy + 2, body);
                tex.SetPixel(cx, cy + 3, body);
                tex.SetPixel(cx, cy + 4, body);
                break;
            case ElementType.Dark:
                // Crescent
                DrawCircle(tex, cx + 2, cy, r - 3, new Color(0.08f, 0.08f, 0.12f));
                break;
            case ElementType.Light:
                // Star cross
                DrawRect(tex, cx - 1, cy - 5, cx + 1, cy + 5, body);
                DrawRect(tex, cx - 5, cy - 1, cx + 5, cy + 1, body);
                break;
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    // ── Color palettes ──

    public static Color GetElementColor(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => new Color(0.85f, 0.25f, 0.10f),       // Red-orange
            ElementType.Nature => new Color(0.20f, 0.65f, 0.30f),     // Forest green
            ElementType.Earth => new Color(0.55f, 0.40f, 0.25f),      // Brown
            ElementType.Lightning => new Color(0.90f, 0.75f, 0.10f),  // Yellow-gold
            ElementType.Dark => new Color(0.30f, 0.12f, 0.40f),       // Purple
            ElementType.Light => new Color(0.85f, 0.85f, 0.70f),      // Warm white
            _ => new Color(0.50f, 0.50f, 0.50f)                       // Gray
        };
    }

    public static Color GetElementAccent(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => new Color(1.0f, 0.60f, 0.10f),        // Orange
            ElementType.Nature => new Color(0.35f, 0.80f, 0.40f),     // Light green
            ElementType.Earth => new Color(0.70f, 0.55f, 0.35f),      // Tan
            ElementType.Lightning => new Color(1.0f, 0.90f, 0.40f),    // Bright yellow
            ElementType.Dark => new Color(0.50f, 0.20f, 0.55f),       // Magenta-purple
            ElementType.Light => new Color(1.0f, 0.95f, 0.80f),       // Bright white
            _ => new Color(0.70f, 0.70f, 0.70f)
        };
    }

    public static Color GetElementTextColor(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => new Color(1.0f, 0.40f, 0.20f),
            ElementType.Nature => new Color(0.30f, 0.75f, 0.35f),
            ElementType.Earth => new Color(0.65f, 0.50f, 0.30f),
            ElementType.Lightning => new Color(1.0f, 0.85f, 0.20f),
            ElementType.Dark => new Color(0.60f, 0.30f, 0.70f),
            ElementType.Light => new Color(0.90f, 0.90f, 0.75f),
            _ => Color.white
        };
    }

    // ── Drawing primitives ──

    private static void DrawRect(Texture2D tex, int x1, int y1, int x2, int y2, Color color)
    {
        x1 = Mathf.Clamp(x1, 0, SpriteSize - 1);
        y1 = Mathf.Clamp(y1, 0, SpriteSize - 1);
        x2 = Mathf.Clamp(x2, x1, SpriteSize - 1);
        y2 = Mathf.Clamp(y2, y1, SpriteSize - 1);

        for (int y = y1; y <= y2; y++)
            for (int x = x1; x <= x2; x++)
                tex.SetPixel(x, y, color);
    }

    private static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color color)
    {
        for (int y = -r; y <= r; y++)
        {
            for (int x = -r; x <= r; x++)
            {
                if (x * x + y * y <= r * r)
                {
                    int px = Mathf.Clamp(cx + x, 0, SpriteSize - 1);
                    int py = Mathf.Clamp(cy + y, 0, SpriteSize - 1);
                    tex.SetPixel(px, py, color);
                }
            }
        }
    }

    private static void DrawTriangle(Texture2D tex, int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
        // Simple triangle rasterization via bounding box + barycentric
        int minX = Mathf.Max(0, Mathf.Min(x1, Mathf.Min(x2, x3)));
        int maxX = Mathf.Min(SpriteSize - 1, Mathf.Max(x1, Mathf.Max(x2, x3)));
        int minY = Mathf.Max(0, Mathf.Min(y1, Mathf.Min(y2, y3)));
        int maxY = Mathf.Min(SpriteSize - 1, Mathf.Max(y1, Mathf.Max(y2, y3)));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float denom = (y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3);
                float w1 = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / denom;
                float w2 = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / denom;
                float w3 = 1 - w1 - w2;
                if (w1 >= 0 && w2 >= 0 && w3 >= 0)
                    tex.SetPixel(x, y, color);
            }
        }
    }

    // ── Enemy shape generators ──

    private static void DrawSlime(Texture2D tex, Color body, Color accent, int sizeMod)
    {
        // Round blob body
        DrawCircle(tex, 64, 70 + sizeMod, 30 + sizeMod, body);
        DrawCircle(tex, 64, 70 + sizeMod, 26 + sizeMod, accent);
        // Eyes
        DrawCircle(tex, 54, 60 + sizeMod, 5, Color.white);
        DrawCircle(tex, 74, 60 + sizeMod, 5, Color.white);
        DrawCircle(tex, 54, 60 + sizeMod, 2, new Color(0.15f, 0.15f, 0.15f));
        DrawCircle(tex, 74, 60 + sizeMod, 2, new Color(0.15f, 0.15f, 0.15f));
        // Crown for boss
        if (sizeMod > 0)
        {
            DrawTriangle(tex, 50, 32, 56, 22, 62, 32, new Color(0.90f, 0.75f, 0.25f));
            DrawTriangle(tex, 60, 28, 66, 18, 72, 28, new Color(0.90f, 0.75f, 0.25f));
            DrawTriangle(tex, 70, 32, 76, 22, 82, 32, new Color(0.90f, 0.75f, 0.25f));
        }
    }

    private static void DrawWolf(Texture2D tex, Color body, Color accent, int sizeMod)
    {
        // Body
        DrawRect(tex, 34, 50 + sizeMod, 80, 90 + sizeMod, body);
        DrawRect(tex, 36, 52 + sizeMod, 78, 88 + sizeMod, accent);
        // Head
        DrawCircle(tex, 90, 40 + sizeMod, 20, body);
        DrawCircle(tex, 90, 40 + sizeMod, 17, accent);
        // Ears
        DrawTriangle(tex, 78, 18, 82, 10, 88, 20, body);
        DrawTriangle(tex, 94, 18, 98, 10, 104, 20, body);
        // Eyes
        DrawCircle(tex, 96, 36, 4, new Color(0.95f, 0.85f, 0.20f));
        DrawCircle(tex, 96, 36, 2, new Color(0.10f, 0.10f, 0.10f));
        // Legs
        DrawRect(tex, 36, 88 + sizeMod, 48, 120 + sizeMod, new Color(0.18f, 0.15f, 0.12f));
        DrawRect(tex, 52, 88 + sizeMod, 64, 120 + sizeMod, new Color(0.18f, 0.15f, 0.12f));
        // Tail
        DrawRect(tex, 28, 56 + sizeMod, 34, 78 + sizeMod, body);
    }

    private static void DrawGolem(Texture2D tex, Color body, Color accent, int sizeMod)
    {
        // Massive rectangular body
        DrawRect(tex, 30, 40 + sizeMod, 98, 90 + sizeMod, body);
        DrawRect(tex, 32, 42 + sizeMod, 96, 88 + sizeMod, accent);
        // Head (square)
        DrawRect(tex, 42, 18 + sizeMod, 82, 44 + sizeMod, body);
        DrawRect(tex, 44, 20 + sizeMod, 80, 42 + sizeMod, accent);
        // Eyes (glowing)
        DrawRect(tex, 52, 26, 58, 32, new Color(0.95f, 0.70f, 0.10f));
        DrawRect(tex, 66, 26, 72, 32, new Color(0.95f, 0.70f, 0.10f));
        // Arms
        DrawRect(tex, 16, 50 + sizeMod, 30, 80 + sizeMod, body);
        DrawRect(tex, 98, 50 + sizeMod, 112, 80 + sizeMod, body);
        // Legs (sturdy pillars)
        DrawRect(tex, 38, 90 + sizeMod, 56, 124 + sizeMod, new Color(0.30f, 0.20f, 0.12f));
        DrawRect(tex, 66, 90 + sizeMod, 84, 124 + sizeMod, new Color(0.30f, 0.20f, 0.12f));
        // Boss rune on chest
        if (sizeMod > 0)
        {
            DrawCircle(tex, 64, 68, 8, new Color(0.40f, 0.80f, 1.0f));
            DrawCircle(tex, 64, 68, 5, new Color(0.60f, 0.90f, 1.0f));
        }
    }

    private static void DrawBird(Texture2D tex, Color body, Color accent, int sizeMod)
    {
        // Body (oval)
        DrawCircle(tex, 64, 60 + sizeMod, 24, body);
        DrawCircle(tex, 64, 60 + sizeMod, 20, accent);
        // Head
        DrawCircle(tex, 64, 30 + sizeMod, 16, body);
        DrawCircle(tex, 64, 30 + sizeMod, 13, accent);
        // Beak
        DrawTriangle(tex, 78, 26, 98, 32, 78, 38, new Color(0.95f, 0.80f, 0.20f));
        // Eye
        DrawCircle(tex, 70, 28, 4, new Color(0.10f, 0.10f, 0.10f));
        DrawCircle(tex, 70, 28, 2, Color.white);
        // Wings (spread)
        DrawTriangle(tex, 24, 44, 46, 52, 38, 72, body);
        DrawTriangle(tex, 104, 44, 82, 52, 90, 72, body);
        // Tail feathers
        DrawTriangle(tex, 46, 80, 64, 110, 82, 80, body);
        // Boss lightning crown
        if (sizeMod > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                int bx = 48 + i * 8;
                DrawTriangle(tex, bx - 2, 12, bx, 2, bx + 2, 12, new Color(1.0f, 0.90f, 0.20f));
            }
        }
    }

    private static void DrawWraith(Texture2D tex, Color body, Color accent, int sizeMod)
    {
        // Ghostly hooded figure
        // Body (flowing robe)
        DrawCircle(tex, 64, 70 + sizeMod, 26, body);
        DrawCircle(tex, 64, 70 + sizeMod, 22, accent);
        // Hood
        DrawCircle(tex, 64, 38 + sizeMod, 18, body);
        DrawCircle(tex, 64, 38 + sizeMod, 15, accent);
        // Glowing eyes
        DrawCircle(tex, 56, 34, 4, new Color(0.60f, 0.30f, 0.70f));
        DrawCircle(tex, 72, 34, 4, new Color(0.60f, 0.30f, 0.70f));
        DrawCircle(tex, 56, 34, 2, new Color(0.80f, 0.50f, 1.0f));
        DrawCircle(tex, 72, 34, 2, new Color(0.80f, 0.50f, 1.0f));
        // Cloak bottom (triangular flow)
        DrawTriangle(tex, 38, 80 + sizeMod, 64, 120 + sizeMod, 90, 80 + sizeMod, body);
        // Wispy arms
        DrawRect(tex, 28, 56 + sizeMod, 40, 70 + sizeMod, accent);
        DrawRect(tex, 88, 56 + sizeMod, 100, 70 + sizeMod, accent);
        // Boss extra glow
        if (sizeMod > 0)
        {
            DrawCircle(tex, 64, 70, 30, new Color(0.40f, 0.15f, 0.50f, 0.30f));
        }
    }

    private static void DrawAngel(Texture2D tex, Color body, Color accent, int sizeMod)
    {
        // Radiant angelic figure
        // Halo
        DrawCircle(tex, 64, 14, 12, new Color(1.0f, 0.95f, 0.60f, 0.50f));
        DrawCircle(tex, 64, 14, 10, new Color(1.0f, 0.95f, 0.60f, 0.70f));
        // Head
        DrawCircle(tex, 64, 30 + sizeMod, 14, new Color(0.85f, 0.82f, 0.75f));
        // Body (robe)
        DrawCircle(tex, 64, 60 + sizeMod, 24, body);
        DrawCircle(tex, 64, 60 + sizeMod, 20, accent);
        // Wings
        DrawTriangle(tex, 20, 40 + sizeMod, 42, 50 + sizeMod, 30, 80 + sizeMod, Color.white * 0.85f);
        DrawTriangle(tex, 108, 40 + sizeMod, 86, 50 + sizeMod, 98, 80 + sizeMod, Color.white * 0.85f);
        // Eyes (serene)
        DrawCircle(tex, 58, 28, 3, new Color(0.20f, 0.30f, 0.50f));
        DrawCircle(tex, 70, 28, 3, new Color(0.20f, 0.30f, 0.50f));
        // Boss extra radiance
        if (sizeMod > 0)
        {
            DrawCircle(tex, 64, 60, 30, new Color(0.90f, 0.90f, 0.70f, 0.20f));
            DrawCircle(tex, 64, 60, 34, new Color(0.90f, 0.90f, 0.70f, 0.12f));
        }
    }
}
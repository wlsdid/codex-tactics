using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// Keeps generated portfolio UI on the project-owned Noto Sans KR TMP asset.
/// Dynamic population preserves Korean, English, and numeric UI text while the
/// default TMP asset remains a readable fallback for any missing glyph.
/// </summary>
public static class TacticalTypography
{
    public const string FontPath = "Assets/Fonts/NotoSansKR-Regular.otf";
    public const string FontAssetPath = "Assets/Fonts/NotoSansKR-Regular SDF.asset";
    private const string CoverageSample = "전술 레퀴엠 전투 선택 영웅 공격 방어 Tactical Requiem Battle Select Hero Attack Guard 0123456789";

    public static TMP_FontAsset EnsureFontAsset()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (fontAsset == null)
        {
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
            if (sourceFont == null)
            {
                Debug.LogError($"[Typography] Missing source font: {FontPath}");
                return null;
            }

            // Path-backed creation survives editor reloads more reliably than a transient Font reference.
            string absoluteFontPath = System.IO.Path.Combine(Application.dataPath, "Fonts/NotoSansKR-Regular.otf");
            fontAsset = TMP_FontAsset.CreateFontAsset(
                absoluteFontPath, 0, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024);
            if (fontAsset == null)
            {
                Debug.LogError($"[Typography] TMP asset creation failed for: {FontPath}");
                return null;
            }
            fontAsset.name = "NotoSansKR-Regular SDF";
            AssetDatabase.CreateAsset(fontAsset, FontAssetPath);
            PersistGeneratedSubAsset(fontAsset.material, fontAsset);
            if (fontAsset.atlasTextures != null)
            {
                foreach (Texture2D atlas in fontAsset.atlasTextures)
                    PersistGeneratedSubAsset(atlas, fontAsset);
            }
            AssetDatabase.SaveAssets();
        }

        fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
        fontAsset.isMultiAtlasTexturesEnabled = true;
        SerializedObject serializedFont = new SerializedObject(fontAsset);
        SerializedProperty clearOnBuild = serializedFont.FindProperty("m_ClearDynamicDataOnBuild");
        if (clearOnBuild != null)
        {
            clearOnBuild.boolValue = false;
            serializedFont.ApplyModifiedPropertiesWithoutUndo();
        }

        TMP_FontAsset fallback = TMP_Settings.defaultFontAsset;
        if (fallback != null && fallback != fontAsset)
        {
            if (fontAsset.fallbackFontAssetTable == null)
                fontAsset.fallbackFontAssetTable = new List<TMP_FontAsset>();
            if (!fontAsset.fallbackFontAssetTable.Contains(fallback))
                fontAsset.fallbackFontAssetTable.Add(fallback);
        }

        fontAsset.TryAddCharacters(CoverageSample, out string missing);
        if (!string.IsNullOrEmpty(missing))
            Debug.LogWarning($"[Typography] Dynamic glyph fallback required for: {missing}");
        EditorUtility.SetDirty(fontAsset);
        return fontAsset;
    }

    private static void PersistGeneratedSubAsset(Object asset, TMP_FontAsset parent)
    {
        if (asset != null && AssetDatabase.GetAssetPath(asset) != FontAssetPath)
            AssetDatabase.AddObjectToAsset(asset, parent);
    }

    public static void ApplyToLoadedScene()
    {
        TMP_FontAsset fontAsset = EnsureFontAsset();
        if (fontAsset == null)
            return;

        TMP_Text[] textObjects = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_Text text in textObjects)
        {
            if (text != null)
                text.font = fontAsset;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        Debug.Log($"[Typography] Applied Noto Sans KR to {textObjects.Length} TMP texts in {activeScene.name}.");
    }

    public static bool HasRequiredCoverage(TMP_FontAsset fontAsset)
    {
        return fontAsset != null && fontAsset.HasCharacters(CoverageSample, out _);
    }

    [MenuItem("Tools/Tactical Requiem/Validate Noto Sans KR Typography")]
    public static void ValidateNotoCoverage()
    {
        TMP_FontAsset fontAsset = EnsureFontAsset();
        string addMissing = string.Empty;
        var coverageMissing = new List<char>();
        bool added = fontAsset != null && fontAsset.TryAddCharacters(CoverageSample, out addMissing);
        bool covered = fontAsset != null && fontAsset.HasCharacters(CoverageSample, out coverageMissing);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Typography] Coverage Korean + English + digits: {(covered ? "PASS" : "FAIL")} | DirectAdd={(added ? "PASS" : "fallback") } | AddMissing={addMissing} | CoverageMissing={new string(coverageMissing.ToArray())}");
    }

    [MenuItem("Tools/Tactical Requiem/Apply Noto Sans KR Typography")]
    public static void ApplyTypographyMenu()
    {
        ApplyToLoadedScene();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
    }
}

using System.IO;
using BannoyasGames.CargoExit.Presentation;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;

namespace BannoyasGames.CargoExit.Editor
{
    public static class CargoExitFontApplier
    {
        private const string MainScenePath =
            "Assets/CargoExit/Scenes/CargoExit.unity";
        private const string FontFolder =
            "Assets/CargoExit/Art/Fonts/AtkinsonHyperlegibleNext";
        private const string RegularFontPath =
            FontFolder + "/AtkinsonHyperlegibleNext-Regular.ttf";
        private const string BoldFontPath =
            FontFolder + "/AtkinsonHyperlegibleNext-Bold.ttf";
        private const string RegularFontAssetPath =
            FontFolder + "/AtkinsonHyperlegibleNext-Regular SDF.asset";
        private const string BoldFontAssetPath =
            FontFolder + "/AtkinsonHyperlegibleNext-Bold SDF.asset";
        private const string RequiredCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
            "ÁÉÍÓÚÜÑáéíóúüñ¡!¿?0123456789 ×/-'.,:";

        [MenuItem("Cargo Exit/Apply Project Fonts", priority = 10)]
        public static void ApplyToMainScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.path != MainScenePath)
            {
                if (!Application.isBatchMode &&
                    !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }

                scene = EditorSceneManager.OpenScene(MainScenePath);
            }

            var controller =
                Object.FindFirstObjectByType<CargoExitPrototypeController>();
            if (controller == null)
            {
                throw new MissingReferenceException(
                    "CargoExit.unity has no CargoExitPrototypeController.");
            }

            var (regular, bold) = EnsureFontAssets();
            controller.ConfigureFonts(regular, bold);
            EditorUtility.SetDirty(controller);

            var updated = 0;
            foreach (var text in controller.GetComponentsInChildren<TMP_Text>(true))
            {
                var wantsBold =
                    text.font == bold ||
                    (text.fontStyle & FontStyles.Bold) != 0;
                text.font = wantsBold ? bold : regular;
                text.fontStyle &= ~FontStyles.Bold;
                EditorUtility.SetDirty(text);
                updated++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log(
                $"Applied Atkinson Hyperlegible Next SDF to {updated} text elements.");
        }

        internal static void Configure(CargoExitPrototypeController controller)
        {
            var (regular, bold) = EnsureFontAssets();
            controller.ConfigureFonts(regular, bold);
        }

        private static (TMP_FontAsset regular, TMP_FontAsset bold)
            EnsureFontAssets()
        {
            if (TMP_Settings.instance == null)
            {
                throw new MissingReferenceException(
                    "Cargo Exit requires the local TMP Essential Resources.");
            }

            var regular = LoadOrCreateFontAsset(
                RegularFontPath,
                RegularFontAssetPath);
            var bold = LoadOrCreateFontAsset(
                BoldFontPath,
                BoldFontAssetPath);
            AssetDatabase.SaveAssets();
            return (regular, bold);
        }

        private static TMP_FontAsset LoadOrCreateFontAsset(
            string sourcePath,
            string assetPath)
        {
            var existing =
                AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (existing != null)
            {
                AddRequiredCharacters(existing);
                return existing;
            }

            var sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourcePath);
            if (sourceFont == null)
            {
                throw new MissingReferenceException(
                    $"Cargo Exit source font is missing: {sourcePath}");
            }

            var fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic,
                true);
            if (fontAsset == null)
            {
                throw new MissingReferenceException(
                    $"Could not create a TMP font asset from {sourcePath}");
            }

            fontAsset.name = Path.GetFileNameWithoutExtension(assetPath);
            fontAsset.atlasTextures[0].name =
                fontAsset.name + " Atlas";
            fontAsset.material.name =
                fontAsset.name + " Material";

            AssetDatabase.CreateAsset(fontAsset, assetPath);
            AssetDatabase.AddObjectToAsset(
                fontAsset.atlasTextures[0],
                fontAsset);
            AssetDatabase.AddObjectToAsset(
                fontAsset.material,
                fontAsset);

            AddRequiredCharacters(fontAsset);
            return fontAsset;
        }

        private static void AddRequiredCharacters(TMP_FontAsset fontAsset)
        {
            if (!fontAsset.TryAddCharacters(
                    RequiredCharacters,
                    out var missingCharacters))
            {
                Debug.LogWarning(
                    $"Missing characters in {fontAsset.name}: " +
                    missingCharacters);
            }

            EditorUtility.SetDirty(fontAsset);
            EditorUtility.SetDirty(fontAsset.atlasTextures[0]);
            EditorUtility.SetDirty(fontAsset.material);
        }
    }
}

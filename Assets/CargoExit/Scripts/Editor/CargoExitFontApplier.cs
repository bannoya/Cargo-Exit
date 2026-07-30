using BannoyasGames.CargoExit.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

            var (regular, bold) = LoadFonts();
            controller.ConfigureFonts(regular, bold);
            EditorUtility.SetDirty(controller);

            var updated = 0;
            foreach (var text in controller.GetComponentsInChildren<Text>(true))
            {
                var wantsBold =
                    text.font == bold ||
                    text.fontStyle == FontStyle.Bold ||
                    text.fontStyle == FontStyle.BoldAndItalic;
                text.font = wantsBold ? bold : regular;
                text.fontStyle = text.fontStyle switch
                {
                    FontStyle.Bold => FontStyle.Normal,
                    FontStyle.BoldAndItalic => FontStyle.Italic,
                    _ => text.fontStyle
                };
                EditorUtility.SetDirty(text);
                updated++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log(
                $"Applied Atkinson Hyperlegible Next to {updated} text elements.");
        }

        internal static void Configure(CargoExitPrototypeController controller)
        {
            var (regular, bold) = LoadFonts();
            controller.ConfigureFonts(regular, bold);
        }

        private static (Font regular, Font bold) LoadFonts()
        {
            var regular = AssetDatabase.LoadAssetAtPath<Font>(RegularFontPath);
            var bold = AssetDatabase.LoadAssetAtPath<Font>(BoldFontPath);
            if (regular == null || bold == null)
            {
                throw new MissingReferenceException(
                    "Cargo Exit fonts are missing from Art/Fonts.");
            }

            return (regular, bold);
        }
    }
}

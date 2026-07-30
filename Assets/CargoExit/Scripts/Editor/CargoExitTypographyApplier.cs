using BannoyasGames.CargoExit.Presentation;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BannoyasGames.CargoExit.Editor
{
    public static class CargoExitTypographyApplier
    {
        private const string MainScenePath =
            "Assets/CargoExit/Scenes/CargoExit.unity";

        [MenuItem("Cargo Exit/Apply Project Typography", priority = 11)]
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

            var updated = 0;
            foreach (var label in controller.GetComponentsInChildren<TMP_Text>(true))
            {
                var size = CargoExitTypography.RecommendedSize(label);
                label.fontSize = size;
                label.enableAutoSizing = false;
                label.fontSizeMin = size;
                label.fontSizeMax = size;
                EnsureLabelHeight(label.rectTransform, size);
                EditorUtility.SetDirty(label);
                updated++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log(
                $"Applied mobile typography sizes to {updated} text elements.");
        }

        private static void EnsureLabelHeight(
            RectTransform rectTransform,
            float fontSize)
        {
            var minimumHeight = Mathf.Ceil(fontSize * 1.25f);
            if (rectTransform.sizeDelta.y >= minimumHeight)
            {
                return;
            }

            rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                minimumHeight);
            EditorUtility.SetDirty(rectTransform);
        }
    }
}

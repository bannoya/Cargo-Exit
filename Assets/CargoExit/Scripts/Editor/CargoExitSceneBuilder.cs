using BannoyasGames.CargoExit.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BannoyasGames.CargoExit.Editor
{
    public static class CargoExitSceneBuilder
    {
        private const string ScenePath = "Assets/CargoExit/Scenes/CargoExit.unity";

        [MenuItem("Cargo Exit/Open Main Scene", priority = 0)]
        public static void Open()
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (sceneAsset == null)
            {
                Build();
                return;
            }

            EditorSceneManager.OpenScene(ScenePath);
            SelectGameUi();
        }

        [MenuItem("Cargo Exit/Rebuild Main Scene", priority = 20)]
        public static void Build()
        {
            EnsureSceneFolder();
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject("Cargo Exit Prototype");
            var controller = root.AddComponent<CargoExitPrototypeController>();
            CargoExitFontApplier.Configure(controller);
            controller.BuildScenePreview();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            AssetDatabase.SaveAssets();
            SelectGameUi();
            Debug.Log(
                $"Cargo Exit main scene created at {ScenePath}. " +
                "It is visible in the Game view without entering Play Mode.");
        }

        private static void SelectGameUi()
        {
            var gameUi = GameObject.Find("Game UI");
            if (gameUi == null)
            {
                return;
            }

            Selection.activeGameObject = gameUi;
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        private static void EnsureSceneFolder()
        {
            const string cargoExitFolder = "Assets/CargoExit";
            const string sceneFolder = cargoExitFolder + "/Scenes";
            if (!AssetDatabase.IsValidFolder(sceneFolder))
            {
                AssetDatabase.CreateFolder(cargoExitFolder, "Scenes");
            }
        }
    }
}

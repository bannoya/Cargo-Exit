using System.IO;
using BannoyasGames.CargoExit.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BannoyasGames.CargoExit.Editor
{
    public static class PrototypeSceneBuilder
    {
        private const string SceneFolder = "Assets/CargoExit/Scenes";
        private const string ScenePath = SceneFolder + "/Prototype.unity";
        private const string PullSortScenePath = SceneFolder + "/PullSortPrototype.unity";
        private const string WarehouseSortScenePath =
            SceneFolder + "/WarehouseSortPrototype.unity";

        [MenuItem("Cargo Exit/Rebuild Prototype Scene")]
        public static void Build()
        {
            Directory.CreateDirectory(SceneFolder);
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject("Cargo Exit Prototype");
            root.AddComponent<PrototypeGameController>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log($"Cargo Exit prototype scene created at {ScenePath}");
        }

        [MenuItem("Cargo Exit/Rebuild Pull Sort Scene")]
        public static void BuildPullSort()
        {
            Directory.CreateDirectory(SceneFolder);
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject("Cargo Exit Pull Sort Prototype");
            root.AddComponent<PullSortPrototypeController>();

            EditorSceneManager.SaveScene(scene, PullSortScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(PullSortScenePath, true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log($"Cargo Exit pull-sort scene created at {PullSortScenePath}");
        }

        [MenuItem("Cargo Exit/Rebuild Warehouse Sort Scene")]
        public static void BuildWarehouseSort()
        {
            Directory.CreateDirectory(SceneFolder);
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject("Cargo Exit Warehouse Sort Prototype");
            root.AddComponent<WarehouseSortPrototypeController>();

            EditorSceneManager.SaveScene(scene, WarehouseSortScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(WarehouseSortScenePath, true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"Cargo Exit warehouse-sort scene created at {WarehouseSortScenePath}");
        }
    }
}

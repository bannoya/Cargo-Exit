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
    }
}


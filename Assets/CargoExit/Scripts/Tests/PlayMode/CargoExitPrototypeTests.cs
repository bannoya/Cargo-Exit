using System.Collections;
using BannoyasGames.CargoExit.Presentation;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.PlayMode.Tests
{
    public sealed class CargoExitPrototypeTests
    {
        [UnityTest]
        public IEnumerator MainSceneStartsWithPlayableSortingInterface()
        {
            yield return SceneManager.LoadSceneAsync(
                "CargoExit",
                LoadSceneMode.Single);
            yield return null;

            Assert.That(
                Object.FindFirstObjectByType<CargoExitPrototypeController>(),
                Is.Not.Null);
            Assert.That(GameObject.Find("Game UI"), Is.Not.Null);
            Assert.That(GameObject.Find("Unsorted Boxes"), Is.Not.Null);
            Assert.That(GameObject.Find("Pallet A"), Is.Not.Null);
            Assert.That(GameObject.Find("Pallet G"), Is.Not.Null);
            Assert.That(
                Object.FindObjectsByType<CargoParcelView>(
                    FindObjectsSortMode.None).Length,
                Is.EqualTo(14));
            Assert.That(
                GameObject.Find("Pallet A").GetComponent<Image>(),
                Is.Not.Null);
            var labels = Object.FindObjectsByType<TMP_Text>(
                FindObjectsSortMode.None);
            Assert.That(labels, Is.Not.Empty);
            foreach (var label in labels)
            {
                Assert.That(
                    label.font,
                    Is.Not.Null,
                    $"{label.name} has no readable font assigned.");
                StringAssert.Contains(
                    "Atkinson",
                    label.font.name,
                    $"{label.name} is not using the project font.");
                StringAssert.Contains(
                    "TextMeshPro",
                    label.fontSharedMaterial.shader.name,
                    $"{label.name} is not using an SDF text shader.");
                Assert.That(
                    label.fontSize,
                    Is.GreaterThanOrEqualTo(
                        CargoExitTypography.MinimumReadable),
                    $"{label.name} is too small for the mobile canvas.");
            }

            Assert.That(
                Object.FindObjectsByType<UnityEngine.UI.Text>(
                    FindObjectsSortMode.None),
                Is.Empty,
                "The rebuilt UI must not use legacy Unity Text.");
            var canvas = GameObject.Find("Game UI").GetComponent<Canvas>();
            Assert.That(canvas.pixelPerfect, Is.True);
            Assert.That(
                GameObject.Find("Game UI")
                    .GetComponent<CanvasScaler>()
                    .referenceResolution,
                Is.EqualTo(new Vector2(540f, 960f)));

            Assert.That(Camera.main, Is.Not.Null);
        }
    }
}

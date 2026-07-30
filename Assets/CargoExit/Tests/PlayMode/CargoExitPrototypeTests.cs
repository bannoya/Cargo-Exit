using System.Collections;
using BannoyasGames.CargoExit.Presentation;
using NUnit.Framework;
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
            Assert.That(Camera.main, Is.Not.Null);
        }
    }
}

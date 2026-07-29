using System.Collections;
using BannoyasGames.CargoExit.Presentation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.PlayMode.Tests
{
    public sealed class PrototypeSmokeTests
    {
        [UnityTest]
        public IEnumerator PrototypeCreatesPlayableInterface()
        {
            var root = new GameObject("Prototype Test");
            root.AddComponent<PrototypeGameController>();
            yield return null;

            Assert.That(Object.FindFirstObjectByType<Canvas>(), Is.Not.Null);
            Assert.That(
                GameObject.Find("CargoBoard"),
                Is.Not.Null,
                "The cargo board should be created.");
            Assert.That(
                GameObject.Find("Deliver").GetComponent<Button>(),
                Is.Not.Null,
                "The delivery action should be available.");
            Assert.That(
                Object.FindObjectsByType<ParcelView>(FindObjectsSortMode.None).Length,
                Is.EqualTo(3));

            Object.Destroy(root);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PullSortPrototypeCreatesLargeGestureTargets()
        {
            var root = new GameObject("Pull Sort Prototype Test");
            root.AddComponent<PullSortPrototypeController>();
            yield return null;

            Assert.That(GameObject.Find("TruckInterior"), Is.Not.Null);
            Assert.That(GameObject.Find("ExitGate"), Is.Not.Null);
            Assert.That(GameObject.Find("Dock A"), Is.Not.Null);
            Assert.That(GameObject.Find("Dock B"), Is.Not.Null);
            Assert.That(
                Object.FindObjectsByType<PullSortParcelView>(FindObjectsSortMode.None).Length,
                Is.EqualTo(6));

            Object.Destroy(root);
            yield return null;
        }
    }
}

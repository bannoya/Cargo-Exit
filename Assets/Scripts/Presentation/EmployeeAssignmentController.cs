using System.Collections;
using System.Collections.Generic;
using BannoyasGames.CargoExit.Application;
using BannoyasGames.CargoExit.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class EmployeeAssignmentController : MonoBehaviour
    {
        private const string StrongEmployeeId = "employee-strength";
        private const string CarefulEmployeeId = "employee-care";

        [SerializeField] private Canvas canvas;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private Text warningLabel;
        [SerializeField] private Button correctButton;
        [SerializeField] private Button confirmAnywayButton;
        [SerializeField] private CargoEmployeeView strongEmployee;
        [SerializeField] private CargoEmployeeView carefulEmployee;
        [SerializeField] private RectTransform heavyStationSurface;
        [SerializeField] private RectTransform fragileStationSurface;
        [SerializeField] private RectTransform heavyEmployeeSlot;
        [SerializeField] private RectTransform fragileEmployeeSlot;
        [SerializeField] private Image heavyStationBackground;
        [SerializeField] private Image fragileStationBackground;

        private WorkAssignmentSession assignmentSession;
        private Color heavyStationColor;
        private Color fragileStationColor;

        public Transform CanvasTransform => canvas.transform;

        private void Awake()
        {
            assignmentSession = new WorkAssignmentSession(new[]
            {
                new WorkEmployee(StrongEmployeeId, EmployeeSkill.Strength),
                new WorkEmployee(CarefulEmployeeId, EmployeeSkill.Care)
            });

            strongEmployee.Initialize(
                this,
                StrongEmployeeId,
                ((RectTransform)strongEmployee.transform).anchoredPosition);
            carefulEmployee.Initialize(
                this,
                CarefulEmployeeId,
                ((RectTransform)carefulEmployee.transform).anchoredPosition);

            heavyStationColor = heavyStationBackground.color;
            fragileStationColor = fragileStationBackground.color;
            confirmButton.interactable = false;
            warningPanel.SetActive(false);
            confirmButton.onClick.AddListener(TryConfirmAssignment);
            correctButton.onClick.AddListener(HideWarning);
            confirmAnywayButton.onClick.AddListener(ConfirmImperfectAssignment);
            ResetStationColors();
        }

        public void BeginEmployeeAssignment(CargoEmployeeView employee)
        {
            employee.RectTransform.localScale = Vector3.one * 1.07f;
            statusLabel.text = "ELEGÍ UNA ESTACIÓN";
            ResetStationColors();
        }

        public void ContinueEmployeeAssignment(
            CargoEmployeeView employee,
            Vector3 worldTarget)
        {
            var canvasRect = (RectTransform)canvas.transform;
            var local = (Vector2)canvasRect.InverseTransformPoint(worldTarget);
            var heavyCenter = (Vector2)canvasRect.InverseTransformPoint(
                heavyEmployeeSlot.position);
            var fragileCenter = (Vector2)canvasRect.InverseTransformPoint(
                fragileEmployeeSlot.position);
            var heavyDistance = Vector2.Distance(local, heavyCenter);
            var fragileDistance = Vector2.Distance(local, fragileCenter);
            var targetStation = heavyDistance <= fragileDistance
                ? WorkStationType.HeavyCargo
                : WorkStationType.FragileCargo;
            var stationCenter = targetStation == WorkStationType.HeavyCargo
                ? heavyCenter
                : fragileCenter;
            var distance = Mathf.Min(heavyDistance, fragileDistance);

            if (distance < 135f)
            {
                var attraction = 1f - Mathf.Clamp01(distance / 135f);
                attraction = attraction * attraction * 0.38f;
                local = Vector2.Lerp(local, stationCenter, attraction);
                HighlightStation(targetStation);
            }
            else
            {
                ResetStationColors();
            }

            employee.RectTransform.anchoredPosition = local;
            employee.RectTransform.localEulerAngles = new Vector3(
                0f,
                0f,
                Mathf.LerpAngle(
                    employee.RectTransform.localEulerAngles.z,
                    0f,
                    0.18f));
        }

        public void EndEmployeeAssignment(
            CargoEmployeeView employee,
            Vector2 pointerPosition)
        {
            var station = StationAt(pointerPosition);
            if (station.HasValue &&
                assignmentSession.TryAssign(employee.EmployeeId, station.Value))
            {
                statusLabel.text = assignmentSession.IsComplete
                    ? "ASIGNACIÓN LISTA PARA CONFIRMAR"
                    : "FALTA OCUPAR UNA ESTACIÓN";
                confirmButton.interactable = assignmentSession.IsComplete;
                MoveEmployeesToCurrentAssignments(0.23f);
                ResetStationColors();
                return;
            }

            statusLabel.text = "SOLTÁ EL EMPLEADO EN UNA ESTACIÓN";
            MoveEmployeeToCurrentAssignment(employee, 0.3f);
            ResetStationColors();
        }

        private WorkStationType? StationAt(Vector2 pointerPosition)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    heavyStationSurface,
                    pointerPosition))
            {
                return WorkStationType.HeavyCargo;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(
                    fragileStationSurface,
                    pointerPosition))
            {
                return WorkStationType.FragileCargo;
            }

            return null;
        }

        private void TryConfirmAssignment()
        {
            if (!assignmentSession.IsComplete)
            {
                return;
            }

            if (!assignmentSession.HasImperfectAssignment)
            {
                ConfirmAssignmentAndContinue();
                return;
            }

            var warnings = new List<string>();
            if (!assignmentSession.IsCompatible(WorkStationType.HeavyCargo))
            {
                warnings.Add("La estación pesada trabajará con menor eficiencia.");
            }

            if (!assignmentSession.IsCompatible(WorkStationType.FragileCargo))
            {
                warnings.Add("La carga frágil puede dañarse.");
            }

            warningLabel.text = string.Join("\n\n", warnings);
            warningPanel.SetActive(true);
        }

        private void HideWarning()
        {
            warningPanel.SetActive(false);
            statusLabel.text = "PODÉS CAMBIAR LA ASIGNACIÓN";
        }

        private void ConfirmImperfectAssignment()
        {
            warningPanel.SetActive(false);
            ConfirmAssignmentAndContinue();
        }

        private void ConfirmAssignmentAndContinue()
        {
            if (!assignmentSession.Confirm())
            {
                return;
            }

            assignmentSession.TryGetStationFor(
                StrongEmployeeId,
                out var strongStation);
            assignmentSession.TryGetStationFor(
                CarefulEmployeeId,
                out var carefulStation);
            assignmentSession.TryGetEmployeeAt(
                WorkStationType.HeavyCargo,
                out var heavyEmployeeId);
            assignmentSession.TryGetEmployeeAt(
                WorkStationType.FragileCargo,
                out var fragileEmployeeId);

            var flow = CargoSessionFlow.Instance;
            flow.State.ConfirmAssignment(
                strongStation,
                carefulStation,
                heavyEmployeeId,
                fragileEmployeeId,
                assignmentSession.GetAssignedSkill(WorkStationType.HeavyCargo),
                assignmentSession.GetAssignedSkill(WorkStationType.FragileCargo));
            strongEmployee.SetInteraction(false);
            carefulEmployee.SetInteraction(false);
            flow.GoToCargoProcessing();
        }

        private void MoveEmployeesToCurrentAssignments(float duration)
        {
            MoveEmployeeToCurrentAssignment(strongEmployee, duration);
            MoveEmployeeToCurrentAssignment(carefulEmployee, duration);
        }

        private void MoveEmployeeToCurrentAssignment(
            CargoEmployeeView employee,
            float duration)
        {
            if (assignmentSession.TryGetStationFor(
                    employee.EmployeeId,
                    out var station))
            {
                StartCoroutine(MoveEmployee(
                    employee,
                    station == WorkStationType.HeavyCargo
                        ? heavyEmployeeSlot
                        : fragileEmployeeSlot,
                    Vector2.zero,
                    Vector3.one * 0.86f,
                    duration));
                return;
            }

            StartCoroutine(MoveEmployee(
                employee,
                employee.HomeParent,
                employee.HomePosition,
                Vector3.one,
                duration));
        }

        private IEnumerator MoveEmployee(
            CargoEmployeeView employee,
            Transform targetParent,
            Vector2 targetAnchoredPosition,
            Vector3 targetScale,
            float duration)
        {
            employee.SetInteraction(false);
            var startPosition = employee.RectTransform.position;
            var startScale = employee.RectTransform.localScale;
            var targetPosition = targetParent.TransformPoint(targetAnchoredPosition);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                employee.RectTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    eased);
                employee.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    targetScale,
                    eased);
                employee.RectTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    Mathf.LerpAngle(
                        employee.RectTransform.localEulerAngles.z,
                        0f,
                        eased));
                yield return null;
            }

            employee.PlaceAt(targetParent, targetAnchoredPosition, targetScale);
            employee.SetInteraction(!assignmentSession.IsConfirmed);
        }

        private void HighlightStation(WorkStationType station)
        {
            var heavy = heavyStationColor;
            var fragile = fragileStationColor;
            heavy.a = station == WorkStationType.HeavyCargo ? 0.62f : 0.18f;
            fragile.a = station == WorkStationType.FragileCargo ? 0.62f : 0.18f;
            heavyStationBackground.color = heavy;
            fragileStationBackground.color = fragile;
        }

        private void ResetStationColors()
        {
            var heavy = heavyStationColor;
            var fragile = fragileStationColor;
            heavy.a = 0.3f;
            fragile.a = 0.3f;
            heavyStationBackground.color = heavy;
            fragileStationBackground.color = fragile;
        }
    }
}

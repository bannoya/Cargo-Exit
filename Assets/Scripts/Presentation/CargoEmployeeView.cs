using UnityEngine;
using UnityEngine.EventSystems;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class CargoEmployeeView :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private CargoExitPrototypeController controller;
        private EmployeeAssignmentController assignmentController;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Transform homeParent;
        private Vector2 homePosition;
        private Vector3 pointerOffset;

        public string EmployeeId { get; private set; }

        public RectTransform RectTransform => rectTransform;

        public Transform HomeParent => homeParent;

        public Vector2 HomePosition => homePosition;

        public void Initialize(
            CargoExitPrototypeController owner,
            string employeeId,
            Vector2 position)
        {
            controller = owner;
            assignmentController = null;
            Initialize(employeeId, position);
        }

        public void Initialize(
            EmployeeAssignmentController owner,
            string employeeId,
            Vector2 position)
        {
            controller = null;
            assignmentController = owner;
            Initialize(employeeId, position);
        }

        private void Initialize(string employeeId, Vector2 position)
        {
            EmployeeId = employeeId;
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            homeParent = rectTransform.parent;
            homePosition = position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            pointerOffset = rectTransform.position - (Vector3)eventData.position;
            rectTransform.SetParent(CanvasTransform, true);
            rectTransform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
            if (assignmentController != null)
            {
                assignmentController.BeginEmployeeAssignment(this);
            }
            else
            {
                controller.BeginEmployeeAssignment(this);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvasGroup.interactable)
            {
                if (assignmentController != null)
                {
                    assignmentController.ContinueEmployeeAssignment(
                        this,
                        (Vector3)eventData.position + pointerOffset);
                }
                else
                {
                    controller.ContinueEmployeeAssignment(
                        this,
                        (Vector3)eventData.position + pointerOffset);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            canvasGroup.blocksRaycasts = true;
            if (assignmentController != null)
            {
                assignmentController.EndEmployeeAssignment(this, eventData.position);
            }
            else
            {
                controller.EndEmployeeAssignment(this, eventData.position);
            }
        }

        public void PlaceAt(
            Transform parent,
            Vector2 anchoredPosition,
            Vector3 scale)
        {
            rectTransform.SetParent(parent, false);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.localEulerAngles = Vector3.zero;
            rectTransform.localScale = scale;
            canvasGroup.alpha = 1f;
        }

        public void SetInteraction(bool enabled)
        {
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }

        private Transform CanvasTransform => assignmentController != null
            ? assignmentController.CanvasTransform
            : controller.CanvasTransform;
    }
}

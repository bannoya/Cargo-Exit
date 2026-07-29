using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class PullSortParcelView :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private PullSortPrototypeController controller;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Transform homeParent;
        private Vector2 homePosition;
        private Vector3 pointerOffset;

        public PullSortDestination Destination { get; private set; }
        public RectTransform RectTransform => rectTransform;

        public void Initialize(
            PullSortPrototypeController owner,
            PullSortDestination destination,
            Color color,
            string destinationSymbol,
            Vector2 position)
        {
            controller = owner;
            Destination = destination;
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            homeParent = rectTransform.parent;
            homePosition = position;

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(210f, 170f);
            rectTransform.anchoredPosition = homePosition;

            var body = gameObject.AddComponent<Image>();
            body.color = color;

            var label = PrototypeUi.Label(
                rectTransform,
                "Destination",
                destinationSymbol,
                60,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                rectTransform.sizeDelta,
                Vector2.zero,
                FontStyle.Bold);
            label.raycastTarget = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            homeParent = rectTransform.parent;
            homePosition = rectTransform.anchoredPosition;
            pointerOffset = rectTransform.position - (Vector3)eventData.position;
            rectTransform.SetParent(controller.CanvasTransform, true);
            rectTransform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
            controller.BeginPull(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvasGroup.interactable)
            {
                controller.ContinuePull(
                    this,
                    (Vector3)eventData.position + pointerOffset);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            canvasGroup.blocksRaycasts = true;
            controller.EndPull(this, eventData.position);
        }

        public void RestoreHome()
        {
            rectTransform.SetParent(homeParent, false);
            rectTransform.anchoredPosition = homePosition;
            rectTransform.localScale = Vector3.one;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void SetInteraction(bool enabled)
        {
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }
    }
}


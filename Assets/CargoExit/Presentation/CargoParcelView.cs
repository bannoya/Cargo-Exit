using BannoyasGames.CargoExit.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class CargoParcelView :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private CargoExitPrototypeController controller;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Transform homeParent;
        private Vector2 homePosition;
        private float homeRotation;
        private int homeSiblingIndex;
        private Vector3 pointerOffset;

        public string ParcelId { get; private set; }

        public CargoDestination Destination { get; private set; }
        public RectTransform RectTransform => rectTransform;

        public Vector3 HomeWorldPosition =>
            homeParent.TransformPoint((Vector3)homePosition);

        public void Initialize(
            CargoExitPrototypeController owner,
            string parcelId,
            CargoDestination destination,
            Color color,
            Vector2 position,
            float rotation)
        {
            controller = owner;
            ParcelId = parcelId;
            Destination = destination;
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(190f, 138f);
            rectTransform.anchoredPosition = position;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, rotation);

            homeParent = rectTransform.parent;
            homePosition = position;
            homeRotation = rotation;
            homeSiblingIndex = rectTransform.GetSiblingIndex();

            var body = gameObject.AddComponent<Image>();
            body.color = color;

            var upperTape = UiElementFactory.Panel(
                rectTransform,
                "Tape Top",
                new Color(1f, 1f, 1f, 0.24f),
                new Vector2(190f, 8f),
                new Vector2(0f, 47f));
            upperTape.GetComponent<Image>().raycastTarget = false;

            var lowerTape = UiElementFactory.Panel(
                rectTransform,
                "Tape Bottom",
                new Color(0f, 0f, 0f, 0.09f),
                new Vector2(190f, 7f),
                new Vector2(0f, -47f));
            lowerTape.GetComponent<Image>().raycastTarget = false;

            var label = UiElementFactory.Label(
                rectTransform,
                "Letter",
                destination.ToString(),
                70,
                UiElementFactory.Hex("#172238"),
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

            pointerOffset = rectTransform.position - (Vector3)eventData.position;
            rectTransform.SetParent(controller.CanvasTransform, true);
            rectTransform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
            controller.BeginSort(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvasGroup.interactable)
            {
                controller.ContinueSort(
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
            controller.EndSort(this, eventData.position);
        }

        public void RestoreHome()
        {
            rectTransform.SetParent(homeParent, false);
            rectTransform.SetSiblingIndex(
                Mathf.Min(homeSiblingIndex, homeParent.childCount - 1));
            rectTransform.anchoredPosition = homePosition;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, homeRotation);
            rectTransform.localScale = Vector3.one;
            canvasGroup.alpha = 1f;
            SetInteraction(true);
        }

        public void SetInteraction(bool enabled)
        {
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }
    }
}

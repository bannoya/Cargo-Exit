using BannoyasGames.CargoExit.Core;
using TMPro;
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
        private CargoProcessingController processingController;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Image body;
        private TMP_Text visualLabel;
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
            processingController = null;
            Destination = destination;
            InitializeVisual(
                parcelId,
                color,
                position,
                rotation,
                destination.ToString(),
                CargoExitTypography.ParcelLetter,
                new Vector2(95f, 69f));
        }

        public void InitializeForProcessing(
            CargoExitPrototypeController owner,
            string parcelId,
            Color color,
            Vector2 position,
            float rotation,
            string label)
        {
            controller = owner;
            processingController = null;
            InitializeVisual(
                parcelId,
                color,
                position,
                rotation,
                label,
                22f,
                new Vector2(150f, 85f));
        }

        public void InitializeForProcessing(
            CargoProcessingController owner,
            string parcelId,
            Color color,
            Vector2 position,
            float rotation,
            string label)
        {
            controller = null;
            processingController = owner;
            InitializeVisual(
                parcelId,
                color,
                position,
                rotation,
                label,
                22f,
                new Vector2(150f, 85f));
        }

        public void SetVisualResult(
            string label,
            Color backgroundColor,
            Color textColor)
        {
            visualLabel.text = label;
            visualLabel.fontSize = 20f;
            visualLabel.color = textColor;
            body.color = backgroundColor;
        }

        private void InitializeVisual(
            string parcelId,
            Color color,
            Vector2 position,
            float rotation,
            string label,
            float fontSize,
            Vector2 size)
        {
            ParcelId = parcelId;
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;
            rectTransform.localEulerAngles = new Vector3(0f, 0f, rotation);

            homeParent = rectTransform.parent;
            homePosition = position;
            homeRotation = rotation;
            homeSiblingIndex = rectTransform.GetSiblingIndex();

            body = GetComponent<Image>();
            if (body == null)
            {
                body = gameObject.AddComponent<Image>();
            }

            body.color = color;

            var upperTape = UiElementFactory.Panel(
                rectTransform,
                "Tape Top",
                new Color(1f, 1f, 1f, 0.24f),
                new Vector2(size.x, 4f),
                new Vector2(0f, 24f));
            upperTape.GetComponent<Image>().raycastTarget = false;

            var lowerTape = UiElementFactory.Panel(
                rectTransform,
                "Tape Bottom",
                new Color(0f, 0f, 0f, 0.09f),
                new Vector2(size.x, 4f),
                new Vector2(0f, -24f));
            lowerTape.GetComponent<Image>().raycastTarget = false;

            visualLabel = UiElementFactory.Label(
                rectTransform,
                "Letter",
                label,
                fontSize,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                rectTransform.sizeDelta,
                Vector2.zero,
                FontStyles.Bold);
            visualLabel.raycastTarget = false;
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
            if (processingController != null)
            {
                processingController.BeginSort(this);
            }
            else
            {
                controller.BeginSort(this);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvasGroup.interactable)
            {
                if (processingController != null)
                {
                    processingController.ContinueSort(
                        this,
                        (Vector3)eventData.position + pointerOffset);
                }
                else
                {
                    controller.ContinueSort(
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
            if (processingController != null)
            {
                processingController.EndSort(this, eventData.position);
            }
            else
            {
                controller.EndSort(this, eventData.position);
            }
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

        private Transform CanvasTransform => processingController != null
            ? processingController.CanvasTransform
            : controller.CanvasTransform;
    }
}

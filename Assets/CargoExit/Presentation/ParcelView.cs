using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class ParcelView :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler
    {
        private readonly List<GameObject> generatedVisuals = new();
        private PrototypeGameController controller;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Color baseColor;
        private float cellSize;
        private Vector3 pointerOffset;
        private Transform dragSourceParent;
        private Vector2 dragSourcePosition;
        private GridPosition dragSourceOrigin;
        private bool dragSourceWasPlaced;
        private bool dragged;

        public ParcelDefinition Definition { get; private set; }
        public QuarterTurn Rotation { get; private set; }
        public GridPosition BoardOrigin { get; private set; }
        public bool IsPlaced { get; private set; }
        public RectTransform RectTransform => rectTransform;

        public void Initialize(
            PrototypeGameController owner,
            ParcelDefinition definition,
            Color color,
            float parcelCellSize)
        {
            controller = owner;
            Definition = definition;
            baseColor = color;
            cellSize = parcelCellSize;
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Rotation = QuarterTurn.None;
            RebuildVisuals();
        }

        public void PutInTray(Transform parent, Vector2 position)
        {
            rectTransform.SetParent(parent, false);
            rectTransform.anchoredPosition = position;
            IsPlaced = false;
        }

        public void PutOnBoard(Transform parent, GridPosition origin)
        {
            rectTransform.SetParent(parent, false);
            BoardOrigin = origin;
            rectTransform.anchoredPosition = new Vector2(
                origin.X * cellSize,
                origin.Y * cellSize);
            IsPlaced = true;
        }

        public void SetRotation(QuarterTurn rotation)
        {
            Rotation = rotation;
            RebuildVisuals();
        }

        public void SetInteraction(bool enabled)
        {
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }

        public void SetHighlight(bool highlighted)
        {
            foreach (var visual in generatedVisuals)
            {
                var image = visual.GetComponent<Image>();
                if (image != null)
                {
                    image.color = highlighted
                        ? Color.Lerp(baseColor, Color.white, 0.55f)
                        : baseColor;
                }
            }
        }

        public void ResetVisualState()
        {
            canvasGroup.alpha = 1f;
            SetHighlight(false);
            SetRotation(QuarterTurn.None);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            dragged = false;
            dragSourceParent = rectTransform.parent;
            dragSourcePosition = rectTransform.anchoredPosition;
            dragSourceOrigin = BoardOrigin;
            dragSourceWasPlaced = IsPlaced;
            pointerOffset = rectTransform.position - (Vector3)eventData.position;
            rectTransform.SetParent(controller.CanvasTransform, true);
            canvasGroup.blocksRaycasts = false;
            controller.OnParcelDragStarted(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            dragged = true;
            rectTransform.position = (Vector3)eventData.position + pointerOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canvasGroup.interactable)
            {
                return;
            }

            canvasGroup.blocksRaycasts = true;
            controller.OnParcelDragEnded(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!dragged && canvasGroup.interactable)
            {
                controller.RotateParcel(this);
            }
        }

        public void RestoreDragSource()
        {
            if (dragSourceWasPlaced)
            {
                PutOnBoard(dragSourceParent, dragSourceOrigin);
            }
            else
            {
                PutInTray(dragSourceParent, dragSourcePosition);
            }
        }

        private void RebuildVisuals()
        {
            foreach (var visual in generatedVisuals)
            {
                Destroy(visual);
            }

            generatedVisuals.Clear();
            var cells = Definition.Shape.GetCells(Rotation);
            var maximumX = 0;
            var maximumY = 0;

            foreach (var cell in cells)
            {
                maximumX = Mathf.Max(maximumX, cell.X);
                maximumY = Mathf.Max(maximumY, cell.Y);

                var visual = new GameObject(
                    $"Cell_{cell.X}_{cell.Y}",
                    typeof(RectTransform),
                    typeof(Image));
                generatedVisuals.Add(visual);

                var cellRect = visual.GetComponent<RectTransform>();
                cellRect.SetParent(rectTransform, false);
                cellRect.anchorMin = Vector2.zero;
                cellRect.anchorMax = Vector2.zero;
                cellRect.pivot = Vector2.zero;
                cellRect.sizeDelta = new Vector2(cellSize - 8f, cellSize - 8f);
                cellRect.anchoredPosition = new Vector2(
                    cell.X * cellSize + 4f,
                    cell.Y * cellSize + 4f);
                visual.GetComponent<Image>().color = baseColor;
            }

            rectTransform.sizeDelta = new Vector2(
                (maximumX + 1) * cellSize,
                (maximumY + 1) * cellSize);

            var order = PrototypeUi.Label(
                rectTransform,
                "DeliveryOrder",
                Definition.DeliveryOrder.ToString(),
                38,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                rectTransform.sizeDelta,
                Vector2.zero,
                FontStyle.Bold);
            order.rectTransform.anchorMin = Vector2.zero;
            order.rectTransform.anchorMax = Vector2.zero;
            order.rectTransform.pivot = Vector2.zero;
            order.rectTransform.anchoredPosition = Vector2.zero;
            order.raycastTarget = false;
            generatedVisuals.Add(order.gameObject);
        }
    }
}


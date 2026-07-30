using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    internal static class UiElementFactory
    {
        private static Font font;

        public static RectTransform Panel(
            Transform parent,
            string name,
            Color color,
            Vector2 size,
            Vector2 anchoredPosition)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            gameObject.GetComponent<Image>().color = color;
            return rect;
        }

        public static Text Label(
            Transform parent,
            string name,
            string content,
            int fontSize,
            Color color,
            TextAnchor alignment,
            Vector2 size,
            Vector2 anchoredPosition,
            FontStyle style = FontStyle.Normal)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var text = gameObject.GetComponent<Text>();
            text.font = Font;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        public static Button ActionButton(
            Transform parent,
            string name,
            string caption,
            Color background,
            Color foreground,
            Vector2 size,
            Vector2 anchoredPosition,
            UnityAction action)
        {
            var rect = Panel(parent, name, background, size, anchoredPosition);
            var button = rect.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.9f);
            colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            button.colors = colors;
            button.targetGraphic = rect.GetComponent<Image>();
            button.onClick.AddListener(action);

            var label = Label(
                rect,
                "Label",
                caption,
                38,
                foreground,
                TextAnchor.MiddleCenter,
                size,
                Vector2.zero,
                FontStyle.Bold);
            label.raycastTarget = false;
            return button;
        }

        public static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out var color)
                ? color
                : Color.magenta;
        }

        private static Font Font
        {
            get
            {
                if (font == null)
                {
                    font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return font;
            }
        }
    }
}

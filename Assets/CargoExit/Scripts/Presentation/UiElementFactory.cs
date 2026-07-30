using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    internal static class UiElementFactory
    {
        private static TMP_FontAsset regularFont;
        private static TMP_FontAsset boldFont;

        public static void ConfigureFonts(
            TMP_FontAsset regular,
            TMP_FontAsset bold)
        {
            regularFont = regular;
            boldFont = bold;
        }

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

        public static TextMeshProUGUI Label(
            Transform parent,
            string name,
            string content,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment,
            Vector2 size,
            Vector2 anchoredPosition,
            FontStyles style = FontStyles.Normal)
        {
            var gameObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(TextMeshProUGUI));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var text = gameObject.GetComponent<TextMeshProUGUI>();
            text.font = FontFor(style);
            text.fontSize = fontSize;
            text.fontStyle = StyleFor(style);
            text.alignment = alignment;
            text.color = color;
            text.text = content;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Truncate;
            text.extraPadding = true;
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
                CargoExitTypography.Button,
                foreground,
                TextAlignmentOptions.Center,
                size,
                Vector2.zero,
                FontStyles.Bold);
            label.raycastTarget = false;
            return button;
        }

        public static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out var color)
                ? color
                : Color.magenta;
        }

        private static TMP_FontAsset FontFor(FontStyles style)
        {
            if ((style & FontStyles.Bold) != 0 &&
                boldFont != null)
            {
                return boldFont;
            }

            return regularFont;
        }

        private static FontStyles StyleFor(FontStyles style)
        {
            if (boldFont == null)
            {
                return style;
            }

            return style & ~FontStyles.Bold;
        }
    }
}

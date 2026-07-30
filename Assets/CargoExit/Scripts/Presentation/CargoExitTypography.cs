using TMPro;

namespace BannoyasGames.CargoExit.Presentation
{
    public static class CargoExitTypography
    {
        public const float MinimumReadable = 20f;
        public const float Brand = 20f;
        public const float Title = 32f;
        public const float Status = 22f;
        public const float Combo = 20f;
        public const float PileTitle = 20f;
        public const float Hint = 20f;
        public const float PalletLetter = 34f;
        public const float PalletCounter = 20f;
        public const float ParcelLetter = 36f;
        public const float Button = 24f;

        public static float RecommendedSize(TMP_Text label)
        {
            return label.name switch
            {
                "Brand" => Brand,
                "Title" => Title,
                "Status" => Status,
                "Combo" => Combo,
                "PileTitle" => PileTitle,
                "Hint" => Hint,
                "Counter" => PalletCounter,
                "Label" => Button,
                "Letter" when
                    label.transform.parent != null &&
                    label.transform.parent.name.StartsWith("Pallet ") =>
                        PalletLetter,
                "Letter" => ParcelLetter,
                _ => label.fontSize
            };
        }
    }
}

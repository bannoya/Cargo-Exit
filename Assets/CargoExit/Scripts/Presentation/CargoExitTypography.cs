using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public static class CargoExitTypography
    {
        public const int MinimumReadable = 40;
        public const int Brand = 40;
        public const int Title = 64;
        public const int Status = 44;
        public const int Combo = 40;
        public const int PileTitle = 40;
        public const int Hint = 42;
        public const int PalletLetter = 66;
        public const int PalletCounter = 40;
        public const int ParcelLetter = 70;
        public const int Button = 48;

        public static int RecommendedSize(Text label)
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

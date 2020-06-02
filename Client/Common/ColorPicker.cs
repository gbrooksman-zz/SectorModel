namespace SectorModel.Client.Common
{
    public static class ColorPicker
    {
        public static string Get(int id)
        {
            string colorName = string.Empty;
            switch (id)
            {
                case 0:
                case 1:
                    colorName = "Red";
                    break;
                case 2:
                    colorName = "Blue";
                    break;
                case 3:
                    colorName = "Green";
                    break;
                case 4:
                    colorName = "Yellow";
                    break;
                case 5:
                    colorName = "Gray";
                    break;
                case 6:
                    colorName = "Coral";
                    break;
                case 7:
                    colorName = "Cyan";
                    break;
                case 8:
                    colorName = "BurlyWood";
                    break;
                case 9:
                    colorName = "CornSilk";
                    break;
                case 10:
                    colorName = "DarkCyan";
                    break;
                default:
                    colorName = "Fuchsia";
                    break;
            }

            return colorName;

        }

    }
}

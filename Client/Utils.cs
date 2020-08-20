using System;
using Microsoft.AspNetCore.Components;

namespace SectorModel.Client
{
    public static class Utils
    {
        public static MarkupString CalcGainLoss(decimal current, decimal cost)
        {
            return FormatNumber(current - cost);
        }

		public static MarkupString FormatNumber(decimal inValue)
		{	
			if ( inValue < 0)
			{
				return (MarkupString) $"<p class='text-danger'> {inValue.ToString("n")} </p>";	
			}
			else
			{
				return (MarkupString) $"<p> {inValue.ToString("n")} </p>";	
			}
		}

		public static string FormatDate(DateTime inValue)
		{
			return  inValue.ToShortDateString();	
		}

		public static int GetInterval(DateTime startDate, DateTime stopDate)
        {
            int ret = 0;

            int daysInPeriod = (stopDate - startDate).Days;

            if ((daysInPeriod >= 0) && (daysInPeriod <= 31))
            {
                ret = 1;
            }
            else if ((daysInPeriod > 31) && (daysInPeriod <= 100))
            {
                ret = 3;
            }
            else if ((daysInPeriod > 101) && (daysInPeriod <= 365))
            {
                ret = 5;
            }
            else if (daysInPeriod > 365)
            {
                ret = 10;
            }

            return ret;
        }

        public static string GetColor(int id)
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

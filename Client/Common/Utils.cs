using System;
using Microsoft.AspNetCore.Components;

namespace SectorModel.Client.Common
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
    }
}

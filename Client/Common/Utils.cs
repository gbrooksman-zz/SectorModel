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
    }
}

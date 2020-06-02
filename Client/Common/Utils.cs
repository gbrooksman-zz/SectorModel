namespace SectorModel.Client.Common
{
    public static class Utils
    {
        public static decimal CalcGainLoss(decimal current, decimal cost)
        {
            return current - cost;
        }
    }
}

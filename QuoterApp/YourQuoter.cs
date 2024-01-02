using System;

namespace QuoterApp
{
    public class YourQuoter : IQuoter
    {
        public double GetQuote(string instrumentId, int quantity)
        {
            throw new NotImplementedException();
        }

        public double GetVolumeWeightedAveragePrice(string instrumentId)
        {
            throw new NotImplementedException();
        }
    }
}

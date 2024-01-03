using QuoterApp.Interfaces;
using QuoterApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoterApp.Services
{
    public class YourQuoter : IQuoter
    {
        private string _savedInstrumentID = string.Empty;
        private List<MarketOrder> _marketOrders = new List<MarketOrder>();

        public double GetQuote(string instrumentId, int quantity, bool allowPartialFilling)
        {
            GetInstrumentMarketOrders(instrumentId);

            int instrumentQuantity;
            if (allowPartialFilling)
            {
                // If partial filling allowed we can fill quote using multiple market orders
                instrumentQuantity = _marketOrders.Sum(x => x.Quantity);
            }
            else
            {
                // If partial filling isn't allowed we can fill quote using only one market orders
                instrumentQuantity = _marketOrders.Max(x => x.Quantity);
            }

            if (instrumentQuantity < quantity)
            {
                throw new ArgumentException($"Can't fill quote, not enough volume in the market. The maximum amount is {instrumentQuantity}");
            }

            // Sort Market Orders by price
            _marketOrders.Sort((x, y) => x.Price.CompareTo(y.Price));

            if (allowPartialFilling)
            {
                // If partial filling allowed we can fill quote using multiple market orders
                double quotePrice = 0;
                foreach (var marketOrder in _marketOrders)
                {
                    if (quantity <= marketOrder.Quantity)
                    {
                        quotePrice += marketOrder.Price * quantity;
                        break;
                    }
                    else
                    {
                        quotePrice += marketOrder.Price * marketOrder.Quantity;
                        quantity -= marketOrder.Quantity;
                    }
                }

                return quotePrice;
            }
            else
            {
                // If partial filling isn't allowed we can fill quote using only one market orders
                foreach (var marketOrder in _marketOrders)
                {
                    if (quantity <= marketOrder.Quantity)
                    {
                        return marketOrder.Price * quantity;
                    }
                }

                throw new ArgumentException($"Can't fill quote, not enough volume in the market. The maximum amount is {instrumentQuantity}");
            }
        }

        public double GetVolumeWeightedAveragePrice(string instrumentId)
        {
            if (_savedInstrumentID != instrumentId)
            {
                GetInstrumentMarketOrders(instrumentId);
            }

            double avgPrice = 0.0;
            double volume = 0.0;
            _marketOrders.ForEach(quote =>
            {
                avgPrice += quote.Quantity * quote.Price;
                volume += quote.Quantity;
            });

            // if volume = 0 -> avgProce = 0
            if (volume == 0)
            {
                return 0.0;
            }

            return avgPrice / volume;
        }


        #region Private Methods

        private void GetInstrumentMarketOrders(string instrumentId)
        {
            var quoteSource = new HardcodedMarketOrderSource();
            var requiredMarketOrders = new List<MarketOrder>();

            // Created a loop which would wait until all market orders will arrive
            while (true)
            {
                var quote = quoteSource.GetNextMarketOrder();
                if (quote == null)
                {
                    break;
                }
                if (quote.InstrumentId == instrumentId)
                {
                    requiredMarketOrders.Add(quote);
                }
            }

            if (requiredMarketOrders.Count == 0)
            {
                throw new ArgumentException("This instrument ID doesn't exist or no market orders curently are available");
            }

            _savedInstrumentID = instrumentId;
            _marketOrders = requiredMarketOrders;
        }

        #endregion
    }
}

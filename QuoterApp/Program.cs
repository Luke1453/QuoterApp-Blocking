using QuoterApp.Services;
using System;

namespace QuoterApp;

class Program
{
    static void Main()
    {
        try
        {
            var gq = new YourQuoter();
            var qty = 120;
            var instrumentID = "DK50782120";

            var quote = gq.GetQuote(instrumentID, qty, false);
            var vwap = gq.GetVolumeWeightedAveragePrice(instrumentID);

            Console.WriteLine($"Quote: {quote}\tAverage Quote Price: {quote / qty}");
            Console.WriteLine($"Average Market Price: {vwap}");
            Console.WriteLine();
            Console.WriteLine($"Done");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
        }
    }
}

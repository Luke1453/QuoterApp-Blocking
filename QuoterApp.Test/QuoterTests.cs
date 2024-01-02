using NUnit.Framework;
using QuoterApp.Services;

namespace QuoterApp.Test;

public class QuoterTests
{
    // If Market Order Source wouldn't be hardcoded I'd Dependancy Inject and mock hardcoded marker order source

    [TestCase("BA79603015", 50, true, 5152.364)]
    [TestCase("BA79603015", 50, false, 5160)]
    [TestCase("AB73567490", 50, true, 5010.65)]
    [TestCase("AB73567490", 50, false, 5162.5)]
    [TestCase("DK50782120", 50, true, 4990.5)]
    [TestCase("DK50782120", 50, false, 4990.5)]
    public void GetQuote_ShouldReturn_CorrectQuote(string instrumentId, int quantity, bool allowPartialFilling, double result)
    {
        // Arrange
        var _sut = new YourQuoter();

        // Act
        var quote = _sut.GetQuote(instrumentId, quantity, allowPartialFilling);

        // Assert
        Assert.That(quote, Is.EqualTo(result));
    }

    [TestCase("DK50782120", 5000, true)]
    [TestCase("AB73567490", 5000, false)]
    [TestCase("BA79603015", 5000, true)]
    public void GetQuote_NoVolume_ShouldThrow(string instrumentId, int quantity, bool allowPartialFilling)
    {
        // Arrange
        var _sut = new YourQuoter();

        // Act
        void test() => _sut.GetQuote(instrumentId, quantity, allowPartialFilling);

        // Assert
        var ex = Assert.Throws<ArgumentException>(test);
        Assert.That(ex?.Message.Contains("Can't fill quote, not enough volume in the market. The maximum amount is"), Is.True);
    }

    [TestCase("KD796030AA")]
    [TestCase("GKI9603033")]
    public void GetQuote_IncorrectInstrument_ShouldThrow(string instrumentId)
    {
        // Arrange
        var _sut = new YourQuoter();

        // Act
        void test() => _sut.GetQuote(instrumentId, 1, true);

        // Assert
        var ex = Assert.Throws<ArgumentException>(test);
        Assert.That(ex?.Message, Is.EqualTo("This instrument ID doesn't exist or no market orders curently are available"));
    }

    [TestCase("BA79603015", 103.09539726027397)]
    [TestCase("AB73567490", 101.86954545454545)]
    [TestCase("DK50782120", 99.940128690386075)]
    public void GetVolumeWeightedAveragePrice_ShouldReturn_CorrectQuotePrice(string instrumentId, double result)
    {
        // Arrange
        var _sut = new YourQuoter();

        // Act
        var avgPrice = _sut.GetVolumeWeightedAveragePrice(instrumentId);

        // Assert
        Assert.That(avgPrice, Is.EqualTo(result));
    }

    [TestCase("KD796030AA")]
    [TestCase("GKI9603033")]
    public void GetVolumeWeightedAveragePrice_IncorrectInstrument_ShouldThrow(string instrumentId)
    {
        // Arrange
        var _sut = new YourQuoter();

        // Act
        void test() => _sut.GetVolumeWeightedAveragePrice(instrumentId);

        // Assert
        var ex = Assert.Throws<ArgumentException>(test);
        Assert.That(ex?.Message, Is.EqualTo("This instrument ID doesn't exist or no market orders curently are available"));
    }

}

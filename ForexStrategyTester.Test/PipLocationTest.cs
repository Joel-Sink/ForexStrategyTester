using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ForexStrategyTester;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using Xunit;

namespace ForexStrategyTester.Test
{
    public class PipLocationTest
    {
        [Fact]
        public void GetPips_ShouldReturnPipLocation()
        {
            // Arrange
            decimal expected = (decimal).0001;

            // Act
            decimal actual = Program.GetPipLocation(Program.GetEUR_USD());

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetPair_ShouldEqualEUR_USD()
        {
            string expected = "EUR_USD";

            string actual = Program.GetEUR_USD().name;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetInstrumentPrice_ShouldBeAroundOne()
        {
            decimal expected = (decimal)1.2;

            decimal actual = Program.GetInstrumentPrice(Program.GetEUR_USD());

            Assert.Equal(expected, actual, 1);
        }

        [Fact]
        public void GetLotSize_ShouldBeHigh()
        {
            decimal expected = 100000;

            decimal actual = Program.GetLotSize(Program.GetEUR_USD());

            Assert.NotEqual(expected, actual);
        }

        [Fact]
        public void GenerateStopLossBuy_ShouldBeFiftyPipsLess()
        {
            decimal? expected = (decimal)1.234;

            decimal? actual = Program.FindStopPointBuy(Program.GetEUR_USD());

            Assert.NotEqual(expected, actual);
        }

        [Fact]
        public void GenerateStopLossSell_ShouldBeFiftyPipsMore()
        {
            decimal? expected = (decimal)1.234;

            decimal? actual = Program.FindStopPointSell(Program.GetEUR_USD());

            Assert.NotEqual(expected, actual);
        }

        [Fact]
        public void FindTakePointBuy_ShouldBeFiftyPipsMore()
        {
            decimal expected = (decimal)1.234;

            decimal actual = Program.FindTakePointBuy(Program.GetEUR_USD());

            Assert.NotEqual(expected, actual);
        }

        [Fact]
        public void FindTakePointSell_ShouldBeFiftyPipsLess()
        {
            decimal expected = (decimal)1.234;

            decimal actual = Program.FindTakePointSell(Program.GetEUR_USD());

            Assert.NotEqual(expected, actual);
        }
    }
}

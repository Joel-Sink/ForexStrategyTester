using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OkonkwoOandaV20;
using OkonkwoOandaV20.TradeLibrary.Instrument;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace ForexStrategyTester
{
    public class Program
    {

        public enum TradeDirection
        {
            BUY,
            SELL
        }

        private static string accesstoken;
        public static string AccountID { get; set; }
        public static int BetPips { get; set; }

        static void SetApiCredentials()
        {
            Console.WriteLine("Setting your V20 credentials ...");

            AccountID = "101-001-14918521-001";
            var environment = EEnvironment.Practice;
            var token = "5814e7cef9981369aefcb5d1354ea38e-fdc8d9d6082a5abbb02a3eedca940252";
            accesstoken = token;

            Credentials.SetCredentials(environment, token, AccountID);

            Console.WriteLine("Nice! Credentials are set.");
        }

        public static Instrument GetEUR_USD()
        {
            SetApiCredentials();
            return GetAccountInstrumentsAsync(AccountID).Result.Where(i => i.displayName.Contains("EUR") && i.displayName.Contains("USD")).FirstOrDefault();
        }

        static void WaitForConnection()
        {
            while (0 < 1)
            {
                HttpWebRequest request = WebRequest.CreateHttp(@"https://api-fxpractice.oanda.com/v3/accounts/" + AccountID);
                request.Method = "GET";
                request.Headers[HttpRequestHeader.Authorization] = "Bearer " + accesstoken;
                request.Timeout = 7000;
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        response.Dispose();
                        continue;
                    }
                    response.Dispose();
                    return;
                }
                catch
                {
                    continue;
                }
            }
        }

        static void Main(string[] args)
        {
            BetPips = 50;

            SetApiCredentials();

            WaitForConnection();

            Instrument instrument = GetEUR_USD();

            double lot = GetLotSize(instrument);

            TradeDirection direction = GenerateTradeDirection();
        }

        #region StopLoss
        public static StopLossDetails GenerateStopLoss(Instrument instrument, TradeDirection direction)
        {
            BetPips = 50;
            SetApiCredentials();

            decimal stopprice = direction == TradeDirection.BUY ? FindStopPointBuy(instrument) : FindStopPointSell(instrument);

            return new StopLossDetails(instrument) { price = stopprice };
        }

        public static decimal FindStopPointBuy(Instrument instrument)
        {
            BetPips = 50;
            SetApiCredentials();

            decimal price = GetInstrumentPrice(instrument);

            decimal pip = GetPipLocation(instrument);

            return price - (pip * BetPips);
        }

        public static decimal FindStopPointSell(Instrument instrument)
        {
            BetPips = 50;
            SetApiCredentials();

            decimal price = GetInstrumentPrice(instrument);

            decimal pip = GetPipLocation(instrument);

            return price + (pip * BetPips);
        }
        #endregion

        #region TakeProfit
        public static TakeProfitDetails GenerateTakeProfit(Instrument instrument, TradeDirection direction)
        {
            BetPips = 50;
            SetApiCredentials();

            decimal takeprice = direction == TradeDirection.BUY ? FindTakePointBuy(instrument) : FindTakePointSell(instrument);

            return new TakeProfitDetails(instrument) { price = takeprice };

        }

        public static decimal FindTakePointBuy(Instrument instrument)
        {
            BetPips = 50;
            SetApiCredentials();

            decimal price = GetInstrumentPrice(instrument);

            decimal pip = GetPipLocation(instrument);

            return price + (pip * BetPips * 2);
        }

        public static decimal FindTakePointSell(Instrument instrument)
        {
            BetPips = 50;
            SetApiCredentials();

            decimal price = GetInstrumentPrice(instrument);

            decimal pip = GetPipLocation(instrument);

            return price - (pip * BetPips * 2);
        }
        #endregion

        public static TradeDirection GenerateTradeDirection()
        {
            Random random = new Random();

            if(random.Next(1, 2) == 1)
            {
                return TradeDirection.BUY;
            }
            else
            {
                return TradeDirection.SELL;
            }
        }

        public static int GetLotSize(Instrument instrument)
        {
            decimal balance = GetAccountAsync(AccountID).Result.balance;

            decimal risk = balance * (decimal).02;

            decimal pipvalue = risk / BetPips;

            decimal price = GetInstrumentPrice(instrument);

            decimal piplocation = GetPipLocation(instrument);

            decimal lotsize = pipvalue / (piplocation / price);

            return Convert.ToInt32(lotsize);
        }

        public static decimal GetPipLocation(Instrument instrument)
        {
            SetApiCredentials();
            return (decimal)Math.Pow(10, instrument.pipLocation);
        }

        public static decimal GetInstrumentPrice(Instrument instrument)
        {
            SetApiCredentials();
            var param = new PricingParameters() { instruments = new List<string>() { instrument.name } };
            return GetPricingAsync(AccountID, param).Result.First().asks.First().price;
        }
    }
}

using CsvHelper;
using FinancialBot.BL.DTOs;
using FinancialBot.Core.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FinancialBot.Services.Mqt
{
    public class StockBotServices: IMqtBot
    {
        private string GetStockCode(string message)
        {
            var stockCode = string.Empty;
            var proccesor = new Regex(@"\/stock=(?<StockCode>.*)");
            Match matches = proccesor.Match(message);

            if (matches.Success)
                stockCode = matches.Groups["StockCode"].Value;

            return stockCode;
        }

        private IList<StockModel> GetStockQuoteFromAPI(string stockCode)
        {
            var request = (HttpWebRequest)WebRequest.Create($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
            var response = (HttpWebResponse)request.GetResponse();

            TextReader reader = new StreamReader(response.GetResponseStream());
            var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csvReader.GetRecords<StockModel>().ToList();

            return records;
        }

        public StockModel GetStockQuote(UsersMessages clientMessage)
        {
            if (clientMessage == null)
                throw new ArgumentNullException($"Fail the argument {nameof(clientMessage)} cannot be null.");

            string stockCode = GetStockCode(clientMessage.Message);

            if (!string.IsNullOrWhiteSpace(stockCode))
            {
                try
                {
                    IList<StockModel> stockQuotes = GetStockQuoteFromAPI(stockCode);

                    if (stockQuotes != null && stockQuotes.Any())
                    {
                        return stockQuotes[0];
                    }
                }
                catch (Exception e)
                {
                    throw new FormatException(e.Message);
                }
            }

            return null;
        }
    }
}

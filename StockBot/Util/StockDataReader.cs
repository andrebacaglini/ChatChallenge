using CsvHelper;
using StockBot.Model;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace StockBot.Util
{
    public static class StockDataReader
    {
        public static StockData ReadStockDataFromCSV(Stream stream)
        {
            try
            {
                using var reader = new StreamReader(stream, Encoding.Default);
                using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                var records = csv.GetRecords<StockData>().ToList();
                return records.First();
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}


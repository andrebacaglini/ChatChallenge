using CsvHelper.Configuration;
using System;

namespace ChatChallenge.StockBot.Model
{
    public class StockData
    {
        public string Symbol { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }

    public sealed class StockDataMap : ClassMap<StockData>
    {

    }
}

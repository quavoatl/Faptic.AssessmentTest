namespace Assessment.Entities
{
    public class Data
    {
        public List<Ohlc>? Ohlc { get; set; }
    }

    public class Ohlc
    {
        public double? Close { get; set; }
    }

    public class BitstampApiResponse
    {
        public Data? Data { get; set; }
    }
}
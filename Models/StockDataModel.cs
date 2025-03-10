using System;
using System.Text.Json.Serialization;

public class StockDataModel
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("open")]
    public double Open { get; set; }

    [JsonPropertyName("high")]
    public double High { get; set; }

    [JsonPropertyName("low")]
    public double Low { get; set; }

    [JsonPropertyName("close")]
    public double Close { get; set; }

    [JsonPropertyName("adjusted_close")]
    public double AdjustedClose { get; set; }

    [JsonPropertyName("volume")]
    public long Volume { get; set; }
}

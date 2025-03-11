using System.Text.Json.Serialization;

public class SentimentsModel {
    [JsonPropertyName("TOTAL_POSITIVE")]
    public int TotalPositive { get; set; }
    [JsonPropertyName("TOTAL_NEGATIVE")]
    public int TotalNegative { get; set; }
    [JsonPropertyName("TOTAL_NEUTRAL")]
    public int TotalNeutral { get; set; }
    [JsonPropertyName("SENTIMENT_SCORE")]
    public double SentimentScore { get; set; }

    public SentimentsModel(int totalPositive, int totalNegative, int totalNeutral, double sentimentScore) {
        TotalPositive = totalPositive;
        TotalNegative = totalNegative;
        TotalNeutral = totalNeutral;
        SentimentScore = sentimentScore;
    }
}
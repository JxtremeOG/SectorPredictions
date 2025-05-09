using System.Data;

public class SectorSentimentsCalculations {
    public List<string> SectorNames { get; set; } = new List<string>
    {
        "Technology", "Financial Services", "Consumer Cyclical",
        "Healthcare", "Communication Services", "Industrials",
        "Consumer Defensive", "Energy", "Basic Materials",
        "Real Estate", "Utilities", "General"
    };
    public Tuple<string, string> quarterToPredict { get; set; }
    public DataReader dataReader { get; set; }

    public Tuple<string, string> lastQuarterDate { get; set; }
    public Tuple<string, string> lastYearDate { get; set; }
    public SentimentsModel lastQuarterData { get; set; }
    public SentimentsModel lastYearData { get; set; }
    public SectorSentimentsCalculations(Tuple<string, string> quarterPassed) {
        dataReader = new DataReader();
        quarterToPredict = quarterPassed;

        lastQuarterDate = DateAdjuster.SubtractQuarters(quarterPassed, 1);
        lastYearDate = DateAdjuster.SubtractQuarters(quarterPassed, 4);
    }

    public Dictionary<string, SectorSentimentModel> SectorSentiments() {
        Dictionary<string, SectorSentimentModel> sentimentsDict = new Dictionary<string, SectorSentimentModel>();

        List<SentimentsModel> lastQuarter = GetSectorSentiments(lastQuarterDate);
        List<SentimentsModel> lastYear = GetSectorSentiments(lastYearDate);

        for (int sectorIndex = 0; sectorIndex < SectorNames.Count; sectorIndex++)
        {
            string name = SectorNames[sectorIndex];
            sentimentsDict[name] = new SectorSentimentModel(lastQuarter[sectorIndex], lastYear[sectorIndex]);
        }
        return sentimentsDict;
    }

    public List<SentimentsModel> GetSectorSentiments(Tuple<string, string> quarterYear)
    {
        List<SentimentsModel> sentiments = new List<SentimentsModel>();

        for (int sectorIndex = 0; sectorIndex < SectorNames.Count; sectorIndex++)
        {
            string name = SectorNames[sectorIndex];
            sentiments.Add(GetSectorSentimentModel(quarterYear, name));
            Console.WriteLine($"Year: {quarterYear.Item2} | Quarter: {quarterYear.Item1} | Sector: {name.PadRight(22)} | Sentiment Score: {sentiments[sentiments.Count-1].SentimentScore.ToString("F2").PadRight(8)}");
        }
        return sentiments;
    }

    public SentimentsModel GetSectorSentimentModel(Tuple<string, string> quarterYear, string sector)
    {
        // Get the DataTable using your existing method.
        DataTable dt = dataReader.GetSectorSentiment(quarterYear, sector);

        // If no data is returned, return a default model.
        if (dt.Rows.Count == 0)
        {
            return new SentimentsModel(0, 0, 0, 0.0);
        }

        // Assuming the query returns only one row per sector/quarter,
        // get the first DataRow.
        DataRow row = dt.Rows[0];

        // Convert each column value to the appropriate type.
        int totalPositive = row["TOTAL_POSITIVE"] != DBNull.Value ? Convert.ToInt32(row["TOTAL_POSITIVE"]) : 0;
        int totalNegative = row["TOTAL_NEGATIVE"] != DBNull.Value ? Convert.ToInt32(row["TOTAL_NEGATIVE"]) : 0;
        int totalNeutral = row["TOTAL_NEUTRAL"] != DBNull.Value ? Convert.ToInt32(row["TOTAL_NEUTRAL"]) : 0;
        double sentimentScore = row["SENTIMENT_SCORE"] != DBNull.Value ? Convert.ToDouble(row["SENTIMENT_SCORE"]) : 0.0;

        // Create and return the SectorSentimentModel.
        return new SentimentsModel(totalPositive, totalNegative, totalNeutral, sector.Equals("General") ? sentimentScore : sentimentScore / 100); //Number of stocks per sector we used
    }


}
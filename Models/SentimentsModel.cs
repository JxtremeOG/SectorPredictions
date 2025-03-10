public class SentimentsModel {
    public SectorSentimentModel LastQuarter { get; set; }
    public SectorSentimentModel LastYear { get; set; }

    public SentimentsModel(SectorSentimentModel lastQuarter, SectorSentimentModel lastYear) {
        LastQuarter = lastQuarter;
        LastYear = lastYear;
    }
}
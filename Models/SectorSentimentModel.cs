public class SectorSentimentModel {
    public SentimentsModel LastQuarter { get; set; }
    public SentimentsModel LastYear { get; set; }

    public SectorSentimentModel(SentimentsModel lastQuarter, SentimentsModel lastYear) {
        LastQuarter = lastQuarter;
        LastYear = lastYear;
    }
}
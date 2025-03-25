public class SectorTechnicalMetricModel {
    public double Day30Percent { get; set; }
    public double Day90Percent { get; set; }
    public double Day365Percent { get; set; }
    public double Day1095Percent { get; set; }
    public double RSI30 { get; set; }
    public double ATR90 { get; set; }
    public double ADLChange30 { get; set; }

    public SectorTechnicalMetricModel(double day30Percent, double day90Percent, double day365Percent, double day1095Percent, double rSI30, double aTR90, double aDLChange30) {
        Day30Percent = day30Percent;
        Day90Percent = day90Percent;
        Day365Percent = day365Percent;
        Day1095Percent = day1095Percent;
        RSI30 = rSI30;
        ATR90 = aTR90;
        ADLChange30 = aDLChange30;
    }
}
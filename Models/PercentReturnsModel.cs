public class PercentReturnsModel {
    public double Day30Percent { get; set; }
    public double Day90Percent { get; set; }
    public double Day365Percent { get; set; }
    public double Day1095Percent { get; set; }

    public PercentReturnsModel(double day30Percent, double day90Percent, double day365Percent, double day1095Percent) {
        Day30Percent = day30Percent;
        Day90Percent = day90Percent;
        Day365Percent = day365Percent;
        Day1095Percent = day1095Percent;
    }
}
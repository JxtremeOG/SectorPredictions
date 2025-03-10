public class QuarterRangeModel
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Quarter { get; set; }  // e.g., "Q1"
    public int Year { get; set; }
}
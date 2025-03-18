public record QuarterRangeRecord
{
    public DateTime Start { get; set; } = DateTime.MinValue;
    public DateTime End { get; set; } = DateTime.MaxValue;
    public string Quarter { get; set; }  // e.g., "Q1"
    public string Year { get; set; }
}
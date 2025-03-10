using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

public class MarketCalculations
{
    // A list of sector names. Populate with your actual sectors.
    public List<string> SectorNames { get; set; } = new List<string>
    {
        "Technology", "Financial Services", "Consumer Cyclical",
        "Healthcare", "Communication Services", "Industrials",
        "Consumer Defensive", "Energy", "Basic Materials",
        "Real Estate", "Utilities"
    };
    public DateTime DaysStartRun { get; set; }
    public DateTime DaysAgo30Date { get; set; }
    public DateTime DaysAgo90Date { get; set; }
    public DateTime DaysAgo180Date { get; set; }
    public DateTime DaysAgo365Date { get; set; }
    public DateTime DaysAgo1095Date { get; set; }

    // Data lists for each period.
    public List<double> Sectors30Data { get; set; }
    public List<double> Sectors90Data { get; set; }
    public List<double> Sectors365Data { get; set; }
    public List<double> Sectors1095Data { get; set; }
    public DataReader dataReader { get; set; }

    public MarketCalculations(DateTime daysStartRun)
    {
        dataReader = new DataReader();

        DaysStartRun = daysStartRun;
        DaysAgo30Date = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-30));
        DaysAgo90Date = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-90));
        DaysAgo180Date = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-180));
        DaysAgo365Date = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-365));
        DaysAgo1095Date = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-1095));

        Sectors30Data = new List<double>();
        Sectors90Data = new List<double>();
        Sectors365Data = new List<double>();
        Sectors1095Data = new List<double>();
    }

    public List<double> GetRollingPercent(int daysAgo)
    {
        List<double> sectorPercents = new List<double>();
        DateTime startDate, endDate;

        if (daysAgo == 30)
        {
            startDate = DaysAgo30Date;
            endDate = DaysStartRun;
        }
        else if (daysAgo == 90)
        {
            startDate = DaysAgo90Date;
            endDate = DaysStartRun;
        }
        else if (daysAgo == 180)
        {
            startDate = DaysAgo180Date;
            endDate = DaysStartRun;
        }
        else if (daysAgo == 365)
        {
            startDate = DaysAgo365Date;
            endDate = DaysStartRun;
        }
        else if (daysAgo == 1095)
        {
            startDate = DaysAgo1095Date;
            endDate = DaysStartRun;
        }
        else
        {
            throw new ArgumentException("Unsupported daysAgo value.");
        }

        foreach (var sector in SectorNames)
        {
            // Get the DataTable for each date.
            DataTable startDataTable = dataReader.SectorCalculations(startDate, sector);
            DataTable endDataTable = dataReader.SectorCalculations(endDate, sector);

            // Ensure the DataTable has rows before processing.
            if (startDataTable.Rows.Count == 0 || endDataTable.Rows.Count == 0)
            {
                Console.WriteLine($"No data for sector {sector} on one of the dates.");
                continue;
            }

            // Use LINQ to convert DataRows to a sequence of double values.
            double startValue = startDataTable.AsEnumerable()
                .Select(row => row.Field<double>("WEIGHTED_ADJUSTED_CLOSE"))
                .Average();

            double endValue = endDataTable.AsEnumerable()
                .Select(row => row.Field<double>("WEIGHTED_ADJUSTED_CLOSE"))
                .Average();

            double percentChange = ((endValue - startValue) / startValue) * 100;
            sectorPercents.Add(percentChange);
            
            Console.WriteLine($"Sector: {sector.PadRight(22)} | Start: {startValue.ToString("F2").PadRight(8)}, End: {endValue.ToString("F2").PadRight(8)}, Change: {percentChange.ToString("F2")}%");
        }

        return sectorPercents;
    }


    // This method builds a dictionary mapping each sector to a dictionary of percent values.
    public Dictionary<string, PercentReturnsModel> MarketPercents()
    {
        Dictionary<string, PercentReturnsModel> percentDict = new Dictionary<string, PercentReturnsModel>();

        List<double> day30 = GetRollingPercent(30);
        List<double> day90 = GetRollingPercent(90);
        List<double> day365 = GetRollingPercent(365);
        List<double> day1095 = GetRollingPercent(1095);

        for (int sectorIndex = 0; sectorIndex < SectorNames.Count; sectorIndex++)
        {
            string name = SectorNames[sectorIndex];
            percentDict[name] = new PercentReturnsModel(day30[sectorIndex], day90[sectorIndex], day365[sectorIndex], day1095[sectorIndex]);
        }
        return percentDict;
    }


}

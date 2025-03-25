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
    public DateTime daysStartRun { get; set; }
    public DataReader dataReader { get; set; }

    public MarketCalculations(DateTime daysStartRun)
    {
        dataReader = new DataReader();

        this.daysStartRun = daysStartRun;
    }

    public MarketSectorResultModel GetCurrentQuarterReturns(Tuple<string, string> quarter) {
        DateTime daysStartRun = DateAdjuster.AdjustWeekendAndHolidays(DateAdjuster.QuarterStartDate(quarter));
        DateTime daysEndRun = DateAdjuster.AdjustWeekendAndHolidays(DateAdjuster.QuarterEndDate(quarter));

        MarketSectorResultModel marketResults = new MarketSectorResultModel();

        foreach (var sector in SectorNames) {
            DataTable startDataTable = dataReader.SectorETFCalculations(daysStartRun, sector);
            DataTable endDataTable = dataReader.SectorETFCalculations(daysEndRun, sector);

            double startValue = startDataTable.AsEnumerable()
                .Select(row => row.Field<double>("WEIGHTED_ADJUSTED_CLOSE"))
                .Average();

            double endValue = endDataTable.AsEnumerable()
                .Select(row => row.Field<double>("WEIGHTED_ADJUSTED_CLOSE"))
                .Average();

            double percentChange = ((endValue - startValue) / startValue) * 100;

            switch (sector) {
                case "Technology":
                    marketResults.TechnologyQuarterResult = percentChange;
                    break;
                case "Financial Services":
                    marketResults.FinancialServicesQuarterResult = percentChange;
                    break;
                case "Consumer Cyclical":
                    marketResults.ConsumerCyclicalQuarterResult = percentChange;
                    break;
                case "Healthcare":
                    marketResults.HealthcareQuarterResult = percentChange;
                    break;
                case "Communication Services":
                    marketResults.CommunicationServicesQuarterResult = percentChange;
                    break;
                case "Industrials":
                    marketResults.IndustrialsQuarterResult = percentChange;
                    break;
                case "Consumer Defensive":
                    marketResults.ConsumerDefensiveQuarterResult = percentChange;
                    break;
                case "Energy":
                    marketResults.EnergyQuarterResult = percentChange;
                    break;
                case "Basic Materials":
                    marketResults.BasicMaterialsQuarterResult = percentChange;
                    break;
                case "Real Estate":
                    marketResults.RealEstateQuarterResult = percentChange;
                    break;
                case "Utilities":
                    marketResults.UtilitiesQuarterResult = percentChange;
                    break;
            }
        }
        return marketResults;
    }

    public MarketSectorResultModel GetLastQuarterReturns(Tuple<string, string> quarter) {
        Tuple<string, string> lastQuarter = DateAdjuster.SubtractQuarters(quarter, 1);
        return GetCurrentQuarterReturns(lastQuarter);
    }

    public List<double> GetRollingPercent(int daysAgo)
    {
        List<double> sectorPercents = new List<double>();
        DateTime startDate, endDate;

        endDate = daysStartRun;
        startDate = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-daysAgo));

        foreach (var sector in SectorNames)
        {
            // Get the DataTable for each date.
            DataTable startDataTable = dataReader.SectorETFCalculations(startDate, sector);
            DataTable endDataTable = dataReader.SectorETFCalculations(endDate, sector);

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
            
            Console.WriteLine($"Start Date: {startDate.ToString("MM/dd/yyyy")} | Days Back {daysAgo.ToString().PadRight(4)} | Sector: {sector.PadRight(22)} | Start: {startValue.ToString("F2").PadRight(8)}, End: {endValue.ToString("F2").PadRight(8)}, Change: {percentChange.ToString("F2")}%");
        }

        return sectorPercents;
    }

    public List<double> GetADLChangePercent(int days) {
        List<double> sectorADLPercents = new List<double>();
        DateTime startDate, endDate;

        endDate = daysStartRun;
        startDate = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-days));

        foreach (var sector in SectorNames)
        {
            // Get the DataTable for each date.
            DataTable averageChangeDataTable1 = dataReader.SectorADLChange(startDate, endDate, sector);
            DataTable averageChangeDataTable2 = dataReader.SectorADLChange(DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-365)), startDate, sector);

            // Use LINQ to convert DataRows to a sequence of double values.
            double ADL1 = averageChangeDataTable1.AsEnumerable()
                .Select(row => row.Field<double>("ADL"))
                .Last();

            double ADL2 = averageChangeDataTable2.AsEnumerable()
                .Select(row => row.Field<double>("ADL"))
                .Last();

            double ADLPercentChange = (ADL2/365 - ADL1/days) / ADL1/days * 100;

            sectorADLPercents.Add(ADLPercentChange);
            
            Console.WriteLine($"Start Date: {startDate.ToString("MM/dd/yyyy")} | Duration: {days.ToString().PadRight(4)} | Sector: {sector.PadRight(22)} | Accumulation/Distribution Line: {ADLPercentChange.ToString("F2").PadRight(8)}");
        }

        return sectorADLPercents;
    }
    public List<double> GetAverageTrueRange(int days) {
        List<double> sectorATR = new List<double>();
        DateTime startDate, endDate;

        endDate = daysStartRun;
        startDate = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-days));

        foreach (var sector in SectorNames)
        {
            // Get the DataTable for each date.
            DataTable averageChangeDataTable = dataReader.SectorETFATR(startDate, endDate, sector);

            // Use LINQ to convert DataRows to a sequence of double values.
            double ATR = averageChangeDataTable.AsEnumerable()
                .Select(row => row.Field<double>("ATR_PERCENT"))
                .Average();

            sectorATR.Add(ATR);
            
            Console.WriteLine($"Start Date: {startDate.ToString("MM/dd/yyyy")} | Duration: {days.ToString().PadRight(4)} | Sector: {sector.PadRight(22)} | Average True Range: {ATR.ToString("F2").PadRight(8)}");
        }

        return sectorATR;
    }
    public List<double> GetRelativeStrengthIndex(int days) {
        List<double> sectorRSI = new List<double>();
        DateTime startDate, endDate;

        endDate = daysStartRun;
        startDate = DateAdjuster.AdjustWeekendAndHolidays(daysStartRun.AddDays(-days));

        foreach (var sector in SectorNames)
        {
            // Get the DataTable for each date.
            DataTable averageChangeDataTable = dataReader.SectorETFRSIInfo(startDate, endDate, sector);

            // Use LINQ to convert DataRows to a sequence of double values.
            double averageGain = averageChangeDataTable.AsEnumerable()
                .Select(row => row.Field<double>("AVERAGE_GAIN"))
                .Average();

            double averageLoss = averageChangeDataTable.AsEnumerable()
                .Select(row => row.Field<double>("AVERAGE_LOSS"))
                .Average();

            double relativeStrength = averageGain / averageLoss;
            double relativeStrengthIndex = 100 - (100 / (1 + relativeStrength));

            sectorRSI.Add(relativeStrengthIndex);
            
            Console.WriteLine($"Start Date: {startDate.ToString("MM/dd/yyyy")} | Duration: {days.ToString().PadRight(4)} | Sector: {sector.PadRight(22)} | Relative Strength Index: {relativeStrengthIndex.ToString("F2").PadRight(8)}");
        }

        return sectorRSI;
    }
    
    // This method builds a dictionary mapping each sector to a dictionary of percent values.
    public Dictionary<string, SectorTechnicalMetricModel> MarketTechnicalIndicators()
    {
        Dictionary<string, SectorTechnicalMetricModel> percentDict = new Dictionary<string, SectorTechnicalMetricModel>();

        List<double> day30 = GetRollingPercent(30);
        List<double> day90 = GetRollingPercent(90);
        List<double> day365 = GetRollingPercent(365);
        List<double> day1095 = GetRollingPercent(1095);
        List<double> RSI30 = GetRelativeStrengthIndex(30);
        List<double> ATR90 = GetAverageTrueRange(90);
        List<double> ADLChange30 = GetADLChangePercent(30);

        for (int sectorIndex = 0; sectorIndex < SectorNames.Count; sectorIndex++)
        {
            string name = SectorNames[sectorIndex];
            percentDict[name] = new SectorTechnicalMetricModel(day30[sectorIndex], day90[sectorIndex], day365[sectorIndex], day1095[sectorIndex], RSI30[sectorIndex], ATR90[sectorIndex], ADLChange30[sectorIndex]);
        }
        return percentDict;
    }


}

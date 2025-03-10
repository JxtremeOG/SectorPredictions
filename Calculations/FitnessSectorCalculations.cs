using System;
using System.Collections.Generic;
using System.Linq;

public class FitnessSectorCalculations
{
    // Static lists corresponding to defensive and cyclical sectors.
    public static List<string> DefensiveSectors = new List<string> {
        "Healthcare", "Communication Services", "Consumer Defensive", "Energy", "Real Estate", "Utilities"
    };

    public static List<string> CyclicalSectors = new List<string> {
        "Technology", "Financial Services", "Consumer Cyclical", "Industrials", "Real Estate", "Basic Materials"
    };

    // Instance fields.
    private MarketCalculations marketCalculations;
    private SectorSentimentsCalculations sectorSentiments;

    // Weights and configuration parameters.
    private double day30Weight = 0.6;
    private double day90Weight = 0.3;
    private double day365Weight = 0.2;
    private double day1095Weight = 0.4;
    private double sentimentWeight = 0.6;
    private double bullvsBearWeight = 100;
    private double sortinoRiskWeight = 7;
    private double hhiWeight = 20;
    private double smallSectorWeight = 74;
    private double largeSectorWeight = 82;
    private double minAllocation = 0.1;

    // Dictionaries from market calculations and sector sentiments.
    private Dictionary<string, PercentReturnsModel> marketPercents;
    private Dictionary<string, SentimentsModel> sentimentDict;

    // Constructor. The parameter daysStartRun is expected to be a DateTime.
    public FitnessSectorCalculations(Tuple<string, string> quarterToPredict, double day30Weight, double day90Weight, double day365Weight, double day1095Weight
    , double sentimentWeight, double bullvsBearWeight, double riskWeight, double hhiWeight, double smallSectorWeight, double largeSectorWeight
    , double minAllocation)
    {
        this.day30Weight = day30Weight;
        this.day90Weight = day90Weight;
        this.day365Weight = day365Weight;
        this.day1095Weight = day1095Weight;
        this.sentimentWeight = sentimentWeight;
        this.bullvsBearWeight = bullvsBearWeight;
        this.sortinoRiskWeight = riskWeight;
        this.hhiWeight = hhiWeight;
        this.smallSectorWeight = smallSectorWeight;
        this.largeSectorWeight = largeSectorWeight;
        this.minAllocation = minAllocation;

        
        DateTime daysStartRun = DateAdjuster.AdjustWeekendAndHolidays(DateAdjuster.QuarterStartDate(quarterToPredict));
        // Assume daysStartRun is already adjusted using a method similar to adjustWeekendAndHolidays.
        marketCalculations = new MarketCalculations(daysStartRun);
        sectorSentiments = new SectorSentimentsCalculations(quarterToPredict);

        marketPercents = marketCalculations.MarketPercents();
        sentimentDict = sectorSentiments.SectorSentiments();
    }

    // Calculate Herfindahl–Hirschman Index (HHI) for a given set of allocations.
    public double CalculateHHI(List<double> allocations)
    {
        return allocations.Sum(w => Math.Pow(w, 2));
    }

    // Calculate the Sortino ratio.
    public double CalculateSortino(double portfolioReturn, List<double> sectorReturns, double targetReturn)
    {
        // Get the downside returns: values below the target.
        var downsideReturns = sectorReturns.Where(r => r < targetReturn).ToList();

        double downsideDeviation = 0.0;
        if (downsideReturns.Count > 0)
        {
            double meanSquaredDiff = downsideReturns
                .Select(r => Math.Pow(r - targetReturn, 2))
                .Average();
            downsideDeviation = Math.Sqrt(meanSquaredDiff);
        }

        // Prevent division by zero.
        if (downsideDeviation == 0)
        {
            return double.PositiveInfinity; // Indicates no downside risk.
        }

        return (portfolioReturn - targetReturn) / downsideDeviation;
    }

    public double CalculateSortinoScore(SectorAllocationModel sectorAllocation) {
        List<double> sectorReturns = marketPercents.Select(kvp => kvp.Value.Day1095Percent).ToList();
        double minAvgReturn = sectorReturns.Average();

        // Calculate portfolio return as a weighted sum over the sectors.
        double portfolioReturn = 0.0;
        for (int i = 0; i < marketCalculations.SectorNames.Count; i++)
        {
            string sectorName = marketCalculations.SectorNames[i];
            portfolioReturn += marketPercents[sectorName].Day1095Percent * sectorAllocation.GetAllocations()[i];
        }

        double sortino = CalculateSortino(portfolioReturn, sectorReturns, minAvgReturn);
        double sortinoScore = sortino * sortinoRiskWeight;
        return sortinoScore;
    }
    
    
    // Evaluate a sector allocation's fitness.
    // The sectorAllocation parameter is assumed to be an instance of SectorAllocationModel,
    // which contains a list "allocations", a fitness value "fitness", and an accessible list of sector names.
    public void Evaluate(SectorAllocationModel sectorAllocation)
    {
        // Reset fitness before evaluation.
        sectorAllocation.Fitness = 0.0;

        // Loop through each sector.
        for (int sectorIndex = 0; sectorIndex < marketCalculations.SectorNames.Count; sectorIndex++)
        {
            string name = marketCalculations.SectorNames[sectorIndex];
            double allocation = sectorAllocation.GetAllocations()[sectorIndex];

            double day30Score = marketPercents[name].Day30Percent * day30Weight * allocation;
            double day90Score = marketPercents[name].Day90Percent * day90Weight * allocation;
            double day365Score = marketPercents[name].Day365Percent * day365Weight * allocation;
            double day1095Score = marketPercents[name].Day1095Percent * day1095Weight * allocation;

            double lastQuarterSentimentScore = sentimentDict[name].LastQuarter.SentimentScore * sentimentWeight * allocation;

            // bullGood is 1 if the sector is cyclical; otherwise, -1.
            int bullGood = CyclicalSectors.Contains(name) ? 1 : -1;
            double marketCyclicalVsDefensiveScore = sentimentDict["General"].LastQuarter.SentimentScore * bullvsBearWeight * bullGood * allocation;

            double largeSectorScore = Math.Pow(allocation, 2) * largeSectorWeight * -1; // Punish too large allocations.
            double smallSectorScore = Math.Pow(Math.Max(minAllocation - allocation, 0), 2) * smallSectorWeight * -1;

            sectorAllocation.Fitness += day30Score + day90Score + day365Score + day1095Score + lastQuarterSentimentScore + marketCyclicalVsDefensiveScore + largeSectorScore + smallSectorScore;
        }

        // Calculate HHI risk score.
        double hhiRiskScore = CalculateHHI(sectorAllocation.GetAllocations()) * hhiWeight * -1;
        double sortinoScore = CalculateSortinoScore(sectorAllocation);
        

        sectorAllocation.Fitness += sortinoScore + hhiRiskScore;
    }
}

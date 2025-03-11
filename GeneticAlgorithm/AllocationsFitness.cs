public class AllocationsFitness {
    public static readonly HashSet<string> DefensiveSectors = new HashSet<string> {
        "Healthcare", "Communication Services", "Consumer Defensive", "Energy", "Real Estate", "Utilities"
    };

    public static readonly HashSet<string> CyclicalSectors = new HashSet<string> {
        "Technology", "Financial Services", "Consumer Cyclical", "Industrials", "Real Estate", "Basic Materials"
    };

    private readonly double day30Weight;
    private readonly double day90Weight;
    private readonly double day365Weight;
    private readonly double day1095Weight;
    private readonly double sentimentWeight;
    private readonly double bullvsBearWeight;
    private readonly double largeSectorWeight;
    private readonly double smallSectorWeight;
    private readonly double hhiWeight;
    private readonly double sortinoRiskWeight;
    private readonly double minAllocation;
    private readonly Dictionary<string, SectorSentimentModel> marketSentiments;
    private readonly Dictionary<string, SectorPercentReturnsModel> marketPercentReturns;
    private readonly List<string> sectorNames;
    public AllocationsFitness(List<double> fitnessWeights, Dictionary<string, SectorPercentReturnsModel> marketPercentReturns, Dictionary<string, SectorSentimentModel> marketSentiments) {
        day30Weight = fitnessWeights[0];
        day90Weight = fitnessWeights[1];
        day365Weight = fitnessWeights[2];
        day1095Weight = fitnessWeights[3];
        sentimentWeight = fitnessWeights[4];
        bullvsBearWeight = fitnessWeights[5];
        largeSectorWeight = fitnessWeights[6];
        smallSectorWeight = fitnessWeights[7];
        hhiWeight = fitnessWeights[8];
        sortinoRiskWeight = fitnessWeights[9];
        minAllocation = fitnessWeights[10];

        this.marketPercentReturns = marketPercentReturns;
        this.marketSentiments = marketSentiments;

        sectorNames = marketPercentReturns.Keys.ToList();
    }

    public void EvaluateIndividualFitness(SectorAllocationModel individual) {
        individual.Fitness = 0.0;

        // Loop through each sector.
        for (int sectorIndex = 0; sectorIndex < sectorNames.Count; sectorIndex++)
        {
            string name = sectorNames[sectorIndex];
            double allocation = individual.GetAllocations()[sectorIndex];

            double day30Score = marketPercentReturns[name].Day30Percent * day30Weight * allocation;
            double day90Score = marketPercentReturns[name].Day90Percent * day90Weight * allocation;
            double day365Score = marketPercentReturns[name].Day365Percent * day365Weight * allocation;
            double day1095Score = marketPercentReturns[name].Day1095Percent * day1095Weight * allocation;

            double lastQuarterSentimentScore = marketSentiments[name].LastQuarter.SentimentScore * sentimentWeight * allocation;

            // bullGood is 1 if the sector is cyclical; otherwise, -1.
            int bullGood = CyclicalSectors.Contains(name) ? 1 : -1;
            double marketCyclicalVsDefensiveScore = marketSentiments["General"].LastQuarter.SentimentScore * bullvsBearWeight * bullGood * allocation;

            double largeSectorScore = Math.Pow(allocation, 2) * largeSectorWeight * -1; // Punish too large allocations.
            double smallSectorScore = Math.Pow(Math.Max(minAllocation - allocation, 0), 2) * smallSectorWeight * -1;

            individual.Fitness += day30Score + day90Score + day365Score + day1095Score + lastQuarterSentimentScore + marketCyclicalVsDefensiveScore + largeSectorScore + smallSectorScore;
        }

        // Calculate HHI risk score.
        double hhiRiskScore = CalculateHHI(individual.GetAllocations()) * hhiWeight * -1;
        double sortinoScore = CalculateSortinoScore(individual);
        

        individual.Fitness += sortinoScore + hhiRiskScore;
    }

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

        if (downsideDeviation == 0)
        {
            return double.PositiveInfinity; // Indicates no downside risk.
        }

        return (portfolioReturn - targetReturn) / downsideDeviation;
    }

    public double CalculateSortinoScore(SectorAllocationModel sectorAllocation) {
        List<double> sectorReturns = marketPercentReturns.Select(kvp => kvp.Value.Day1095Percent).ToList();
        double minAvgReturn = sectorReturns.Average();

        // Calculate portfolio return as a weighted sum over the sectors.
        double portfolioReturn = 0.0;
        for (int i = 0; i < sectorNames.Count; i++)
        {
            string sectorName = sectorNames[i];
            portfolioReturn += marketPercentReturns[sectorName].Day1095Percent * sectorAllocation.GetAllocations()[i];
        }

        double sortino = CalculateSortino(portfolioReturn, sectorReturns, minAvgReturn);
        double sortinoScore = sortino * sortinoRiskWeight;
        return sortinoScore;
    }
}
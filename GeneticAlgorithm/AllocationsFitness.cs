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
    private readonly double quarterSentimentWeight;
    private readonly double bullvsBearWeight;
    private readonly double largeSectorWeight;
    private readonly double smallSectorWeight;
    private readonly double hhiWeight;
    private readonly double sortinoRiskWeight;
    private readonly double minAllocation;
    private readonly double yearSentimentWeight;
    private readonly double RSIWeight;
    private readonly double ATRWeight;
    private readonly double ADLPercentWeight;
    private readonly double UnevenAllocationWeight;
    private readonly Dictionary<string, SectorSentimentModel> marketSentiments;
    private readonly Dictionary<string, SectorTechnicalMetricModel> marketPercentReturns;
    private readonly List<string> sectorNames;
    public AllocationsFitness(List<double> fitnessWeights, Dictionary<string, SectorTechnicalMetricModel> marketPercentReturns, Dictionary<string, SectorSentimentModel> marketSentiments) {
        day30Weight = fitnessWeights[0];
        day90Weight = fitnessWeights[1];
        day365Weight = fitnessWeights[2];
        day1095Weight = fitnessWeights[3];
        quarterSentimentWeight = fitnessWeights[4];
        bullvsBearWeight = fitnessWeights[5] * 40; //Needs higher value
        largeSectorWeight = fitnessWeights[6] * 40; //Needs higher value
        smallSectorWeight = fitnessWeights[7] * 40; //Needs higher value
        hhiWeight = fitnessWeights[8] * 10; //Needs medium value
        sortinoRiskWeight = fitnessWeights[9] * 5; //Needs slightly higher value
        minAllocation = fitnessWeights[10];
        yearSentimentWeight = fitnessWeights[11];
        RSIWeight = fitnessWeights[12] * 5; //Needs slightly higher value
        ATRWeight = fitnessWeights[13] * 9; //Needs slightly higher value
        ADLPercentWeight = fitnessWeights[14] * 40; //Needs slightly higher value
        UnevenAllocationWeight = fitnessWeights[15] * 40; //Needs slightly higher value

        // Initialize the market percent returns and sentiments.

        this.marketPercentReturns = marketPercentReturns;
        this.marketSentiments = marketSentiments;

        sectorNames = marketPercentReturns.Keys.ToList();
    }

    public void EvaluateIndividualFitness(SectorAllocationModel individual) {
        individual.Fitness = 0.0;
        var allocations = individual.GetAllocations();
        double generalSentimentScore = marketSentiments["General"].LastQuarter.SentimentScore;


        // Loop through each sector.
        for (int sectorIndex = 0; sectorIndex < sectorNames.Count; sectorIndex++)
        {
            string name = sectorNames[sectorIndex];
            double allocation = allocations[sectorIndex];
            var percentReturn = marketPercentReturns[name];
            var sentiment = marketSentiments[name];


            double day30Score = percentReturn.Day30Percent * day30Weight * allocation;
            double day90Score = percentReturn.Day90Percent * day90Weight * allocation;
            double day365Score = percentReturn.Day365Percent * day365Weight * allocation;
            double day1095Score = percentReturn.Day1095Percent * day1095Weight * allocation;
            double rsiScore = percentReturn.RSI30 * RSIWeight * allocation;
            double atrScore = percentReturn.ATR90 * ATRWeight * allocation * -1; // Punish high ATR.
            double adlScore = percentReturn.ADLChange30 * ADLPercentWeight * allocation;

            double lastQuarterSentimentScore = sentiment.LastQuarter.SentimentScore * quarterSentimentWeight * allocation;
            double lastYearSentimentScore = sentiment.LastYear.SentimentScore * yearSentimentWeight * allocation;

            // bullGood is 1 if the sector is cyclical; otherwise, -1.
            int bullGood = CyclicalSectors.Contains(name) ? 1 : -1;
            double marketCyclicalVsDefensiveScore = generalSentimentScore * bullvsBearWeight * bullGood * allocation;

            double largeSectorScore = allocation * allocation * largeSectorWeight * -1; // Punish too large allocations.
            double unevenallocationScore = Math.Abs(allocation - 9.09) * -1 * UnevenAllocationWeight; // Punish allocations away from average
            double allocationDiff = Math.Max(minAllocation - allocation, 0);
            double smallSectorScore = allocationDiff * allocationDiff * smallSectorWeight * -1;

            individual.Fitness += day30Score + day90Score + day365Score + day1095Score + lastQuarterSentimentScore + lastYearSentimentScore + marketCyclicalVsDefensiveScore + largeSectorScore + smallSectorScore + rsiScore + atrScore + adlScore + unevenallocationScore;
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
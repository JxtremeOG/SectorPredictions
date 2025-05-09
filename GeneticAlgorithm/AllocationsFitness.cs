public class AllocationsFitness {
    public static readonly HashSet<string> DefensiveSectors = new HashSet<string> {
        "Healthcare", "Communication Services", "Consumer Defensive", "Energy", "Real Estate", "Utilities"
    };

    public static readonly HashSet<string> CyclicalSectors = new HashSet<string> {
        "Technology", "Financial Services", "Consumer Cyclical", "Industrials", "Real Estate", "Basic Materials"
    };

    public Dictionary<string, double> SectorsMarketWeight { get; set; } = new Dictionary<string, double>
    {
        { "Technology", 27.87 },
        { "Financial Services", 15.87 },
        { "Consumer Cyclical", 10.76 },
        { "Healthcare", 10.26 },
        { "Communication Services", 9.21 },
        { "Industrials", 8.38 },
        { "Consumer Defensive", 5.5 },
        { "Energy", 4.76 },
        { "Basic Materials", 2.51 },
        { "Real Estate", 2.54 },
        { "Utilities", 2.34 }
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
    private readonly double RSI30DayWeight;
    private readonly double ATRWeight;
    private readonly double ADL30DayPercentWeight;
    private readonly double UnevenAllocationWeight;
    private readonly double ADL90DayPercentWeight;
    private readonly double RSI90DayWeight;
    private readonly double quarterToYearRatioWeight;
    private readonly double RSI30to90DayRatioWeight;
    private readonly double ADL30to90DayRatioWeight;
    private readonly double percent30to90DayRatioWeight;
    private readonly double ATR90ToPercentChangeWeight;
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
        RSI30DayWeight = fitnessWeights[12] * 5; //Needs slightly higher value
        ATRWeight = fitnessWeights[13] * 9; //Needs slightly higher value
        ADL30DayPercentWeight = fitnessWeights[14] * 40; //Needs slightly higher value
        UnevenAllocationWeight = fitnessWeights[15] * 40; //Needs slightly higher value

        ADL90DayPercentWeight = fitnessWeights[16] * 40;
        RSI90DayWeight = fitnessWeights[17] * 5;
        quarterToYearRatioWeight = fitnessWeights[18];
        RSI30to90DayRatioWeight = fitnessWeights[19] * 5;
        ADL30to90DayRatioWeight = fitnessWeights[20] * 40;
        percent30to90DayRatioWeight = fitnessWeights[21];
        ATR90ToPercentChangeWeight = fitnessWeights[22];

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
            double day30to90Score = percentReturn.Day30Percent / percentReturn.Day90Percent * percent30to90DayRatioWeight * allocation;

            double rsi30Score = percentReturn.RSI30 * RSI30DayWeight * allocation;
            double atr90Score = percentReturn.ATR90 * ATRWeight * allocation;
            double adl30Score = percentReturn.ADLChange30 * ADL30DayPercentWeight * allocation;
            double rsi90Score = percentReturn.RSI90 * RSI90DayWeight * allocation;
            double adl90Score = percentReturn.ADLChange90 * ADL90DayPercentWeight * allocation;
            double adl30to90Score = percentReturn.ADLChange30 / percentReturn.ADLChange90 * ADL30to90DayRatioWeight * allocation;
            double rsi30to90Score = percentReturn.RSI30 / percentReturn.RSI90 * RSI30to90DayRatioWeight * allocation;
            double atr90toPercentChangeScore = percentReturn.ATR90 / percentReturn.Day90Percent * ATR90ToPercentChangeWeight * allocation;

            double lastQuarterSentimentScore = sentiment.LastQuarter.SentimentScore * quarterSentimentWeight * allocation;
            double lastYearSentimentScore = sentiment.LastYear.SentimentScore * yearSentimentWeight * allocation;
            double lastQuarterToYearRatio = sentiment.LastQuarter.SentimentScore / sentiment.LastYear.SentimentScore * quarterToYearRatioWeight * allocation;

            // bullGood is 1 if the sector is cyclical; otherwise, -1.
            int bullGood = CyclicalSectors.Contains(name) ? 1 : -1;
            double marketCyclicalVsDefensiveScore = generalSentimentScore * bullvsBearWeight * bullGood * allocation;

            double largeSectorScore = allocation * allocation * largeSectorWeight; // Punish too large allocations.
            double unevenallocationScore = Math.Abs(allocation - SectorsMarketWeight[name]/100) * UnevenAllocationWeight; // Punish allocations away from average
            double allocationDiff = Math.Max(minAllocation - allocation, 0);
            double smallSectorScore = Math.Sqrt(allocationDiff) * smallSectorWeight;

            individual.Fitness += day30Score + day90Score + day365Score + day1095Score + lastQuarterSentimentScore + 
            lastYearSentimentScore + marketCyclicalVsDefensiveScore + largeSectorScore + smallSectorScore + rsi30Score + 
            atr90Score + adl30Score + unevenallocationScore + rsi90Score + adl90Score + day30to90Score + adl30to90Score +
            rsi30to90Score + lastQuarterToYearRatio + atr90toPercentChangeScore;
        }

        // Calculate HHI risk score.
        double hhiRiskScore = CalculateHHI(allocations) * hhiWeight;
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
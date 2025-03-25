public class SectorsFitness {
    private readonly int tunePopulation;
    private readonly double tuneMutation;
    private readonly int tuneImmigrantCount;
    private readonly int tuneGenerationCount;
    private readonly QuarterRangeRecord rangeStart;
    private readonly QuarterRangeRecord rangeEnd;
    private readonly List<QuarterRangeRecord> testingQuarters;
    private readonly List<QuarterRangeRecord> quarterRanges;
    private Dictionary<string, Tuple<Dictionary<string, SectorTechnicalMetricModel>, Dictionary<string, SectorSentimentModel>>> trainingData = 
        new Dictionary<string, Tuple<Dictionary<string, SectorTechnicalMetricModel>, Dictionary<string, SectorSentimentModel>>>();
    private Dictionary<string, MarketSectorResultModel> marketResults = 
        new Dictionary<string, MarketSectorResultModel>();
    public SectorsFitness(int tunePopulation, double tuneMutation, int tuneImmigrantCount, int tuneGenerationCount,
        QuarterRangeRecord rangeStart, QuarterRangeRecord rangeEnd, List<QuarterRangeRecord> testingQuarters) {
        this.tunePopulation = tunePopulation;
        this.tuneMutation = tuneMutation;
        this.tuneImmigrantCount = tuneImmigrantCount;
        this.tuneGenerationCount = tuneGenerationCount;
        this.rangeStart = rangeStart;
        this.rangeEnd = rangeEnd;
        this.testingQuarters = testingQuarters;
        this.quarterRanges = DateAdjuster.GetQuarterRange(rangeStart, rangeEnd);

        PreCalculateTrainingData();
    }

    private void PreCalculateTrainingData() {
        foreach (QuarterRangeRecord quarter in this.quarterRanges) {
            Tuple<string, string> quarterToPredict = new Tuple<string, string>(quarter.Quarter, quarter.Year);
            DateTime daysStartRun = DateAdjuster.AdjustWeekendAndHolidays(DateAdjuster.QuarterStartDate(quarterToPredict));

            MarketCalculations marketCalculations = new MarketCalculations(daysStartRun);
            SectorSentimentsCalculations sectorSentiments = new SectorSentimentsCalculations(quarterToPredict);

            Dictionary<string, SectorTechnicalMetricModel> marketPercents = marketCalculations.MarketTechnicalIndicators();
            Dictionary<string, SectorSentimentModel> sentimentDict = sectorSentiments.SectorSentiments();

            trainingData.Add(quarter.Quarter.ToString()+quarter.Year.ToString(), 
            new Tuple<Dictionary<string, SectorTechnicalMetricModel>, Dictionary<string, SectorSentimentModel>>(marketPercents, sentimentDict));

            marketResults.Add(quarter.Quarter.ToString()+quarter.Year.ToString(), marketCalculations.GetLastQuarterReturns(quarterToPredict));
        }
    }

    public void EvaluateIndividualFitness(SectorsTuneModel individual) {
        individual.Fitness = 0.0;
        QuarterRangeRecord lastQuarter = quarterRanges.Last();
        List<double> parameterWeights = individual.GetParameters();

        foreach (QuarterRangeRecord quarter in this.quarterRanges) {
            if (testingQuarters.Contains(quarter)) { // skip testing quarters
                continue;
            }

            string key = quarter.Quarter.ToString() + quarter.Year.ToString();

            Dictionary<string, SectorTechnicalMetricModel> marketPercents = trainingData[key].Item1; // Dictionary<string, SectorPercentReturnsModel>
            Dictionary<string, SectorSentimentModel> sentimentDict = trainingData[key].Item2; // Dictionary<string, SectorSentimentModel>
            
            AllocationsFitness allocationsFitness = new AllocationsFitness(parameterWeights, marketPercents, sentimentDict);

            AllocationsAlgorithm allocationAlgorithm = new AllocationsAlgorithm(allocationsFitness);

            GeneticAlgorithmBase<SectorAllocationModel> allocationGeneticAlgorithm = new GeneticAlgorithmBase<SectorAllocationModel>(
                tunePopulation, tuneMutation, tuneImmigrantCount, tuneGenerationCount, allocationAlgorithm.CreateRandomIndividual, allocationAlgorithm.EvaluateIndividual, 
                allocationAlgorithm.Crossover, allocationAlgorithm.Mutate
            );

            SectorAllocationModel bestAllocation = allocationGeneticAlgorithm.Run().Result;

            MarketSectorResultModel quarterResults = marketResults[key];

            double fitness = 0;
            foreach (var allocation in bestAllocation.ToDict())
            {
                switch (allocation.Key) {
                    case "Technology":
                        fitness += quarterResults.TechnologyQuarterResult * allocation.Value;
                        break;
                    case "Financial Services":
                        fitness += quarterResults.FinancialServicesQuarterResult * allocation.Value;
                        break;
                    case "Consumer Cyclical":
                        fitness += quarterResults.ConsumerCyclicalQuarterResult * allocation.Value;
                        break;
                    case "Healthcare":
                        fitness += quarterResults.HealthcareQuarterResult * allocation.Value;
                        break;
                    case "Communication Services":
                        fitness += quarterResults.CommunicationServicesQuarterResult * allocation.Value;
                        break;
                    case "Industrials":
                        fitness += quarterResults.IndustrialsQuarterResult * allocation.Value;
                        break;
                    case "Consumer Defensive":
                        fitness += quarterResults.ConsumerDefensiveQuarterResult * allocation.Value;
                        break;
                    case "Energy":
                        fitness += quarterResults.EnergyQuarterResult * allocation.Value;
                        break;
                    case "Basic Materials":
                        fitness += quarterResults.BasicMaterialsQuarterResult * allocation.Value;
                        break;
                    case "Real Estate":
                        fitness += quarterResults.RealEstateQuarterResult * allocation.Value;
                        break;
                    case "Utilities":
                        fitness += quarterResults.UtilitiesQuarterResult * allocation.Value;
                        break;
                }
            }
            if (fitness < 0) {
                fitness = fitness * 3;
            }
            individual.Fitness += fitness;
            if (quarter == lastQuarter) {
                Console.WriteLine($"SectorAllocationModel | {quarter.Quarter} {quarter.Year} | Individual Fitness: {individual.Fitness}");
            }
        }
        // Console.WriteLine($"SectorsTuneModel | Fitness: {individual.Fitness}");
    }
}
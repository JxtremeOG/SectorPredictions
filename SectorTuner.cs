public class SectorTuner {

    private int tunePopulation = 500; //500
    private double tuneMutation = 0.2;
    private int tuneImmigrantCount = 10;
    private int tuneGenerationCount = 500; //250
    private int population = 100; 
    private double mutation = 0.2;
    private int immigrantCount = 4;
    private int generationCount = 100; //100
    private QuarterRangeRecord rangeStart = new QuarterRangeRecord { Quarter = "Q1", Year = "2021" }; //inclusive
    private QuarterRangeRecord rangeEnd = new QuarterRangeRecord { Quarter = "Q4", Year = "2024" }; //inclusive
    List<QuarterRangeRecord> testingQuarters = new List<QuarterRangeRecord> {
        new QuarterRangeRecord { Quarter = "Q4", Year = "2021" },
        new QuarterRangeRecord { Quarter = "Q3", Year = "2022" },
        new QuarterRangeRecord { Quarter = "Q2", Year = "2023" },
        new QuarterRangeRecord { Quarter = "Q1", Year = "2024" }
    };
    GeneticAlgorithmBase<SectorsTuneModel> sectorGeneticAlgorithm;

    public SectorTuner() {
        SectorsFitness sectorsFitness = new SectorsFitness(tunePopulation, tuneMutation, tuneImmigrantCount, tuneGenerationCount, rangeStart, rangeEnd, testingQuarters);
        SectorAlgorithm sectorAlgorithm = new SectorAlgorithm(sectorsFitness);
        sectorGeneticAlgorithm = new GeneticAlgorithmBase<SectorsTuneModel>(
            population, mutation, immigrantCount, generationCount, sectorAlgorithm.CreateRandomIndividual, sectorAlgorithm.EvaluateIndividual, 
            sectorAlgorithm.Crossover, sectorAlgorithm.Mutate);
    }

    public void Tune() {
        SectorsTuneModel bestWeights = sectorGeneticAlgorithm.Run().Result;
        foreach (var weight in bestWeights.ToDict())
        {
            Console.WriteLine($"{weight.Key}: {weight.Value}");
        }
    }
    
}
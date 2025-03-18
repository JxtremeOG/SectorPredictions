public class SectorTuner {

    private int tunePopulation = 1000;
    private double tuneMutation = 0.4;
    private int tuneImmigrantCount = 20;
    private int tuneGenerationCount = 600; 
    private int population = 1000;
    private double mutation = 0.4;
    private int immigrantCount = 20;
    private int generationCount = 600;
    private QuarterRangeRecord rangeStart = new QuarterRangeRecord { Quarter = "Q1", Year = "2020" }; //inclusive
    private QuarterRangeRecord rangeEnd = new QuarterRangeRecord { Quarter = "Q4", Year = "2024" }; //inclusive
    List<QuarterRangeRecord> testingQuarters = new List<QuarterRangeRecord> {
        new QuarterRangeRecord { Quarter = "Q4", Year = "2020" },
        new QuarterRangeRecord { Quarter = "Q3", Year = "2021" },
        new QuarterRangeRecord { Quarter = "Q2", Year = "2022" },
        new QuarterRangeRecord { Quarter = "Q1", Year = "2023" },
        new QuarterRangeRecord { Quarter = "Q3", Year = "2024" }
    };
    public SectorTuner() {

    }

    public void Tune() {
        
    }
    
}
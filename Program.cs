using System.Threading.Tasks;

class Program {
    static async Task Main() {
        int population = 1000;
        double mutation = 0.4;
        int immigrantCount = 20;
        int generationCount = 600;
        Tuple<string, string> quarterToPredict = new Tuple<string, string>( "Q1", "2025" );
        FitnessSectorCalculations sectorFitness = new FitnessSectorCalculations(quarterToPredict,
        day30Weight: 0.6, day90Weight: 0.3, day365Weight: 0.2, day1095Weight: 0.4, sentimentWeight: 0.6, bullvsBearWeight: 100,
        riskWeight: 7, hhiWeight: 20, smallSectorWeight: 74, largeSectorWeight: 82, minAllocation: 0.1);
        AllocationsGeneticAlgorithm geneticAlgorithm = new AllocationsGeneticAlgorithm(population, mutation, immigrantCount, generationCount, sectorFitness);
        Console.WriteLine("Starting Genetic Algorithm...");
        var bestAllocation = geneticAlgorithm.TranGenetically();
        Console.WriteLine("Best Allocation: ");
        foreach (var allocation in bestAllocation.ToDict())
        {
            Console.WriteLine($"{allocation.Key}: {allocation.Value}");
        }

        SQLiteTerminal terminal = new SQLiteTerminal();
        StockNews stockNews = new StockNews();
        StockData stockData = new StockData();
        // terminal.CreateNewsTable();
        // terminal.CreateSectorsTable();
        // terminal.InsertSectorsTable();
        // terminal.ClearNewsTable();
        // terminal.AlterNewsTable();
        // terminal.CreateNewsTable();
        // terminal.CreateStocksDataTable();
        // terminal.ClearStockTables();

        // await stockNews.FetchAndStoreStockSectorNewsAsync();
        // await stockNews.FetchAndStoreStockGeneralNewsAsync();
        // await stockData.FetchAndStoreStockDataAsync();
    }
}
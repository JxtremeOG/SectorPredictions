using System.Threading.Tasks;

class Program {
    static async Task Main() {

        // #region Train Allocations
        // SectorTuner sectorTuner = new SectorTuner();
        // sectorTuner.Tune();
        // #endregion 


        #region Run Sectors
        int population = 5000;
        double mutation = 0.4;
        int immigrantCount = 200;
        int generationCount = 1000;
        Tuple<string, string> quarterToPredict = new Tuple<string, string>( "Q1", "2023" );

        // List<double> allocationFitnessWeights = new List<double> { 0.6, 0.3, 0.2, 0.4, 0.6, 100, 82, 74, 20, 7, 0.1 }; //OLD
        // List<double> allocationFitnessWeights = new List<double> { 2.70, 10.81, 1.69, .47, 2.67, 2.98 * 40, 0.49 * 40, 0.57 * 40, 2.38 * 10, 1.20 * 5, 2.33 }; //NEW
        List<double> allocationFitnessWeights = new List<double> { 1.38, 3.04, 4.07, 0.92, 2.53, 4.21 * 40, 0.37 * 40, 1.64 * 40, 1.34 * 10, 0.57 * 5, 3.4 }; //NEW

        DateTime daysStartRun = DateAdjuster.AdjustWeekendAndHolidays(DateAdjuster.QuarterStartDate(quarterToPredict));
        MarketCalculations marketCalculations = new MarketCalculations(daysStartRun);
        SectorSentimentsCalculations sectorSentiments = new SectorSentimentsCalculations(quarterToPredict);

        Dictionary<string, SectorPercentReturnsModel> marketPercents = marketCalculations.MarketPercents();
        Dictionary<string, SectorSentimentModel> sentimentDict = sectorSentiments.SectorSentiments();

        AllocationsFitness allocationsFitness = new AllocationsFitness(allocationFitnessWeights, marketPercents, sentimentDict);

        AllocationsAlgorithm allocationAlgorithm = new AllocationsAlgorithm(allocationsFitness);

        GeneticAlgorithmBase<SectorAllocationModel> allocationGeneticAlgorithm = new GeneticAlgorithmBase<SectorAllocationModel>(
            population, mutation, immigrantCount, generationCount, allocationAlgorithm.CreateRandomIndividual, allocationAlgorithm.EvaluateIndividual, 
            allocationAlgorithm.Crossover, allocationAlgorithm.Mutate
        );

        SectorAllocationModel bestAllocation = allocationGeneticAlgorithm.Run().Result;
        Console.WriteLine("Best Allocation: ");
        foreach (var allocation in bestAllocation.ToDict())
        {
            Console.WriteLine($"{allocation.Key}: {allocation.Value}");
        }

        Dictionary<string, MarketSectorResultModel> marketResults = 
        new Dictionary<string, MarketSectorResultModel>();

        marketResults.Add(quarterToPredict.Item1+quarterToPredict.Item2, marketCalculations.GetLastQuarterReturns(DateAdjuster.SubtractQuarters(quarterToPredict, -1)));

        MarketSectorResultModel quarterResults = marketResults[quarterToPredict.Item1+quarterToPredict.Item2];

        double totalReturn = 0;
        foreach (var allocation in bestAllocation.ToDict())
        {
            double sectorReturn = 0;
            double quarterResult = 0;

            switch (allocation.Key)
            {
                case "Technology":
                    sectorReturn = quarterResults.TechnologyQuarterResult * allocation.Value;
                    quarterResult = quarterResults.TechnologyQuarterResult;
                    break;
                case "Financial Services":
                    sectorReturn = quarterResults.FinancialServicesQuarterResult * allocation.Value;
                    quarterResult = quarterResults.FinancialServicesQuarterResult;
                    break;
                case "Consumer Cyclical":
                    sectorReturn = quarterResults.ConsumerCyclicalQuarterResult * allocation.Value;
                    quarterResult = quarterResults.ConsumerCyclicalQuarterResult;
                    break;
                case "Healthcare":
                    sectorReturn = quarterResults.HealthcareQuarterResult * allocation.Value;
                    quarterResult = quarterResults.HealthcareQuarterResult;
                    break;
                case "Communication Services":
                    sectorReturn = quarterResults.CommunicationServicesQuarterResult * allocation.Value;
                    quarterResult = quarterResults.CommunicationServicesQuarterResult;
                    break;
                case "Industrials":
                    sectorReturn = quarterResults.IndustrialsQuarterResult * allocation.Value;
                    quarterResult = quarterResults.IndustrialsQuarterResult;
                    break;
                case "Consumer Defensive":
                    sectorReturn = quarterResults.ConsumerDefensiveQuarterResult * allocation.Value;
                    quarterResult = quarterResults.ConsumerDefensiveQuarterResult;
                    break;
                case "Energy":
                    sectorReturn = quarterResults.EnergyQuarterResult * allocation.Value;
                    quarterResult = quarterResults.EnergyQuarterResult;
                    break;
                case "Basic Materials":
                    sectorReturn = quarterResults.BasicMaterialsQuarterResult * allocation.Value;
                    quarterResult = quarterResults.BasicMaterialsQuarterResult;
                    break;
                case "Real Estate":
                    sectorReturn = quarterResults.RealEstateQuarterResult * allocation.Value;
                    quarterResult = quarterResults.RealEstateQuarterResult;
                    break;
                case "Utilities":
                    sectorReturn = quarterResults.UtilitiesQuarterResult * allocation.Value;
                    quarterResult = quarterResults.UtilitiesQuarterResult;
                    break;
            }
            totalReturn += sectorReturn;
            string FormatPercentage(double value) => 
                value == 0 ? " 00.00" : value.ToString("+00.00;-00.00");

            Console.WriteLine($"{quarterToPredict.Item2} {quarterToPredict.Item1} | {allocation.Key.PadRight(22)}: {FormatPercentage(sectorReturn)}% | Actual Change {FormatPercentage(quarterResult)} | Allocation: {FormatPercentage(allocation.Value)}");

            // Console.WriteLine($"Start Date: {startDate.ToString("MM/dd/yyyy")} | Days Back {daysAgo.ToString().PadRight(4)} | Sector: {sector.PadRight(22)} | Start: {startValue.ToString("F2").PadRight(8)}, End: {endValue.ToString("F2").PadRight(8)}, Change: {percentChange.ToString("F2")}%");
        }
        Console.WriteLine($"{quarterToPredict.Item2} {quarterToPredict.Item1} | Total Return: {totalReturn:F2}%");
        #endregion 

        // SQLiteTerminal terminal = new SQLiteTerminal();
        // StockNews stockNews = new StockNews();
        // StockData stockData = new StockData();
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
        // await stockData.FetchAndStoreETFDataAsync();
    }
}
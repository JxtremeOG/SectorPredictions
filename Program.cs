using System.Threading.Tasks;

class Program {
    static async Task Main() {

        #region Train Allocations
        SectorTuner sectorTuner = new SectorTuner();
        sectorTuner.Tune();
        #endregion 


        // #region Run Sectors
        // int population = 5000;
        // double mutation = 0.4;
        // int immigrantCount = 200;
        // int generationCount = 500;
        // Tuple<string, string> quarterToPredict = new Tuple<string, string>( "Q4", "2024" );

        // // List<double> allocationFitnessWeights = new List<double> { 0.6, 0.3, 0.2, 0.4, 0.6, 100 / 40, 82 / 40, 74 / 40, 20 / 10, 7 / 5, 0.1, 0.4, .5, .7, .5 }; //OLD
        // // List<double> allocationFitnessWeights = new List<double> { 2.70, 10.81, 1.69, .47, 2.67, 2.98, 0.49, 0.57, 2.38, 1.20, 2.33 }; //NEW
        // // List<double> allocationFitnessWeights = new List<double> { 1.38, 3.04, 4.07, 0.92, 2.53, 4.21, 0.37, 1.64, 1.34, 0.57, 3.4 }; //NEW
        // // List<double> allocationFitnessWeights = new List<double> { 2.51, 4.22, 3.98, 0.15, 1.47, 3.26, 9.97 /*Large Sector*/, 1.21, 0.83, 2.20, 3.23, 2.78, 0.83 * 2, 0.08 * 2, 1.15 * 9 }; //NEW
        // // List<double> allocationFitnessWeights = new List<double> { 3.7744449165908733, 4.453828757831697, 6.427488466313948, 
        // //  0.5909910407491608, 2.790112097599609, 3.258877401318383, 0.6933470429159871, .8759903180114815, 1.7985536575783176,
        // //  2.1043681525423947, 2.440114587705316, 3.6484864228950458, 0.9453605699745579, 1.3114204706447752, 2.9704314672117875, 
        // //  2.928827322925022}; //NEW
        // //  List<double> allocationFitnessWeights = new List<double> { 1.6255733032614483, 1.4747471644433297, 1.856451046595284,
        // //  0.11038698785295564, 3.178421953931739, 1.2907296957293846, -4.411065590078694, -3.4010020321189103, -2.573858650133109,
        // //  2.030154088589544, 4.66225300887451, 4.062500852748998, 1.5265529541437173, -1.167555615366933, 2.345865810977302,
        // //  -1.634171102464611}; //NEW
        // // List<double> allocationFitnessWeights = new List<double> { 3.67066893534658, 4.738850937400466, 3.967633098301475, 
        // // 0.28364262328214207, 3.4966331171212173, 4.8232948424123325, 4.198825846153785, 4.453598124052118, 1.6897482154385368,
        // // 2.921477493111726, 1.75443090684272, 4.727369853638885, 4.832174674433364, 3.603550532473977, 0.055489242069015066,
        // // -4.834632198815553}; //NEW
        // List<double> allocationFitnessWeights = new List<double> { 2.791662640794344, 0.34979386792142547, 0.049349821914817106,
        // -1.2452334967854526, 2.1134803457957076, 2.221313471582299, 0.10389452691210294, 2.978321617639883, 1.1965399977827724,
        // 0.37631412618329485, 1.1963661380015034, 0.3796327365349573, -0.3178467220728443, 0.7021552583248709, -0.666043681662526,
        // 0.9870615886716908}; //NEW
        // // List<double> allocationFitnessWeights = new List<double> { 
        // //     2.791662640794344, //day30Weight
        // //     0.34979386792142547, //day90Weight
        // //     0.049349821914817106, //day365Weight
        // //     1.2452334967854526 - 0.5, //day1095Weight (made positive) (subtracted 0.5)
        // //     2.1134803457957076, //quarterSentimentWeight
        // //     2.221313471582299, //bullvsBearWeight
        // //     -6.10389452691210294, //largeSectorWeight (added 6) (made negative)
        // //     2.978321617639883, //smallSectorWeight
        // //     -1.1965399977827724, //hhiWeight (Made negative)
        // //     0.37631412618329485, //sortinoRiskWeight
        // //     1.1963661380015034, //minAllocation
        // //     0.3796327365349573, //yearSentimentWeight
        // //     -0.3178467220728443, //RSIWeight
        // //     0.7021552583248709, //ATRWeight
        // //     0.666043681662526, //ADLPercentWeight (made positive)
        // //     -0.9870615886716908 //UnevenAllocationWeight (made negative)
        // // }; //NEW


        // DateTime daysStartRun = DateAdjuster.AdjustWeekendAndHolidays(DateAdjuster.QuarterStartDate(quarterToPredict));
        // MarketCalculations marketCalculations = new MarketCalculations(daysStartRun);
        // SectorSentimentsCalculations sectorSentiments = new SectorSentimentsCalculations(quarterToPredict);

        // Dictionary<string, SectorTechnicalMetricModel> marketPercents = marketCalculations.MarketTechnicalIndicators();
        // Dictionary<string, SectorSentimentModel> sentimentDict = sectorSentiments.SectorSentiments();

        // AllocationsFitness allocationsFitness = new AllocationsFitness(allocationFitnessWeights, marketPercents, sentimentDict);

        // AllocationsAlgorithm allocationAlgorithm = new AllocationsAlgorithm(allocationsFitness);

        // GeneticAlgorithmBase<SectorAllocationModel> allocationGeneticAlgorithm = new GeneticAlgorithmBase<SectorAllocationModel>(
        //     population, mutation, immigrantCount, generationCount, allocationAlgorithm.CreateRandomIndividual, allocationAlgorithm.EvaluateIndividual, 
        //     allocationAlgorithm.Crossover, allocationAlgorithm.Mutate
        // );

        // SectorAllocationModel bestAllocation = allocationGeneticAlgorithm.Run().Result;
        // Console.WriteLine("Best Allocation: ");
        // foreach (var allocation in bestAllocation.ToDict())
        // {
        //     Console.WriteLine($"{allocation.Key}: {allocation.Value}");
        // }

        // Dictionary<string, MarketSectorResultModel> marketResults = 
        // new Dictionary<string, MarketSectorResultModel>();

        // marketResults.Add(quarterToPredict.Item1+quarterToPredict.Item2, marketCalculations.GetLastQuarterReturns(DateAdjuster.SubtractQuarters(quarterToPredict, -1)));

        // MarketSectorResultModel quarterResults = marketResults[quarterToPredict.Item1+quarterToPredict.Item2];

        // double totalReturn = 0;
        // foreach (var allocation in bestAllocation.ToDict())
        // {
        //     double sectorReturn = 0;
        //     double quarterResult = 0;

        //     switch (allocation.Key)
        //     {
        //         case "Technology":
        //             sectorReturn = quarterResults.TechnologyQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.TechnologyQuarterResult;
        //             break;
        //         case "Financial Services":
        //             sectorReturn = quarterResults.FinancialServicesQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.FinancialServicesQuarterResult;
        //             break;
        //         case "Consumer Cyclical":
        //             sectorReturn = quarterResults.ConsumerCyclicalQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.ConsumerCyclicalQuarterResult;
        //             break;
        //         case "Healthcare":
        //             sectorReturn = quarterResults.HealthcareQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.HealthcareQuarterResult;
        //             break;
        //         case "Communication Services":
        //             sectorReturn = quarterResults.CommunicationServicesQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.CommunicationServicesQuarterResult;
        //             break;
        //         case "Industrials":
        //             sectorReturn = quarterResults.IndustrialsQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.IndustrialsQuarterResult;
        //             break;
        //         case "Consumer Defensive":
        //             sectorReturn = quarterResults.ConsumerDefensiveQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.ConsumerDefensiveQuarterResult;
        //             break;
        //         case "Energy":
        //             sectorReturn = quarterResults.EnergyQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.EnergyQuarterResult;
        //             break;
        //         case "Basic Materials":
        //             sectorReturn = quarterResults.BasicMaterialsQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.BasicMaterialsQuarterResult;
        //             break;
        //         case "Real Estate":
        //             sectorReturn = quarterResults.RealEstateQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.RealEstateQuarterResult;
        //             break;
        //         case "Utilities":
        //             sectorReturn = quarterResults.UtilitiesQuarterResult * allocation.Value;
        //             quarterResult = quarterResults.UtilitiesQuarterResult;
        //             break;
        //     }
        //     totalReturn += sectorReturn;
        //     string FormatPercentage(double value) => 
        //         value == 0 ? " 00.00" : value.ToString("+00.00;-00.00");

        //     Console.WriteLine($"{quarterToPredict.Item2} {quarterToPredict.Item1} | {allocation.Key.PadRight(22)}: {FormatPercentage(sectorReturn)}% | Actual Change {FormatPercentage(quarterResult)} | Allocation: {FormatPercentage(allocation.Value)}");

        //     // Console.WriteLine($"Start Date: {startDate.ToString("MM/dd/yyyy")} | Days Back {daysAgo.ToString().PadRight(4)} | Sector: {sector.PadRight(22)} | Start: {startValue.ToString("F2").PadRight(8)}, End: {endValue.ToString("F2").PadRight(8)}, Change: {percentChange.ToString("F2")}%");
        // }
        // Console.WriteLine($"{quarterToPredict.Item2} {quarterToPredict.Item1} | Total Return: {totalReturn:F2}%");
        // #endregion 

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
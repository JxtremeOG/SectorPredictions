using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

public class StockData {
    private SqliteConnection sqlite;
    private string apiKey;
    public DateTime dateStart;
    public DateTime dateEnd;
    public DataReader dataReader;
    public Dictionary<string, string> sectorTables = new Dictionary<string, string>
    {
        { "Technology", "TECHNOLOGY_STOCK_DATA" },
        { "Financial Services", "FINANCIAL_STOCK_DATA" },
        { "Consumer Cyclical", "CYCLICAL_STOCK_DATA" },
        { "Healthcare", "HEALTH_STOCK_DATA" },
        { "Communication Services", "COMMUNICATION_STOCK_DATA" },
        { "Industrials", "INDUSTRIALS_STOCK_DATA" },
        { "Consumer Defensive", "DEFENSIVE_STOCK_DATA" },
        { "Energy", "ENERGY_STOCK_DATA" },
        { "Basic Materials", "BASIC_STOCK_DATA" },
        { "Real Estate", "ESTATE_STOCK_DATA" },
        { "Utilities", "UTILS_STOCK_DATA" },
    };
    public StockData() {
        // Use an absolute path to ensure the correct DB is opened.
        string dbPath = @"C:\Users\zoldw\Projects\Coding\CodeFest2025\AdvancedBackend\MARKETADMIN.db";
        sqlite = new SqliteConnection($"Data Source={dbPath}");
        apiKey = Environment.GetEnvironmentVariable("CODEFEST2025_EOD");
        dateStart = DateTime.ParseExact("01012015", "ddMMyyyy", CultureInfo.InvariantCulture);
        dateEnd = DateTime.ParseExact("31122024", "ddMMyyyy", CultureInfo.InvariantCulture);
        dataReader = new DataReader();
    }

    public async Task FetchAndStoreStockDataAsync() {
        var sectorTickerPairs = dataReader.GetSectorTickerPairs();
        try {
            using (HttpClient client = new HttpClient()) {
                dataReader.OpenConnection();
                dataReader.BeginTransaction();
                foreach (var (sector, ticker) in sectorTickerPairs)
                {
                    if (!sector.Equals("Healthcare"))
                        continue;
                    string url = $"https://eodhd.com/api/eod/{ticker}.US?period=d&api_token={apiKey}&fmt=json&from={dateStart:yyyy-MM-dd}&to={dateEnd:yyyy-MM-dd}";
                    Console.WriteLine($"Fetching URL for general stocks {dateStart} {dateEnd}: {url}");

                    string rawJson = string.Empty;
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Request error for general stocks: {response.StatusCode} ({response.ReasonPhrase})");
                            continue; // Skip to the next ticker
                        }
                        rawJson = await response.Content.ReadAsStringAsync();
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Request error for general stocks: {ex.Message}");
                    }

                    if (string.IsNullOrWhiteSpace(rawJson))
                    {
                        Console.WriteLine("Empty response, skipping ticker.");
                        continue; // Skip if no data was returned.
                    }

                    // Deserialize into our StockNewsModel.
                    List<StockDataModel> stockDataResponse = null;
                    try
                    {
                        stockDataResponse = JsonSerializer.Deserialize<List<StockDataModel>>(rawJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing JSON for general stocks: {ex.Message}");
                    }

                    foreach (var stockData in stockDataResponse)
                    {
                        dataReader.InsertStockData(sectorTables[sector], sector, ticker, stockData);
                    }
                }
                dataReader.CommitTransaction();
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        } finally {
            sqlite.Close();
        }
    }
}
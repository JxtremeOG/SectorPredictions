using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

public class StockNews
{
    private string apiKey;
    public DateTime dateStart;
    public DateTime dateEnd;
    public DataReader dataReader;

    public StockNews()
    {
        apiKey = Environment.GetEnvironmentVariable("CODEFEST2025_STOCKNEWS");
        dateStart = DateTime.ParseExact("01012020", "ddMMyyyy", CultureInfo.InvariantCulture);
        dateEnd = DateTime.ParseExact("31122024", "ddMMyyyy", CultureInfo.InvariantCulture);
        dataReader = new DataReader();
    }

    // Generate a list of quarter ranges between dateStart and dateEnd.
    public List<QuarterRangeRecord> GetQuarterRanges(DateTime start, DateTime end)
    {
        var quarters = new List<QuarterRangeRecord>();

        // Determine the first quarter start date on or after 'start'
        int startMonth = ((start.Month - 1) / 3) * 3 + 1;
        DateTime current = new DateTime(start.Year, startMonth, 1);

        while (current <= end)
        {
            DateTime quarterStart = current;
            DateTime quarterEnd = quarterStart.AddMonths(3).AddDays(-1);
            if (quarterEnd > end)
                quarterEnd = end;

            string quarterLabel = "Q" + ((quarterStart.Month - 1) / 3 + 1).ToString();
            quarters.Add(new QuarterRangeRecord { Start = quarterStart, End = quarterEnd, Quarter = quarterLabel, Year = quarterStart.Year.ToString() });

            current = quarterStart.AddMonths(3);
        }

        return quarters;
    }

    public async Task FetchAndStoreStockGeneralNewsAsync() {
        try {
            var quarterRanges = GetQuarterRanges(dateStart, dateEnd);
            using (HttpClient client = new HttpClient()) {
                foreach (var quarter in quarterRanges) {
                    // Construct API request URL using the quarter date range.
                    string url = $"https://stocknewsapi.com/api/v1/stat?section=general&date={quarter.Start:ddMMyyyy}-{quarter.End:ddMMyyyy}&page=1&token={apiKey}";
                    Console.WriteLine($"Fetching URL for general {quarter.Quarter} {quarter.Year}: {url}");

                    string rawJson = string.Empty;
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode(); // Throws if status code is not 2xx
                        rawJson = await response.Content.ReadAsStringAsync();
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Request error for general: {ex.Message}");
                        continue;
                    }

                    // Deserialize into our StockNewsModel.
                    StockNewsModel stockNewsResponse = null;
                    try
                    {
                        stockNewsResponse = JsonSerializer.Deserialize<StockNewsModel>(rawJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing JSON for general: {ex.Message}");
                        continue;
                    }

                    // Process only the "total" data.
                    if (stockNewsResponse.total.ValueKind == JsonValueKind.Object)
                    {
                        // Deserialize "total" as a dictionary where key = ticker and value = dictionary of sentiment metrics.
                        var totalValues = JsonSerializer.Deserialize<Dictionary<string, double>>(stockNewsResponse.total.GetRawText());

                        if (totalValues == null)
                        {
                            Console.WriteLine($"No total values for general.");
                            continue;
                        }

                        Console.WriteLine($"Processing total for general for {quarter.Quarter} {quarter.Year}");

                        dataReader.InsertNewsData("General", null, quarter, totalValues);
                    }
                    else
                    {
                        Console.WriteLine($"No total data object returned for general.");
                    }
                } // End quarter loop
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }



    public async Task FetchAndStoreStockSectorNewsAsync() {
        try {
            // Get the ticker groups per sector (two groups per sector).
            var sectorTickers = dataReader.GetSectorTickerStrings();
            // Generate all quarter ranges between dateStart and dateEnd.
            var quarterRanges = GetQuarterRanges(dateStart, dateEnd);

            using (HttpClient client = new HttpClient())
            {
                // For each sector group (e.g., "Technology1", "Technology2")
                foreach (var kvp in sectorTickers)
                {
                    string sectorKey = kvp.Key; // e.g., "Technology1" or "Technology2"
                    sectorKey = Regex.Replace(sectorKey, @"\d+$", "");
                    // Extract the sector name (non-digit prefix)
                    string sector = new string(sectorKey.TakeWhile(c => !char.IsDigit(c)).ToArray());
                    // The tickerList is a comma-separated list of 50 tickers.
                    string tickerList = kvp.Value;

                    // For each quarter, fetch total data.
                    foreach (var quarter in quarterRanges)
                    {
                        // Construct API request URL using the quarter date range.
                        string url = $"https://stocknewsapi.com/api/v1/stat?tickers={tickerList}&date={quarter.Start:ddMMyyyy}-{quarter.End:ddMMyyyy}&page=1&token={apiKey}";
                        Console.WriteLine($"Fetching URL for {sector} {quarter.Quarter} {quarter.Year}: {url}");

                        string rawJson = string.Empty;
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(url);
                            response.EnsureSuccessStatusCode(); // Throws if status code is not 2xx
                            rawJson = await response.Content.ReadAsStringAsync();
                        }
                        catch (HttpRequestException ex)
                        {
                            Console.WriteLine($"Request error for tickers {tickerList}: {ex.Message}");
                            continue;
                        }

                        // Deserialize into our StockNewsModel.
                        StockNewsModel stockNewsResponse = null;
                        try
                        {
                            stockNewsResponse = JsonSerializer.Deserialize<StockNewsModel>(rawJson);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deserializing JSON for tickers {tickerList}: {ex.Message}");
                            continue;
                        }

                        // Process only the "total" data.
                        if (stockNewsResponse.total.ValueKind == JsonValueKind.Object)
                        {
                            // Deserialize "total" as a dictionary where key = ticker and value = dictionary of sentiment metrics.
                            var totalValues = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(stockNewsResponse.total.GetRawText());

                            if (totalValues == null)
                            {
                                Console.WriteLine($"No total values for tickers {tickerList}.");
                                continue;
                            }

                            // Split the comma-separated tickers into individual tickers.
                            var individualTickers = tickerList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var individualTicker in individualTickers)
                            {
                                if (totalValues.ContainsKey(individualTicker))
                                {
                                    var stockSentiment = totalValues[individualTicker];
                                    Console.WriteLine($"Processing total for {individualTicker} for {quarter.Quarter} {quarter.Year}");

                                    dataReader.InsertNewsData(sector, individualTicker, quarter, stockSentiment);
                                }
                                else
                                {
                                    Console.WriteLine($"Ticker {individualTicker} not found in total data for {quarter.Quarter} {quarter.Year}.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No total data object returned for tickers {tickerList}.");
                        }
                    } // End quarter loop
                } // End sectorTicker loop
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database error: " + ex.Message);
        }
    }
}

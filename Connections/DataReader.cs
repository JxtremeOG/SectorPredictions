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

public class DataReader {
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
    private SqliteConnection sqlite;
    private SqliteTransaction transaction;
    public DataReader() {
        string dbPath = @"C:\Users\zoldw\Projects\Coding\CodeFest2025\AdvancedBackend\MARKETADMIN.db";
        sqlite = new SqliteConnection($"Data Source={dbPath}");
    }

    public void OpenConnection()
    {
        if (sqlite.State != System.Data.ConnectionState.Open)
        {
            sqlite.Open();
        }
    }
    public void BeginTransaction()
    {
        OpenConnection();
        transaction = sqlite.BeginTransaction();
    }

    // Commits the current transaction.
    public void CommitTransaction()
    {
        transaction?.Commit();
        transaction = null;
    }

    // Rolls back the current transaction.
    public void RollbackTransaction()
    {
        transaction?.Rollback();
        transaction = null;
    }

    // Closes the connection.
    public void CloseConnection()
    {
        if (sqlite.State != System.Data.ConnectionState.Closed)
        {
            sqlite.Close();
        }
    }

    public List<(string sector, string ticker)> GetSectorTickerPairs()
    {
        List<(string sector, string ticker)> sectorTickerPairs = new List<(string, string)>();

        sqlite.Open();
        using (var command = new SqliteCommand("SELECT SECTOR, TICKER FROM SECTOR_STOCKS", sqlite))
        {
            try
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sectorTickerPairs.Add((reader.GetString(0), reader.GetString(1)));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing reader: " + ex.Message);
                throw;  // rethrow after logging
            }
        }
        sqlite.Close();

        return sectorTickerPairs;
    }

    // This method groups tickers into two sets of 50 per sector and returns 22 dictionary entries (e.g., "Technology1" and "Technology2").
    public Dictionary<string, string> GetSectorTickerStrings()
    {
        var pairs = GetSectorTickerPairs();
        var result = new Dictionary<string, string>();

        foreach (var group in pairs.GroupBy(p => p.sector))
        {
            var tickers = group.Select(p => p.ticker).ToList();

            // Get the first 50 tickers and the next 50 tickers
            string first50 = string.Join(",", tickers.Take(50));
            string second50 = string.Join(",", tickers.Skip(50).Take(50));

            result.Add(group.Key + "1", first50);
            result.Add(group.Key + "2", second50);
        }

        return result;
    }

    public void InsertNewsData(string sector, string individualTicker, QuarterRangeModel quarter, Dictionary<string, double> stockSentiment) {
        sqlite.Open();
        try {
            using (var insertDataCommand = new SqliteCommand(@"
                INSERT INTO MARKET_NEWS_DATA 
                (SECTOR, TICKER, QUARTER, YEAR, TOTAL_POSITIVE, TOTAL_NEGATIVE, TOTAL_NEUTRAL, SENTIMENT_SCORE) 
                VALUES (@sector, @ticker, @quarter, @year, @positive, @negative, @neutral, @score)", sqlite))
            {
                insertDataCommand.Parameters.AddWithValue("@sector", sector);
                insertDataCommand.Parameters.AddWithValue("@ticker", individualTicker ?? (object)DBNull.Value);
                insertDataCommand.Parameters.AddWithValue("@quarter", quarter.Quarter);
                insertDataCommand.Parameters.AddWithValue("@year", quarter.Year);
                insertDataCommand.Parameters.AddWithValue("@positive", stockSentiment.GetValueOrDefault("Total Positive", 0));
                insertDataCommand.Parameters.AddWithValue("@negative", stockSentiment.GetValueOrDefault("Total Negative", 0));
                insertDataCommand.Parameters.AddWithValue("@neutral", stockSentiment.GetValueOrDefault("Total Neutral", 0));
                insertDataCommand.Parameters.AddWithValue("@score", stockSentiment.GetValueOrDefault("Sentiment Score", 0.0));

                insertDataCommand.ExecuteNonQuery();
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex.Message}");
        } finally {
            sqlite.Close();
        }
    }

    public void InsertStockData(string table, string sector, string individualTicker, StockDataModel stockData)
    {
        try
        {
            using (var insertDataCommand = new SqliteCommand(@$"
                INSERT INTO {table} 
                (SECTOR, TICKER, DATE, OPEN, HIGH, LOW, CLOSE, ADJUSTED_CLOSE, VOLUME) 
                VALUES (@sector, @ticker, @date, @open, @high, @low, @close, @adjusted_close, @volume)", sqlite, transaction))
            {
                insertDataCommand.Parameters.AddWithValue("@sector", sector);
                insertDataCommand.Parameters.AddWithValue("@ticker", individualTicker);
                insertDataCommand.Parameters.AddWithValue("@date", stockData.Date);
                insertDataCommand.Parameters.AddWithValue("@open", stockData.Open);
                insertDataCommand.Parameters.AddWithValue("@high", stockData.High);
                insertDataCommand.Parameters.AddWithValue("@low", stockData.Low);
                insertDataCommand.Parameters.AddWithValue("@close", stockData.Close);
                insertDataCommand.Parameters.AddWithValue("@adjusted_close", stockData.AdjustedClose);
                insertDataCommand.Parameters.AddWithValue("@volume", stockData.Volume);

                insertDataCommand.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public DataTable SectorCalculations(DateTime runDate, string sector)
    {
        // Format the runDate as "yyyy-MM-dd".
        // string formattedDate = runDate.ToString("yyyy-MM-dd");

        // Construct the SQL query.
        string query = $@"
            SELECT
                MAST.SECTOR,
                MAST.DATE,
                SUM(MAST.OPEN * CAST(MAST.MARKET_CAP AS REAL)) / SUM(CAST(MAST.MARKET_CAP AS REAL)) AS WEIGHTED_OPEN,
                SUM(MAST.HIGH * CAST(MAST.MARKET_CAP AS REAL)) / SUM(CAST(MAST.MARKET_CAP AS REAL)) AS WEIGHTED_HIGH,
                SUM(MAST.LOW * CAST(MAST.MARKET_CAP AS REAL)) / SUM(CAST(MAST.MARKET_CAP AS REAL)) AS WEIGHTED_LOW,
                SUM(MAST.CLOSE * CAST(MAST.MARKET_CAP AS REAL)) / SUM(CAST(MAST.MARKET_CAP AS REAL)) AS WEIGHTED_CLOSE,
                SUM(MAST.ADJUSTED_CLOSE * CAST(MAST.MARKET_CAP AS REAL)) / SUM(CAST(MAST.MARKET_CAP AS REAL)) AS WEIGHTED_ADJUSTED_CLOSE,
                SUM(MAST.VOLUME * CAST(MAST.MARKET_CAP AS REAL)) / SUM(CAST(MAST.MARKET_CAP AS REAL)) AS WEIGHTED_VOLUME 
            FROM 
            (
                SELECT 
                    HS.*, 
                    TOP.MARKET_CAP
                FROM {sectorTables[sector]} HS
                JOIN SECTOR_STOCKS TOP 
                    ON TOP.TICKER = HS.TICKER
                WHERE 
                    HS.SECTOR = @sector 
                    AND HS.DATE = @date
            ) AS MAST
            GROUP BY MAST.SECTOR, MAST.DATE";

        DataTable dt = new DataTable();

        // Open the connection, execute the query, and fill the DataTable.
        OpenConnection();
        try
        {
            using (var cmd = new SqliteCommand(query, sqlite))
            {
                cmd.Parameters.AddWithValue("@sector", sector);
                cmd.Parameters.AddWithValue("@date", runDate);
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing SectorCalculations: {ex.Message}");
            throw;
        }
        finally
        {
            CloseConnection();
        }
        
        return dt;
    } 

    public DataTable GetSectorSentiment(Tuple<string, string> quarterYear, string sector) {
        // Construct the SQL query.
        string query = $@"
            SELECT 
            MND.SECTOR,
            SUM(MND.TOTAL_POSITIVE) AS TOTAL_POSITIVE,
            SUM(MND.TOTAL_NEGATIVE) AS TOTAL_NEGATIVE,
            SUM(MND.TOTAL_NEUTRAL) AS TOTAL_NEUTRAL,
            SUM(MND.SENTIMENT_SCORE) AS SENTIMENT_SCORE
        FROM
            MARKET_NEWS_DATA MND
        WHERE MND.QUARTER = @quarter
        AND MND.YEAR = @year
        AND MND.SECTOR = @sector";

        DataTable dt = new DataTable();

        // Open the connection, execute the query, and fill the DataTable.
        OpenConnection();
        try
        {
            using (var cmd = new SqliteCommand(query, sqlite))
            {
                cmd.Parameters.AddWithValue("@sector", sector);
                cmd.Parameters.AddWithValue("@quarter", quarterYear.Item1);
                cmd.Parameters.AddWithValue("@year", quarterYear.Item2);
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing SectorCalculations: {ex.Message}");
            throw;
        }
        finally
        {
            CloseConnection();
        }
        
        return dt;
    }

}
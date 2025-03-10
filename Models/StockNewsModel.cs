using System.Text.Json;

public class StockNewsModel
{
    public JsonElement total { get; set; }
    public JsonElement data { get; set; }
    public int total_pages { get; set; }
}
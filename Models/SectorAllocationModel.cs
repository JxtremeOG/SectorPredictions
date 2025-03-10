using System;
using System.Collections.Generic;
using System.Linq;

public class SectorAllocationModel {
    public static List<string> SectorNames = new List<string> {
        "Technology",
        "Financial Services",
        "Consumer Cyclical",
        "Healthcare",
        "Communication Services",
        "Industrials",
        "Consumer Defensive",
        "Energy",
        "Basic Materials",
        "Real Estate",
        "Utilities"
    };

    private List<double> Allocations { get; set; } = new List<double>();
    public int SectorAllocationCount;
    public double Fitness;

    public SectorAllocationModel() {
        this.SectorAllocationCount = SectorNames.Count;
        // Initialize with zeros for each sector.
        this.Fitness = 0.0;
    }

    public void SetAllocations(List<double> allocations) {
        this.Allocations = allocations;
    }

    public void AddAllocation(double allocation) {
        this.Allocations.Add(allocation);
    }

    public List<double> GetAllocations() {
        return this.Allocations;
    }

    public void AssignAllocation(int index, double value) {
        this.Allocations[index] = value;
    }

    public void ChangeAllocation(int index, double value) {
        this.Allocations[index] += value;
    }

    public Dictionary<string, double> ToDict() {
        Dictionary<string, double> dict = new Dictionary<string, double>();
        for (int i = 0; i < this.SectorAllocationCount; i++) {
            dict.Add(SectorNames[i], this.Allocations[i]);
        }
        return dict;
    }

    public void NormalizeVector() {
        double total = this.Allocations.Sum();
        if (total == 0) {
            return;
        }
        List<double> normalized = new List<double>();
        foreach (double x in this.Allocations) {
            normalized.Add(x / total);
        }
        List<double> rounded = new List<double>();
        foreach (double x in normalized) {
            rounded.Add(Math.Round(x, 2));
        }
        double diff = Math.Round(1.0 - rounded.Sum(), 2);
        if (Math.Abs(diff) > 0) {
            int maxIndex = rounded.IndexOf(rounded.Max());
            rounded[maxIndex] = Math.Round(rounded[maxIndex] + diff, 2);
        }
        this.Allocations = rounded;
    }
}

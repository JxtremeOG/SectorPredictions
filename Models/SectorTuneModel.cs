using System;
using System.Collections.Generic;
using System.Linq;

public class SectorsTuneModel : IIndividual<SectorsTuneModel>{
    public static List<string> ParameterNames = new List<string> {
        "day30Weight",
        "day90Weight",
        "day365Weight",
        "day1095Weight",
        "quarterSentimentWeight",
        "bullvsBearWeight",
        "largeSectorWeight",
        "smallSectorWeight",
        "hhiWeight",
        "sortinoRiskWeight",
        "minAllocation",
        "yearSentimentWeight",
        "RSI30DayWeight",
        "ATRWeight",
        "ADL30DayPercentWeight",
        "UnevenAllocationWeight",
        "ADL90DayPercentWeight",
        "RSI90DayWeight",
        "quarterToYearRatioWeight",
        "RSI30to90DayRatioWeight",
        "ADL30to90DayRatioWeight",
        "percent30to90DayRatioWeight",
        "ATR90ToPercentChangeWeight"

    };
    public string id { get; set; } = "0";
    private List<double> Parameters { get; set; } = new List<double>();
    public double Fitness { get; set; } = 0.0;
    public int ParameterCount;

    public SectorsTuneModel() {
        this.ParameterCount = ParameterNames.Count;
    }

    public void SetParameters(List<double> allocations) {
        this.Parameters = allocations;
    }

    public void AddParameter(double allocation) {
        this.Parameters.Add(allocation);
    }

    public List<double> GetParameters() {
        return this.Parameters;
    }

    public void AssignParameter(int index, double value) {
        this.Parameters[index] = value;
    }

    public void ChangeParameter(int index, double value) {
        this.Parameters[index] += value;
    }

    public Dictionary<string, double> ToDict() {
        Dictionary<string, double> dict = new Dictionary<string, double>();
        for (int i = 0; i < this.ParameterCount; i++) {
            dict.Add(ParameterNames[i], this.Parameters[i]);
        }
        return dict;
    }

    public SectorsTuneModel Clone()
    {
        throw new NotImplementedException();
    }
}

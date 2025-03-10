using System;
using System.Collections.Generic;
using System.Linq;

public class SectorWeightsModel {
    private List<double> Weights { get; set; } = new List<double>();
    public int WeightCount;
    public double Fitness;

    public SectorWeightsModel() {
        // Initialize with zeros for each sector.
        this.Fitness = 0.0;
    }

    public void SetWeights(List<double> allocations) {
        this.Weights = allocations;
    }

    public void AddWeight(double allocation) {
        this.Weights.Add(allocation);
    }

    public List<double> GetWeights() {
        return this.Weights;
    }

    public void AssignWeight(int index, double value) {
        this.Weights[index] = value;
    }

    public void ChangeWeight(int index, double value) {
        this.Weights[index] += value;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

public class SectorsGeneticAlgorithm 
{
    public int populationSize;
    public double mutationChance;
    public int immigrantCount;
    public int generationCount;
    public Random random;
    public FitnessSectorCalculations fitnessSector;
    public List<SectorAllocationModel> population;

    public SectorsGeneticAlgorithm(int populationSize, double mutationChance, int immigrantCount, int generationCount, FitnessSectorCalculations fitnessSector)
    {
        this.populationSize = populationSize;
        this.mutationChance = mutationChance;
        this.immigrantCount = immigrantCount;
        this.generationCount = generationCount;
        this.random = new Random();
        this.fitnessSector = fitnessSector;
        this.population = new List<SectorAllocationModel>();

        for (int i = 0; i < populationSize; i++)
        {
            SectorAllocationModel sectorAllocation = CreateRandomAllocation();
            this.population.Add(sectorAllocation);
        }
    }

    public SectorAllocationModel CreateRandomAllocation()
    {
        SectorAllocationModel sectorAllocation = new SectorAllocationModel();
        sectorAllocation.SetAllocations(Enumerable.Repeat(random.NextDouble(), sectorAllocation.SectorAllocationCount).ToList());
        sectorAllocation.NormalizeVector();
        return sectorAllocation;
    }

    public void EvaluatePopulation()
    {
        foreach (SectorAllocationModel sectorAllocation in this.population)
        {
            // Reset fitness before evaluation
            sectorAllocation.Fitness = 0;
            fitnessSector.Evaluate(sectorAllocation);
        }
    }

    public List<SectorAllocationModel> GetWeakestAllocations()
    {
        // Sort population by fitness in ascending order
        List<SectorAllocationModel> sortedPopulation = this.population.OrderBy(sa => sa.Fitness).ToList();
        // (Make sure that the population is large enough for this slice.)
        return sortedPopulation.Skip(Convert.ToInt32(populationSize * .05)).Take(Convert.ToInt32(populationSize * .05)).ToList();
    }

    public List<Tuple<SectorAllocationModel, SectorAllocationModel>> GetParentPairs(int remainingPopulationCount)
    {
        List<Tuple<SectorAllocationModel, SectorAllocationModel>> parentPairs = new List<Tuple<SectorAllocationModel, SectorAllocationModel>>();
        int pairCount = remainingPopulationCount / 2;
        for (int j = 0; j < pairCount; j++)
        {
            SectorAllocationModel parent1 = this.population[random.Next(this.population.Count)];
            SectorAllocationModel parent2 = this.population[random.Next(this.population.Count)];
            parentPairs.Add(new Tuple<SectorAllocationModel, SectorAllocationModel>(parent1, parent2));
        }
        return parentPairs;
    }

    public Tuple<SectorAllocationModel, SectorAllocationModel> CrossOver(SectorAllocationModel parent1, SectorAllocationModel parent2)
    {
        SectorAllocationModel child1 = new SectorAllocationModel();
        SectorAllocationModel child2 = new SectorAllocationModel();

        for (int i = 0; i < parent1.GetAllocations().Count; i++)
        {
            double randomTheta1 = random.NextDouble();
            double randomTheta2 = random.NextDouble();

            double child1Allocation = randomTheta1 * parent1.GetAllocations()[i] + (1 - randomTheta1) * parent2.GetAllocations()[i];
            double child2Allocation = randomTheta2 * parent1.GetAllocations()[i] + (1 - randomTheta2) * parent2.GetAllocations()[i];

            child1.AddAllocation(child1Allocation);
            child2.AddAllocation(child2Allocation);
        }

        Mutate(child1);
        Mutate(child2);
        child1.NormalizeVector();
        child2.NormalizeVector();

        return new Tuple<SectorAllocationModel, SectorAllocationModel>(child1, child2);
    }

    public void Mutate(SectorAllocationModel sectorAllocation)
    {
        for (int i = 0; i < sectorAllocation.GetAllocations().Count; i++)
        {
            if (random.NextDouble() < mutationChance)
            {
                // Generate a mutation amount between -0.1 and 0.1
                double mutationAmount = (random.NextDouble() * 0.2) - 0.1;
                sectorAllocation.ChangeAllocation(i, mutationAmount);
                // Ensure that the allocation is not negative
                if (sectorAllocation.GetAllocations()[i] < 0)
                {
                    sectorAllocation.AssignAllocation(i, 0);
                }
            }
        }
    }

    public List<SectorAllocationModel> CreateChildren(List<Tuple<SectorAllocationModel, SectorAllocationModel>> parentPairs)
    {
        List<SectorAllocationModel> children = new List<SectorAllocationModel>();
        foreach (var pair in parentPairs)
        {
            if (pair.Item1.GetAllocations().Count != pair.Item2.GetAllocations().Count)
            {
                throw new Exception("Parent is weird");
            }
            var offspring = CrossOver(pair.Item1, pair.Item2);
            children.Add(offspring.Item1);
            children.Add(offspring.Item2);
        }
        return children;
    }

    public List<SectorAllocationModel> CreateImmigrants() {
        List<SectorAllocationModel> immigrants = new List<SectorAllocationModel>();
        for (int i = 0; i < this.immigrantCount; i++) {
            immigrants.Add(CreateRandomAllocation());
        }
        return immigrants;
    }

    public SectorAllocationModel TranGenetically() {
        Console.WriteLine($"Training with population size: {this.populationSize}, and generation count: {this.generationCount}");
        SectorAllocationModel currentBestAllocation = null;

        for (int generation = 0; generation < this.generationCount; generation++)
        {
            EvaluatePopulation();

            // Keep 30% of the population (top performers)
            int remainingPopulationCount = (int)(this.population.Count * 0.3);
            List<SectorAllocationModel> newPopulation = this.population
                .OrderByDescending(sa => sa.Fitness)
                .Take(remainingPopulationCount)
                .ToList();

            currentBestAllocation = newPopulation.First();

            if (generation % 50 == 0)
            {
                Console.WriteLine($"Generation: {generation} Overall best fitness: {currentBestAllocation.Fitness} and population size {this.population.Count}");
            }

            // Extend population with some of the weakest individuals
            newPopulation.AddRange(GetWeakestAllocations());

            // Extend population with newly created allocations (immigrants)
            newPopulation.AddRange(CreateImmigrants());

            remainingPopulationCount = this.populationSize - newPopulation.Count;

            // Create parent pairs based on the remaining population count
            var parentPairs = GetParentPairs(remainingPopulationCount);
            newPopulation.AddRange(CreateChildren(parentPairs));

            // If we still don't have enough individuals, keep creating children until we do
            while (newPopulation.Count < this.populationSize)
            {
                var extraParentPairs = GetParentPairs(1);
                var createdChildren = CreateChildren(extraParentPairs);
                newPopulation.Add(createdChildren[0]);
                if (newPopulation.Count < this.populationSize)
                {
                    newPopulation.Add(createdChildren[1]);
                }
            }

            if (newPopulation.Count < this.populationSize)
            {
                throw new Exception($"New population count {newPopulation.Count} is less than required population size {this.populationSize}");
            }

            this.population = newPopulation;

            if (generation % 50 == 0)
            {
                Console.WriteLine($"Generation {generation} completed");
            }
        }
        return currentBestAllocation;
    }
}

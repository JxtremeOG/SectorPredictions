public class AllocationsAlgorithm : IAlgorithms<SectorAllocationModel> {
    public static Random random = new Random();
    public AllocationsFitness allocationsFitness;

    public AllocationsAlgorithm(AllocationsFitness allocationsFitness) {
        this.allocationsFitness = allocationsFitness;
    }
    public SectorAllocationModel CreateRandomIndividual()
    {
        SectorAllocationModel sectorAllocation = new SectorAllocationModel();
        var allocations = Enumerable.Range(0, sectorAllocation.SectorAllocationCount)
            .Select(_ => Random.Shared.NextDouble())
            .ToList();
        sectorAllocation.id = "SectorAllocationModel-"+DateTime.UtcNow.Ticks.ToString().Replace(".","")+"-"+Random.Shared.Next(0, 1000000).ToString(); // Assign a random ID for the individual.
        sectorAllocation.SetAllocations(allocations);
        sectorAllocation.NormalizeVector();
        return sectorAllocation;
    }



    public Tuple<SectorAllocationModel, SectorAllocationModel> Crossover(SectorAllocationModel parent1, SectorAllocationModel parent2, double mutationChance)
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

        Mutate(child1, mutationChance);
        Mutate(child2, mutationChance);

        return new Tuple<SectorAllocationModel, SectorAllocationModel>(child1, child2);
    }

    public void EvaluateIndividual(SectorAllocationModel individual)
    {
        allocationsFitness.EvaluateIndividualFitness(individual);
    }

    public void Mutate(SectorAllocationModel individual, double mutationChance)
    {
        var allocations = individual.GetAllocations();
        for (int i = 0; i < individual.SectorAllocationCount; i++)
        {
            if (random.NextDouble() < mutationChance)
            {
                // Generate a mutation amount between -0.1 and 0.1
                double mutationAmount = (random.NextDouble() * 0.2) - 0.1;
                individual.ChangeAllocation(i, mutationAmount);
                // Ensure that the allocation is not negative
                if (allocations[i] < 0)
                {
                    individual.AssignAllocation(i, 0);
                }
            }
        }
        individual.NormalizeVector();
    }
}
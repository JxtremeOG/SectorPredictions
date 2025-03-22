
public class SectorAlgorithm : IAlgorithms<SectorsTuneModel>
{
    public static Random random = new Random();
    public SectorsFitness sectorsFitness;

    public SectorAlgorithm(SectorsFitness sectorsFitness) {
        this.sectorsFitness = sectorsFitness;
    }

    public Tuple<SectorsTuneModel, SectorsTuneModel> Crossover(SectorsTuneModel parent1, SectorsTuneModel parent2, double mutationChance)
    {
        SectorsTuneModel child1 = new SectorsTuneModel();
        SectorsTuneModel child2 = new SectorsTuneModel();

        for (int i = 0; i < parent1.GetParameters().Count; i++)
        {
            double randomTheta1 = random.NextDouble();
            double randomTheta2 = random.NextDouble();

            double child1Allocation = randomTheta1 * parent1.GetParameters()[i] + (1 - randomTheta1) * parent2.GetParameters()[i];
            double child2Allocation = randomTheta2 * parent1.GetParameters()[i] + (1 - randomTheta2) * parent2.GetParameters()[i];

            child1.AddParameter(child1Allocation);
            child2.AddParameter(child2Allocation);
        }

        return new Tuple<SectorsTuneModel, SectorsTuneModel>(child1, child2);
    }

    public void EvaluateIndividual(SectorsTuneModel individual)
    {
        sectorsFitness.EvaluateIndividualFitness(individual);
    }

    public void Mutate(SectorsTuneModel individual, double mutationChance)
    {
        // Retrieve the current list of parameters.
        List<double> parameters = individual.GetParameters();
        
        // Iterate over each parameter.
        for (int i = 0; i < parameters.Count; i++)
        {
            if (random.NextDouble() < mutationChance)
            {
                // Determine a mutation factor: a change of up to +/- 10% of the current value.
                double mutationFactor = 1 + ((random.NextDouble() - 0.5) * 0.2); // 0.2 gives +/-10%
                double mutatedValue = parameters[i] * mutationFactor;
                
                // Optional: Clamp mutatedValue to a valid range, e.g. 0 to 100.
                mutatedValue = Math.Max(0, Math.Min(mutatedValue, 100));
                
                // Update the parameter with the mutated value.
                parameters[i] = mutatedValue;
            }
        }
        
        // If SectorsTuneModel requires re-setting the parameters rather than modifying in place,
        // uncomment the following line:
        // individual.SetParameters(parameters);
    }


    public SectorsTuneModel CreateRandomIndividual()
    {
        SectorsTuneModel sectorsTune = new SectorsTuneModel();
        sectorsTune.SetParameters(
            Enumerable.Range(0, sectorsTune.ParameterCount)
                    .Select(_ => random.NextDouble() * 5)
                    .ToList()
        );

        return sectorsTune;
    }
}
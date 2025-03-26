
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

            child1.AddParameter(Math.Round(child1Allocation, 5));
            child2.AddParameter(Math.Round(child2Allocation, 5));
        }

        return new Tuple<SectorsTuneModel, SectorsTuneModel>(child1, child2);
    }

    public void EvaluateIndividual(SectorsTuneModel individual)
    {
        sectorsFitness.EvaluateIndividualFitness(individual);
    }

    public void OptimizeWeights(SectorsTuneModel individual) {
        List<double> parameters = individual.GetParameters();
        for (int i = 0; i < parameters.Count; i++) {
            double roundedValue = Math.Round(parameters[i], 5);
            individual.AssignParameter(i, roundedValue);
        }
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
                if (random.NextDouble() < .9) {
                    // Determine a mutation factor: a change of up to +/- 10% of the current value.
                    double mutationFactor = 1 + ((random.NextDouble() - 0.5) * 0.2); // 0.2 gives +/-10%
                    double mutatedValue = parameters[i] * mutationFactor;
                    
                    // Update the parameter with the mutated value.
                    parameters[i] = Math.Round(mutatedValue, 5);
                }
                else {
                    parameters[i] = parameters[i] * -1;
                }
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
                    .Select(_ => Math.Round(Random.Shared.NextDouble() * 5 * (Random.Shared.NextDouble() < .9 ? 1 : -1), 5))
                    .ToList()
        );
        sectorsTune.id = "SectorsTuneModel-"+DateTime.UtcNow.Ticks.ToString().Replace(".","")+"-"+Random.Shared.Next(0, 1000000).ToString(); // Assign a random ID for the individual.

        return sectorsTune;
    }
}
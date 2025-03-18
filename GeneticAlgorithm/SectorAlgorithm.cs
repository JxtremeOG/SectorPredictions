
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

        Mutate(child1, mutationChance);
        Mutate(child2, mutationChance);

        return new Tuple<SectorsTuneModel, SectorsTuneModel>(child1, child2);
    }

    public void EvaluateIndividual(SectorsTuneModel individual)
    {
        sectorsFitness.EvaluateIndividualFitness(individual);
    }

    public void Mutate(SectorsTuneModel individual, double mutationChance)
    {
        throw new NotImplementedException();
    }

    public SectorsTuneModel CreateRandomIndividual()
    {
        SectorsTuneModel sectorsTune = new SectorsTuneModel();
        sectorsTune.SetParameters(Enumerable.Repeat(random.NextDouble() * 100, sectorsTune.ParameterCount).ToList());
        return sectorsTune;
    }
}
public interface IAlgorithms<T>
{
    abstract T CreateRandomIndividual();
    abstract void EvaluateIndividual(T individual);
    abstract Tuple<SectorAllocationModel, SectorAllocationModel> Crossover(T parent1, T parent2, double mutationChance);
    abstract void Mutate(T individual, double mutationChance);
}

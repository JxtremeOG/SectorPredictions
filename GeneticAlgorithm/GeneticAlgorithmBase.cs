using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Define a generic interface that every individual must implement.
public interface IIndividual<T>
{
    double Fitness { get; set; }
    T Clone();
}

// The generic genetic algorithm class.
public class GeneticAlgorithmBase<T> where T : IIndividual<T>
{
    public int PopulationSize { get; }
    public double MutationChance { get; }
    public int ImmigrantCount { get; }
    public int GenerationCount { get; }
    public Random Random { get; } = new Random();
    
    // Delegates for problem-specific behavior.
    public Func<T> CreateRandomIndividual { get; }
    public Action<T> EvaluateIndividual { get; }
    public Func<T, T, double, Tuple<T, T>> Crossover { get; }
    public Action<T, double> Mutate { get; }

    public List<T> Population { get; private set; }

    public GeneticAlgorithmBase(
        int populationSize,
        double mutationChance,
        int immigrantCount,
        int generationCount,
        Func<T> CreateRandomIndividual,
        Action<T> EvaluateIndividual,
        Func<T, T, double, Tuple<T, T>> Crossover,
        Action<T, double> Mutate)
    {
        PopulationSize = populationSize;
        MutationChance = mutationChance;
        ImmigrantCount = immigrantCount;
        GenerationCount = generationCount;
        this.CreateRandomIndividual = CreateRandomIndividual;
        this.EvaluateIndividual = EvaluateIndividual;
        this.Crossover = Crossover;
        this.Mutate = Mutate;

        // Initialize population.
        Population = new List<T>();
        for (int i = 0; i < PopulationSize; i++)
        {
            Population.Add(this.CreateRandomIndividual());
        }
    }

    public async Task EvaluatePopulationAsync()
    {
        // var tasks = Population.Select(individual => Task.Run(() =>
        // {
        //     individual.Fitness = 0;
        //     EvaluateIndividual(individual);
        // }));
        // await Task.WhenAll(tasks);
        foreach (T individual in Population)
        {
            individual.Fitness = 0; // reset fitness
            EvaluateIndividual(individual);
        }
    }


    public List<T> GetWeakestIndividuals(double fraction)
    {
        int skipCount = (int)(PopulationSize * fraction);
        int takeCount = (int)(PopulationSize * fraction);
        return Population.OrderBy(ind => ind.Fitness)
                         .Skip(skipCount)
                         .Take(takeCount)
                         .ToList();
    }

    public List<Tuple<T, T>> GetParentPairs(int count)
    {
        List<Tuple<T, T>> parentPairs = new List<Tuple<T, T>>();
        for (int i = 0; i < count / 2; i++)
        {
            T parent1 = Population[Random.Next(Population.Count)];
            T parent2 = Population[Random.Next(Population.Count)];
            parentPairs.Add(new Tuple<T, T>(parent1, parent2));
        }
        return parentPairs;
    }

    public List<T> CreateChildren(List<Tuple<T, T>> parentPairs)
    {
        List<T> children = new List<T>();
        foreach (var pair in parentPairs)
        {
            var offspring = Crossover(pair.Item1, pair.Item2, MutationChance);
            // Apply mutation to both offspring.
            Mutate(offspring.Item1, MutationChance);
            Mutate(offspring.Item2, MutationChance);
            children.Add(offspring.Item1);
            children.Add(offspring.Item2);
        }
        return children;
    }

    public List<T> CreateImmigrants()
    {
        List<T> immigrants = new List<T>();
        for (int i = 0; i < ImmigrantCount; i++)
        {
            immigrants.Add(CreateRandomIndividual());
        }
        return immigrants;
    }

    public async Task<T> Run()
    {
        T currentBest = default;
        for (int generation = 0; generation < GenerationCount; generation++)
        {
            await EvaluatePopulationAsync();

            // Select top performers (e.g., top 30%).
            int survivorsCount = (int)(PopulationSize * 0.3);
            List<T> newPopulation = Population.OrderByDescending(ind => ind.Fitness)
                                                .Take(survivorsCount)
                                                .ToList();

            currentBest = newPopulation.First();

            if (typeof(T) == typeof(SectorsTuneModel)) //generation % 50 == 0 && 
            {
                Console.WriteLine($"SectorsTuneModel | Generation: {generation}, Best Fitness: {currentBest.Fitness}");
            }

            // Add a slice of weakest individuals.
            newPopulation.AddRange(GetWeakestIndividuals(0.05));

            // Add immigrants.
            newPopulation.AddRange(CreateImmigrants());

            // Create children to fill the remaining population.
            int remainingCount = PopulationSize - newPopulation.Count;
            var parentPairs = GetParentPairs(remainingCount);
            newPopulation.AddRange(CreateChildren(parentPairs)); 

            // If needed, keep creating children until reaching the required population size.
            while (newPopulation.Count < PopulationSize) 
            {
                var extraParents = GetParentPairs(2);
                var extraChildren = CreateChildren(extraParents);
                newPopulation.AddRange(extraChildren);
            }

            Population = newPopulation.Take(PopulationSize).ToList();
        }
        return currentBest;
    }
}

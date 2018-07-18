using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetControlApp.Algorithms
{
    class Population
    {
        public List<Chromosome> Chromosomes { get; set; }
        public Int32 MaximumSize { get; set; }

        /// <summary>
        /// Constructor for the population.
        /// </summary>
        /// <param name="size">The maximum size of the population.</param>
        public Population(Int32 size)
        {
            this.Chromosomes = new List<Chromosome>(size);
            this.MaximumSize = size;
        }

        /// <summary>
        /// Calculates the fitness array to be used in a Monte-Carlo selection algorithm.
        /// </summary>
        /// <returns>An array containing the fitness of all the chromosomes in the population.</returns>
        public Double[] GetFitnessArray()
        {
            var fitnessArray = new Double[this.MaximumSize + 1];
            fitnessArray[0] = 0.0;
            var totalFitness = 0.0;
            foreach (var item in this.Chromosomes)
            {
                totalFitness = totalFitness + item.Fitness;
            }
            for (int i = 0; i < this.MaximumSize; i++)
            {
                fitnessArray[i + 1] = fitnessArray[i] + this.Chromosomes[i].Fitness / totalFitness;
            }
            return fitnessArray;
        }

        /// <summary>
        /// Selects a chromosome based on its fitness. The better the fitness, the better chances it has.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns>A specific chromosome.</returns>
        public Chromosome Select(Double[] fitnessArray, Random rand)
        {
            var value = rand.NextDouble();
            var index = 0;
            for (int i = 0; i < fitnessArray.Length - 1; i++)
            {
                if (value < fitnessArray[i + 1])
                {
                    index = i;
                    break;
                }
            }
            return this.Chromosomes[index];
        }

        /// <summary>
        /// Initializes the population with randomly generated chromosomes.
        /// </summary>
        /// <param name="matrixPowers">The array with the powers of the target-corresponding matrix C.</param>
        /// <param name="list"></param>
        /// <param name="maximumRandom"></param>
        /// <param name="rand"></param>
        public void Initialize(Matrix<Double>[] matrixPowers, Int32[][] list, Int32 maximumRandom, Random rand)
        {
            var numberOfGroups = (int)Math.Ceiling((double)list.Length / maximumRandom);
            var elementsPerGroup = (int)Math.Ceiling((double)this.MaximumSize / numberOfGroups);
            var lowerLimit = 0;
            var upperLimit = maximumRandom;
            for (int index1 = 0; index1 < numberOfGroups; index1++)
            {
                for (int index2 = 0; index2 < elementsPerGroup; index2++)
                {
                    Chromosome chromosome = new Chromosome(list.Length);
                    chromosome.Initialize(matrixPowers, list, lowerLimit, upperLimit, rand);
                    this.Chromosomes.Add(chromosome);
                }
                lowerLimit = upperLimit;
                upperLimit = upperLimit + maximumRandom;
            }
        }

        /// <summary>
        /// Moves the genetic algorithm over to a new generation (and a new population).
        /// </summary>
        /// <param name="elitismPercentage"></param>
        /// <param name="randomPercentage"></param>
        /// <param name="mutationProbability"></param>
        /// <param name="matrixPowers"></param>
        /// <param name="list"></param>
        /// <param name="maximumRandom"></param>
        /// <param name="rand"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public Population nextPopulation(Double elitismPercentage, Double randomPercentage, Double mutationProbability,
            Matrix<Double>[] matrixPowers, Int32[][] list, Int32 maximumRandom, Random rand, String[] nodes)
        {
            var Population = new Population(this.MaximumSize);
            // Order the chromosomes in the descending order of their fitness.
            var orderedChromosomes = this.Chromosomes.OrderByDescending(c => c.Fitness).ToArray();
            // Determine the number of elites to be added to the new population and adds them.
            var maximumNumberOfElites = (int)Math.Floor(elitismPercentage * this.MaximumSize);
            var elites = this.GetBestChromosomes();
            while (elites.Count > maximumNumberOfElites)
            {
                elites.RemoveAt(rand.Next(elites.Count));
            }
            Population.Chromosomes.AddRange(elites);
            Console.WriteLine($"{Population.Chromosomes.Count} chromomsomes in population after adding the elite chromosomes.");
            // Determine the number of random individuals to be added to the population.
            var numberOfRandoms = (int)Math.Floor(randomPercentage * this.MaximumSize);
            for (int i = 0; i < numberOfRandoms; i++)
            {
                var lowerLimit = rand.Next(Math.Max(Math.Min(list.Length, list.Length - maximumRandom), 0));
                var upperLimit = Math.Min(lowerLimit + maximumRandom, list.Length);
                var chromosome = new Chromosome(list.Length);
                chromosome.Initialize(matrixPowers, list, lowerLimit, upperLimit, rand);
                Population.Chromosomes.Add(chromosome);
            }
            Console.WriteLine($"{Population.Chromosomes.Count} chromomsomes in population after adding the random chromosomes.");
            // Add new chromosomes.
            var fitnessArray = this.GetFitnessArray();
            System.Threading.Tasks.Parallel.For(Population.Chromosomes.Count, Population.MaximumSize, index =>
            {
                var isValid = false;
                var chromosome = new Chromosome(list.Length);
                while (!isValid)
                {
                    var chromosome1 = this.Select(fitnessArray, rand);
                    var chromosome2 = this.Select(fitnessArray, rand);
                    chromosome = chromosome1.Crossover(chromosome2, rand);
                    isValid = chromosome.IsValid(matrixPowers);
                }
                chromosome.Mutate(matrixPowers, list, maximumRandom, rand, mutationProbability);
                Population.Chromosomes.Add(chromosome);
            });
            if (Population.Chromosomes.Count < Population.MaximumSize)
            {
                for (int i = Population.Chromosomes.Count; i < Population.MaximumSize; i++)
                {
                    var isValid = false;
                    var chromosome = new Chromosome(list.Length);
                    while (!isValid)
                    {
                        var chromosome1 = this.Select(fitnessArray, rand);
                        var chromosome2 = this.Select(fitnessArray, rand);
                        chromosome = chromosome1.Crossover(chromosome2, rand);
                        isValid = chromosome.IsValid(matrixPowers);
                    }
                    chromosome.Mutate(matrixPowers, list, maximumRandom, rand, mutationProbability);
                    Population.Chromosomes.Add(chromosome);
                }
            }
            return Population;
        }

        /// <summary>
        /// Computes the best fitness of the population.
        /// </summary>
        /// <returns></returns>
        public double getBestFitness()
        {
            return this.Chromosomes.Max(c => c.Fitness);
        }

        public List<Chromosome> GetBestChromosomes()
        {
            var bestFitness = this.getBestFitness();
            var bestChromosomes = new List<Chromosome>();
            foreach (var item1 in this.Chromosomes)
            {
                if (item1.Fitness == bestFitness)
                {
                    Boolean exists = false;
                    foreach (var item2 in bestChromosomes)
                    {
                        if (item1.IsEqual(item2))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        bestChromosomes.Add(item1);
                    }
                }
            }
            return bestChromosomes;
        }

        /// <summary>
        /// Displays the population in the console. For debug purposes only.
        /// </summary>
        /// <param name="nodes">The list of nodes in the graph.</param>
        public void Display(String[] nodes)
        {
            Console.WriteLine($"Population (maximum size {this.MaximumSize})");
            foreach (var item in this.Chromosomes)
            {
                item.Display(nodes);
            }
        }

        /// <summary>
        /// Displays the best chromosomes in the population in the console. For debug purposes only.
        /// </summary>
        /// <param name="nodes">The list of nodes in the graph.</param>
        public void DisplayBest(String[] nodes, String[] singleNodes, String[] drugTargetNodes = null)
        {
            var bestChromosomes = this.GetBestChromosomes();
            Console.WriteLine($"Best chromosomes (a number of {bestChromosomes.Count}, with best fitness {bestChromosomes[0].Fitness})");
            foreach (var item in bestChromosomes)
            {
                item.Display(nodes, singleNodes, drugTargetNodes);
            }
        }

        /// <summary>
        /// Savethe best chromosomes in the population in the console. For debug purposes only.
        /// </summary>
        /// <param name="nodes">The list of nodes in the graph.</param>
        public void WriteBest(String filename, String[] nodes, String[] singleNodes, String[] drugTargetNodes = null)
        {
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
            var bestChromosomes = this.GetBestChromosomes();
            System.IO.File.AppendAllText(filename, $"Best chromosomes (a number of {bestChromosomes.Count}, with best fitness {bestChromosomes[0].Fitness})\n");
            foreach (var item in bestChromosomes)
            {
                item.SaveToFile(filename, nodes, singleNodes, drugTargetNodes);
            }
        }
    }
}

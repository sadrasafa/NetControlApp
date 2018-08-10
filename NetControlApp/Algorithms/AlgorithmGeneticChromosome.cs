using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace NetControlApp.Algorithms
{
    class Chromosome
    {
        public double Fitness { get; set; }
        public int[] Genes { get; set; }

        /// <summary>
        /// Constructor for the chromosome.
        /// </summary>
        /// <param name="length">The length of the chromosome (the number of target nodes).</param>
        public Chromosome(Int32 length)
        {
            this.Genes = new Int32[length];
            this.Fitness = 0.0;
        }

        /// <summary>
        /// Creates a deep copy of the current chromosome.
        /// </summary>
        /// <returns>A new copy of the chromosome.</returns>
        public Chromosome DeepCopy()
        {
            var copy = new Chromosome(this.Genes.Length);
            Array.Copy(this.Genes, copy.Genes, this.Genes.Length);
            copy.Fitness = this.Fitness;
            return copy;
        }

        /// <summary>
        /// Checks if the given chromosome has the same edges as the current one.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        public Boolean IsEqual(Chromosome chromosome)
        {
            return this.Genes.SequenceEqual(chromosome.Genes);
        }

        /// <summary>
        /// Computes the drug targets existing in the chromosome.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="drugTargets"></param>
        /// <returns></returns>
        public String[] ContainedDrugTargets(List<String> nodes, List<String> drugTargets)
        {
            var form = this.GetForm();
            var chromosomeNodes = new String[form.Length];
            for (int i = 0; i < form.Length; i++)
            {
                chromosomeNodes[i] = nodes[form[i]];
            }
            return drugTargets.Intersect(chromosomeNodes).ToArray();
        }

        /// <summary>
        /// Computes the fitness of the chromosome.
        /// </summary>
        public void ComputeFitness()
        {
            // Compute the fitness.
            this.Fitness = (double)(this.Genes.Length - this.Genes.Distinct().Count() + 1) * 100 / this.Genes.Length;
        }

        /// <summary>
        /// Computes the "final form" of the chromosome, that is removing any duplicate genes.
        /// </summary>
        /// <returns>The chromosome array with no duplicate genes.</returns>
        public Int32[] GetForm()
        {
            return this.Genes.Distinct().ToArray();
        }

        /// <summary>
        /// Checks if the chromosome is a solution or not.
        /// </summary>
        /// <param name="matrixPowers">The array with the powers of the target-corresponding matrix C.</param>
        /// <returns>"true" if the chromosome is a solution to the problem, "false" otherwise.</returns>
        public Boolean IsValid(List<Matrix<Double>> matrixPowers)
        {
            bool isValid = false;
            var form = this.GetForm();
            var B = Matrix<Double>.Build.Dense(matrixPowers[0].ColumnCount, form.Length);
            for (int i = 0; i < form.Length; i++)
            {
                B[form[i], i] = 1.0;
            }
            var R = Matrix<Double>.Build.DenseOfMatrix(matrixPowers[0]).Multiply(B);
            if (R.Rank() == this.Genes.Length)
            {
                isValid = true;
            }
            else
            {
                for (int i = 1; i < matrixPowers.Count; i++)
                {
                    var M = Matrix<Double>.Build.DenseOfMatrix(matrixPowers[i]).Multiply(B);
                    R = R.Append(M);
                    if (R.Rank() == this.Genes.Length)
                    {
                        isValid = true;
                        break;
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Initializes the chromosome with randomly generated values.
        /// </summary>
        /// <param name="matrixPowers">The array with the powers of the target-corresponding matrix C.</param>
        /// <param name="list"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <param name="rand"></param>
        public void Initialize(List<Matrix<Double>> matrixPowers, List<List<Int32>> list, Int32 lowerLimit, Int32 upperLimit, Random rand)
        {
            var isValid = false;
            var tries = upperLimit - lowerLimit;
            var available = new List<Int32>(upperLimit - lowerLimit);
            for (int i = 0; i < list.Count; i++)
            {
                this.Genes[i] = list[i][0];
                if (lowerLimit <= i && i < upperLimit)
                {
                    available.Add(i);
                }
            }
            while (!isValid)
            {
                foreach (var item in available)
                {
                    this.Genes[item] = list[item][rand.Next(list[item].Count)];
                }
                isValid = this.IsValid(matrixPowers);
                if (!isValid && available.Count > 0 && tries == 0)
                {
                    //Console.WriteLine($"Couldn't randomly generate with {available.Count} values, now trying with {available.Count - 1}.");
                    int index = rand.Next(available.Count);
                    this.Genes[available[index]] = list[available[index]][0];
                    available.RemoveAt(index);
                    tries = upperLimit - lowerLimit;
                }
                else if (!isValid && available.Count > 0 && tries > 0)
                {
                    //Console.WriteLine($"Couldn't randomly generate with {available.Count} values, trying again with {available.Count} for {tries} more times.");
                }
                tries--;
            }
            this.ComputeFitness();
        }

        /// <summary>
        /// Mutates the current chromosome based on the given mutation probability.
        /// </summary>
        /// <param name="matrixPowers">The array with the powers of the target-corresponding matrix C.</param>
        /// <param name="list"></param>
        /// <param name="maximumTries"></param>
        /// <param name="rand"></param>
        /// <param name="probability"></param>
        public void Mutate(List<Matrix<Double>> matrixPowers, List<List<Int32>> list, Int32 maximumTries, Random rand, Double probability)
        {
            var selectedGenes = new List<Int32>();
            var copyOfSelectedGenes = new List<Int32>();
            var copy = this.DeepCopy();
            for (int i = 0; i < this.Genes.Length; i++)
            {
                if (rand.NextDouble() < probability)
                {
                    selectedGenes.Add(i);
                    copyOfSelectedGenes.Add(i);
                }
            }
            var count = maximumTries;
            do
            {
                foreach (var item in selectedGenes)
                {
                    int index = rand.Next(list[item].Count);
                    this.Genes[item] = list[item][index];
                }
                count--;
                if (count == 0)
                {
                    foreach (var item in copyOfSelectedGenes)
                    {
                        this.Genes[item] = copy.Genes[item];
                    }
                    selectedGenes.RemoveAt(rand.Next(selectedGenes.Count));
                    count = maximumTries;
                }
            } while (!this.IsValid(matrixPowers) && selectedGenes.Count > 0);
            if (selectedGenes.Count == 0)
            {
                for (int i = 0; i < copy.Genes.Length; i++)
                {
                    this.Genes[i] = copy.Genes[i];
                }
            }
            this.ComputeFitness();
        }

        /// <summary>
        /// Performs crossover between the current chromosome and the given one. The result is returned in a different chromosome.
        /// </summary>
        /// <param name="matrixPowers">The array with the powers of the target-corresponding matrix C.</param>
        /// <param name="chromosome">The chromosome to perform crossover with.</param>
        /// <param name="maximumTries"></param>
        /// <param name="rand"></param>
        /// <returns>The offspring chromosome.</returns>
        public Chromosome Crossover(List<Matrix<Double>> matrixPowers, Chromosome chromosome, Int32 maximumTries, Random rand)
        {
            var offspring = new Chromosome(this.Genes.Length);
            var isValid = false;
            var tries = maximumTries;
            while (!isValid && tries > 0)
            {
                for (int i = 0; i < offspring.Genes.Length; i++)
                {
                    if (rand.NextDouble() < 0.5)
                    {
                        offspring.Genes[i] = this.Genes[i];
                    }
                    else
                    {
                        offspring.Genes[i] = chromosome.Genes[i];
                    }
                }
                isValid = offspring.IsValid(matrixPowers);
            }
            if (!isValid)
            {
                if (rand.NextDouble() < 0.5)
                {
                    offspring = this.DeepCopy();
                }
                else
                {
                    offspring = chromosome.DeepCopy();
                }
            }
            offspring.ComputeFitness();
            return offspring;
        }

        /// <summary>
        /// Performs crossover between the current chromosome and the given one. The resulting offspring may or may not be valid.
        /// </summary>
        /// <param name="chromosome">The second chromo</param>
        /// <param name="rand"></param>
        /// <returns></returns>
        public Chromosome Crossover(Chromosome chromosome, Random rand)
        {
            var offspring = new Chromosome(this.Genes.Length);
            var uniqueGenes = this.Genes.Concat(chromosome.Genes).Distinct().ToList();
            var occurances = new List<Int32>(uniqueGenes.Count());
            var total = 0;
            foreach (var item in uniqueGenes)
            {
                int count = this.Genes.Count(gene => gene == item) + chromosome.Genes.Count(gene => gene == item);
                total = total + count;
                occurances.Add(count);
            }
            for (int i = 0; i < offspring.Genes.Length; i++)
            {
                int occ1 = occurances[uniqueGenes.IndexOf(this.Genes[i])];
                int occ2 = occurances[uniqueGenes.IndexOf(chromosome.Genes[i])];
                if (rand.NextDouble() < (double)occ1 / (occ1 + occ2))
                {
                    offspring.Genes[i] = this.Genes[i];
                }
                else
                {
                    offspring.Genes[i] = chromosome.Genes[i];
                }
            }
            offspring.ComputeFitness();
            return offspring;
        }

        /// <summary>
        /// Displays the chromosome in the console. For debug purposes only.
        /// </summary>
        /// <param name="nodes">The array of nodes which maps from node ID to node name.</param>
        public void Display(List<String> nodes)
        {
            Console.WriteLine($"Chromosome (fitness {this.Fitness}, nodes {this.Genes.Distinct().Count()}):");
            foreach (var item in this.Genes)
            {
                Console.Write($"{nodes[item]} ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays the chromosome in the console. For debug purposes only.
        /// </summary>
        /// <param name="nodes">The current list of nodes.</param>
        /// <param name="singleNodes">The list of single nodes.</param>
        /// <param name="drugTargetNodes">(optional) The list of target nodes.</param>
        public void Display(List<String> nodes, List<String> singleNodes, List<String> drugTargetNodes = null)
        {
            Console.WriteLine($"Chromosome (fitness {this.Fitness}, nodes {this.Genes.Distinct().Count() + singleNodes.Count}):");
            foreach (var item in this.Genes)
            {
                Console.Write($"{nodes[item]} ");
            }
            foreach (var item in singleNodes)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();
            if (drugTargetNodes != null)
            {
                var drugTargets = this.ContainedDrugTargets(nodes, drugTargetNodes);
                Console.WriteLine($"out of which {drugTargets.Length} drug targets.");
                foreach (var item in drugTargets)
                {
                    Console.Write($"{item} ");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Appends the chromosome to the given file. For debug purposes only.
        /// </summary>
        /// <param name="filename">The path to the file in which to save the solution</param>
        /// <param name="nodes">The current list of nodes.</param>
        /// <param name="singleNodes">The list of single nodes.</param>
        /// <param name="drugTargetNodes">(optional) The list of target nodes.</param>
        public void SaveToFile(String filename, List<String> nodes, List<String> singleNodes, List<String> drugTargetNodes = null)
        {
            foreach (var item in this.Genes)
            {
                System.IO.File.AppendAllText(filename, $"{nodes[item]} ");
            }
            foreach (var item in singleNodes)
            {
                System.IO.File.AppendAllText(filename, $"{item} ");
            }
            System.IO.File.AppendAllText(filename, $"\n");
            if (drugTargetNodes != null)
            {
                var drugTargets = this.ContainedDrugTargets(nodes, drugTargetNodes);
                System.IO.File.AppendAllText(filename, $"out of which {drugTargets.Length} drug targets.");
                foreach (var item in drugTargets)
                {
                    System.IO.File.AppendAllText(filename, $"{item} ");
                }
            }
            System.IO.File.AppendAllText(filename, $"\n");
        }
    }

}

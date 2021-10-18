using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    class Program
    {
        public Population[] pool;
        public int populationSize = 250;
        public double mergeChance = 0.80;
        public double bitSwitchChance = 0.02;
        public int numberOfGen = 100;
        public double currentBestFitness = 999;
        public int bestStreak = 0;

        static void Main(string[] _)
        {
            Random rand = new Random();
            var mc = new Program();

            //collect inputs
            mc.Setup(mc);

            //create initial pool
            mc.pool = new Population[mc.populationSize];

            //Fill the pool with random populations
            for (int i=0; i<mc.pool.Length; i++)
            {
                mc.pool[i] = new Population(mc.fetchRandomArray(rand));
            }

            //from the pool, create the next generation 'numberOfGen' times
            //Stops early if the best fitness has not changed in 25 generations
            for(int i=0; i<mc.numberOfGen; i++)
            {
                mc.CreateNextPool(mc);

                mc.calculateBest(mc);

                if(mc.bestStreak >=25)
                {
                    Console.WriteLine("Best fitness has not change in 25 generations. Orginally found in Generation "+(i-25));
                    break;
                }
            }
        }

        //Calculates the best fitness from the current pool and prints it
        public void calculateBest(Program mc)
        {
            int posOfBest = 0;
            for (int i = 0; i < mc.pool.Length; i++)
            {
                //if a smaller fitness is found, save its position in pool
                if (mc.pool[i].fitness < mc.pool[posOfBest].fitness)
                {
                    posOfBest = i;
                }
            }

            if (mc.currentBestFitness == mc.pool[posOfBest].fitness)
            {
                mc.bestStreak++;
            } else
            {
                mc.bestStreak = 0;
            }

            mc.currentBestFitness = mc.pool[posOfBest].fitness;
            mc.pool[posOfBest].Print();
        }

        //Collects inputs from console. The inputs are assumed correct and not checked
        public void Setup(Program mc)
        {
            Console.Write("Enter Population Size (Must be Even)(recommended 250): ");
            mc.populationSize = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter Gene Merge Chance (between 0 and 1)(recommended 0.8): ");
            mc.mergeChance = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Bit Switch Change Chance (between 0 and 1)(recommended 0.02): ");
            mc.bitSwitchChance = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Maximum Number of Generations(recommended 100): ");
            mc.numberOfGen = Convert.ToInt32(Console.ReadLine());
        }

        /*
        -From the current pool it creates an equal sized pool where the children are stored
        -At the end of the function the current pool is overwritten with the children pool
        */
        public void CreateNextPool(Program mc)
        {
            Random rand = new Random();

            Population[] childPool = new Population[mc.pool.Length];

            for (int i = 0; i < childPool.Length-1; i+=2)
            {
                //Tournament style parent selection (2 parents)
                Population parentOne = mc.Fight(mc.pool[rand.Next(0, mc.pool.Length)], mc.pool[rand.Next(0, mc.pool.Length)]);
                Population parentTwo = mc.Fight(mc.pool[rand.Next(0, mc.pool.Length)], mc.pool[rand.Next(0, mc.pool.Length)]);

                //merge the parents if within probability
                if(rand.NextDouble() < mc.mergeChance)
                {
                    childPool[i] = mc.Merge(parentOne, parentTwo);
                    childPool[i + 1] = mc.Merge(parentTwo, parentOne);
                } else //the children are the parents
                {
                    childPool[i] = parentOne;
                    childPool[i + 1] = parentTwo;
                }

                childPool[i].switchBit(rand, mc.bitSwitchChance);
                childPool[i+1].switchBit(rand, mc.bitSwitchChance);
            }
            //the current generation is overwritten with the new
            mc.pool = childPool;
        }

        //Two populations are parsed (parents), return a single child where-
        //the first half of its set is from parent a and second half from parent b
        public Population Merge(Population a, Population b)
        {
            int[] arr = new int[10];
            for(int i=0;i<5; i++)
            {
                arr[i] = a.set[i];
            }
            for (int i = 5; i < 10; i++)
            {
                arr[i] = b.set[i];
            }
            return new Population(arr);
        }

        //Two populations are parsed, the one with the smaller fitness is returned
        public Population Fight(Population a, Population b)
        {
            if(a.fitness < b.fitness)
            {
                return a;
            } else
            {
                return b;
            }
        }

        //returns an int array of size 10 where most of the values are randomly between [0, 10)
        //value a[0] is either 0 or 1. because any other value will exceed bounds
        //value a[5] is between [-2,2] because any other value will exceed bounds
        public int[] fetchRandomArray(Random rand)
        {
            int[] a = { rand.Next(0, 2), rand.Next(0, 10), rand.Next(0, 10), rand.Next(0, 10), rand.Next(0, 10), rand.Next(-2, 3), rand.Next(0, 10), rand.Next(0, 10), rand.Next(0, 10), rand.Next(0, 10)}; ;
            return a;
        }
    }
}

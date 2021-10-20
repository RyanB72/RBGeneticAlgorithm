using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
    /*
    -This class represents a single piece of the population
    -Its genes are held in 'set'. A size 10 integer array
    -The genes repersent 2 numbers. Each with a single digit and 4 decimal places

    -The bool 'yIsNeg' tells you if the value 'y' is negative. This is needed as the integer -0 does not exist

    for example the set [0,1,2,3,4,5,6,7,8,9] represents the following 2 numbers
    x = 0.1234
    y = 5.6789
    */
    public class Population
    {
        public int[] set = new int[10];
        public double x;
        public double y;
        public double fitness;
        public bool yIsNeg = false;

        //intialize the population to have a set
        //Then calculate the numbers it represents
        //Then calculate the fitness
        public Population(int[] values)
        {
            set = values;
            setDoubles(values);
            calculateFitness();
        }

        //sets the bool to true and recalculates x, y and fitness
        public void setYNegative()
        {
            yIsNeg = true;
            setDoubles(set);
            calculateFitness();
        }

        //from the gene set, get the values x and y
        private void setDoubles(int[] values)
        {
            //set the digit
            x = values[0] * 10000;
            y = values[5] * 10000;

            for(int i=1; i<5; i++)
            {
                x += values[i] * Math.Pow(10, 4-i);
            }
            x = x / 10000;

            for (int i = 6; i < 10; i++)
            {
                y += values[i] * Math.Pow(10, 9 - i);
            }

            if(yIsNeg)
            {
                y = (y / 10000)* -1;
            } else
            {
                y = y / 10000;
            }
        }

        //Loop over all pieces of the set, by the probability of 'chance' randomize a piece
        public void switchBit(Random rand, double chance)
        {
            bool changeHappened = false;
            for(int i=0; i<set.Length; i++)
            {
                if(rand.NextDouble() < chance)
                {
                    set[i] = rand.Next(0, 10);
                    changeHappened = true;
                }
            }
            
            //if the set has changed, recalculate values and fitness
            if(changeHappened)
            {
                setDoubles(set);
                calculateFitness();
            }
        }

        /*
        -Calculate the fitness of the set
        -If the values are outside the allowed params, then set a useless fitness
        -Because this is a minimization problem, a useless fitness is large (999)
        */
        private void calculateFitness()
        {
            // 0 <= x <= 1
            if(x > 1 || x < 0)
            {
                fitness = 999;
                return;
            }

            // -2 <= y <= 2
            if (y > 2 || y < -2)
            {
                fitness = 999;
                return;
            }

            //f(x) = x
            //g(x, y) = 1 + Math.Pow(y,2) - x - 0.1 * Math.Sin(3 * Math.PI * x)
            //fitness = f(x) + g(x, y)
            fitness = x + 1 + Math.Pow(y, 2) - x - 0.1 * Math.Sin(3 * Math.PI * x);
        }

        //Display inline the set, values and fitness
        //E.G. [0,7,7,9,3,0,3,5,3,4]    X = 0.7793      Y = 0.3534      Fitness = 1.037580626242288
        public void Print()
        {
            Console.Write("[");
            for (int i = 0; i < 9; i++)
            {
                Console.Write(set[i] + ",");
            }
            Console.Write(set[9] + "]");

            Console.WriteLine("\t X = "+x+ "\t Y = " + y + "\t Fitness = "+fitness);
        }
    }
}

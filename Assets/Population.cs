using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{   
    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public DNA[] DNAs;                // Array to hold the current population
    [HideInInspector] public List<DNA> matingPool;      // ArrayList which we will use for our "mating pool"
    [HideInInspector] public string target;                               // Target phrase
    [HideInInspector] public int generations;                             // Number of generations
    [HideInInspector] public bool finished;                               // Are we finished evolving?
    [HideInInspector] public bool mating;

    private float maxFitness;
    private int perfectScore;

    public void InitPopulation(string target, float m, int num, bool mating)
    {
        this.target = target;
        mutationRate = m;
        this.mating = mating;
        
        DNAs = new DNA[num];
        for (int i = 0; i < DNAs.Length; i++)
        {
            DNAs[i] = new DNA(target.Length);
        }

        matingPool = new List<DNA>();
        finished = false;
        generations = 0;

        perfectScore = 1;
    }

    public void CalculateFitness(string target)
    {
        for (int i = 0; i < DNAs.Length; i++)
        {
            DNAs[i].CalculateFitness(target);

        }
    }

    // Generate a mating pool
    public void NaturalSelection()
    {
        if (mating)
        {
            // Clear the ArrayList
            matingPool.Clear();

            float maxFitness = 0;
            for (int i = 0; i < DNAs.Length; i++)
            {
                //Console.WriteLine(dnas[i].fitness);
                if (DNAs[i].fitness > maxFitness)
                {
                    maxFitness = DNAs[i].fitness;
                }
            }

            // Based on fitness, each member will get added to the mating pool a certain number of times
            // a higher fitness = more entries to mating pool = more likely to be picked as a parent
            // a lower fitness = fewer entries to mating pool = less likely to be picked as a parent
            for (int i = 0; i < DNAs.Length; i++)
            {

                float fitness = Remap(DNAs[i].fitness, 0, maxFitness, 0, 1);
                int n = (int)(fitness * 100);  // Arbitrary multiplier, we can also use monte carlo method
                for (int j = 0; j < n; j++)
                {              // and pick two random numbers
                    matingPool.Add(DNAs[i]);
                }
            }
        }
        else
        {
            int totalScore = 0;
            for (int i = 0; i < DNAs.Length; i++)
            {
                totalScore += DNAs[i].score;
            }
            for (int i = 0; i < DNAs.Length; i++)
            {
                DNAs[i].probability = (double)DNAs[i].score / (double)totalScore;
            }
        }
    }

    // Create a new generation
    public void Generate()
    {
        DNA[] temp = new DNA[DNAs.Length];
        // Refill the population with children from the mating pool
        for (int i = 0; i < DNAs.Length; i++)
        {
            //if (matingPool.Count != 0)
            {
                DNA partnerA;
                DNA partnerB;
                if (mating)
                {
                    int a = (Random.Range(0 ,matingPool.Count));
                    int b = (Random.Range(0, matingPool.Count));
                    partnerA = matingPool[a];
                    partnerB = matingPool[b];
                }
                else
                {
                    partnerA = PickOne(DNAs);
                    partnerB = PickOne(DNAs);
                }
                DNA child = partnerA.CrossOver(partnerB);
                child.Mutate(mutationRate);
                if(mating)
                    DNAs[i] = child;
                else
                    temp[i] = child;
            }
        }
        if(!mating)
            DNAs = (DNA[])temp.Clone();
        //if (matingPool.Count != 0)
        {
            generations++;
        }
    }

    // Compute the current "most fit" member of the population
    public string Evaluate()
    {
        float worldrecord = 0.0f;
        int index = 0;
        for (int i = 0; i < DNAs.Length; i++)
        {
            //Console.WriteLine(dnas[i].fitness);
            if (DNAs[i].fitness > worldrecord)
            {
                index = i;
                worldrecord = DNAs[i].fitness;
            }
        }

        if (worldrecord == perfectScore) finished = true;
        return $"{DNAs[index].getPhrase()} {worldrecord} {Time.timeSinceLevelLoad}";
    }

    public bool IsFinish()
    {
        return finished;
    }

    public float Remap(float fitness, float from1, float to1, float from2, float to2)
    {
        return from2 + (fitness - from1) * (to2 - from2) / (to1 - from1);
    }

    private DNA PickOne(DNA[] DNAs)
    {
        int index = 0;
        double r = Random.Range(0f, 1f);

        while (r > 0)
        {
            r -= DNAs[index].probability;
            index++;
        }
        index--;

        return DNAs[index];
    }
}

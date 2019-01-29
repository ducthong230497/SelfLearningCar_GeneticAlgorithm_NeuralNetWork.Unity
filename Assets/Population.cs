using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{   
    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public DNA[] DNAs;                // Array to hold the current population
    [HideInInspector] public string target;                               // Target phrase
    [HideInInspector] public int generations;                             // Number of generations
    [HideInInspector] public bool finished;                               // Are we finished evolving?

    private float maxFitness;
    private int perfectScore;

    public void InitPopulation(string target, float m, int num, bool mating)
    {
        this.target = target;
        mutationRate = m;
        
        DNAs = new DNA[num];
        for (int i = 0; i < DNAs.Length; i++)
        {
            DNAs[i] = new DNA(target.Length);
        }
        
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

                partnerA = PickOne(DNAs);
                partnerB = PickOne(DNAs);

                DNA child = partnerA.CrossOver(partnerB);
                child.Mutate(mutationRate);
                temp[i] = child;
            }
        }
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

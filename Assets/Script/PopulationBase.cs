using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopulationBase<T>
{
    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public DNA[] DNAs;                                // Array to hold the current population
    [HideInInspector] public T target;                               // Target phrase
    [HideInInspector] public int generations;                             // Number of generations
    [HideInInspector] public bool finished;                               // Are we finished evolving?

    private float maxFitness;
    private int perfectScore;

    public abstract void InitPopulation(T target, float mutation, int num);
    public abstract void CalculateFitness(T target);
    public abstract void NaturalSelection();
    //{
    //    int totalScore = 0;
    //    for (int i = 0; i < DNAs.Length; i++)
    //    {
    //        totalScore += DNAs[i].score;
    //    }
    //    for (int i = 0; i < DNAs.Length; i++)
    //    {
    //        DNAs[i].probability = (double)DNAs[i].score / (double)totalScore;
    //    }
    //}
    // Create a new generation
    public abstract void Generate();
    //{
    //    DNA[] temp = new DNA[DNAs.Length];
    //    // Refill the population with children from the mating pool
    //    for (int i = 0; i < DNAs.Length; i++)
    //    {
    //        //if (matingPool.Count != 0)
    //        {
    //            DNA partnerA;
    //            DNA partnerB;

    //            partnerA = PickOne(DNAs);
    //            partnerB = PickOne(DNAs);

    //            DNA child = partnerA.CrossOver(partnerB);
    //            child.Mutate(mutationRate);
    //            temp[i] = child;
    //        }
    //    }
    //    DNAs = (DNA[])temp.Clone();
    //    //if (matingPool.Count != 0)
    //    {
    //        generations++;
    //    }
    //}
    // Compute the current "most fit" member of the population
    public abstract string Evaluate();
    //{
    //    float worldrecord = 0.0f;
    //    int index = 0;
    //    for (int i = 0; i < DNAs.Length; i++)
    //    {
    //        //Console.WriteLine(dnas[i].fitness);
    //        if (DNAs[i].fitness > worldrecord)
    //        {
    //            index = i;
    //            worldrecord = DNAs[i].fitness;
    //        }
    //    }

    //    if (worldrecord == perfectScore) finished = true;
    //    return $"{DNAs[index].getPhrase()} {worldrecord} {Time.timeSinceLevelLoad}";
    //}
}

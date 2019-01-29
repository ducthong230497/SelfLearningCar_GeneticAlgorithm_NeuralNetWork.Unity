using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    // The genetic sequence
    public char[] genes;

    public float fitness;
    public int score;
    public double probability;
    // Constructor (makes a random DNA)
    public DNA(int num)
    {
        genes = new char[num];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = NewChar();  // Pick from range of chars
        }
    }

    // Converts character array to a String
    public string getPhrase()
    {
        return new string(genes);
    }

    public void CalculateFitness(string target)
    {
        score = 0;
        for (int i = 0; i < genes.Length; i++)
        {
            if (genes[i] == target[i])
            {
                score++;
                if(score == target.Length)
                {
                    int a = 2;
                }
            }
        }
        fitness = (float)score / (float)genes.Length;
    }

    // Crossover
    public DNA CrossOver(DNA partner)
    {
        // A new child
        DNA child = new DNA(genes.Length);

        int midpoint = (Random.Range(0, genes.Length)); // Pick a midpoint

        // Half from one, half from the other
        for (int i = 0; i < genes.Length; i++)
        {
            if (i > midpoint) child.genes[i] = genes[i];
            else child.genes[i] = partner.genes[i];
            //int test = rand.Next();
            //if (test % 2 == 0)
            //{
            //    child.genes[i] = genes[i];
            //}
            //else
            //{
            //    child.genes[i] = partner.genes[i];
            //}
        }
        return child;
    }

    // Based on a mutation probability, picks a new random character
    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < genes.Length; i++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                genes[i] = NewChar();
            }
        }
    }

    private char NewChar()
    {
        int ret = Random.Range(32, 126);
        return (char)ret;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPopulation : MonoBehaviour
{
    public GameObject agentPrefab;
    public float agentMaxSpeed = 10;
    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public List<GameObject> agents;                // Array to hold the current population
    [HideInInspector] public List<GameObject> matingPool;      // ArrayList which we will use for our "mating pool"
    [HideInInspector] public Vector3 target;                               // Target phrase
    [HideInInspector] public int generations;                             // Number of generations
    [HideInInspector] public bool finished;                               // Are we finished evolving?
    [HideInInspector] public bool mating;

    private Vector3 spawnPosition;
    private float maxFitness;
    private int perfectScore;
    private int currentBestAgent;

    public void InitPopulation(Vector3 target, Vector3 spawnPos, float m, int numberAgent, int brainSize, bool mating)
    {
        this.target = target;
        mutationRate = m;
        this.mating = mating;
        spawnPosition = spawnPos;
        generations = 1;
        finished = false;
        perfectScore = 1;

        agents = new List<GameObject>();
        for (int i = 0; i < numberAgent; i++)
        {
            agents.Add(Instantiate(agentPrefab, spawnPosition, Quaternion.identity));
            agents[i].GetComponent<Agent>().InitAgent(brainSize, Vector3.SqrMagnitude(target - spawnPosition));
            agents[i].name = $"Agent {i + 1}";
        }

        matingPool = new List<GameObject>();
    }

    public void CalculateFitness()
    {
        currentBestAgent = -1;
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].GetComponent<Agent>().CalculateFitness(target, spawnPosition);
        }
    }

    public void NaturalSelection()
    {
        if (mating)
        {
            // Clear the ArrayList
            matingPool.Clear();

            float maxFitness = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                //Console.WriteLine(dnas[i].fitness);
                if (agents[i].GetComponent<Agent>().fitness > maxFitness)
                {
                    maxFitness = agents[i].GetComponent<Agent>().fitness;
                }
            }

            // Based on fitness, each member will get added to the mating pool a certain number of times
            // a higher fitness = more entries to mating pool = more likely to be picked as a parent
            // a lower fitness = fewer entries to mating pool = less likely to be picked as a parent
            for (int i = 0; i < agents.Count; i++)
            {

                float fitness = Remap(agents[i].GetComponent<Agent>().fitness, 0, maxFitness, 0, 1);
                int n = (int)(fitness * 100);  // Arbitrary multiplier, we can also use monte carlo method
                for (int j = 0; j < n; j++)
                {              // and pick two random numbers
                    matingPool.Add(agents[i]);
                }
            }
        }
        else
        {
            //float totalScore = 0;
            //for (int i = 0; i < agents.Count; i++)
            //{
            //    totalScore += agents[i].GetComponent<Agent>().score;
            //}
            float totalFitness = 0;
            for (int i = 0; i < agents.Count; i++)
            {
                totalFitness += agents[i].GetComponent<Agent>().fitness;
            }
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].GetComponent<Agent>().probability = (double)agents[i].GetComponent<Agent>().fitness / (double)totalFitness;
            }
            //double sumProb = 0;
            //for (int i = 0; i < agents.Count; i++)
            //{
            //    sumProb += agents[i].GetComponent<Agent>().probability;
            //}
            //Debug.Log($"Sum of probability: {sumProb}");
        }
    }

    public string Generate()
    {
        List<Vector3[]> temp = new List<Vector3[]>();
        // Refill the population with children from the mating pool
        for (int i = 0; i < agents.Count; i++)
        {
            if(i == currentBestAgent)
            {
                temp.Add(agents[i].GetComponent<Agent>().directions);
                continue;
            }
            //if (matingPool.Count != 0)
            {
                Agent partnerA;
                Agent partnerB;
                if (mating)
                {
                    int a = (Random.Range(0, matingPool.Count));
                    int b = (Random.Range(0, matingPool.Count));
                    partnerA = matingPool[a].GetComponent<Agent>();
                    partnerB = matingPool[b].GetComponent<Agent>();
                }
                else
                {
                    partnerA = PickOne(agents);
                    partnerB = PickOne(agents);
                }
                Vector3[] newDirections = partnerA.CrossOver(partnerB);
                Mutate(ref newDirections, mutationRate);
                if (mating)
                    agents[i].GetComponent<Agent>().directions = newDirections;
                else
                    temp.Add(newDirections);
            }
        }
        if (!mating)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].GetComponent<Agent>().directions = (Vector3[])temp[i].Clone();
            }
        }
        temp.Clear();
        //if (matingPool.Count != 0)
        {
            //generations++;
        }

        //StartCoroutine(RespawnAgent());
        return RespawnAgent();
    }

    // Compute the current "most fit" member of the population
    public (string, string, float) Evaluate()
    {
        int index = 0;
        float bestScore = -1;
        int sumReachTarget = 0;
        for (int i = 0; i < agents.Count; i++)
        {
            if (bestScore < agents[i].GetComponent<Agent>().fitness)
            {
                currentBestAgent = i;
                bestScore = agents[i].GetComponent<Agent>().fitness;
            }
            if (agents[i].GetComponent<Agent>().reachTarget)
            {
                sumReachTarget++;
            }
        }
        if(sumReachTarget == agents.Count)
        {
            //finished = true;
            Debug.Log($"Sum target reach goal: {sumReachTarget}");
        }
        return (agents[index].gameObject.name, finished ? generations.ToString() : (++generations).ToString(), Time.timeSinceLevelLoad);
    }

    private Agent PickOne(List<GameObject> agents)
    {
        int index = 0;
        double r = Random.Range(0f, 1f);

        while (r > 0)
        {
            r -= agents[index].GetComponent<Agent>().probability;
            index++;
        }
        index--;

        return agents[index].GetComponent<Agent>();
    }

    private string RespawnAgent()
    {
        //yield return new WaitForSeconds(.01f);
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].GetComponent<Agent>().Respawn(spawnPosition);
        }
        agents[currentBestAgent].GetComponent<Agent>().SetChampion();
        return agents[currentBestAgent].gameObject.name;
    }

    public bool IsFinish()
    {
        return finished;
    }

    public float Remap(float fitness, float from1, float to1, float from2, float to2)
    {
        return from2 + (fitness - from1) * (to2 - from2) / (to1 - from1);
    }

    // Based on a mutation probability, picks a new random character
    public void Mutate(ref Vector3[] newDirections, float mutationRate)
    {
        for (int i = 0; i < newDirections.Length; i++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                newDirections[i] = new Vector3(Random.Range(-10f, 11), 0, Random.Range(10, -11));
            }
        }
    }

    public bool AllDead()
    {
        foreach (var agent in agents)
        {
            if (!agent.GetComponent<Agent>().IsDead())
            {
                return false;
            }
        }
        return true;
    }

    public void MoveAgents()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            if(agents[i].GetComponent<Rigidbody>().velocity.sqrMagnitude <= Mathf.Pow(agentMaxSpeed, 2))
            {
                agents[i].GetComponent<Agent>().MoveAgent();
            }
        }
    }
}

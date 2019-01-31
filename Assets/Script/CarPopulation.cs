using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CarPopulation : MonoBehaviour
{
    public GameObject carPrefab;
    public int numberOfCars;
    public float carMaxSpeed;

    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public List<GameObject> cars;                // Array to hold the current population
    //[HideInInspector] public string target;                               // Target phrase
    [HideInInspector] public int generations;                             // Number of generations
    [HideInInspector] public bool finished;                               // Are we finished evolving?

    private Vector3 spawnPosition;
    private float maxFitness;
    private int perfectScore;
    private int outputNodes;
    private int bestCar;

    public void InitPopulation(float mutation, int inputNodes, int hiddenNodes, int outputNodes)
    {
        spawnPosition = carPrefab.transform.position;
        //this.target = target;
        mutationRate = mutation;
        finished = false;
        generations = 1;
        perfectScore = 1;
        this.outputNodes = outputNodes;

        cars = new List<GameObject>();
        for (int i = 0; i < numberOfCars; i++)
        {
            cars.Add(Instantiate(carPrefab, carPrefab.transform.position, carPrefab.transform.rotation));
            cars[i].GetComponent<CarDNA>().InitCar(inputNodes, hiddenNodes, outputNodes);
            cars[i].name = $"Car {i + 1}";
        }

    }

    public void CalculateFitness()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].GetComponent<CarDNA>().CalculateFitness(spawnPosition);

        }
    }

    // Generate a mating pool
    public void NaturalSelection()
    {
        float totalScore = 0;
        for (int i = 0; i < cars.Count; i++)
        {
            totalScore += cars[i].GetComponent<CarDNA>().score;
        }
        float totalFitness = 0;
        for (int i = 0; i < cars.Count; i++)
        {
            totalFitness += cars[i].GetComponent<CarDNA>().fitness;
        }
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].GetComponent<CarDNA>().probability = (double)cars[i].GetComponent<CarDNA>().fitness / (double)totalFitness;
        }
    }

    // Create a new generation
    public void Generate()
    {
        NeuralNetwork[] temp = new NeuralNetwork[cars.Count];
        // Refill the population with children from the mating pool
        for (int i = 0; i < cars.Count - 1; i++)
        {
            if(i == bestCar)
            {
                temp[i] = cars[i].GetComponent<CarDNA>().neuralNetwork;
            }
            else
            {
                CarDNA partnerA = PickOne(cars);
                CarDNA partnerB = PickOne(cars);
                NeuralNetwork child = partnerA.CrossOver(partnerB);
                Mutate(ref child, mutationRate);
                temp[i] = child;
            }
        }
        for (int i = 0; i < cars.Count - 1; i++)
        {
            cars[i].GetComponent<CarDNA>().neuralNetwork = temp[i];
        }
        cars[cars.Count - 1].GetComponent<CarDNA>().neuralNetwork = new NeuralNetwork(6, 5, 2);
        //if (matingPool.Count != 0)
        {
            generations++;
        }
        RestartCars();
    }

    // Compute the current "most fit" member of the population
    public string Evaluate()
    {
        float worldrecord = 0.0f;
        for (int i = 0; i < cars.Count; i++)
        {
            //Console.WriteLine(dnas[i].fitness);
            if (cars[i].GetComponent<CarDNA>().fitness > worldrecord)
            {
                worldrecord = cars[i].GetComponent<CarDNA>().fitness;
                bestCar = i;
            }
            if (cars[i].GetComponent<CarBehaviour>().finish)
            {
                finished = true;
            }
        }
        File.WriteAllBytes("Assets/Training_Result/bestCar.txt", cars[bestCar].GetComponent<CarDNA>().neuralNetwork.ToByteArray());
        //if (worldrecord == perfectScore) finished = true;
        return $"{generations}";
    }

    // Based on a mutation probability, picks a new random character
    public void Mutate(ref NeuralNetwork newNeural, float mutationRate)
    {
        DoMutate(ref newNeural.ihWeights, mutationRate);
        DoMutate(ref newNeural.hoWeights, mutationRate);
        DoMutate(ref newNeural.biasH, mutationRate);
        DoMutate(ref newNeural.biasO, mutationRate);
    }

    private void DoMutate(ref Matrix m, float mutaionRate)
    {
        for(int i = 0; i < m.rowNb; i++)
        {
            for(int j = 0; j < m.columnNb; j++)
            {
                if(Random.Range(0f, 1f) < mutaionRate)
                {
                    m[i][j] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public bool IsFinish()
    {
        return finished;
    }

    public bool AllOff()
    {
        foreach (var car in cars)
        {
            if (!car.GetComponent<CarBehaviour>().off)
            {
                return false;
            }
        }
        return true;
    }

    public void RestartCars()
    {
        foreach (var car in cars)
        {
            car.GetComponent<CarBehaviour>().RestartCar(carPrefab.transform.position, carPrefab.transform.rotation);
        }
    }

    public void RunCars()
    {
        float[] output = new float[outputNodes];
        float[] axis;
        foreach (var car in cars)
        {
            if (!car.GetComponent<CarBehaviour>().off /*&& car.GetComponent<Rigidbody>().velocity.sqrMagnitude <= Mathf.Pow(carMaxSpeed, 2)*/)
            {
                output = car.GetComponent<CarBehaviour>().GetOutput();
                axis = car.GetComponent<CarBehaviour>().GetAxisFromOutput(output);
                car.GetComponent<CarBehaviour>().RunCar(axis);
            }
        }
    }

    public void UpdateDriveTime()
    {
        foreach (var car in cars)
        {
            car.GetComponent<CarBehaviour>().UpdateDriveTime(carPrefab.transform.position);
        }
    }

    public void ShutDownCars()
    {
        foreach (var car in cars)
        {
            car.GetComponent<CarBehaviour>().ShutDownCar();
        }
    }

    public float Remap(float fitness, float from1, float to1, float from2, float to2)
    {
        return from2 + (fitness - from1) * (to2 - from2) / (to1 - from1);
    }

    private CarDNA PickOne(List<GameObject> cars)
    {
        int index = 0;
        double r = Random.Range(0f, 1f);

        while (r > 0)
        {
            r -= cars[index].GetComponent<CarDNA>().probability;
            index++;
        }
        index--;

        return cars[index].GetComponent<CarDNA>();
    }
}

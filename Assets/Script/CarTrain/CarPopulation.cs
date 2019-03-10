using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class CarPopulation : MonoBehaviour
{
    public Transform carParent;
    public GameObject carPrefab;
    public GameObject bestCarPrefab;
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
    private int inputNodes;
    private int hiddenNodes;
    private int outputNodes;
    private int bestCar;
    private bool trainingMode;
    Vector3 SmoothPosVelocity = Vector3.zero; // Velocity of Position Smoothing

    public void InitPopulation(float mutation, int inputNodes, int hiddenNodes, int outputNodes, Vector3 spawnPos, bool trainingMode)
    {
        this.trainingMode = trainingMode;
        if (trainingMode)
        {
            //spawnPosition = carPrefab.transform.position;
            spawnPosition = spawnPos;
            //this.target = target;
            mutationRate = mutation;
            finished = false;
            generations = 1;
            perfectScore = 1;
            this.inputNodes = inputNodes;
            this.hiddenNodes = hiddenNodes;
            this.outputNodes = outputNodes;

            cars = new List<GameObject>();
            for (int i = 0; i < numberOfCars; i++)
            {
                cars.Add(Instantiate(carPrefab, spawnPosition, carPrefab.transform.rotation, carParent));
                cars[i].GetComponent<CarDNA>().InitCar(inputNodes, hiddenNodes, outputNodes);
                cars[i].name = $"Car {i + 1}";
            }
        }
        else
        {
            cars.Add(Instantiate(bestCarPrefab, bestCarPrefab.transform.position, bestCarPrefab.transform.rotation));
            cars[0].GetComponent<CarDNA>().InitCar(inputNodes, hiddenNodes, outputNodes);
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
            if (i == bestCar)
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
        cars[cars.Count - 1].GetComponent<CarDNA>().neuralNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
        //if (matingPool.Count != 0)
        {
            generations++;
        }
        RestartCars(spawnPosition);
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
        for (int i = 0; i < m.rowNb; i++)
        {
            for (int j = 0; j < m.columnNb; j++)
            {
                if (Random.Range(0f, 1f) < mutaionRate)
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

    public void RestartCars(Vector3 spawnPos)
    {
        foreach (var car in cars)
        {
            car.GetComponent<CarBehaviour>().RestartCar(spawnPos, carPrefab.transform.rotation);
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

        if (!trainingMode)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, cars[0].transform.GetChild(1).position, ref SmoothPosVelocity, 0.7f); // Smoothly set the position

            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation,
                                                             Quaternion.LookRotation(cars[0].transform.position - Camera.main.transform.position),
                                                             0.1f); // Smoothly set the rotation
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

    private void ReadBestCarTrainedData(string filePath, ref NeuralNetwork neuralNetwork)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string str = Encoding.ASCII.GetString(bytes);
        string[] data = str.Split('\n');
        int i = 0;
        for (; i < neuralNetwork.ihWeights.rowNb; i++)
        {
            int j = 0;
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (; j < neuralNetwork.ihWeights.columnNb; j++)
            {
                neuralNetwork.ihWeights[i][j] = float.Parse(number[j]);
            }
        }
        int t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < 5; j++)
            {
                neuralNetwork.hoWeights[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb + neuralNetwork.biasH.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < neuralNetwork.biasH.columnNb; j++)
            {
                neuralNetwork.biasH[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb + neuralNetwork.biasH.rowNb + neuralNetwork.biasO.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < neuralNetwork.biasO.columnNb; j++)
            {
                neuralNetwork.biasO[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        int b = 2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralController : MonoBehaviour
{
    [Header("Neural Network")]
    public      float           raycastDistance;
    [Tooltip("add a little distance to front ray")]
    public      float           additional;
    public      int             inputNodes;
    public      int             hiddenNodes;
    public      int             outputNodes;

    [Header("Genetic Algorithm")]
    public      float           mutationRate;
    

    private     CarPopulation   carPopulation;
    private     float[]         inputArr;

    // Start is called before the first frame update
    void Awake()
    {
        carPopulation = GetComponent<CarPopulation>();

        carPopulation.InitPopulation(mutationRate, inputNodes, hiddenNodes, outputNodes);

        inputArr            = new float[inputNodes];
    }

    private void FixedUpdate()
    {
        if (carPopulation.AllOff())
        {
            carPopulation.CalculateFitness();

            carPopulation.NaturalSelection();

            carPopulation.Evaluate();

            carPopulation.Generate();
        }
        else
        {
            carPopulation.RunCars();
            carPopulation.UpdateDriveTime();
            carPopulation.ShutDownCars();
        }
    }
}

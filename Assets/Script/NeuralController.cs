using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class NeuralController : MonoBehaviour
{
    [Header("Neural Network")]
    public      int                 inputNodes;
    public      int                 hiddenNodes;
    public      int                 outputNodes;

    [Header("Genetic Algorithm")]
    public      float               mutationRate;

    [Header("Timescale")]
    public      float               timeScale;

    [Header("UI")]
    public      TextMeshProUGUI     textMeshPro;

    private     CarPopulation       carPopulation;
    private     float[]             inputArr;

    // Start is called before the first frame update
    void Awake()
    {
        carPopulation = GetComponent<CarPopulation>();

        carPopulation.InitPopulation(mutationRate, inputNodes, hiddenNodes, outputNodes);

        inputArr = new float[inputNodes];
    }

    private void Update()
    {
        if(Time.timeScale != timeScale)
        {
            Time.timeScale = timeScale;
        }
    }

    private void FixedUpdate()
    {
        if (!carPopulation.IsFinish())
        {
            if (carPopulation.AllOff())
            {
                carPopulation.CalculateFitness();

                carPopulation.NaturalSelection();

                textMeshPro.text = $"Generation: {carPopulation.Evaluate()}";

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
}

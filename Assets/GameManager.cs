using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string target;
    public float mutationRate;
    public int numberOfPopulation;
    public TextMeshProUGUI text;
    public bool useMatingPool;
    private Population population;
    // Start is called before the first frame update
    void Start()
    {
        population = GetComponent<Population>();
        population.InitPopulation(target, mutationRate, numberOfPopulation, useMatingPool);
    }

    // Update is called once per frame
    void Update()
    {
        if (!population.IsFinish())
        {
            population.CalculateFitness(target);

            population.NaturalSelection();

            text.text = population.Evaluate();

            population.Generate();
        }
    }
}

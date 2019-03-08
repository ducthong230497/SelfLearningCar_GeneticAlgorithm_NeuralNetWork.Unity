using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TobeManager : MonoBehaviour
{
    #region To be or not to be
    public string target;
    public float mutationRate;
    public int numberOfPopulation;
    public bool useMatingPool;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;

    private Population population;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region To be or not to be
        population = GetComponent<Population>();
        population.InitPopulation(target, mutationRate, numberOfPopulation, useMatingPool);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region To be or not to be
        if (!population.IsFinish())
        {
            population.CalculateFitness(target);

            population.NaturalSelection();

            var result = population.Evaluate();

            text1.text = result.Item1;

            text2.text = $"percent: {result.Item2}";

            text3.text = $"time: {result.Item3}";

            population.Generate();
        }
        #endregion
    }
}

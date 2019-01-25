using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region To be or not to be
    //public string target;
    //public float mutationRate;
    //public int numberOfPopulation;
    //public bool useMatingPool;

    //private Population population;
    #endregion

    public TextMeshProUGUI textGeneration;
    public TextMeshProUGUI textBestAgent;
    public TextMeshProUGUI textPreviousBestAgent;
    public Transform target;
    public Transform spawnPos;
    public float mutationRate;
    public int numberOfAgent;
    public int brainSize;
    public bool mating;

    private AgentPopulation agentPopulation;
    // Start is called before the first frame update
    void Start()
    {
        #region To be or not to be
        //population = GetComponent<Population>();
        //population.InitPopulation(target, mutationRate, numberOfPopulation, useMatingPool);
        #endregion

        textGeneration.text = "<color=red>Generation: 1</color>";
        agentPopulation = GetComponent<AgentPopulation>();
        agentPopulation.InitPopulation(target.position, spawnPos.position, mutationRate, numberOfAgent, brainSize, mating);
    }

    // Update is called once per frame
    void Update()
    {
        #region To be or not to be
        //if (!population.IsFinish())
        //{
        //    population.CalculateFitness(target);

        //    population.NaturalSelection();

        //    text.text = population.Evaluate();

        //    population.Generate();
        //}
        #endregion
    }

    private void FixedUpdate()
    {
        if(!agentPopulation.IsFinish())
        {
            if (agentPopulation.AllDead())
            {
                agentPopulation.CalculateFitness();

                agentPopulation.NaturalSelection();

                (bool, string, string, float) info = agentPopulation.Evaluate();

                textGeneration.text = $"<color=red>Generation: {info.Item3}</color>";

                textPreviousBestAgent.text = $"<color=red>Previous best agent: {agentPopulation.Generate()}</color>";

                Debug.Log("All agents are dead");
            }
            else
            {
                agentPopulation.MoveAgents();
            }
        }
    }
}

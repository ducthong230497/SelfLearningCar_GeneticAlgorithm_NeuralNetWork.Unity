using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private Material aliveMaterial;
    [SerializeField] private Material dieMaterial;
    [SerializeField] private Material championMaterial;
    [SerializeField] private int      fitnessPow;

    [HideInInspector] public Vector3[] directions;
    [HideInInspector] public float moveSpeed = 10;
    [HideInInspector] public float fitness;
    [HideInInspector] public double probability;
    [HideInInspector] public bool reachTarget;
    [HideInInspector] public int iterator;

    private MeshRenderer meshRenderer;
    private Rigidbody rigidbody;
    private float sqrDistance;
    private bool dead;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void InitAgent(int brainSize, float distance)
    {
        sqrDistance = distance;
        iterator = 0;
        directions = new Vector3[brainSize];
        for (int i = 0; i < directions.Length; i++)
        {
            directions[i] = new Vector3(Random.Range(-10f, 11), 0, Random.Range(10, -11));
        }
    }

    public void CalculateFitness(Vector3 target, Vector3 spawnPos)
    {
        //fitness = 1 - Mathf.Clamp01((Vector3.SqrMagnitude(target - gameObject.transform.position) / sqrDistance));
        fitness = 1 / Mathf.Pow(Vector3.SqrMagnitude(target - gameObject.transform.position), fitnessPow);
        //score = fitness > 0 ? Vector3.SqrMagnitude(gameObject.transform.position - spawnPos) : 0;
        //Debug.Log($"fitness of {gameObject.name}: {fitness} score: {score}");
    }

    // Crossover
    public Vector3[] CrossOver(Agent partner)
    {
        Vector3[] childDirections = new Vector3[directions.Length];

        int midpoint = (Random.Range(0, directions.Length)); // Pick a midpoint

        // Half from one, half from the other
        for (int i = 0; i < directions.Length; i++)
        {
            //if (i > midpoint) childDirections[i] = directions[i];
            //else childDirections[i] = partner.directions[i];
            
            if (Random.Range(0, 3) == 0)
            {
                childDirections[i] = directions[i];
            }
            else
            {
                childDirections[i] = partner.directions[i];
            }
        }
        return childDirections;
    }

    public void MoveAgent()
    {
        if (!dead)
        {
            rigidbody.AddForce(directions[iterator] * moveSpeed);
            iterator++;
            if(iterator >= directions.Length)
            {
                OnAgentDead();
            }
        }
    }

    public bool IsDead()
    {
        return dead;
    }

    public void SetChampion()
    {
        meshRenderer.material = championMaterial;
    }

    public void Respawn(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        meshRenderer.material = aliveMaterial;
        rigidbody.velocity = Vector3.zero;
        iterator = 0;
        dead = false;
    }

    public void KillAgent()
    {
        OnAgentDead();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {
            OnAgentDead();
        }
        else if (collision.gameObject.tag.Equals("Goal"))
        {
            reachTarget = true;
            OnAgentDead();
        }
    }

    private void OnAgentDead()
    {
        dead = true;
        meshRenderer.material = dieMaterial;
    }
}

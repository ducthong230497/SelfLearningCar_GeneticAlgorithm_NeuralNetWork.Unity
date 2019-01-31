using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [Range(-2, 2)]
    public int angleVelocity;
    NeuralNetwork neuralNetwork;
    Transform headObject;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        neuralNetwork = new NeuralNetwork(5, 5, 2);
        headObject = transform.Find("Head");
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float[] input = new float[5];
        RaycastHit hitInfo;
        for (int i = 0; i < 5; i++)
        {
            if (i * -45 == -90)
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (22.5f), Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, (22.5f), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (22.5f), Color.red);
                }
            }
            else
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (8.5f), Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, (8.5f), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (8.5f), Color.red);
                }
            }
        }
        var output = neuralNetwork.FeedForward(input);
        Debug.Log($"{output[0]} {output[1]}");

    }

    private void FixedUpdate()
    {
        rigidbody.angularVelocity = transform.up * angleVelocity;
    }
}

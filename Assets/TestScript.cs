using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class TestScript : MonoBehaviour
{
    [Range(-2, 2)]
    public int angleVelocity;
    [Tooltip("Default is 22.5")]
    public float raycastFrontDistance = 22.5f;
    [Tooltip("Default is 4")]
    public float raycastOthersDistance = 4;


    NeuralNetwork neuralNetwork;
    Transform headObject;
    Rigidbody rigidbody;
    string[] data;
    // Start is called before the first frame update
    void Start()
    {
        neuralNetwork = new NeuralNetwork(6, 5, 2);
        headObject = transform.Find("Head");
        rigidbody = GetComponent<Rigidbody>();
        byte[] bytes = File.ReadAllBytes("Assets/Training_Result/test.txt");
        string str = Encoding.ASCII.GetString(bytes);
        data = str.Split('\n');
        int i = 0;
        for (; i < neuralNetwork.ihWeights.rowNb; i++)
        {
            int j = 0;
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for(; j < neuralNetwork.ihWeights.columnNb; j++)
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

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        float[] input = new float[6];
        RaycastHit hitInfo;
        for (int i = 0; i < 5; i++)
        {
            if (i * -45 == -90)
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * raycastFrontDistance, Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, (raycastFrontDistance), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (raycastFrontDistance), Color.red);
                }
                else
                {
                    input[i] = 4;
                }
            }
            else
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * raycastOthersDistance, Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, raycastOthersDistance, LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * raycastOthersDistance, Color.red);
                }
                else
                {
                    input[i] = 4;
                }
            }
        }
        Debug.DrawRay(headObject.position, (Quaternion.Euler(0, 90, 0) * transform.right).normalized * raycastOthersDistance, Color.green);
        if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, 90, 0) * transform.right).normalized, out hitInfo, raycastOthersDistance, LayerMask.GetMask("Wall")))
        {
            input[5] = hitInfo.distance;
            Debug.DrawRay(headObject.position, (Quaternion.Euler(0, 90, 0) * transform.right).normalized * raycastOthersDistance, Color.red);
        }
        else
        {
            input[5] = 4;
        }
        var output = neuralNetwork.FeedForward(input);
        Debug.Log($"{output[0]} {output[1]}");
        float vertical;
        float horizontal;
        if (output[1] <= 0.25f)
        {
            horizontal = -1;
        }
        else if (output[0] >= 0.65f)
        {
            horizontal = 1;
        }
        else
        {
            horizontal = 0;
        }

        if (output[1] <= 0.25)
        {
            vertical = -1;
        }
        else if (output[0] >= 0.65)
        {
            vertical = 1;
        }
        else
        {
            vertical = 0;
        }

        // If the output is just standing still, then move the car forward
        if (vertical == 0 && horizontal == 0)
            vertical = 1;
        rigidbody.angularVelocity = transform.up * horizontal * 3;
        rigidbody.velocity = (transform.forward * vertical * 4);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Goal"))
        {
            gameObject.SetActive(false);
        }
    }
}

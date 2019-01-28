using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralController : MonoBehaviour
{
    public      float           raycastDistance;
    [Tooltip("add a little distance to front ray")]
    public      float           additional;
    public      int             inputNodes;
    public      int             hiddenNodes;
    public      int             outputNodes;

    private     NeuralNetwork   neuralNetwork;
    private     Transform       headObject;
    private     float[]         inputArr;

    // Start is called before the first frame update
    void Start()
    {
        neuralNetwork       = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);

        headObject          = transform.Find("Head");

        inputArr            = new float[inputNodes];
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        //foreach (var rayDir in raycastDirection)
        //{
        //    if(rayDir == transform.forward)
        //    {
        //        Debug.DrawRay(headObject.position, rayDir * (raycastDistance + offset), Color.red);
        //        if (Physics.Raycast(headObject.position, rayDir, out hitInfo, raycastDistance + offset, LayerMask.GetMask("Wall")))
        //        {
        //            Debug.Log($"Hit forward: {hitInfo.distance}");
        //        }
        //    }
        //    Debug.DrawRay(headObject.position, rayDir * raycastDistance, Color.red);
        //    if (Physics.Raycast(headObject.position, rayDir, out hitInfo, raycastDistance, LayerMask.GetMask("Wall")))
        //    {
        //        Debug.Log($"Hit {rayDir.ToString()}: {hitInfo.distance}");
        //    }
        //}

        for (int i = 0; i < inputNodes; i++)
        {
            Vector3 direction = (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized;
            float distance = i * -45 == -90 ? raycastDistance + additional : raycastDistance;
            Debug.DrawRay(headObject.position, direction * distance, Color.red);
            if (Physics.Raycast(headObject.position, direction, out hitInfo, distance, LayerMask.GetMask("Wall")))
            {
                Debug.Log($"Hit {direction}: {hitInfo.distance}");
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(transform.up * Time.deltaTime * 10);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(-transform.up * Time.deltaTime * 10, Space.World);
        }
    }

    private void FixedUpdate()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    public float moveForce;
    public ForceMode forceMode;

    private Rigidbody rigidbody;
    private CarDNA carDNA;
    public bool off;
    [HideInInspector] public float driveTime;

    //Test
    Transform headObject;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        carDNA = GetComponent<CarDNA>();

        //TEST
        headObject = transform.Find("Head");
        carDNA.InitCar(5, 5, 2);
    }

    public float[] GetOutput()
    {
        float[] input = new float[5];
        RaycastHit hitInfo;
        for (int i = 0; i < 5; i++)
        {
            if (i * 45 == -90)
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * 5.5f, Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, 5.5f, LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * 5.5f, Color.red);
                }
            }
            else
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * 4.5f, Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, 4.5f, LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * 4.5f, Color.red);
                }
            }
        }
        return carDNA.neuralNetwork.FeedForward(input);
    }

    public void MoveCar(float[] output)
    {
        rigidbody.AddForce(transform.forward * moveForce * output[1], forceMode);
    }

    public void RestartCar(Vector3 spawnPos)
    {
        off = false;
        transform.position = spawnPos;
    }

    public void ShutDownCar()
    {
        if (driveTime > 3 && rigidbody.velocity.sqrMagnitude < 0.000025)
        {
            off = true;
        }
    }

    public void UpdateDriveTime()
    {
        driveTime += Time.fixedDeltaTime;
    }
}

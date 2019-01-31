using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CarBehaviour : MonoBehaviour
{
    public float moveForce;
    public ForceMode forceMode;
    public float raycastDistance;
    [Tooltip("add a little distance to front ray")]
    public float additional;
    public bool off;

    private Rigidbody rigidbody;
    private CarDNA carDNA;
    private float oldFitness;
    private float newFitness;
    [HideInInspector] public float driveTime;
    [HideInInspector] public bool finish;

    //Test
    Transform headObject;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        carDNA = GetComponent<CarDNA>();
        headObject = transform.Find("Head");

        //TEST
        //carDNA.InitCar(5, 5, 2);
    }

    public float[] GetOutput()
    {
        float[] input = new float[5];
        RaycastHit hitInfo;
        for (int i = 0; i < 5; i++)
        {
            if (i * -45 == -90)
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (raycastDistance + additional), Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, (raycastDistance + additional), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (raycastDistance + additional), Color.red);
                }
            }
            else
            {
                Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (raycastDistance), Color.green);
                if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized, out hitInfo, (raycastDistance), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(headObject.position, (Quaternion.Euler(0, i * -45, 0) * transform.right).normalized * (raycastDistance), Color.red);
                }
            }
        }
        return carDNA.neuralNetwork.FeedForward(input);
    }

    public (float, float) GetAxisFromOutput(float[] output)
    {
        float vertical;
        float horizontal;
        if(output[0] <= 0.25f)
        {
            horizontal = -1;
        }
        else if(output[0] >= 0.5f)
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
        else if (output[1] >= 0.5)
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
        Debug.Log($"{output[0]} {output[1]} {vertical} {horizontal}");
        return (horizontal, vertical);
    }

    public void RunCar((float, float) axis)
    {
        //transform.rotation = Quaternion.Euler(0, output[0] * Mathf.Rad2Deg, 0);
        rigidbody.angularVelocity = transform.up * axis.Item1 * 3;
        rigidbody.velocity = (transform.forward * axis.Item2 * 4);

        //calculate by how much to rotate and a new angle.
        //float rightForce = output[0];
        //float leftForce = output[1];

        //float angleChange = (rightForce - leftForce) * 180 * Time.deltaTime;
        //float headingAngle = -transform.rotation.eulerAngles.y + angleChange;
        //float radian = headingAngle * Mathf.PI / 180;

        ////set new angle
        //transform.eulerAngles += new Vector3(0, angleChange, 0);
        ////set velocity based on direction
        //rigidbody.velocity = new Vector3(moveForce * Mathf.Cos(radian),
        //                                  0,
        //                                  moveForce * Mathf.Sin(radian));
    }

    public void RestartCar(Vector3 spawnPos, Quaternion spawnRotation)
    {
        off = false;
        transform.position = spawnPos;
        transform.rotation = spawnRotation;
        oldFitness = newFitness = 0;
    }

    public void ShutDownCar()
    {
        if (driveTime > 3 && rigidbody.velocity.sqrMagnitude < 0.0005)
        {
            off = true;
        }
    }

    public void UpdateDriveTime(Vector3 startPos)
    {
        driveTime += Time.fixedDeltaTime;
        newFitness = Vector3.Magnitude(transform.position - startPos) / driveTime;
        if(newFitness > oldFitness)
        {
            oldFitness = newFitness;
        }
        else
        {
            off = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {
            off = true;
        }
        else if (collision.gameObject.tag.Equals("Goal"))
        {
            File.WriteAllBytes("Assets/Training_Result/result.txt", carDNA.neuralNetwork.ToByteArray());
            finish = true;
        }
    }
}

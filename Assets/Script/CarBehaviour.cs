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
    private bool moveThroughPitch;
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
        float[] input = new float[6];
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
                else
                {
                    input[i] = raycastDistance + additional;
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
                else
                {
                    input[i] = raycastDistance;
                }
            }
        }
        Debug.DrawRay(headObject.position, (Quaternion.Euler(0, 90, 0) * transform.right).normalized * (raycastDistance + additional), Color.green);
        if (Physics.Raycast(headObject.position, (Quaternion.Euler(0, 90, 0) * transform.right).normalized, out hitInfo, (raycastDistance + additional), LayerMask.GetMask("Wall")))
        {
            input[5] = hitInfo.distance;
            Debug.DrawRay(headObject.position, (Quaternion.Euler(0, 90, 0) * transform.right).normalized * (raycastDistance + additional), Color.red);
        }
        else
        {
            input[5] = hitInfo.distance;
        }
        return carDNA.neuralNetwork.FeedForward(input);
    }

    public float[] GetAxisFromOutput(float[] output)
    {
        float vertical;
        float horizontal;
        if(output[1] <= 0.25f)
        {
            horizontal = -1;
        }
        else if(output[0] >= 0.65f)
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
        //Debug.Log($"{output[0]} {output[1]} {vertical} {horizontal}");
        return new float[] { horizontal, vertical};
    }

    public void RunCar(float[] axis)
    {
        //transform.rotation = Quaternion.Euler(0, output[0] * Mathf.Rad2Deg, 0);
        rigidbody.angularVelocity = transform.up * axis[0] * 3;
        rigidbody.velocity = (transform.forward * axis[1] * 4);

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
        gameObject.SetActive(true);
        off = false;
        moveThroughPitch = false;
        transform.position = spawnPos;
        transform.rotation = spawnRotation;
        oldFitness = newFitness = 0;
        driveTime = 0;
    }

    public void ShutDownCar()
    {
        if (driveTime > 4 && !moveThroughPitch)
        {
            off = true;
            gameObject.SetActive(false);
        }
    }

    public void UpdateDriveTime(Vector3 startPos)
    {
        driveTime += Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {
            off = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Pitch"))
        {
            moveThroughPitch = true;
        }
        else if (other.gameObject.tag.Equals("Goal"))
        {
            File.WriteAllBytes("Assets/Training_Result/result.txt", carDNA.neuralNetwork.ToByteArray());
            finish = true;
        }
    }
}

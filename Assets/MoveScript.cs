using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    public float moveSpeed = 10;
    public float turnSpeed = 10;
    public float instanceForce = 15;
    private bool turnRight;
    private bool turnLeft;
    private bool addInstanceForce;
    private Rigidbody rigidbody;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down * turnSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Mouse Left");
            //Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray,out RaycastHit hitInfo))
            //{
            //    Vector3 newDir = Vector3.RotateTowards(transform.forward, hitInfo.point - transform.position, Time.deltaTime, 0.0f);
            //    Debug.DrawRay(transform.position, newDir, Color.red);

            //    // Move our position a step closer to the target.
            //    transform.rotation = Quaternion.LookRotation(newDir);
            //}
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, hitInfo.point - transform.position, Time.deltaTime, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.red);

            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    private IEnumerator LerpRotationToMouse()
    {
        yield return null;
        
    }

    private void FixedUpdate()
    {
        rigidbody.AddForce(transform.forward * moveSpeed, ForceMode.Impulse);
    }
}

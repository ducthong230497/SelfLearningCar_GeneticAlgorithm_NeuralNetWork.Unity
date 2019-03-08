﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed;

    private Rigidbody rigidbody;
    private Vector3 startPosition;
    private bool canMove;
    private Transform bestCar;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        canMove = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            canMove = false;
            StartCoroutine(MoveToStartPos());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
        {
            float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed;

            float moveVertical = Input.GetAxis("Vertical") * moveSpeed;

            transform.Translate(moveHorizontal, 0, moveVertical, Space.World);
        }
    }

    private IEnumerator MoveToStartPos()
    {
        float t = 0;
        Vector3 temp = transform.position;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(temp, startPosition, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        transform.position = startPosition;
        canMove = true;
    }
}
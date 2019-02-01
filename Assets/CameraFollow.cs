using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera camera;

    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position - offset, Time.deltaTime);
        //camera.transform.rotation = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, Mathf.Lerp(camera.transform.rotation.z, transform.rotation.z, Time.deltaTime), 1);
    }
}

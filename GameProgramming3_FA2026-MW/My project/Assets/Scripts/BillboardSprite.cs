using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BillboardSprite : MonoBehaviour
{
    private Transform cameraTransform;


    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTransform); //facing the transform of our camera
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); //this allows the object to only turn on it's y axis, no up or down, left or right
    }
}

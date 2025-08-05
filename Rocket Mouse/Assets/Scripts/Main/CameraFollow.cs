using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;

    private float distanceToTarger;

    private void Start()
    {
        distanceToTarger = transform.position.x - targetObject.transform.position.x;
    }

    private void Update()
    {
        float targetObjectX = targetObject.transform.position.x;
        Vector3 newCameraPosition = transform.position;
        newCameraPosition.x = targetObjectX + distanceToTarger;
        transform.position = newCameraPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    private float angularSpeed = 360f;

    void Update()
    {
        transform.rotation *= Quaternion.Euler(0, 0, angularSpeed * Time.deltaTime);       
    }
}

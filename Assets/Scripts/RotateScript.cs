using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, 0, 360 * speed * Time.deltaTime);
    }
}
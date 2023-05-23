using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapData : MonoBehaviour
{
    public Transform respawnPoint;
    
    private void Start()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        foreach(Transform transform in parent.transform) {
            if(transform.CompareTag("Respawn Point")) {
                GameObject child = transform.gameObject;
                respawnPoint = child.transform;
                break;
            }
        }
    }
}

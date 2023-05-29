using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.InitiateRivals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

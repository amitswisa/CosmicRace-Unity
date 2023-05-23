using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        try  {
            GameController.Instance.InitiateRivals();
        } catch(Exception e) {
            Debug.Log(e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

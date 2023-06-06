using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeController : MonoBehaviour
{
    [SerializeField] private List<RuntimeAnimatorController> controllers;
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        var selectedController = PlayerPrefs.GetInt("SelectedCharacter");
        Debug.Log("selectedController= " + selectedController);
        animator.runtimeAnimatorController = controllers[selectedController];
        
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
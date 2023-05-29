// using System.Collections.Generic;
// using UnityEditor.Animations;
// using UnityEngine;
//
// public class RivalCharacter : MonoBehaviour
// {
//     [SerializeField] private List<AnimatorController> controllers;
//     private Animator animator;
//     private int characterID;
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         this.animator = GetComponent<Animator>();
//     }
//
//     public void SetCharacter(int i_CharacterId)
//     {
//         var selectedController = i_CharacterId;
//         animator.runtimeAnimatorController = controllers[selectedController];
//         
//         Destroy(this);
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
// }
using System.Collections.Generic;
using UnityEngine;

public class RivalCharacter : MonoBehaviour
{
    [SerializeField] private List<RuntimeAnimatorController> controllers;
    private Animator animator;
    private int characterID;

    // Start is called before the first frame update
    void Awake()
    {
        this.animator = GetComponent<Animator>();
    }

    public void SetCharacter(int i_CharacterId)
    {
        var selectedController = i_CharacterId;
        animator.runtimeAnimatorController = controllers[selectedController];
        
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
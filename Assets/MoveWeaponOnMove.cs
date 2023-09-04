using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWeaponOnMove : MonoBehaviour
{
    [SerializeField] private Transform RightHand;
    [SerializeField] private Transform LeftHand;
    [SerializeField] private GameObject Weapon;
    
    
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_spriteRenderer.flipX)
        {
            Weapon.transform.position = LeftHand.position;
            // Weapon.GetComponent<SpriteRenderer>().flipX = true;
            Weapon.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {

            Weapon.transform.position = RightHand.position;
            // Weapon.GetComponent<SpriteRenderer>().flipX = false;
            Weapon.GetComponent<SpriteRenderer>().flipY = false;
        }
        
    }
}

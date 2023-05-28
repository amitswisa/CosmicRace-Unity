using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerData>().playerName = User.getUsername();
    }

}

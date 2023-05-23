using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class upgradeStars : MonoBehaviour
{
  
    
    public void OnButtonClick()
    {
        StarScript starScript = gameObject.GetComponent<StarScript>();
        var points = starScript.points;
        int max_points = 100;
        starScript.updatePoints(++points);
        if (points == max_points)
        {
            Button button = gameObject.GetComponent<Button>();
            button.interactable = false;
        }
    }
}

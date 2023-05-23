using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarScript : MonoBehaviour
{
    [SerializeField]
    private GameObject startListObjGameObject;
    [SerializeField]
    private Sprite starImage;
    [SerializeField]
    [Range(0,100)]
    public int points;

    private int _prev_points = -1;

    private Color[] colors = new []{Color.green, Color.yellow, Color.red, Color.cyan, Color.blue,
                                    Color.magenta, Color.white, new Color(123,123,123),
                                    new Color(213,213,213), new Color(42,42,42)};

    
    public List<GameObject> starList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject childGameObject = new GameObject("Start #" + i);
            childGameObject.transform.parent = startListObjGameObject.transform;
            
            childGameObject.AddComponent<Image>();
            var spriteRenderer = childGameObject.GetComponent<Image>();
            spriteRenderer.sprite = starImage;

            var rectTransform = childGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i*30, 0);
            rectTransform.localScale = new Vector3(0.3f, 0.3f, 1);
            starList.Add(childGameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_prev_points == points) return;
        for (var i = 0; i < starList.Count; i++)
        {
            var starObj = starList[i].GetComponent<Image>();
            starObj.color = Color.gray;
        }


        for (int i = 0; i < points; i++)
        {
            var starObj = starList[i%10].GetComponent<Image>();
            starObj.color = colors[(int)i/10];
        }

        _prev_points = points;
    }

    public void updatePoints(int points)
    {
        this.points = points;
    }
}

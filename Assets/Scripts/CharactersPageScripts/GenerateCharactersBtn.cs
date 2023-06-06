using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class GenerateCharactersBtn : MonoBehaviour
{
    private List<Character> charactersList;
    public GameObject characterTemplate;

    // Start is called before the first frame update
    void Start()
    {
        charactersList = User.GetCharactersList();

        // Script that creates a button for each character.
        GameObject newBtn;
        
        foreach(Character character in charactersList)
        {
            int characterID = character.characterId - 1;

            newBtn = Instantiate(characterTemplate, transform);
            newBtn.GetComponentInChildren<TMP_Text>().text = character.c_Name;

            // Load image file as texture
            Texture2D[] texture = Resources.LoadAll<Texture2D>("Prefabs/Match_Fabs/Main Characters/Idle_Images");

            // Create sprite from texture
            Sprite sprite = Sprite.Create(
                texture[characterID], 
                new Rect(0, 0, texture[characterID].width, texture[characterID].height),
                new Vector2(0.5f, 0.5f)
            );
                
            newBtn.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = sprite;

            // Character stats
            Transform statsChildObject = newBtn.transform.GetChild(3);
            statsChildObject.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = character.c_Power/10;
            statsChildObject.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = character.c_Speed/10;
            statsChildObject.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = character.c_Jump/10;
            statsChildObject.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = character.c_Defense/10;
            statsChildObject.GetChild(4).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = character.c_MagicPoints/10;
            statsChildObject.GetChild(5).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = character.c_Xp/10;
        }

        Destroy(characterTemplate);
    }

    private Texture2D LoadImageFile(string path)
    {
        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        return texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

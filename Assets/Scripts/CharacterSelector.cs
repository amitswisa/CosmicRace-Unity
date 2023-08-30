using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public Button enterBtn; 
    private Text enterButtonText;
    public Button prevBtn; 
    public Button nextBtn; 
    public GameObject[] skins;
    public TextMeshProUGUI skin_name;
    public TextMeshProUGUI character_name_statistic;
    public GameObject[] skins_statistic;
    public int selectedCharacter;
    private static readonly string[] characters_names = new string[4] { "Mask Dude", "Ninja Frog", "Pink Man", "Virtual Guy"};

    private void Awake()
    {
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        foreach (GameObject player in skins)
        {
            player.SetActive(false);
        }
        foreach (GameObject skin in skins_statistic)
        {
            skin.SetActive(false);
        }
        skin_name.gameObject.SetActive(false);
        skin_name.SetText(characters_names[selectedCharacter]);
        character_name_statistic.SetText(characters_names[selectedCharacter] );
        skin_name.gameObject.SetActive(true);
        skins_statistic[selectedCharacter].SetActive(true);
        skins[selectedCharacter].SetActive(true);
    }

    void Start() {
        this.enterButtonText = enterBtn.GetComponentInChildren<Text>();
    }

    public void NextCharacter()
    {
        skins[selectedCharacter].SetActive(false);
        skins_statistic[selectedCharacter].SetActive(false);
        skin_name.SetText("");
        selectedCharacter++;
        if (selectedCharacter == skins.Length)
            selectedCharacter = 0;
        skins[selectedCharacter].SetActive(true);
        skins_statistic[selectedCharacter].SetActive(true);
        skin_name.SetText(characters_names[selectedCharacter]);
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
    }
    
    public void PreviousCharacter()
    {
        skins[selectedCharacter].SetActive(false);
        skins_statistic[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
            selectedCharacter += skins.Length;
        skin_name.SetText(characters_names[selectedCharacter]);
        skins[selectedCharacter].SetActive(true);
        skins_statistic[selectedCharacter].SetActive(true);
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
    }

    void modifyButtonsInteractivity(bool state) {           
            enterBtn.interactable = state;
            prevBtn.interactable = state;
            nextBtn.interactable = state;
    }

    public void StartGame()
    {
        modifyButtonsInteractivity(false);
        GameController.Instance.m_player_to_prefab_skin.Add(User.getUsername(), skins[selectedCharacter].GetComponent<Image>().sprite);
        GameController.Instance.Connect();
    }

}
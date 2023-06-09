using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static PlayerCommand;

public class GameController : MonoBehaviour
{
    //[SerializeField] GameObject m_ClientUpdatesPanel;
    private StartGameUpdating updatePanelScript;
    private GameObject m_RivalPrefab;
    private string m_MatchIdentifier;
    public bool m_IsGameRunning = false;
    public bool m_IsMatchStarted {get; private set;}
    public bool m_IsQuit {get; set;}
    public bool m_IsPlayerFinished {get; set;}
    public JObject RivalsData {get; private set;}
    public Dictionary<string, MatchRival> m_Rivals {get; private set;}
    private static GameController instance; // Singleton instance
    public static GameController Instance // Public instance property
    {
        get
        {
            // Create instance if it doesn't exist
            if (instance == null)
            {
                instance = FindObjectOfType<GameController>();

                // If no instance found, create a new game object and attach the GameController script to it
                if (instance == null)
                {
                    GameObject gameControllerObject = new GameObject("GameController");
                    instance = gameControllerObject.AddComponent<GameController>();
                }
            }

            return instance;
        }
    }
    
    private void Awake()
    {
        // Ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set instance to this if it doesn't exist
        if (instance == null)
        {
            instance = this;
        }

        // Ensure that this GameController and all of its children are not destroyed
        // when loading a new scene
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        this.m_MatchIdentifier = "";
        this.RivalsData = null;
        this.m_IsMatchStarted = false;
        this.m_IsGameRunning = false;
        this.m_IsPlayerFinished = false;
        this.m_Rivals = null;

        GameObject[] rivalPrefabs = Resources.LoadAll<GameObject>("Prefabs/Match/Rival");

        if (rivalPrefabs.Length > 0)
        {
            m_RivalPrefab = rivalPrefabs[0];
            Debug.Log("Rival prefab loaded successfully.");
        }
        else
        {
            Debug.LogError("Rival prefab not found in Resources folder.");
        }
    }
    
    public async void Connect()
    {
        await GameClient.Instance.Connect();
    }
    
    public void SendMessageToServer(string message)
    {
        GameClient.Instance.SendMessageToServer(message);
    }
    
    public void PlayerQuit()
    {
        this.m_IsQuit = true;

        PlayerCommand quitCommand
            = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                    , PlayerAction.QUIT, new Location(0,0));
        
        this.SendMessageToServer(quitCommand.ToJson()+"\n");
        
        this.Disconnect("You have quit the match, no progress made during this match will be saved.");
    }

    public void InitiateRivals()
    {
        this.m_MatchIdentifier = (string) RivalsData["MatchIdentifier"];

        JObject playerList = (JObject)RivalsData["Players"];

        foreach (var player in playerList)
        {
            string playerName = player.Key.Trim('"');
            
            if(!playerName.Equals(User.getUsername()))
            {

                if(this.m_Rivals == null)
                    this.m_Rivals = new Dictionary<string, MatchRival>();

                GameObject newRival = Instantiate(m_RivalPrefab);
                
                // Convert CharacterData to JObject
                JObject characterData = JObject.Parse((string)player.Value["CharacterData"]);
                int characterID = ((int)characterData["characterID"]) -1;

                MatchRival matchRival = new MatchRival(playerName, newRival, characterID);
                this.m_Rivals.Add(playerName, matchRival);
            }
        }
    }

    public void UpdateServerAfterCoinCollection()
    {
        // TODO - Fix Location to original location of player.
        PlayerCommand coinCollectCommand
            = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                    , PlayerAction.COIN_COLLECT, new Location(0,0));
        this.SendMessageToServer(coinCollectCommand.ToJson()+"\n");
    }

    public void SetMatchStarted()
    {
        this.m_IsMatchStarted = true;
    }

    public void UpdateRivalesData(JObject i_RivalsData)
    {
        this.RivalsData = i_RivalsData;
    }

    public void Disconnect(String i_Message = "")
    {
        GameClient.Instance.Disconnect();

        UnityMainThreadDispatcher.Instance.EnqueueAsync(() => {
            SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);

            if(this.m_IsQuit) 
            {
                OKDialogManager.Instance.ShowDialog("Match Quit", i_Message);
            }
            else
                OKDialogManager.Instance.ShowDialog("Match Terminated", i_Message);
        });

        resetGameControllerSettings();
    }

    public void EndMatch()
    {

    }

    private void resetGameControllerSettings()
    {
        this.m_MatchIdentifier = "";
        this.RivalsData = null;
        this.m_IsMatchStarted = false;
        this.m_IsGameRunning = false;
        this.m_Rivals = null;
        this.m_IsQuit = false;
    }

    private void OnApplicationQuit()
    {
        if (GameClient.Instance != null)
        {
            GameClient.Instance.Disconnect();
        }
    }

    internal void MatchCompleted(string i_MatchCompleteMessage)
    {
        this.m_IsPlayerFinished = true;
        GameClient.Instance.SendMessageToServer(i_MatchCompleteMessage);
    }
}

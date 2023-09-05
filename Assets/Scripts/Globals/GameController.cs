using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DefaultNamespace;
using static PlayerCommand;

public class GameController : MonoBehaviour
{
    public bool m_IsFriendMode {get; private set;}
    private StartGameUpdating updatePanelScript;
    private GameObject m_RivalPrefab;
    private string m_MatchIdentifier;
    public bool m_IsGameRunning = false;
    public bool m_IsMatchStarted {get; private set;}
    public bool m_IsQuit {get; set;}
    public bool m_IsPlayerFinished {get; set;}
    public JObject RivalsData {get; private set;}
    public Dictionary<string, MatchRival> m_Rivals {get; private set;}
    public GameObject[] rivalPrefabs;
    public GameObject projectilePrefab;
    public Dictionary<string, Projectile> m_projectiles {get; private set;}
    public Dictionary<string, int> m_finish_players {get; set;}
    public Dictionary<string, int> m_player_to_prefab_skin_id {get; set;}

    // public Dictionary<string, string> m_endMatch; // timestamp to character that end the match
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

    public static void ClearInstance()
    {
        Instance.resetGameControllerSettings();
        Instance.m_MatchIdentifier = "";
        Instance.RivalsData = null;
        Instance.m_IsMatchStarted = false;
        Instance.m_IsGameRunning = false;
        Instance.m_IsPlayerFinished = false;
        Instance.m_IsFriendMode = false;
        Instance.m_Rivals = null;
        Instance.m_RivalPrefab = null;
        Instance.m_projectiles = null;

        Instance.m_finish_players = new Dictionary<string, int>();
        Instance.m_player_to_prefab_skin_id = new Dictionary<string, int>();
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
        this.m_IsFriendMode = false;
        this.m_Rivals = null;
        this.m_projectiles = null;

        m_finish_players = new Dictionary<string, int>();
        m_player_to_prefab_skin_id = new Dictionary<string, int>();

       rivalPrefabs = Resources.LoadAll<GameObject>("Prefabs/Match/Rival");
       projectilePrefab = Resources.Load<GameObject>("2D Pixel Spaceship - Two Small Ships/Prefabs/fireballs/fireball-red-tail-med.prefab");
    }
    
    public async void Connect()
    {
        this.m_IsFriendMode = false;
        await GameClient.Instance.Connect(m_IsFriendMode);
    }
    
    public async void ConnectInFriendMode()
    {
        this.m_IsFriendMode = true;
        await GameClient.Instance.Connect(m_IsFriendMode);
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
        
        if(this.m_projectiles == null)
            this.m_projectiles = new Dictionary<string, Projectile>();

        foreach (var player in playerList)
        {
            string playerName = player.Key.Trim('"');
            
            if(!playerName.Equals(User.getUsername()))
            {
                if (this.m_Rivals == null)
                {
                    this.m_Rivals = new Dictionary<string, MatchRival>();
                    this.m_finish_players = new Dictionary<string, int>();
                }

                // Convert CharacterData to JObject
                JObject characterData = JObject.Parse((string)player.Value["CharacterData"]);
                int characterID = ((int)characterData["characterID"]) - 1;

                m_RivalPrefab = rivalPrefabs[0];
                
                GameObject newRival = Instantiate(m_RivalPrefab);
                m_player_to_prefab_skin_id.Add(playerName, characterID);

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
        this.Disconnect("Match has ended!");
    }

    private void resetGameControllerSettings()
    {
        this.m_MatchIdentifier = "";
        this.RivalsData = null;
        this.m_IsMatchStarted = false;
        this.m_IsGameRunning = false;
        this.m_IsFriendMode = false;
        this.m_Rivals = null;
        this.m_projectiles = null;
        this.m_IsQuit = false;
    }

    private void OnApplicationQuit()
    {
        if (GameClient.Instance != null)
        {
            GameClient.Instance.Disconnect();
        }
    }

    public bool IsConnected()
    {
        return GameClient.Instance.IsConnectionAlive();
    }

    public void SetMatchIdentifier(string i_MatchIdentifier)
    {
        this.m_MatchIdentifier = i_MatchIdentifier;
    }

    internal void MatchCompleted(string i_MatchCompleteMessage)
    {
        this.m_IsPlayerFinished = true;
        GameClient.Instance.SendMessageToServer(i_MatchCompleteMessage);
    }

    internal string GetMatchIdentifier()
    {
        return this.m_MatchIdentifier;
    }

    public void NewBullet(string id, string username, Vector3 shotPointPosition, bool isToRight)
    {
        if (username == User.getUsername())
        {
            var bulletInfo = new BulletInfo
            {
                id = id,
                owner = username,
                position = shotPointPosition,
                isToRight = isToRight,
            };
            var playerCommand = new PlayerCommand(MessageType.COMMAND, username,
                PlayerAction.BULLET_CREATED, null, i_bulletInfo: bulletInfo);
            GameClient.Instance.SendMessageToServer(JsonUtility.ToJson(playerCommand));
        }
        else
        {
            var rotation = Quaternion.Euler(0f, 0f, isToRight ? 0 : 180);
            var p = Instantiate(projectilePrefab, shotPointPosition, rotation).GetComponent<Projectile>();
            p.isRight = isToRight;
            p.id = id;
            p.owner = username;
            m_projectiles.Add(p.id, p);
        }
    }
    
    public void UpdateBullet(string id, Vector3 bulletPosition)
    {
        if (m_projectiles[id].owner == User.getUsername())
        {
            var bulletInfo = new BulletInfo()
            {
                id = id,
                position = bulletPosition,
            };
            var playerCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername(),
                PlayerAction.BULLET_UPDATED, null, i_bulletInfo: bulletInfo);
            GameClient.Instance.SendMessageToServer(JsonUtility.ToJson(playerCommand) + "\n");
        }
        else
        {
            m_projectiles[id].transform.position = bulletPosition;
        }
    }
    
    public void CollideBullet(string id, String rival_name)
    {
        var bulletInfo = new BulletInfo()
        {
            id = id,
            rivalName = rival_name,
        };
        var playerCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername(),
            PlayerAction.BULLET_COLLIED, null, i_bulletInfo: bulletInfo);
        GameClient.Instance.SendMessageToServer(JsonUtility.ToJson(playerCommand)+"\n");
    }
    
    public void DestroyBullet(string id)
    {
        if (m_projectiles[id].owner == User.getUsername())
        {
            var bulletInfo = new BulletInfo()
            {
                id = id,
            };
            var playerCommand = new PlayerCommand(MessageType.COMMAND, User.getUsername(),
                PlayerAction.BULLET_DESTROY, null, i_bulletInfo: bulletInfo);
            GameClient.Instance.SendMessageToServer(JsonUtility.ToJson(playerCommand) + "\n");
        }
        else
        {
            try
            {
                Destroy(m_projectiles[id]);
            }
            catch (Exception e)
            {
                // ignored
            }

            m_projectiles.Remove(id);
        }
    }

    public void AddUpdateViewListener(Text o_UpdateOject)
    {
        GameClient.Instance.AddUpdateViewListener(o_UpdateOject);
    }
}

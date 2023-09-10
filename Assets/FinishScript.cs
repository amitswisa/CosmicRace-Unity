using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    [SerializeField]
    public List<FinishCharacterScript> go_characters;
    [SerializeField]
    public List<Sprite> go_sprite;

    private Dictionary<string, int> instanceMFinishPlayers;
    private Dictionary<string, int> player_to_prefab;

    private void Start()
    {
        instanceMFinishPlayers = GameController.Instance.m_finish_players;
        player_to_prefab = GameController.Instance.m_player_to_prefab_skin_id;

        List<string> position_players = instanceMFinishPlayers // max 4 cells
            .OrderBy(pair => pair.Value)  // Order the dictionary by value (index)
            .Select(pair => pair.Key)     // Select the keys (names)
            .ToList();
            
        for (var i = 0; i < position_players.Count; i++)
        {
            go_characters[i].setSkin(go_sprite[player_to_prefab[position_players[i]]]);
            go_characters[i].setUsername(position_players[i]);
            go_characters[i].gameObject.SetActive(true);
        }
    }

    public void Update()
    {
        
    }

    public void OnDestroy()
    {
        GameController.Instance.m_player_to_prefab_skin_id.Clear();
    }
}

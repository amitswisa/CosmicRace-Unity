using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    [SerializeField]
    public List<FinishCharacterScript> go_characters;
    // Start is called before the first frame update
    void Start()
    {
        var instanceMFinishPlayers = GameController.Instance.m_finish_players;
        Debug.Log(instanceMFinishPlayers);
        Debug.Log(instanceMFinishPlayers.Count);
        List<string> position_players = instanceMFinishPlayers // max 4 cells
            .OrderBy(pair => pair.Value)  // Order the dictionary by value (index)
            .Select(pair => pair.Key)     // Select the keys (names)
            .ToList();
        Debug.Log(position_players);
        for (var i = 0; i < position_players.Count; i++)
        {
            Debug.Log(position_players[i]);
            go_characters[i].setSkin(GameController.Instance.m_player_to_prefab_skin[position_players[i]]);
            go_characters[i].setUsername(position_players[i]);
            go_characters[i].gameObject.SetActive(true);
        }
    }
}

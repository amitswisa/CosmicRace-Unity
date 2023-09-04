using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerCommand;

public class FinishPoint : MonoBehaviour
{
    private AudioSource finishSound;
    private bool levelCompleted = false;
    void Start()
    {
        finishSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !levelCompleted)
        {
            finishSound.Play();
            levelCompleted = true;
            
            var player = col.gameObject;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            playerMovement.setDirX(0);
            playerMovement.UpdateAnimationState(new PlayerCommand(MessageType.COMMAND, null, PlayerAction.COMPLETE_LEVEL,null));
            
            PlayerCommand completeLevelCommand
                        = new PlayerCommand(MessageType.COMMAND, User.getUsername()
                                , PlayerAction.COMPLETE_LEVEL, new Location(col.gameObject.transform.position.x,
                                         col.gameObject.transform.position.y));

            GameController.Instance.MatchCompleted(completeLevelCommand.ToJson()+"\n");
        }
    }
}
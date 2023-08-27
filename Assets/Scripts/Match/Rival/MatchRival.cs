using System;
using UnityEngine;

public sealed class MatchRival
{
    public string m_Username { get; private set; }
    public int mCharacterId { get; private set; }
    public GameObject m_rivalInstance { get; set; }
    private RivalMovement m_RivalMovement;
    private Transform m_Position;
    private RivalCharacter m_RivalCharacterDefinitions;
    public MatchRival(string i_Username, GameObject i_Object, int i_CharacterId)
    {
        this.m_Username = i_Username;
        this.m_rivalInstance = i_Object;
        this.mCharacterId = i_CharacterId;
        
        this.m_Position = this.m_rivalInstance.GetComponent<Transform>();
        this.m_RivalMovement = this.m_rivalInstance.GetComponent<RivalMovement>();
        this.m_RivalCharacterDefinitions = this.m_rivalInstance.GetComponent<RivalCharacter>();
        this.m_rivalInstance.GetComponent<PlayerData>().playerName = m_Username;
        this.m_rivalInstance.GetComponent<PlayerData>()._selected_charecter = i_CharacterId;
        this.m_rivalInstance.tag = "Player";
        
        this.m_RivalCharacterDefinitions.SetCharacter(i_CharacterId);
    }

    public void PerformJump(PlayerCommand command)
    {
        m_RivalMovement.PerformJump();
        this.PositionCorrection(command);
    }

    public void MoveRight(PlayerCommand command)
    {
        m_RivalMovement.MoveRight();
        this.PositionCorrection(command);
    }

    public void MoveLeft(PlayerCommand command)
    {
        m_RivalMovement.MoveLeft();
        this.PositionCorrection(command);
    }

    public void StopMoving(PlayerCommand command)
    {
        m_RivalMovement.StopMoving();
        this.PositionCorrection(command);
    }

    public void ActivateDeath(Location i_Location)
    {
        m_RivalMovement.TriggerDeath(i_Location);
    }

    public void Quit(PlayerCommand command) 
    {
        // Instance still exists
        if(m_rivalInstance != null)
        {
            GameObject.Destroy(m_rivalInstance);
            Debug.Log("Player " + this.m_Username + " has quit the match.");
        }
    }

    public void PositionCorrection(PlayerCommand playerCommand)
    {
        if(GameController.Instance.m_IsFriendMode)
        {
            return;
        }

        float getInstancePositionX = this.m_Position.position.x;
        float getInstancePositionY = this.m_Position.position.y;

        Location rivalLocation = playerCommand.m_Location;

        if (Mathf.Abs(rivalLocation.x - getInstancePositionX) >= 2f)
        {
            Vector3 newPosition = m_rivalInstance.gameObject.transform.position;
            newPosition.x = rivalLocation.x;
            newPosition.y = rivalLocation.y;
            m_rivalInstance.gameObject.transform.position = newPosition;
            Debug.Log("Player " + this.m_Username + " location was updated due to sync problems.");
        }
    }

    public void Attacked(PlayerCommand command)
    {
        m_RivalMovement.Attacked(command.m_Location, 1.5f);
    }
}
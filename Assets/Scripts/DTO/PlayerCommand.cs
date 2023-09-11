using System;
using Newtonsoft.Json;
using UnityEngine;

public sealed class PlayerCommand
{
    public static class PlayerAction
    {
        public const string JUMP = "JUMP";
        public const string IDLE = "IDLE";
        public const string RUN_LEFT = "RUN_LEFT";
        public const string RUN_RIGHT = "RUN_RIGHT";
        public const string DEATH = "DEATH";
        public const string ATTACK = "ATTACK";
        public const string UPDATE_LOCATION = "UPDATE_LOCATION";
        public const string COIN_COLLECT = "COIN_COLLECT";
        public const string RIVAL_QUIT = "RIVAL_QUIT";
        public const string COMPLETE_LEVEL = "COMPLETE_LEVEL";
        public const string QUIT = "QUIT";
        public const string ELIMINATION = "ELIMINATION";
        public const string REVIVE = "REVIVE";

    }
    
    public class BulletInfo
    {
        public string id;
        public string owner;
        public Vector3 position;
        public bool isToRight;
        public string rivalName;
    }

    public class AttackInfo
    {
        public String m_AttackerName;
        public String m_Victim;
        public String m_AttackID;

        public override string ToString()
        {
            return "m_AttackerName: " + m_AttackerName + " m_Victim: " + m_Victim + " m_AttackID: " + m_AttackID;
        }
    }

    public string m_MessageType {get; set;}
    public string m_Username {get; set;}
    public string m_Action {get; set;}
    public Location m_Location {get; set;}
    public BulletInfo m_bulletInfo { get; set; }
    public AttackInfo m_AttackInfo { get; set; }

    public PlayerCommand(string i_MessageType, string i_Username, string i_Action,
        Location i_Location, AttackInfo i_AttackInfo = null, BulletInfo  i_bulletInfo = null) 
    {
        this.m_MessageType = i_MessageType;
        this.m_Username = i_Username;
        this.m_Action = i_Action;
        this.m_Location = i_Location;
        this.m_AttackInfo = i_AttackInfo;
        this.m_bulletInfo = i_bulletInfo;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public override int GetHashCode()
    {
        return this.m_Action.GetHashCode();
    }

    public bool isEqual(string currentCommand)
    {
        return this.m_Action.Equals(currentCommand);
    }
}
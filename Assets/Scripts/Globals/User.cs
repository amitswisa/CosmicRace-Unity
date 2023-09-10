using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public static class User {
    private static string m_Username = "";
    private static int m_Userid = -1;
    private static int m_UserLevel = 0;
    private static int m_UserCoins = 0;
    private static int m_XP = 0;
    private static string jwtoken = "";
    private static List<Character> m_Characters;

    public static bool setUser(string username_g, JObject data)
    {

        if(!(bool)data["success"])
            return false;

        // Login verified
        m_Username = username_g;
        jwtoken = (string) data["token"];
        m_UserLevel = (int)data["level"];
        m_Userid = (int)data["userid"];
        m_XP = (int)data["xp"];
        m_UserCoins = (int) data["coins"];
        m_Characters = new List<Character>();

        // Get characters array from json.
        JArray characterArray = (JArray) data["characters"];
        foreach(JObject character in characterArray)
        {
            m_Characters.Add(new Character(character));
        }

        return true;
    }

    public static void Logout() {
        m_Username = "";
        jwtoken = "";
        m_UserLevel = 0;
        m_UserCoins = 0;
        m_XP = 0;
        m_Characters.Clear();
    }

    public static string getUsername() {
        return m_Username;
    }

    public static int getCoinsAmount() {
        return m_UserCoins;
    }

    public static string getToken() {
        return jwtoken;
    }

    public static void updateCoinsAmount(int amount) {
        m_UserCoins += amount;
    }

    public static bool isTokenExist() {
        return (jwtoken != "" && jwtoken != null);
    }

    public static int getUserId() {
        return m_Userid;
    }

    public static float getProgress()
    {
        return m_XP / 100f;
    }

    public static int getLevel()
    {
        return m_UserLevel;
    }

    public static List<Character> GetCharactersList()
    {
        return m_Characters;
    }
}
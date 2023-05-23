
using Newtonsoft.Json.Linq;

public class Character
{
    public int characterId { get; }
    public string c_Name { get; }
    private int c_Level { get; }
    public int c_Xp { get; }
    public int c_MagicPoints { get; }
    public float c_Speed { get; }
    public float c_Jump { get; }
    public float c_Power { get; }
    public float c_Defense { get; }
    private int total_Wins { get; }
    private int total_Loses { get; }

    public Character(JObject characterData) {
        this.characterId = (int) characterData["characterID"];
        this.c_Name = (string) characterData["characterName"];
        this.c_Level = (int) characterData["level"];
        this.c_Xp = (int) characterData["xp"];
        this.c_MagicPoints = (int) characterData["magicPoints"];
        this.c_Speed = (int) characterData["speed"];
        this.c_Defense = (int) characterData["defense"];
        this.c_Jump = (int) characterData["jump"];
        this.total_Wins = (int) characterData["wins"];
        this.total_Loses = (int) characterData["loses"];
    }

    public static float SpeedLevelToSpeedValue(int speedLevel)
    {
        return 0f;
    }
}


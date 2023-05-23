using System;

public class PlayerQuitException : Exception
{
    public PlayerQuitException(string i_Message) : base(i_Message)
    {
        
    }
}
using System.Collections.Generic;

/// <summary>
/// A typical PlayerData holds the values for a player in the game.
/// <br/>- name
/// <br/>- isAI
/// <br/>- score
/// </summary>
public class PlayerData
{
    string playerName;
    bool isAI;
    int score;

    /// <summary>
    /// The PlayerData constructor, creates a PlayerData object with a name and isAI value. The score is 0 by default.
    /// </summary>
    /// <param name="playerName">The name of this player</param>
    /// <param name="isAI">whether or not this player is controlled by the computer</param>
    public PlayerData(string playerName, bool isAI)
    {
        this.playerName = playerName;
        this.isAI = isAI;
        score = 0;
    }

    /// <summary>
    /// Add 1 to this score
    /// </summary>
    public void AddScore()
    {
        score++;
    }

    /// <summary>
    /// Returns the score of this player
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Returns the name of this player
    /// </summary>
    /// <returns></returns>
    public string GetName()
    {
        return playerName;
    }

    /// <summary>
    /// Returns true if this player is an AI, false if not
    /// </summary>
    /// <returns></returns>
    public bool IsAI()
    {
        return isAI;
    }
}

/// <summary>
/// A typical GameData holds the values of the players in the game and their score
/// </summary>
public static class GameData
{
    /// <summary>
    /// The list of players in the game
    /// </summary>
    static List<PlayerData> playerData = new List<PlayerData>();

    /// <summary>
    /// The number of turns that this game lasts for
    /// </summary>
    static int duration;

    /// <summary>
    /// Sets the duration of the next game
    /// </summary>
    /// <param name="duration">how many turns does this game last for</param>
    public static void SetDuration(int duration)
    {
        GameData.duration = duration;
    }

    /// <summary>
    /// Loops through the players and returns the player or players with the highest score
    /// </summary>
    /// <returns></returns>
    public static string GetVictors()
    {
        List<PlayerData> players = new List<PlayerData>();

        //Add the first player to the list
        players.Add(playerData[0]);

        for (int i = 1; i < playerData.Count; i++)
        {
            //is the current player better than those in the list?
            if (playerData[i].GetScore() > players[0].GetScore())
            {
                players.Clear();
                players.Add(playerData[i]);
            } 
            //is the current player equal to those in the list?
            else if (playerData[i].GetScore() == players[0].GetScore())
            {
                players.Add(playerData[i]);
            }
        }

        //initialize the return string
        string playerScores = "The "+(players.Count==1?"winner of the game is :":"winners of the game are :");

        foreach (PlayerData player in players) playerScores += "\n"+ player.GetName() + " : " + player.GetScore();

        return playerScores;
    }

    /// <summary>
    /// Adds a player to the game
    /// </summary>
    /// <param name="name">The name of the player</param>
    /// <param name="isAI">If the player is controlled by the computer or not</param>
    public static void AddPlayer(string name, bool isAI)
    {
        //verify that no other player has this name
        foreach(PlayerData player in playerData)
        {
            if (player.GetName().Equals(name))
            {
                //this players name is used already

                //option 1: The Nuclear Option - Keep for testing purposes only
                throw new System.Exception("There is already a player with that name.");

                //option 2: The Friendly Option - Use in development
                //AddPlayer(name + 0, isAI);
                //return;
            }
        }

        //The name does not already exist, add the player
        playerData.Add(new PlayerData(name, isAI));
    }

    /// <summary>
    /// Clears the list of PlayerData to prevent having players from a previous game added to the new game
    /// </summary>
    public static void ClearPlayers()
    {
        playerData.Clear();
    }

    /// <summary>
    /// Get the PlayerData of the next player or returns null if the next player is not found or does not exist
    /// </summary>
    /// <param name="name">The name of the player to look for</param>
    /// <returns></returns>
    public static PlayerData GetNextPlayer(string name)
    {
        bool foundPlayer = false;

        foreach(PlayerData player in playerData)
        {
            if (foundPlayer) return player;

            if (name.Equals(player.GetName())) foundPlayer = true;
        }

        return null;
    }

    /// <summary>
    /// Return the first player in the PlayerData list
    /// 
    /// Used at the start of a new match
    /// </summary>
    /// <returns></returns>
    public static PlayerData GetFirstPlayer()
    {
        return playerData[0];
    }

    /// <summary>
    /// Checks if the game has another game
    /// </summary>
    /// <returns>true if the current duration is greater than 0, then removes 1 from duration</returns>
    public static bool HasNextGame()
    {
        return duration-- >0;
    }

    /// <summary>
    /// Returns the current duration
    /// </summary>
    /// <returns></returns>
    public static int GetDuration()
    {
        return duration;
    }

    /// <summary>
    /// Add 1 score to the player with a given name
    /// </summary>
    /// <param name="name">The name of the player to add 1 score to</param>
    public static void AddScore(string name)
    {
        //Loop through every player in player data and if the name of the player is equal to the given name, then add 1 score to that player
        foreach (PlayerData player in playerData) if (player.GetName().Equals(name)) player.AddScore();
    }

    /// <summary>
    /// collect all of the players scores throughout the entire game
    /// into a formatted string ready to print to the console of display in a text box.
    /// 
    /// Shows the scores in order of when the player was added to the game (not from highest to lowest score)
    /// </summary>
    /// <returns></returns>
    public static string GetPlayerScores()
    {
        //initialize the return string
        string playerScores = "";

        bool hasFirst = false;

        foreach(PlayerData player in playerData)
        {
            if (hasFirst) playerScores += "\n";

            hasFirst = true;

            playerScores += player.GetName() + " : " + player.GetScore();
        }

        return playerScores;
    }
}

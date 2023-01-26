using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Rock, Paper, Scissors
/// </summary>
public enum GameChoice
{
    Rock,
    Paper,
    Scissors
}

/// <summary>
/// Local, Online
/// </summary>
enum GameType
{
    Local,
    Online
}

/// <summary>
/// A typical PlayerChoice tracks the choice made by a player for the current match
/// </summary>
class PlayerChoice
{
    string playerName;
    GameChoice choice;
    
    /// <summary>
    /// Initializes this object with the given values
    /// </summary>
    /// <param name="playerName">The name of this player</param>
    /// <param name="choice">The choice that the player made this match</param>
    public PlayerChoice(string playerName, GameChoice choice)
    {
        this.playerName = playerName;
        this.choice = choice;
    }

    /// <summary>
    /// Returns the name of this player
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName()
    {
        return playerName;
    }

    /// <summary>
    /// Returns the choice that this player made during this match
    /// </summary>
    /// <returns></returns>
    public GameChoice GetPlayerChoice()
    {
        return choice;
    }

    /// <summary>
    /// Compares two GameChoice elements
    /// </summary>
    /// <param name="choice0">The first choice</param>
    /// <param name="choice1">The second choice</param>
    /// <returns>1 if choice0 is better than choice1, 0 if tied, -1 if choice1 is better than choice0</returns>
    public static int CompareChoice(GameChoice choice0, GameChoice choice1)
    {
        //Tie game
        if (choice0 == choice1) return 0;

        switch (choice0)
        {
            case GameChoice.Rock:
                return choice1==GameChoice.Paper ? -1 : 1;
            case GameChoice.Paper:
                return choice1 == GameChoice.Scissors ? -1 : 1;
            case GameChoice.Scissors:
                return choice1 == GameChoice.Rock ? -1 : 1;
        }

        //Unless GameChoice is changed, this should not be reachable
        throw new System.Exception("Compare choices were not equal, and choice0 was not Rock, Paper or Scissors");
    }
}

/// <summary>
/// A typical MatchScore holds a name, score and choice for a single match
/// 
/// then, displays the score
/// </summary>
class MatchScore
{
    /// <summary>
    /// The name of this player
    /// </summary>
    string name;
    /// <summary>
    /// The score of this player
    /// </summary>
    int score;
    /// <summary>
    /// The choice that this player made during this match
    /// </summary>
    GameChoice choice;

    /// <summary>
    /// Creates a default MatchScore with a given name and choice and a default score of 1.
    /// </summary>
    /// <param name="name">The name of this player</param>
    /// <param name="choice">The choice that this player made during this match</param>
    public MatchScore(string name, GameChoice choice)
    {
        this.name = name;
        this.choice = choice;
        score = 1;
    }

    /// <summary>
    /// Returns true if this player is equal to the given name
    /// </summary>
    /// <param name="name">The name to compare this player to</param>
    /// <returns></returns>
    public bool IsPlayer(string name)
    {
        return this.name.Equals(name);
    }

    /// <summary>
    /// Adds 1 to this score
    /// </summary>
    public void AddScore()
    {
        score++;
    }

    /// <summary>
    /// Returns the data stored in this object as a formatted string.
    /// </summary>
    /// <returns></returns>
    public string GetResult()
    {
        return name + " bested " + score + (score == 1 ? " player" : " players") + " with the choice of " + choice.ToString() + ".";
    }
}

/// <summary>
/// A typical GameController is used to control the game of Rock Paper Scissors
/// <br/>
/// <br/>A game controller can be set to 2 play modes:
/// <br/>- Local Game
/// <br/>- Online Game (Implemented after Firebase is added to this project)
/// <br/>
/// <br/>A GameController requires a <strong>WinCountText</strong>, <strong>LossCountText</strong> and <strong>LastGameResultText</strong>
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// A singleton of the GameController class
    /// </summary>
    public static GameController instance;

    [Header("Display Area")]
    /// <summary>
    /// The area where we display the score of the entire game
    /// </summary>
    [SerializeField] TMP_Text playerScoreArea;
    /// <summary>
    /// The area where we display the score of the last match
    /// </summary>
    [SerializeField] TMP_Text lastMatchResultText;

    [Header("The type of game that is played")]
    /// <summary>
    /// The type of game that is played
    /// </summary>
    [SerializeField] GameType gameType;

    [Header("In Between Turns Screen")]
    /// <summary>
    /// The screen that is displayed when the phone is passed
    /// </summary>
    [SerializeField] GameObject passThePhoneScreen;
    /// <summary>
    /// The name of the player that the phone should be passed to
    /// </summary>
    [SerializeField] TMP_Text passThePhoneScreenPlayerNameText;

    [Header("On Match Finish")]
    /// <summary>
    /// The object displayed when transitioning to the next match
    /// </summary>
    [SerializeField] GameObject resetBackground;

    /// <summary>
    /// The choices that players make throughout a single match
    /// 
    /// Cleared at the end of each match
    /// </summary>
    List<PlayerChoice> playerChoices = new List<PlayerChoice>();
    /// <summary>
    /// The list of scores throughout the match
    /// </summary>
    List<MatchScore> matchScores = new List<MatchScore>();

    [Header("Scene names")]
    [SerializeField] string createLocalGameSceneName;
    [SerializeField] string createOnlineGameSceneName;

    [Header("On Game Finished")]
    /// <summary>
    /// The screen shown at the end of the game
    /// </summary>
    [SerializeField] GameObject gameOverScreen;
    /// <summary>
    /// The text that shows the winners of the game
    /// </summary>
    [SerializeField] TMP_Text gameOverText;

    /// <summary>
    /// Called every time the GameObject that this script is attached to becomes active in the Unity Hierarchy
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartNextMatch();
    }

    /// <summary>
    /// On back button press,
    /// 
    /// if gameType is local, return to the create local game scene
    /// if gameType is online, return to the create online game scene
    /// </summary>
    public void OnBackPress()
    {
        SceneManager.LoadScene(gameType == GameType.Local ? createLocalGameSceneName : createOnlineGameSceneName);
    }

    /// <summary>
    /// Controls what happens next in the game and who can take their turn when
    /// </summary>
    /// <param name="lastPlayerName">The name of the player who had their turn last</param>
    void ControlGame(string lastPlayerName)
    {
        //Check the game type
        if(gameType == GameType.Local)
        {
            //Get the next player - is null if no next player
            PlayerData nextPlayer = GameData.GetNextPlayer(lastPlayerName);

            //see if there is a next player
            if (nextPlayer == null)
            {//Tally the core, display it and end this match
                //see if we have enough players
                if (playerChoices.Count < 2)
                {
                    throw new System.Exception("There are not enough players in this game.");
                }
                if(playerChoices.Count > 2)
                {
                    //You get 1 point per player that your choice beat
                    foreach(PlayerChoice playerChoice0 in playerChoices)
                    {
                        foreach (PlayerChoice playerChoice1 in playerChoices)
                        {
                            if(PlayerChoice.CompareChoice(playerChoice0.GetPlayerChoice(), playerChoice1.GetPlayerChoice()) == 1)
                            {
                                AddPlayerScore(playerChoice0.GetPlayerName(), playerChoice0.GetPlayerChoice());
                            }
                        }
                    }

                    #region then display the results for this match
                    //initialize the result string for this match
                    string gameResult = "";

                    //Tie game
                    if(matchScores.Count < 1)
                    {
                        gameResult = "This match was a perfect tie.\nAll players chose " + playerChoices[0].GetPlayerChoice().ToString() + ".";
                    }
                    else
                    {
                        bool hasFirst = false;

                        foreach(MatchScore matchScore in matchScores)
                        {
                            if (hasFirst) gameResult += "\n";

                            hasFirst = true;

                            gameResult += matchScore.GetResult();
                        }
                    }

                    lastMatchResultText.text = gameResult;
                    #endregion
                }
                else //we only have 2 players this game
                {
                    //use a switch to find the values
                    switch(PlayerChoice.CompareChoice(playerChoices[0].GetPlayerChoice(), playerChoices[1].GetPlayerChoice()))
                    {
                        case 0://Tie
                            lastMatchResultText.text = "Both players chose " + playerChoices[0].GetPlayerChoice().ToString() + ".";
                            break;
                        case 1://playerChoices[0] is the winner
                            AddPlayerScore(playerChoices[0].GetPlayerName(), playerChoices[0].GetPlayerChoice());

                            lastMatchResultText.text = playerChoices[0].GetPlayerName() + " is the winner." +
                                "\n" + playerChoices[0].GetPlayerChoice().ToString() + " beats " + playerChoices[1].GetPlayerChoice().ToString() + ".";
                            break;
                        case -1://playerChoices[1] is the winner
                            AddPlayerScore(playerChoices[1].GetPlayerName(), playerChoices[1].GetPlayerChoice());

                            lastMatchResultText.text = playerChoices[1].GetPlayerName() + " is the winner." +
                                "\n" + playerChoices[1].GetPlayerChoice().ToString() + " beats " + playerChoices[0].GetPlayerChoice().ToString() + ".";
                            break;
                    }
                }

                //display the new text now that all of the scores have been tallied
                playerScoreArea.text = GameData.GetPlayerScores();

                resetBackground.SetActive(true);

                void AddPlayerScore(string name, GameChoice choice)
                {
                    //Add score to the player for the whole game
                    GameData.AddScore(name);

                    //increase of create a MatchScore for the player
                    foreach(MatchScore matchScore in matchScores)
                    {
                        //does this name equal the current matchScore
                        if (matchScore.IsPlayer(name))
                        {
                            matchScore.AddScore();
                            //we found the player, exit this function
                            return;
                        }
                    }

                    //we did not find the score, we need to create a new one
                    matchScores.Add(new MatchScore(name, choice));
                }
            }
            else
            {
                //run the turn for the next player
                RunPlayerTurn(nextPlayer);
            }
        }
        else //online gae
        {
            throw new System.Exception("Implement Online Play after Firebase is added.");
        }
    }

    /// <summary>
    /// Runs the turn for the next player
    /// </summary>
    /// <param name="nextPlayer">The player data of the player who gets to move next</param>
    void RunPlayerTurn(PlayerData nextPlayer)
    {
        if (nextPlayer.IsAI())
        {
            GetAIInput(nextPlayer.GetName());
        }
        else
        {
            passThePhoneScreen.SetActive(true);
            passThePhoneScreenPlayerNameText.text = "Pass the phone to " + nextPlayer.GetName() + " then tap anywhere to continue.";

            //setup the PlayerController to wait for input from the nextPlayer
            PlayerController.CheckForPlayerInput(nextPlayer.GetName());
        }
    }

    /// <summary>
    /// Gets the <strong>basic</strong> input from the AI
    /// </summary>
    /// <param name="name">The name of the AI taking a turn</param>
    void GetAIInput(string name)
    {
        //Submits a nearly random choice
        SubmitPlayerInput(name, (GameChoice)Random.Range(0, 3));
    }

    /// <summary>
    /// Ran when the player clicks a choice button on the screen to submit a choice
    /// </summary>
    /// <param name="playerName">The name of the current player</param>
    /// <param name="gameChoice">The choice that the player just made</param>
    public void SubmitPlayerInput(string playerName, GameChoice gameChoice)
    {
        //verify that the player is not submitting multiple choices
        foreach(PlayerChoice playerChoice in playerChoices)
        {
            if (playerChoice.GetPlayerName().Equals(playerName))
            {
                Debug.LogError("Player " + playerName + " has made multiple choices this game.");
                return;
            }
        }

        //Add the choice to the list of choices
        playerChoices.Add(new PlayerChoice(playerName, gameChoice));

        //Find the next player or end the match
        ControlGame(playerName);
    }

    /// <summary>
    /// Ran on button press to start the next match
    /// </summary>
    public void StartNextMatch()
    {
        //Have we played all of our games yet?
        if (GameData.HasNextGame())
        {
            //allow input again
            resetBackground.SetActive(false);

            //clear the choices made during the previous match
            playerChoices.Clear();

            //clears the scores from the end of the last match
            matchScores.Clear();

            //Display the players scores
            playerScoreArea.text = GameData.GetPlayerScores();

            //empty the results display for the results of the previous match
            lastMatchResultText.text = "";

            //Restart the game with the first player added
            RunPlayerTurn(GameData.GetFirstPlayer());
        }
        else
        {
            gameOverScreen.SetActive(true);
            gameOverText.text = GameData.GetVictors();
        }
    }

    /// <summary>
    /// Ran on button press after the phone was passed to the appropriate player
    /// 
    /// Turns of the passThePhoneScreen object so a turn can be taken
    /// </summary>
    public void PhoneWasPassed()
    {
        passThePhoneScreen.SetActive(false);
    }
}

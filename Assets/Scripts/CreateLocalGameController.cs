using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A typical CreateLocalGameController is used to create a local game.
/// 
/// keeps a list of players and AI and their names
/// if multiple players and AI have the same name on game start, add indexing to the end of their names
/// 
/// I.E. 
///     P0 = "Paul"
///     P1 = "Paul"
///     
///     on game start, 
///         P0 = "Paul0"
///         P1 = "Paul1"
/// </summary>
public class CreateLocalGameController : MonoBehaviour
{
    //turn this class into a singleton
    public static CreateLocalGameController instance;

    [Header("The scenes to load on game start and on back button pressed")]
    [SerializeField] string gameSceneName;
    [SerializeField] string mainMenuSceneName;

    [Header("LGPE creation variables")]
    List<LocalGamePlayerElement> localGamePlayerElements = new List<LocalGamePlayerElement>();
    [SerializeField] GameObject localGamePlayerElementPrefab;
    [SerializeField] Transform localGamePlayerElementInstantiationLocation;

    [Header("How many turns does the game last")]
    string DURATION_PREFS = "DURATION_PREFS";
    [SerializeField] TMP_Text durationTitle;
    [SerializeField] Slider durationSlider;
    int duration;

    /// <summary>
    /// When the game object that this script is attached to turns on, set the instance to this class
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        duration = PlayerPrefs.GetInt(DURATION_PREFS, 0);
        durationSlider.value = duration;
        SetDurationTitle();
    }

    /// <summary>
    /// Ran on slider change to set the current duration of the game
    /// </summary>
    /// <param name="duration">How long does the game last</param>
    public void OnDurationChange(float duration)
    {
        this.duration = (int)duration;
        PlayerPrefs.SetInt(DURATION_PREFS, (int)duration);
        SetDurationTitle();
    }

    /// <summary>
    /// Display the title next to the durationSlider
    /// </summary>
    void SetDurationTitle()
    {
        switch (duration)
        {
            case 0:
                durationTitle.text = "Single";
                break;
            case 1:
                durationTitle.text = "Best of 3";
                break;
            case 2:
                durationTitle.text = "Best of 5";
                break;
            case 3:
                durationTitle.text = "Infinite";
                break;
        }
    }

    /// <summary>
    /// Add a player to the game, as long as there are not more than 7 players
    /// </summary>
    public void AddPlayer()
    {
        //do nothing if there are too many players
        if (ActiveElementsCount() > 7) return;

        //count the number of players
        int count = 0; 
        foreach(LocalGamePlayerElement playerElement in localGamePlayerElements)
        {
            if (playerElement.gameObject.activeInHierarchy && !playerElement.isAI) count++;
        }

        SetOrCreateElement("Player" + count, false);
    }

    /// <summary>
    /// Add an AI to the game, as long as there are not more than 7 players
    /// </summary>
    public void AddAI()
    {
        //do nothing if there are too many players
        if (ActiveElementsCount() > 7) return;

        //count the number of players
        int count = 0;
        foreach (LocalGamePlayerElement playerElement in localGamePlayerElements)
        {
            if (playerElement.gameObject.activeInHierarchy && playerElement.isAI) count++;
        }

        SetOrCreateElement("AI" + count, true);
    }

    /// <summary>
    /// Tells us how many elements are actually active
    /// </summary>
    /// <returns></returns>
    int ActiveElementsCount()
    {
        int count = 0;
        foreach (LocalGamePlayerElement playerElement in localGamePlayerElements)
        {
            if (playerElement.gameObject.activeInHierarchy) count++;
        }

        return count;
    }

    /// <summary>
    /// If there is a LocalGamePlayerElement in the list of localGamePlayerElements that is not being used, 
    ///     turn it on, set it as the first child and assign new values to it.
    ///     
    /// otherwise, instantiate a new LocalGamePlayerElement with the given values
    /// </summary>
    /// <param name="playerName">the name of the player to assign</param>
    /// <param name="isAI">if the player is AI or not</param>
    void SetOrCreateElement(string playerName, bool isAI)
    {
        //look for an innactive element
        foreach(LocalGamePlayerElement playerElement in localGamePlayerElements)
        {
            if (!playerElement.gameObject.activeInHierarchy)
            {
                playerElement.gameObject.SetActive(true);
                playerElement.transform.SetAsFirstSibling();
                playerElement.InitializeElement(playerName, isAI);
                return;
            }
        }

        //no innactive elements found, create a new one
        LocalGamePlayerElement localGamePlayerElement = Instantiate(localGamePlayerElementPrefab, localGamePlayerElementInstantiationLocation)
            .GetComponent<LocalGamePlayerElement>();

        localGamePlayerElement.transform.SetAsFirstSibling();
        localGamePlayerElement.InitializeElement(playerName, isAI);

        localGamePlayerElements.Add(localGamePlayerElement);
    }

    /// <summary>
    /// Turns an element in the list off
    /// </summary>
    /// <param name="playerElement"></param>
    public void DeleteElement(LocalGamePlayerElement playerElement)
    {
        foreach (LocalGamePlayerElement localGamePlayerElement in localGamePlayerElements)
        {
            if(localGamePlayerElement.Equals(playerElement))
            {
                localGamePlayerElement.gameObject.SetActive(false);
                return;
            }
        }
    }

    /// <summary>
    /// Start the game with the players added, if and only if there is more than 1 player.
    /// </summary>
    public void StartGame()
    {
        if (localGamePlayerElements.Count < 2) return;

        switch (duration)
        {
            case 0:
                GameData.SetDuration(1);
                break;
            case 1:
                GameData.SetDuration(3);
                break;
            case 2:
                GameData.SetDuration(5);
                break;
            case 3:
                GameData.SetDuration(int.MaxValue);
                break;
        }

        GameData.ClearPlayers();

        foreach(LocalGamePlayerElement localGamePlayerElement in localGamePlayerElements)
        {
            GameData.AddPlayer(localGamePlayerElement.playerName, localGamePlayerElement.isAI);
        }

        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Returns to the main screen
    /// </summary>
    public void BackButton()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

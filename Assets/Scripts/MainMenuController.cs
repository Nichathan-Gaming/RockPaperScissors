using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A typical MainMenuController controls the main menu by sending the user to either the local or online scenes on button press
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Get scene names")]
    [SerializeField] string localSceneName;
    [SerializeField] string onlineSceneName;

    /// <summary>
    /// Loads the CreateLocalGameScene
    /// </summary>
    public void LoadLocalScene()
    {
        SceneManager.LoadScene(localSceneName);
    }

    /// <summary>
    /// Loads the OnlineScene
    /// </summary>
    public void LoadOnlineScene()
    {
        SceneManager.LoadScene(onlineSceneName);
    }
}

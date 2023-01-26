using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A typical PlayerController obtains input from the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// determines if the player can submit a choice or not
    /// </summary>
    static bool canGetInput;
    /// <summary>
    /// The name of the player submitting a choice
    /// </summary>
    static string playerName;

    /// <summary>
    /// Changes the current player and awaits their input
    /// </summary>
    /// <param name="playerName">The name of the player we are awaiting input from</param>
    public static void CheckForPlayerInput(string playerName)
    {
        PlayerController.playerName = playerName;
        canGetInput = true;
    }

    /// <summary>
    /// Ran on button press to return the player choice
    /// </summary>
    /// <param name="gameChoice">Rock, Paper, Scissors</param>
    public void SubmitPlayerInput(int gameChoice)
    {
        if (!canGetInput) return;

        canGetInput = false;
        GameController.instance.SubmitPlayerInput(playerName, (GameChoice)gameChoice);
    }
}

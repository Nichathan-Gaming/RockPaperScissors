using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A typical LocalGamePlayerElement is added to a LocalGamePlayerElement prefab and is used to control the players data for a local game
/// </summary>
public class LocalGamePlayerElement : MonoBehaviour
{
    [Header("Element Values")]
    public string playerName;
    string defaultPlayerName;
    public bool isAI;

    [Header("Name Initialization Field")]
    [SerializeField] TMP_Text playerNamePlaceholder;

    [Header("Checkbox Control")]
    [SerializeField] Image checkImage;
    Color checkedColor = Color.green;
    Color uncheckedColor = Color.red;

    /// <summary>
    /// When the delete button on this element is pressed, tell CreateLocalGameController to delete this
    /// </summary>
    public void OnDelete()
    {
        CreateLocalGameController.instance.DeleteElement(this);
    }

    /// <summary>
    /// Initialize this element with default values
    /// </summary>
    /// <param name="playerName">the default player name in the placeholder</param>
    /// <param name="isAI">the default value shown in this element</param>
    public void InitializeElement(string playerName, bool isAI)
    {
        this.playerName = playerName;
        defaultPlayerName = playerName;
        this.isAI = isAI;

        checkImage.color = isAI ? checkedColor : uncheckedColor;
        playerNamePlaceholder.text = playerName;
    }

    /// <summary>
    /// Change the playerName of this element when the InputField is changed
    /// </summary>
    /// <param name="playerName"></param>
    public void OnPlayerNameChange(string playerName)
    {
        this.playerName = playerName;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            this.playerName = defaultPlayerName;
        }
    }

    /// <summary>
    /// Switch the isAI value on buttonPress
    /// </summary>
    public void SwitchIsAI()
    {
        isAI = !isAI;
        checkImage.color = isAI ? checkedColor : uncheckedColor;
    }
}

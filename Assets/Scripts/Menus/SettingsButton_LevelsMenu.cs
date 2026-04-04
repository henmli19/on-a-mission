using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsButton_LevelsMenu : MonoBehaviour
{
  
    [Header("Buttons")]
    public Button menuButton;
    
    void Start()
    {
   // Button listeners
        menuButton.onClick.AddListener(GoToMenu);
    }

 
    void GoToMenu()
    {
        Time.timeScale = 1f; // make sure game is unpaused
        SceneManager.LoadScene("MainMenu"); 
    }

}


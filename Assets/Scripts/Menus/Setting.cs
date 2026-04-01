using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Setting : MonoBehaviour
{
    [Header("Slider")]
    public Slider volumeSlider;

    [Header("Test Audio")]
    public AudioSource testAudio;

    [Header("Buttons")]
    public Button menuButton;
    public Button resumeButton;

    [Header("Panel")]
    public GameObject settingsPanel;
    public GameObject pausePanel;

    private const string VolumeKey = "GameVolume";

    void Start()
    {
        // Load volume
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Button listeners
        menuButton.onClick.AddListener(GoToMenu);
        resumeButton.onClick.AddListener(ResumeGame);

        // Play test audio
        if (testAudio != null && !testAudio.isPlaying)
        {
            testAudio.loop = true;
            testAudio.Play();
        }
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;

        if (testAudio != null)
            testAudio.volume = value;

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    void GoToMenu()
    {
        Time.timeScale = 1f; // make sure game is unpaused
        SceneManager.LoadScene("MainMenu"); 
    }

    void ResumeGame()
    {
        if (settingsPanel != null){
            settingsPanel.SetActive(false);
            pausePanel.SetActive(true);}


        
    }

}

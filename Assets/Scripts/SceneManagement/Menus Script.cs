using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public AK.Wwise.Event buttonClick;

    GameObject resumeButton;
    GameObject playButton;
    GameObject settingsButton;
    GameObject keybindsButton;
    GameObject quitButton;
    GameObject toMainMenuButton;

    Slider masterVolumeSlider;
    Slider musicVolumeSlider;
    Slider soundVolumeSlider;
    Slider dialogueVolumeSlider;
    Slider ambienceVolumeSlider;

    GameObject defaultMenuGroup;
    GameObject settingsGroup;
    GameObject keybindsMenuGroup;
    GameObject mainMenuObjects;

    bool menuOpen;
    bool settingsOpen;

    [NonSerialized] public bool muteAudio;
    [NonSerialized] public float masterVolume;
    [NonSerialized] public float musicVolume;
    [NonSerialized] public float soundVolume;
    [NonSerialized] public float dialogueVolume;
    [NonSerialized] public float ambienceVolume;

    public static MenuScript instance { get; private set; }
    private void Awake() {
        if (instance != null && instance != this) {
            DestroyImmediate(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    public void ChangeScene(string sceneName) {
        buttonClick.Post(gameObject);

        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToLevel() {
        buttonClick.Post(gameObject);

        SceneChangeTracker.GetTracker().GoBack();
    }

    public void Quit() {
        buttonClick.Post(gameObject);

        Application.Quit();
    }

    public void OpenSettings() {
        buttonClick.Post(gameObject);

        //defaultMenuGroup.GetComponent<RectTransform>().anchoredPosition = new Vector3(-600, -100, 0);
        settingsOpen = true;
        settingsGroup.SetActive(true);
        settingsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        settingsButton.GetComponent<Button>().onClick.AddListener(CloseSubMenu);
    }
    public void OpenMenu() {
        menuOpen = true;
        settingsGroup.SetActive(false);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu")) {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            resumeButton.SetActive(false);
            playButton.SetActive(true);
            toMainMenuButton.SetActive(false);
            quitButton.SetActive(true);
            playButton.GetComponent<Button>().Select();
            mainMenuObjects.SetActive(true);

        } else {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Time.timeScale = 0f;
            playButton.SetActive(false);
            resumeButton.SetActive(true);
            quitButton.SetActive(false);
            toMainMenuButton.SetActive(true);
            resumeButton.GetComponent<Button>().Select();
            mainMenuObjects.SetActive(false);
        }
    }

    public void CloseMenu() {
        menuOpen = false;
        Time.timeScale = 1f;
    }

    public void CloseSubMenu() {
        buttonClick.Post(gameObject);

        settingsOpen = false;
        settingsGroup.SetActive(false);
        //defaultMenuGroup.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -100, 0);
        settingsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Find all references
        resumeButton = GameObject.Find("ResumeButton");
        playButton = GameObject.Find("PlayButton");
        settingsButton = GameObject.Find("SettingsButton");
        keybindsButton = GameObject.Find("KeybindsButton");
        quitButton = GameObject.Find("QuitButton");
        toMainMenuButton = GameObject.Find("MainMenuButton");

        masterVolumeSlider = GameObject.Find("Master Volume").GetComponent<Slider>();
        musicVolumeSlider = GameObject.Find("Music Volume").GetComponent<Slider>();
        soundVolumeSlider = GameObject.Find("Sound Volume").GetComponent<Slider>();
        dialogueVolumeSlider = GameObject.Find("Dialogue Volume").GetComponent<Slider>();
        ambienceVolumeSlider = GameObject.Find("Ambience Volume").GetComponent<Slider>();

        defaultMenuGroup = GameObject.Find("DefaultMenuGroup");
        settingsGroup = GameObject.Find("SettingsGroup");
        keybindsMenuGroup = GameObject.Find("KeybindsGroup");
        mainMenuObjects = GameObject.Find("MainMenuObjects");

        //Set button functions
        resumeButton.GetComponent<Button>().onClick.AddListener(CloseMenu);
        //playButton
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        keybindsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        //toMainMenuButton.

        OpenMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (settingsOpen && menuOpen) {
            masterVolume = masterVolumeSlider.value;
            musicVolume = musicVolumeSlider.value;
            soundVolume = soundVolumeSlider.value;
            dialogueVolume = dialogueVolumeSlider.value;
            ambienceVolume = ambienceVolumeSlider.value;
        }
    }
}

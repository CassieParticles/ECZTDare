using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public AK.Wwise.Event buttonClick;
    public AK.Wwise.Event titleMusic;
    public AK.Wwise.Event titleRain;
    private AK.Wwise.Event sliderSound;
    private AlarmMusicHandler gameMusicScript;

    GameObject resumeButton;
    GameObject playButton;
    GameObject levelSelectButton;
    GameObject settingsButton;
    GameObject keybindsButton;
    GameObject quitButton;
    GameObject toMainMenuButton;

    Toggle muteAudioToggle;
    Slider masterVolumeSlider;
    Slider musicVolumeSlider;
    Slider soundVolumeSlider;
    Slider dialogueVolumeSlider;
    Slider ambienceVolumeSlider;

    GameObject defaultMenuGroup;
    GameObject settingsGroup;
    GameObject keybindsMenuGroup;


    bool menuOpen;
    bool settingsOpen;
    bool switchingScene = false;
    string previousScene;

    bool canPause = true;
    public bool paused;
    uint pausedMusic;

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
        AkSoundEngine.StopAll();
        buttonClick.Post(gameObject);

        SceneManager.LoadScene(sceneName);

        switchingScene = true;
        previousScene = SceneManager.GetActiveScene().name;
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

        canPause = true;
        menuOpen = true;
        CloseSubMenu();
        defaultMenuGroup.SetActive(true);
        if (SceneManager.GetActiveScene().name == "Main Menu") {
            Time.timeScale = 1f;
            //Sets the "Music" State Group's active State to "Hidden"
            AkSoundEngine.SetState("Music", "Menu");
            //Sets the "Ambience" State Group's active State to "NoAmbience"
            AkSoundEngine.SetState("Ambience", "Outside");
            titleMusic.Post(gameObject);
            titleRain.Post(gameObject);


            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            //GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            resumeButton.SetActive(false);
            playButton.SetActive(true);
            levelSelectButton.SetActive(true);
            toMainMenuButton.SetActive(false);
            quitButton.SetActive(true);
            playButton.GetComponent<Button>().Select();

        } else {
           // AkSoundEngine.GetState("Music", out pausedMusic);

            //Sets the "Music" State Group's active State to "Hidden"
            //AkSoundEngine.SetState("Music", "Menu");
            //titleMusic.Post(gameObject);

            paused = true;
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Time.timeScale = 0f;
            playButton.SetActive(false);
            levelSelectButton.SetActive(false);
            resumeButton.SetActive(true);
            quitButton.SetActive(false);
            toMainMenuButton.SetActive(true);
            resumeButton.GetComponent<Button>().Select();
        }
    }

    public void CloseMenu() {
        if (SceneManager.GetActiveScene().name != "Main Menu" && pausedMusic != 0) {
            //AkSoundEngine.SetState("Music", );
            pausedMusic = 0;
        }
        titleMusic.Stop(gameObject);
        titleRain.Stop(gameObject);

        paused = false;
        menuOpen = false;
        defaultMenuGroup.SetActive(false);
        settingsGroup.SetActive(false);
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
        levelSelectButton = GameObject.Find("LevelSelectButton");
        settingsButton = GameObject.Find("SettingsButton");
        keybindsButton = GameObject.Find("KeybindsButton");
        quitButton = GameObject.Find("QuitButton");
        toMainMenuButton = GameObject.Find("MainMenuButton");

        muteAudioToggle = GameObject.Find("MuteAudioToggle").GetComponent<Toggle>();
        masterVolumeSlider = GameObject.Find("Master Volume").GetComponent<Slider>();
        musicVolumeSlider = GameObject.Find("Music Volume").GetComponent<Slider>();
        soundVolumeSlider = GameObject.Find("Sound Volume").GetComponent<Slider>();
        dialogueVolumeSlider = GameObject.Find("Dialogue Volume").GetComponent<Slider>();
        ambienceVolumeSlider = GameObject.Find("Ambience Volume").GetComponent<Slider>();

        defaultMenuGroup = GameObject.Find("DefaultMenuGroup");
        settingsGroup = GameObject.Find("SettingsGroup");
        keybindsMenuGroup = GameObject.Find("KeybindsGroup");

        //Set button functions
        resumeButton.GetComponent<Button>().onClick.AddListener(CloseMenu);
        //playButton
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        keybindsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        //toMainMenuButton.
        if (SceneManager.GetActiveScene().name == "Main Menu") {
            OpenMenu();
        } else {
            CloseMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (switchingScene && previousScene != SceneManager.GetActiveScene().name) {
            switchingScene = false;
            if (SceneManager.GetActiveScene().name == "Main Menu") {
                OpenMenu();
            } else {
                CloseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Main Menu" && canPause) {
            if (!paused) {
                OpenMenu();
            } else {
                CloseMenu();
            }
        }

        if (settingsOpen && menuOpen) {
            muteAudio = muteAudioToggle.isOn;
            if (muteAudio) {
                AkSoundEngine.SetRTPCValue("MasterVolume", 0);
            } else {
                masterVolume = masterVolumeSlider.value;
                AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
            }    
            musicVolume = musicVolumeSlider.value;
            AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);
            soundVolume = soundVolumeSlider.value;
            AkSoundEngine.SetRTPCValue("SoundVolume", soundVolume);
            dialogueVolume = dialogueVolumeSlider.value;
            AkSoundEngine.SetRTPCValue("DialogueVolume", dialogueVolume);
            ambienceVolume = ambienceVolumeSlider.value;
            AkSoundEngine.SetRTPCValue("AmbienceVolume", ambienceVolume);
            if (!Input.GetMouseButton(0)) {
                //sliderSound.Stop(gameObject);
            }
        }
    }

    public void SliderChangeSound(AK.Wwise.Event sound) {
        
        if (sound != sliderSound) {
            sound = sliderSound;
            sound.Post(gameObject);
        }
    }
}

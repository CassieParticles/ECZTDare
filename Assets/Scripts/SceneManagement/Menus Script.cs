using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static ControlsScript;

public class MenuScript : MonoBehaviour
{
    public AK.Wwise.Event buttonClick;
    public AK.Wwise.Event titleMusic;
    public AK.Wwise.Event titleRain;
    public AK.Wwise.Event loseSound;
    private AK.Wwise.Event sliderSound;
    private AlarmMusicHandler gameMusicScript;

    GameObject resumeButton;
    GameObject playButton;
    GameObject levelSelectButton;
    GameObject settingsButton;
    GameObject keybindsButton;
    GameObject quitButton;
    GameObject toMainMenuButton;
    GameObject nextLevelButton;
    GameObject creditsButton;

    Toggle muteAudioToggle;
    Slider masterVolumeSlider;
    Slider musicVolumeSlider;
    Slider soundVolumeSlider;
    Slider dialogueVolumeSlider;
    Slider ambienceVolumeSlider;

    GameObject defaultMenuGroup;
    GameObject slideshowGroup;
    GameObject settingsGroup;
    GameObject keybindsGroup;
    GameObject levelsGroup;
    GameObject winGroup;
    GameObject loseGroup;
    GameObject scoringSubGroup;
    GameObject creditsGroup;

    GameObject uiCanvas;
    GameObject player;

    ControlsScript controlScript;

    bool menuOpen;
    bool settingsOpen;
    bool switchingScene = false;
    string previousScene;
    public bool hasUpgrade = false;

    bool lost = false;
    int deathCounter; //This is to allow for skipping lose screen but can be reused I guess

    bool canPause = true;
    public bool paused;
    public bool keybindsOpen;
    public bool creditsOpen;
    uint pausedMusic;

    [NonSerialized] public bool muteAudio;
    [NonSerialized] public float masterVolume;
    [NonSerialized] public float musicVolume;
    [NonSerialized] public float soundVolume;
    [NonSerialized] public float dialogueVolume;
    [NonSerialized] public float ambienceVolume;

    public float loseSoundDelay;

    //I didnt want to do this but due to controlsScript's update function literally just not running in exclusively build mode I had to move all of this shit here instead :(
    TextMeshProUGUI rebindLeftButtonKey;
    TextMeshProUGUI rebindRightButtonKey;
    TextMeshProUGUI rebindJumpButtonKey;
    TextMeshProUGUI rebindSlideButtonKey;
    TextMeshProUGUI rebindBoostCloakButtonKey;
    TextMeshProUGUI rebindHackButtonKey;

    GameObject resetRunButton;
    GameObject resetJumpButton;
    GameObject resetSlideButton;
    GameObject resetBoostCloakButton;
    GameObject resetHackButton;


    public static MenuScript instance { get; private set; }
    private void Awake() {
        if (instance != null && instance != this) {
            DestroyImmediate(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(this);
            Starts();
        }
    }

    private void Start() {
        if (hasUpgrade) {
            GameObject.Find("GameController").GetComponent<UIModeChange>().CollectUpgrade();
        }
    }
    public void ChangeScene(string sceneName)
    {
        AkSoundEngine.StopAll();
        buttonClick.Post(gameObject);
        canPause = true;

        if (sceneName == "Next Level") {
            if (SceneManager.GetActiveScene().name == "Tutorial") {
                sceneName = "Level1 Redesign";
            } else if (SceneManager.GetActiveScene().name == "Level1 Redesign") {
                sceneName = "Level2 Redesign";
            } else if (SceneManager.GetActiveScene().name == "Level2 Redesign") {
                sceneName = "Main Menu"; 
            }
        }

        winGroup.SetActive(false);
        loseGroup.SetActive(false);

        MainScoreController scoreController = MainScoreController.GetInstance();
        CheckpointManager checkpointManager = FindAnyObjectByType<CheckpointManager>();
        //DEstroy the main score controller when quitting

        if (scoreController)
        {
            scoreController.Quit();
        }

        if (SceneManager.GetActiveScene().name != "Main Menu" && checkpointManager) {
            checkpointManager.Quit();
        }
    
        

        

        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
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

    public void OpenSlideshow() {
        buttonClick.Post(gameObject);

        CloseSubMenu();
        if (SceneManager.GetActiveScene().name == "Main Menu") {
            slideshowGroup.SetActive(true);
        }
    }

    public void OpenSettings() {
        buttonClick.Post(gameObject);

        CloseSubMenu();

        settingsOpen = true;
        settingsGroup.SetActive(true);
        settingsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSlideshow);
    }

    public void OpenLevelSelect() {
        buttonClick.Post(gameObject);
        
        CloseSubMenu();

        levelsGroup.SetActive(true);
        levelSelectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        levelSelectButton.GetComponent<Button>().onClick.AddListener(OpenSlideshow);
    }

    public void OpenKeybinds() {
        buttonClick.Post(gameObject);

        CloseSubMenu();

        keybindsOpen = true;
        keybindsGroup.SetActive(true);
        keybindsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        keybindsButton.GetComponent<Button>().onClick.AddListener(OpenSlideshow);
    }

    public void ResetAudioSettings() {
        muteAudioToggle.isOn = false;
        masterVolumeSlider.value = 80;
        musicVolumeSlider.value = 80;
        soundVolumeSlider.value = 80;
        dialogueVolumeSlider.value = 80;
        ambienceVolumeSlider.value = 80;
    }

    public void OpenCredits() {
        buttonClick.Post(gameObject);


        if (creditsOpen) {
            CloseSubMenu();
            OpenSlideshow();
            creditsOpen = false;
        } else {
            CloseSubMenu();
            creditsGroup.SetActive(true);
            creditsOpen = true;
        }


    }
    public void OpenMenu() {
        winGroup.SetActive(false);
        loseGroup.SetActive(false);
        canPause = true;
        menuOpen = true;
        CloseSubMenu();
        defaultMenuGroup.SetActive(true);
        if (SceneManager.GetActiveScene().name == "Main Menu") {
            Time.timeScale = 1f;
            hasUpgrade = false;
            //Sets the "Music" State Group's active State to "Hidden"
            AkSoundEngine.SetState("Music", "Menu");
            //Sets the "Ambience" State Group's active State to "NoAmbience"
            AkSoundEngine.SetState("Ambience", "Outside");
            titleMusic.Post(gameObject);
            titleRain.Post(gameObject);

            slideshowGroup.SetActive(true);

            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            //GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            resumeButton.SetActive(false);
            playButton.SetActive(true);
            levelSelectButton.SetActive(true);
            toMainMenuButton.SetActive(false);
            quitButton.SetActive(true);
            creditsButton.SetActive(true);
            
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
            creditsButton.SetActive(false);
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
        
        CloseSubMenu();

        defaultMenuGroup.SetActive(false);
        creditsButton.SetActive(false);
        winGroup.SetActive(false);
        loseGroup.SetActive(false);

        GetComponent<ControlsScript>().controls.Enable();

        Time.timeScale = 1f;
    }

    public void CloseSubMenu() {    
        slideshowGroup.SetActive(false);

        levelsGroup.SetActive(false);
        levelSelectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        levelSelectButton.GetComponent<Button>().onClick.AddListener(OpenLevelSelect);

        settingsOpen = false;
        settingsGroup.SetActive(false);
        settingsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);

        if (SceneManager.GetActiveScene().name == "Tutorial" && GameObject.Find("TutText") != null) {
            ControlsScript controls = GetComponent<ControlsScript>();
            GameObject.Find("TutText").GetComponent<TutorialText>().Refresh(controlScript.controls.GameplayControls.Jumping.bindings[0].ToDisplayString(),
                                                                            controlScript.controls.GameplayControls.Sliding.bindings[0].ToDisplayString(),
                                                                            controlScript.controls.GameplayControls.Hacking.bindings[0].ToDisplayString());
        }
        keybindsOpen = false;
        keybindsGroup.SetActive(false);
        keybindsButton.GetComponent<Button>().onClick.RemoveAllListeners();
        keybindsButton.GetComponent<Button>().onClick.AddListener(OpenKeybinds);

        creditsGroup.SetActive(false);
        creditsOpen = false;
    }

    public void Win() {
        canPause = false;
        Time.timeScale = 0;
        uiCanvas = GameObject.Find("UICanvas");
        uiCanvas.SetActive(false);
        if (SceneManager.GetActiveScene().name == "Tutorial") {
            scoringSubGroup.SetActive(false);
        } else {
            scoringSubGroup.SetActive(true);
        }
        winGroup.SetActive(true);
        nextLevelButton.GetComponent<Button>().Select();
    }
    
    public void Lose() {
        canPause = false;
        if (!loseGroup.activeSelf) {
            player = GameObject.Find("Player");
            GameObject.Find("MovementFollowerCamera").GetComponent<CinemachineVirtualCamera>().Follow.position += Vector3.up * 1000; 
            GameObject.Find("StealthFollowerCamera").GetComponent<CinemachineVirtualCamera>().Follow.position += Vector3.up * 1000;
            //Bec add your music mode change

            //
            player.SetActive(false);
            loseGroup.SetActive(true);
            StartCoroutine(LoseDelay(7f));
        }
    }

    IEnumerator LoseDelay(float seconds) {
        if (!hasUpgrade) {
            hasUpgrade = player.GetComponent<MovementScript>().boostCloakUnlocked;
        }

        int currentDeaths = deathCounter;
        yield return new WaitForSeconds(loseSoundDelay);
        loseSound.Post(gameObject);
        yield return new WaitForSeconds(seconds-loseSoundDelay);
        if (currentDeaths == deathCounter) {
            StartCoroutine(LoseFinalize());
        }

    }

    IEnumerator LoseFinalize() {
        loseSound.Stop(gameObject);
        deathCounter++;
        switchingScene = true;
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (MainScoreController.GetInstance()) {
            MainScoreController.GetInstance().Unpause();
        }
        yield return new WaitForFixedUpdate();
        loseGroup.SetActive(false);
        canPause = true;
        lost = true;
    }

    // Start is called before the first frame update
    void Starts()
    {
        controlScript = GetComponent<ControlsScript>();

        //Find all references
        resumeButton = GameObject.Find("ResumeButton");
        playButton = GameObject.Find("PlayButton");
        levelSelectButton = GameObject.Find("LevelSelectButton");
        settingsButton = GameObject.Find("SettingsButton");
        keybindsButton = GameObject.Find("KeybindsButton");
        quitButton = GameObject.Find("QuitButton");
        toMainMenuButton = GameObject.Find("MainMenuButton");
        nextLevelButton = GameObject.Find("NextLevelButton");
        creditsButton = GameObject.Find("CreditsButton");

        muteAudioToggle = GameObject.Find("MuteAudioToggle").GetComponent<Toggle>();
        masterVolumeSlider = GameObject.Find("Master Volume").GetComponent<Slider>();
        musicVolumeSlider = GameObject.Find("Music Volume").GetComponent<Slider>();
        soundVolumeSlider = GameObject.Find("Sound Volume").GetComponent<Slider>();
        dialogueVolumeSlider = GameObject.Find("Dialogue Volume").GetComponent<Slider>();
        ambienceVolumeSlider = GameObject.Find("Ambience Volume").GetComponent<Slider>();

        defaultMenuGroup = GameObject.Find("DefaultMenuGroup");
        slideshowGroup = GameObject.Find("SlideshowGroup");
        levelsGroup = GameObject.Find("LevelsGroup");
        settingsGroup = GameObject.Find("SettingsGroup");
        keybindsGroup = GameObject.Find("KeybindsGroup");
        winGroup = GameObject.Find("WinGroup");
        loseGroup = GameObject.Find("LoseGroup");
        scoringSubGroup = GameObject.Find("ScoringSubGroup");
        creditsGroup = GameObject.Find("CreditsGroup");

        //Set button functions
        resumeButton.GetComponent<Button>().onClick.AddListener(CloseMenu);
        //playButton
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        keybindsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        //toMainMenuButton.

        rebindLeftButtonKey = GameObject.Find("RebindLeftKey").GetComponent<TextMeshProUGUI>();
        rebindRightButtonKey = GameObject.Find("RebindRightKey").GetComponent<TextMeshProUGUI>();
        rebindJumpButtonKey = GameObject.Find("RebindJumpKey").GetComponent<TextMeshProUGUI>();
        rebindSlideButtonKey = GameObject.Find("RebindSlideKey").GetComponent<TextMeshProUGUI>();
        rebindBoostCloakButtonKey = GameObject.Find("RebindBoostCloakKey").GetComponent<TextMeshProUGUI>();
        rebindHackButtonKey = GameObject.Find("RebindHackKey").GetComponent<TextMeshProUGUI>();

        resetRunButton = GameObject.Find("ResetRunButton");
        resetJumpButton = GameObject.Find("ResetJumpButton");
        resetSlideButton = GameObject.Find("ResetSlideButton");
        resetBoostCloakButton = GameObject.Find("ResetBoostCloakButton");
        resetHackButton = GameObject.Find("ResetHackButton");

        defaultMenuGroup.SetActive(true);
        slideshowGroup.SetActive(true);
        levelsGroup.SetActive(false);
        settingsGroup.SetActive(false);
        keybindsGroup.SetActive(false);
        winGroup.SetActive(false);
        loseGroup.SetActive(false);
        creditsGroup.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Main Menu") {
            OpenMenu();
        } else {
            if (SceneManager.GetActiveScene().name == "Level2") {
                hasUpgrade = true;
            }
            CloseMenu();
        }

        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (switchingScene && (previousScene != SceneManager.GetActiveScene().name || lost)) {
            switchingScene = false;
            lost = false;
            CloseSubMenu();
            if (SceneManager.GetActiveScene().name == "Main Menu") {
                OpenMenu();
            } else {
                CloseMenu();
                if (hasUpgrade || SceneManager.GetActiveScene().name == "Level2") {
                    GameObject.Find("GameController").GetComponent<UIModeChange>().CollectUpgrade();
                    hasUpgrade = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Main Menu" && canPause) {
            if (!paused) {
                OpenMenu();
                GetComponent<ControlsScript>().controls.Disable();
            } else {
                CloseMenu();
                GetComponent<ControlsScript>().controls.Enable();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && loseGroup.activeSelf && SceneManager.GetActiveScene().name != "Main Menu") {
            StartCoroutine(LoseFinalize());

        }

        if (menuOpen) {
            if (settingsOpen) {
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
            } if (keybindsOpen) {
                rebindLeftButtonKey.text = controlScript.controls.GameplayControls.Running.bindings[1].ToDisplayString();
                rebindRightButtonKey.text = controlScript.controls.GameplayControls.Running.bindings[2].ToDisplayString();
                rebindJumpButtonKey.text = controlScript.controls.GameplayControls.Jumping.bindings[0].ToDisplayString();
                rebindSlideButtonKey.text = controlScript.controls.GameplayControls.Sliding.bindings[0].ToDisplayString();
                rebindBoostCloakButtonKey.text = controlScript.controls.GameplayControls.BoostCloak.bindings[0].ToDisplayString();
                rebindHackButtonKey.text = controlScript.controls.GameplayControls.Hacking.bindings[0].ToDisplayString();

                if (controlScript.controls.GameplayControls.Running.bindings[1].hasOverrides || controlScript.controls.GameplayControls.Running.bindings[2].hasOverrides) {
                    resetRunButton.SetActive(true);
                } else {
                    resetRunButton.SetActive(false);
                }
                if (controlScript.controls.GameplayControls.Jumping.bindings[0].hasOverrides) {
                    resetJumpButton.SetActive(true);
                } else {
                    resetJumpButton.SetActive(false);
                }
                if (controlScript.controls.GameplayControls.Sliding.bindings[0].hasOverrides) {
                    resetSlideButton.SetActive(true);
                } else {
                    resetSlideButton.SetActive(false);
                }
                if (controlScript.controls.GameplayControls.BoostCloak.bindings[0].hasOverrides) {
                    resetBoostCloakButton.SetActive(true);
                } else {
                    resetBoostCloakButton.SetActive(false);
                }
                if (controlScript.controls.GameplayControls.Hacking.bindings[0].hasOverrides) {
                    resetHackButton.SetActive(true);
                } else {
                    resetHackButton.SetActive(false);
                }
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

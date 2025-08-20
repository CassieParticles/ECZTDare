using Cinemachine;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ControlsScript;
using static PlayerControls;

public class MenuScript : MonoBehaviour, IMenuControlsActions {
    public AK.Wwise.Event buttonClick;
    public AK.Wwise.Event titleMusic;
    public AK.Wwise.Event titleRain;
    public AK.Wwise.Event loseSound;
    public AK.Wwise.Event playerDeath;
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
    Image TransitionImage;

    ControlsScript controlScript;
    PlayerControls controls;

    bool menuOpen;
    bool settingsOpen;
    bool switchingScene = false;
    string previousScene;
    public bool hasUpgrade = false;

    bool lost = false;
    int deathCounter; //This is to allow for skipping lose screen but can be reused I guess

    public bool canPause = true;
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
    public float sceneTransitionSeconds = 1f;
    bool transitioning = false;

    //I didnt want to do this but due to controlsScript's update function literally just not running in exclusively build mode I had to move all of this shit here instead :(
    TextMeshProUGUI controlSchemeText;
    TextMeshProUGUI aimHackText;

    TextMeshProUGUI rebindLeftButtonKey;
    TextMeshProUGUI rebindRightButtonKey;
    TextMeshProUGUI rebindJumpButtonKey;
    TextMeshProUGUI rebindSlideButtonKey;
    TextMeshProUGUI rebindDashButtonKey;
    TextMeshProUGUI rebindCloakButtonKey;
    TextMeshProUGUI rebindHackButtonKey;

    GameObject resetRunButton;
    GameObject resetJumpButton;
    GameObject resetSlideButton;
    GameObject resetDashButton;
    GameObject resetCloakButton;
    GameObject resetHackButton;

    public StealthCamera currentCameraPan = null;


    private StringBuilder stringBuilder;

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
        stringBuilder = new StringBuilder();
        controls.MenuControls.Pause.started += ctx => Pause();
    }

    public void FinishAndSaveLevel(string sceneName) {

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null) {
            scoreManager.SaveScoresToJson();
        }

        ChangeScene(sceneName);
    }
    public void ChangeScene(string sceneName)
    {
        //Immediately remove current camera pan
        currentCameraPan = null;

        if (transitioning) {
            return;
        }
        transitioning = true;
        if (sceneName == "Next Level") {
            if (SceneManager.GetActiveScene().name == "Tutorial") {
                sceneName = "Level 1";
            } else if (SceneManager.GetActiveScene().name == "Level 1") {
                sceneName = "MiddleCutscene";
            } else if (SceneManager.GetActiveScene().name == "Boss Level (2v3)") {
                sceneName = "FinalCutscene";
            }
        }
        if (GameObject.Find("Lights") != null) {
            GameObject.Find("Lights").SetActive(false);
        }

        buttonClick.Post(gameObject);
        StartCoroutine(MenuTransitionFade(sceneName));
    }

    IEnumerator MenuTransitionFade(string sceneName) {
        Color black = new Color(0, 0, 0, 1);
        Color empty = new Color(0, 0, 0, 0);
        for (float i = 0; i < sceneTransitionSeconds;) { //Fade to black
            i += Time.unscaledDeltaTime;
            float percentage = i / sceneTransitionSeconds;
            TransitionImage.color = Color.Lerp(empty, black, i);
            yield return null;
        }

        //Do general setup for scene switching
        AkSoundEngine.StopAll();

        winGroup.SetActive(false);
        loseGroup.SetActive(false);

        //Destroy the main score controller when quitting
        MainScoreController scoreController = MainScoreController.GetInstance();
        if (scoreController) {
            scoreController.Quit();
        } //And the checkpoint manager
        CheckpointManager checkpointManager = FindAnyObjectByType<CheckpointManager>();
        if (SceneManager.GetActiveScene().name != "Main Menu" && checkpointManager) {
            checkpointManager.Quit();
        }
        //Yes this sucks
        DeathwallRespawn deathWallRespawner = FindAnyObjectByType<DeathwallRespawn>();
        if(deathWallRespawner)
        {
            deathWallRespawner.Quit();
        } //Im hopping onto the sucking train
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager) {
            scoreManager.Quit();
        }
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
        switchingScene = true;
        previousScene = SceneManager.GetActiveScene().name;
        yield return new WaitForFixedUpdate();
        for (int i  = 0; i < 10; i++) {
            yield return null;
        }

        for (float i = 0; i < sceneTransitionSeconds;) { //Fade back to game
            i += Time.unscaledDeltaTime;
            float percentage = i / sceneTransitionSeconds;
            TransitionImage.color = Color.Lerp(black, empty, i);
            yield return null;
        }
        canPause = true;
        transitioning = false;


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

        controlScript.controls.GameplayControls.Disable();

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
            GetComponent<Canvas>().worldCamera = Camera.main;
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
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            GetComponent<Canvas>().worldCamera = Camera.main;
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
        

        defaultMenuGroup.SetActive(false);
        creditsButton.SetActive(false);
        winGroup.SetActive(false);
        loseGroup.SetActive(false);

        if (GetComponent<ControlsScript>().controls != null) {
            GetComponent<ControlsScript>().controls.Enable();
        }

        Time.timeScale = 1f;
        CloseSubMenu();

        //Update all GUIs
        foreach (TutorialTextUpdate text in FindObjectsByType<TutorialTextUpdate>(FindObjectsSortMode.None))
        {
            text.RefreshText();
        }
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

        //if (SceneManager.GetActiveScene().name == "Tutorial" && GameObject.Find("TutText") != null) {
        //    ControlsScript controls = GetComponent<ControlsScript>();
        //    GameObject.Find("TutText").GetComponent<TutorialText>().Refresh(controlScript.controls.GameplayControls.Jumping.bindings[0].ToDisplayString(),
        //                                                                    controlScript.controls.GameplayControls.Sliding.bindings[0].ToDisplayString(),
        //                                                                    controlScript.controls.GameplayControls.Hacking.bindings[0].ToDisplayString());
        //}
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
        uiCanvas.transform.GetChild(0).gameObject.SetActive(false);
        uiCanvas.transform.GetChild(1).gameObject.SetActive(false);

        StartCoroutine(WinFinalize());
    }

    IEnumerator WinFinalize() {
        BreakroomDisplay breakroomDisplay = FindAnyObjectByType<BreakroomDisplay>();
        if (breakroomDisplay != null) {
            while (breakroomDisplay.scoringCoroutineRunning) {
                yield return new WaitForSecondsRealtime(0.016f);
            }
        }
        
        winGroup.SetActive(true);
        nextLevelButton.GetComponent<Button>().Select();
    }
    
    public void Lose() {

        //Already losing
        if (loseGroup.activeSelf){ return; }
        //INITIAL LOSE STEPS

        canPause = false;
        player = GameObject.Find("Player");
        GameObject.Find("MovementFollowerCamera").GetComponent<CinemachineVirtualCamera>().Follow.position += Vector3.up * 1000;
        player.SetActive(false);
        loseGroup.SetActive(true);

        //Death SFX
        playerDeath.Post(gameObject);
        //Turns off the music event
        FindAnyObjectByType<AlarmMusicHandler>().TurnOffMusic();
        Debug.Log("Turned Off Music");

        //Check if player should have upgrade
        if (!hasUpgrade)
        {
            hasUpgrade = player.GetComponent<MovementScript>().cloakUnlocked;
            
        }
    }

    IEnumerator LoseFinalize() {
        loseSound.Stop(gameObject);
        playerDeath.Stop(gameObject);
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
        TransitionImage = GameObject.Find("MenuTransitionFade").GetComponent<Image>();

        //Set button functions
        resumeButton.GetComponent<Button>().onClick.AddListener(CloseMenu);
        //playButton
        settingsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        keybindsButton.GetComponent<Button>().onClick.AddListener(OpenSettings);
        quitButton.GetComponent<Button>().onClick.AddListener(Quit);
        //toMainMenuButton.

        controlSchemeText = GameObject.Find("ControlSchemeText").GetComponent<TextMeshProUGUI>();
        aimHackText = GameObject.Find("AimhackText").GetComponent<TextMeshProUGUI>();

        rebindLeftButtonKey = GameObject.Find("RebindLeftKey").GetComponent<TextMeshProUGUI>();
        rebindRightButtonKey = GameObject.Find("RebindRightKey").GetComponent<TextMeshProUGUI>();
        rebindJumpButtonKey = GameObject.Find("RebindJumpKey").GetComponent<TextMeshProUGUI>();
        rebindSlideButtonKey = GameObject.Find("RebindSlideKey").GetComponent<TextMeshProUGUI>();
        rebindDashButtonKey = GameObject.Find("RebindDashKey").GetComponent<TextMeshProUGUI>();
        rebindCloakButtonKey = GameObject.Find("RebindCloakKey").GetComponent<TextMeshProUGUI>();
        rebindHackButtonKey = GameObject.Find("RebindHackKey").GetComponent<TextMeshProUGUI>();

        resetRunButton = GameObject.Find("ResetRunButton");
        resetJumpButton = GameObject.Find("ResetJumpButton");
        resetSlideButton = GameObject.Find("ResetSlideButton");
        resetDashButton = GameObject.Find("ResetDashButton");
        resetCloakButton = GameObject.Find("ResetCloakButton");
        resetHackButton = GameObject.Find("ResetHackButton");

        defaultMenuGroup.SetActive(true);
        slideshowGroup.SetActive(true);
        levelsGroup.SetActive(false);
        settingsGroup.SetActive(false);
        keybindsGroup.SetActive(false);
        winGroup.SetActive(false);
        loseGroup.SetActive(false);
        creditsGroup.SetActive(false);

        controlScript.Setup();

        controls = controlScript.controls;
        controls.MenuControls.SetCallbacks(this);

        gameMusicScript = FindAnyObjectByType<AlarmMusicHandler>();

        if (SceneManager.GetActiveScene().name == "Main Menu") {
            OpenMenu();
        } else {
            CloseMenu();
        }

        //Create a new save file if none exists
        string filePath = Application.persistentDataPath + "/ScoreData.json";
        if (!System.IO.File.Exists(filePath)) {
            System.IO.File.Create(filePath);
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
            GetComponent<Canvas>().worldCamera = Camera.main;
            if (SceneManager.GetActiveScene().name == "Main Menu") {
                OpenMenu();
            } else {
                CloseMenu();
                if (hasUpgrade || SceneManager.GetActiveScene().name == "Boss Level (2v3)") {
                    GameObject.Find("GameController").GetComponent<UIModeChange>().CollectUpgrade();
                    FindAnyObjectByType<BatteryBar>().SetCloakBar();
                    hasUpgrade = true;
                }
            }
        }

        

        //if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "Main Menu" && canPause) {
        //    if (!paused) {
        //        OpenMenu();
        //        GetComponent<ControlsScript>().controls.Disable();
        //    } else {
        //        CloseMenu();
        //        GetComponent<ControlsScript>().controls.Enable();
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Escape)) {
        //    Pause();
        //}

        //if (Input.GetKeyDown(KeyCode.Escape) && loseGroup.activeSelf && SceneManager.GetActiveScene().name != "Main Menu") {
        //    StartCoroutine(LoseFinalize());

        //}

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
            }
            KeybindDisplay();
        }
    }

    private void KeybindDisplay() {
        if (keybindsOpen) {
            if (controlScript.playerInput.currentControlScheme == "KeyboardMouse") {
                controlSchemeText.text = "Currently using: Keyboard and Mouse";
                aimHackText.text = "To aim the Hacking Reticle, move the Mouse";

                rebindLeftButtonKey.text = controlScript.controls.GameplayControls.Running.bindings[1].ToDisplayString();
                rebindRightButtonKey.text = controlScript.controls.GameplayControls.Running.bindings[2].ToDisplayString();
                rebindJumpButtonKey.text = controlScript.controls.GameplayControls.Jumping.bindings[0].ToDisplayString();
                rebindSlideButtonKey.text = controlScript.controls.GameplayControls.Sliding.bindings[0].ToDisplayString();
                rebindDashButtonKey.text = controlScript.controls.GameplayControls.Dashing.bindings[0].ToDisplayString();
                rebindCloakButtonKey.text = controlScript.controls.GameplayControls.Cloaking.bindings[0].ToDisplayString();
                rebindHackButtonKey.text = controlScript.controls.GameplayControls.Hacking.bindings[0].ToDisplayString();
            } else {
                controlSchemeText.text = "Currently using: Gamepad";
                aimHackText.text = "To aim the Hacking Reticle, use the Right Stick";

                if (controlScript.controls.GameplayControls.Running.bindings[4].hasOverrides) { 
                    rebindLeftButtonKey.text = controlScript.controls.GameplayControls.Running.bindings[4].ToDisplayString();
                } else {
                    rebindLeftButtonKey.text = stringBuilder.Clear().Append(controlScript.controls.GameplayControls.Running.bindings[4].ToDisplayString()).Append('\n').Append(controlScript.controls.GameplayControls.Running.bindings[7].ToDisplayString()).ToString();
                } //Running Left
                if (controlScript.controls.GameplayControls.Running.bindings[5].hasOverrides) {
                    rebindRightButtonKey.text = controlScript.controls.GameplayControls.Running.bindings[5].ToDisplayString();
                } else {
                    rebindRightButtonKey.text = stringBuilder.Clear().Append(controlScript.controls.GameplayControls.Running.bindings[5].ToDisplayString()).Append('\n').Append(controlScript.controls.GameplayControls.Running.bindings[8].ToDisplayString()).ToString();
                } //Running Right
                rebindJumpButtonKey.text = controlScript.controls.GameplayControls.Jumping.bindings[1].ToDisplayString(); //Jumping
                if (controlScript.controls.GameplayControls.Sliding.bindings[0].hasOverrides) {
                    rebindSlideButtonKey.text = controlScript.controls.GameplayControls.Sliding.bindings[1].ToDisplayString();
                } else {
                    rebindSlideButtonKey.text = stringBuilder.Clear().Append(controlScript.controls.GameplayControls.Sliding.bindings[1].ToDisplayString()).Append('\n').Append(controlScript.controls.GameplayControls.Sliding.bindings[2].ToDisplayString()).ToString();
                } //Sliding
                rebindDashButtonKey.text = controlScript.controls.GameplayControls.Dashing.bindings[1].ToDisplayString(); //Dashing
                rebindCloakButtonKey.text = controlScript.controls.GameplayControls.Cloaking.bindings[1].ToDisplayString(); //Cloaking
                rebindHackButtonKey.text = controlScript.controls.GameplayControls.Hacking.bindings[1].ToDisplayString(); //Hacking
            }

            if (controlScript.controls.GameplayControls.Running.bindings[1].hasOverrides || controlScript.controls.GameplayControls.Running.bindings[2].hasOverrides || controlScript.controls.GameplayControls.Running.bindings[4].hasOverrides || controlScript.controls.GameplayControls.Running.bindings[5].hasOverrides) {
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
            if (controlScript.controls.GameplayControls.Dashing.bindings[0].hasOverrides) {
                resetDashButton.SetActive(true);
            } else {
                resetDashButton.SetActive(false);
            }
            if (controlScript.controls.GameplayControls.Cloaking.bindings[0].hasOverrides) {
                resetCloakButton.SetActive(true);
            } else {
                resetCloakButton.SetActive(false);
            }
            if (controlScript.controls.GameplayControls.Hacking.bindings[0].hasOverrides) {
                resetHackButton.SetActive(true);
            } else {
                resetHackButton.SetActive(false);
            }
        }
    }

    public void Pause() {
        if (SceneManager.GetActiveScene().name != "Main Menu" && canPause && !keybindsOpen) {
            if (!paused) {
                OpenMenu();
                GetComponent<ControlsScript>().controls.GameplayControls.Disable();
            } else {
                CloseMenu();
                GetComponent<ControlsScript>().controls.GameplayControls.Enable();
            }
        }
        VideoPlayUI videoPlayer = FindAnyObjectByType<VideoPlayUI>(FindObjectsInactive.Exclude);
        if (videoPlayer)
        {
            videoPlayer.CloseVideo();
        }
        if (loseGroup.activeSelf && SceneManager.GetActiveScene().name != "Main Menu") {
            StartCoroutine(LoseFinalize());
        }
        if(currentCameraPan)
        {
            currentCameraPan.ExitCutscene();
        }
    }

    

    public void OnPause(InputAction.CallbackContext context) {

    }

    public void OnReset(InputAction.CallbackContext context) {
        if (SceneManager.GetActiveScene().name != "Main Menu" && !settingsOpen && !keybindsOpen) {
            StartCoroutine(LoseFinalize());
            if (paused) {
                Pause();
            }
        }
    }
}

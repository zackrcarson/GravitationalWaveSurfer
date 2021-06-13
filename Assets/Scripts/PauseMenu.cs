using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Config Paramters
    [SerializeField] GameObject pauseButton = null;
    [SerializeField] GameObject pauseScreen = null;
    [SerializeField] GameObject raycastBlocker = null;
    [SerializeField] GameObject instructionsPanel = null;
    [SerializeField] GameObject confirmationPanel = null;

    [SerializeField] Image difficultyBox = null;
    [SerializeField] Text difficultyText = null;
    [SerializeField] Color[] difficultyColors;

    // Cached References
    Player player = null;
    ParticleSpawner particleSpawner;
    GridWave gridWave = null;
    Instructions instructions = null;
    WaveRider[] waveRiders;

    // State variables
    bool isPaused = false;
    bool canPause = true;
    string confirmationType = "";

    Color difficultyColor;
    string difficultyName = "Normal";

    // Constants
    const string QUIT_GAME = "quit";
    const string RESTART_GAME = "restart";

    const string STORY_IDENTIFIER = "Story";
    const string EASY_IDENTIFIER = "Easy";
    const string NORMAL_IDENTIFIER = "Normal";
    const string HARD_IDENTIFIER = "Hard";

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        particleSpawner = FindObjectOfType<ParticleSpawner>();
        gridWave = FindObjectOfType<GridWave>();
        instructions = GetComponent<Instructions>();
        instructionsPanel.SetActive(false);
        confirmationPanel.SetActive(false);
        pauseScreen.SetActive(false);
    }

    void Update()
    {
        if (canPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    PauseGame();
                }
                else
                {
                    UnpauseGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        pauseButton.SetActive(false);

        AudioListener.pause = true;

        Time.timeScale = 0;

        difficultyBox.color = difficultyColor;
        difficultyText.text = difficultyName;

        pauseScreen.SetActive(true);

        if (!player) { player = FindObjectOfType<Player>(); }
        if (!particleSpawner) { particleSpawner = FindObjectOfType<ParticleSpawner>(); }

        player.AllowMovement(true);
        gridWave.AllowWaving(true);
        particleSpawner.AllowSpawning(true);

        waveRiders = FindObjectsOfType<WaveRider>();
        foreach (WaveRider waveRider in waveRiders)
        {
            waveRider.AllowRiding(false);
        }

        isPaused = true;
    }

    public void UnpauseGame()
    {
        foreach (WaveRider waveRider in waveRiders)
        {
            waveRider.AllowRiding(false);
        }

        pauseButton.SetActive(true);

        AudioListener.pause = false;

        Time.timeScale = 1;

        raycastBlocker.SetActive(false);
        instructionsPanel.SetActive(false);
        confirmationPanel.SetActive(false);
        pauseScreen.SetActive(false);

        player.AllowMovement(true);
        gridWave.AllowWaving(true);
        particleSpawner.AllowSpawning(true);

        isPaused = false;
    }

    public void OpenConfirmation(string type)
    {
        if (isPaused)
        {
            confirmationType = type;

            raycastBlocker.SetActive(true);
            confirmationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Pause menu is not activated.");
        }
    }

    public void CloseConfirmation()
    {
        if (isPaused)
        {
            raycastBlocker.SetActive(false);
            confirmationPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause menu is not activated.");
        }
    }

    public void ConfirmConfirmation()
    {
        if (isPaused)
        {
            raycastBlocker.SetActive(false);
            confirmationPanel.SetActive(false);
            instructionsPanel.SetActive(false);
            pauseScreen.SetActive(false);

            switch (confirmationType)
            {
                case QUIT_GAME:
                    AudioListener.pause = false;
                    Time.timeScale = 1;

                    SceneLoader.LoadMainMenu();

                    break;
                case RESTART_GAME:
                    AudioListener.pause = false;
                    Time.timeScale = 1;

                    SceneLoader.ReloadCurrentScene();

                    break;
                default:
                    Debug.LogError("Unknown quit option " + confirmationType);
                    break;
            }
        }
        else
        {
            Debug.LogError("Pause menu is not activated.");
        }
    }

    public void CanPause(bool isAllowed)
    {
        canPause = isAllowed;
    }

    public void SetDifficulty(int difficulty)
    {
        difficultyColor = difficultyColors[difficulty];        

        switch (difficulty)
        {
            case 0:
                difficultyName = STORY_IDENTIFIER;

                break;
            case 1:
                difficultyName = EASY_IDENTIFIER;
                break;
            case 2:
                difficultyName = NORMAL_IDENTIFIER;
                break;
            case 3:
                difficultyName = HARD_IDENTIFIER;
                break;
            default:
                Debug.LogError("Unknown difficulty " + difficulty + ".");
                break;
        }
    }

    public void OpenInstructions()
    {
        canPause = false;

        pauseScreen.SetActive(false);

        pauseButton.SetActive(true);
        GameManager.instance.SwitchRaycastBlocker(true);

        instructions.OpenInstructions(true);
    }

    public void CloseInstructions()
    {
        canPause = true;

        pauseButton.SetActive(false);
        GameManager.instance.SwitchRaycastBlocker(false);

        pauseScreen.SetActive(true);
    }

    public Color GetDifficultyColor()
    {
        return difficultyColor;
    }

    public string GetDifficultyName()
    {
        return difficultyName;
    }
}

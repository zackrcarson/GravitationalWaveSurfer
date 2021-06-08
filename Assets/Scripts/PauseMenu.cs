using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Config Paramters
    [SerializeField] GameObject pauseScreen = null;
    [SerializeField] GameObject raycastBlocker = null;
    [SerializeField] GameObject instructionsPanel = null;
    [SerializeField] GameObject confirmationPanel = null;

    // Cached References
    Player player = null;
    ParticleSpawner particleSpawner;
    GridWave gridWave = null;
    WaveRider[] waveRiders;

    // State variables
    bool isPaused = false;
    bool canPause = true;
    string confirmationType = "";

    // Constants
    const string QUIT_GAME = "quit";
    const string RESTART_GAME = "restart";

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        particleSpawner = FindObjectOfType<ParticleSpawner>();
        gridWave = FindObjectOfType<GridWave>();

        raycastBlocker.SetActive(false);
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
        AudioListener.pause = true;

        Time.timeScale = 0;

        pauseScreen.SetActive(true);

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

    public void OpenInstructions()
    {
        if (isPaused)
        {
            raycastBlocker.SetActive(true);
            instructionsPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Pause menu is not activated.");
        }
    }

    public void CloseInstructions()
    {
        if (isPaused)
        {
            raycastBlocker.SetActive(false);
            instructionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause menu is not activated.");
        }
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
}

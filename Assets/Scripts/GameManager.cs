using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Config Paramters
    [Header("Initial Numbers")]
    [SerializeField] int numProtons = 1;
    [SerializeField] int numNeutrons = 0;
    [SerializeField] int numElectrons = 1;

    [Header("Text Boxes")]
    [SerializeField] Text protonsText = null;
    [SerializeField] Text neutronsText = null;
    [SerializeField] Text electronsText = null;

    [SerializeField] Text elementText = null;
    [SerializeField] Text massNumberText = null;
    [SerializeField] Text AtomicNumberText = null;
    [SerializeField] Text IonicNumberText = null;

    [SerializeField] Text difficultyDescriptionText = null;

    [Header("GameObjects")]
    [SerializeField] GameObject pauseButton = null;
    [SerializeField] GameObject stabilityBar = null;
    [SerializeField] GameObject blackHoleDisplay = null;
    [SerializeField] GameObject goalDisplay = null;
    [SerializeField] GameObject scoreDisplay = null;
    [SerializeField] GameObject atomDisplay = null;
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject numGoalsDisplay = null;

    [SerializeField] GameObject difficultySelector = null;
    [SerializeField] GameObject[] difficultyButtons = null;

    [SerializeField] GameObject rayCastBlocker = null;

    [Header("Data")]
    [SerializeField] string elementsData = "Assets/Data/elements.csv";

    // State Variables
    public int difficulty = 2;
    bool hasReachedEnd = false;

    // Cached References
    Dictionary<int, string> elementsDict = null;
    Dictionary<int, string> elementsFullDict = null;

    Goals goals = null;
    Stability stability = null;
    Instructions instructions = null;

    int[] currentParticles = null;
    public int maxProtons = 118;

    // Constants
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";
    const string ANTI_PREFIX = "Anti-";

    const string STORY_IDENTIFIER = "story";
    const string EASY_IDENTIFIER = "easy";
    const string NORMAL_IDENTIFIER = "normal";
    const string HARD_IDENTIFIER = "hard";

    const string GAME_WON_NAME = "won";

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        instructions = GetComponent<Instructions>();
        rayCastBlocker.SetActive(false);

        if (!FindObjectOfType<DifficultyHolder>())
        {
            Time.timeScale = 0;
            AudioListener.pause = false;
            GetComponent<PauseMenu>().CanPause(false);

            pauseButton.SetActive(false);
            stabilityBar.SetActive(false);
            blackHoleDisplay.SetActive(false);
            goalDisplay.SetActive(false);
            scoreDisplay.SetActive(false);
            atomDisplay.SetActive(false);
            numGoalsDisplay.SetActive(false);
            player.SetActive(false);
            
            difficultySelector.SetActive(true);
        }
        else
        {
            difficulty = FindObjectOfType<DifficultyHolder>().difficulty;
            Destroy(FindObjectOfType<DifficultyHolder>().gameObject);

            Time.timeScale = 0;
            AudioListener.pause = false;

            GetComponent<PauseMenu>().CanPause(false);

            StartCoroutine(DelayedOpenInstructions());
        }
    }

    private IEnumerator DelayedOpenInstructions()
    {
        yield return null;

        OpenInstructions();
    }

    public void OpenInstructions()
    {
        difficultySelector.SetActive(false);

        pauseButton.SetActive(true);
        SwitchRaycastBlocker(true);

        stabilityBar.SetActive(true);
        blackHoleDisplay.SetActive(true);
        goalDisplay.SetActive(true);
        scoreDisplay.SetActive(true);
        atomDisplay.SetActive(true);
        player.SetActive(true);
        numGoalsDisplay.SetActive(true);

        currentParticles = new int[] { numProtons, numNeutrons, numElectrons };

        stability = GetComponent<Stability>();
        goals = GetComponent<Goals>();

        CollectElements();

        ShowScore();

        goals.ExternalStart();
        stability.ExternalStart();
        FindObjectOfType<ParticleSpawner>().ExternalStart();
        FindObjectOfType<MicroBlackHole>().ExternalStart();

        instructions.OpenInstructions(false);
    }

    public void CloseInstructions()
    {
        Time.timeScale = 1;
        AudioListener.pause = true;

        SwitchRaycastBlocker(true);

        GetComponent<PauseMenu>().SetDifficulty(difficulty);
        GetComponent<PauseMenu>().CanPause(true);
    }

    public void ButtonSelect(string option)
    {
        switch (option)
        {
            case STORY_IDENTIFIER:
                difficultyDescriptionText.text = "In story mode, no anti-particles will spawn, your atoms will remain ultra-stable, and your only goal will be to forge the heaviest element known. Choose this mode if you do not want any challenge and would prefer to focus on creating fun and unique atoms!";
                difficulty = 0;

                SetOutline(difficulty);
                break;

            case EASY_IDENTIFIER:
                difficultyDescriptionText.text = "In easy mode, only a small number of anti-particles will spawn, your atoms will remain fairly stable, and the goals will be very easy to reach. Choose this mode if you want only a small challenge!";
                difficulty = 1;

                SetOutline(difficulty);
                break;

            case NORMAL_IDENTIFIER:
                difficultyDescriptionText.text = "In normal mode, a decent number of anti-particles will spawn, your atoms will decay at a standard physical rate, and the goals will be more difficult to reach. Choose this mode if you want a balanced gameplay with some difficulty!";
                difficulty = 2;

                SetOutline(difficulty);
                break;

            case HARD_IDENTIFIER:
                difficultyDescriptionText.text = "In hard mode, a lot of anti-particles will spawn, your atoms will become unstable very easily, and the goals will be extremely difficult to reach. Only choose this mode if you want an extreme challenge!";
                difficulty = 3;

                SetOutline(difficulty);
                break;

            default:
                Debug.LogError("No valid button ( " + option + " ) found.");
                break;
        }
    }

    private void SetOutline(int numberOn)
    {
        int i = 0;
        foreach (GameObject button in difficultyButtons)
        {
            button.GetComponent<Outline>().enabled = (i == numberOn);

            i++;
        }
    }

    private void CollectElements()
    {
        elementsDict = new Dictionary<int, string>();
        elementsFullDict = new Dictionary<int, string>();

        StreamReader strReader = new StreamReader(elementsData);
        bool endOfFile = false;


        while (!endOfFile)
        {
            string dataString = strReader.ReadLine();

            if (dataString == null)
            {
                endOfFile = true;
                break;
            }

            string[] dataValues = dataString.Split(',');
            maxProtons = int.Parse(dataValues[0]);

            elementsDict.Add(int.Parse(dataValues[0]), dataValues[2]);
            elementsFullDict.Add(int.Parse(dataValues[0]), dataValues[1]);
        }
    }

    public bool IsValidElement(int protons)
    {
        return elementsDict.ContainsKey(protons);
    }

    private void ShowScore()
    {
        protonsText.text = numProtons.ToString();
        neutronsText.text = numNeutrons.ToString();
        electronsText.text = numElectrons.ToString();

        if (elementsDict.ContainsKey(numProtons))
        {
            elementText.text = elementsDict[numProtons];
        }
        else
        {
            elementText.text = "??";
            goals.StopGoals();

            if (!hasReachedEnd)
            {
                GetComponent<GameOver>().StartGameOver(GAME_WON_NAME);
                hasReachedEnd = true;
            }
        }

        massNumberText.text = (numProtons + numNeutrons).ToString();
        AtomicNumberText.text = numProtons.ToString();

        if (numElectrons == numProtons)
        {
            IonicNumberText.gameObject.SetActive(false);
        }
        else
        {
            IonicNumberText.gameObject.SetActive(true);

            if (numElectrons > numProtons)
            {
                IonicNumberText.text = "+" + (numElectrons - numProtons).ToString();
            }
            else if (numElectrons < numProtons)
            {
                IonicNumberText.text = "-" + (numProtons - numElectrons).ToString();
            }
        }
    }

    public void AddParticles(List<string> types)
    {
        foreach (string type in types)
        {
            AddParticle(type, false);

            goals.CheckGoal(currentParticles);
        }

        stability.updateStability(currentParticles);
        ShowScore();
    }

    public void AddParticle(string type, bool singleParticle)
    {
        if (type == PROTON_NAME)
        {
            numProtons++;
        }
        else if (type == NEUTRON_NAME)
        {
            numNeutrons++;
        }
        else if (type == ELECTRON_NAME)
        {
            numElectrons++;
        }
        else if (type.StartsWith(ANTI_PREFIX))
        {
            return;
        }
        else
        {
            Debug.LogError("Unknown particle (" + type + ") not added!");
        }

        currentParticles[0] = numProtons;
        currentParticles[1] = numNeutrons;
        currentParticles[2] = numElectrons;

        if (singleParticle)
        {
            stability.updateStability(currentParticles);
            ShowScore();
        }
    }

    public void RemoveParticle(string type)
    {
        if (type == PROTON_NAME)
        {
            numProtons--;
        }
        else if (type == NEUTRON_NAME)
        {
            numNeutrons--;
        }
        else if (type == ELECTRON_NAME)
        {
            numElectrons--;
        }
        else if (type.StartsWith(ANTI_PREFIX))
        {
            return;
        }
        else
        {
            Debug.LogError("Unknown particle (" + type + ") not removed!");
        }

        currentParticles[0] = numProtons;
        currentParticles[1] = numNeutrons;
        currentParticles[2] = numElectrons;

        ShowScore();
        goals.CheckGoal(currentParticles);
        stability.updateStability(currentParticles);
    }

    /// <summary>
    /// Returns the final numbers of particles.
    /// </summary>
    /// <returns>Returns an int list in the order of numProtons, numNeutrons, numElectrons.</returns>
    public int[] GetScore()
    {
        return new int[] { numProtons, numNeutrons, numElectrons };
    }

    /// <summary>
    /// Returns the final element names.
    /// </summary>
    /// <param name="atomicNumber"></param>
    /// <returns>Returns a string list in the order of shorthand element name, full element name.</returns>
    public string[] GetElementName(int atomicNumber)
    {
        return new string[] { elementsDict[atomicNumber], elementsFullDict[atomicNumber] };
    }

    public void SwitchRaycastBlocker(bool isOn)
    {
        rayCastBlocker.SetActive(isOn);
    }
}

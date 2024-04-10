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
    private List<string> elementsData = new List<string> {"1,Hydrogen,H",
                                            "2,Helium,He",
                                            "3,Lithium,Li",
                                            "4,Beryllium,Be",
                                            "5,Boron,B",
                                            "6,Carbon,C",
                                            "7,Nitrogen,N",
                                            "8,Oxygen,O",
                                            "9,Fluorine,F",
                                            "10,Neon,Ne",
                                            "11,Sodium,Na",
                                            "12,Magnesium,Mg",
                                            "13,Aluminum,Al",
                                            "14,Silicon,Si",
                                            "15,Phosphorus,P",
                                            "16,Sulfur,S",
                                            "17,Chlorine,Cl",
                                            "18,Argon,Ar",
                                            "19,Potassium,K",
                                            "20,Calcium,Ca",
                                            "21,Scandium,Sc",
                                            "22,Titanium,Ti",
                                            "23,Vanadium,V",
                                            "24,Chromium,Cr",
                                            "25,Manganese,Mn",
                                            "26,Iron,Fe",
                                            "27,Cobalt,Co",
                                            "28,Nickel,Ni",
                                            "29,Copper,Cu",
                                            "30,Zinc,Zn",
                                            "31,Gallium,Ga",
                                            "32,Germanium,Ge",
                                            "33,Arsenic,As",
                                            "34,Selenium,Se",
                                            "35,Bromine,Br",
                                            "36,Krypton,Kr",
                                            "37,Rubidium,Rb",
                                            "38,Strontium,Sr",
                                            "39,Yttrium,Y",
                                            "40,Zirconium,Zr",
                                            "41,Niobium,Nb",
                                            "42,Molybdenum,Mo",
                                            "43,Technetium,Tc",
                                            "44,Ruthenium,Ru",
                                            "45,Rhodium,Rh",
                                            "46,Palladium,Pd",
                                            "47,Silver,Ag",
                                            "48,Cadmium,Cd",
                                            "49,Indium,In",
                                            "50,Tin,Sn",
                                            "51,Antimony,Sb",
                                            "52,Tellurium,Te",
                                            "53,Iodine,I",
                                            "54,Xenon,Xe",
                                            "55,Cesium,Cs",
                                            "56,Barium,Ba",
                                            "57,Lanthanum,La",
                                            "58,Cerium,Ce",
                                            "59,Praseodymium,Pr",
                                            "60,Neodymium,Nd",
                                            "61,Promethium,Pm",
                                            "62,Samarium,Sm",
                                            "63,Europium,Eu",
                                            "64,Gadolinium,Gd",
                                            "65,Terbium,Tb",
                                            "66,Dysprosium,Dy",
                                            "67,Holmium,Ho",
                                            "68,Erbium,Er",
                                            "69,Thulium,Tm",
                                            "70,Ytterbium,Yb",
                                            "71,Lutetium,Lu",
                                            "72,Hafnium,Hf",
                                            "73,Tantalum,Ta",
                                            "74,Wolfram,W",
                                            "75,Rhenium,Re",
                                            "76,Osmium,Os",
                                            "77,Iridium,Ir",
                                            "78,Platinum,Pt",
                                            "79,Gold,Au",
                                            "80,Mercury,Hg",
                                            "81,Thallium,Tl",
                                            "82,Lead,Pb",
                                            "83,Bismuth,Bi",
                                            "84,Polonium,Po",
                                            "85,Astatine,At",
                                            "86,Radon,Rn",
                                            "87,Francium,Fr",
                                            "88,Radium,Ra",
                                            "89,Actinium,Ac",
                                            "90,Thorium,Th",
                                            "91,Protactinium,Pa",
                                            "92,Uranium,U",
                                            "93,Neptunium,Np",
                                            "94,Plutonium,Pu",
                                            "95,Americium,Am",
                                            "96,Curium,Cm",
                                            "97,Berkelium,Bk",
                                            "98,Californium,Cf",
                                            "99,Einsteinium,Es",
                                            "100,Fermium,Fm",
                                            "101,Mendelevium,Md",
                                            "102,Nobelium,No",
                                            "103,Lawrencium,Lr",
                                            "104,Rutherfordium,Rf",
                                            "105,Dubnium,Db",
                                            "106,Seaborgium,Sg",
                                            "107,Bohrium,Bh",
                                            "108,Hassium,Hs",
                                            "109,Meitnerium,Mt",
                                            "110,Darmstadtium,Ds",
                                            "111,Roentgenium,Rg",
                                            "112,Copernicium,Cn",
                                            "113,Nihonium,Nh",
                                            "114,Flerovium,Fl",
                                            "115,Moscovium,Mc",
                                            "116,Livermorium,Lv",
                                            "117,Tennessine,Ts",
                                            "118,Oganesson,Og"
    };

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

    // TODO: This script pauses/unpauses the audioListener for some reason. Come back if needed :)
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
            //AudioListener.pause = true;
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
            //AudioListener.pause = false;

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
        //AudioListener.pause = true;

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

        foreach(string elementData in elementsData)
        {
            string[] dataValues = elementData.Split(',');
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

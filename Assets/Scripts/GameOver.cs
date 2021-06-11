using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    // Config Parameters
    [Header("Text Boxes")]
    [SerializeField] Text elementText = null;
    [SerializeField] Text massNumberText = null;
    [SerializeField] Text AtomicNumberText = null;
    [SerializeField] Text IonicNumberText = null;

    [SerializeField] Text subHeaderText = null;
    [SerializeField] Text atomicDescriptionText = null;
    [SerializeField] Text annihilationDescriptionText = null;

    [SerializeField] Text difficultyName = null;
    [SerializeField] Image difficultyBox = null;

    [Header("Game Objects")]
    [SerializeField] GameObject gameOverScreen = null;
    [SerializeField] GameObject scoreBox = null;
    [SerializeField] GameObject finalForm = null;
    [SerializeField] GameObject pauseButton = null;
    [SerializeField] GameObject blackHoleInformation = null;
    [SerializeField] GameObject goalBox = null;

    // Cached References
    PauseMenu pauseMenu = null;
    ParticleSpawner particleSpawner = null;

    // Constants
    const string INSTABILITY_NAME = "instability";
    const string ELECTRON_NAME = "electron";
    const string POSITRON_NAME = "positron";
    const string ANTI_PREFIX = "anti-";
    static readonly string[] VOWEL_SOUNDS = { "a", "e", "i", "o", "u" };
    static readonly string[] NOT_VOWEL_SOUNDS = { "eu" , "ur" };

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
        particleSpawner = FindObjectOfType<ParticleSpawner>();
    }

    public void StartGameOver(string annihilatedParticle)
    {
        if (!pauseMenu) { pauseMenu = FindObjectOfType<PauseMenu>(); }
        if (!particleSpawner) { particleSpawner = FindObjectOfType<ParticleSpawner>(); }

        pauseMenu.CanPause(false);
        particleSpawner.AllowSpawning(true);

        scoreBox.SetActive(false);
        finalForm.SetActive(false);
        pauseButton.SetActive(false);
        blackHoleInformation.SetActive(false);
        goalBox.SetActive(false);

        int[] particles = GameManager.instance.GetScore();
        string[] elements = GameManager.instance.GetElementName(particles[0]);

        ShowResults(particles[0], particles[1], particles[2], elements[0], elements[1], annihilatedParticle);

        gameOverScreen.SetActive(true);
    }

    private void ShowResults(int numProtons, int numNeutrons, int numElectrons, string elementShorthand, string elementName, string annihilatedParticle)
    {
        if (annihilatedParticle == INSTABILITY_NAME)
        {
            subHeaderText.text = "Your atom became unstable and decayed to nothing!";

            int surplusNeutrons = numNeutrons - numProtons;
            int surplusElectrons = numElectrons - numProtons;
            string singularNeutrons = "";
            string singularElectrons = "";
            string gainLoseWordNeutrons = "accumulated ";
            string gainLoseWordElectrons = "accumulated ";

            if (Mathf.Abs(surplusNeutrons) > 1)
            {
                singularNeutrons = "s";
            }
            if (Mathf.Abs(surplusElectrons) > 1)
            {
                singularElectrons = "s";
            }

            if (surplusNeutrons < 0)
            {
                gainLoseWordNeutrons = "lost ";
            }
            if (surplusElectrons < 0)
            {
                gainLoseWordElectrons = "lost ";
            }

            if (gainLoseWordNeutrons == gainLoseWordElectrons && surplusNeutrons != 0)
            {
                gainLoseWordElectrons = "";
            }

            if (surplusNeutrons != 0 && surplusElectrons != 0)
            {
                annihilationDescriptionText.text = "Your atom reached instability and decayed when it " + gainLoseWordNeutrons + Mathf.Abs(surplusNeutrons) + " extra neutron" + singularNeutrons + " and " + gainLoseWordElectrons + Mathf.Abs(surplusElectrons) + " extra electron" + singularElectrons + "!";
            }
            else if (surplusNeutrons != 0 && surplusElectrons == 0)
            {
                annihilationDescriptionText.text = "Your atom reached instability and decayed when it " + gainLoseWordNeutrons + Mathf.Abs(surplusNeutrons) + " extra neutron" + singularNeutrons + "!";
            }
            else if (surplusNeutrons == 0 && surplusElectrons != 0)
            {
                annihilationDescriptionText.text = "Your atom reached instability and decayed when it " + gainLoseWordElectrons + Mathf.Abs(surplusElectrons) + " extra electron" + singularElectrons + "!";
            }
        }
        else
        {
            subHeaderText.text = "You were annihilated riding the gravitational waves!";

            if (annihilatedParticle == ELECTRON_NAME)
            {
                annihilationDescriptionText.text = "Your atom was annihilated when one of your " + ELECTRON_NAME.ToLower() + "s collided with a " + POSITRON_NAME.ToLower() + ".";
            }
            else
            {
                annihilationDescriptionText.text = "Your atom was annihilated when one of your " + annihilatedParticle.ToLower() + "s collided with an " + ANTI_PREFIX.ToLower() + annihilatedParticle.ToLower() + ".";
            }
        }

        elementText.text = elementShorthand;
        massNumberText.text = (numProtons + numNeutrons).ToString();
        AtomicNumberText.text = numProtons.ToString();

        if (!pauseMenu) { pauseMenu = GetComponent<PauseMenu>(); }

        difficultyName.text = pauseMenu.GetDifficultyName();
        difficultyBox.color = pauseMenu.GetDifficultyColor();

        string indefiniteArticle = FindIndefiniteArticle(elementName);
        string protonPlural = "s";
        string neutronPlural = "s";
        string electronPlural = "s";

        if (numProtons == 1) { protonPlural = ""; }
        if (numNeutrons == 1) { neutronPlural = ""; }
        if (numElectrons == 1) { electronPlural = ""; }

        atomicDescriptionText.text = "That is, you created " + indefiniteArticle + " " + elementName + " isotope with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";

        bool extraCharge = false;
        bool extraNeutrons = false;

        if (numElectrons == numProtons)
        {
            IonicNumberText.gameObject.SetActive(false);
        }
        else
        {
            extraCharge = true;

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

        if (numNeutrons != numProtons)
        {
            extraNeutrons = true;
        }


        if (extraCharge && extraNeutrons)
        {
            string extraNeutronsPlural = "s";
            if (Mathf.Abs(numNeutrons - numProtons) == 1) 
            { 
                extraNeutronsPlural = ""; 
            }

            string gainLose = "gained";
            string neutronQuantifier = " extra";
            string electronGainLose = "";
            if (numNeutrons < numProtons)
            {
                gainLose = "lost";
                neutronQuantifier = "";
                electronGainLose = " gained";
            }

            atomicDescriptionText.text += " That means you " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " and" + electronGainLose + " a net electric charge of ";
            if (numElectrons > numProtons)
            {
                atomicDescriptionText.text += "+" + (numElectrons - numProtons) + "e.";
            }
            else if (numElectrons < numProtons)
            {
                atomicDescriptionText.text += "-" + (numProtons - numElectrons) + "e.";
            }
        }
        else if (extraCharge && !extraNeutrons)
        {
            atomicDescriptionText.text += " That means you gained a net electric charge of ";
            if (numElectrons > numProtons)
            {
                atomicDescriptionText.text += "+" + (numElectrons - numProtons) + "e.";
            }
            else if (numElectrons < numProtons)
            {
                atomicDescriptionText.text += "-" + (numProtons - numElectrons) + "e.";
            }
        }
        else if (!extraCharge && extraNeutrons)
        {
            string extraNeutronsPlural = "s";
            if (Mathf.Abs(numNeutrons - numProtons) == 1) 
            { 
                extraNeutronsPlural = ""; 
            }

            string gainLose = "gained";
            string neutronQuantifier = " extra";
            if (numNeutrons < numProtons)
            {
                gainLose = "lost";
                neutronQuantifier = "";
            }

            atomicDescriptionText.text += " That means you " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + ".";
        }
        else
        {
            atomicDescriptionText.text = "That is, you created a perfectly balanced " + elementName + " atom with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
        }
    }

    private string FindIndefiniteArticle(string elementName)
    {
        string indefiniteArticle = "a";

        foreach (string vowel in VOWEL_SOUNDS)
        {
            if (elementName.ToLower().StartsWith(vowel))
            {
                indefiniteArticle = "an";
                break;
            }
        }
        foreach (string consonant in NOT_VOWEL_SOUNDS)
        {
            if (elementName.ToLower().StartsWith(consonant))
            {
                indefiniteArticle = "a";
                break;
            }
        }

        return indefiniteArticle;
    }

    public void ResetGame()
    {
        SceneLoader.ReloadCurrentScene();
    }

    public void QuitGame()
    {
        SceneLoader.LoadMainMenu();
    }
}

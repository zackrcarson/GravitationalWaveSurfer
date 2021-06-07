using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] Text atomicDescriptionText = null;
    [SerializeField] Text annihilationDescriptionText = null;

    [Header("Game Objects")]
    [SerializeField] GameObject gameOverScreen = null;
    [SerializeField] GameObject scoreBox = null;
    [SerializeField] GameObject finalForm = null;
    [SerializeField] GameObject pauseButton = null;
    [SerializeField] GameObject blackHoleInformation = null;

    // Cached References
    Player player = null;
    PauseMenu pauseMenu = null;
    ParticleSpawner particleSpawner = null;

    // Constants
    const string ELECTRON_NAME = "electron";
    const string POSITRON_NAME = "positron";
    const string ANTI_PREFIX = "anti-";
    static readonly string[] VOWEL_SOUNDS = { "a", "e", "i", "o", "u" };
    static readonly string[] NOT_VOWEL_SOUNDS = { "eu" , "ur" };

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        particleSpawner = FindObjectOfType<ParticleSpawner>();
    }

    public void StartGameOver(string annihilatedParticle)
    {
        pauseMenu.CanPause(false);
        particleSpawner.AllowSpawning(true);

        scoreBox.SetActive(false);
        finalForm.SetActive(false);
        pauseButton.SetActive(false);
        blackHoleInformation.SetActive(false);

        int[] particles = GameManager.instance.GetScore();
        string[] elements = GameManager.instance.GetElementName(particles[0]);

        ShowResults(particles[0], particles[1], particles[2], elements[0], elements[1], annihilatedParticle);

        gameOverScreen.SetActive(true);
    }

    private void ShowResults(int numProtons, int numNeutrons, int numElectrons, string elementShorthand, string elementName, string annihilatedParticle)
    {
        elementText.text = elementShorthand;
        massNumberText.text = (numProtons + numNeutrons).ToString();
        AtomicNumberText.text = numProtons.ToString();

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


        if (annihilatedParticle == ELECTRON_NAME)
        {
            annihilationDescriptionText.text = "Your atom was annihilated when one of your " + ELECTRON_NAME.ToLower() + "s collided with a " + POSITRON_NAME.ToLower() + ".";
        }
        else
        {
            annihilationDescriptionText.text = "Your atom was annihilated when one of your " + annihilatedParticle.ToLower() + "s collided with an " + ANTI_PREFIX.ToLower() + annihilatedParticle.ToLower() + ".";
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
}

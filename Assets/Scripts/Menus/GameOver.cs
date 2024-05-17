using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameOver : MonoBehaviour
{
    // Config Parameters
    [Header("Control Parameters")]
    [SerializeField] float fadeOutRate = 2.0f;
    [SerializeField] float fadeInRate = 2.0f;

    [Header("Text Boxes")]
    [Header("Game Over Menu")]
    [SerializeField] Text elementText = null;
    [SerializeField] Text massNumberText = null;
    [SerializeField] Text AtomicNumberText = null;
    [SerializeField] Text IonicNumberText = null;
    [SerializeField] Text scoreText = null;

    [SerializeField] Text subHeaderText = null;
    [SerializeField] Text atomicDescriptionText = null;
    [SerializeField] Text scoreDescriptionText = null;
    [SerializeField] Text annihilationDescriptionText = null;

    [SerializeField] Text difficultyName = null;
    [SerializeField] Image difficultyBox = null;

    [Header("Game Won Menu")]
    [SerializeField] Text elementTextWon = null;
    [SerializeField] Text massNumberTextWon = null;
    [SerializeField] Text AtomicNumberTextWon = null;
    [SerializeField] Text IonicNumberTextWon = null;
    [SerializeField] Text scoreTextWon = null;

    [SerializeField] Text atomicDescriptionTextWon = null;
    [SerializeField] Text scoreDescriptionTextWon = null;

    [SerializeField] Text difficultyNameWon = null;
    [SerializeField] Image difficultyBoxWon = null;

    [Header("Game Objects")]
    [SerializeField] GameObject stabilityBar = null;
    [SerializeField] GameObject gameOverScreen = null;
    [SerializeField] GameObject gameWonScreen = null;
    [SerializeField] GameObject scoreBox = null;
    [SerializeField] GameObject finalForm = null;
    [SerializeField] GameObject pauseButton = null;
    [SerializeField] GameObject blackHoleInformation = null;
    [SerializeField] GameObject goalBox = null;
    [SerializeField] GameObject pointsBox = null;
    [SerializeField] CanvasGroup fadeOutCanvasGroup = null;
    [SerializeField] GameObject mBHArrow = null;
    [SerializeField] CanvasGroup fadeInCanvasGroup = null;
    // [SerializeField] GameObject raycastBlocker = null;

    // Cached References
    PauseMenu pauseMenu = null;
    ParticleSpawner particleSpawner = null;
    Player player = null;

    // Constants
    const string INSTABILITY_NAME = "instability";
    const string GAME_WON_NAME = "won";
    const string ELECTRON_NAME = "electron";
    const string POSITRON_NAME = "positron";
    const string ANTI_PREFIX = "anti-";
    const string BLACK_HOLE_NAME = "Black Hole";
    static readonly string[] VOWEL_SOUNDS = { "a", "e", "i", "o", "u" };
    static readonly string[] NOT_VOWEL_SOUNDS = { "eu" , "ur" };

    // State Variables
    public bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;

        pauseMenu = FindObjectOfType<PauseMenu>();
        particleSpawner = FindObjectOfType<ParticleSpawner>();
        player = FindObjectOfType<Player>();
    }

    public void StartGameOver(string annihilatedParticle)
    {
        StartCoroutine(GameOverCoroutine(annihilatedParticle));
    }

    public IEnumerator GameOverCoroutine(string annihilatedParticle)
    {
        //raycastBlocker.SetActive(true);

        if (!pauseMenu) { pauseMenu = FindObjectOfType<PauseMenu>(); }
        if (!particleSpawner) { particleSpawner = FindObjectOfType<ParticleSpawner>(); }

        isGameOver = true;
        pauseMenu.CanPause(false);
        particleSpawner.AllowSpawning(true);

        FindObjectOfType<MicroBlackHole>().isGameOver = true;

        int[] particles = GameManager.instance.GetScore();

        string[] elements = new string[] { "", "" };
        if (annihilatedParticle != GAME_WON_NAME)
        {
            if (GameManager.instance.IsValidElement(particles[0]))
            {
                elements = GameManager.instance.GetElementName(particles[0]);
            }
            else
            {
                elements[0] = "??";
                elements[1] = "??";
            }
        }
        else
        {
            elements = new string[] { "??", "??" };

            if (!player) { player = FindObjectOfType<Player>(); }

            FindObjectOfType<GridWave>().StopWaving();
            player.AllowMovement(false);
            Time.timeScale = 0;
        }

        ShowResults(particles[0], particles[1], particles[2], elements[0], elements[1], annihilatedParticle);

        fadeInCanvasGroup.alpha = 0.0f;

        if (annihilatedParticle == GAME_WON_NAME)
        {
            gameWonScreen.SetActive(true);
        }
        else
        {
            gameOverScreen.SetActive(true);
        }
        
        float currentOutAlpha = fadeOutCanvasGroup.alpha;
        float currentInAlpha = fadeInCanvasGroup.alpha;

        float targetOutAlpha = 0.0f;
        float targetInAlpha = 1.0f;

        while (Mathf.Abs(currentOutAlpha - targetOutAlpha) > 0.02f)
        {
            currentOutAlpha = Mathf.Lerp(currentOutAlpha, targetOutAlpha, fadeOutRate * Time.deltaTime);
            fadeOutCanvasGroup.alpha = currentOutAlpha;

            yield return null;
        }

        mBHArrow.SetActive(false);
        stabilityBar.SetActive(false);
        scoreBox.SetActive(false);
        finalForm.SetActive(false);
        pauseButton.SetActive(false);
        blackHoleInformation.SetActive(false);
        goalBox.SetActive(false);
        pointsBox.SetActive(false);

        while (Mathf.Abs(currentInAlpha - targetInAlpha) > 0.25f)
        {
            currentInAlpha = Mathf.Lerp(currentInAlpha, targetInAlpha, fadeInRate * Time.deltaTime);
            fadeInCanvasGroup.alpha = currentInAlpha;

            yield return null;
        }

        gameOverScreen.transform.SetParent(fadeInCanvasGroup.transform.parent, true);
    }

    private void ShowResults(int numProtons, int numNeutrons, int numElectrons, string elementShorthand, string elementName, string annihilatedParticle)
    {
        if (annihilatedParticle == BLACK_HOLE_NAME)
        {
            subHeaderText.text = "Your atom plunged into the event horizon of a stray micro black hole!";

            annihilationDescriptionText.text = "Inside the black hole's event horizon, the extreme curvature swapped your space and time dimensions; forcing you inwards as the extreme tidal gravitational field immediately shredded your atom into elementary particles.";
        }
        else if (annihilatedParticle == INSTABILITY_NAME)
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
                annihilationDescriptionText.text = "Your isotope reached instability and decayed when it " + gainLoseWordNeutrons + Mathf.Abs(surplusNeutrons) + " extra neutron" + singularNeutrons + " and " + gainLoseWordElectrons + Mathf.Abs(surplusElectrons) + " extra electron" + singularElectrons + "!";
            }
            else if (surplusNeutrons != 0 && surplusElectrons == 0)
            {
                annihilationDescriptionText.text = "Your isotope reached instability and decayed when it " + gainLoseWordNeutrons + Mathf.Abs(surplusNeutrons) + " extra neutron" + singularNeutrons + "!";
            }
            else if (surplusNeutrons == 0 && surplusElectrons != 0)
            {
                annihilationDescriptionText.text = "Your ion reached instability and decayed when it " + gainLoseWordElectrons + Mathf.Abs(surplusElectrons) + " extra electron" + singularElectrons + "!";
            }
        }
        else if (annihilatedParticle != GAME_WON_NAME)
        {
            subHeaderText.text = "You were annihilated riding the gravitational waves!";

            if (annihilatedParticle == ELECTRON_NAME)
            {
                annihilationDescriptionText.text = "Your atom was annihilated when your last " + ELECTRON_NAME.ToLower() + " collided with a " + POSITRON_NAME.ToLower() + ", and your atom become unstable and decayed.";
            }
            else
            {
                annihilationDescriptionText.text = "Your atom was annihilated when your last " + annihilatedParticle.ToLower() + " collided with an " + ANTI_PREFIX.ToLower() + annihilatedParticle.ToLower() + ", and your atom become unstable and decayed.";
            }
        }


        if (!pauseMenu) { pauseMenu = GetComponent<PauseMenu>(); }

        string indefiniteArticle = FindIndefiniteArticle(elementName);
        string protonPlural = "s";
        string neutronPlural = "s";
        string electronPlural = "s";

        if (numProtons == 1) { protonPlural = ""; }
        if (numNeutrons == 1) { neutronPlural = ""; }
        if (numElectrons == 1) { electronPlural = ""; }

        int[] score = GetComponent<Goals>().GetScore();

        if (annihilatedParticle != GAME_WON_NAME)
        {
            elementText.text = elementShorthand;
            massNumberText.text = (numProtons + numNeutrons).ToString();
            AtomicNumberText.text = numProtons.ToString();

            difficultyName.text = pauseMenu.GetDifficultyName();
            difficultyBox.color = pauseMenu.GetDifficultyColor();

            scoreText.text = score[0].ToString();
            scoreDescriptionText.text = "You achieved " + score[1] + " goals, and missed " + score[2] + " goals, making your final score " + score[0] + "!";

            if (elementName == "??")
            {
                atomicDescriptionText.text = "That is, you created an new unknown isotope with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
            }
            else
            {
                atomicDescriptionText.text = "That is, you created " + indefiniteArticle + " " + elementName + " isotope with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
            }
        }
        else
        {
            elementTextWon.text = elementShorthand;
            massNumberTextWon.text = (numProtons + numNeutrons).ToString();
            AtomicNumberTextWon.text = numProtons.ToString();

            difficultyNameWon.text = pauseMenu.GetDifficultyName();
            difficultyBoxWon.color = pauseMenu.GetDifficultyColor();

            scoreTextWon.text = score[0].ToString();
            scoreDescriptionTextWon.text = "You achieved " + score[1] + " goals, and missed " + score[2] + " goals, making your final score " + score[0] + "!";

            atomicDescriptionTextWon.text = "You created an unknown isotope with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
        }

        bool extraCharge = false;
        bool extraNeutrons = false;

        if (numElectrons == numProtons)
        {
            if (annihilatedParticle != GAME_WON_NAME)
            {
                IonicNumberText.gameObject.SetActive(false);
            }
            else
            {
                IonicNumberTextWon.gameObject.SetActive(false);
            }
        }
        else
        {
            if (annihilatedParticle != GAME_WON_NAME)
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
            else
            {
                extraCharge = true;

                IonicNumberTextWon.gameObject.SetActive(true);

                if (numElectrons > numProtons)
                {
                    IonicNumberTextWon.text = "+" + (numElectrons - numProtons).ToString();
                }
                else if (numElectrons < numProtons)
                {
                    IonicNumberTextWon.text = "-" + (numProtons - numElectrons).ToString();
                }
            }
        }

        if (numNeutrons != numProtons)
        {
            extraNeutrons = true;
        }

        if (numProtons == 1)
        {
            if (numElectrons == 1 && numNeutrons == 0)
            {
                atomicDescriptionText.text = "That is, you created a perfectly balanced Hydrogen atom with 1 proton and 1 electron.";
            }
            else if (numElectrons > 1 && numNeutrons == 0)
            {
                atomicDescriptionText.text = "That is, you created a Hydrogen ion with 1 proton and " + numElectrons + " electron" + electronPlural + ". That means you gained a net electric charge of +" + (numElectrons - 1) + "e from its stable state.";
            }
            else if (numElectrons == 1 && numNeutrons > 0)
            {
                atomicDescriptionText.text = "That is, you created a Hydrogen isotope with 1 proton, 1 electron, and " + numNeutrons + " neutron" + neutronPlural + ". That means you gained " + numNeutrons + " extra neutron" + neutronPlural + " from its stable state.";
            }
            else if (numElectrons > 1 && numNeutrons > 0)
            {
                atomicDescriptionText.text = "That is, you created a Hydrogen isotope with 1 proton, " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ". That means you gained " + numNeutrons + " extra neutron " + neutronPlural + " and a net electric charge of +" + (numElectrons - numProtons) + "e from its stable state.";
            }
        }
        else
        {
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

                if (annihilatedParticle != GAME_WON_NAME)
                {
                    if (elementName == "??")
                    {
                        atomicDescriptionText.text += " This new isotope " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " and" + electronGainLose + " a net electric charge of ";
                    }
                    else
                    {
                        atomicDescriptionText.text += " That means you " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " and" + electronGainLose + " a net electric charge of ";
                    }

                    if (numElectrons > numProtons)
                    {
                        if (elementName == "??")
                        {
                            atomicDescriptionText.text += "+" + (numElectrons - numProtons) + "e from its stable state.";
                        }
                        else
                        {
                            atomicDescriptionText.text += "+" + (numElectrons - numProtons) + "e from its stable state.";
                        }
                    }
                    else if (numElectrons < numProtons)
                    {
                        if (elementName == "??")
                        {
                            atomicDescriptionText.text += "-" + (numProtons - numElectrons) + "e from its stable state.";
                        }
                        else
                        {
                            atomicDescriptionText.text += "-" + (numProtons - numElectrons) + "e from its stable state.";
                        }
                    }
                }
                else
                {
                    atomicDescriptionTextWon.text += " This isotope " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " and" + electronGainLose + " a net electric charge of ";
                    if (numElectrons > numProtons)
                    {
                        atomicDescriptionTextWon.text += "+" + (numElectrons - numProtons) + "e from its stable state.";
                    }
                    else if (numElectrons < numProtons)
                    {
                        atomicDescriptionTextWon.text += "-" + (numProtons - numElectrons) + "e from its stable state.";
                    }
                }
            }
            else if (extraCharge && !extraNeutrons)
            {
                atomicDescriptionText.text = atomicDescriptionText.text.Replace("isotope", "ion");
                if (annihilatedParticle != GAME_WON_NAME)
                {
                    if (elementName == "??")
                    {
                        atomicDescriptionText.text += " This ion gained a net electric charge of ";
                    }
                    else
                    {
                        atomicDescriptionText.text += " That means you gained a net electric charge of ";
                    }

                    if (numElectrons > numProtons)
                    {
                        if (elementName == "??")
                        {
                            atomicDescriptionText.text += "+" + (numElectrons - numProtons) + "e from its stable state.";
                        }
                        else
                        {
                            atomicDescriptionText.text += "+" + (numElectrons - numProtons) + "e from its stable state.";
                        }
                    }
                    else if (numElectrons < numProtons)
                    {
                        if (elementName == "??")
                        {
                            atomicDescriptionText.text += "-" + (numProtons - numElectrons) + "e from its stable state.";
                        }
                        else
                        {
                            atomicDescriptionText.text += "-" + (numProtons - numElectrons) + "e from its stable state.";
                        }
                    }
                }
                else
                {
                    atomicDescriptionTextWon.text += " This ion gained a net electric charge of ";
                    if (numElectrons > numProtons)
                    {
                        atomicDescriptionTextWon.text += "+" + (numElectrons - numProtons) + "e from its stable state.";
                    }
                    else if (numElectrons < numProtons)
                    {
                        atomicDescriptionTextWon.text += "-" + (numProtons - numElectrons) + "e from its stable state.";
                    }
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

                if (annihilatedParticle != GAME_WON_NAME)
                {
                    if (elementName == "??")
                    {
                        atomicDescriptionText.text += " This isotope " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " from its stable state.";
                    }
                    else
                    {
                        atomicDescriptionText.text += " That means you " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " from its stable state.";
                    }
                }
                else
                {
                    atomicDescriptionText.text += " This isotope " + gainLose + " " + Mathf.Abs(numNeutrons - numProtons) + neutronQuantifier + " neutron" + extraNeutronsPlural + " from its stable state.";
                }
            }
            else
            {
                if (annihilatedParticle != GAME_WON_NAME)
                {
                    if (elementName == "??")
                    {
                        atomicDescriptionText.text = "This unknown isotope is a perfectly balanced " + elementName + " atom with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
                    }
                    else
                    {
                        atomicDescriptionText.text = "That is, you created a perfectly balanced " + elementName + " atom with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
                    }
                }
                else
                {
                    atomicDescriptionText.text = "This unknown isotope you created was a perfectly balanced atom with " + numProtons + " proton" + protonPlural + ", " + numNeutrons + " neutron" + neutronPlural + ", and " + numElectrons + " electron" + electronPlural + ".";
                }
            }
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

    public void KeepPlaying()
    {
        pauseMenu.CanPause(true);
        particleSpawner.AllowSpawning(true);

        stabilityBar.SetActive(true);
        scoreBox.SetActive(true);
        finalForm.SetActive(true);
        pauseButton.SetActive(true);
        blackHoleInformation.SetActive(true);
        goalBox.SetActive(true);
        pointsBox.SetActive(true);

        Time.timeScale = 1;
        FindObjectOfType<GridWave>().StartWaving();
        player.AllowMovement(true);

        gameWonScreen.SetActive(false);
    }
}
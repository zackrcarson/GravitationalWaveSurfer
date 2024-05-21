using UnityEngine.UI;
using UnityEngine;

public class Goals : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject scoreBox = null;
    [SerializeField] Text scoreText = null;

    [SerializeField] int nextGoalDistanceMin = 3;
    [SerializeField] int nextGoalDistanceMax = 7;
    [SerializeField] int extraNeutronsMax = 3;
    [SerializeField] int extraElectronsMax = 3;

    [SerializeField] Text elementText = null;
    [SerializeField] Text massNumberText = null;
    [SerializeField] Text AtomicNumberText = null;
    [SerializeField] Text IonicNumberText = null;

    [SerializeField] int goalPoints = 10;
    [SerializeField] int missedGoalDeduction = 20;

    // State Variables
    int nextGoalProtons = 0;
    int nextGoalNeutrons = 0;
    int nextGoalElectrons = 0;

    int score = 0;
    int numGoals = 0;
    int numMisses = 0;

    bool hasGoals = true;

    // Cached Referencess
    int difficulty = 2;
    int storyGoalProtons = 118;

    // Start is called before the first frame update
    public void ExternalStart()
    {
        storyGoalProtons = GetComponent<GameManager>().maxProtons;

        difficulty = GetComponent<GameManager>().difficulty;

        PickNextGoal(GameManager.instance.GetScore()[0]);

        ShowGoal();

        UpdateScore(0);
        if (difficulty == 0)
        {
            scoreBox.SetActive(false);
        }
    }

    private void PickNextGoal(int currentProtons)
    {
        if (!hasGoals) { return; }

        nextGoalProtons = currentProtons + Random.Range(nextGoalDistanceMin, nextGoalDistanceMax);

        if (nextGoalProtons > storyGoalProtons) { nextGoalProtons = storyGoalProtons; }

        if (difficulty == 3)
        {
            nextGoalNeutrons = nextGoalProtons + Random.Range(-extraNeutronsMax, extraNeutronsMax);
            nextGoalElectrons = nextGoalProtons + Random.Range(-extraElectronsMax, extraElectronsMax);
        }
        else if (difficulty == 2)
        {
            nextGoalNeutrons = nextGoalProtons + Random.Range(-extraNeutronsMax, extraNeutronsMax);
            nextGoalElectrons = nextGoalProtons;
        }
        else if (difficulty == 1)
        {
            nextGoalNeutrons = nextGoalProtons;
            nextGoalElectrons = nextGoalProtons;
        }
        else if (difficulty == 0)
        {
            nextGoalProtons = storyGoalProtons;
            nextGoalNeutrons = nextGoalProtons;
            nextGoalElectrons = nextGoalProtons;
        }

        if (nextGoalNeutrons < 1)
        {
            nextGoalNeutrons = 1;
        }

        if (nextGoalElectrons < 1)
        {
            nextGoalElectrons = 1;
        }
    }

    private void ShowGoal()
    {
        if (!hasGoals) { return; }

        elementText.text = GameManager.instance.GetElementName(nextGoalProtons)[0];

        if (difficulty > 1)
        {
            massNumberText.text = (nextGoalProtons + nextGoalNeutrons).ToString();
        }
        else
        {
            massNumberText.gameObject.SetActive(false);
        }

        AtomicNumberText.text = nextGoalProtons.ToString();

        if (nextGoalElectrons == nextGoalProtons)
        {
            IonicNumberText.gameObject.SetActive(false);
        }
        else
        {
            IonicNumberText.gameObject.SetActive(true);

            if (nextGoalElectrons > nextGoalProtons)
            {
                IonicNumberText.text = "+" + (nextGoalElectrons - nextGoalProtons).ToString();
            }
            else if (nextGoalElectrons < nextGoalProtons)
            {
                IonicNumberText.text = "-" + (nextGoalProtons - nextGoalElectrons).ToString();
            }
        }
    }

    public void CheckGoal(int[] newParticles)
    {
        if (!hasGoals) { return; }

        /*if ((newParticles[0] > nextGoalProtons) || (difficulty == 2 && newParticles[0] == nextGoalProtons && newParticles[1] > nextGoalNeutrons) || (difficulty == 3 && newParticles[0] == nextGoalProtons && (newParticles[1] > nextGoalNeutrons || newParticles[2] > nextGoalElectrons)))
        {
            MissedGoalLogic(newParticles[0]);
        }*/

        if (difficulty == 3 && newParticles[0] == nextGoalProtons && newParticles[1] == nextGoalNeutrons && newParticles[2] == nextGoalElectrons)
        {
            GoalLogic(newParticles[0]);
        }
        else if (difficulty == 2 && newParticles[0] == nextGoalProtons && newParticles[1] == nextGoalNeutrons)
        {
            GoalLogic(newParticles[0]);
        }
        else if (difficulty <= 1 && newParticles[0] == nextGoalProtons)
        {
            if (difficulty == 1)
            {
                GoalLogic(newParticles[0]);
            }
            else
            {
                UpdateScore(goalPoints);

                // TODO: Ding and cool particle effect or animation

                StopGoals();
            }
        }
    }

    private void GoalLogic(int newProtons)
    {
        UpdateScore(goalPoints);

        // TODO: Ding and cool particle effect or animation

        if (nextGoalProtons == storyGoalProtons)
        {
            StopGoals();
        }
        else
        {
            PickNextGoal(newProtons);

            ShowGoal();
        }
    }

    private void UpdateScore(int amount)
    {
        score += amount;

        if (amount < 0)
        {
            numMisses += 1;
        }
        else if(amount > 0)
        {
            numGoals += 1;
        }

        scoreText.text = score.ToString();
    }

    private void MissedGoalLogic(int newProtons)
    {
        UpdateScore(-missedGoalDeduction);

        // TODO: Erhhhh and cool particle effect or animation or screen shake, etc.

        if (nextGoalProtons > storyGoalProtons)
        {
            StopGoals();
        }
        else
        {
            PickNextGoal(newProtons);

            ShowGoal();
        }
    }

    public void StopGoals()
    {
        hasGoals = false;

        elementText.text = "??";
        IonicNumberText.gameObject.SetActive(false);
        massNumberText.gameObject.SetActive(false);
        AtomicNumberText.gameObject.SetActive(false);
    }

    public int[] GetScore()
    {
        return new int[] { score, numGoals, numMisses };
    }
}
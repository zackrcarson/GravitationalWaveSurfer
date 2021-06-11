using UnityEngine.UI;
using UnityEngine;

public class Goals : MonoBehaviour
{
    // Config Parameters
    [SerializeField] int nextGoalDistanceMin = 3;
    [SerializeField] int nextGoalDistanceMax = 7;
    [SerializeField] int extraNeutronsMax = 3;
    [SerializeField] int extraElectronsMax = 3;

    [SerializeField] Text elementText = null;
    [SerializeField] Text massNumberText = null;
    [SerializeField] Text AtomicNumberText = null;
    [SerializeField] Text IonicNumberText = null;

    [SerializeField] int storyGoalProtons = 118;

    // State Variables
    int nextGoalProtons = 0;
    int nextGoalNeutrons = 0;
    int nextGoalElectrons = 0;

    // Cached Referencess
    int difficulty = 2;

    // Start is called before the first frame update
    void Start()
    {
        difficulty = GetComponent<GameManager>().difficulty;

        Debug.Log(("goal", difficulty));

        PickNextGoal(GameManager.instance.GetScore()[0]);

        ShowGoal();
    }

    private void PickNextGoal(int currentProtons)
    {
        nextGoalProtons = currentProtons + Random.Range(nextGoalDistanceMin, nextGoalDistanceMax);

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
        elementText.text = GameManager.instance.GetElementName(nextGoalProtons)[0];

        massNumberText.text = (nextGoalProtons + nextGoalNeutrons).ToString();
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
        if (newParticles[0] == nextGoalProtons && newParticles[1] == nextGoalNeutrons && newParticles[2] == nextGoalElectrons)
        {
            PickNextGoal(newParticles[0]);

            ShowGoal();

            // TODO: Ding and cool particle effect or animation
        }
    }
}

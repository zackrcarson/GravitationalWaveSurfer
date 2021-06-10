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

    // State Variables
    int nextGoalProtons = 0;
    int nextGoalNeutrons = 0;
    int nextGoalElectrons = 0;

    // Start is called before the first frame update
    void Start()
    {
        PickNextGoal(GameManager.instance.GetScore()[0]);

        ShowGoal();
    }

    private void PickNextGoal(int currentProtons)
    {
        nextGoalProtons = currentProtons + Random.Range(nextGoalDistanceMin, nextGoalDistanceMax);
        nextGoalNeutrons = nextGoalProtons + Random.Range(-extraNeutronsMax, extraNeutronsMax);
        nextGoalElectrons = nextGoalProtons + Random.Range(-extraElectronsMax, extraElectronsMax);

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
        // TODO: Switch on difficulty
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
        // TODO: Switch on difficulty
        if (newParticles[0] == nextGoalProtons && newParticles[1] == nextGoalNeutrons && newParticles[2] == nextGoalElectrons)
        {
            PickNextGoal(newParticles[0]);

            ShowGoal();

            // TODO: Ding and cool particle effect or animation
        }
    }
}

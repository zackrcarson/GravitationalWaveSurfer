using UnityEngine.UI;
using UnityEngine;

public class Stability : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Slider stabilityBar = null;
    [SerializeField] int[] surplusAmountToInstabilities = { 15, 11, 8, 6 };

    // State variables
    float instability = 0;

    // Cached References
    Player player = null;
    float maxBarValue = 100f;
    int surplusAmountToInstability = 8; 

    // Constants
    const string INSTABILITY_NAME = "instability";

    // Start is called before the first frame update
    public void ExternalStart()
    {
        int difficulty = GetComponent<GameManager>().difficulty;
        surplusAmountToInstability = surplusAmountToInstabilities[difficulty];

        player = FindObjectOfType<Player>();

        stabilityBar.value = instability;

        maxBarValue = stabilityBar.maxValue;
    }

    public void updateStability(int[] particleCounts)
    {
        instability = (float)Mathf.Abs(particleCounts[1] - particleCounts[0]) * (maxBarValue / (float)surplusAmountToInstability);
        instability += (float)Mathf.Abs(particleCounts[2] - particleCounts[0]) * (maxBarValue / (float)surplusAmountToInstability);

        if (instability > maxBarValue) { instability = maxBarValue; }

        if (particleCounts[0] == 1)
        {
            instability = (float)Mathf.Abs(particleCounts[2] - 1) * (maxBarValue / (float)surplusAmountToInstability);

            instability += (float)particleCounts[1] * (maxBarValue / (float)surplusAmountToInstability);
        }

        stabilityBar.value = instability;

        if (instability >= maxBarValue)
        {
            player.KillPlayer(INSTABILITY_NAME);
        }
    }
}

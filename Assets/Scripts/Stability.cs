using UnityEngine.UI;
using UnityEngine;

public class Stability : MonoBehaviour
{
    // Config Parameters
    [SerializeField] Slider stabilityBar = null;
    [SerializeField] int surplusAmountToInstability = 8;

    // State variables
    float instability = 0;

    // Cached References
    Player player = null;
    float maxBarValue = 100f;

    // Constants
    const string INSTABILITY_NAME = "instability";

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        stabilityBar.value = instability;

        maxBarValue = stabilityBar.maxValue;
    }

    public void updateStability(int[] particleCounts)
    {
        instability = (float)Mathf.Abs(particleCounts[1] - particleCounts[0]) * (maxBarValue / (float)surplusAmountToInstability);
        instability += (float)Mathf.Abs(particleCounts[2] - particleCounts[0]) * (maxBarValue / (float)surplusAmountToInstability);

        if (instability > maxBarValue) { instability = maxBarValue; }

        stabilityBar.value = instability;

        if (instability >= maxBarValue)
        {
            player.KillPlayer("instability");
        }
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // State Variables
    [SerializeField] int numProtons = 1;
    [SerializeField] int numNeutrons = 1;
    [SerializeField] int numElectrons = 1;

    // Constants
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void AddParticle(string type)
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
        else
        {
            Debug.LogError("Unknown particle (" + type + ") not added!");
        }
    }
}

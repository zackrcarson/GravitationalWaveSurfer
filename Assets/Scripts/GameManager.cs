using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // State Variables
    [Header("Initial Numbers")]
    [SerializeField] int numProtons = 1;
    [SerializeField] int numNeutrons = 1;
    [SerializeField] int numElectrons = 1;

    [Header("Text Boxes")]
    [SerializeField] Text protonsText = null;
    [SerializeField] Text neutronsText = null;
    [SerializeField] Text electronsText = null;

    [SerializeField] Text elementText = null;
    [SerializeField] Text massNumberText = null;
    [SerializeField] Text AtomicNumberText = null;
    [SerializeField] Text IonicNumberText = null;

    [Header("Data")]
    [SerializeField] string elementsData = "Assets/Data/elements.csv";
    
    // Cached References
    Dictionary<int, string> elementsDict = null;

    // Constants
    const string PROTON_NAME = "Proton";
    const string NEUTRON_NAME = "Neutron";
    const string ELECTRON_NAME = "Electron";
    const string ANTI_PREFIX = "Anti-";

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CollectElements();

        ShowScore();
    }

    private void CollectElements()
    {
        elementsDict = new Dictionary<int, string>();

        StreamReader strReader = new StreamReader(elementsData);
        bool endOfFile = false;

        while (!endOfFile)
        {
            string dataString = strReader.ReadLine();

            if (dataString == null)
            {
                endOfFile = true;
                break;
            }

            string[] dataValues = dataString.Split(',');

            elementsDict.Add(int.Parse(dataValues[0]), dataValues[1]);
        }
    }

    private void ShowScore()
    {
        protonsText.text = numProtons.ToString();
        neutronsText.text = numNeutrons.ToString();
        electronsText.text = numElectrons.ToString();

        elementText.text = elementsDict[numProtons];
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
            AddParticle(type);
        }

        ShowScore();
    }

    private void AddParticle(string type)
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
    }
}

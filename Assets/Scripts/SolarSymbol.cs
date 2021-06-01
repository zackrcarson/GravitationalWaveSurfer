using UnityEngine;
using UnityEngine.UI;

public class SolarSymbol : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = "\u2609";
    }
}

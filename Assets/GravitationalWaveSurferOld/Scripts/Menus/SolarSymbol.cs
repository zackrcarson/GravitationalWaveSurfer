using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolarSymbol : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = GetComponent<TextMeshProUGUI>().text.Replace("<sub>o</sub>", "<sub>\u2609</sub>");//"\u2609";
    }
}

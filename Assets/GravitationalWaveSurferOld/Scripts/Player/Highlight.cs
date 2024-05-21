using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] GameObject highlightObject = null; 
    [SerializeField] bool startsOnPlayer = false; 

    // Start is called before the first frame update
    void Start()
    {
        highlightObject.SetActive(startsOnPlayer);
    }

    public void ToggleHighlight(bool toggle)
    {
        highlightObject.SetActive(toggle);
    }
}

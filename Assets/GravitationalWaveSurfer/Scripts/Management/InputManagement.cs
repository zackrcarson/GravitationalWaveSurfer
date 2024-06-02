using UnityEngine;
using System;

public class InputManagement : MonoBehaviour 
{
    public static InputManagement Instance;
    public GameObject UI;
    public bool UIVisible = true;

    private void Start() 
    {
        Instance = this;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle the visibility of UI elements
            UI.SetActive(!UIVisible);
            UIVisible = !UIVisible;
        }
    }

    void HandleKeyPressed()
    {

    }
}
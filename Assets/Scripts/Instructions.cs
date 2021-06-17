using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Instructions : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject instructionsPanel = null;
    [SerializeField] GameObject[] panels = null;

    [SerializeField] Text headerText = null;

    [SerializeField] GameObject backButton = null;
    [SerializeField] GameObject forwardButton = null;
    [SerializeField] TextMeshProUGUI skipButtonText = null;

    // Cached References
    PauseMenu pauseMenu = null;

    // State Variables
    int currentPanel = 0;
    bool openedFromPause = false;

    private void Start()
    {
        pauseMenu = GetComponent<PauseMenu>();
    }

    public void OpenInstructions(bool fromPause = false)
    {
        pauseMenu.CanPause(false);

        openedFromPause = fromPause;

        currentPanel = 0;

        OpenPanel(currentPanel);

        instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        if (openedFromPause)
        {
            pauseMenu.CloseInstructions();
        }
        else
        {
            GameManager.instance.CloseInstructions();
        }

        instructionsPanel.SetActive(false);

        currentPanel = 0;

        OpenPanel(currentPanel);

        StartCoroutine(DelayedAllowPause());
    }

    private IEnumerator DelayedAllowPause()
    {
        yield return null;

        pauseMenu.CanPause(true);
    }

    private void OpenPanel(int panelNumber)
    {
        int i = 0;
        foreach (GameObject panel in panels)
        {
            panel.SetActive(i == panelNumber);
            
            i++;
        }

        UpdateButtons(panelNumber);
        UpdateHeader(panelNumber);
    }

    private void UpdateButtons(int panelNumber)
    {
        if (panelNumber == 0)
        {
            backButton.SetActive(false);
            forwardButton.SetActive(true);

            skipButtonText.text = "Skip";
        }
        else if (panelNumber == panels.Length - 1)
        {
            backButton.SetActive(true);
            forwardButton.SetActive(false);

            skipButtonText.text = "Done";
        }
        else
        {
            backButton.SetActive(true);
            forwardButton.SetActive(true);

            skipButtonText.text = "Skip";
        }
    }

    private void UpdateHeader(int panelNumber)
    {
        headerText.text = panels[panelNumber].name;
    }

    public void NextPanel()
    {
        currentPanel++;

        OpenPanel(currentPanel);
    }

    public void PreviousPanel()
    {
        currentPanel--;

        OpenPanel(currentPanel);
    }
}

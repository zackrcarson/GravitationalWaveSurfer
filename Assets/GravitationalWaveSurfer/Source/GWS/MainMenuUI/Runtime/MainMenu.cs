using Eflatun.SceneReference;
using GWS.CommandPattern.Runtime;
using GWS.SceneManagement.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace GWS.MainMenuUI.Runtime
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] 
        private SceneReference gameEntryScene;
        
        // Config Parameters
        [SerializeField] GameObject difficultyPopup = null;
        [SerializeField] Text descriptionText = null;
        [SerializeField] GameObject[] difficultyButtons = null;

        // State Variables
        int difficulty = 2;

        // Constants
        const string STORY_IDENTIFIER = "story";
        const string EASY_IDENTIFIER = "easy";
        const string NORMAL_IDENTIFIER = "normal";
        const string HARD_IDENTIFIER = "hard";

        private void Start()
        {
            difficultyPopup.SetActive(false);
        }

        public void StartGame()
        {
            GameObject difficultyHolder = new GameObject("Difficulty Holder");
            difficultyHolder.AddComponent<DifficultyHolder>().difficulty = difficulty;
            DontDestroyOnLoad(difficultyHolder);

            CommandInvoker.Execute(new LoadSceneCommand(gameEntryScene));
        }

        public void OpenDifficultyPopup()
        {
            ButtonSelect(NORMAL_IDENTIFIER);

            difficultyPopup.SetActive(true);
        }

        public void ButtonSelect(string option)
        {
            switch (option)
            {
                case STORY_IDENTIFIER:
                    descriptionText.text = "In story mode, no anti-particles will spawn, your atoms will remain ultra-stable, and your only goal will be to forge the heaviest element known. Choose this mode if you do not want any challenge and would prefer to focus on creating fun and unique atoms!";
                    difficulty = 0;

                    SetOutline(difficulty);
                    break;

                case EASY_IDENTIFIER:
                    descriptionText.text = "In easy mode, only a small number of anti-particles will spawn, your atoms will remain fairly stable, and the goals will be very easy to reach. Choose this mode if you want only a small challenge!";
                    difficulty = 1;

                    SetOutline(difficulty);
                    break;

                case NORMAL_IDENTIFIER:
                    descriptionText.text = "In normal mode, a decent number of anti-particles will spawn, your atoms will decay at a standard physical rate, and the goals will be more difficult to reach. Choose this mode if you want a balanced gameplay with some difficulty!";
                    difficulty = 2;

                    SetOutline(difficulty);
                    break;

                case HARD_IDENTIFIER:
                    descriptionText.text = "In hard mode, a lot of anti-particles will spawn, your atoms will become unstable very easily, and the goals will be extremely difficult to reach. Only choose this mode if you want an extreme challenge!";
                    difficulty = 3;

                    SetOutline(difficulty);
                    break;

                default:
                    Debug.LogError("No valid button ( " + option + " ) found.");
                    break;
            }
        }

        private void SetOutline(int numberOn)
        {
            int i = 0;
            foreach (GameObject button in difficultyButtons)
            {
                button.GetComponent<Outline>().enabled = (i == numberOn);

                i++;
            }
        }
    }
}

using UnityEngine;
using System.Collections;

using TMPro;

namespace GWS.GameStage
{
    public class GameStageManager : MonoBehaviour
    {
        public static GameStageManager Instance { get; private set; }

        [Header("Game stage parameters")]
        public float gameTime = 600f;
        public int multiplier = 1;
        public int incPerQuizQuestion = 3;
        public int incPassiveTotal = 40;
        public float incPassiveTime = 2.2f;

        [Space(6)]
        [Header("Game stage UI")]
        public GameObject gameStageObject;
        public TextMeshProUGUI gameStageText;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); }
        }

        private void Start() 
        {
            if (gameStageObject == null) Debug.LogWarning("Game stage UI not set!!!");

            gameStageText = gameStageObject.GetComponent<TextMeshProUGUI>();

            incPassiveTime =  gameTime / (float) incPassiveTotal;
            Debug.Log(incPassiveTime);

            StartCoroutine(PassiveGameStageInc());
        }

        private IEnumerator PassiveGameStageInc()
        {
            while (true)
            {
                yield return new WaitForSeconds(incPassiveTime);
                multiplier++;
                gameStageText.text = $"Multiplier: 10^{multiplier}";
            }
        }

        public void GameStageIncQuiz()
        {
            multiplier += incPerQuizQuestion;
            gameStageText.text = $"Multiplier: 10^{multiplier}";
        }

    }
}
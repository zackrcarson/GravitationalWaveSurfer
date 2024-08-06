using UnityEngine;

namespace GWS.Quiz
{
    [CreateAssetMenu(fileName = "New Quiz Question", menuName = "GWS/Quiz Question")]
    public class QuizQuestion : ScriptableObject
    {
        public int id;
        public string questionText;
        public string[] answerOptions;
        public int correctAnswerIndex;
    }
}
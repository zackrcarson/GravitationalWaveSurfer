using UnityEngine;
using System.Collections.Generic;

namespace GWS.Quiz
{
    [CreateAssetMenu(fileName = "POI Question Database", menuName = "GWS/Quiz Question Database")]
    public class QuizQuestionDatabase : ScriptableObject
    {
        public List<QuizQuestion> questions = new List<QuizQuestion>();

        /// <summary>
        /// Get a random question ID from the list of quiz questions
        /// </summary>
        /// <returns>index position of question in the list</returns>
        public int GetRandomQuestion()
        {
            if (questions.Count > 0)
            {
                int randomIndex = Random.Range(0, questions.Count);
                return randomIndex;
            }
            else
            {
                Debug.LogWarning("Quiz question database empty!!!");
            }
            return -1;
        }

        /// <summary>
        /// Get QuizQuestion object by index
        /// </summary>
        /// <param name="id">index of question</param>
        /// <returns>QuizQuestion object of the question</returns>
        public QuizQuestion GetQuestionById(int id)
        {
            return questions.Find(q => q.id == id);
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "POI Question Database", menuName = "POI/Question Database")]
public class POIQuestionDatabase : ScriptableObject
{
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    public QuizQuestion GetRandomQuestion()
    {
        if (questions.Count > 0)
        {
            int randomIndex = Random.Range(0, questions.Count);
            return questions[randomIndex];
        }
        return null;
    }

    public QuizQuestion GetQuestionById(int id)
    {
        return questions.Find(q => q.id == id);
    }
}
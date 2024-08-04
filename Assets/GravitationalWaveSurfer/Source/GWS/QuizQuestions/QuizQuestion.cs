using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Question", menuName = "POI/QuizQuestion")]
public class QuizQuestion : ScriptableObject
{
    public int id;
    public string questionText;
    public string[] answerOptions;
    public int correctAnswerIndex;
}
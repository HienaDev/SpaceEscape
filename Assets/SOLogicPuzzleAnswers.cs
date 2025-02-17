using UnityEngine;

public class LogicPuzzleSocket : MonoBehaviour
{
    [SerializeField] private SOLogicPuzzleAnswers correctAnswer;
    private TypeLogic currentAnswer;

    private GameObject currentAnswerObject;

    public bool IsCorrect() => correctAnswer.typeLogic == currentAnswer;

    public void AttemptAnswer(TypeLogic answerAttempt, GameObject answerAttemptObject)
    {

        if(currentAnswerObject != null)
            Destroy(currentAnswerObject);

        currentAnswer = answerAttempt;
        currentAnswerObject = answerAttemptObject;
    }
}

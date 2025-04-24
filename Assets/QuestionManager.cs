using UnityEngine;
using TMPro;
using DG.Tweening; // Import DOTween namespace

public class QuestionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inputKeyboardUI;
    public GameObject multipleChoiceUI;
    public MultipleChoiceUI multipleChoiceHandler;

    [Header("Question Data")]
    public QuestionSO[] questions;

    private int currentIndex = 0;
    private QuestionSO currentQuestion;

    [Header("UI Elements")]
    public TMP_InputField inputField;
    public TMP_Text questionText;
    public TMP_Text timerText;
    public TMP_Text timeDeducedText;  // New TMP for time deducted
    public AudioSource correctSound;
    public AudioSource wrongSound;
    public RectTransform inputFieldRect;
    public RectTransform timerRect;

    private float timeRemaining = 3600f;
    private bool isTimerRunning = true;

    private float totalTimeDeduced = 0f;
    private Vector3 deduceTextOriginalPosition;

    private bool isTransitioning = false; // Flag to track if an answer is in transition

    void Start()
    {
        // Store the original position of the time deducted text at the start
        deduceTextOriginalPosition = timeDeducedText.transform.localPosition;

        if (questions.Length > 0)
        {
            LoadQuestion(currentIndex);
        }
        else
        {
            Debug.LogWarning("No questions assigned to the manager.");
        }

        inputField.onEndEdit.AddListener(OnInputFieldSubmit);
        InvokeRepeating("UpdateTimer", 1f, 1f); // Timer update every second

        // Initially hide the time deducted text
        timeDeducedText.gameObject.SetActive(false);
    }

    private void UpdateTimer()
    {
        if (isTimerRunning)
        {
            timeRemaining -= 1f;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                isTimerRunning = false;
                Debug.Log("Time's up! Quiz over.");
            }
        }
    }

    private void LoadQuestion(int index)
    {


        currentQuestion = questions[index];

        inputKeyboardUI.SetActive(currentQuestion.isInputBased);
        multipleChoiceUI.SetActive(!currentQuestion.isInputBased);

        // Pop out the current UI
        if (currentQuestion.isInputBased)
        {
            inputKeyboardUI.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                // Load the new question
                if (questionText != null)
                {
                    questionText.text = "PERGUNTA " + (currentIndex + 1);
                }
                // Set input field color back to white after pop-in
                inputField.image.color = Color.white;

                // Clear the input field text after animation
                inputField.text = "";
                // Pop in the new UI (input or multiple choice)
                inputKeyboardUI.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                {

                    isTransitioning = false; // End transition
                });
            });
        }
        else
        {
            // Pop out the current UI
            multipleChoiceUI.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                if (questionText != null)
                {
                    questionText.text = "PERGUNTA " + (currentIndex + 1);
                }

                // Set up options for multiple choice question
                Option[] shuffledOptions = (Option[])currentQuestion.options.Clone();
                ShuffleOptions(shuffledOptions);

                // Set up the buttons with shuffled options
                if (multipleChoiceHandler != null)
                {
                    multipleChoiceHandler.SetupOptions(shuffledOptions, CheckAnswer);
                }

                // Pop in the new UI
                multipleChoiceUI.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                {
                    isTransitioning = false; // End transition
                });
            });
        }
    }

    public void CheckAnswer(string answer)
    {
        // Prevent CheckAnswer from being called while transitioning
        if (isTransitioning) return;

        bool isCorrect = answer.Trim().ToLower() == currentQuestion.correctAnswer.Trim().ToLower();
        Debug.Log("Answer Correct? " + isCorrect);

        if (isCorrect)
        {
            CorrectAnswer();
        }
        else
        {
            IncorrectAnswer();
        }
    }

    private void CorrectAnswer()
    {
        // Change input field to green before pop out
        inputField.image.color = Color.green;

        // Play correct answer sound (buzz)
        if (correctSound != null) correctSound.Play();

        // Get the current scale of the input field
        Vector3 currentScale = inputFieldRect.localScale;

        isTransitioning = true; // Start transition

        // Proceed to next question after a small delay
        Invoke("NextQuestion", 1f);

        // Animate the input field (pop out and back in) using the current scale
        inputFieldRect.DOScale(currentScale * 1.2f, 0.2f).OnComplete(() =>
        {
            // Pop back in using the current scale (set to white after pop-in)
            inputFieldRect.DOScale(currentScale, 0.2f).OnComplete(() =>
            {

            });
        });


    }

    private void IncorrectAnswer()
    {
        // Change input field to red
        inputField.image.color = Color.red;

        // Play wrong answer sound
        if (wrongSound != null) wrongSound.Play();

        // Shake the input field while it's red
        inputFieldRect.DOShakePosition(0.5f, 10f, 10, 90f, false, true).OnComplete(() =>
        {
            // After shaking, reset input field color to white
            inputField.image.color = Color.white;
        });

        // Deduct 5 minutes from the timer
        DeductTime(5f);

        // Show time deducted with animation (instantiate a new time deducted text)
        ShowTimeDeducted(5f);

        // Shake the timer
        timerRect.DOShakePosition(0.5f, 5f, 10, 90f, false, true);
    }

    private void NextQuestion()
    {
        currentIndex++;
        if (currentIndex < questions.Length)
        {
            LoadQuestion(currentIndex);
        }
        else
        {
            Debug.Log("Quiz complete!");
        }
    }

    private void DeductTime(float minutes)
    {
        timeRemaining -= minutes * 60;
        if (timeRemaining < 0)
        {
            timeRemaining = 0;
        }

        UpdateTimer();
    }

    private void ShowTimeDeducted(float minutes)
    {
        totalTimeDeduced = minutes;

        // Format the time deduction string as -{minutes}:{seconds}
        int minutesDeducted = Mathf.FloorToInt(totalTimeDeduced);
        int secondsDeducted = Mathf.FloorToInt((totalTimeDeduced - minutesDeducted) * 60);
        string timeText = string.Format("-{0:D2}:{1:D2}", minutesDeducted, secondsDeducted);

        // Instantiate a new TMP_Text object to show the time deducted
        TMP_Text newTimeDeducedText = Instantiate(timeDeducedText, timeDeducedText.transform.parent);

        // Update the new instance of the text with the time deduction
        newTimeDeducedText.text = timeText;

        // Show the text object
        newTimeDeducedText.gameObject.SetActive(true);

        // Move 20 units up from the original position
        Vector3 targetPosition = deduceTextOriginalPosition + new Vector3(0, 20, 0); // Add 20 units to the Y position

        // Fade in immediately and move vertically up
        newTimeDeducedText.DOFade(1, 0f);
        newTimeDeducedText.transform.DOLocalMoveY(targetPosition.y, 3f).SetEase(Ease.OutCubic);

        // Animate the time deduction text (move up and fade out)
        newTimeDeducedText.DOFade(0, 3f).OnComplete(() =>
        {
            // Reset position to original position and hide after fading out
            newTimeDeducedText.transform.localPosition = deduceTextOriginalPosition;  // Reset to the original position
            newTimeDeducedText.gameObject.SetActive(false);
        });
    }

    private void OnInputFieldSubmit(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            CheckAnswer(input);
        }
    }

    private void ShuffleOptions(Option[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            Option temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}

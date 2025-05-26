using UnityEngine;

public class MainMenuTransition : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private GameObject mainMenuView;
    [SerializeField] private GameObject customizationView;
    [SerializeField] private GameObject missionView;
    [SerializeField] private GameObject winView;
    [SerializeField] private GameObject loseView;

    [Header("Transition")]
    [SerializeField] private GateTransition transition;

    private GameObject currentObject;

    void Start()
    {
        // Set the initial screen
        currentObject = mainMenuView;
        ShowOnly(mainMenuView);
    }

    public void TransitionToCustomization()
    {
        transition.TriggerTransition(currentObject, customizationView);
        currentObject = customizationView;
    }

    public void TransitionToMission()
    {
        transition.TriggerTransition(currentObject, missionView);
        currentObject = missionView;
    }

    public void TransitionToWin()
    {
        transition.TriggerTransition(currentObject, winView);
        currentObject = winView;
    }

    public void TransitionToLose()
    {
        transition.TriggerTransition(currentObject, loseView);
        currentObject = loseView;
    }

    public void TransitionToMainMenu()
    {
        transition.TriggerTransition(currentObject, mainMenuView);
        currentObject = mainMenuView;
    }

    private void ShowOnly(GameObject target)
    {
        // Disable all views except the target one
        mainMenuView.SetActive(false);
        customizationView.SetActive(false);
        missionView.SetActive(false);
        winView.SetActive(false);
        loseView.SetActive(false);

        target.SetActive(true);
    }
}

using UnityEngine;

public class LogicGateLevelManager : MonoBehaviour , IPuzzle
{

    [SerializeField] private LogicPuzzleSocket[] levelSockets;
    public MissionSelection missionSelection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckIfPuzzleSolved()
    {
        bool wrong = false;
        foreach (LogicPuzzleSocket levelSocket in levelSockets)
        {

            if (!levelSocket.IsCorrect())
            {
                levelSocket.GetComponent<SpriteRenderer>().color = Color.red;
                wrong = true;
            }
            else
                levelSocket.GetComponent<SpriteRenderer>().color = Color.green;
        }
        
        if(!wrong)
        {
            Debug.Log("Level Right");
            CompleteLevel();
        }
        else
        {
            Debug.Log("Level Wrong");
            return;
        }

    }

    public void CompleteLevel()
    {
        CompletePuzzle();
    }

    public void StartPuzzle(MissionSelection m)
    {
        missionSelection = m;
    }

    public void CompletePuzzle()
    {
        missionSelection.CompleteMission();
    }
}

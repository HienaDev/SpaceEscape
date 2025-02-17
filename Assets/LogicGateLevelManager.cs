using UnityEngine;

public class LogicGateLevelManager : MonoBehaviour
{

    [SerializeField] private LogicPuzzleSocket[] levelSockets;

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
        foreach (LogicPuzzleSocket levelSocket in levelSockets)
        {
            if (!levelSocket.IsCorrect())
            {
                Debug.Log("Level Wrong"); 
                return;
            }
        }
        Debug.Log("Level Right");
        CompleteLevel();
    }

    public void CompleteLevel()
    {

    }
}

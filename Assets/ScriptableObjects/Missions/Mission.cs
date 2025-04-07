using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Scriptable Objects/Mission")]
public class Mission : ScriptableObject
{
    public GameObject missionPrefab;
    public string missionTitle = "Test Title";
    public string missionDescription = "Test Description";
}

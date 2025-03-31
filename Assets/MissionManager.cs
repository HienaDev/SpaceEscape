using TMPro;
using UnityEngine;

public class MissionManager : MonoBehaviour
{

    [SerializeField] private int numberOfPlayers = 4;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spacingBetweenPlayers = 3f;
    [SerializeField] private string[] names;

    public GameObject trianglePrefab;  // Triangle prefab to spawn
    public int triangleCount = 8;      // Number of triangles (default: 8)

    [SerializeField] private Mission[] missions;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTriangles();
        //InstanciatePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnTriangles()
    {
        if (trianglePrefab == null)
        {
            Debug.LogError("No triangle prefab assigned!");
            return;
        }

        float triangleBase = 1f; // Default scale.x = 1
        float angleStep = 360f / triangleCount; // Angle between each triangle
        float radius = (triangleBase / 2) / Mathf.Sin(Mathf.PI / triangleCount); // Correct radius

        for (int i = 0; i < triangleCount; i++)
        {

            float angle = i * angleStep; // Angle in degrees
            float radians = angle * Mathf.Deg2Rad; // Convert to radians

            // Calculate position using circular coordinates
            float xPos = Mathf.Cos(radians) * radius;
            float yPos = Mathf.Sin(radians) * radius;
            Vector2 spawnPosition = new Vector2(xPos, yPos);

            // Instantiate triangle
            GameObject triangle = Instantiate(trianglePrefab, spawnPosition, Quaternion.identity);

            // Rotate it to face the center
            float rotationAngle = angle + 90f; // Rotate 90 degrees so the tip points inward
            triangle.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            if (i < missions.Length)
                if (missions[i] != null)
                {
                    triangle.GetComponent<MissionSelection>().SetMission(missions[i]);
                    triangle.GetComponent<SpriteRenderer>().color = Color.green;
                }

        }
    }

    public void InstanciatePlayers()
    {
        // Calculate the total width occupied by all objects
        float totalWidth = (numberOfPlayers - 1) * spacingBetweenPlayers;

        int numberOfRepeats = 0;

        for (int i = 0; i < numberOfPlayers; i++)
        {   
            // Compute position: Centered around (0,0) in world space
            float xPos = (i * spacingBetweenPlayers) - (totalWidth / 2f);
            Vector2 spawnPosition = new Vector2(xPos, transform.position.y);

            // Instantiate the object
            GameObject playerTemp = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

            if(i - (numberOfRepeats * names.Length) >= names.Length)
                numberOfRepeats++;

            playerTemp.GetComponentInChildren<TextMeshProUGUI>().text = names[i - (numberOfRepeats * names.Length)];
        }
    }
}

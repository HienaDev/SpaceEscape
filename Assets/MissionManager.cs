using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class MissionManager : MonoBehaviour
{

    [SerializeField] private int numberOfPlayers = 4;
    [SerializeField] private GameObject playerPrefab;
    private List<GameObject> players = new List<GameObject>(); // List to store player instances
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
        float verticalOffset = 2f;
        float duration = 1f;

        int numberOfRepeats = 0;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            // Compute position: Centered around (0,0) in world space
            float xPos = (i * spacingBetweenPlayers) - (totalWidth / 2f);
            Vector2 spawnPosition = new Vector2(xPos, transform.position.y);
            Vector2 startPosition = spawnPosition + Vector2.up * verticalOffset;

            // Instantiate the object at the *above* position
            GameObject playerTemp = Instantiate(playerPrefab, startPosition, Quaternion.identity);

            if (i - (numberOfRepeats * names.Length) >= names.Length)
                numberOfRepeats++;

            players.Add(playerTemp);

            // Set player name
            playerTemp.GetComponentInChildren<TextMeshProUGUI>().text = names[i - (numberOfRepeats * names.Length)];

            // Get components
            SpriteRenderer spriteRendererCircle = playerTemp.GetComponent<SpriteRenderer>();
            SpriteRenderer spriteRendererImage = playerTemp.GetComponentInChildren<SpriteRenderer>();
            TextMeshProUGUI text = playerTemp.GetComponentInChildren<TextMeshProUGUI>();

            // Set initial alpha to 0
            if (spriteRendererCircle != null)
            {
                Color col = spriteRendererCircle.color;
                col.a = 0f;
                spriteRendererCircle.color = col;
                spriteRendererCircle.DOFade(1f, duration);
            }

            // Set initial alpha to 0
            if (spriteRendererImage != null)
            {
                Color col = spriteRendererImage.color;
                col.a = 0f;
                spriteRendererImage.color = col;
                spriteRendererImage.DOFade(1f, duration);
            }

            if (text != null)
            {
                Color col = text.color;
                col.a = 0f;
                text.color = col;
                text.DOFade(1f, duration);
            }

            // Move from above with whip-like motion
            playerTemp.transform.DOMove(spawnPosition, duration).SetEase(Ease.OutElastic);
        }
    }
}

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
    private List<GameObject> players = new List<GameObject>();
    [SerializeField] private float spacingBetweenPlayers = 3f;
    [SerializeField] private string[] names;

    public GameObject trianglePrefab;
    public int triangleCount = 8;

    [SerializeField] private Mission[] missions;

    [SerializeField] private Transform trianglesParent;

    void Start()
    {
        InstanciatePlayers();
    }

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

        float triangleBase = 1f;
        float angleStep = 360f / triangleCount;
        float radius = (triangleBase / 2) / Mathf.Sin(Mathf.PI / triangleCount);

        for (int i = 0; i < triangleCount; i++)
        {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            float xPos = Mathf.Cos(radians) * radius;
            float yPos = Mathf.Sin(radians) * radius;
            Vector2 spawnPosition = new Vector2(xPos, yPos);

            GameObject triangle = Instantiate(trianglePrefab, trianglesParent);
            triangle.transform.position = spawnPosition;
            float rotationAngle = angle + 90f;
            triangle.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            if (i < missions.Length && missions[i] != null)
            {
                triangle.GetComponent<MissionSelection>().SetMission(missions[i]);
                triangle.GetComponent<MissionSelection>().triangleParent = trianglesParent;
                triangle.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    public void InstanciatePlayers()
    {
        float totalWidth = (numberOfPlayers - 1) * spacingBetweenPlayers;
        float verticalOffset = 2f;
        float duration = 2.5f;
        int numberOfRepeats = 0;
        List<Tweener> moveTweens = new List<Tweener>();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            float xPos = (i * spacingBetweenPlayers) - (totalWidth / 2f);
            Vector2 spawnPosition = new Vector2(xPos, transform.position.y);
            Vector2 startPosition = spawnPosition + Vector2.up * verticalOffset;

            GameObject playerTemp = Instantiate(playerPrefab, startPosition, Quaternion.identity);

            if (i - (numberOfRepeats * names.Length) >= names.Length)
                numberOfRepeats++;

            players.Add(playerTemp);

            Person person = playerTemp.GetComponent<Person>();
            person.nameOfPerson.text = names[i - (numberOfRepeats * names.Length)];

            if (person.spriteRendererCircle != null)
            {
                Color col = person.spriteRendererCircle.color;
                col.a = 0f;
                person.spriteRendererCircle.color = col;
                person.spriteRendererCircle.DOFade(1f, duration);
            }

            if (person.spriteRendererImage != null)
            {
                Color col = person.spriteRendererImage.color;
                col.a = 0f;
                person.spriteRendererImage.color = col;
                person.spriteRendererImage.DOFade(1f, duration);
            }

            if (person.nameOfPerson != null)
            {
                Color col = person.nameOfPerson.color;
                col.a = 0f;
                person.nameOfPerson.color = col;
                person.nameOfPerson.DOFade(1f, duration);
            }

            Tweener moveTween = playerTemp.transform.DOMove(spawnPosition, duration).SetEase(Ease.OutElastic);
            moveTweens.Add(moveTween);
        }

        Sequence sequence = DOTween.Sequence();
        foreach (var tween in moveTweens)
        {
            sequence.Join(tween);
        }

        sequence.OnComplete(() =>
        {
            ApplySpecialEffectsToRandomPlayer();
        });
    }


    private void ApplySpecialEffectsToRandomPlayer()
    {
        if (players.Count == 0) return;

        int randomIndex = Random.Range(0, players.Count);
        GameObject selectedPlayer = players[randomIndex];

        for (int i = 0; i < players.Count; i++)
        {
            GameObject player = players[i];
            Person person = player.GetComponent<Person>();

            if (person == null || person.nameOfTitle == null) continue;

            person.nameOfTitle.gameObject.SetActive(true); // 🔛 Make sure it's active

            if (i == randomIndex)
            {
                // 🧑‍✈️ This is the Captain
                person.nameOfTitle.text = "Captain";

                if (person.spriteRendererCircle != null)
                {
                    Material mat = new Material(person.spriteRendererCircle.material);
                    person.spriteRendererCircle.material = mat;

                    if (mat.HasProperty("_Outline"))
                        mat.SetFloat("_Outline", 1f);

                    if (mat.HasProperty("_OutlineThickness"))
                        mat.SetFloat("_OutlineThickness", 0f);

                    if (mat.HasProperty("_Distortion"))
                        mat.SetFloat("_Distortion", 1f);

                    if (mat.HasProperty("_DistortionStrength"))
                        mat.SetFloat("_DistortionStrength", 0f);

                    DOTween.To(() => mat.GetFloat("_OutlineThickness"), x => mat.SetFloat("_OutlineThickness", x), 0.05f, 1f);
                    DOTween.To(() => mat.GetFloat("_DistortionStrength"), x => mat.SetFloat("_DistortionStrength", x), 0.03f, 1f);

                    if (mat.HasProperty("_OutlineColor"))
                    {
                        Color targetColor = mat.GetColor("_OutlineColor");
                        Color startColor = person.nameOfTitle.color;

                        // Fade color
                        DOTween.To(() => person.nameOfTitle.color,
                                   x => person.nameOfTitle.color = x,
                                   targetColor,
                                   1f).SetEase(Ease.OutSine);
                    }
                }

                // Fade in the title text
                Color titleColor = person.nameOfTitle.color;
                titleColor.a = 0f;
                person.nameOfTitle.color = titleColor;
                person.nameOfTitle.DOFade(1f, 1f);
            }
            else
            {
                // 👨‍🚀 Just a humble Crewmate
                person.nameOfTitle.text = "Crewmate";

                // Fade in the crewmate title too
                Color titleColor = person.nameOfTitle.color;
                titleColor.a = 0f;
                person.nameOfTitle.color = titleColor;
                person.nameOfTitle.DOFade(1f, 1f);
            }
        }

        DOTween.Sequence()
            .AppendInterval(3f)
            .AppendCallback(() =>
            {
                DetransitionAndSpawnTriangles();
            });
    }




    private void DetransitionAndSpawnTriangles()
    {
        float fadeDuration = 1.2f;
        int completed = 0;

        foreach (GameObject player in players)
        {
            if (player == null) continue;

            Person person = player.GetComponent<Person>();
            if (person == null) continue;

            SpriteRenderer circle = person.spriteRendererCircle;
            SpriteRenderer image = person.spriteRendererImage;
            TextMeshProUGUI text = person.nameOfPerson;
            TextMeshProUGUI title = person.nameOfTitle;  // Reference the title text

            Material circleMaterial = null;
            Material imageMaterial = null;

            if (circle != null)
            {
                circleMaterial = new Material(circle.material);
                circle.material = circleMaterial;

                if (circleMaterial.HasProperty("_Distortion"))
                    circleMaterial.SetFloat("_Distortion", 1f);

                if (circleMaterial.HasProperty("_DistortionStrength"))
                    DOTween.To(() => circleMaterial.GetFloat("_DistortionStrength"), x => circleMaterial.SetFloat("_DistortionStrength", x), 0.1f, fadeDuration);

                if (circleMaterial.HasProperty("_Pixelation"))
                    circleMaterial.SetFloat("_Pixelation", 1f);

                // Apply Pixelation for the circle sprite
                if (circleMaterial.HasProperty("_PixelResolution"))
                    DOTween.To(() => circleMaterial.GetFloat("_PixelResolution"), x => circleMaterial.SetFloat("_PixelResolution", x), 0f, fadeDuration);
            }

            if (image != null)
            {
                imageMaterial = new Material(image.material);
                image.material = imageMaterial;

                if (imageMaterial.HasProperty("_Pixelation"))
                    imageMaterial.SetFloat("_Pixelation", 1f);

                // Apply Pixelation for the image sprite
                if (imageMaterial.HasProperty("_PixelResolution"))
                    DOTween.To(() => imageMaterial.GetFloat("_PixelResolution"), x => imageMaterial.SetFloat("_PixelResolution", x), 0f, fadeDuration);
            }

            int fadeComponents = 0;

            if (circle != null)
            {
                fadeComponents++;
                circle.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            if (image != null && image != circle)
            {
                fadeComponents++;
                image.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            if (text != null)
            {
                fadeComponents++;
                text.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            if (title != null)
            {
                fadeComponents++;
                title.DOFade(0f, fadeDuration).OnComplete(() => { CheckDone(); });
            }

            void CheckDone()
            {
                fadeComponents--;
                if (fadeComponents <= 0)
                {
                    Destroy(player);
                    completed++;

                    if (completed >= players.Count)
                    {
                        players.Clear();
                        SpawnTriangles();
                    }
                }
            }
        }
    }



}

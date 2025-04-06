using DG.Tweening;
using System;
using UnityEngine;


public class MissionSelection : MonoBehaviour
{
    public float moveDistance = 1f; // Distance to move up
    public float moveDuration = 1f; // Time to move up/down
    public Ease easingType = Ease.InOutSine; // Smoother animation
    private Vector3 startPosition;

    private Mission missionSO;
    public void SetMission(Mission mission) => missionSO = mission;

    public Transform triangleParent;

    private EventHandler onMissionClicked;

    private delegate void OnMissionClicked(int num);

    private bool isUp = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            //Debug.Log($"Touch detected at: {touch.position} - Phase: {touch.phase}");
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0; // Keep it on the same plane

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        GoUp();
                        Debug.Log("Start Clicking");
                    }
                    break;

                case TouchPhase.Moved:

                    hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject && !isUp) 
                    {
                        GoUp();
                        Debug.Log("Not Clicked");
                    }

                    if ((hit.collider == null || hit.collider.gameObject != gameObject) && isUp)
                    {
                        GoDown();
                        Debug.Log("Not Clicked");
                    }
                    break;

                case TouchPhase.Ended:
                    Debug.Log("Ended");

                    hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        GoDown();

                        if(missionSO != null)
                            if (missionSO.missionPrefab != null)
                            {  
                                Instantiate(missionSO.missionPrefab);
                                triangleParent.gameObject.SetActive(false);
                            }
                                

                        Debug.Log("Clicked");
                    }

                    break;
            }
        }

    }

    void GoUp()
    {
        isUp = true;
        Vector3 moveDirection = transform.up * moveDistance; // Move in local up direction

        transform.DOMove(startPosition - moveDirection, moveDuration)
            .SetEase(easingType); // Once finished, call GoDown
    }

    void GoDown()
    {
        Vector3 moveDirection = transform.up * moveDistance; // Move in local up direction

        transform.DOMove(startPosition, moveDuration)
            .SetEase(easingType).OnComplete(() => isUp = false);
    }
}

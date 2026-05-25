using UnityEngine;

public class SadnessElevator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private float fillSpeed = 1f;
    [SerializeField] private float drainSpeed = 0.5f;

    private float fillAmount = 0f;
    private bool isSad = false;
    private bool playerInside = false;
    private Vector3 startPos;
    private GameObject activeUI;

    public GameObject elevatorUIPrefab; // Prefab

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void OnEnable()
    {
        GameplayFaceInput.OnEmotionChanged += HandleEmotion;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnEmotionChanged -= HandleEmotion;
    }

    private void HandleEmotion(Emotion e)
    {
        isSad = (e == Emotion.Sad);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (isSad)
            fillAmount += fillSpeed * Time.deltaTime;
        else
            fillAmount -= drainSpeed * Time.deltaTime;

        fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);

        float y = Mathf.Lerp(0f, maxHeight, fillAmount);
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            other.transform.SetParent(transform);

            other.GetComponent<PlayerController>().IsOnMovingPlatform = true;
            if (activeUI == null)
            {
                Canvas canvas = GameObject.FindGameObjectWithTag("GameplayCanvas").GetComponent<Canvas>();

                activeUI = Instantiate(elevatorUIPrefab, canvas.transform);

                // Elevator referenzieren
                activeUI.GetComponent<ElevatorUI>().SetElevator(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            other.transform.SetParent(null);

            other.GetComponent<PlayerController>().IsOnMovingPlatform = false;
            if (activeUI != null)
            {
                Destroy(activeUI);
                activeUI = null;
            }
        }
    }
    public float GetFillAmount()
    {
        return fillAmount;
    }


}
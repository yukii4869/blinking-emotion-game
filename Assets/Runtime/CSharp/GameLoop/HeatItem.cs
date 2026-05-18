using UnityEngine;

public class HeatItem : PickupItem
{
    [Header("Heat Settings")]
    public float heatRate = 2f;
    public float maxHeat = 100f;
    public float currentHeat = 0f;

    private bool isAngry = false;

    [Header("UI")]
    private GameObject heatUIPrefab;
    private HeatUI heatUIInstance;

    private void OnEnable()
    {
        GameplayFaceInput.OnEmotionChanged += HandleEmotion;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnEmotionChanged -= HandleEmotion;
    }

    private void Start()
    {
        var prefab = Resources.Load<GameObject>("UI/HeatUI");

        if (prefab != null)
        {
            GameObject ui = Instantiate(prefab, transform);
            ui.transform.localPosition = new Vector3(0, 1.2f, 0); // UI über dem Item
            ui.transform.localRotation = Quaternion.identity;

            heatUIInstance = ui.GetComponent<HeatUI>();
            heatUIInstance.Initialize(this);
            ui.SetActive(false);
        }
    }


    private void HandleEmotion(Emotion e)
    {
        isAngry = (e == Emotion.Angry);
    }

    private void Update()
    {
        if (IsHeld && isAngry)
        {
            currentHeat += Time.deltaTime * heatRate;
            currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);
        }

        if (heatUIInstance != null)
            heatUIInstance.gameObject.SetActive(IsHeld);
    }

    public bool IsFullyHeated => currentHeat >= maxHeat;
}

using UnityEngine;

public class HeatItem : PickupItem, ICondition
{
    [Header("Heat Settings")]
    public float heatRate = 2f;
    public float coolRate = 1f;
    public float maxHeat = 100f;
    public float minHeat = 10f;
    public float currentHeat = 20f;

    [Header("UI")]
    private HeatUI heatUIInstance;

    public bool IsHeated { get; private set; }
    public bool IsBroken { get; private set; }
    public bool IsMet => IsHeated && !IsBroken;
    public string Description => "Item muss erhitzt sein";

    private bool isAngry = false;

    public event System.Action OnHeatCompleted;
    public event System.Action OnHeatBroken;
    private void OnDestroy()
    {
        InputSelector.Instance.ActiveInput.OnEmotionChanged -= HandleEmotion;
    }

    private void Start()
    {
        InputSelector.Instance.ActiveInput.OnEmotionChanged += HandleEmotion;
        var prefab = Resources.Load<GameObject>("UI/HeatUI");

        if (prefab != null)
        {
            uiInstance = Instantiate(prefab, UIManager.Instance.itemUI.transform);
            uiInstance.transform.localPosition = new Vector3(0, -1, 0);
            uiInstance.transform.localRotation = Quaternion.identity;

            heatUIInstance = uiInstance.GetComponent<HeatUI>();
            heatUIInstance.Initialize(this);
        }
    }

    private void Update()
    {
        // UI nur anzeigen wenn Item gehalten 
        if (heatUIInstance != null)
            heatUIInstance.gameObject.SetActive(IsHeld);

        // Wenn kaputt oder fertig → NICHTS mehr berechnen
        if (IsBroken || IsHeated)
            return;

        HandleHeatLogic();
    }

    private void HandleHeatLogic()
    {
        if (IsInResetZone)
        {
            currentHeat = 20f;
            IsBroken = false;
            IsHeated = false;
            return;
        }
        // Erhitzen
        if (IsHeld && isAngry)
        {
            currentHeat += Time.deltaTime * heatRate;
        }
        // Abkühlen
        else if (IsHeld)
        {
            currentHeat -= Time.deltaTime * coolRate;
        }

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        // Kaputt
        if (currentHeat <= minHeat)
        {
            Debug.Log("currentHeat < minHeat");
            IsBroken = true;
            OnHeatBroken?.Invoke();
            return;
        }

        // Fertig
        if (currentHeat >= maxHeat)
        {
            IsHeated = true;
            AudioManager.Instance.PlaySFX("correctSound");
            OnHeatCompleted?.Invoke();
        }
    }

    private void HandleEmotion(Emotion e)
    {
        isAngry = e == Emotion.Angry;
    }
    public void ResetFood()
    {

        currentHeat = 20f;
        IsBroken = false;
        IsHeated = false;

        isAngry = false;

        if (heatUIInstance != null)
            heatUIInstance.Initialize(this);
    }
}

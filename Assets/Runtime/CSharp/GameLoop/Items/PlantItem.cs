using UnityEngine;

public class PlantItem : PickupItem, ICondition
{
    [Header("Bloom Settings (FAST)")]
    public float maxBloom = 10f;
    public float currentBloom = 5f;

    public float bloomRate;     // wenn lächeln
    public float decayRate;     // wenn nicht lächeln

    [Header("Sweet Spot")]
    public float sweetSpotMin = 4f;
    public float sweetSpotMax = 7f;

    [Header("Out of Range Timer")]
    public float outOfRangeLimit = 2f;   // 2 Sekunden außerhalb → tot
    [HideInInspector] public float outOfRangeTimer = 0f;

    [Header("Visuals")]
    public ParticleSystem particles;
    public Renderer plantRenderer;

    [Header("Colors")]
    public Color perfectColor = new Color(0.6f, 1f, 0.6f);
    public Color wiltColor = new Color(0.2f, 0.3f, 0.2f);
    public Color overgrownColor = new Color(1f, 0.8f, 0.2f);
    public Color deadColor = Color.black;

    public float fadeSpeed = 25f;

    private ParticleSystem.MainModule main;

    private bool isHappy = false;

    public bool IsPerfect { get; private set; }
    public bool IsWilted { get; private set; }
    public bool IsOvergrown { get; private set; }
    public bool IsDead { get; private set; }

    // ICondition
    public bool IsMet => IsPerfect && !IsDead;
    public string Description => "Pflanze muss im Sweet Spot bleiben";

    // UI
    private PlantUI plantUIInstance;

    private void Start()
    {
        main = particles.main;
        particles.Play();

        GameplayFaceInput.OnEmotionChanged += HandleEmotion;

        // UI instanziieren
        var prefab = Resources.Load<GameObject>("UI/PlantUI");
        if (prefab != null)
        {
            GameObject ui = Instantiate(prefab, transform);
            ui.transform.localPosition = new Vector3(0, 0.15f, 0);
            ui.transform.localRotation = Quaternion.identity;

            plantUIInstance = ui.GetComponent<PlantUI>();
            plantUIInstance.Initialize(this);
        }
    }

    private void OnDestroy()
    {
        GameplayFaceInput.OnEmotionChanged -= HandleEmotion;
    }

    private void HandleEmotion(Emotion e)
    {
        if (IsDead) return;
        isHappy = (e == Emotion.Happy);
        Debug.Log("Emotion:" + isHappy);
    }

    private void Update()
    {
        if (plantUIInstance != null)
            plantUIInstance.gameObject.SetActive(IsHeld);

        if (!IsHeld || IsDead) return;

        UpdateBloom();
        UpdateSweetSpotTimer();
        UpdateVisuals();
    }

    private void UpdateBloom()
    {
        if (isHappy)
        {
            Debug.Log("HAPPY BLOOM");
            currentBloom += bloomRate * Time.deltaTime;
        }

        else
            currentBloom -= decayRate * Time.deltaTime;
        
        currentBloom = Mathf.Clamp(currentBloom, 0, maxBloom);
        Debug.Log("CurrentBloom" + currentBloom);
    }

    private void UpdateSweetSpotTimer()
    {
        if (IsDead) return;

        bool inSweetSpot = currentBloom >= sweetSpotMin && currentBloom <= sweetSpotMax;

        if (inSweetSpot)
        {
            // Reset Timer
            outOfRangeTimer = 0f;

            IsPerfect = true;
            IsWilted = false;
            IsOvergrown = false;
        }
        else
        {
            IsPerfect = false;

            if (currentBloom < sweetSpotMin)
            {
                IsWilted = true;
                IsOvergrown = false;
            }
            else
            {
                IsOvergrown = true;
                IsWilted = false;
            }

            // Timer läuft hoch
            outOfRangeTimer += Time.deltaTime;

            if (outOfRangeTimer >= outOfRangeLimit)
                Die();
        }
    }

    private void UpdateVisuals()
    {
        Color target;

        if (IsDead)
            target = deadColor;
        else if (IsPerfect)
            target = perfectColor;
        else if (IsWilted)
            target = wiltColor;
        else
            target = overgrownColor;

        // Pflanze färben
        plantRenderer.material.color = Color.Lerp(
            plantRenderer.material.color,
            target,
            Time.deltaTime * fadeSpeed
        );

        // Partikel färben
        var col = particles.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(target, 0f),
                new GradientColorKey(target, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );

        col.color = grad;
    }

    private void Die()
    {
        IsDead = true;
        plantRenderer.material.color = deadColor;
        main.startColor = deadColor;
        particles.Stop();
    }
}

using UnityEngine;

public class CandleItem : PickupItem, ICondition
{
    [Header("Visuals")]
    [SerializeField] private GameObject flame;


    private bool extinguished = false;

    public bool IsMet => !extinguished;
    public string Description => "Nicht blinzeln!";

    private void OnEnable()
    {
        GameplayFaceInput.OnBlink += HandleBlink;
    }

    private void OnDisable()
    {
        GameplayFaceInput.OnBlink -= HandleBlink;
    }

    private void HandleBlink()
    {
        // Kerze geht NUR aus, wenn sie gehalten wird
        if (IsHeld)
        {
            extinguished = true;
            flame.SetActive(false);


        }
    }
    public void Relight()
{
    if (!extinguished)
        return;

    extinguished = false;

    flame.SetActive(true);

}
}

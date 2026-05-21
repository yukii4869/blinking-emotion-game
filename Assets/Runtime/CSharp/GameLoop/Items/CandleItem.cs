public class CandleItem : PickupItem, ICondition
{
    private BlinkCondition blinkCondition;

    public bool IsMet => blinkCondition.IsMet;
    public string Description => blinkCondition.Description;

    public bool IsExtinguished => !blinkCondition.IsMet;

    private void Start()
    {
        blinkCondition = new BlinkCondition();
    }
}

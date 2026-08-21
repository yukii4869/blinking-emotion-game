using UnityEngine;

public class ResetZone : MonoBehaviour
{
private void OnTriggerEnter(Collider other)
{
    var item = other.GetComponentInChildren<PickupItem>();
    if (item == null) return;

    item.IsInResetZone = true;

    // Sofort resetten
    if (item is CandleItem candle)
        candle.Relight();

    if (item is PlantItem plant)
        plant.ResetPlant();

    if (item is HeatItem food)
        food.ResetFood();
}

private void OnTriggerExit(Collider other)
{
    var item = other.GetComponentInChildren<PickupItem>();
    if (item == null) return;

    item.IsInResetZone = false;
}

}
using UnityEngine;

public class ResetZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CandleItem candle = other.GetComponentInChildren<CandleItem>();
        PlantItem plant = other.GetComponentInChildren<PlantItem>();
        HeatItem food = other.GetComponentInChildren<HeatItem>();

        if (candle != null && candle.IsHeld)
        {
            candle.Relight();
        }
        if (plant != null && plant.IsHeld)
        {
            plant.ResetPlant();
        }
        if (food != null && food.IsHeld)
        {
            food.ResetFood();
        }
    }
}
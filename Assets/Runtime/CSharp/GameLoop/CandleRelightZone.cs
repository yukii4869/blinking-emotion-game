using UnityEngine;

public class CandleRelightZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CandleItem candle = other.GetComponentInChildren<CandleItem>();

        if (candle != null && candle.IsHeld)
        {
            candle.Relight();
        }
    }
}
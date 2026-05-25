using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour
{
    private SadnessElevator elevator;
    public Image fillImage;

    public void SetElevator(SadnessElevator e)
    {
        elevator = e;
    }

    private void Update()
    {
        if (elevator == null) return;

        float fill = elevator.GetFillAmount();
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, fill, Time.deltaTime * 8f);
    }
}

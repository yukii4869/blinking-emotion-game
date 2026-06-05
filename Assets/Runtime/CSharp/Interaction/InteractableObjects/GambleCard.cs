using UnityEngine;

public class GambleCard : MonoBehaviour, IInteractable
{
    public GambleGuest owner;
    [SerializeField] private int cardIndex;

    public void Interact()
    {
        owner.OnCardChosen(cardIndex);
    }
}
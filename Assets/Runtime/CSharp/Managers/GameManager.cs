using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {

        GameStateManager.Instance.SetState(GameState.Initialze);
        if (GameStateManager.Instance.CurrentState == GameState.Initialze)
        {
            Debug.Log("StartInit");
            OnInitialize();
        }
    }
    void Update()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Initialze)
        {
            Debug.Log("StartInit");
            OnInitialize();
        }
    }
    public void OnInitialize()
    {
        StartCoroutine(DelayedInit());
    }
    private IEnumerator DelayedInit()
    {
        Debug.Log("IN ENUM");
        yield return null; // 1 Frame warten
        RoomManager.instance.InitializeRooms();
        yield return new WaitForSeconds(0.2f);
        DeliveryManager.Instance.InitializeDelivery();
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
    public void OnPause()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
        {
            Time.timeScale = 0;

            GameStateManager.Instance.SetState(GameState.Pause);
        }
    }
    public void OnResume()
    {
        if (GameStateManager.Instance.CurrentState == GameState.Pause)
        {
            Time.timeScale = 1f;
            GameStateManager.Instance.SetState(GameState.Gameplay);
        }

    }
}
using UnityEngine;
using TMPro;
using System.Collections;

public class FinishCalibrationUI : MonoBehaviour
{
    public static FinishCalibrationUI Instance;

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private TextMeshProUGUI statusTxt;
    [SerializeField] private Typewriter typewriter;

    private void Awake()
    {
        Instance = this;
        fadeGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void StartSequence(string playerName)
    {
        gameObject.SetActive(true);
        StartCoroutine(RunSequence(playerName));
    }

    private IEnumerator RunSequence(string playerName)
    {
        statusTxt.text = "";

        yield return StartCoroutine(typewriter.TypeText(statusTxt, "IDENTITÄT BESTÄTIGT"));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(typewriter.TypeText(statusTxt, "WILLKOMMEN ZURÜCK"));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(typewriter.TypeText(statusTxt, playerName.ToUpper()));
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(typewriter.TypeText(statusTxt, "IHRE SCHICHT HAT BEREITS BEGONNEN"));
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(typewriter.TypeText(statusTxt, "SCHLIESSEN SIE DIE AUGEN"));
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeToBlack());

        GameSceneManager.Instance.LoadGame();
    }

    private IEnumerator FadeToBlack()
    {
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            fadeGroup.alpha = t;
            yield return null;
        }
        fadeGroup.alpha = 1;
    }
}

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class FinishCalibration : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI statusTxt;
    [SerializeField] private Typewriter typewriter;

    private bool waitingForEyes = false;
    private bool eyesConfirmed = false;

    private void OnEnable()
    {
          CalibrationFaceInput.Instance.OnEyesClosedHold += HandleEyesClosedHold;
          StartCoroutine(RunSequence(ActiveProfile.Instance.CurrentProfile.playerName));
    }

    private void OnDisable()
    {
        CalibrationFaceInput.Instance.OnEyesClosedHold -= HandleEyesClosedHold;
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

        waitingForEyes = true;

        while (!eyesConfirmed)
            yield return null;

        yield return StartCoroutine(FadeToBlack());
        GameSceneManager.Instance.LoadGame();
        AudioManager.Instance.PlaySFX("elevatorBing");
        AudioManager.Instance.PlaySFX("elevatorOpen");
    }

    private void HandleEyesClosedHold()
    {
        if (!waitingForEyes)
            return;

        eyesConfirmed = true;
    }

    private IEnumerator FadeToBlack()
    {
        Color c = fadeImage.color;

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            c.a = t;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1;
        fadeImage.color = c;
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class Typewriter : MonoBehaviour
{
    public float charsPerSecond = 40f;

    public IEnumerator TypeText(TextMeshProUGUI textField, string fullText)
    {
        textField.text = "";
        float delay = 1f / charsPerSecond;

        foreach (char c in fullText)
        {
            textField.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}

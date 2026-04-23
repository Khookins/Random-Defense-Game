using System.Collections;
using TMPro;
using UnityEngine;

public class Label : MonoBehaviour
{
    private TMP_Text text;
    Color originalColor;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        originalColor = text.color;
    }

    public void UpdateText(string value)
    {
        text.text = value;
    }

    public void FlashColor(Color color, float flashTime)
    {
        StopAllCoroutines();
        StartCoroutine(FlashColorInternal(color, flashTime));
    }

    private IEnumerator FlashColorInternal(Color startColor, float flashTime)
    {
        float elapsed = 0f;

        while (elapsed < flashTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashTime;

            text.color = Color.Lerp(startColor, originalColor, t);

            yield return null;
        }

        text.color = originalColor;
    }
}

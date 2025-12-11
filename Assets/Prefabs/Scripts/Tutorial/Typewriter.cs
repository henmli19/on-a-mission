using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox;
    [SerializeField, TextArea] private string fullText;
    [SerializeField] private float typingSpeed = 0.05f;

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        textBox.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            textBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
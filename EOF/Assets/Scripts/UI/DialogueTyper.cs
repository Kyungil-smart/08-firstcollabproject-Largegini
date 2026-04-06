using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class DialogueTyper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float charDelay = 0.05f;

    private Coroutine _typingCoroutine;
    private bool _isTyping = false;
    private string _fullText = "";
    private readonly StringBuilder _sb = new StringBuilder();

    public void ShowText(string text)
    {
        _fullText = text;
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        _typingCoroutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        _sb.Clear();
        dialogueText.text = "";

        foreach (char c in text)
        {
            _sb.Append(c);
            dialogueText.text = _sb.ToString();
            yield return new WaitForSeconds(charDelay);
        }

        _isTyping = false;
    }

    public void OnClickDialogue()
    {
        if (_isTyping)
        {
            StopCoroutine(_typingCoroutine);
            dialogueText.text = _fullText;
            _isTyping = false;
        }
        else
        {
            
        }
    }
}
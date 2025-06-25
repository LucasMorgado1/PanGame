using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TutorialText : MonoBehaviour
{
    private bool fadeIsComplete = true;
    private bool _once = false; //bool to not stack calling the function WaitForFadeEnds
    private TextMeshProUGUI _text;

    public delegate IEnumerator CallFunction();
    public CallFunction _callFunction;
    public static event CallFunction _delegateEvent;

    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        //_text.color = new Color (1,1,1,0.15f);
    }

    private void SetListener(CallFunction eventFunction)
    {
        _callFunction = eventFunction;
        _delegateEvent += eventFunction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && fadeIsComplete)
        {
            StartCoroutine(FadeIn());
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && fadeIsComplete)
        {
            StartCoroutine(FadeOut());
        }

        if (collision.CompareTag("Player") && !fadeIsComplete && !_once)
        {
            _once = true;
            SetListener(FadeOut);
            StartCoroutine(WaitForFadeEnds(fadeIsComplete));
        }
    }

    IEnumerator FadeIn ()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        if (_text.color.a >= 0.9f)
        {
            yield break;
        }

        fadeIsComplete = false;

        for (float i = 0; i <= 1f; i += Time.deltaTime)
        {
            _text.color = new Color(1, 1, 1, i);
            yield return null;
        } 

        if (_text.color.a >= 0.9)
        {
            _text.color = new Color(1,1,1,1);
        }
        fadeIsComplete = true;
    }

    IEnumerator FadeOut ()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        if (_text.color.a <= 0.05f)
        {
            yield break; 
        }

        fadeIsComplete = false;

        for (float i = 1; i >= 0f; i -= Time.deltaTime)
        {
            _text.color = new Color(1, 1, 1, i);
            yield return null;
        }

        if (_text.color.a >= 0.05)
        {
            _text.color = new Color(1, 1, 1, 0);
        }

        fadeIsComplete = true;
    }

    IEnumerator WaitForFadeEnds (bool fade)
    {
        if (!this.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        while (fadeIsComplete == false)
        {
            yield return null;
        }

        StartCoroutine(_delegateEvent());
        _once = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollesiumUI : MonoBehaviour      
{
    private static CollesiumUI _instance;
    public static CollesiumUI instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<CollesiumUI>();
            return _instance;
        }
    }

    [SerializeField] TextMeshProUGUI subtitleText = default;
    // Start is called before the first frame update
    private void Awake()
    {
        clear();
    }
    public void SetSubtitle(string subtitle, float delay)
    {
        //subtitleText.text = subtitle;
        StartCoroutine(thesequence());
        StartCoroutine(ClearAfterSecond(20));
    }
    public void clear()
    {
        subtitleText.text = "";
    }
    private IEnumerator ClearAfterSecond(float delay)
    {
        yield return new WaitForSeconds(88);
        clear();
    }
    IEnumerator thesequence()
    {
        yield return new WaitForSeconds(1);
        subtitleText.text = "Show is about the begin";
        yield return new WaitForSeconds(2);
        subtitleText.text = "";
    }

}

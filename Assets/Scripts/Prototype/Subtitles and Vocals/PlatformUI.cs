using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlatformUI : MonoBehaviour
{
    private static PlatformUI _instance;
    public static PlatformUI instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<PlatformUI>();
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
        yield return new WaitForSeconds(0);
        subtitleText.text = "Don't fear,We are on the getting high";
        yield return new WaitForSeconds(4);
        subtitleText.text = "";
        GameObject.Find("Gradient").SetActive(false);
    }

}

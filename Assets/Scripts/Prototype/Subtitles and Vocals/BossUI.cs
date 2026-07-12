using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossUI : MonoBehaviour
{
    private static BossUI _instance;
    public static BossUI instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<BossUI>();
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
        subtitleText.text = "Let's see the kaizers big boy";
        yield return new WaitForSeconds(3);
        subtitleText.text = "";
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitRiverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleText = default;
    // Start is called before the first frame update
    public static ExitRiverUI instance;
    private void Awake()
    {
        instance = this;
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
        subtitleText.text = "Finally! Edge of the Water.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "";
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AfterRiver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleText = default;
    // Start is called before the first frame update
    public static AfterRiver instance;
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
        subtitleText.text = "The first user of the Kaizer ";
        yield return new WaitForSeconds(2);
        subtitleText.text = "and this unquenchable fire";
        yield return new WaitForSeconds(2);
        subtitleText.text = "founded Köningsfort.";
        yield return new WaitForSeconds(2);
        subtitleText.text = "Of course,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "the biggest share belonged It.";
        yield return new WaitForSeconds(2);
        subtitleText.text = "Kaizer's power combined with the first fire";
        yield return new WaitForSeconds(3);
        subtitleText.text = "was enough to create this breathless";
        yield return new WaitForSeconds(3);
        subtitleText.text = "but also magnificent city,";
        yield return new WaitForSeconds(3);
        subtitleText.text = "but the Kaizer who claimed that to ";
        yield return new WaitForSeconds(3);
        subtitleText.text = "people’s ability of decision.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "Years passed,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "rulers changed,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "and life got worse with each change.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "Now, this huge city has become a cage";
        yield return new WaitForSeconds(3);
        subtitleText.text = "for its inhabitants.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "";
        GameObject.Find("Gradient").SetActive(false);
    }
}

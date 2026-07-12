using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AfterFall : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleText = default;
    // Start is called before the first frame update
    public static AfterFall instance;
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
        subtitleText.text = "Before facing on Khan,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "there are some important points";
        yield return new WaitForSeconds(2);
        subtitleText.text = "you should know about the kaizer and the city.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "Kaizer is not the only element that";
        yield return new WaitForSeconds(2);
        subtitleText.text = "has ensured the continuity of K�ningsfort";
        yield return new WaitForSeconds(2);
        subtitleText.text = "for centuries.";
        yield return new WaitForSeconds(1);
        subtitleText.text = "There is also the first fire, ";
        yield return new WaitForSeconds(3);
        subtitleText.text = "which gives life to the creatures";
        yield return new WaitForSeconds(2);
        subtitleText.text = "in the city for a limited time,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "but in the end,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "every born thing with fire ";
        yield return new WaitForSeconds(2);
        subtitleText.text = "turns back to it";
        yield return new WaitForSeconds(2);
        subtitleText.text = "if it is worthy.";
        yield return new WaitForSeconds(2);
        subtitleText.text = "";
        GameObject.Find("Gradient").SetActive(false);
    }

}

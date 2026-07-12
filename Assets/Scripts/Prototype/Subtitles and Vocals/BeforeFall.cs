using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeforeFall : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleText = default;
    // Start is called before the first frame update
    public static BeforeFall instance;
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
        subtitleText.text = "You are now in a place";
        yield return new WaitForSeconds(2);
        subtitleText.text = "where pain will follow you ";
        yield return new WaitForSeconds(2);
        subtitleText.text = "like a shadow on the streets.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "but you should know that ";
        yield return new WaitForSeconds(2);
        subtitleText.text = "he is a khan who owes all his power to the Kaizer,";
        yield return new WaitForSeconds(5);
        subtitleText.text = "by the way";
        yield return new WaitForSeconds(1);
        subtitleText.text = "do you afraid of heights?";
        yield return new WaitForSeconds(3);
        subtitleText.text = "";
        GameObject.Find("Gradient").SetActive(false);
    }
    
      
    
     
}

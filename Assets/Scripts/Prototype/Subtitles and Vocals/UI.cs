using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI subtitleText = default;
    // Start is called before the first frame update
    public static UI instance;
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
        subtitleText.text = "Kaizer: Yes, I can feel you boy.";
        yield return new WaitForSeconds(5);
        subtitleText.text = "You're the one coming out of the Maze.";
        yield return new WaitForSeconds(4);
        subtitleText.text = "How a pity child can find a way";
        yield return new WaitForSeconds(3);
        subtitleText.text = "to break our Realm I wonder?";
        yield return new WaitForSeconds(3);
        subtitleText.text = "...";
        yield return new WaitForSeconds(1);
        subtitleText.text = "Maybe he is not a regular soul,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "maybe he is the new one. ";
        yield return new WaitForSeconds(2);
        subtitleText.text = "I KNOW WHO YOU ARE ";
        yield return new WaitForSeconds(2);
        /*subtitleText.text = "...";
        yield return new WaitForSeconds(1);*/
        subtitleText.text = "Chains...";
        yield return new WaitForSeconds(2);
        subtitleText.text = "I searched for you for many years.";
        yield return new WaitForSeconds(4);
        subtitleText.text = "I can't tell you why,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "but you will know eventually.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "Know that boy, your morals are";
        yield return new WaitForSeconds(2);
        subtitleText.text = "NO MORE THAN CHAINS.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "I will break yours,";
        yield return new WaitForSeconds(3);
        subtitleText.text = "make you free.";
        yield return new WaitForSeconds(4);
        subtitleText.text = "Come,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "come to me.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "...";
        yield return new WaitForSeconds(1);
        subtitleText.text = "It's all about passion.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "All about the claim.";
        yield return new WaitForSeconds(2);
        subtitleText.text = "AND IF THERE IS A CLAIM,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "IT WAS ALWAYS ME WHO WON IT!";
        yield return new WaitForSeconds(4);
        subtitleText.text = "Even if no one knows";
        yield return new WaitForSeconds(2);
        subtitleText.text = "or sees.";
        yield return new WaitForSeconds(2);
        subtitleText.text = "...";
        yield return new WaitForSeconds(3);
        subtitleText.text = "My claim once was a throne and a helmet.";
        yield return new WaitForSeconds(4);
        subtitleText.text = "Now it's nothing,";
        yield return new WaitForSeconds(2);
        subtitleText.text = "nothing but you.";
        yield return new WaitForSeconds(3);
        subtitleText.text = "";
        
    }
    
}

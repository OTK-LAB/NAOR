using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutScript : MonoBehaviour
{
    public Image image;
    public float time = 2f;
    bool fadeout = true;


    // Update is called once per frame
    void Update()
    {
        if (fadeout)
        {
            StartCoroutine(FadeOut());
                
        }
    }



    public IEnumerator FadeOut()
    {
        Time.timeScale = 1.0f;
        var tempColor = image.color;
        for (float f = 0; f <= 2; f += Time.deltaTime)
        {
            tempColor = image.color;
            tempColor.a = Mathf.Lerp(1f, 0f, f / 2);
            image.color = tempColor;
            yield return null;
        }
        fadeout = false;
        tempColor = image.color;
        tempColor.a = 0;
        image.color = tempColor;
        yield return new WaitForSecondsRealtime(time);
    }

}

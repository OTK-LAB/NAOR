using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class TypeWriterEffect : MonoBehaviour {

	//public float delay = 0.1f;
	//public string fullText;
	//private string currentText = "";
	public float time=0.5f;
	// Use this for initialization
	void OnEnable() {
		

		StartCoroutine(Fade());
	}
	
	IEnumerator Fade()
    {
		
        for(float f = 0; f <= 2; f += Time.deltaTime)
        {
            this.GetComponent<TextMeshProUGUI>().alpha = Mathf.Lerp(0f, 1f, f / 2);
			yield return null;
        }
        this.GetComponent<TextMeshProUGUI>().alpha = 1;
		yield return new WaitForSecondsRealtime(time);
			for (float f = 0; f <= 2; f += Time.deltaTime)
			{
				this.GetComponent<TextMeshProUGUI>().alpha = Mathf.Lerp(1f, 0f, f / 2);
				yield return null;
			}
			this.GetComponent<TextMeshProUGUI>().alpha = 0;
	}
	
}

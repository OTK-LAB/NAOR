using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    public TypeWriterEffect typeWriterScript1;
    public TypeWriterEffect typeWriterScript2;
    AsyncOperation async;
    int index=0;
    //public Transform[] allChildren;
    //List<GameObject> childObjects = new List<GameObject>();
    void OnEnable()
    {
        StartCoroutine(PrintLines());
        /*allChildren = GetComponentsInChildren<Transform>(true);
        
        foreach (Transform child in allChildren)
        {
            childObjects.Add(child.gameObject);
        }*/

    }
  
    // Update is called once per frame
  
    private void Update() 
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene(1);
        }
    
    }

        

       // else if (index == 5)
        //{
          //  transform.GetChild(index).gameObject.SetActive(false);
            //index++;
        //}
       
       
        

        /* if (Input.anyKeyDown)
         {
             childObjects[index].active = false;
             if (index <allChildren.Length-1)
             {          
                     childObjects[index].SetActive(true);             
             }
         }
     }*/
    
    /*IEnumerator FadeOut()
    {
        for (float f = 0; f <= 2; f += Time.deltaTime)
        {
            this.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, f / 2);
            yield return null;
        }
        this.GetComponent<CanvasGroup>().alpha = 0;
       for (float f = 0; f <= 2; f += Time.deltaTime)
        {
            this.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, f / 2);
            yield return null;
        }
        this.GetComponent<CanvasGroup>().alpha = 1;
        async.allowSceneActivation = true;
    }
*/

    IEnumerator PrintLines()
    {
        int time;
        while(index <= 5)
        {
            time = 4;
            if (index == 4)
            {
                //typeWriterScript = transform.GetChild(index).gameObject.GetComponent<TypeWriterEffect>();
                typeWriterScript1.time = 4f;
                transform.GetChild(index).gameObject.SetActive(true);
                
                yield return new WaitForSecondsRealtime(8);
                //transform.GetChild(index).gameObject.SetActive(false);
                index++;
            }

            else if (index == 5)
            {
                //typeWriterScript = transform.GetChild(index).gameObject.GetComponent<TypeWriterEffect>();
                typeWriterScript2.time = 5f;
                transform.GetChild(index).gameObject.SetActive(true);
                index++;
                async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); //Loads the next scene in line
                async.allowSceneActivation = false;
                yield return new WaitForSecondsRealtime(10);
                async.allowSceneActivation = true;
               // StartCoroutine(FadeOut());
            }
            // if (index == 3) time = 4;
            //else if (index == 4) time = 5;
            //else if (index == 5) time = 7;
            else
            {
                transform.GetChild(index).gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(time);
                //transform.GetChild(index).gameObject.SetActive(false);
                index++;
            }
            

            // transform.GetChild(index).gameObject.SetActive(true);

            
        }

        //time = 3;
        //yield return new WaitForSecondsRealtime(time);
        
        


    }
}




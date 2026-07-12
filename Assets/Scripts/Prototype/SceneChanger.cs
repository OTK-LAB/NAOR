using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public PlayerController player;
    //public Image siyahEkran;

    [Header("Locks")]
    public string keyToThisDoor;                //kilitli kap� i�in bunlar
    public string requiredKey;                  //anahtara sahip oldu�unu tek tek playerpref'e yaz�caz gibi...
    public bool locked;                         //�imdilik kilitlerle u�ra�m�yorum

    [Header("Door Type")]
    public bool leftDoor;
    public bool rightDoor;
    public bool ceilingDoor;

    [Header("Next scene details")]
    public string nextSceneName;
    public float nextScenePlayerPositionX = 0;
    public float nextScenePlayerPositionY = 0;

    private bool buttonEpressed = false;
    private bool playerHere;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //siyahEkran = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<Image>();

        //StartCoroutine(WelcomeToScene());

        if (PlayerPrefs.GetFloat("usedLeftDoor") == 1)
          player.transform.position = new Vector2(PlayerPrefs.GetFloat("nextLeftPlayerPositionX"), PlayerPrefs.GetFloat("nextLeftPlayerPositionY"));

        if(PlayerPrefs.GetFloat("usedRightDoor") == 1)
            player.transform.position = new Vector2(PlayerPrefs.GetFloat("nextRightPlayerPositionX"), PlayerPrefs.GetFloat("nextRightPlayerPositionY"));

        if(PlayerPrefs.GetFloat("usedCeilingDoor") == 1)
            player.transform.position = new Vector2(PlayerPrefs.GetFloat("nextCeilingPlayerPositionX"), PlayerPrefs.GetFloat("nextCeilingPlayerPositionY"));

        PlayerPrefs.SetFloat("usedLeftDoor", 0);
        PlayerPrefs.SetFloat("usedRightDoor", 0);
        PlayerPrefs.SetFloat("usedCeilingDoor", 0);

        playerHere = false;
    }

    private void Update()
    {
        CheckInputs();

        if (requiredKey == keyToThisDoor)
            locked = false;
    }
    void CheckInputs()
    {
        if (Input.GetKeyDown(KeyCode.E))
            buttonEpressed = true;
        else if (Input.GetKeyUp(KeyCode.E))
            buttonEpressed = false;
    }

    /*public IEnumerator WelcomeToScene()
    {
        for (float f = 0; f <= 2; f += Time.deltaTime)
        {
            Color newColor = siyahEkran.color;
            newColor.a = Mathf.Lerp(1f, 0f, f / 2); ;
            siyahEkran.color = newColor;
            yield return null;
        }
    }*/

    IEnumerator ChangeTheScene(string nextscene)
    {
        /*for (float f = 0; f <= 2; f += Time.deltaTime)
        {
            Color newColor = siyahEkran.color;
            newColor.a = Mathf.Lerp(0f, 1f, f / 2); ;
            siyahEkran.color = newColor;
            yield return null;
        }*/
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(nextscene);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerHere = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerHere && !locked)
        {
            if (leftDoor && buttonEpressed)
            { 
                PlayerPrefs.SetFloat("nextLeftPlayerPositionX", nextScenePlayerPositionX);
                PlayerPrefs.SetFloat("nextLeftPlayerPositionY", nextScenePlayerPositionY);

                PlayerPrefs.SetFloat("usedLeftDoor", 1);

                StartCoroutine(ChangeTheScene(nextSceneName));
            }

            if (rightDoor && buttonEpressed)
            { 
                PlayerPrefs.SetFloat("nextRightPlayerPositionX", nextScenePlayerPositionX);
                PlayerPrefs.SetFloat("nextRightPlayerPositionY", nextScenePlayerPositionY);

                PlayerPrefs.SetFloat("usedRightDoor", 1);

                StartCoroutine(ChangeTheScene(nextSceneName));
            }
        }

        if (ceilingDoor && playerHere) // no need for E press
        {
            PlayerPrefs.SetFloat("nextCeilingPlayerPositionX", nextScenePlayerPositionX);
            PlayerPrefs.SetFloat("nextCeilingPlayerPositionY", nextScenePlayerPositionY);

            PlayerPrefs.SetFloat("usedCeilingDoor", 1);

            StartCoroutine(ChangeTheScene(nextSceneName));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerHere = false;
    }
}

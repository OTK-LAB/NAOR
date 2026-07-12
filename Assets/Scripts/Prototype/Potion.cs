using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public GameObject[] potions;
    public int potionCount;
    public int maxpotionCount = 3;
    public bool control;
    private PlayerManager playerManager;
    public static Potion instance;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        
        potionCount = maxpotionCount;
        potions[3].gameObject.SetActive(false);
    }

    void Update()
    {
        control = GameObject.FindGameObjectWithTag("CheckPoint").GetComponent<CheckPointController>().checkpointReached;
        //checkpoint fonksiyonu
        //farklı scriptte çağırmak için
        //Potion.instance.CheckPoint();
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckPoint();            
        }
       
        //iksir ekleme fonksiyonu
        //farklı scriptte çağırmak için
        //Potion.instance.AddPotion();
        if(control){
            if (Input.GetKeyDown(KeyCode.M))
            {
                AddPotion();
            }
        }
    }

    public void UsePotions(int p)
    {
        if (potionCount >= 1)
        {   
            potionCount -= p;
            potions[potionCount].gameObject.SetActive(false);
            if (potionCount < 1)
            {
                Debug.Log("potions are over");
            }
        }

    }

    public void CheckPoint()
    {
        potionCount = maxpotionCount;
        potions[0].gameObject.SetActive(true);
        potions[1].gameObject.SetActive(true);
        potions[2].gameObject.SetActive(true);
        if (maxpotionCount == 4)
        {
            potions[3].gameObject.SetActive(true);
        }
    }

    
    public void AddPotion()
    {
        maxpotionCount = 4;
        potions[3].gameObject.SetActive(true);
        if (potionCount != 4)
        {
            potionCount = maxpotionCount;
            potions[0].gameObject.SetActive(true);
            potions[1].gameObject.SetActive(true);
            potions[2].gameObject.SetActive(true);
            potions[3].gameObject.SetActive(true);
        }
    }

}
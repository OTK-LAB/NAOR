using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerUI : MonoBehaviour
{
    public GameObject player;
    public GameObject dagger1;
    public GameObject dagger2;
    public GameObject dagger3;
    ItemStack daggerStack;
    // Start is called before the first frame update
    void Start()
    {
        daggerStack = player.GetComponent<PlayerController>().GetDaggerStack();
    }

    // Update is called once per frame
    void Update()
    {
        if(daggerStack.GetStack().Count == 0)
        {
            dagger1.SetActive(false);
            dagger2.SetActive(false);
            dagger3.SetActive(false);
        }
        if(daggerStack.GetStack().Count == 1)
        {
            dagger1.SetActive(true);
            dagger2.SetActive(false);
            dagger3.SetActive(false);
        }
        if(daggerStack.GetStack().Count == 2)
        {
            dagger1.SetActive(true);
            dagger2.SetActive(true);
            dagger3.SetActive(false);
        }
        if(daggerStack.GetStack().Count == 3)
        {
            dagger1.SetActive(true);
            dagger2.SetActive(true);
            dagger3.SetActive(true);
        }
    }
}

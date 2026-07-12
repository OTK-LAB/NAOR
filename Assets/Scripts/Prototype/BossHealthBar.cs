using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour

{
    public int maxHealth = 500;
    public float smoothing = 5;
    
    public BossHealthBar healthBar;
    public Slider slider;
    
    public static BossHealthBar instance;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {  
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (Boss_Manager.instance.health != slider.value)
        {
            slider.value = Mathf.Lerp(slider.value, Boss_Manager.instance.health, smoothing * Time.deltaTime);
        }       
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthBar : MonoBehaviour

{
    public int maxHealth = 500;
    public float smoothing = 5;
    
    public BossHealthBar healthBar;
    public Slider slider;
    
    public Boss_Manager bossManager;


    private void Awake()
    {
    }

    void Start()
    {  
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (bossManager != null && bossManager.health != slider.value)
        {
            slider.value = Mathf.Lerp(slider.value, bossManager.health, smoothing * Time.deltaTime);
        }       
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

}
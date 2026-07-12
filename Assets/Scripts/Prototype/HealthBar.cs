using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public PlayerManager playerManager;
    public Slider slider;
    public Image HealthImage;
    public Sprite RespawnHealthBar;
    public Sprite BackgroundImage3Gem; 
    public Sprite BackgroundImage2Gem;
    public Sprite BackgroundImage1Gem;
    public Sprite BackgroundImage0Gem;

    private void Start()
    {
        
    }
    private void Update()
    {
        if (playerManager.CurrentHealth != slider.value)
        {
            slider.value = Mathf.Lerp(slider.value, playerManager.CurrentHealth, 5 * Time.deltaTime);
        }
    }
    public void SetHealth(float health)
    {
         slider.value = health;
    }
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    public void DeathDefienceGem(int Gemcount)
    {
        if (Gemcount == 3)
        {
            HealthImage.sprite = BackgroundImage3Gem;
        }
        if (Gemcount == 2)
        {
            HealthImage.sprite = BackgroundImage2Gem;
        }
        if (Gemcount == 1)
        {
            HealthImage.sprite = BackgroundImage1Gem;
        }
        if (Gemcount == 0)
        {
            HealthImage.sprite = BackgroundImage0Gem;
        }
    }
    public void RevertHealthBar()
    {
        HealthImage.sprite = RespawnHealthBar;
    }
}  
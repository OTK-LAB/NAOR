using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private static StaminaBar _instance;
    public static StaminaBar instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<StaminaBar>();
            return _instance;
        }
    }

    public Slider staminaBar;

    private float maxStamina = 100;
    public float currentStamina;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    public float smoothing = 5;
    public float staminaIncreasingSpeed = 1;
    private void Awake()
    {
    }
    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }
    void Update()
    {
        if (currentStamina != staminaBar.value)
        {
            staminaBar.value = Mathf.Lerp(staminaBar.value, currentStamina, smoothing * Time.deltaTime);
        }
        //stamina kullanma fonksiyonu
        //başka scriptte çağırmak için 
        //StaminaBar.instance.useStamina(12);
        if(currentStamina >= 100)
            currentStamina = 100;
    }
    public void useStamina(float amount)
    {
        if (currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            //  staminaBar.value = currentStamina;

            if (regen != null)
            {
                StopCoroutine(regen);
            }

            regen = StartCoroutine(RegenStamina());
        }
        else
        {
            Debug.Log("Not enough stamina");
        }
    }


    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(1);

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaIncreasingSpeed * (maxStamina / 100);
            // staminaBar.value = currentStamina;
            yield return regenTick;
        }
        regen = null;
    }
}

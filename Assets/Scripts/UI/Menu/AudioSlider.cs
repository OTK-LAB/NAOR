using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour
{
    [SerializeField]
    private AudioMixer Mixer;
   /* [SerializeField]
    private AudioSource AudioSource;
    [SerializeField]
    private TextMeshProUGUI ValueText;*/
   
    private void Start()
    {
        Mixer.SetFloat("Volume", Mathf.Log10(PlayerPrefs.GetFloat("Volume", 1) * 20));
    }

    public void OnChangeSlider(float Value)
    {
        //ValueText.SetText($"{Value.ToString("N4")}");

        Mixer.SetFloat("Volume", Mathf.Log10(Value) * 20);

        float a = Mathf.Log10(Value) * 20;


    }



}
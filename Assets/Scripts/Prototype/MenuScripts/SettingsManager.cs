using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class SettingsManager : MonoBehaviour
{
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public AudioMixer audiomixer;
    private bool fullscreenOnOff;

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void SetMusicVolume(float value)
    {
        audiomixer.SetFloat("MusicVolume", value);
    }
    public void SetAmbianceVolume(float value)
    {
        audiomixer.SetFloat("AmbianceVolume", value);
    }

    public void SetFullscreen(bool fullscreenEnabled)
    {
        Screen.fullScreen = fullscreenEnabled;
        fullscreenOnOff = fullscreenEnabled;
    }

    //public void SaveSettings(){

    //PlayerPrefs.SetInt("QualitySettingPreference",QualitySettings.GetQualityLevel());
    //PlayerPrefs.SetInt("ResolutionPreference",resolutionDropdown.value);
    //PlayerPrefs.SetInt("FullScreenPreference",Convert.ToInt32(Screen.fullScreen));
    //PlayerPrefs.SetFloat("MusicVolumePreference",audiomixer.MusicVolume);
    //PlayerPrefs.SetFloat("AmbianceVolumePreference",audiomxier.AmbianceVolume);
    //}

    //public void LoadSettings(){

    //}
}

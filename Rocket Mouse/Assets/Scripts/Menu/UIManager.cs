using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // scripts -> parameter <- transition
    public Animator startButton;
    public Animator settingsButton;
    public Animator dialog;
    public Animator contentPanel;
    public Animator gearImage;
    public AudioSource bgAudioSource;
    public Slider volumeSlider;
    public Toggle muteToggle;

    private void Start()
    {
        volumeSlider.value = bgAudioSource.volume;
        muteToggle.isOn = bgAudioSource.mute;
    }

    public void ToggleMenu()
    {
        bool isHidden = contentPanel.GetBool("isHidden");
        contentPanel.SetBool("isHidden", !isHidden);
        gearImage.SetBool("isHidden", !isHidden);
    }

    public void OpenCloseSettings(bool openSetiings)
    {
        startButton.SetBool("isHidden", openSetiings);
        settingsButton.SetBool("isHidden", openSetiings);
        dialog.SetBool("isHidden", !openSetiings);
    }

    /*public void OpenSettings()
    {
        startButton.SetBool("isHidden", true);
        settingsButton.SetBool("isHidden", true);
        dialog.SetBool("isHidden", false); // 초기값이 true false가 되야 밖에서 안으로 들어 옴
    }

    public void ColseSettings()
    {
        startButton.SetBool("isHidden", false);
        settingsButton.SetBool("isHidden", false);
        dialog.SetBool("isHidden", true);
    }*/

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SaveVolume()
    {
        //AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
        //VolumeController.SaveVolume(AudioSource.volume);
        VolumeController.SaveVolume(bgAudioSource.volume);
    }

    public void SaveMute()
    {
        VolumeController.SaveMute(bgAudioSource.mute);
    }
}
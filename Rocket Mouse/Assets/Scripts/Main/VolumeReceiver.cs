using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeReceiver : MonoBehaviour
{
    private void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        float volume = VolumeController.LoadVolume();
        audioSource.volume = volume;

        bool mute = VolumeController.LoadMute();
        audioSource.mute = mute;
    }
}

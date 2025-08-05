using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeController : MonoBehaviour
{
    const string volumeKey = "BgVolume";
    const string muteKey = "BgMute";

    public static void SaveMute(bool mute)
    {
        PlayerPrefs.SetInt(muteKey, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadMute()
    {
        int mute = PlayerPrefs.GetInt(muteKey, 0);
        return mute == 1 ? true : false;
    }

    public static void SaveVolume(float volume)
    {
        //Debug.Log("SaveVolume" + volume);
        PlayerPrefs.SetFloat(volumeKey, volume);
        PlayerPrefs.Save();
    }

    public static float LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat(volumeKey, 1f);
        return volume;
    }
}

using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioClip drop;
    public AudioClip click;

    public void PlayClickSound() {
        NGUITools.PlaySound(click);
    }

    public void PlayDropSound() {
        NGUITools.PlaySound(drop);
    }

    public void AdjustMusicVolume(UISlider slider) {
        gameObject.GetComponent<AudioSource>().volume = slider.value;
    }

    public void AdjustSFXVolume(UISlider slider) {
        NGUITools.soundVolume = slider.value;
        if (NGUITools.soundVolume % 0.25 == 0) {
            PlayDropSound();
        }
    }
}

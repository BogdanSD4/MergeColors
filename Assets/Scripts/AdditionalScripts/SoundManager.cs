using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _music;
    [SerializeField] private AudioSource _sound;

    private static AudioSource _Music;
    private static AudioSource _Sound;

    [Space]
    public Music Music;
    public Sound Sound;
    private void Start()
    {
        _Music = _music;
        _Sound = _sound;
    }
    public void PlayMusic(MusicList music, bool loop)
    {
        _Music.clip = CheckMusicClip(music);
        _Music.loop = loop;
        _Music.Play();
    }
    public void PlaySound(SoundList audio, bool loop)
    {
        _Sound.clip = CheckSoundClip(audio);
        _Sound.loop = loop;
        _Sound.Play();
    }
    public void PlayOneShotMusic(MusicList music)
    {
        _Music.PlayOneShot(CheckMusicClip(music));
    }
    public void PlayOneShotSound(SoundList audio)
    {
        _Sound.PlayOneShot(CheckSoundClip(audio));
    }
    public void StopMusic()
    {
        _Music.Stop();
    }
    public void StopSound()
    {
        _Sound.Stop();
    }


    public void PlayOneShotMusicGUI(AudioClip music)
    {
        _Music.PlayOneShot(music);
    }
    public void PlayOneShotSoundGUI(AudioClip audio)
    {
        _Sound.PlayOneShot(audio);
    }

    private AudioClip CheckMusicClip(MusicList music)
    {
        switch (music)
        {
            case MusicList.Music0: return Music.Music0;
            case MusicList.Music1: return Music.Music1;
            case MusicList.Music2: return Music.Music2;
            default: return null;
        }
    }

    private AudioClip CheckSoundClip(SoundList sound)
    {
        switch (sound)
        {
            case SoundList.MenuSwitch: return Sound.MenuSwitch;
            case SoundList.ClassUpdate: return Sound.ClassUpdate;
            case SoundList.BuyCoin: return Sound.BuyCoin;
            case SoundList.BuyPaint: return Sound.BuyPaint;
            case SoundList.Update: return Sound.Update;
            default: return null;
        }
    }
}
[System.Serializable]
public class Music
{
    public AudioClip Music0;
    public AudioClip Music1;
    public AudioClip Music2;
}
[System.Serializable]
public class Sound
{
    public AudioClip MenuSwitch;
    public AudioClip ClassUpdate;
    public AudioClip BuyCoin;
    public AudioClip BuyPaint;
    public AudioClip Update;
}
public enum MusicList
{
    Music0,
    Music1,
    Music2,
}
public enum SoundList
{
    MenuSwitch,
    ClassUpdate,
    BuyCoin,
    BuyPaint,
    Update,
}

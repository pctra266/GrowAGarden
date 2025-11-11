using UnityEngine;
using UnityEngine.Audio; // <-- RẤT QUAN TRỌNG
using System;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager instance;

    // --- THÊM 2 DÒNG NÀY ---
    // Kéo 2 Group từ AudioMixer vào đây
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // --- THAY THẾ TOÀN BỘ VÒNG LẶP FOREACH NÀY ---
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;

            // --- LOGIC MỚI QUAN TRỌNG ---
            // Kiểm tra loại âm thanh và gán vào đúng Kênh (Group)
            if (sound.type == SoundType.Music)
            {
                sound.source.outputAudioMixerGroup = musicGroup;
            }
            else
            {
                sound.source.outputAudioMixerGroup = sfxGroup;
            }
        }

        playBGM();
    }


    void playBGM()
    {
        Play("BackgroundMusic");
    }

    public void MuteAll()
    {
        foreach (var sound in sounds)
        {
            sound.source.mute = !sound.source.mute;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public bool SoundIsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return false;
        }
        return s.source.isPlaying;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio
{
    private static GameObject Root;
    private const string LoadAudioFrontPath = "AudioClips/";
    private static AudioSource bgmAs;
    public Audio(GameObject audioRoot)
    {
        Root = audioRoot;
        PlayBgm();
    }
    private static readonly Dictionary<AudioPlayArea, string> audioClipPathDic = new Dictionary<AudioPlayArea, string>()
    {
        {AudioPlayArea.BGM,"bgm" },
        {AudioPlayArea.Button,"button" },
        {AudioPlayArea.Spin,"spin" },
        {AudioPlayArea.FlyOver,"fly" },
    };
    private static readonly Dictionary<AudioPlayArea, AudioClip> loadedAudioclipDic = new Dictionary<AudioPlayArea, AudioClip>();
    private static readonly List<AudioSource> allPlayer = new List<AudioSource>();
    public static AudioSource PlayOneShot(AudioPlayArea playArea,bool loop = false)
    {
        if(loadedAudioclipDic.TryGetValue(playArea,out AudioClip tempClip))
        {
            if (tempClip == null)
            {
                loadedAudioclipDic.Remove(playArea);
                Debug.LogError("音频路径配置错误，类型:" + playArea);
                return null;
            }
            int asCount = allPlayer.Count;
            AudioSource tempAs;
            for (int i = 0; i < asCount; i++)
            {
                tempAs = allPlayer[i];
                if (!tempAs.isPlaying)
                {
                    tempAs.clip = tempClip;
                    tempAs.loop = loop;
                    tempAs.mute = !Save.data.sound_on;
                    tempAs.Play();
                    return tempAs;
                }
            }
            tempAs = Root.AddComponent<AudioSource>();
            allPlayer.Add(tempAs);
            tempAs.clip = tempClip;
            tempAs.loop = loop;
            tempAs.mute = !Save.data.sound_on;
            tempAs.Play();
            return tempAs;
        }
        else
        {
            if(audioClipPathDic.TryGetValue(playArea,out string tempClipFileName))
            {
                tempClip = Resources.Load<AudioClip>(LoadAudioFrontPath + tempClipFileName);
                if (tempClip == null)
                {
                    Debug.LogError("配置的音频文件路径错误，类型:" + playArea);
                    return null;
                }
                int asCount = allPlayer.Count;
                AudioSource tempAs;
                for (int i = 0; i < asCount; i++)
                {
                    tempAs = allPlayer[i];
                    if (!tempAs.isPlaying)
                    {
                        tempAs.clip = tempClip;
                        tempAs.loop = loop;
                        tempAs.mute = !Save.data.sound_on;
                        tempAs.Play();
                        return tempAs;
                    }
                }
                tempAs = Root.AddComponent<AudioSource>();
                allPlayer.Add(tempAs);
                tempAs.clip = tempClip;
                tempAs.loop = loop;
                tempAs.mute = !Save.data.sound_on;
                tempAs.Play();
                return tempAs;
            }
            else
            {
                Debug.LogError("没有配置音频文件路径，类型:" + playArea);
                return null;
            }
        }
    }
    public static AudioSource PlayLoop(AudioPlayArea playArea)
    {
        return PlayOneShot(playArea, true);
    }
    private void PlayBgm()
    {
        if(audioClipPathDic.TryGetValue(AudioPlayArea.BGM,out string bgmFileName))
        {
            AudioClip tempClip = Resources.Load<AudioClip>(LoadAudioFrontPath + bgmFileName);
            if(tempClip == null)
            {
                Debug.LogError("背景音乐文件路径配置错误");
                return;
            }
            bgmAs = Root.AddComponent<AudioSource>();
            bgmAs.clip = tempClip;
            bgmAs.loop = true;
            bgmAs.mute = !Save.data.music_on;
            bgmAs.Play();
        }
        else
        {
            Debug.LogError("背景音乐没有配置文件路径");
        }
    }
    public static void SetMusicState(bool isOn)
    {
        bgmAs.mute = !isOn;
    }
    public static void SetSoundState(bool isOn)
    {
        int count = allPlayer.Count;
        for(int i = 0; i < count; i++)
        {
            allPlayer[i].mute = !isOn;
        }
    }
    public static void PauseBgm(bool pause)
    {
        if (pause)
            bgmAs.Pause();
        else
            bgmAs.UnPause();
    }
}
public enum AudioPlayArea
{
    Button,
    BGM,
    Spin,
    FlyOver,
}

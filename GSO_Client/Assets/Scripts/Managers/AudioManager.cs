using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private Dictionary<string, List<AudioClip>> audioclips;
    private List<AudioSource> audioSources;
    private void Awake()
    {
        if(instance == null)
            instance = this;
        Init();
    }

    private void Init()
    {
        audioclips = new Dictionary<string, List<AudioClip>>();
        
        AudioClip[] audioClip = Resources.LoadAll<AudioClip>("Audio");

        foreach (var clip in audioClip)
        {
            string namePart = clip.name.Split('_').First();
            
            if(!audioclips.ContainsKey(namePart))
                audioclips[namePart] = new List<AudioClip>();
            
            audioclips[namePart].Add(clip);
        }
        
        audioSources = new List<AudioSource>();
        
        Debug.Log("Audio Load Complete");
    }

    public void PlaySound(String keyword, AudioSource source)
    {
        //키워드에 해당하는 클립 없을 때
        if (!audioclips.ContainsKey(keyword))
            return;
        //실행 중이면서 현재 키워드 소리가 실행되고 있을 때
        if (source.isPlaying && source.clip.name == keyword)
            return;
        
        if(!audioSources.Contains(source))
            audioSources.Add(source);
        List<AudioClip> audioclipList = audioclips[keyword];
        if (audioclipList.Count > 1)
        {
            int index = Random.Range(0, audioclipList.Count);
            source.clip = audioclipList[index];
        }
        else
            source.clip = audioclipList.First();
        
        if(source.isPlaying)
            source.Stop();
        source.Play();
        Debug.Log($"{keyword} play success");
    }

    public void StopSound(AudioSource source)
    {
        if(source.isPlaying)
            source.Stop();
    }

    public void StopAllSounds()
    {
        foreach(var source in audioSources)
            source.Stop();
    }
}

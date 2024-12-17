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
        
        Debug.Log("Audio Load Complete");
    }

    public void PlaySound(String keyword, AudioSource source)
    {
        if (!audioclips.ContainsKey(keyword))
            return;
        
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
}

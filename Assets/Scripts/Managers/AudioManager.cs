using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _clips;
    //[SerializeField] private prefa _clips;
    public static AudioManager Instance;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void PlaySoud(string name)
    {
        foreach (var clip in _clips)
        {
            if (clip.name == name)
            {
                AudioSource newSource = new AudioSource();
                newSource.clip = clip;
                Destroy(newSource, clip.length + 1f);
            }
        }

    }
}

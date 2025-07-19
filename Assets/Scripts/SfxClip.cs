using System;
using UnityEngine;

[Serializable]
public class SfxClip
{
    public AudioClip clip;
    public float volume = 1f;
    
    [Range(0f, 1f)]
    public float startTime = 0f;
}

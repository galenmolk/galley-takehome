using System;
using UnityEngine;

[Serializable]
public class SfxClip
{
    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float startTime = 0f;
}

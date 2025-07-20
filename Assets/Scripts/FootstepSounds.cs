using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float footstepDelay = 0.5f;
    [SerializeField] private Vector2 randomPitchRange = new(0.9f, 1.1f);

    private readonly List<AudioClip> availableClips = new();

    private AudioClip lastClip;
    private float lastFootstepTime;

    private void Start()
    {
        foreach (var clip in footstepClips)
        {
            availableClips.Add(clip);
        }
    }

    private void TriggerFootstep()
    {
        var nextClip = availableClips[Random.Range(0, availableClips.Count)];
        availableClips.Remove(nextClip);

        lastClip = audioSource.clip;
        audioSource.clip = nextClip;

        audioSource.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
        audioSource.Play();

        if (lastClip != null)
        {
            availableClips.Add(lastClip);
        }
    }

    private void Update()
    {
        if (UserInputListener.Instance.MoveValue.magnitude > 0f)
        {
            if (Time.time - footstepDelay > lastFootstepTime)
            {
                TriggerFootstep();
                lastFootstepTime = Time.time;
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepDelay = 0.5f;
    [SerializeField] private Vector2 randomPitchRange = new(0.9f, 1.1f);

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    private List<AudioClip> availableClips;

    private AudioClip lastClip;
    private float lastFootstepTime;

    private void Start()
    {
        // Make a copy of the serialized array of clips so we can easily prevent the same clip from being played back-to-back.
        availableClips = footstepClips.ToList();
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
        var isWalking = UserInputListener.Instance.MoveValue.magnitude > 0f;
         
         if (isWalking && Time.time - footstepDelay > lastFootstepTime)
        {
            TriggerFootstep();
            lastFootstepTime = Time.time;
        }
    }
}

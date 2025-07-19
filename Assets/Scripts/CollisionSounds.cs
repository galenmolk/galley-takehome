using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollisionSounds : MonoBehaviour
{
    public LayerMask collisionSoundLayer;

    public SfxClip[] clips;

    public AudioSource audioSource;

    public float minimumTimeBetweenClips = 1f;

    private float lastClipTime;

    private int trackIdx;

    public Rigidbody rb;

    public float maxVolume = 0.7f;

    public float baseVolume = 0.1f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float minVelocityForSound = 0.05f;
    [SerializeField] private float quietTimeAfterGameStart = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < quietTimeAfterGameStart)
        {
            return;
        }

        if (((1 << gameObject.layer) & collisionSoundLayer.value) == 0)
        {
            return;
        }

        var velocity = collision.relativeVelocity.magnitude;
        if (velocity < minVelocityForSound)
        {
            return;
        }

        var currentTime = Time.time;
        if (currentTime < lastClipTime + minimumTimeBetweenClips)
        {
            return;
        }

        var sfxClip = clips[Random.Range(0, clips.Length)];

        //var sfxClip = clips[trackIdx];
        trackIdx = ((trackIdx + 1) % clips.Length);
        var clip = sfxClip.clip;
        audioSource.clip = clip;

        var logSpeed = Mathf.Log10(velocity);
        audioSource.volume = Mathf.Min(baseVolume + logSpeed, maxVolume);
        Debug.Log($"Vol: {audioSource.volume} (vel: {logSpeed})");
        audioSource.time = clip.length * sfxClip.startTime;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        lastClipTime = currentTime;
        audioSource.Play();
    }
}

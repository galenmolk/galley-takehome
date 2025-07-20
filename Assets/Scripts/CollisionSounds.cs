using UnityEngine;

public class CollisionSounds : MonoBehaviour
{
    [SerializeField] private SfxClip[] clips;
    [SerializeField] private LayerMask collisionSoundLayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minimumTimeBetweenClips = 1f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxVolume = 0.7f;
    
    [Tooltip("Some extra volume will be added on top of this proportional to the object's velocity.")]
    [SerializeField] private float baseVolume = 0.1f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;
    [SerializeField] private float minVelocityForSound = 0.05f;
    [SerializeField] private float quietTimeAfterGameStart = 2f;

    private float lastClipTime;

    private void OnCollisionEnter(Collision collision)
    {
        // Quick fix to prevent ship physics from causing a lot of loud sounds at the top of the scene.
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

        var clip = sfxClip.clip;
        audioSource.clip = clip;

        // Add some volume based on the velocity.
        audioSource.volume = Mathf.Min(baseVolume + Mathf.Log10(velocity), maxVolume);

        // startTime could allow you to specify an offset but I ended up not using it and 
        // just quickly trimming the clips in Audacity.
        audioSource.time = clip.length * sfxClip.startTime;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        lastClipTime = currentTime;
        audioSource.Play();
    }
}

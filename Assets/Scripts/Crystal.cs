using System;
using System.Collections;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public enum Type
    {
        Green,
        Peach,
        Purple
    }
    public const int MaxNeededPerType = 3;

    public static event Action<Crystal.Type> OnCrystalAcquired;

    [SerializeField] private Crystal.Type type;
    [SerializeField] private AnimationCurve buildUpCurve;
    [SerializeField] private AudioSource shimmerAudioSource;
    [SerializeField] private float buildUpDuration = 4f;
    [SerializeField] private float audioPitchStart = -1f;
    [SerializeField] private float audioPitchEnd = 1f;
    [SerializeField] private float cameraShakeMax = 5f;
    [SerializeField] private Light crystalLight;
    [SerializeField] private float lightIntensityMax = 7f;
    [SerializeField] private AudioClip spawnSfx;
    [SerializeField] private float preSequenceDelay = 0.35f;
    [SerializeField] private AudioClip poofSfx;

    private float spawnTime;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(preSequenceDelay);
        AudioSource.PlayClipAtPoint(spawnSfx, transform.position);

        spawnTime = Time.time;
        var lightIntensityStart = crystalLight.intensity;
        var timer = 0f;

        while (timer < buildUpDuration)
        {
            timer = Time.time - spawnTime;

            var t = buildUpCurve.Evaluate(timer / buildUpDuration);
            CameraShaker.Instance.SetAmplitude(Mathf.Lerp(0f, cameraShakeMax, t));
            shimmerAudioSource.pitch = Mathf.Lerp(audioPitchStart, audioPitchEnd, t);
            crystalLight.intensity = Mathf.Lerp(lightIntensityStart, lightIntensityMax, t);
            yield return null;
        }

        ParticlePooler.Instance.SpawnCrystalCollectEffect(transform.position, type);
        AudioSource.PlayClipAtPoint(poofSfx, transform.position);

        OnCrystalAcquired?.Invoke(type);

        CameraShaker.Instance.SetAmplitude(0f);
        gameObject.SetActive(false);
    }
}

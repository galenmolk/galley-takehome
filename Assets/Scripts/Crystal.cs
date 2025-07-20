using System.Collections;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class Crystal : MonoBehaviour
{
    [SerializeField] private AnimationCurve buildUpCurve;
    [SerializeField] private AudioSource shimmerAudioSource;
    [SerializeField] private float buildUpDuration = 4f;
    [SerializeField] private float audioPitchStart = -1f;
    [SerializeField] private float audioPitchEnd = 1f;
    [SerializeField] private float cameraShakeMax = 5f;
    [SerializeField] private Light crystalLight;
    [SerializeField] private float lightIntensityMax = 7f;

    private float spawnTime;

    private IEnumerator Start()
    {
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

        CameraShaker.Instance.SetAmplitude(0f);
        gameObject.SetActive(false);
        // insensity of glow increases
        // crystal explosion sfx
        // crystal disappears
        // particle effect shoots up to crystal meter in UI
        // crystal icon charges
        // 
    }
}

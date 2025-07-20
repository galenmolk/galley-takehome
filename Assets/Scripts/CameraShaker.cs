using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;

    private void Start()
    {
        Instance = this;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void SetAmplitude(float intensity)
    {
        perlin.m_AmplitudeGain = intensity;
    }
}

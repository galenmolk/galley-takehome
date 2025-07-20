using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePooler : MonoBehaviour
{
    public static ParticlePooler Instance { get; private set; }

    [SerializeField] private ParticleSystem dustPrefab;
    [SerializeField] private int initialSize = 2;

    private Queue<ParticleSystem> dustInstances = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            EnqueueNewInstance();
        }
    }

    public void SpawnDust(Vector3 position)
    {
        if (dustInstances.TryDequeue(out var dustInstance))
        {
            dustInstance.transform.position = position;
        }
        else
        {
            dustInstance = EnqueueNewInstance();
        }

        Debug.Log($"Enabling {dustInstance.gameObject} {dustInstance.main.duration}");
        dustInstance.gameObject.SetActive(true);
        StartCoroutine(EnqueueAfterLifetime(dustInstance));
    }

    private ParticleSystem EnqueueNewInstance()
    {
        var instance = Instantiate(dustPrefab);
        instance.gameObject.SetActive(false);
        dustInstances.Enqueue(instance);
        return instance;
    }

    private IEnumerator EnqueueAfterLifetime(ParticleSystem dust)
    {
        yield return new WaitForSeconds(dust.main.duration);
        dust.gameObject.SetActive(false);
        dustInstances.Enqueue(dust);
    }
}

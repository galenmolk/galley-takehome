using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePooler : MonoBehaviour
{
    public static ParticlePooler Instance { get; private set; }

    [SerializeField] private int initialSize = 2;

    [SerializeField] private ParticleSystem dustPrefab;
    [SerializeField] private ParticleSystem explosionPrefab;

    private readonly Queue<ParticleSystem> dustInstances = new();
    private readonly Queue<ParticleSystem> explosionInstances = new();

    private Dictionary<Crystal.Type, (Queue<ParticleSystem> queue, ParticleSystem prefab)> crystalCollectInstances;
    [SerializeField] private ParticleSystem greenCollectPrefab, purpleCollectPrefab, peachCollectPrefab;

    private void Awake()
    {
        Instance = this;

        crystalCollectInstances = new Dictionary<Crystal.Type, (Queue<ParticleSystem>, ParticleSystem)>()
        {
            { Crystal.Type.Green, (new Queue<ParticleSystem>(), greenCollectPrefab) },
            { Crystal.Type.Peach, (new Queue<ParticleSystem>(), peachCollectPrefab) },
            { Crystal.Type.Purple, (new Queue<ParticleSystem>(), purpleCollectPrefab) },
        };

        for (int i = 0; i < initialSize; i++)
        {
            EnqueueNewInstance(dustInstances, dustPrefab);

            EnqueueNewInstance(explosionInstances, explosionPrefab);

            var (greenQueue, greenPrefab) = crystalCollectInstances[Crystal.Type.Green];
            EnqueueNewInstance(greenQueue, greenPrefab);

            var (peachQueue, peachPrefab) = crystalCollectInstances[Crystal.Type.Peach];
            EnqueueNewInstance(peachQueue, peachPrefab);

            var (purpleQueue, purplePrefab) = crystalCollectInstances[Crystal.Type.Purple];
            EnqueueNewInstance(purpleQueue, purplePrefab);
        }
    }

    public void SpawnDustEffect(Vector3 position)
    {
        SpawnEffect(position, dustInstances, dustPrefab);
    }

    public void SpawnExplosionEffect(Vector3 position)
    {
        SpawnEffect(position, explosionInstances, explosionPrefab);
    }

    public void SpawnCrystalCollectEffect(Vector3 position, Crystal.Type type)
    {
        var (queue, prefab) = crystalCollectInstances[type];
        SpawnEffect(position, queue, prefab);
    }

    private void SpawnEffect(Vector3 position, Queue<ParticleSystem> queue, ParticleSystem prefab)
    {
        if (queue.TryDequeue(out var effectInstance))
        {
            effectInstance.transform.position = position;
        }
        else
        {
            effectInstance = EnqueueNewInstance(queue, prefab);
        }

        effectInstance.gameObject.SetActive(true);
        StartCoroutine(EnqueueAfterLifetime(effectInstance, queue));
    }

    private ParticleSystem EnqueueNewInstance(Queue<ParticleSystem> queue, ParticleSystem prefab)
    {
        var instance = Instantiate(prefab, transform);
        instance.gameObject.SetActive(false);
        queue.Enqueue(instance);
        return instance;
    }

    private IEnumerator EnqueueAfterLifetime(ParticleSystem instance, Queue<ParticleSystem> queue)
    {
        yield return new WaitForSeconds(instance.main.duration);
        instance.gameObject.SetActive(false);
        queue.Enqueue(instance);
    }
}

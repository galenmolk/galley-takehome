using DiabolicalGames;
using UnityEngine;

public class CrateDestructionController : MonoBehaviour
{
    [SerializeField] private GameObject destroyParticles;
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private LayerMask ignoreOtherVelocityLayer;
    [SerializeField] private float safeTimeThreshold;
    [SerializeField] private DestructibleObject destructibleObject;
    [SerializeField] private Vector2 startingHealthRange = new(50, 125);
    [SerializeField] private float damageThreshold = 5f;
    [SerializeField] private Rigidbody rb;

    private float currentHealth;

    private void Start()
    {
        currentHealth = Random.Range(startingHealthRange.x, startingHealthRange.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < safeTimeThreshold)
        {
            return;
        }

        var ignoreOtherVelocity = ((1 << collision.gameObject.layer) & ignoreOtherVelocityLayer.value) != 0;

        var damage = ignoreOtherVelocity ?
            rb.linearVelocity.magnitude :
            collision.relativeVelocity.magnitude;

        if (damage < damageThreshold)
        {
            return;
        }

        currentHealth -= damage;
        ParticlePooler.Instance.SpawnDust(collision.GetContact(0).point);

        if (currentHealth <= 0f)
        {
            rb.detectCollisions = false;
            destructibleObject.Break();
            Instantiate(destroyParticles, transform.position, Quaternion.identity);
            if (crystalPrefab != null)
            {
                Instantiate(crystalPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}

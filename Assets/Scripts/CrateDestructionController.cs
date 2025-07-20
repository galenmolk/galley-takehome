using DiabolicalGames;
using UnityEngine;

public class CrateDestructionController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Relative velocity will be used for collision damage calculations. " +
        "Objects on these layers will be excluded - only this object's velocity will be used in those cases.")]
    [SerializeField] private LayerMask ignoreOtherVelocityLayer;
    [SerializeField] private float safeTimeThreshold;
    [SerializeField] private Vector2 startingHealthRange = new(50, 125);
    [SerializeField] private float damageThreshold = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private DestructibleObject destructibleObject;
    [Tooltip("Optional. If assigned, this GameObject will be instantiated when the object is destroyed.")]
    [SerializeField] private GameObject crystalPrefab;


    private float currentHealth;
    private bool isBroken;

    private void Start()
    {
        currentHealth = Random.Range(startingHealthRange.x, startingHealthRange.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple crystals from being granted by a single destructible.
        if (isBroken)
        {
            return;
        }

        // Prevent damage for a bit at the top of the game.
        if (Time.timeSinceLevelLoad < safeTimeThreshold)
        {
            return;
        }

        // Ignore ship velocity when calculating damage as its kinematic.
        var ignoreOtherVelocity = ((1 << collision.gameObject.layer) & ignoreOtherVelocityLayer.value) != 0;

        var damage = ignoreOtherVelocity ?
            rigidbody.linearVelocity.magnitude :
            collision.relativeVelocity.magnitude;

        // Prevent minor damage in the case of stacked crates, for example.
        if (damage < damageThreshold)
        {
            return;
        }

        currentHealth -= damage;

        // Place some dust at the first collision contact point.
        ParticlePooler.Instance.SpawnDustEffect(collision.GetContact(0).point);

        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    private void Break()
    {
        isBroken = true;

        // Triggers breaking SFX and debris spawning.
        destructibleObject.Break();

        // Create a large cloud of dust.
        ParticlePooler.Instance.SpawnExplosionEffect(transform.position);

        // Not all destructibles will contain crystals (i.e. Pallets).
        if (crystalPrefab != null)
        {
            Instantiate(crystalPrefab, transform.position, Quaternion.identity);
        }
    }
}

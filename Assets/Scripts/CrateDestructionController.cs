using DiabolicalGames;
using UnityEngine;

public class CrateDestructionController : MonoBehaviour
{
    [SerializeField] private LayerMask ignoreOtherVelocityLayer;
    [SerializeField] private float safeTimeThreshold;

    [SerializeField] private DestructibleObject destructibleObject;

    [SerializeField, Min(0f)] private float startingHealth;

    [SerializeField] private float damageThreshold = 5f;
    [SerializeField] private Rigidbody rb;

    private float currentHealth;

    private void Start()
    {
        currentHealth = startingHealth;
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

        if (damage > 0) {
            Debug.Log($"{gameObject.name} was hit by {collision.gameObject.name} ({damage}) (ignore other? {ignoreOtherVelocity})");
        }

        if (damage > damageThreshold)
        {
            currentHealth -= damage;

            if (currentHealth <= 0f)
            {
                destructibleObject.Break();
            }
        }
    }
}

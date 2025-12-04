using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [Header("Direct Hit")]
    [SerializeField] private int damage = 15;

    [Header("Burning Area Effect")]
    [SerializeField] private GameObject burnEffectPrefab;
    [SerializeField] private LayerMask burnLayerMask;
    [SerializeField] private float burnBoxRadiusFactor = 3f;
    [SerializeField] private int burnTickDamage = 5;
    [SerializeField] private float burnTickRate = 0.1f;
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private int burnForce = 50;

    private bool exploded = false;

    private void Awake()
    {
        StartCoroutine(DIE());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return; // prevent double explosions

        // Direct damage
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();
        if (target != null)
        {
            target.Damage(damage, true);
        }

        if(burnEffectPrefab != null)
        {
            Instantiate(burnEffectPrefab, collision.contacts[0].point, Quaternion.identity, gameObject.transform);
        }

        // Start burn area on impact
        StartCoroutine(BurnArea());

        exploded = true;

        // Destroy the physical fireball immediately after impact
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }

    IEnumerator BurnArea()
    {
        float timer = 0f;

        while (timer < burnDuration)
        {
            Collider[] hits = CheckBurnArea();

            foreach (Collider col in hits)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                IDamageable dmg = col.GetComponentInParent<IDamageable>();

                if (rb != null)
                {
                    Vector3 forceDir = (col.transform.position - transform.position).normalized;
                    rb.AddForce(forceDir * burnForce);
                }

                if (dmg != null)
                {
                    dmg.Damage(burnTickDamage, true);
                }
            }

            timer += burnTickRate;
            yield return new WaitForSeconds(burnTickRate);
        }

        Destroy(gameObject);
    }

    Collider[] CheckBurnArea()
    {
        Vector3 boxCenter = transform.position;
        Vector3 boxSize = transform.localScale * burnBoxRadiusFactor;

        return Physics.OverlapBox(
            boxCenter,
            boxSize / 2f,
            Quaternion.identity,
            burnLayerMask
        );
    }

    IEnumerator DIE()
    {
        yield return new WaitForSeconds(7f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale * burnBoxRadiusFactor);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragonbreath : MonoBehaviour
{
    [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private int burnForce = 50;
    [SerializeField] private ParticleSystem fireBreathParticles;
    private int damage = 5;

    private int burnBoxRadiusFactor = 3;
    private Vector3 burnBoxPosition;
    private Coroutine burnObjects;

    private void Start()
    {
        Activate();
    }

    /*
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (fireBreathParticles.isPlaying)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }
    }
    */

    void Activate()
    {
        fireBreathParticles.Play();
        burnObjects = StartCoroutine(Burn());
    }

    void Deactivate()
    {
        StopCoroutine(burnObjects);
        fireBreathParticles.Stop();
    }

    Collider[] checkCollisions()
    {
        burnBoxPosition = gameObject.transform.position + gameObject.transform.forward * 2.5f;
        return Physics.OverlapBox(burnBoxPosition, transform.localScale * burnBoxRadiusFactor / 2, Quaternion.identity, m_LayerMask);
    }

    IEnumerator Burn()
    {
        while (true)
        {
            //Debug.Log("Burning");
            Collider[] hitColliders = checkCollisions();

            //Debug.Log("# hit: " + hitColliders.Length);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].GetComponentInParent<IDamageable>() != null)
                {
                    Vector3 finalBurnForce = (hitColliders[i].transform.position - gameObject.transform.position) * burnForce;
                    hitColliders[i].GetComponent<Rigidbody>().AddForce(finalBurnForce);
                    hitColliders[i].GetComponentInParent<IDamageable>().Damage(damage, true);
                }

                //Debug.Log("i value: " + i);
                i++;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireCube(burnBoxPosition, transform.localScale * burnBoxRadiusFactor);
        }
    }
}

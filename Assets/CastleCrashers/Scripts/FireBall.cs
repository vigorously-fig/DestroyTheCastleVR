using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private int damage = 15;

    private void Awake()
    {
        StartCoroutine("DIE");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            collision.gameObject.GetComponent<IDamageable>().Damage(damage, false);
        }
    }

    IEnumerator DIE()
    {
        yield return new WaitForSeconds(7f);
        Destroy(gameObject);
    }
}

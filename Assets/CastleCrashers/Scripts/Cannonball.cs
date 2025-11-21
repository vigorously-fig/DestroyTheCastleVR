using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    private int damage = 30;

    private void Awake()
    {
        StartCoroutine("killYOURSELF");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            collision.gameObject.GetComponent<IDamageable>().Damage(damage, false);
        }
    }

    IEnumerator killYOURSELF()
    {
        yield return new WaitForSeconds(7f);
        Destroy(gameObject);
    }
}

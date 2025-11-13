using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, ITriggerable
{
    [SerializeField] private GameObject cannonballPrefab;

    private float cannonballForce = 100f;
    private float cooldown = 3f;
    Coroutine cooldownTimer;

    private bool test = true;

    public void Trigger()
    {
        if (test)
        {
            GameObject spawnedCannonball = Instantiate(cannonballPrefab, transform.position, transform.rotation);
            spawnedCannonball.GetComponent<Rigidbody>().AddForce(spawnedCannonball.transform.forward * cannonballForce);
            test = false;
        }
    }
}

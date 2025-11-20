using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour, ITriggerable
{
    [SerializeField] private GameObject fireballPrefab;

    //Sound and Visual effects
    AudioSource audioSource;
    private AudioClip wandSoundEffect;
    [SerializeField] ParticleSystem particles;


    //Make the wands have different power levels
    [SerializeField] public float wandForce = 500f;
    [SerializeField] public float cooldownDelay = 1.5f; //half of cannon cooldown
    [SerializeField] Coroutine cooldownTimer;

    private float wandSoundPitchMin = 0.8f; //higher pitch range since tiny
    private float wandSoundPitchMax = 1.5f;

    private bool shouldCast = true;

    // Start is called before the first frame update
    void Start()
    {
       audioSource = gameObject.GetComponent<AudioSource>();
       //wandAnimator = gameObject.GetComponent<Animator>(); (do we want a wand animation? What would it do?)
    }

    public void Trigger()
    {
        if (shouldCast)
        {
            GameObject spawnedMagic = Instantiate(fireballPrefab, transform.position + transform.right * -1 + transform.forward * 1, transform.rotation);
            spawnedMagic.GetComponent<Rigidbody>().AddForce((transform.right * -1 + transform.forward * 0.5f) * wandForce);

            playFX();

            shouldCast = false;
            cooldownTimer = StartCoroutine("doCooldown");
        }
    }

    public void playFX()
    {
        audioSource.pitch = Random.Range(wandSoundPitchMin, wandSoundPitchMax);
        audioSource.PlayOneShot(wandSoundEffect);
        particles.Play();
        
        //No wand animation for now
    }

    IEnumerator doCooldown()
    {
        yield return new WaitForSeconds(cooldownDelay);
        shouldCast = true;
    }

    /* IEnumerable playCast()
    {
        //No wand animation for now
        yield return null;
    }
    */
}

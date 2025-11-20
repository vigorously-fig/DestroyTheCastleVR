using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] GameObject cannon;

    private void FixedUpdate()
    {
        //Debug.Log(transform.eulerAngles.x);
        if (10 < transform.eulerAngles.x && transform.eulerAngles.x <= 310)
        {
            //Debug.Log("HIIII");
            if (cannon.GetComponent<ITriggerable>() != null)
            {
                cannon.GetComponent<ITriggerable>().Trigger();
            }
        }
    }
}

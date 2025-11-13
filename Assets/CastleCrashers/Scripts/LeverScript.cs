using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    GameObject cannon;

    private void Start()
    {
        cannon = gameObject.transform.parent.gameObject;
    }

    private void FixedUpdate()
    {
        if (cannon.GetComponent<ITriggerable>() != null)
        {
            cannon.GetComponent<ITriggerable>().Trigger();
        }
    }
}

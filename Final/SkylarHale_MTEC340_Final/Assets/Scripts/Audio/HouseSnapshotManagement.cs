using System;
using UnityEngine;

public class HouseSnapshotManagement : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("House Trigger"))
        {
            //Debug.Log("Entered house");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Mix/Enter House");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("House Trigger"))
        {
            //Debug.Log("Exited house");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Mix/Exit House");
        }
    }
}

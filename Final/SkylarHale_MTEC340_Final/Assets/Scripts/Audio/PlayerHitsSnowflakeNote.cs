using System;
using UnityEngine;

public class PlayerHitsSnowflakeNote : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Snowflake")) FMODUnity.RuntimeManager.PlayOneShot("event:/Music/Player Colliding With Snowflake");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeLootHandler : MonoBehaviour
{
    public GameObject weaponMesh;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.PickupWeapon(gameObject, weaponMesh);
            }
        }
    }
}

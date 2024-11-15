using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{

    public int damagePerSecond = 10;

    private GameManager _GameManager;

    private bool playerInLava = false;

    private void Start()
    {
        _GameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("OnTriggerEnter работает");
            playerInLava = true;
            StartCoroutine(ApplyDamageOverTime());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLava = false;
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (playerInLava)
        {
            Debug.Log("Игрок получает урон!");
            _GameManager.Healing(-damagePerSecond);
            yield return new WaitForSeconds(1f);
        }
    }
}

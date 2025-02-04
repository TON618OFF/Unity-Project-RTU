using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Lava : MonoBehaviour
{
    public float damagePerSecond = 10f;
    private GameManager _GameManager;
    public Slider HealthBar;

    private bool playerInLava = false;

    private void Start()
    {
        _GameManager = FindObjectOfType<GameManager>();

        if (_GameManager == null)
        {
            Debug.LogError("GameManager не найден на сцене!");
            return; 
        }

        if (HealthBar == null)
        {
            HealthBar = GameObject.Find("HealthBar")?.GetComponent<Slider>();
            if (HealthBar == null)
            {
                Debug.LogError("HealthBar не найден в сцене!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошел в лаву");
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
            if (_GameManager != null)
            {
                Debug.Log("Игрок получает урон от лавы!");
                _GameManager.TakeDamage(damagePerSecond);
            }
            else
            {
                Debug.Log("GameManager не инициализирован!");
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}

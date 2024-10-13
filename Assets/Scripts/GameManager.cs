using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int Health;

    public int MaxHealth = 100;

    private int Stamina = 100;

    public bool IsStaminaRestoring = false;

    private void Start()
    {
        Health = MaxHealth;
    }

    private IEnumerator StaminaRestore()
    {
        IsStaminaRestoring = true;
        yield return new WaitForSeconds(3f);
        Stamina = 100;
        IsStaminaRestoring = false;
    }

    private void StaminaCheck()
    {
        Debug.Log("Стамина: " + Stamina);

        if (Stamina <= 0) StartCoroutine(StaminaRestore());
    }

    public void SpendStamina()
    {
        Stamina -= 1;
    }

    private void FixedUpdate()
    {
        StaminaCheck();
    }

    public void Healing(int HealthPointCount)
    {
        Health += HealthPointCount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        if (Health <= 0)
        {
            Health = 0;
            Debug.Log("Игрок умер!");
            ReloadLevel();
            // Здесь можно добавить логику, например, перезапуск уровня или конец игры
        }

        Debug.Log("HP: " + Health);

    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

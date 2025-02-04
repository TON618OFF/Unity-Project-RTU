using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int Health;

    public int MaxHealth = 100;

    public int Stamina = 100;

    public Slider StaminaBar;

    public Slider HealthBar;

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
        StaminaBar.value = 100;
        IsStaminaRestoring = false;
    }

    private void StaminaCheck()
    {
        //Debug.Log("�������: " + Stamina);

        if (Stamina <= 0) StartCoroutine(StaminaRestore());
    }

    public void SpendStamina()
    {
        if (Stamina > 0)
        {
            Stamina -= 1;
            StaminaBar.value -= 1;
        }
    }

    private void FixedUpdate()
    {
        StaminaCheck();
    }

    public void Healing(int HealthPointCount)
    {
        Health += HealthPointCount;
        HealthBar.value += HealthPointCount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        if (Health <= 0)
        {
            Health = 0;
            Debug.Log("����� ����!");
            ReloadLevel();
        }

        Debug.Log("HP: " + Health);

    }

    public void TakeDamage(float damagePerSecond)
    {
        HealthBar.value -= damagePerSecond;

        if (HealthBar.value < 0)
        {
            HealthBar.value = 0;
            Debug.Log("����� ����!");
            ReloadLevel();
        }
    }

    private void ReloadLevel()
    {
        // ���������� ��� �������� ����� �������������
        StopAllCoroutines();

        // �������� ��� ����, ����� ��� �������� ������ ����������� �� ������������
        StartCoroutine(WaitBeforeReload());
    }

    private IEnumerator WaitBeforeReload()
    {
        yield return new WaitForSeconds(1f);  // ��������� ��������� �������� ����� �������������
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

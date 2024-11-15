using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponsEnum WeaponType;

    public int MaxMagazineBulletCount;

    public int CurrentMagazineBulletCount;

    public int MaxAmmoSupply;

    public float TimeBetweenShots;

    public int TimeForReloading;

    public AudioSource ShotSound;

    public AudioSource ReloadSound;

    bool Canfire = true;

    bool IsReloading = false;

    private IEnumerator LockFire(float Time)
    {
        yield return new WaitForSeconds(Time);
        Canfire = true;
    }

    private IEnumerator LockFireForReloading(float Time)
    {
        ReloadSound.Play();
        yield return new WaitForSeconds(Time);
        CurrentMagazineBulletCount = MaxMagazineBulletCount;
        Canfire = true;
        IsReloading = false;
        Debug.Log("Перезарядка завершена!");
    }

    public bool IsMagazineEmpty()
    {
        if (CurrentMagazineBulletCount == 0) return true;
        else return false;
    }

    public void Reload()
    {
        if (!IsReloading)
        {
            Debug.Log("Перезарядка!");
            IsReloading = true;
            Canfire = false;

            StartCoroutine(LockFireForReloading(TimeForReloading));
        }
    }

    public void Fire()
    {
        if (Canfire && !IsMagazineEmpty() && !IsReloading)
        {
            Debug.Log(CurrentMagazineBulletCount);
            ShotSound.Play();
            CurrentMagazineBulletCount--;

            RaycastHit HitInfo = new RaycastHit();

            // Проверка, что главная камера существует
            if (Camera.main != null)
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo))
                {
                    Debug.Log(HitInfo.transform.name);
                }
            }
            else
            {
                Debug.LogError("Main camera not found! Make sure your camera has the tag 'MainCamera'.");
            }

            Canfire = false;
            StartCoroutine(LockFire(TimeBetweenShots));
        }
    }
}

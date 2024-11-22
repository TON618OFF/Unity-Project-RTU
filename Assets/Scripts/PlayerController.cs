using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovmentSpeed = 5.0f;

    public float SprintSpeed = 10.0f;

    public float JumpForce = 5.0f;

    public float RotationSmothing = 20f;

    public float MouseSensitivity = 3.0f;

    public List<GameObject> WeaponInventory = new List<GameObject>();

    public List<GameObject> WeaponMeshes = new List<GameObject>();

    private int SelectedWeaponId = 0;

    private Weapon _Weapon;

    public GameObject HandMeshes;

    private GameManager _GameManager;

    private float pitch, yaw;

    private Rigidbody _Rigidbody;

    public bool IsGrounded;

    public float DistationToGround = 1.08f;

    private AnimationManager _AnimationManager;

    private bool IsSprinting = false;

    private void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _GameManager = FindObjectOfType<GameManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (WeaponInventory.Count > 0)
        {
            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
            WeaponMeshes[SelectedWeaponId].SetActive(true);
        }

        _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();
    }

    public void PickupWeapon(GameObject newWeapon, GameObject weaponModel)
    {
        // Добавление оружия и скина в списки
        WeaponInventory.Add(newWeapon);
        WeaponMeshes.Add(weaponModel);

        Debug.Log($"Оружие добавлено: {newWeapon.name}");
        Debug.Log($"Скин добавлен: {weaponModel.name}");

        // Проверка наличия AnimationManager
        SelectedWeaponId = WeaponMeshes.Count - 1; // Установите выбранное оружие как последнее добавленное
        _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
        _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

        if (_AnimationManager == null)
        {
            Debug.LogError($"AnimationManager не найден для оружия {WeaponMeshes[SelectedWeaponId].name}");
        }

        // Отключение коллайдера у оружия
        BoxCollider boxCollider = newWeapon.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // Настройка скина оружия
        weaponModel.transform.SetParent(HandMeshes.transform);
        weaponModel.transform.localPosition = new Vector3(0, 0.24f, 0.3f);
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.SetActive(false); // Отключить объект, чтобы он не отображался сразу

        Debug.Log("Подобрано оружие: " + newWeapon.GetComponent<Weapon>().WeaponType);
    }



    private void Jump()
    {
        _Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private void GroundCheck()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistationToGround);
    }

    private Vector3 CalculateMovment()
    {
        IsSprinting = false;

        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * HorizontalDirection + transform.forward * VerticalDirection;

        return _Rigidbody.transform.position + Move * Time.fixedDeltaTime * MovmentSpeed;
    }

    private Vector3 CalculateSprint()
    {
        IsSprinting = true;

        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * HorizontalDirection + transform.forward * VerticalDirection;

        return _Rigidbody.transform.position + Move * Time.fixedDeltaTime * SprintSpeed;
    }

    private void FixedUpdate()
    {
        GroundCheck();

        _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

        if (Input.GetKey(KeyCode.Space) && IsGrounded) Jump();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _Weapon.Fire();
            _AnimationManager.SetAnimationFire();
        }

        if (Input.GetKey(KeyCode.R))
        {
            _Weapon.Reload();
            _AnimationManager.SetAnimationReload();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) SelectNextWeapon();
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) SelectPrevWeapon();

        if (Input.GetKey(KeyCode.LeftShift) && !_GameManager.IsStaminaRestoring && _GameManager.Stamina > 0)
        {
            _GameManager.SpendStamina();
            _Rigidbody.MovePosition(CalculateSprint());
        }
        else
        {
            _Rigidbody.MovePosition(CalculateMovment());
        }

        SetRotation();

        SetAnimation();
    }

    private void OnDrawnGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * DistationToGround));
    }

    public void SetRotation()
    {
        yaw += Input.GetAxis("Mouse X") * MouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity;

        pitch = Mathf.Clamp(pitch, -60, 90);

        Quaternion SmoothRotation = Quaternion.Euler(pitch, yaw, 0);


        HandMeshes.transform.rotation = Quaternion.Slerp(HandMeshes.transform.rotation, SmoothRotation,
            RotationSmothing * Time.fixedDeltaTime);

        SmoothRotation = Quaternion.Euler(0, yaw, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, SmoothRotation, RotationSmothing * Time.fixedDeltaTime);
    }

    private void SelectNextWeapon()
    {
        if (WeaponInventory.Count > SelectedWeaponId + 1)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId += 1;
            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
            WeaponMeshes[SelectedWeaponId].SetActive(true);

            _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

            Debug.Log("Оружие: " + _Weapon.WeaponType);
        }

    }

    private void SelectPrevWeapon()
    {
        if (SelectedWeaponId > 0)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId -= 1;
            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
            WeaponMeshes[SelectedWeaponId].SetActive(true);

            _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

            Debug.Log("Оружие: " + _Weapon.WeaponType);
        }
    }

    private bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    private void SetAnimation()
    {
        if (_AnimationManager == null)
        {
            Debug.LogWarning("AnimationManager не назначен или равен null.");
            return;
        }

        if (IsMoving())
        {
            if (IsSprinting) _AnimationManager.SetAnimationRun();
            else _AnimationManager.SetAnimationWalk();
        }
        else
        {
            _AnimationManager.SetAnimationIdle();
        }
    }

}

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
        WeaponInventory.Add(newWeapon);
        WeaponMeshes.Add(weaponModel);

        Debug.Log($"Оружие добавлено: {newWeapon.name}");
        Debug.Log($"Скин добавлен: {weaponModel.name}");

        SelectedWeaponId = WeaponMeshes.Count - 1; 
        _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
        _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

        if (_AnimationManager == null)
        {
            Debug.LogError($"AnimationManager не найден для оружия {WeaponMeshes[SelectedWeaponId].name}");
        }

        BoxCollider boxCollider = newWeapon.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        weaponModel.transform.SetParent(HandMeshes.transform);
        weaponModel.transform.localPosition = new Vector3(0, 0.24f, 0.3f);
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.SetActive(false); 

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

        if (WeaponMeshes.Count > 0 && WeaponMeshes[SelectedWeaponId] != null)
        {
            _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();
        }

        if (Input.GetKey(KeyCode.Space) && IsGrounded) Jump();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (_Weapon != null)
            {
                _Weapon.Fire();
                if (_AnimationManager != null)
                {
                    _AnimationManager.SetAnimationFire();
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (_Weapon != null)
            {
                _Weapon.Reload();
                if (_AnimationManager != null)
                {
                    _AnimationManager.SetAnimationReload();
                }
            }
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
        if (WeaponInventory.Count == 0 || WeaponMeshes.Count == 0)
        {
            Debug.LogWarning("Нет оружия в инвентаре или оружейных скинов.");
            return;
        }

        if (SelectedWeaponId > 0)
        {
            WeaponMeshes[SelectedWeaponId].SetActive(false);  

            SelectedWeaponId -= 1;  

            if (WeaponInventory.Count > SelectedWeaponId)
            {
                _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapon>();
                _AnimationManager = WeaponMeshes[SelectedWeaponId].GetComponent<AnimationManager>();

                if (_Weapon == null)
                {
                    Debug.LogError("Компонент Weapon не найден для оружия " + WeaponInventory[SelectedWeaponId].name);
                }

                if (_AnimationManager == null)
                {
                    Debug.LogError("Компонент AnimationManager не найден для скина " + WeaponMeshes[SelectedWeaponId].name);
                }

                WeaponMeshes[SelectedWeaponId].SetActive(true); 

                Debug.Log("Оружие выбрано: " + _Weapon.WeaponType);
            }
            else
            {
                Debug.LogError("Оружие не существует по индексу " + SelectedWeaponId);
            }
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

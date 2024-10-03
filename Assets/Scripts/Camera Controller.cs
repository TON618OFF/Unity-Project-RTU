using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _sensivity;
    [SerializeField] private Transform _player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var horizontal = -Input.GetAxis("Mouse Y") * _sensivity * Time.deltaTime;
        var vertical = Input.GetAxis("Mouse X") * _sensivity * Time.deltaTime;

        _player.Rotate(0, vertical, 0);
        transform.Rotate(horizontal, 0, 0);
    }

}

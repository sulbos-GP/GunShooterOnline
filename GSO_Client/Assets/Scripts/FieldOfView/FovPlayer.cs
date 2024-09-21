using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class FovPlayer : MonoBehaviour
{
    private Vector2 lookInput;
    private Camera mainCamera;
    public AimFov aimFov;
    public BasicFov basicFov;

    public Gun playerGun;
    [SerializeField]
    private bool isFiring;

    private void Awake()
    {
        mainCamera = Camera.main;
        playerGun = transform.GetChild(0).GetComponent<Gun>();
    }

    private void OnEnable()
    {
        var playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Look.performed += OnLookInput;
        playerInput.Player.Fire.started += OnStartFireInput;
        playerInput.Player.Fire.canceled += OnStopFireInput;
        playerInput.Player.Reload.performed += OnReloadInput;
    }

    private void OnDisable()
    {
        var playerInput = new PlayerInput();
        playerInput.Player.Look.performed -= OnLookInput;
        playerInput.Player.Fire.started -= OnStartFireInput;
        playerInput.Player.Fire.canceled -= OnStopFireInput;
        playerInput.Player.Reload.performed -= OnReloadInput;
        playerInput.Player.Disable();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    private void OnStartFireInput(InputAction.CallbackContext context)
    {
        isFiring = true;
        StartCoroutine(FireContinuously());
    }

    private void OnStopFireInput(InputAction.CallbackContext context)
    {
        isFiring = false;
    }

    private IEnumerator FireContinuously()
    {
        while (isFiring)
        {
            playerGun.Fire(); // 발사 메서드 호출
            yield return new WaitForSeconds(playerGun.GetFireRate()); // 발사 속도에 따라 대기
        }
    }
    private void OnReloadInput(InputAction.CallbackContext context)
    {
        Debug.Log("RightClicked");
        playerGun.Reload();
    }

    private void Update()
    {
        Vector3 mousePosition = lookInput;
        mousePosition.z = -mainCamera.transform.position.z;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90f));
        
        aimFov.SetAimDirection(direction);
        aimFov.SetOrigin(transform.position);
        basicFov.SetOrigin(transform.position);
        
    }
}

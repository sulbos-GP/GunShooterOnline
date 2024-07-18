using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class InputController : MonoBehaviour
{
    static InputController instance;
    [SerializeField]
    private Vector2 _direction;
    public Rigidbody2D rig;
    private Unit localUnit;
    public GameObject LeftJoystick;


    private Vector2 lookInput;
    private Camera mainCamera;
    public AimFov aimFov;
    public BasicFov basicFov;

    private bool isFiring;

    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
    }

    public void Start()
    {
        rig= GetComponent<Rigidbody2D>();
        localUnit = GetComponent<Unit>(); // TO-DO : 추후에 instance에서 긁거나 localplayer[0]식으로 할 예정.
    }

    public void FixedUpdate()
    {
        //Move Logic
        Vector2 newVec2 = _direction * 5.0f * Time.fixedDeltaTime;
        rig.MovePosition(rig.position + newVec2);

        //Mouse Move Logic
        //Vector3 mousePosition = lookInput;
        //mousePosition.z = -mainCamera.transform.position.z;
        //mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        //mousePosition.z = 0f;


        //Vector3 direction = mousePosition - transform.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90f));

        //Fov Logic
        aimFov.SetAimDirection(lookInput);
        aimFov.SetOrigin(transform.position);
        basicFov.SetOrigin(transform.position);

    }
    private void OnEnable()
    {
        var playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Look.performed += OnLookInput;
        playerInput.Player.Fire.started += OnStartFireInput;
        playerInput.Player.Fire.canceled += OnStopFireInput;
        playerInput.Player.Reload.performed += OnReloadInput;
        playerInput.Player.Move.started += OnMove; ;
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        var playerInput = new PlayerInput();
        playerInput.Player.Look.performed -= OnLookInput;
        playerInput.Player.Fire.started -= OnStartFireInput;
        playerInput.Player.Fire.canceled -= OnStopFireInput;
        playerInput.Player.Reload.performed -= OnReloadInput;
        playerInput.Player.Move.started -= OnMove;
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
        playerInput.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.started)
        {
            
        }
        else if(callbackContext.canceled)
        {

        }
        Vector2 input = callbackContext.ReadValue<Vector2>();
        _direction = new Vector2(input.x,input.y);
    }

    private void OnMove(InputValue inputValue)
    {
        //TO-DO : 제거 시 조이스틱 오류 뜸.
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
            Gun playerGun = localUnit._guns[localUnit.CurGun];
            playerGun.Fire(); // 발사 메서드 호출
            yield return new WaitForSeconds(1 / playerGun.GetFireRate()); // 발사 속도에 따라 대기
        }
    }
    private void OnReloadInput(InputAction.CallbackContext context)
    {
        Debug.Log("RightClicked");
        Gun playerGun = localUnit._guns[localUnit.CurGun];
        playerGun.Reload();
    }
}

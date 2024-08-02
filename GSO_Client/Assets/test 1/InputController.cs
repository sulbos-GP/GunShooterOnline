using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Google.Protobuf.Protocol;
using UnityEngine.UIElements;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    private Rigidbody2D rig;

    
    private Vector2 _direction;
    private Vector2 lookInput;

    private Unit localUnit => UnitManager.Instance.CurrentPlayer;
    public AimFov aimFov;
    public BasicFov basicFov;




    private bool isFiring;
    //private CreatureState State;
    private Vector3 lastPos;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Managers.Network.ConnectToGame();
        rig = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        UpdateState();

        if(isFiring)
        {
            StartCoroutine(FireContinuously());
        }
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
        aimFov = GameObject.Find("AimView").GetComponent<AimFov>();
        basicFov = GameObject.Find("BasicView").GetComponent<BasicFov>();
        aimFov.SetAimDirection(lookInput);
        aimFov.SetOrigin(transform.position);
        basicFov.SetOrigin(transform.position);

    }
    private void OnEnable()
    {
        var playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Look.performed += OnLookInput;
        playerInput.Player.Look.canceled += OnLookInput;
        //playerInput.Player.Fire.started += OnStartFireInput;
        //playerInput.Player.Fire.canceled += OnStopFireInput;
        playerInput.Player.Reload.performed += OnReloadInput;
        playerInput.Player.Move.started += OnMove; ;
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        var playerInput = new PlayerInput();
        playerInput.Player.Look.performed -= OnLookInput;
        playerInput.Player.Look.canceled -= OnMove;
        //playerInput.Player.Fire.started -= OnStartFireInput;
        //playerInput.Player.Fire.canceled -= OnStopFireInput;
        playerInput.Player.Reload.performed -= OnReloadInput;
        playerInput.Player.Move.started -= OnMove;
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
        playerInput.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        //if(callbackContext.performed)
        //{
        //    State = CreatureState.Moving;
        //}
        //else if(callbackContext.canceled)
        //{
        //    State = CreatureState.Idle;
        //}
        Vector2 input = callbackContext.ReadValue<Vector2>();
        _direction = new Vector2(input.x,input.y);
    }

    private void OnMove(InputValue inputValue)
    {
        //TO-DO : 제거 시 조이스틱 오류 뜸.
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lookInput = context.ReadValue<Vector2>();
            if (Mathf.Abs(lookInput.x) + Mathf.Abs(lookInput.y) > 1.0f)
                isFiring = true;
        }
        if(context.canceled)
            isFiring = false;
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
            Debug.Log(isFiring);
            Gun playerGun = localUnit.GetComponentInChildren<Gun>();
            playerGun.Fire(); // 발사 메서드 호출
            yield return new WaitForSeconds(1 / playerGun.GetFireRate()); // 발사 속도에 따라 대기
        }
    }
    private void OnReloadInput(InputAction.CallbackContext context)
    {
        Debug.Log("RightClicked");
        Gun playerGun = localUnit.GetComponentInChildren<Gun>();
        playerGun.Reload();
    }

    private void UpdateState()
    {
        //switch (State)
        //{
        //    case CreatureState.Moving:
        //        UpdateMove();
        //        break;
        //    case CreatureState.Idle:
        //        break;
        //    default:
        //        break;
        //}
    }
    private void UpdateMove()
    {
        //Move Logic
        Vector2 newVec2 = _direction * 5.0f * Time.fixedDeltaTime;
        rig.MovePosition(rig.position + newVec2);
        UpdateServer();
    }

    private void UpdateServer()
    {
        if (lastPos == null || Vector3.Distance(lastPos, transform.position)>0.05f)
        {
            lastPos = transform.position;
            var movePack = new C_Move();
            movePack.PositionInfo = new PositionInfo
            {
                CurrentRoomId = 0,
                DirX = lookInput.x,
                DirY = lookInput.y,
                PosX = transform.position.x,
                PosY = transform.position.y,
                RotZ = transform.rotation.z,
            };
            Managers.Network.Send(movePack);
        }
    }
}

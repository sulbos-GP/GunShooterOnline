using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Google.Protobuf.Protocol;
using UnityEngine.Rendering;
using NPOI.SS.Formula.Functions;
using System;
using UnityEngine.UI;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public float distance;

    private Rigidbody2D rig;
    public PlayerInput playerInput;
    
    private Vector2 _direction;
    private Vector2 lookInput;

    //private Unit localUnit => UnitManager.Instance.CurrentPlayer;
    public AimFov aimFov;
    public BasicFov basicFov;

    //private CreatureState State;
    private Vector3 lastPos;

    public bool _isFiring; 
    private bool _isRooting; 

    public List<GameObject> interactList;
    public GameObject interactTarget;
    private Button interactBtn;
    private Animator animator;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;

    private void Awake()
    {
        instance = this;
        interactList = new List<GameObject>();
        interactBtn = GameObject.Find("InteractBtn").GetComponent<Button>();
        if(interactBtn == null) { Debug.Log("��ư�� ã������"); }
        interactBtn.interactable = false;
        interactBtn.onClick.AddListener(PlayerInteract);
    }

    public void Start()
    {
        //Managers.Network.ConnectToGame();
        rig = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(1).GetComponent<Animator>();
        playerSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        gunSpriteRenderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void FixedUpdate()
    {
        UpdateState();
        if(interactList.Count != 0)
        {
            interactBtn.interactable = true;
            ChooseInteractObj();
        }
        else
        {
            interactBtn.interactable = false;
            interactTarget = null;
        }

        if(_isFiring)
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
        var gunTrn = transform.GetChild(0);
        gunTrn.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90f));

        //Fov Logic
        aimFov = GameObject.Find("AimView").GetComponent<AimFov>();
        basicFov = GameObject.Find("BasicView").GetComponent<BasicFov>();
        aimFov.SetAimDirection(lookInput);
        aimFov.SetOrigin(transform.position);
        basicFov.SetOrigin(transform.position);

    }

    private void ChooseInteractObj()
    {
        float nearestDistance = 0;

        for (int i = 0; i < interactList.Count; i++)
        {
            float distance = Vector2.Distance(gameObject.transform.position, interactList[i].gameObject.transform.position);

            if (i == 0)
            {
                interactTarget = interactList[i].gameObject;
                nearestDistance = distance;
                continue;
            }

            if (nearestDistance < distance)
            {
                interactTarget = interactList[i].gameObject;
                nearestDistance = distance;
            }
        }
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Look.performed += OnLookInput;
        playerInput.Player.Look.canceled += OnLookInput;
        //playerInput.Player.Fire.started += OnStartFireInput;
        //playerInput.Player.Fire.canceled += OnStopFireInput;
        playerInput.Player.Reload.performed += OnReloadInput;
        playerInput.Player.Move.started += OnMove; ;
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
        playerInput.Player.Interaction.started += OnInteraction;
    }

    private void OnDisable()
    {
        playerInput.Player.Look.performed -= OnLookInput;
        playerInput.Player.Look.canceled -= OnMove;
        //playerInput.Player.Fire.started -= OnStartFireInput;
        //playerInput.Player.Fire.canceled -= OnStopFireInput;
        playerInput.Player.Reload.performed -= OnReloadInput;
        playerInput.Player.Move.started -= OnMove;
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Player.Move.canceled -= OnMove;
        playerInput.Player.Interaction.started -= OnInteraction;
        playerInput.Player.Disable();
        playerInput = null;
    }

    private void OnInteraction(InputAction.CallbackContext callbackContext)
    {
        PlayerInteract();
    }

    private void PlayerInteract()
    {
        //if(!_isRooting)
        //return;

        if (interactTarget == null)
        {
            return;
        }

        if (interactTarget.gameObject.GetComponent<Box>() != null)
        {
            interactTarget.gameObject.GetComponent<Box>().Interact();
        }
        else if (interactTarget.gameObject.GetComponent<ExitZone>() != null)
        {
            interactTarget.gameObject.GetComponent<ExitZone>().Interact();
        }
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
        if (callbackContext.started)
        {
            animator.SetBool("IsMove",true);
        }
        if (callbackContext.canceled)
        {
            animator.SetBool("IsMove", false);
        }
        Vector2 input = callbackContext.ReadValue<Vector2>();
        //�����̴� ���⿡ ���� �÷��̾��� ��������Ʈ ����
        if (input.x < 0)
        {
            playerSpriteRenderer.flipX = true;
        }
        else if (input.x > 0)
        {
            playerSpriteRenderer.flipX = false;
        }

        _direction = new Vector2(input.x,input.y);
    }

    private void OnMove(InputValue inputValue)
    {
        //TO-DO : ���� �� ���̽�ƽ ���� ��.
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lookInput = context.ReadValue<Vector2>();

            //�÷��̾ �̵��ϴ� ��ġ������ �÷��̾� ��������Ʈ�� �����ϵ� �� �̹��� ���� ������Ŵ
            //�̹����� ���� ������ ��������Ʈ ��ü�� ������ �����Ͽ� ����ȭ ��Ŵ.
            //�� ������ ���� ��������Ʈ�� ������ų������ z rotation ���� �������Ѿ���
            if (lookInput.x > 0)
            {
                gunSpriteRenderer.flipX = false;
                gunSpriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0, 45f);
                gunSpriteRenderer.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0, -45f);
            }
            else if (lookInput.x < 0)
            {
                gunSpriteRenderer.flipX = true;
                gunSpriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0, -45f);
                gunSpriteRenderer.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0, 45f);
            }

            if (Mathf.Abs(lookInput.x) + Mathf.Abs(lookInput.y) > 1.0f)
                _isFiring = true;
        }
        if(context.canceled)
            _isFiring = false;
    }
    private void OnStartFireInput(InputAction.CallbackContext context)
    {
        _isFiring = true;
        StartCoroutine(FireContinuously());
    }

    private void OnStopFireInput(InputAction.CallbackContext context)
    {
        _isFiring = false;
    }

    private IEnumerator FireContinuously()
    {
        while (_isFiring)
        {
            Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
            if(playerGun.CurGunState != GunState.Shootable)
            {
                break;
            }
            playerGun.Fire(); // �߻� �޼��� ȣ��
            yield return new WaitForSeconds(playerGun.GetFireRate());
        }
    }
    private void OnReloadInput(InputAction.CallbackContext context)
    {
        Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
        playerGun.Reload();
    }

    private void UpdateState()
    {
        UpdateMove();
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
        gameObject.GetComponent<MyPlayerController>().UpdateDrawLine();
        UpdateServer();
    }

    private void UpdateServer()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isRooting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isRooting=false;
    }
}

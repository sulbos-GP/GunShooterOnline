using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Google.Protobuf.Protocol;
using UnityEngine.UI;
using Unity.Burst.Intrinsics;

public class InputController : MonoBehaviour
{
    //singleton
    public static InputController instance;

    //Component
    private Animator animator;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;
    private Button interactBtn;

    //Input
    public PlayerInput playerInput;
    private Vector2 lookInput;
    private Vector2 direction;
    //private Vector3 lastPos; //didn't use

    //FOV Objects
    public AimFov aimFov;
    public BasicFov basicFov;

    //PlayerState
    public bool isFiring; 
    private bool isRooting; 

    //Interact
    public List<GameObject> interactList;
    public GameObject interactTarget;
    
    

    private bool onMove = false;
    private void Awake()
    {
        instance = this;

        SetComponent();

        if (interactBtn == null) {
            Debug.Log("버튼을 찾지못함"); 
            return; 
        }

        
        interactBtn.interactable = false;
        interactBtn.onClick.AddListener(InteractOn);
    }

    public void Start()
    {
        //Managers.Network.ConnectToGame();
        Aim(Vector2.up);
        UpdateMove();
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

    private void SetComponent()
    {
        interactBtn = GameObject.Find("InteractBtn").GetComponent<Button>();
        aimFov = GameObject.Find("AimView").GetComponent<AimFov>();
        basicFov = GameObject.Find("BasicView").GetComponent<BasicFov>();

        animator = transform.GetChild(1).GetComponent<Animator>();
        playerSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        gunSpriteRenderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        
        interactList = new List<GameObject>();
    }

    public void FixedUpdate()
    {
        UpdateState();
        HandleFiring();

        //Mouse Move Logic
        //Vector3 mousePosition = lookInput;
        //mousePosition.z = -mainCamera.transform.position.z;
        //mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        //mousePosition.z = 0f;

        //Vector3 direction = mousePosition - transform.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }

    private void HandleFiring()
    {
        if (isFiring)
        {
            StartCoroutine(FireContinuously());
        }
    }

    /// <summary>
    /// 인터렉트 가능한 오브젝트가 들어있는 리스트의 개수를 검색하고 1개 이상이면 인터렉트 실행 가능
    /// </summary>
    public void HandleInteraction()
    {
        if (interactList.Count > 0)
        {
            interactBtn.interactable = true;
            ChooseInteractObj();
        }
        else
        {
            interactBtn.interactable = false;
            interactTarget = null;
        }
    }

    /// <summary>
    /// 가장 가까운 인터렉트 가능 오브젝트를 도출
    /// </summary>
    private void ChooseInteractObj()
    {
        float nearestDistance = 0;

        for (int i = 0; i < interactList.Count; i++)
        {
            float distance = Vector2.Distance(gameObject.transform.position, interactList[i].transform.position);
            interactTarget = interactList[i].gameObject;
            nearestDistance = distance;
        }
    }

    

    private void OnInteraction(InputAction.CallbackContext callbackContext)
    {
        InteractOn();
    }

    private void InteractOn()
    {
        //if(!isRooting)
        //return;

        if (interactTarget == null)
        {
            return;
        }

        interactTarget.GetComponent<InteractableObject>().Interact();
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
            animator.SetBool("IsMove", true);
            onMove = true;
        }
        if (callbackContext.canceled)
        {
            animator.SetBool("IsMove", false);
            onMove = false;
            return; //여기서 리턴해줘야 방향이 초기화되지 않음
        }

        Vector2 input = callbackContext.ReadValue<Vector2>();
        FlipPlayerSprite(input.x);

        if (!isFiring) //플레이어가 사격 조이스틱을 조정하지 않는 상태라면 이동 방향을 바라봄
        {
            Aim(input);
        }

        direction = new Vector2(input.x, input.y);
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

            Aim(lookInput);

            if (Mathf.Abs(lookInput.x) + Mathf.Abs(lookInput.y) > 1.0f)
                isFiring = true;
        }

        else if (context.canceled)
        {
            isFiring = false;
        }
            
    }

    /*
    private void OnStartFireInput(InputAction.CallbackContext context)
    {
        isFiring = true;
        StartCoroutine(FireContinuously());
    }

    private void OnStopFireInput(InputAction.CallbackContext context)
    {
        isFiring = false;
    }*/

    private void OnReloadInput(InputAction.CallbackContext context)
    {
        Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
        playerGun.Reload();
    }


    private void FlipPlayerSprite(float inputX)
    {
        if (inputX < 0)
        {
            playerSpriteRenderer.flipX = true;
        }
        else if (inputX > 0)
        {
            playerSpriteRenderer.flipX = false;
        }
    }

    private void Aim(Vector2 dir)
    {
        //pivot 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var gunTrn = transform.GetChild(0);
        gunTrn.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

        aimFov.SetAimDirection(dir); //fov 회전
        FlipGunSprite(dir.x);
    }

    private void FlipGunSprite(float inputX)
    {
        //방향에 따라 총의 x회전을 플립
        if (inputX > 0)
        {
            gunSpriteRenderer.flipX = false;
            gunSpriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0, 45f);
            gunSpriteRenderer.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0, -45f);
        }
        else if (inputX < 0)
        {
            gunSpriteRenderer.flipX = true;
            gunSpriteRenderer.transform.localRotation = Quaternion.Euler(0f, 0, -45f);
            gunSpriteRenderer.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0, 45f);
        }
    }

    
    private IEnumerator FireContinuously()
    {
        while (isFiring)
        {
            Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
            if(playerGun.UsingGunState != GunState.Shootable)
            {
                break;
            }

            playerGun.Fire(lookInput); // 발사 메서드 호출
            yield return new WaitForSeconds(playerGun.GetFireRate());
        }
    }
    
    private void UpdateState()
    {
        if(onMove)
        { 
            UpdateMove(); 
        }
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
        Vector2 newVec2 = direction * 5.0f * Time.fixedDeltaTime;

        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        rig.MovePosition(rig.position + newVec2);

        aimFov.SetOrigin(transform.position); //fov의 중심을 플레이어의 위치로 설정
        basicFov.SetOrigin(transform.position);


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
        isRooting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isRooting=false;
    }
}

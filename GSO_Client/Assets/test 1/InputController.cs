using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Google.Protobuf.Protocol;
using UnityEngine.UI;
using Unity.Burst.Intrinsics;
//using static UnityEditor.Progress;


public class InputController : MonoBehaviour
{
    //singleton
    public static InputController instance;

    //Component
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer playerSpriteRenderer;
    private SpriteRenderer gunSpriteRenderer;
    private Button interactBtn;

    //Input
    public PlayerInput playerInput;
    private Vector2 lookInput;
    private Vector2 direction;
    private float moveInterval = 0.5f;
    private float timer = 0.0f;
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
            Debug.Log("��ư�� ã������"); 
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
        playerInput.Player.UIActive.performed += SetUIActive;
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
        playerInput.Player.UIActive.performed -= SetUIActive;
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
        audioSource = GetComponent<AudioSource>();
        playerSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        gunSpriteRenderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        
        interactList = new List<GameObject>();
    }

    public void FixedUpdate()
    {
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }
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
    /// ���ͷ�Ʈ ������ ������Ʈ�� ����ִ� ����Ʈ�� ������ �˻��ϰ� 1�� �̻��̸� ���ͷ�Ʈ ���� ����
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
    /// ���� ����� ���ͷ�Ʈ ���� ������Ʈ�� ����
    /// </summary>
    private void ChooseInteractObj()
    {
        float nearestDistance = float.MaxValue; // 초기값 설정
        GameObject nearestTarget = null;

        for (int i = 0; i < interactList.Count; i++)
        {
            var currentObject = interactList[i];
            float distance = Vector2.Distance(gameObject.transform.position, currentObject.transform.position);
            if (currentObject.GetComponent<Box>() != null)
            {
                var mat = currentObject.transform.GetChild(0).GetComponent<SpriteRenderer>().material;
                mat.DisableKeyword("OUTBASE_ON");
                mat.DisableKeyword("INNEROUTLINE_ON");
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = currentObject;
            }
        }

        interactTarget = nearestTarget;
        if (interactTarget != null)
        {
            if (interactTarget.GetComponent<Box>() != null)
            {
                var mat = interactTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().material;
                mat.EnableKeyword("OUTBASE_ON");
                mat.EnableKeyword("INNEROUTLINE_ON");
            }
        }
    }

    private void SetUIActive(InputAction.CallbackContext callbackContext)
    {
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }

        UIManager.Instance.SetUIActive();
    }

    private void OnInteraction(InputAction.CallbackContext callbackContext)
    {
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }

        InteractOn();
    }

    private void InteractOn()
    {
        //if(!isRooting)
        //return;
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }

        if (interactTarget == null)
        {
            return;
        }

        interactTarget.GetComponent<InteractableObject>().Interact();
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }

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
            //audioSource.Play();
            AudioManager.instance.PlaySound("Move",audioSource);
            onMove = true;
        }
        if (callbackContext.canceled)
        {
            animator.SetBool("IsMove", false);
            //audioSource.Stop();
            AudioManager.instance.StopSound(audioSource);
            onMove = false;
            UpdateMove();
            return; //���⼭ ��������� ������ �ʱ�ȭ���� ����
        }

        Vector2 input = callbackContext.ReadValue<Vector2>();
        FlipPlayerSprite(input.x);

        if (!isFiring) //�÷��̾ ��� ���̽�ƽ�� �������� �ʴ� ���¶�� �̵� ������ �ٶ�
        {
            Aim(input);
        }

        direction = new Vector2(input.x, input.y);
    }

    private void OnMove(InputValue inputValue)
    {
        //TO-DO : ���� �� ���̽�ƽ ���� ��.
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }

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
        if (Managers.Object.MyPlayer.IsDead)
        {
            return;
        }

        Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
        playerGun.Reload();
    }



    private void moveSound()
    {
        if(onMove && timer > moveInterval)
        {
            Debug.Log("�ȴ���");
            timer = 0.0f;
            audioSource.PlayOneShot(audioSource.clip);
        }
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

    private void Aim(Vector2 dir) //돌리기
    {
        //pivot 회전



        /*float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var gunTrn = transform.GetChild(0);
        gunTrn.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));


        Debug.Log("Dir" + angle);

        C_InputData inputPacket = new C_InputData();
        inputPacket.GunRoation = angle;

        Managers.Network.Send(inputPacket);*/



        Gun g = transform.GetChild(0).GetChild(0).GetComponent<Gun>();
        if(g != null)
        {
            g.SetGunRoation(dir);
        }


        aimFov.SetAimDirection(dir); //fov ȸ��
        FlipGunSprite(dir.x);
    }




    private void FlipGunSprite(float inputX)
    {
        //���⿡ ���� ���� xȸ���� �ø�

        //float newAngle = MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed);
        //Debug.Log(inputX);

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

    
    private IEnumerator FireContinuously() // 총 쏘기
    {
        while (isFiring)
        {
            Gun playerGun = Managers.Object.MyPlayer.GetComponentInChildren<Gun>();
            if(playerGun.gunState != GunState.Shootable)
            {
                break;
            }
            playerGun.Fire(lookInput); // �߻� �޼��� ȣ��
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
        if(onMove)
            rig.MovePosition(rig.position + newVec2);

        aimFov.SetOrigin(transform.position); //fov�� �߽��� �÷��̾��� ��ġ�� ����
        basicFov.SetOrigin(transform.position);

#if UNITY_EDITOR
        gameObject.GetComponent<DebugShape>().UpdateDrawLine();
#endif
        UpdateServer();
    }


    Vector2 last;
    private void UpdateServer()
    {

        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);

        // 위치 차이가 0.1 이상일 때만 실행
        if (Vector2.Distance(last, currentPosition) > 0.1f)
        {
            var movePack = new C_Move();
            movePack.PositionInfo = new PositionInfo
            {
                /*Tick = 0,*/
                DirX = lookInput.x,
                DirY = lookInput.y,
                PosX = transform.position.x,
                PosY = transform.position.y,
                RotZ = transform.rotation.z,
            };

            Managers.Network.Send(movePack);

            // 마지막 위치를 현재 위치로 갱신
            last = currentPosition;
        }
        if (!onMove) // Idle 상태 전달을 위한 마지막 패킷
        {
            var movePack = new C_Move();
            movePack.PositionInfo = new PositionInfo
            {
                /*Tick = 0,*/
                DirX = lookInput.x,
                DirY = lookInput.y,
                PosX = transform.position.x,
                PosY = transform.position.y,
                RotZ = transform.rotation.z,
            };

            Managers.Network.Send(movePack);
        }
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

using MathNet.Numerics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyPlayer : MonoBehaviour
{
    public PlayerInput playerInput;

    private Vector2 _direction;

    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer spriteRenderer;



    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMove();
    }

    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.Move.started += OnMove; ;
        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;
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
        if(callbackContext.started)
        {
            animator.SetTrigger("MoveState");
        }
        if(callbackContext.canceled)
        {
            Debug.Log("cancle");
            animator.SetTrigger("MoveState");
        }
        Vector2 input = callbackContext.ReadValue<Vector2>();
        if (input.x < 0)
            spriteRenderer.flipX = true;
        else if (input.x > 0)
            spriteRenderer.flipX = false;
        _direction = new Vector2(input.x, input.y);
    }

    private void OnMove(InputValue inputValue)
    {
        //TO-DO : Á¦°Å ½Ã Á¶ÀÌ½ºÆ½ ¿À·ù ¶ä.
    }

    private void UpdateMove()
    {
        //Move Logic
        Vector2 newVec2 = _direction * 5.0f * Time.fixedDeltaTime;
        rig.MovePosition(rig.position + newVec2);
    }
}

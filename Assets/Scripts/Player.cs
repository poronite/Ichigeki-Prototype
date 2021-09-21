using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

//Reference: https://www.youtube.com/watch?v=4HpC--2iowE

public class Player : MonoBehaviour
{
    public enum MovementState {Idle, Moving, Dashing, Jumping};
    public MovementState CurrentMovementState = MovementState.Idle;

    public enum AttackState {NotAttacking, ReadyingAttack, AttackReady, ReleaseAttack, Parry};
    public AttackState CurrentAttackState = AttackState.NotAttacking;

    private PlayerInput input;
    private Vector2 move;
    private Vector3 moveDirection;
    private float dashCooldownTimer = 0, dashStackAmount = 0;
    private float rotationVelocity;
    private float swingingSpeedMultiplier = 1, gravity = -9.8f, velocityY; //this is the speed of the character when readying the attack or jump attacking

    [SerializeField]
    private CharacterController playerController = null;

    [SerializeField]
    private CinemachineFreeLook CameraController = null;

    [SerializeField]
    private Transform Camera = null;

    [SerializeField]
    private Animator PlayerAnimator = null;

    [SerializeField]
    private MeshRenderer swordRenderer = null;

    [SerializeField]
    private Material releaseAttackMaterial = null, swordMaterial = null;

    [SerializeField]
    private float speed = 1, dashSpeed = 1, dashDuration = 1, dashStackMax = 1, dashCooldown = 1, rotationTime = 0, jumpHeight = 0, jumpGravityScale = 1, fallGravityScale = 1;


    private void Awake()
    {
        SetInputs();
        dashStackAmount = dashStackMax;
    }

    private void SetInputs()
    {
        input = new PlayerInput();

        input.Player.Look.performed += context =>
        {
            Vector2 look = context.ReadValue<Vector2>();
            CameraController.m_XAxis.m_InputAxisValue = look.x;
            CameraController.m_YAxis.m_InputAxisValue = -look.y;
        };

        input.Player.Look.canceled += context =>
        {
            CameraController.m_XAxis.m_InputAxisValue = 0;
            CameraController.m_YAxis.m_InputAxisValue = 0;
        };

        input.Player.Move.performed += context => 
        {
            if (!(CurrentMovementState == MovementState.Dashing))
            {
                move = context.ReadValue<Vector2>();
                CurrentMovementState = MovementState.Moving;
            }
            
        };
        
        input.Player.Move.canceled += context => 
        {
            move = Vector2.zero;
            if (!(CurrentMovementState == MovementState.Dashing))
                CurrentMovementState = MovementState.Idle;
        };

        input.Player.Dash.performed += context => 
        {
            if (dashStackAmount > 0)
            {
                if (CurrentMovementState == MovementState.Moving)
                {
                    dashStackAmount -= 1;
                    CurrentMovementState = MovementState.Dashing;
                    StartCoroutine(Dash());
                }
            }
        };

        input.Player.Attack.performed += context =>
        {
            if (CurrentAttackState == AttackState.NotAttacking)
            {
                CurrentAttackState = AttackState.ReadyingAttack;
                PlayerAnimator.Play("ReadyAttack", 0);
            }
        };

        input.Player.Attack.canceled += context =>
        {
            switch (CurrentAttackState)
            {
                case AttackState.ReadyingAttack:
                    CurrentAttackState = AttackState.NotAttacking;
                    PlayerAnimator.Play("NotAttacking", 0);
                    break;
                case AttackState.AttackReady:
                    CurrentAttackState = AttackState.ReleaseAttack;
                    swordRenderer.material = releaseAttackMaterial;
                    PlayerAnimator.Play("ReleaseAttack", 0);
                    break;                
                default:
                    break;
            }
        };

        input.Player.Jump.performed += context =>
        {
            if (playerController.isGrounded && CurrentAttackState == AttackState.AttackReady)
            {
                swordRenderer.material = swordMaterial;
                PlayerAnimator.Play("JumpImpulse", 0);
            }
        };
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Movement()
    {
        Vector3 direction = new Vector3(move.x, 0.0f, move.y).normalized;

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationTime);

            switch (CurrentAttackState)
            {
                case AttackState.NotAttacking:
                    swingingSpeedMultiplier = 1;
                    transform.rotation = Quaternion.Euler(0f, rotation, 0f);
                    break;
                case AttackState.ReadyingAttack:
                case AttackState.AttackReady:
                case AttackState.ReleaseAttack:              
                case AttackState.Parry:
                    swingingSpeedMultiplier = 0.7f;
                    break;
                default:
                    break;
            }            
            
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            playerController.Move(((moveDirection.normalized * speed * swingingSpeedMultiplier) + new Vector3(0, velocityY, 0)) * Time.deltaTime);            
        }
        else
        {
            playerController.Move(new Vector3(0, velocityY, 0) * Time.deltaTime);
        }
    }

    IEnumerator Dash() 
    {
        if (CurrentMovementState == MovementState.Dashing)
        {
            float dashStartTime = Time.time;
            float dashEndTime = dashStartTime + dashDuration;

            while (Time.time < dashEndTime)
            {
                playerController.Move(moveDirection * dashSpeed * Time.deltaTime);
                yield return null;
            }

            CurrentMovementState = MovementState.Idle;
        }
    }

    private void RestoreDashStack()
    {
        if (dashStackAmount < dashStackMax)
        {
            dashCooldownTimer += Time.deltaTime;
        }

        if (dashCooldownTimer >= dashCooldown)
        {
            dashCooldownTimer = 0;
            dashStackAmount += 1;
            Debug.Log($"DashReady | Dash Stacks: {dashStackAmount}");
        }
    }

    private void CustomGravity()
    {
        if (playerController.velocity.y > 0)
        {
            velocityY += gravity * jumpGravityScale * Time.deltaTime;
        }
        else if (playerController.velocity.y < 0)
        {
            velocityY += gravity * fallGravityScale * Time.deltaTime;
        }
    }

    public void Jump()
    {
        velocityY = Mathf.Sqrt(jumpHeight * -2f * (gravity * jumpGravityScale));
    }


    void Update()
    {
        CustomGravity();

        RestoreDashStack();

        Movement();
    }
}

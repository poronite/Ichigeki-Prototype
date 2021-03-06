using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackReady : MonoBehaviour
{
    public Player PlayerController;

    [SerializeField]
    private MeshRenderer swordRenderer = null;

    [SerializeField]
    private Material attackReadyMaterial = null, swordMaterial = null;

    public void ReadyAttack()
    {
        PlayerController.CurrentAttackState = Player.AttackState.AttackReady;
        swordRenderer.material = attackReadyMaterial;
    }

    public void EndAttack()
    {
        PlayerController.CurrentAttackState = Player.AttackState.NotAttacking;
        swordRenderer.material = swordMaterial;
        if (!PlayerController.PlayerController.isGrounded)
        {
            PlayerController.PlayerAnimator.Play("AirborneReleaseAttack");
        }
    }

    public void JumpImpulse()
    {
        PlayerController.CurrentAttackState = Player.AttackState.AttackReady;
        PlayerController.CurrentMovementState = Player.MovementState.Jumping;
        swordRenderer.material = attackReadyMaterial;
        PlayerController.Jump();
    }
}

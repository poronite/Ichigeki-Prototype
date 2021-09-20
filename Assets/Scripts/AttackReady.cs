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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDoll : MonoBehaviour
{
    [SerializeField]
    private Player PlayerController = null;

    [SerializeField]
    private Material trainingDollMaterial = null, hitFeedbackMaterial = null;

    [SerializeField]
    private MeshRenderer trainingDollRenderer = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GodSword") && PlayerController.CurrentAttackState == Player.AttackState.ReleaseAttack)
        {
            StartCoroutine(GetHit());
        }
    }

    IEnumerator GetHit()
    {
        trainingDollRenderer.material = hitFeedbackMaterial;

        yield return new WaitForSeconds(1.0f);

        trainingDollRenderer.material = trainingDollMaterial;

        yield return null;
    }
}

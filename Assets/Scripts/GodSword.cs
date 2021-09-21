using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSword : MonoBehaviour
{
    [SerializeField]
    private Player playerController;

    [SerializeField]
    private MeshRenderer trainingDollRenderer = null;

    [SerializeField]
    private Material trainingDollMaterial = null, hitFeedbackMaterial = null;

    [SerializeField]
    private bool isStopped = false;

    [SerializeField]
    private float stopTimeDuration = 0.0f, slowTimeDuration = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && playerController.CurrentAttackState == Player.AttackState.ReleaseAttack)
        {
            trainingDollRenderer = other.GetComponent<MeshRenderer>();

            StartCoroutine(GetHit(stopTimeDuration, slowTimeDuration));
        }
    }

    IEnumerator GetHit(float stopTimeDuration, float slowTimeDuration)
    {
        trainingDollRenderer.material = hitFeedbackMaterial;

        if (!isStopped)
        {
            isStopped = true;
            Time.timeScale = 0.0f;

            yield return new WaitForSecondsRealtime(stopTimeDuration);

            Time.timeScale = 0.01f;

            yield return new WaitForSecondsRealtime(slowTimeDuration);

            Time.timeScale = 1.0f;
            isStopped = false;
        }

        yield return new WaitForSeconds(1.0f);

        trainingDollRenderer.material = trainingDollMaterial;

        yield return null;
    }
}

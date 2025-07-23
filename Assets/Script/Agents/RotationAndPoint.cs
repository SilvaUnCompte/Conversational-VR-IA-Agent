using System.Collections;
using UnityEngine;

public class RotationAndPoint : MonoBehaviour
{
    public float turnDuration = 0.5f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is not assigned or found on the GameObject.");
        }

        animator.SetBool("isTurning", false);
        animator.SetInteger("pointDirection", 0);
        animator.ResetTrigger("point");
    }

    public void LookAt(Vector3 target)
    {
        StopAllCoroutines(); // avoid multiples rotations
        animator.SetBool("isTurning", true);
        StartCoroutine(RotateTo(target));
    }

    public void LookAtMe(Vector3 target)
    {
        // Si la rotation est petite, on ne fait pas de rotation
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.magnitude > 0.2f)
        {
            LookAt(target);
        }
    }

    private IEnumerator RotateTo(Vector3 target)
    {
        Quaternion initalRotation = transform.rotation;
        Vector3 direction = target - transform.position;
        direction.y = 0f; // Keep the rotation on the horizontal plane

        if (direction == Vector3.zero)
            yield break;

        Quaternion finalRotation = Quaternion.LookRotation(direction.normalized);
        float tempTime = 0f;

        while (tempTime < turnDuration)
        {
            tempTime += Time.deltaTime;
            float t = tempTime / turnDuration;
            transform.rotation = Quaternion.Slerp(initalRotation, finalRotation, t);
            yield return null;
        }

        transform.rotation = finalRotation;
        animator.SetBool("isTurning", false);
    }

    public void PointTo(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // On ignore l'axe vertical
        directionToTarget.y = 0;
        Vector3 forward = transform.forward;
        forward.y = 0;

        // Angle entre l'avant du personnage et la direction de la cible
        float angle = Vector3.SignedAngle(forward, directionToTarget, Vector3.up);

        // Cas selon l'angle
        if (angle >= -15f && angle <= 15f)
        {
            // Devant
            animator.SetInteger("pointDirection", 1);
        }
        else if (angle <= -160f || angle >= 160f)
        {
            // Derrière
            animator.SetInteger("pointDirection", 2);
        }
        else if (angle > 40f && angle < 80f)
        {
            // Très à droite
            animator.SetInteger("pointDirection", 4);
        }
        else if (angle < -40f && angle > -80f)
        {
            // Très à gauche
            animator.SetInteger("pointDirection", 3);
        }
        else
        {
            // Autres cas : tourner vers la cible, puis lancer animation
            animator.SetInteger("pointDirection", 1);
            LookAt(targetPosition);
        }
        animator.SetTrigger("point");
    }
}

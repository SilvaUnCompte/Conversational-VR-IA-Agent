using UnityEngine;

public class CheckingHand : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;       // Ce que l'avatar regarde
    [SerializeField] Transform rightHandTarget;    // Position cible pour la main droite

    private Animator animator;
    public bool hanging = false;
    public bool ikActive = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ProposeHand()
    {
        transform.rotation = Quaternion.Euler(0, -35, 0) * transform.rotation;
        animator.SetBool("isGivingHand", true);
        animator.SetTrigger("giveHand");
        Invoke("ActivateIK", 0.5f);
    }
    private void ActivateIK()
    {
        ikActive = true;
    }

    void OnAnimatorIK(int layerIndex)
    {
        Vector3 lookAt = lookAtTarget.position + Vector3.up * 1.5f; // Ajuster la hauteur pour éviter de regarder le sol
        animator.SetLookAtPosition(lookAt);

        if (ikActive)
        {
            // Regarder un objet
            animator.SetLookAtWeight(1f);

            if (hanging)
            {
                // Main droite vers une cible
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                Quaternion rotated = rightHandTarget.rotation * Quaternion.Euler(180, 120, 0);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rotated);

            }
        }
        else
        {
            // Réinitialiser
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }
    }
}

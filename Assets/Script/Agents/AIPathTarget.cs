using System;
using UnityEngine;
using UnityEngine.AI;

public class AIPathTarget : MonoBehaviour
{
    private NavMeshAgent meshAgent;
    private IAVisionManager iAVisionManager;
    private Animator animator;

    private Vector3 StaticTarget;
    private GameObject DynamicTarget;
    private bool isDynamicTarget = false;

    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        iAVisionManager = GetComponent<IAVisionManager>();
        animator = GetComponent<Animator>();

        if (!iAVisionManager || !meshAgent)
        {
            Debug.LogError("IAVisionManager or NavMeshAgent not found on the GameObject.");
            return;
        }
        
        StaticTarget = transform.position;
    }

    public void SetTarget(Vector3 newTarget)
    {
        foreach (RegisteredGameObjects obj in iAVisionManager.GetVisionObjects())
        {
            if (Math.Round(obj.registeredPos.x, 2) == Math.Round(newTarget.x, 2) &&
                Math.Round(obj.registeredPos.y, 2) == Math.Round(newTarget.y, 2) &&
                Math.Round(obj.registeredPos.z, 2) == Math.Round(newTarget.z, 2))
            {
                isDynamicTarget = true;
                DynamicTarget = obj.gameObject;
                Debug.Log($"Dynamic target set to: {DynamicTarget.name} at position {DynamicTarget.transform.position}");
                return; // Exit after finding the first matching dynamic target
            }
        }

        isDynamicTarget = false; // Reset to false if no dynamic target found
        StaticTarget = newTarget;
        Debug.Log($"Static target set to: ({StaticTarget.x};{StaticTarget.y};{StaticTarget.z})");
    }

    private void Update()
    {
        if (isDynamicTarget && DynamicTarget != null)
        {
            meshAgent.SetDestination(DynamicTarget.transform.position);
        }
        else
        {
            meshAgent.SetDestination(StaticTarget);
        }

        if (animator)
        {
            if (meshAgent.velocity.magnitude > 0.1f)
            {
                animator.ResetTrigger("point");
                animator.ResetTrigger("talk");
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
}

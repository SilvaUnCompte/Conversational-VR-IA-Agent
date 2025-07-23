using ReadyPlayerMe.Core;
using UnityEngine;
using static UnityEditor.VersionControl.Message;

public class HandManager : MonoBehaviour
{
    Animator animator;
    VoiceHandler voiceHandler;
    AudioSource audioSource;
    [SerializeField] GameObject Avatar;
    [SerializeField] AudioClip introClip;
    int security;

    private void Start()
    {
        security = 0;
        animator = Avatar.GetComponent<Animator>();
        voiceHandler = Avatar.GetComponent<VoiceHandler>();
        audioSource = Avatar.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered: " + other.gameObject.name);
        if (other.gameObject.name != "RightMiddleProximityCollider") return;
        Avatar.GetComponent<CheckingHand>().hanging = true;
        Invoke("StopHanging", 2f);
    }

    void StopHanging()
    {
        if (security == 1) return; security = 1;

        Avatar.GetComponent<CheckingHand>().hanging = false;
        Avatar.GetComponent<CheckingHand>().ikActive = false;
        Avatar.GetComponent<Animator>().SetBool("isGivingHand", false);

        animator.SetTrigger("talk");
        voiceHandler.AudioClip = introClip;
        audioSource.resource = introClip;
        voiceHandler.PlayCurrentAudioClip();

        Invoke("PointToShow", 21f);
    }

    void PointToShow()
    {
        animator.SetLookAtWeight(0);
        Avatar.GetComponent<AgentActionManager>().ExecuteAction("{\"pointTo\":[\"-10.5\",\"1.1\",\"30.736\"]}");
    }
}

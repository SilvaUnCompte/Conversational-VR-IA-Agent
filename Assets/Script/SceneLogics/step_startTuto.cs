using ReadyPlayerMe.Core;
using UnityEngine;
using static UnityEditor.VersionControl.Message;

public class step_startTuto : MonoBehaviour
{
    GameObject bob1;
    GameObject globalManager;
    Animator animator;
    VoiceHandler voiceHandler;
    AudioSource audioSource;
    [SerializeField] AudioClip bonjourClip;
    [SerializeField] AudioClip introClip;

    private void Start()
    {
        bob1 = GameObject.Find("Bob1");
        animator = bob1.GetComponent<Animator>();
        voiceHandler = bob1.GetComponent<VoiceHandler>();
        audioSource = bob1.GetComponent<AudioSource>();

        globalManager = GameObject.Find("GlobalManager");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "User") return;
        bob1.GetComponent<AgentActionManager>().ExecuteAction("{\"moveTo\":[\"-12.12\",\"1.1\",\"17.7\"]}");
        Invoke("speachIntro", 2.7f);

        if (globalManager.GetComponent<InitXP>().Haptic)
        {
            Invoke("waitHand", 3f);
        }
        else
        {
            Invoke("dontWaitHand", 4.5f);
        }


        GetComponent<Collider>().enabled = false; // Désactiver le collider pour éviter les déclenchements multiples
    }

    private void speachIntro()
    {
        animator.SetTrigger("talk");

        voiceHandler.AudioClip = bonjourClip;
        audioSource.resource = bonjourClip;
        voiceHandler.PlayCurrentAudioClip();
    }
    private void waitHand()
    {
        bob1.GetComponent<CheckingHand>().ProposeHand();
    }
    private void dontWaitHand()
    {
        animator.SetTrigger("talk");
        voiceHandler.AudioClip = introClip;
        audioSource.resource = introClip;
        voiceHandler.PlayCurrentAudioClip();

        Invoke("PointToShow", 21f);
    }
    void PointToShow()
    {
        animator.SetLookAtWeight(0);
        bob1.GetComponent<AgentActionManager>().ExecuteAction("{\"pointTo\":[\"-10.5\",\"1.1\",\"30.736\"]}");
    }
}

using UnityEngine;
using System.Linq;

public class MachineManager : MonoBehaviour
{
    AudioSource audioSource;
    GameObject bob;
    GameObject endXPDoor;
    public string[] part;

    void Start()
    {
        part = new string[3] { "Cube A59", "Barre B42", "Batterie F14" };
        bob = GameObject.Find("Bob");
        endXPDoor = GameObject.Find("EndXPDoor");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the GameObject.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);

        if (part.Contains(other.gameObject.name))
        {
            audioSource.Play();
            Debug.Log("Part collected: " + other.gameObject.name);
            part = part.Where(p => p != other.gameObject.name).ToArray();

            if (part.Length == 0)
            {
                Debug.Log("Victoire");
                bob.GetComponent<TTS_GCloud>().Say("Parfait, c'est tout ce qu'il me faut ! Maintenant sort du labo par l'autre porte.");
                endXPDoor.GetComponent<DoorManager>().Open();
                Logger.SaveActionsToCSV();
            }
        }
    }
}

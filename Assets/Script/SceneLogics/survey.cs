using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Survey : MonoBehaviour
{
    [SerializeField] ToggleGroup[] toggleGroups;
    AudioSource audioSource;
    GameObject survey;
    GameObject bob;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        survey = GameObject.Find("Survey");
        bob = GameObject.Find("Bob");
    }
    public void SubmitSurvey()
    {
        foreach (ToggleGroup group in toggleGroups)
        {
            if (group.AnyTogglesOn())
            {
                Toggle selectedToggle = group.ActiveToggles().FirstOrDefault();
                if (selectedToggle != null)
                {
                    Logger.AddSurveyEntry(group.name, selectedToggle.GetComponentInChildren<Text>().text);
                }
            }
        }

        Logger.SaveSurveyToCSV();
        audioSource.Play();
        survey.transform.position = new Vector3(0, -100, 0); // Move survey off-screen

        bob.GetComponent<AgentActionManager>().ExecuteAction("{\"message\":\"Merci d'avoir participer. Vous pouvez maintenant retirer le casque.\"}");
    }
}

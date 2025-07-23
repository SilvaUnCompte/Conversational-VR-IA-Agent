using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentActionManager : MonoBehaviour
{
    TTS_GCloud TTS_instance;
    AIPathTarget aiPathTarget;
    RotationAndPoint rotationAndPoint;
    NavMeshAgent navMeshAgent;
    public string GoogleToken;

    // TODO: remove pour réutiliser le script
    GameObject bob2;

    private void Start()
    {
        rotationAndPoint = GetComponent<RotationAndPoint>();
        aiPathTarget = GetComponent<AIPathTarget>();
        TTS_instance = GetComponent<TTS_GCloud>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (TTS_instance == null)
        {
            Debug.LogWarning("TTS_GCloud component not found on the GameObject.");
        }
        if (aiPathTarget == null)
        {
            Debug.LogWarning("AIPathTarget component not found on the GameObject.");
        }
        if (rotationAndPoint == null)
        {
            Debug.LogWarning("RotationAndPoint component not found on the GameObject.");
        }
        if (navMeshAgent == null)
        {
            Debug.LogWarning("NavMeshAgent component not found on the GameObject.");
        }
        StartCoroutine(WaitForToken());

        // TODO: remove pour réutiliser le script
        bob2 = GameObject.Find("Bob");
    }


    private IEnumerator WaitForToken()
    {
        // Attendre que le token soit prêt
        yield return new WaitUntil(() => GoogleAuthToken.Instance != null && GoogleAuthToken.Instance.IsTokenReady);
        GoogleToken = GoogleAuthToken.Instance.AccessToken;
    }

    public void ExecuteAction(string jsonResponse)
    {
        try
        {
            Debug.Log("Executing action with JSON response: " + jsonResponse);

            JsonLLMActionResponse response = JsonConvert.DeserializeObject<JsonLLMActionResponse>(jsonResponse);
            JsonVecLLMActionResponse vecResponse = TextUtils.convertLLMResponseToVec(response);

            if (TTS_instance && response.message != null && response.message.Trim().Length > 0)
            {
                Debug.Log("Saying message: " + response.message);
                TTS_instance.Say(response.message);
            }

            if (response.lookAt != null)
            {
                Debug.Log("Turning to: " + vecResponse.lookAt);
                navMeshAgent.enabled = false;
                rotationAndPoint.LookAt(vecResponse.lookAt);
            }

            if (response.pointTo != null)
            {
                Debug.Log("Pointing to: " + vecResponse.pointTo);
                navMeshAgent.enabled = false;
                rotationAndPoint.PointTo(vecResponse.pointTo);
                Logger.AddLog(ActionType.Point, vecResponse.pointTo.ToString());
            }

            if (response.moveTo != null)
            {
                Debug.Log("Move to: " + vecResponse.moveTo);
                navMeshAgent.enabled = true;
                aiPathTarget.SetTarget(vecResponse.moveTo);
                Logger.AddLog(ActionType.Walk);
            }

            if (response.moveTo == null && response.pointTo == null && response.message != null)
            {
                Logger.AddLog(ActionType.IATalk, response.message);
            }


            // INFO: spécial, à remove si réutilisé
            if (response.level != null)
            {
                GameObject.Find("EndTutoDoor").GetComponent<DoorManager>().Open();
                navMeshAgent.enabled = true;
                TTS_instance.Say("Parfait! allons zi !");
                aiPathTarget.SetTarget(new Vector3(-8.53f, 4f, -2.8f));
                Invoke("WaitForLevel", 11.2f);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing JSON response: " + e.Message);
            return;
        }
    }



    // =============== INFO: spécial, à remove si réutilisé =================
    private void WaitForLevel()
    {
        GameObject.Find("Bob1").SetActive(false);
        GameObject.Find("Burner_Hot").SetActive(false);
        bob2.GetComponent<NavMeshAgent>().Warp(new Vector3(-9.661f, 2f, -2.475f));
        bob2.GetComponent<NavMeshAgent>().enabled = false;
        bob2.GetComponent<AgentActionManager>().ExecuteAction("{\"message\":\"A vous de jouer maintenant. Je reste disponible au besoin.\"}");
        Logger.Initialize();

        Invoke("AutoHelp1", 100f);
        Invoke("AutoHelp2", 200f);
        Invoke("AutoHelp3", 300f);
        Invoke("AutoHelp3", 400f);
    }

    void AutoHelp1()
    {
        bob2.GetComponent<AgentActionManager>().ExecuteAction("{\"message\":\"Est-ce que tu as besoin d'aide ?\"}");
    }

    void AutoHelp2()
    {
        if (GameObject.Find("Machine").GetComponent<MachineManager>().part.Length != 3) return;
        bob2.GetComponent<AgentActionManager>().ExecuteAction("{\"message\":\"Si tu as du mal à trouver les éléments, n'hésite pas à me demander!\"}");
    }

    void AutoHelp3()
    {
        if (GameObject.Find("Machine").GetComponent<MachineManager>().part.Length != 3) return;
        bob2.GetComponent<AgentActionManager>().ExecuteAction("{\"message\":\"As tu besoin de connaitre des informations sur les éléments? Leur couleur par exemple?\"}");
    }
}

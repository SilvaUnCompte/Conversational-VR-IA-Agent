using ReadyPlayerMe.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TTS_GCloud : MonoBehaviour
{
    private enum VoiceType
    {
        fr_FR_Standard_A, fr_FR_Standard_B, fr_FR_Standard_C, fr_FR_Standard_D, // Voix standard, A et C femmes || B et D hommes
        fr_FR_Neural2_A, fr_FR_Neural2_B, // Plus cher mais meilleure qualité
        fr_FR_Studio_A // Voix de studio, meilleure qualité prix ++
    }

    [SerializeField] private VoiceType selectVoice;

    private const string TTS_API_URL = "https://texttospeech.googleapis.com/v1/text:synthesize";
    private string sfVoice;
    AudioSource audioSource;
    AgentActionManager agentActionManager;
    Animator animator;
    VoiceHandler voiceHandler;

    void Start()
    {
        agentActionManager = GetComponent<AgentActionManager>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        voiceHandler = GetComponent<VoiceHandler>();

        if (agentActionManager == null)
        {
            Debug.LogWarning("AgentActionManager component not found on the GameObject.");
        }
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on the GameObject.");
        }
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on the GameObject.");
        }
        if (voiceHandler == null)
        {
            Debug.LogWarning("VoiceHandler component not found on the GameObject.");
        }

        sfVoice = selectVoice.ToString().Replace('_', '-');
        Debug.Log("Selected voice: " + sfVoice);
    }

    public void Say(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Le texte à dire est vide ou null.");
            return;
        }

        string cleanedText = SimpleCleanText(text);
        StartCoroutine(SendTTSRequest(cleanedText, sfVoice));
    }

    private string SimpleCleanText(string text)
    {
        string cleanedText = "";
        for (int i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '+':
                    cleanedText += " plus ";
                    break;
                case '&':
                    cleanedText += " et ";
                    break;
                case '*':
                    cleanedText += " fois ";
                    break;
                case ':':
                    cleanedText += ", ";
                    break;
                case '#':
                    cleanedText += " hashtag ";
                    break;
                case '=':
                    cleanedText += " égal à ";
                    break;
                default:
                    cleanedText += text[i];
                    break;
            }
        }
        return cleanedText.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r"); ;
    }

    private IEnumerator SendTTSRequest(string inputText, string sfVoice, bool useSSML = false)
    {
        string inputType = useSSML ? "ssml" : "text";

        // Construire le JSON avec la variable inputText
        string jsonData = $@"
        {{
            ""input"": {{
                ""{inputType}"": ""{inputText}""
            }},
            ""voice"": {{
                ""languageCode"": ""fr-FR"",
                ""name"": ""{sfVoice}"",
            }},
            ""audioConfig"": {{
                ""audioEncoding"": ""MP3""
            }}
        }}";

        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Créer la requête UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(TTS_API_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + agentActionManager.GoogleToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            // Parse JSON pour récupérer "audioContent"
            var responseObj = JsonUtility.FromJson<TTSResponse>(jsonResponse);

            if (string.IsNullOrEmpty(responseObj.audioContent))
            {
                Debug.LogError("Pas de audioContent dans la réponse !");
                yield break;
            }

            // Décoder base64 en byte[]
            byte[] audioBytes = Convert.FromBase64String(responseObj.audioContent);

            string tempPath = System.IO.Path.Combine(Application.temporaryCachePath, "temp_tts.mp3");
            System.IO.File.WriteAllBytes(tempPath, audioBytes);

            using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + tempPath, AudioType.MPEG))
            {
                yield return audioRequest.SendWebRequest();

                if (audioRequest.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                    animator.SetTrigger("talk");

                    voiceHandler.AudioClip = clip;
                    audioSource.resource = clip;
                    voiceHandler.PlayCurrentAudioClip();

                    StartCoroutine(WaitForTalkingFinish());
                }
                else
                {
                    Debug.LogError("Erreur chargement clip audio : " + audioRequest.error);
                }
            }
        }
        else
        {
            Debug.LogError("Erreur : " + request.error);
            Debug.LogError("Détail : " + request.downloadHandler.text);
        }
    }

    private IEnumerator WaitForTalkingFinish()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }

    [Serializable]
    public class TTSResponse
    {
        public string audioContent;
    }
}

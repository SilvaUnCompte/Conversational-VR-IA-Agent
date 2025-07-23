using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;

public class STT_GCloud : MonoBehaviour
{
    private const string STT_API_URL = "https://speech.googleapis.com/v1/speech:recognize";
    private const int SAMPLE_RATE = 16000; // Sample rate for the audio recording
    private AudioClip recordedClip;

    AgentActionManager agentActionManager;
    AudioSource audioSource;
    TTS_GCloud TTS_instance;
    LLM_Groq llm_Groq;
    RotationAndPoint rotationAndPoint;

    // For stats in logs
    private float recordStartTime;

    private void Start()
    {
        agentActionManager = GetComponent<AgentActionManager>();
        audioSource = GetComponent<AudioSource>();
        TTS_instance = GetComponent<TTS_GCloud>();
        llm_Groq = GetComponent<LLM_Groq>();
        rotationAndPoint = GetComponent<RotationAndPoint>();

        if (agentActionManager == null)
        {
            Debug.LogWarning("AgentActionManager component not found on the GameObject.");
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on this GameObject.");
        }
        if (TTS_instance == null)
        {
            Debug.LogWarning("TTS_GCloud component not found on the GameObject.");
        }
        if (llm_Groq == null)
        {
            Debug.LogWarning("LLM_Groq component not found on the GameObject.");
        }
        if (rotationAndPoint == null)
        {
            Debug.LogWarning("RotationAndPoint component not found on the GameObject.");
        }
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found. Please connect a microphone.");
        }
        else
        {
            Debug.Log("Microphone devices found: " + string.Join(", ", Microphone.devices));
        }
    }

    public void StartSpeaking(SelectEnterEventArgs args)
    {
        if (Microphone.IsRecording(null))
        {
            Debug.LogWarning("Already recording audio. Please stop the current recording first.");
            return;
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on this GameObject.");
            return;
        }
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found. Please connect a microphone.");
            return;
        }

        Debug.Log("Recording...");
        rotationAndPoint.LookAtMe(args.interactorObject.transform.position);
        recordedClip = Microphone.Start(null, false, 30, SAMPLE_RATE);
        recordStartTime = Time.time;
    }

    public void StopSpeaking(SelectExitEventArgs args)
    {
        if (Microphone.IsRecording(null))
        {
            int position = Microphone.GetPosition(null);
            Microphone.End(null);
            Debug.Log("Recording ended.");

            if (position <= 0)
            {
                Debug.LogWarning("No audio data recorded.");
                return;
            }

            AudioClip trimmedClip = WavUtils.TrimClip(recordedClip, position);
            byte[] wavData = WavUtils.ConvertClipToWav(trimmedClip);

            string timeElapsed = (Time.time - recordStartTime).ToString("F2");
            Logger.AddLog(ActionType.UserTalk, timeElapsed);
            StartCoroutine(SendSTTRequest(wavData));
        }
    }

    private IEnumerator SendSTTRequest(byte[] wavData)
    {
        string base64Audio = Convert.ToBase64String(wavData);
        SpeechRequest requestPayload = new SpeechRequest
        {
            config = new RecognitionConfig(),
            audio = new RecognitionAudio { content = base64Audio }
        };

        string jsonPayload = JsonConvert.SerializeObject(requestPayload, Formatting.None);

        UnityWebRequest request = new UnityWebRequest(STT_API_URL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + agentActionManager.GoogleToken);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        STTResponse response = JsonConvert.DeserializeObject<STTResponse>(responseText);

        if (response.results != null && response.results.Length > 0 &&
            response.results[0].alternatives != null && response.results[0].alternatives.Length > 0)
        {
            string transcript = response.results[0].alternatives[0].transcript;
            Debug.Log("Transcript: " + transcript);

            if (llm_Groq)
            {
                llm_Groq.Ask(transcript);
            }
            else
            {
                Debug.LogWarning("LLM_Groq instance is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("No transcription found in the STT response: " + responseText);
            var errorObj = JsonConvert.DeserializeObject<ErrorSTTResponse>(responseText);
            if (errorObj != null && errorObj.error != null && !string.IsNullOrEmpty(errorObj.error.message) && errorObj.error.code == "401")
            {
                // Mettre l'objet en rouge pour signaler l'erreur d'authentification
                var renderer = GetComponent<Renderer>();
                if (renderer != null) renderer.material.color = Color.red;
            }
            else
            {
                if (TTS_instance) TTS_instance.Say("Verifiez que votre micro marche correctement. Je n'ai rien entendu.");
            }
        }
    }

    // Serialization classes for the request body
    [Serializable]
    public class ErrorSTTResponse
    {
        public ErrorSTT error;
    }
    [Serializable]
    public class ErrorSTT
    {
        public string code;
        public string message;
    }
    [Serializable]
    public class RecognitionConfig
    {
        public string encoding = "LINEAR16";
        public int sampleRateHertz = SAMPLE_RATE;
        public string languageCode = "fr-FR";
        public bool enableAutomaticPunctuation = false;
        public int maxAlternatives = 1;
        public bool profanityFilter = false;
    }
    [Serializable]
    public class RecognitionAudio
    {
        public string content;
    }
    [Serializable]
    public class SpeechRequest
    {
        public RecognitionConfig config;
        public RecognitionAudio audio;
    }

    [Serializable]
    public class STTResponse
    {
        public Result[] results;
    }
    [Serializable]
    public class Result
    {
        public Alternative[] alternatives;
    }
    [Serializable]
    public class Alternative
    {
        public string transcript;
        public float confidence;
    }
}

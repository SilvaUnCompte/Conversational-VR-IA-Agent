using System;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using UnityEngine;

public class GoogleAuthToken : MonoBehaviour
{
    public static GoogleAuthToken Instance { get; private set; }

    public string AccessToken { get; private set; }
    public bool IsTokenReady { get; private set; } = false;

    public event Action<string> OnTokenReceived;

    private async void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �vite les doublons
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre les sc�nes

        await GetAccessTokenAsync(); // D�marre une fois
    }

    private async Task GetAccessTokenAsync()
    {
        string jsonPath = Application.dataPath + "/Resources/unityconversasionalia-f05b8c7aa857.json";

        try
        {
            GoogleCredential credential;
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }
            AccessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            IsTokenReady = true;
            Debug.Log("Token: " + AccessToken);
            OnTokenReceived?.Invoke(AccessToken);
        }
        catch (Exception e)
        {
            Debug.LogError("Error retrieving token: " + e.Message);
        }
    }
}
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class JsonLLMActionResponse
{
    public string[] moveTo;
    public string[] lookAt;
    public string[] pointTo;
    public string message;
    public string level;
}

[System.Serializable]
public class JsonVecLLMActionResponse
{
    public Vector3 moveTo;
    public Vector3 lookAt;
    public Vector3 pointTo;
    public string message;
}

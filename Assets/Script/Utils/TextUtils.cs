using UnityEngine;

public class TextUtils
{
    public static string ToSSML(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return "<speak></speak>";

        // Nettoyage simple
        string text = rawText.Trim();

        // Remplacer les caractères spéciaux XML
        text = System.Security.SecurityElement.Escape(text);

        // Ajouter une pause après chaque phrase
        text = text.Replace(".", ".<break time=\"500ms\"/>")
                   .Replace("!", "!<break time=\"500ms\"/>")
                   .Replace("?", "?<break time=\"600ms\"/>")
                   .Replace(":", ":<break time=\"300ms\"/>")
                   .Replace(",", ",<break time=\"200ms\"/>");

        // Encapsuler dans <speak>
        return $"<speak>{text}</speak>";
    }

    public static float stringToFloat(string input)
    {
        input = input.Replace('.', ',');
        if (float.TryParse(input, out float result))
        {
            return result;
        }
        else
        {
            Debug.LogError($"Failed to parse '{input}' as float.");
            return 0f; // Valeur par défaut en cas d'échec
        }
    }

    public static Vector3 stringToVector3(string[] input)
    {
        if (input == null || input.Length < 3)
        {
            return Vector3.zero; // Valeur par défaut en cas d'échec
        }
        float x = stringToFloat(input[0]);
        float y = stringToFloat(input[1]);
        float z = stringToFloat(input[2]);
        return new Vector3(x, y, z);
    }

    public static Vector2 stringToVector2(string[] input)
    {
        if (input == null || input.Length < 2)
        {
            return Vector2.zero; // Valeur par défaut en cas d'échec
        }
        float x = stringToFloat(input[0]);
        float y = stringToFloat(input[1]);
        return new Vector2(x, y);
    }

    public static JsonVecLLMActionResponse convertLLMResponseToVec(JsonLLMActionResponse response)
    {
        JsonVecLLMActionResponse vecResponse = new JsonVecLLMActionResponse
        {
            moveTo = stringToVector3(response.moveTo),
            lookAt = stringToVector3(response.lookAt),
            pointTo = stringToVector3(response.pointTo),
            message = response.message
        };
        return vecResponse;
    }
}
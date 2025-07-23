using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IAVisionManager : MonoBehaviour
{
    [SerializeField] string AIVisionObjectTag = "AIVisionObject";
    private List<RegisteredGameObjects> registeredGameObjects = new List<RegisteredGameObjects>();

    public void UpdateVisionObjectPos()
    {
        registeredGameObjects.Clear();
        List<GameObject> visionGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag(AIVisionObjectTag));

        foreach (GameObject obj in visionGameObjects)
        {
            registeredGameObjects.Add(new RegisteredGameObjects
            {
                registeredPos = obj.transform.position,
                gameObject = obj
            });
        }
    }

    public string GetFormatObjectList()
    {
        string formattedList = "Objets dans la salle et leur position:\n";
        foreach (RegisteredGameObjects obj in registeredGameObjects)
        {
            formattedList += $"-{obj.gameObject.name}: [\"{Math.Round(obj.registeredPos.x, 2)}\",\"{Math.Round(obj.registeredPos.y, 2)}\",\"{Math.Round(obj.registeredPos.z, 2)}\"]\n";
        }
        return formattedList;
    }

    public List<RegisteredGameObjects> GetVisionObjects()
    {
        return registeredGameObjects;
    }
}


[Serializable]
public class RegisteredGameObjects
{
    public Vector3 registeredPos;
    public GameObject gameObject;
}
using System.Collections.Generic;
using UnityEngine;
using WeArt.Components;

public class SetTouchableObj : MonoBehaviour
{
    void Start()
    {
        // Get reference to the WeArtTouchableObject
        var reference = GetComponent<WeArtTouchableObject>();


        // Get la list des element du layer Touchable
        GameObject[] touchableObjects = FindGameObjectsWithLayer(3);
        if (touchableObjects.Length == 0)
        {
            Debug.LogWarning("No objects with the 'Touchable' tag found in the scene.");
            return;
        }


        foreach (GameObject obj in touchableObjects)
        {
            obj.AddComponent<WeArtTouchableObject>();
            WeArtTouchableObject touchableObj = obj.GetComponent<WeArtTouchableObject>();
            Collider collider = obj.GetComponent<Collider>();

            touchableObj.enabled = true;
            touchableObj.Texture = reference.Texture;
            touchableObj.Temperature = reference.Temperature;
            touchableObj.Stiffness = reference.Stiffness;
            touchableObj.Graspable = reference.Graspable;
            touchableObj.AddColliderToTouchableColliders(collider);
        }
    }

    public static GameObject[] FindGameObjectsWithLayer(int layer)
    {
        GameObject[] goArray = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> goList = new List<GameObject>();

        foreach (GameObject go in goArray)
        {
            if (go.layer == layer)
            {
                goList.Add(go);
            }
        }

        return goList.Count > 0 ? goList.ToArray() : null;
    }
}

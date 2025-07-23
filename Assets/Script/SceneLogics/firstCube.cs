using UnityEngine;

public class firstCube : MonoBehaviour
{
    GameObject bob;
    bool firstTime = true;

    private void Start()
    {
        bob = GameObject.Find("Bob");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name != "Cube5") return;
        if (!firstTime) return;
        bob.GetComponent<AgentActionManager>().ExecuteAction("{\"message\":\"Ce cube me semble trop gros pour être celui que tu cherches. Tu ne crois pas?\"}");
        firstTime = false;
    }
}

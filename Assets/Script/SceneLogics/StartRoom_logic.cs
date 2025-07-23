using UnityEngine;

public class StartRoom_logic : MonoBehaviour
{
    public void syncValided()
    {
        Invoke("openDoor", 2f);
    }
    private void openDoor()
    {
        GameObject startDoor = GameObject.Find("StartDoor");
        startDoor.GetComponent<DoorManager>().Open();
    }
}

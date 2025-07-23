using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] Vector3 openOffset = new Vector3(5f, 0, 0);
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] bool closable = true;

    Vector3 closePos;
    Vector3 openPos;
    private bool isOpening = false;

    void Start()
    {
        closePos = transform.position;
        openPos = closePos + openOffset;
    }

    public void Open()
    {
        isOpening = true;
    }

    public void Close()
    {
        if (!closable) return; // If the door is not closable, do nothing
        isOpening = false;
    }

    private void Update()
    {
        if (isOpening)
        {
            if (Vector3.Distance(transform.position, openPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, openPos, moveSpeed * Time.deltaTime);
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, closePos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, closePos, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "User") return;
        Close();
    }
}

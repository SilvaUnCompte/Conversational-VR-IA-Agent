using UnityEngine;

public class ResetObjPosition : MonoBehaviour
{
    InitialStat initialStat;

    void Start()
    {
        initialStat = new InitialStat();
        initialStat.position = transform.position;
        initialStat.rotation = transform.rotation;
        initialStat.localScale = transform.localScale;
    }

    void Update()
    {
        if (transform.position.y < -5f)
        {
            ResetPosition();
        }
    }

    void ResetPosition()
    {
        transform.position = initialStat.position;
        transform.rotation = initialStat.rotation;
        transform.localScale = initialStat.localScale;

        // Reset momentum
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    [System.Serializable]
    public class InitialStat
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
    }
}

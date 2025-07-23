using UnityEngine;

public class InitXP : MonoBehaviour
{
    [SerializeField] string TestGroup = "groupe_A"; // Nom du groupe pour les logs
    [SerializeField] public bool Haptic = true;

    void Start()
    {
        Logger.Group = TestGroup; // Initialiser le groupe de logs
    }
}

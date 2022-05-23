using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    // ===== Public Functions =====================================================================

    public void Toggle () => gameObject.SetActive(!gameObject.activeInHierarchy);
}
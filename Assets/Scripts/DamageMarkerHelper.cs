using System;
using TMPro;
using UnityEngine;

public class DamageMarkerHelper : MonoBehaviour
{
    [SerializeField] private GameObject damageMarkerPrefab;
    [SerializeField, Min(0f)] private float markerLifetime = 0.75f;

    public static DamageMarkerHelper Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SpawnDamageMarker(Vector2 position, int damage)
    {
        if (damageMarkerPrefab == null)
        {
            Debug.LogWarning("Damage marker prefab is not assigned.");
            return;
        }

        GameObject marker = Instantiate(damageMarkerPrefab, position, Quaternion.identity);
        TMP_Text text = marker.GetComponentInChildren<TMP_Text>();

        if (text != null)
            text.text = damage.ToString();
        else
            Debug.LogWarning("Damage marker prefab has no TMP text component.");

        Destroy(marker, markerLifetime);
    }
}

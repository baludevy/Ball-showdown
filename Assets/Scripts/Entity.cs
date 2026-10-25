using UnityEngine;

public class Entity : MonoBehaviour
{
    public ushort entityId {get; private set; }

    public void Initialize(ushort id)
    {
        this.entityId = id;
    }

    public void ApplySnapshot(EntitySnapshot from, EntitySnapshot to, float t)
    {
        
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum BallType
{
    Cell,
    Burst,
    Charge,
    Fibonacci,
    Glass,
    Harpoon,
    Ice,
    PoisonSpike,
    Reforge,
    Kawaity,
    Shuriken,
    Speedy,
    Spider,
    Spike,
    Verity,
}

[Serializable]
public class BallEntry
{
    public string name;
    [TextArea(10, 20)]
    public string descrption;
    public BallType type;
    public Sprite sprite;
    public GameObject prefab;
}

public class BallCatalog : MonoBehaviour
{
    public List<BallEntry> balls;
    
    public static BallCatalog Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public BallEntry GetBall(BallType type) => balls.Find(x => x.type == type);
}
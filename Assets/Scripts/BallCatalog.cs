using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BallEntry
{
    public string name;
    [TextArea(10, 20)]
    public string descrption;
    public Sprite sprite;
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
}
using System.Collections.Generic;
using UnityEngine;

public class BallArena : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 16;
    [SerializeField] private float baseArenaSize = 12f;
    [SerializeField] private float minimumPlayerSpacing = 4f;
    [SerializeField] private float spacePerPlayer = 1.5f;
    [SerializeField] private float wallThickness = 1f;
    [SerializeField] private float spawnPadding = 1f;
    [SerializeField] private float ballRadius = 0.5f;
    [SerializeField] private float cameraPadding = 2f;
    [SerializeField] private Color wallColor = Color.white;
    [SerializeField] private Camera arenaCamera;

    private float arenaSize;
    private Sprite wallSprite;
    private readonly List<GameObject> walls = new List<GameObject>();

    public float ArenaSize => arenaSize;

    public static BallArena Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetupArena(List<Ball> balls)
    {
        if (balls == null)
        {
            Debug.LogError("Player list is null.");
            return;
        }

        int playerCount = balls.Count;

        if (playerCount == 0)
        {
            return;
        }

        if (playerCount > Mathf.Min(maxPlayers, 16))
        {
            Debug.LogError("Arena supports a maximum of 16 players.");
            return;
        }

        CalculateArenaSize(playerCount);
        CreateWalls();
        PlacePlayers(balls);
        UpdateCamera();
    }

    private void CalculateArenaSize(int playerCount)
    {
        float growth = Mathf.Max(0, playerCount - 2) * spacePerPlayer;
        float normalSize = baseArenaSize + growth;

        float clearance = GetSpawnClearance();
        float requiredPerimeter = playerCount * minimumPlayerSpacing;
        float requiredInnerSize= requiredPerimeter / 4f;
        float requiredArenaSize = requiredInnerSize + clearance * 2f;

        arenaSize = Mathf.Max(normalSize, requiredArenaSize);
        arenaSize = Mathf.Max(arenaSize, clearance * 2f + 1f);
    }

    private float GetSpawnClearance()
    {
        return Mathf.Max(0f, spawnPadding) + Mathf.Max(0f, ballRadius);
    }

    private void CreateWalls()
    {
        ClearWalls();

        if (wallSprite == null)
        {
            CreateWallSprite();
        }

        float halfSize = arenaSize / 2f;
        float thickness = Mathf.Max(0.01f, wallThickness);
        float wallOffset = halfSize + thickness / 2f;
        float fullWallLength = arenaSize + thickness * 2f;

        CreateWall(
            "Top Wall",
            new Vector2(0f, wallOffset),
            new Vector2(fullWallLength, thickness)
        );

        CreateWall(
            "Bottom Wall",
            new Vector2(0f, -wallOffset),
            new Vector2(fullWallLength, thickness)
        );

        CreateWall(
            "Left Wall",
            new Vector2(-wallOffset, 0f),
            new Vector2(thickness, arenaSize)
        );

        CreateWall(
            "Right Wall",
            new Vector2(wallOffset, 0f),
            new Vector2(thickness, arenaSize)
        );
    }

    private void CreateWallSprite()
    {
        Texture2D texture = Texture2D.whiteTexture;

        wallSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            texture.width
        );
    }

    public void ClearWalls()
    {
        foreach (GameObject wall in walls)
        {
            if (wall == null)
            {
                continue;
            }

            wall.SetActive(false);

            if (Application.isPlaying)
            {
                Destroy(wall);
            }
            else
            {
                DestroyImmediate(wall);
            }
        }

        walls.Clear();
    }

    private void CreateWall(string wallName, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(wallName);

        wall.transform.SetParent(transform, false);
        wall.transform.localPosition = new Vector3(position.x, position.y, 0f);
        wall.transform.localRotation = Quaternion.identity;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = wallSprite;
        spriteRenderer.color = wallColor;
        spriteRenderer.sortingOrder = 10;

        BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
        wallCollider.size = Vector2.one;
        wallCollider.isTrigger = false;
        
        wall.tag = "Wall";

        walls.Add(wall);
    }

    private void PlacePlayers(List<Ball> balls)
    {
        int playerCount = balls.Count;

        float clearance = GetSpawnClearance();
        float halfSize = arenaSize / 2f;
        float spawnRadius = halfSize - clearance;

        if (playerCount == 1)
        {
            PlacePlayer(balls[0], Vector2.zero);
            return;
        }

        float perimeter = spawnRadius * 8f;
        float distanceBetweenPlayers = perimeter / playerCount;
        float startingDistance = GetStartingDistance(playerCount, perimeter);

        for (int i = 0; i < playerCount; i++)
        {
            float distance = startingDistance + i * distanceBetweenPlayers;
            Vector2 position = GetPerimeterPosition(distance, spawnRadius);

            PlacePlayer(balls[i], position);
        }
    }

    private float GetStartingDistance(int playerCount, float perimeter)
    {
        if (playerCount == 2)
        {
            return perimeter / 8f;
        }

        if (playerCount % 4 == 0)
        {
            return 0f;
        }

        return perimeter / (playerCount * 2f);
    }

    private Vector2 GetPerimeterPosition(float distance, float radius)
    {
        float sideLength = radius * 2f;
        float perimeter = sideLength * 4f;

        distance = Mathf.Repeat(distance, perimeter);

        if (distance < sideLength)
        {
            return new Vector2(
                -radius + distance,
                radius
            );
        }

        distance -= sideLength;

        if (distance < sideLength)
        {
            return new Vector2(
                radius,
                radius - distance
            );
        }

        distance -= sideLength;

        if (distance < sideLength)
        {
            return new Vector2(
                radius - distance,
                -radius
            );
        }

        distance -= sideLength;

        return new Vector2(
            -radius,
            -radius + distance
        );
    }

    private void PlacePlayer(Ball ball, Vector2 position)
    {
        if (ball == null)
            return;

        Vector3 spawnCenter = transform.TransformPoint(
            new Vector3(position.x, position.y, 0f)
        );
        
        Vector3 worldOffset = ball.transform.TransformVector(
            new Vector3(0, 0.5f, 0f)
        );
        
        Vector3 worldPosition = spawnCenter - worldOffset;
        worldPosition.z = ball.transform.position.z;

        Rigidbody2D body = ball.GetComponent<Rigidbody2D>();

        if (body != null)
        {
            body.position = new Vector2(worldPosition.x, worldPosition.y);
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        ball.transform.position = worldPosition;
    }

    private void UpdateCamera()
    {
        if (arenaCamera == null)
        {
            arenaCamera = Camera.main;
        }

        if (arenaCamera == null)
        {
            return;
        }

        arenaCamera.orthographic = true;

        Vector3 cameraPosition = arenaCamera.transform.position;
        cameraPosition.x = transform.position.x;
        cameraPosition.y = transform.position.y;
        arenaCamera.transform.position = cameraPosition;

        float halfSize = arenaSize / 2f;
        float outerSize = halfSize + Mathf.Max(0f, wallThickness);
        float visibleSize = outerSize + Mathf.Max(0f, cameraPadding);

        float verticalSize = visibleSize;
        float horizontalSize = visibleSize / arenaCamera.aspect;

        arenaCamera.orthographicSize = Mathf.Max(
            verticalSize,
            horizontalSize
        );
    }

    public void SetWallColor(Color color)
    {
        wallColor = color;

        foreach (GameObject wall in walls)
        {
            if (wall == null)
            {
                continue;
            }
            
            SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();

            if (renderer != null)
            {
                renderer.color = wallColor;
            }
        }
    }

    public float GetArenaSize()
    {
        return arenaSize;
    }

    public Vector2 GetArenaCenter()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public Vector2 GetArenaBounds()
    {
        return new Vector2(arenaSize, arenaSize);
    }
    private void OnDestroy()
    {
        ClearWalls();
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSelectionManager : MonoBehaviour
{
    public List<BallEntry> ownSelectableBalls = new List<BallEntry>();

    public Dictionary<ushort, List<BallType>> selectableBallsByPlayer = new Dictionary<ushort, List<BallType>>();

    public static BallSelectionManager Instance;

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

    public List<BallType> GetSelectableBalls()
    {
        List<BallEntry> balls = BallCatalog.Instance.balls;

        List<BallType> selectableBalls = balls
            .OrderBy(_ => UnityEngine.Random.value)
            .Select(ball => ball.type)
            .Distinct()
            .Take(3)
            .ToList();

        return selectableBalls;
    }

    public void SendSelectableBalls()
    {
        foreach (Player player in GameManager.Instance.players)
        {
            if (player.id == 0)
            {
                List<BallType> ownBallTypes = GetSelectableBalls();
                ownSelectableBalls.Clear();
                
                foreach (BallType ballType in ownBallTypes)
                {
                    BallEntry ball = BallCatalog.Instance.GetBall(ballType);
                    ownSelectableBalls.Add(ball);
                }

                UIManager.Instance.ballSelectionUI.DisplayBallCards();

                continue;
            }

            List<BallType> ballTypes = GetSelectableBalls();
            selectableBallsByPlayer[player.id] = ballTypes;
        }
    }
}

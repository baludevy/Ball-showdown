using System.Collections.Generic;
using UnityEngine;

public class BallSelectionUIManager : MonoBehaviour
{
    public Transform ballCardsParent;
    public GameObject ballCardPrefab;
    
    public void DisplayBallCards()
    {
        List<BallEntry> balls = BallSelectionManager.Instance.ownSelectableBalls;
        
        for (int i = 0; i < ballCardsParent.childCount; i++)
        {
            Destroy(ballCardsParent.GetChild(i).gameObject);
        }

        int ballIndex = 0;
        
        foreach (BallEntry ball in balls)
        {
            BallCard ballCard = Instantiate(ballCardPrefab, ballCardsParent).GetComponent<BallCard>();
            
            ballCard.SetBall(ballIndex, ball);

            ballIndex++;
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallCard : MonoBehaviour
{
    public int index;
    public Image image;
    
    public TMP_Text ballName;
    public TMP_Text description;

    public void SetBall(int index, BallEntry ball)
    {
        this.index = index;
        
        ballName.text = ball.name;
        description.text = ball.descrption;
        image.sprite = ball.sprite;
    }

    public void SelectBall()
    {
        
    }
}
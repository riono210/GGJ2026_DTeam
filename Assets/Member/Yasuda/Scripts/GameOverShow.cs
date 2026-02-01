using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverShow : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image gameOverImage;
    [SerializeField] private float showFadeDuration = 1f;
    [SerializeField] private float showDuration = 1f;

    private Sequence gameOverShowSequence;
    
    private void Start()
    {
        gameOverImage.color = new Color(1, 1, 1, 0);
    }

    public void ShowGameOver()
    {
        Debug.Log("Show Game Over");
        gameOverShowSequence = DOTween.Sequence();

        gameOverShowSequence.Append(gameOverImage.DOFade(1f, showFadeDuration));
        gameOverShowSequence.Join(rectTransform.DOAnchorPosY(0, showDuration).SetEase(Ease.OutBounce));
    }

    private void OnDestroy()
    {
        gameOverShowSequence?.Kill();
    }
}

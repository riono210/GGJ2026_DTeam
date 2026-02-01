using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverShow : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image gameOverImage;
    [SerializeField] private float showFadeDuration = 1f;
    [SerializeField] private float showDuration = 1f;
    [SerializeField] private Image fadeImage;

    private Sequence gameOverShowSequence;
    private PlayerActions playerActions;
    private bool isGameOver = false;
    
    private void Start()
    {
        isGameOver = false;
        fadeImage.color = new Color(0, 0, 0, 0);
        playerActions = new PlayerActions();
        playerActions.Enable();
        playerActions.gameplay.attack.performed += RetryGame;
        gameOverImage.color = new Color(1, 1, 1, 0);
    }
    

    public void ShowGameOver()
    {
        Debug.Log("Show Game Over");
        gameOverShowSequence = DOTween.Sequence();

        gameOverShowSequence.Append(gameOverImage.DOFade(1f, showFadeDuration));
        gameOverShowSequence.Join(rectTransform.DOAnchorPosY(0, showDuration).SetEase(Ease.OutBounce));
        
        isGameOver = true;
    }

    private async void RetryGame(InputAction.CallbackContext ctx)
    {
        if (!isGameOver)
        {
            return;
        }
        
        await fadeImage.DOFade(1f, 1f).ToUniTask();
        
        SceneManager.LoadScene("MainStage");
    }

    private void OnDestroy()
    {
        gameOverShowSequence?.Kill();
        isGameOver = false;
        playerActions.gameplay.attack.performed -= RetryGame;
    }
}

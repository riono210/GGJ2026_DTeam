using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerGoal : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    
    private Subject<MoveObjectHitEventType> goalSubject = new Subject<MoveObjectHitEventType>();
    public Observable<MoveObjectHitEventType> GoalObservable => goalSubject;
    
    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<GoalMoveObject>() != null)
        {
            Debug.Log("Goal Reached!");
            goalSubject.OnNext(MoveObjectHitEventType.Clear);
            
            await fadeImage.DOFade(1f, 1f).ToUniTask();
            SceneManager.LoadScene("ClearScene");
        }
    }
}

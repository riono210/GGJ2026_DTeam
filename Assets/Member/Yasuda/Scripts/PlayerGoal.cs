using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGoal : MonoBehaviour
{
    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<GoalMoveObject>() != null)
        {
            Debug.Log("Goal Reached!");
            // TODO:fade フェードしたい
            await UniTask.Delay(TimeSpan.FromSeconds(1))
                .ContinueWith(() => SceneManager.LoadScene("EndScene"));
        }
    }
}

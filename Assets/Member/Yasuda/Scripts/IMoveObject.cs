using R3;
using UnityEngine;

public class IMoveObject : MonoBehaviour
{
    protected float currentSpeed;
    private CompositeDisposable disposables = new CompositeDisposable();
    private bool isActiveMove = false;

    public CompositeDisposable Disposables => disposables;

    private void Update()
    {
        if (!isActiveMove)
        {
            return;
        }

        MoveObject();
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
    
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void StartMove(float initialSpeed)
    {
        currentSpeed = initialSpeed;
        isActiveMove = true;
    }

    public void StopMove()
    {
        isActiveMove = false;
    }

    protected virtual void MoveObject()
    {
        // 共通の移動: Z-方向へ流れる
        transform.position += Vector3.back * (currentSpeed * Time.deltaTime);
    }
}

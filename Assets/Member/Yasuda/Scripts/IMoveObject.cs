using R3;
using UnityEngine;

public class IMoveObject : MonoBehaviour
{
    protected float currentSpeed;
    private CompositeDisposable disposables = new CompositeDisposable();

    public CompositeDisposable Disposables => disposables;

    private void Start()
    {
        disposables =  new CompositeDisposable();
    }

    private void Update()
    {
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

    protected virtual void MoveObject()
    {
        // 共通の移動: Z-方向へ流れる
        transform.position += Vector3.back * (currentSpeed * Time.deltaTime);
    }
}

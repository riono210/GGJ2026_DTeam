using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MoveObjectDestroyer : MonoBehaviour
{
    private void Reset()
    {
        SetupTrigger();
    }

    private void Awake()
    {
        SetupTrigger();
    }

    private void SetupTrigger()
    {
        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var moveObject = other.GetComponentInParent<IMoveObject>();
        if (moveObject == null)
        {
            return;
        }

        Destroy(moveObject.gameObject);
    }
}

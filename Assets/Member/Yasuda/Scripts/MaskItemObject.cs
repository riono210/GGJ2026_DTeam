using R3;
using UnityEngine;

public class MaskItemObject : IMoveObject
{
    public enum ItemType
    {
        Barrier,
        ForcedExcellent,
        SlowImmunity,
    }
    
    [SerializeField] private ItemType itemType;
    public ItemType GetItemType() => itemType;
    
    private ReactiveProperty<float> speed = new ReactiveProperty<float>();
    
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //     
    // }
}

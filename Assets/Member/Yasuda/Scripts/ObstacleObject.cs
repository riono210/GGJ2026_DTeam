using System;
using UnityEngine;

public enum ObstacleType
{
    None = 0,
    TypeA = 1,
    TypeB = 2,
    TypeC = 3,
}

public class ObstacleObject : IMoveObject
{
    [SerializeField] private ObstacleType obstacleType;
    public ObstacleType ObstacleType => obstacleType;
    
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string Name;
    public int Id;
    public int Cost;
    public Sprite Icon;

    public int Level = 1;

    public int UpgradeHitValuePrice;
    public float HitValue;

    public float FireRate;

    public bool IsSplashDamage;
    public float SplashRadius;

    public bool IsSpeedReduce;
    public int SpeedReduceValue;

    public bool IsAuraBuilding;
    public int AuraId;
    public int AuraValue;

    public GameObject SelectionCircle;
   
}

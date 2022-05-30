using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Normal, Fire, Water, Electric }

[CreateAssetMenu(fileName = "AttackData", menuName = "Data/AttackData")]
public class AttackData : ScriptableObject
{
    public string title = "Basic";
    public Color color = Color.white;
    public DamageType damageType = DamageType.Normal;
    public bool auto;
    public float cooldown = 1f;
    public int damage = 10;
    public Projectile projectile;
}

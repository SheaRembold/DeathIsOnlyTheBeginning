using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "Data/CreatureData")]
public class CreatureData : CharacterData
{
    public CreatureController Prefab;
    public AttackData BasicAttack;
    public AttackData SpecialAttack;
}

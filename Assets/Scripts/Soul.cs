using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour, ISelectable
{
    [SerializeField]
    CreatureData creatureData;
    [SerializeField]
    GameObject highlight;
    [SerializeField]
    GameObject select;
    [SerializeField]
    Transform hitTarget;

    public Transform HitTarget { get { return hitTarget; } }

    void Awake()
    {
        highlight.SetActive(false);
        select.SetActive(false);
    }

    public void Charm()
    {
        CreatureController creature = Instantiate(creatureData.Prefab, transform.position, Quaternion.identity);
        creature.Data = creatureData;
        PlayerController.Instance.AddCreature(creatureData, creature);
        Destroy(gameObject);
    }

    public void SetHighlighted(bool selected)
    {
        highlight.SetActive(selected);
    }

    public void SetSelected(bool selected)
    {
        select.SetActive(selected);
    }

}

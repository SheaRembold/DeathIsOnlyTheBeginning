using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureRevive : MonoBehaviour, IInteractable
{
    [SerializeField]
    int reviveHealth = 10;

    CreatureController controller;

    void Awake()
    {
        controller = GetComponentInParent<CreatureController>();
    }

    public void Interact()
    {
        controller.Heal(reviveHealth);
    }
}

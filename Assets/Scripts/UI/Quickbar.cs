using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quickbar : MonoBehaviour
{
    [SerializeField]
    QuickItem[] quickItems;
    [SerializeField]
    Image[] creatureRoots;

    List<CreatureController> creatures = new List<CreatureController>();

    private void OnEnable()
    {
        creatures.Clear();
        for (int i = 0; i < PlayerController.Instance.Creatures.Count; i++)
            AddActions(PlayerController.Instance.Creatures[i]);
        for (int i = creatures.Count; i < creatureRoots.Length; i++)
            creatureRoots[i].gameObject.SetActive(false);
        PlayerController.OnCreatureAdd += AddActions;
    }

    private void OnDisable()
    {
        PlayerController.OnCreatureAdd -= AddActions;
    }

    void AddActions(CreatureController creature)
    {
        creatureRoots[creatures.Count].gameObject.SetActive(true);
        creatureRoots[creatures.Count].color = creature.Data.Color;
        quickItems[creatures.Count * 2].Init(UnityEngine.InputSystem.Key.Digit1 + creatures.Count * 2, new QuickAction() { creature = creature, attack = creature.Data.BasicAttack, lastTime = Time.time - creature.Data.BasicAttack.cooldown });
        quickItems[creatures.Count * 2 + 1].Init(UnityEngine.InputSystem.Key.Digit1 + creatures.Count * 2 + 1, new QuickAction() { creature = creature, attack = creature.Data.SpecialAttack, lastTime = Time.time - creature.Data.SpecialAttack.cooldown });
        creatures.Add(creature);
    }
}

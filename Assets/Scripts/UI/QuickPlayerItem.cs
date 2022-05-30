using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuickPlayerItem : MonoBehaviour
{
    [SerializeField]
    Key key;
    [SerializeField]
    Projectile item;

    private void Update()
    {
        if (Keyboard.current[key].wasPressedThisFrame)
            Use();
    }

    public void Use()
    {
        PlayerController.Instance.ThrowItem(item);
    }
}

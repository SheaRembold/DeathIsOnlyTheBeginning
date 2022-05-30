using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuickAction
{
    public CreatureController creature;
    public AttackData attack;
    public float lastTime;
}

public class QuickItem : MonoBehaviour
{
    [SerializeField]
    Text keyText;
    [SerializeField]
    GameObject autoText;
    [SerializeField]
    Text attackText;
    [SerializeField]
    Image attackImage;
    [SerializeField]
    Image cooldownImage;

    Key key;
    QuickAction action;

    public void Init(Key key, QuickAction action)
    {
        this.key = key;
        keyText.text = (key - Key.Digit1 + 1).ToString();
        this.action = action;
        if (action != null)
        {
            GetComponent<Button>().interactable = true;
            autoText.SetActive(action.attack.auto);
            attackText.text = action.attack.title;
            attackImage.color = action.attack.color;
        }
        else
        {
            GetComponent<Button>().interactable = false;
            autoText.SetActive(false);
            attackText.text = string.Empty;
            attackImage.color = Color.white;
        }
    }

    private void Update()
    {
        if (Keyboard.current[key].wasPressedThisFrame)
            Use();
    }

    private void LateUpdate()
    {
        if (action.attack.cooldown > 0f)
            cooldownImage.fillAmount = 1f - (Time.time - action.lastTime) / action.attack.cooldown;
    }

    public void Use()
    {
        if (action != null && Time.time - action.lastTime >= action.attack.cooldown)
        {
            action.creature.ManualAttack(PlayerController.Instance.TargetEnemy, action);
        }
    }
}

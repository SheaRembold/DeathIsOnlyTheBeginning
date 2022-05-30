using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    Text charmCount;

    void LateUpdate()
    {
        charmCount.text = PlayerController.Instance.CharmCount.ToString();
    }
}

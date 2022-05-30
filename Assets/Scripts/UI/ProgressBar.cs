using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    Image fill;

    public void SetProgress(float progress)
    {
        fill.fillAmount = progress;
    }
}

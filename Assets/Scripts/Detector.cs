using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [System.NonSerialized]
    public List<AiController> detected = new List<AiController>();

    private void FixedUpdate()
    {
        detected.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AiController controller = collision.GetComponentInParent<AiController>();
        if (controller != null && controller.IsAlive)
            detected.Add(controller);
    }
}

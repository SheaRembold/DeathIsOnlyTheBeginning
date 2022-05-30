using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charm : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit)
            return;

        Soul soul = collision.GetComponentInParent<Soul>();
        if (soul != null)
        {
            soul.Charm();
            hit = true;
            Destroy(gameObject);
        }
    }
}

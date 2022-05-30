using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float speed = 20f;

    protected AttackData attack;
    protected Vector2 dir;
    protected bool hit;

    public void Launch(Transform target, AttackData attack)
    {
        this.attack = attack;
        dir = target.position - transform.position;
    }

    protected void FixedUpdate()
    {
        transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit)
            return;

        AiController controller = collision.GetComponentInParent<AiController>();
        if (controller != null && controller.IsAlive)
        {
            controller.TakeDamage(attack.damage, attack.damageType);
            hit = true;
            Destroy(gameObject);
        }
    }
}

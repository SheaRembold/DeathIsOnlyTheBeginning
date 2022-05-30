using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [SerializeField]
    protected int maxHealth = 100;
    [SerializeField]
    protected int startHealth = 100;
    [SerializeField]
    protected ProgressBar healthBar;
    [SerializeField]
    protected float attackDelay = 1f;
    [SerializeField]
    protected float attackDistance = 3f;
    [SerializeField]
    protected Transform hitTarget;
    [SerializeField]
    protected DamageType[] weaknesses;
    [SerializeField]
    protected DamageType[] resistances;

    public bool IsAlive { get { return health > 0; } }
    public Transform HitTarget { get { return hitTarget; } }

    protected Detector detector;
    protected int health;

    protected virtual void Awake()
    {
        detector = GetComponentInChildren<Detector>();

        health = Mathf.Clamp(startHealth, 0, maxHealth);
        healthBar.SetProgress((float)health / maxHealth);
        if (health <= 0)
        {
            OnDeath();
        }
        else
        {
            OnRevive();
        }
    }

    protected void Attack(AiController target, AttackData attack)
    {
        Projectile projectile = Instantiate(attack.projectile, HitTarget.position, Quaternion.identity);
        projectile.Launch(target.HitTarget, attack);
    }

    public void TakeDamage(int damage, DamageType damageType)
    {
        if (System.Array.IndexOf(weaknesses, damageType) != -1)
            damage *= 2;
        else if (System.Array.IndexOf(resistances, damageType) != -1)
            damage = Mathf.CeilToInt(damage / 2f);
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        healthBar.SetProgress((float)health / maxHealth);

        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void Heal(int damage)
    {
        health = Mathf.Clamp(health + damage, 0, maxHealth);
        healthBar.SetProgress((float)health / maxHealth);

        if (!enabled)
        {
            OnRevive();
        }
    }

    protected virtual void OnDeath()
    {
        enabled = false;
    }

    protected virtual void OnRevive()
    {
        enabled = true;
    }
}

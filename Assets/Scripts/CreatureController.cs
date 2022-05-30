using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : AiController
{
    public CreatureData Data;

    [SerializeField]
    float minSpeed = 10f;
    [SerializeField]
    float maxSpeed = 10f;
    [SerializeField]
    float maxDistance = 1f;
    [SerializeField]
    GameObject reviveCollider;

    float lastAttack = float.MinValue;
    AiController target;

    QuickAction queuedAction;

    bool HasTarget()
    {
        return target != null && target.IsAlive || detector.detected.Count > 0 || PlayerController.Instance.Engaged.Count > 0;
    }

    void Update()
    {
        if (queuedAction != null)
        {
            if (HasTarget())
            {
                queuedAction.lastTime = Time.time;
            }
            else
            {
                queuedAction.lastTime = Time.time - queuedAction.attack.cooldown;
                queuedAction = null;
            }
        }
        if (HasTarget() && Time.time - lastAttack >= attackDelay)
        {
            if (target == null || !target.IsAlive)
            {
                if (detector.detected.Count > 0)
                {
                    target = detector.detected[0];
                    float closestDist = Vector2.Distance(transform.position, detector.detected[0].transform.position);
                    for (int i = 0; i < detector.detected.Count; i++)
                    {
                        float dist = Vector2.Distance(transform.position, detector.detected[i].transform.position);
                        if (dist < closestDist)
                        {
                            target = detector.detected[i];
                            closestDist = dist;
                        }
                    }
                }
                else
                {
                    target = PlayerController.Instance.Engaged[0];
                    float closestDist = Vector2.Distance(transform.position, PlayerController.Instance.Engaged[0].transform.position);
                    for (int i = 0; i < PlayerController.Instance.Engaged.Count; i++)
                    {
                        float dist = Vector2.Distance(transform.position, PlayerController.Instance.Engaged[i].transform.position);
                        if (dist < closestDist)
                        {
                            target = PlayerController.Instance.Engaged[i];
                            closestDist = dist;
                        }
                    }
                }
            }
            if (queuedAction != null)
            {
                Attack(target, queuedAction.attack);
                queuedAction = null;
            }
            else
            {
                Attack(target, Data.BasicAttack);
            }
            lastAttack = Time.time;
        }
    }

    public void ManualAttack(EnemyController target, QuickAction action)
    {
        if (target != null && target.IsAlive)
        {
            this.target = target;
            PlayerController.Instance.AddEnemy(target);
        }
        if (action.attack != Data.BasicAttack && HasTarget())
        {
            queuedAction = action;
            queuedAction.lastTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (target != null && target.IsAlive)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 dir = targetPos - (Vector2)transform.position;
            if (dir.magnitude > attackDistance)
                transform.position += (Vector3)(dir.normalized * maxSpeed * Time.deltaTime);
        }
        else if (!PlayerController.Instance.InCombat)
        {
            Vector2 targetPos = PlayerController.Instance.GetCreaturePosition(this);
            Vector2 dir = targetPos - (Vector2)transform.position;
            float speed = Mathf.Lerp(minSpeed, maxSpeed, dir.magnitude / maxDistance);
            if (dir.magnitude < speed * Time.deltaTime)
                transform.position = targetPos;
            else
                transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        reviveCollider.SetActive(true);
    }

    protected override void OnRevive()
    {
        base.OnRevive();
        reviveCollider.SetActive(false);
    }
}

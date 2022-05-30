using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AiController, ISelectable
{
    [SerializeField]
    protected AttackData mainAttack;
    [SerializeField]
    protected AttackData[] specialAttacks;
    [SerializeField]
    protected GameObject highlight;
    [SerializeField]
    protected GameObject select;
    [SerializeField]
    protected float speed = 10f;
    [SerializeField]
    protected Soul soul;

    protected List<EnemyController> party = new List<EnemyController>();
    protected List<CreatureController> targets = new List<CreatureController>();
    protected CreatureController lastTarget;
    protected List<int> available = new List<int>();
    protected float[] lastSpecial;
    protected float lastMain = float.MinValue;
    protected float lastAttack = float.MinValue;

    protected override void Awake()
    {
        base.Awake();
        highlight.SetActive(false);
        select.SetActive(false);
        lastSpecial = new float[specialAttacks.Length];
        for (int i = 0; i < lastSpecial.Length; i++)
            lastSpecial[i] = float.MinValue;
    }

    protected void FindTargets()
    {
        party.Clear();
        targets.Clear();
        for (int i = 0; i < detector.detected.Count; i++)
        {
            if (detector.detected[i] is EnemyController && detector.detected[i] != this)
                party.Add(detector.detected[i] as EnemyController);
            else if (detector.detected[i] is CreatureController)
                targets.Add(detector.detected[i] as CreatureController);
        }
        for (int i = 0; i < party.Count; i++)
        {
            if (party[i].lastTarget != null)
                targets.Add(party[i].lastTarget);
        }
    }

    protected virtual void Update()
    {
        FindTargets();
        if (targets.Count > 0 && Time.time - lastAttack >= attackDelay)
        {
            available.Clear();
            for (int i = 0; i < specialAttacks.Length; i++)
            {
                if (Time.time - lastSpecial[i] >= specialAttacks[i].cooldown)
                    available.Add(i);
            }
            if (available.Count > 0)
            {
                lastTarget = targets[Random.Range(0, targets.Count)];
                int attack = available[Random.Range(0, available.Count)];
                Attack(lastTarget, specialAttacks[attack]);
                lastSpecial[attack] = Time.time;
                lastAttack = Time.time;
                PlayerController.Instance.AddEnemy(this);
            }
            else if (mainAttack != null && Time.time - lastMain >= mainAttack.cooldown)
            {
                lastTarget = targets[Random.Range(0, targets.Count)];
                Attack(lastTarget, mainAttack);
                lastMain = Time.time;
                lastAttack = Time.time;
                PlayerController.Instance.AddEnemy(this);
            }
        }
    }

    void FixedUpdate()
    {
        if (lastTarget != null && lastTarget.IsAlive)
        {
            Vector2 targetPos = lastTarget.transform.position;
            Vector2 dir = targetPos - (Vector2)transform.position;
            if (dir.magnitude > attackDistance)
                transform.position += (Vector3)(dir.normalized * speed * Time.deltaTime);
        }
    }

    public void SetHighlighted(bool selected)
    {
        highlight.SetActive(selected);
    }

    public void SetSelected(bool selected)
    {
        select.SetActive(selected);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        PlayerController.Instance.RemoveEnemy(this);
        if (soul != null)
            Instantiate(soul, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

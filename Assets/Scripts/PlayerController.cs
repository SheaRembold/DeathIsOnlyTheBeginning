using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CreaturePermutation
{
    public List<CreatureController> creatures = new List<CreatureController>();
    public float distance;
}

public class ColliderComparer : IComparer<Collider2D>
{
    int IComparer<Collider2D>.Compare(Collider2D a, Collider2D b)
    {
        return a.transform.position.y.CompareTo(b.transform.position.y);
    }
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10f;
    [SerializeField]
    CreatureData[] startCreatures;
    [SerializeField]
    float creatureDistance = 1f;
    [SerializeField]
    float creatureSeparation = 45f;
    [SerializeField]
    LayerMask enemySelectMask;
    [SerializeField]
    float healRate = 10f;
    [SerializeField]
    Transform throwPos;
    [SerializeField]
    int initialCharmCount = 2;
    [SerializeField]
    ConversationData startConversation;
    [SerializeField]
    ConversationData gameOverConversation;

    float healing;

    public static PlayerController Instance;
    public static event System.Action<CreatureController> OnCreatureAdd;

    public EnemyController TargetEnemy { get { return selected as EnemyController; } }
    public List<CreatureController> Creatures { get { return creatures; } }
    public List<EnemyController> Engaged { get { return engaged; } }
    public bool InCombat { get { return engaged.Count > 0; } }
    public int CharmCount { get; private set; }

    Rigidbody2D rigidbody;
    InputActionAsset actions;

    Vector2 dir = Vector2.down;

    List<CreatureController> creatures = new List<CreatureController>();
    List<Vector2> creaturePositions = new List<Vector2>();
    Dictionary<CreatureController, int> creatureIndices = new Dictionary<CreatureController, int>();
    List<CreaturePermutation> permutations = new List<CreaturePermutation>();

    Collider2D[] colliders = new Collider2D[10];
    ColliderComparer colliderComparer = new ColliderComparer();
    ISelectable highlighted;
    ISelectable selected;

    List<EnemyController> engaged = new List<EnemyController>();

    void Awake()
    {
        Instance = this;

        rigidbody = GetComponent<Rigidbody2D>();
        actions = GetComponent<PlayerInput>().actions;

        CharmCount = initialCharmCount;

        CalcCreaturePositions(startCreatures.Length);
        for (int i = 0; i < startCreatures.Length; i++)
            AddCreature(startCreatures[i], null);
    }

    private void Start()
    {
        if (startConversation != null)
            ConversationManager.Instance.StartConversation(startConversation, null);
    }

    void CalcCreaturePositions(int count)
    {
        for (int i = creaturePositions.Count; i < count; i++)
            creaturePositions.Add(Vector2.zero);
        creaturePositions.RemoveRange(count, creaturePositions.Count - count);
        for (int i = 0; i < count; i++)
            creaturePositions[i] = transform.position + Quaternion.Euler(0f, 0f, -creatureSeparation * (count - 1) / 2f + creatureSeparation * i) * -dir * creatureDistance;
    }

    public void AddCreature(CreatureData data, CreatureController obj)
    {
        if (creaturePositions.Count < creatures.Count + 1)
            CalcCreaturePositions(creatures.Count + 1);
        if (obj == null)
        {
            obj = Instantiate(data.Prefab, creaturePositions[creatures.Count], Quaternion.identity);
            obj.Data = data;
        }
        creatureIndices[obj] = creatures.Count;
        if (permutations.Count == 0)
        {
            CreaturePermutation perm = new CreaturePermutation();
            perm.creatures.Add(obj);
            permutations.Add(perm);
        }
        else
        {
            int oldCount = permutations.Count;
            for (int i = 0; i < oldCount; i++)
            {
                for (int j = 0; j < permutations[i].creatures.Count; j++)
                {
                    CreaturePermutation perm = new CreaturePermutation();
                    perm.creatures.AddRange(permutations[i].creatures);
                    perm.creatures.Insert(j, obj);
                    permutations.Add(perm);
                }
                permutations[i].creatures.Add(obj);
            }
        }
        creatures.Add(obj);
        if (OnCreatureAdd != null)
            OnCreatureAdd(obj);
    }

    void AssignCreatures()
    {
        for (int i = 0; i < permutations.Count; i++)
        {
            permutations[i].distance = 0f;
            for (int j = 0; j < permutations[i].creatures.Count; j++)
            {
                permutations[i].distance += Vector2.Distance(permutations[i].creatures[j].transform.position, creaturePositions[j]);
            }
        }
        int minIndex = 0;
        float minDist = permutations[0].distance;
        for (int i = 1; i < permutations.Count; i++)
        {
            if (permutations[i].distance < minDist)
            {
                minIndex = i;
                minDist = permutations[i].distance;
            }
        }
        for (int j = 0; j < permutations[minIndex].creatures.Count; j++)
        {
            creatureIndices[permutations[minIndex].creatures[j]] = j;
        }
    }

    public void AddEnemy(EnemyController enemy)
    {
        if (!engaged.Contains(enemy))
            engaged.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        engaged.Remove(enemy);
    }

    public void ThrowItem(Projectile item)
    {
        if (selected as MonoBehaviour == null || !(selected is Soul) && CharmCount > 0)
            return;

        Projectile inst = Instantiate(item, throwPos.position, Quaternion.identity);
        inst.Launch(selected.HitTarget, null);
        CharmCount--;
    }

    void Update()
    {
        if (ConversationManager.Instance.IsActive)
        {
            return;
        }

        bool anyAlive = creatures.Count == 0;
        for (int i = 0; i < creatures.Count; i++)
        {
            if (creatures[i].IsAlive)
            {
                anyAlive = true;
                break;
            }
        }
        if (!anyAlive)
        {
            if (gameOverConversation != null)
                ConversationManager.Instance.StartConversation(gameOverConversation, () => SceneManager.LoadScene(0));
            else
                SceneManager.LoadScene(0);
            return;
        }

        rigidbody.velocity = actions.FindAction("Move").ReadValue<Vector2>() * moveSpeed;
        if (rigidbody.velocity.magnitude > 0)
        {
            dir = rigidbody.velocity.normalized;
        }
        int colCount = Physics2D.OverlapPointNonAlloc(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), colliders, enemySelectMask.value);
        System.Array.Sort(colliders, 0, colCount, colliderComparer);
        ISelectable newHighlighted = null;
        for (int i = 0; i < colCount; i++)
        {
            ISelectable selectable = colliders[i].GetComponentInParent<ISelectable>();
            if (selectable != null)
            {
                newHighlighted = selectable;
                break;
            }
        }
        if (newHighlighted != highlighted)
        {
            if (highlighted as MonoBehaviour != null)
                highlighted.SetHighlighted(false);
            highlighted = newHighlighted;
            if (highlighted as MonoBehaviour != null)
                highlighted.SetHighlighted(true);
        }
        if (highlighted != selected && !EventSystem.current.IsPointerOverGameObject(0) && actions.FindAction("Select").triggered)
        {
            if (selected as MonoBehaviour != null)
                selected.SetSelected(false);
            selected = highlighted;
            if (selected as MonoBehaviour != null)
                selected.SetSelected(true);
        }

        if (!InCombat)
        {
            healing += healRate * Time.deltaTime;
            if (healing >= 1f)
            {
                for (int i = 0; i < creatures.Count; i++)
                    creatures[i].Heal((int)healing);
                healing -= (int)healing;
            }
        }
        else
        {
            healing = 0f;
        }
    }

    private void FixedUpdate()
    {
        CalcCreaturePositions(creatures.Count);
        AssignCreatures();
    }

    public Vector2 GetCreaturePosition(CreatureController creature)
    {
        return creaturePositions[creatureIndices[creature]];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!InCombat)
        {
            IInteractable[] interactables = collision.GetComponents<IInteractable>();
            for (int i = 0; i < interactables.Length; i++)
            {
                interactables[i].Interact();
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class EnemyCapsule : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Déplacement")]
    public float moveSpeed = 2f;
    public float minDistance = 1f;
    public float maxDistance = 6f;
    public float decisionInterval = 1.5f;

    [Header("Attaque")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public string enemyAttackTag = "AtkEnemy";
    public float hitboxSize = 1f;
    public float hitboxSpawnDistance = 1.2f;
    public float hitboxDuration = 1f;

    private float nextDecisionTime;
    private float nextAttackTime;
    private int moveDirection = 1;
    private bool isAttacking = false;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }

        PickNewDirection();
    }

    void Update()
    {
        if (player == null) return;

        FacePlayer();

        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
            return;
        }

        if (Time.time >= nextDecisionTime)
        {
            PickNewDirection();
        }

        Move(distance);
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void PickNewDirection()
    {
        moveDirection = Random.value > 0.5f ? 1 : -1;
        nextDecisionTime = Time.time + decisionInterval;
    }

    private void Move(float distance)
    {
        if (distance <= minDistance) moveDirection = -1;
        else if (distance >= maxDistance) moveDirection = 1;

        transform.position += transform.forward * moveDirection * moveSpeed * Time.deltaTime;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        SpawnHitbox();

        nextAttackTime = Time.time + attackCooldown;

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }

    private void SpawnHitbox()
    {
        Vector3 spawnPos = transform.position + transform.forward * hitboxSpawnDistance + Vector3.up * (hitboxSize * 0.5f);

        GameObject hitbox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hitbox.transform.position = spawnPos;
        hitbox.transform.rotation = transform.rotation;
        hitbox.transform.localScale = Vector3.one * hitboxSize;
        hitbox.tag = "Untagged";

        Collider col = hitbox.GetComponent<Collider>();
        col.isTrigger = true;

        Renderer rend = hitbox.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Sprites/Default"))
        {
            color = new Color(0f, 1f, 0f, 0.5f)
        };

        Destroy(hitbox, hitboxDuration);
        StartCoroutine(ActivateHitboxAfterDelay(hitbox, rend));
    }

    private IEnumerator ActivateHitboxAfterDelay(GameObject hitbox, Renderer rend)
    {
        yield return new WaitForSeconds(0.5f);

        if (hitbox == null) yield break;

        hitbox.tag = enemyAttackTag;
        rend.material.color = new Color(1f, 0.5f, 0f, 0.5f);
    }
}
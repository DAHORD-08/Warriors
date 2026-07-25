using UnityEngine;
using UnityEngine.InputSystem;

public class ClickCubeSpawner : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public GameObject parryVisualEffectPrefab;

    [Header("Paramètres du cube")]
    public float spawnDistance = 1.5f;
    public float cubeSize = 0.5f;
    public float lifetime = 0.2f;
    [Range(0f, 1f)] public float alpha = 0.25f;

    [Header("Cooldown")]
    public float actionCooldown = 1f;

    [Header("Tags")]
    public string attackTag = "AtkPlayer";
    public string parryTag = "ParryPlayer";
    public string enemyAttackTag = "AtkEnemy";

    private Material redMaterial;
    private Material blueMaterial;
    private float lastActionTime = -999f;

    void Start()
    {
        if (player == null) player = transform;

        Shader shader = Shader.Find("Sprites/Default");
        redMaterial = new Material(shader) { color = new Color(1f, 0f, 0f, alpha) };
        blueMaterial = new Material(shader) { color = new Color(0f, 0f, 1f, alpha) };
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        bool onCooldown = Time.time - lastActionTime < actionCooldown;
        if (onCooldown) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            SpawnAttackCube();
            lastActionTime = Time.time;
        }
        else if (mouse.rightButton.wasPressedThisFrame)
        {
            SpawnParryCube();
            lastActionTime = Time.time;
        }
    }

    private void SpawnAttackCube()
    {
        GameObject cube = CreateCube(redMaterial, attackTag);
        Destroy(cube.GetComponent<Collider>());
        Destroy(cube, lifetime);
    }

    private void SpawnParryCube()
    {
        GameObject cube = CreateCube(blueMaterial, parryTag);

        Collider col = cube.GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        ParryCubeTrigger trigger = cube.AddComponent<ParryCubeTrigger>();
        trigger.enemyAttackTag = enemyAttackTag;
        trigger.visualEffectPrefab = parryVisualEffectPrefab;

        Destroy(cube, lifetime);
    }

    private GameObject CreateCube(Material material, string tag)
    {
        Vector3 spawnPos = player.position + player.forward * spawnDistance + Vector3.up * (cubeSize * 0.5f);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = spawnPos;
        cube.transform.rotation = player.rotation;
        cube.transform.localScale = Vector3.one * cubeSize;
        cube.tag = tag;
        cube.GetComponent<Renderer>().material = material;

        return cube;
    }
}

public class ParryCubeTrigger : MonoBehaviour
{
    public string enemyAttackTag = "AtkEnemy";
    public GameObject visualEffectPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyAttackTag)) return;

        if (visualEffectPrefab != null)
            Instantiate(visualEffectPrefab, transform.position, Quaternion.identity);
    }
}
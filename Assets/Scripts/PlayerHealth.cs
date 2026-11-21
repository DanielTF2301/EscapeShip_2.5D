using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int lives = 5;
    [SerializeField] private float invulnerableDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private Color damageColor = new Color(1f, 0.25f, 0.25f);

    int currentLives;
    bool invulnerable;
    bool dead;
    Renderer[] bodyRenderers;
    MaterialPropertyBlock tintBlock;
    PlayerShrink shrink;
    CollisionManager collisionManager;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        currentLives = lives;
        tintBlock = new MaterialPropertyBlock();
        shrink = GetComponent<PlayerShrink>();
        collisionManager = GetComponent<CollisionManager>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        List<Renderer> body = new List<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] is MeshRenderer || renderers[i] is SkinnedMeshRenderer)
            {
                body.Add(renderers[i]);
            }
        }

        bodyRenderers = body.ToArray();
    }

    void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    void OnCollisionStay(Collision collision)
    {
        TryDamage(collision.collider);
    }

    void TryDamage(Collider other)
    {
        if (dead || invulnerable)
        {
            return;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
        {
            return;
        }

        // While shrunk the ship takes more damage than normal.
        float multiplier = shrink != null ? shrink.DamageTakenMultiplier : 1f;
        int finalDamage = Mathf.CeilToInt(enemy.Damage * multiplier);

        invulnerable = true;
        currentLives -= finalDamage;

        if (currentLives <= 0)
        {
            dead = true;
            if (collisionManager != null)
            {
                collisionManager.Explode();
            }
            return;
        }

        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        float elapsed = 0f;
        bool showDamageColor = false;

        while (elapsed < invulnerableDuration)
        {
            showDamageColor = !showDamageColor;
            if (showDamageColor)
            {
                SetTint(damageColor);
            }
            else
            {
                ClearTint();
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        ClearTint();
        invulnerable = false;
    }

    void SetTint(Color color)
    {
        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            bodyRenderers[i].GetPropertyBlock(tintBlock);
            tintBlock.SetColor(BaseColorId, color);
            tintBlock.SetColor(ColorId, color);
            tintBlock.SetColor(EmissionColorId, color);
            bodyRenderers[i].SetPropertyBlock(tintBlock);
        }
    }

    void ClearTint()
    {
        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            bodyRenderers[i].SetPropertyBlock(null);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(16f, 16f, 220f, 32f), "Lives: " + Mathf.Max(currentLives, 0));
    }
}
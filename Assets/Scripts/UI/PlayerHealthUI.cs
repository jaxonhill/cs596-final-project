using System.Collections.Generic;
using Combat;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Combat;
using Player;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health Source")]
    [SerializeField] private Damageable damageable;
    [SerializeField] private bool autoFindPlayerDamageable = true;

    [Header("Heart Display")]
    [SerializeField] private Transform heartsParent;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Vector2 heartSize = new Vector2(56f, 56f);
    [SerializeField] private Color fullHeartColor = Color.white;
    [SerializeField] private Color emptyHeartColor = new Color(1f, 1f, 1f, 0.25f);

    private readonly List<Image> hearts = new List<Image>();
    private int lastCurrentHealth = -1;
    private int lastMaxHealth = -1;
    private bool loggedMissingSource;
    private bool loggedMissingDisplay;

    private void Awake()
    {
        if (heartsParent == null)
        {
            heartsParent = transform;
        }

        ResolveDamageable();
    }

    private void OnEnable()
    {
        ResolveDamageable();
    }

    private void Start()
    {
        UpdateHealthDisplay();
    }

    private void Update()
    {
        UpdateHealthDisplay();
    }

    private void ResolveDamageable()
    {
        if (damageable != null || !autoFindPlayerDamageable)
        {
            return;
        }

        PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
        if (player != null && player.PlayerDamageable != null)
        {
            damageable = player.PlayerDamageable;
            return;
        }

        damageable = FindFirstObjectByType<Damageable>();
    }

    private void UpdateHealthDisplay()
    {
        if (damageable == null)
        {
            ResolveDamageable();

            if (damageable == null)
            {
                if (!loggedMissingSource)
                {
                    Debug.LogWarning("PlayerHealthUI has no Damageable reference.", this);
                    loggedMissingSource = true;
                }

                return;
            }
        }

        int currentHealth = damageable.CurrentHealth;
        int maxHealth = damageable.MaxHealth;

        if (heartSprite == null || heartsParent == null)
        {
            if (!loggedMissingDisplay)
            {
                Debug.LogWarning("PlayerHealthUI needs a heart sprite and hearts parent assigned.", this);
                loggedMissingDisplay = true;
            }

            return;
        }

        if (currentHealth == lastCurrentHealth && maxHealth == lastMaxHealth)
        {
            return;
        }

        if (lastMaxHealth != maxHealth)
        {
            RebuildHearts(maxHealth);
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].color = i < currentHealth ? fullHeartColor : emptyHeartColor;
        }

        lastCurrentHealth = currentHealth;
        lastMaxHealth = maxHealth;
    }

    private void RebuildHearts(int maxHealth)
    {
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            if (hearts[i] != null)
            {
                Destroy(hearts[i].gameObject);
            }
        }

        hearts.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heartObject = new GameObject($"Heart {i + 1}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            heartObject.transform.SetParent(heartsParent, false);

            RectTransform heartTransform = heartObject.GetComponent<RectTransform>();
            heartTransform.sizeDelta = heartSize;

            Image heart = heartObject.GetComponent<Image>();
            heart.sprite = heartSprite;
            heart.preserveAspect = true;
            heart.raycastTarget = false;

            hearts.Add(heart);
        }
    }
}

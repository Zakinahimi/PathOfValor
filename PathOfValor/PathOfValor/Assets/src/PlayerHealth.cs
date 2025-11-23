
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 20;
    public int health;
    public AudioSource audioPlayer;
    private float delayTime = 1.0f;
    private float timeElapsed;
    public GameObject Player;
    static public bool playerAlive = true;

    [Header("Hit Settings")]
    [Tooltip("Minimum time in seconds between registering damage hits.")]
    public float minHitInterval = 0.2f;

    float lastHitTime;

    PlayerHandler playerHandler;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        health = maxHealth;
        Transition.Alive = true;
        playerHandler = GetComponent<PlayerHandler>();
        animator = GetComponent<Animator>();
        Transition.level = SceneManager.GetActiveScene().name;
        Transition.lvlindex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void changeScene()
    {
        Transition.Alive = false;
        SceneManager.LoadScene("deathAnimation");
        Destroy(Player);
    }
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // Prevent multiple hits in the same instant so health doesn't drop by many at once.
        if (Time.time < lastHitTime + minHitInterval) return;
        lastHitTime = Time.time;

        health -= amount;
        Debug.Log($"Player took {amount} damage, health now: {health}/{maxHealth}");

        if (health > 0 && animator != null)
        {
            animator.SetTrigger("Hurt"); // HeroKnight controller expects this trigger.
        }

        if(health <= 0)
        {
            health = 0;
            playerHandler.TriggerDeathAnimation();
            if (audioPlayer != null)
            {
                audioPlayer.Play();
            }
            Invoke("changeScene", delayTime);
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;
        int delta = Mathf.CeilToInt(amount);
        health = Mathf.Clamp(health + delta, 0, maxHealth);
    }
    
}

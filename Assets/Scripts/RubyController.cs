using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{

    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip victorySound;
    public AudioClip loseSound;
    public GameObject backgroundMusic;

    public ParticleSystem hitEffect;
    public ParticleSystem healthEffect;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    public AudioSource musicSource;

    private static int scoreValue = 0;
    public Text scoreText;
    public Text gameOverText;
    public Text cogText;
    private int cogs = 4;
    
    


    bool gameOver = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        cogText.text = "Cogs: " + cogs.ToString();

    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C) && cogs > 0)
        {
            Launch();
            cogs -= 1;
            cogText.text = "Cogs: " + cogs.ToString();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (scoreValue == 4)
                {
                    SceneManager.LoadScene("Level 2");
                }
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);

 
    }

   

    public void ChangeScore(int scoreAmount)
    {
        scoreValue += scoreAmount;
        scoreText.text = "Robots Fixed: " + scoreValue.ToString();

        if (scoreValue == 4)
        {
            gameOverText.text = "Talk to Jambi to visit stage two!";

        }

        if (scoreValue == 8)
        {
            gameOverText.text = "You Win! Created by: Joseph Coover";
            gameOver = true;
            backgroundMusic.SetActive(false);
            PlaySound(victorySound);
        }

    }

    public void ChangeAmmo(int count)
    {
        cogs += count;
        cogText.text = "Cogs: " + cogs;
        
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            ParticleSystem Hit = Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }


        if (amount > 0)
        {
            ParticleSystem Health = Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        if (health == 1)
        {
            gameOverText.text = "You Lose! Press R to Restart";
            gameOver = true;
            speed = 0.0f;
            backgroundMusic.SetActive(false);
            PlaySound(loseSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }



    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

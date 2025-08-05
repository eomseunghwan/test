using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MouseController : MonoBehaviour
{
    public float jetpackForce;
    public float fowardMovementSpeed; // fwdMoveSpeed

    private Rigidbody2D rb;

    public Transform groundCheckTransform;
    public LayerMask groundChecLayerMask;

    private bool grounded;
    private Animator animator;

    public ParticleSystem jetpack;

    private bool dead = false;

    private uint coins = 0;
    public Text textCoins;

    public GameObject buttonRestart;
    public GameObject buttonGoMenu;

    public AudioClip coinCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;

    public ParallaxScroll parallaxScroll;

    private int level;
    private Coroutine lvUpCoroutine;

    public Text lvTxt;

    public FeverController fc;

    private bool pause;

    private void Start()
    {
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        textCoins.text = coins.ToString();

        level = 1;
        LevelApply();

        lvUpCoroutine = StartCoroutine(LevelUpLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    private void FixedUpdate()
    {
        bool jetpackActive = false;

        if (!dead)
        {
            jetpackActive = Input.GetButton("Fire1");

            if (jetpackActive)
            {
                rb.AddForce(jetpackForce * Vector2.up);
            }

            Vector2 newVelocity = rb.velocity;
            newVelocity.x = fowardMovementSpeed;
            rb.velocity = newVelocity;
        }

        UpdateGroundedStatus();

        AdjustJetpack(jetpackActive);

        DisplayRestartButton();

        AdjustFootstepAndJetpackSound(jetpackActive);

        parallaxScroll.offset = transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coins")
        {
            CollectCoin(collision);
        }

        else
        {
            HitByLaser(collision);
        }
    }

    IEnumerator LevelUpLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);

            level++;

            LevelApply();
        }
    }

    public void LevelApply()
    {
        lvTxt.text = $"Lv.{level:00}";

        if (!fc.fever)
        {
            fowardMovementSpeed = 2.7f + level * 0.3f;
        }
        //fowardMovementSpeed += 0.3f;
    }

    private void CollectCoin(Collider2D coinCollider)
    {
        ++coins;
        //Debug.Log("Coin : " + coins);
        textCoins.text = coins.ToString();
        Destroy(coinCollider.gameObject);

        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
    }

    private void HitByLaser(Collider2D laserCollider)
    {
        if (dead) // == if (dead) return;
        {
            return;
        }

        AudioSource laser = laserCollider.GetComponent<AudioSource>();
        laser.Play();

        dead = true;
        animator.SetBool("dead", true);

        StopCoroutine(lvUpCoroutine);
    }

    private void AdjustJetpack(bool jetpackActive)
    {
        var emission = jetpack.emission;
        emission.enabled = !grounded;
        emission.rateOverTime = jetpackActive ? 300f : 75f;
    }

    private void UpdateGroundedStatus()
    {
        grounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundChecLayerMask);
        animator.SetBool("grounded", grounded);
    }

    private void DisplayRestartButton()
    {
        bool active = buttonRestart.activeSelf;
        if (grounded && dead && !active)
        {
            buttonRestart.SetActive(true);
            buttonGoMenu.SetActive(true);
        }
    }

    public void OnClickedRestartButton()
    {
        Time.timeScale = 1f;
        // string SceneName = SceneManager.GetActiveScene().name;
        // SceneManager.LoadScene(SceneName)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClicedkGoMenuButton()
    {
        //SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // buildsetting 에서 나오는 값
    }

    public void AdjustFootstepAndJetpackSound(bool jetpackActive)
    {
        footstepsAudio.enabled = !dead && grounded;
        jetpackAudio.enabled = !dead && !grounded;
        jetpackAudio.volume = jetpackActive ? 1f : 0.5f;
    }

    public void Pause()
    {
        if (pause)
        {
            pause = false;
            Time.timeScale = 1f;

            buttonRestart.SetActive(false);
            buttonGoMenu.SetActive(false);
        }

        else
        {
            pause = true;
            Time.timeScale = 0f;

            buttonRestart.SetActive(true);
            buttonGoMenu.SetActive(true);
        }
    }
}

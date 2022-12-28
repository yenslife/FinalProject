using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 3.5f;
    public float jumpVelocity = 0f;
    bool canJump = false;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public CharacterController characterController;
    public Transform cam;
    public bool IsPause = false;
    public BarScript healthBar;
    public BarScript shieldBar;
    
    
    
    public int currentHealth;
    public int currentShield;
    public int maxShield = 20;
    public int maxHealth = 100;
    public int coinNumber = 0;

    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        myAnimator = GetComponent<Animator>();
        InitializeHealthStatus(); // initialize player's health
        InitializeShieldStatus(); // initialize player's shield

        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; // 鼠標消失
    }

    // Update is called once per frame
    void Update()
    {
        myAnimator.SetBool("run", false);
        Movement();
        PauseAndResumeTheGame();
        PlayerAliveCheck();
        ShieldRecover();
    }

    private void Movement()//��k
    {
        // 用角度進行方向控制
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (gameObject.GetComponent<WeaponChange>().now_is_sword)
        {
            if (direction.magnitude >= 0.1f)
            {
                myAnimator.SetBool("run", true);
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f); // when using sword
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                characterController.Move(moveDir.normalized * speed * Time.deltaTime);
                myAnimator.SetBool("run", true);
            }
        } else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y, transform.eulerAngles.z); // when using gun
            if (direction.magnitude >= 0.1f)
            {
                myAnimator.SetBool("run", true);
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                characterController.Move(moveDir.normalized * speed * Time.deltaTime);
                myAnimator.SetBool("run", true);
            }
        }

        // jump
        if (characterController.isGrounded)
        {
            canJump = false;
            jumpVelocity = 0f;
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log("jumping!!");
                jumpVelocity = jumpHeight;
                canJump = true;
            }
        }
        Vector3 yDirection = new Vector3(0f, jumpVelocity, 0f).normalized;
        if (yDirection.magnitude >= 0.5 && canJump) characterController.Move(yDirection * 5 * Time.deltaTime);
    }

    // pause the game using esc key
    private void PauseAndResumeTheGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPause = !IsPause;
            Cursor.visible = (IsPause) ? true : false;
            Cursor.lockState = (IsPause) ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = (IsPause) ? 0 : 1;
            // todo: show menu when pausing the game
        }
    }

    private void InitializeHealthStatus()
    {
        healthBar.GetComponent<BarScript>().SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        if (currentShield <= 0) currentHealth -= damage;
        else currentShield -= damage;
        healthBar.GetComponent<BarScript>().SetHealth(currentHealth);
        shieldBar.GetComponent<BarScript>().SetHealth(currentShield);
    }

    public void PlayerAliveCheck()
    {
        if (currentHealth <= 0)
        {
            PlayerPrefs.SetInt("coin", coinNumber); // store the coin number, this will be used in other scene
            PlayerPrefs.SetInt("health", currentHealth); // same as above

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("GameOverScene");
        }
    }
    public void addHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(maxHealth);
    }
    private void InitializeShieldStatus()
    {
        shieldBar.GetComponent<BarScript>().SetMaxHealth(maxShield);
        currentShield = maxShield;
    }

    // shield recover system
    public float period = 0.0f;
    int test = 0;
    private void ShieldRecover()
    {
        period += UnityEngine.Time.deltaTime;
        if (period > 1)
        {
            Debug.Log(test++);
            currentShield += 5;
            if (currentShield > maxShield) currentShield = maxShield;
            shieldBar.SetHealth(currentShield);
            period = 0;
        }
    }
    
}
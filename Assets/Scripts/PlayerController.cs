using UnityEngine;
using System.Collections;
using static UnityEngine.Input;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D _rb;
    public float movementSpeed;
    [SerializeField] SpriteRenderer _characterBody;
    [SerializeField] private Animator _animator;
    Color color;
    private bool isDead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        color = _characterBody.color;
        movementSpeed = 4.5f;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDead = true;
            Debug.Log("space pressed, hero dies");
            _animator.SetBool("isDead", isDead);
            StartCoroutine(alphaLerpingFunction(0.5f, 4.5f));

        }

        if (!isDead)
        {
            HandlePlayerMovement();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
           
        }

    }

    private void HandlePlayerMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        movement = Vector2.ClampMagnitude(movement, 1.0f);
        _rb.linearVelocity = movement * movementSpeed;

        bool characterIsWalking = movement.magnitude > 0.0f;
        _animator.SetBool("isWalking", characterIsWalking);

        bool flipSprite = movement.x < 0.0f;
        _characterBody.flipX = flipSprite;
    }

    IEnumerator alphaLerpingFunction(float endValue, float duration)
    {
        float time = 0;
        float startValue = color.a;

        while(time < duration)
        {
            
            time += Time.deltaTime;
            if(time > 1.0)
            {
                color.a = Mathf.Lerp(startValue, endValue, (time - 1.0f) / duration);
                _characterBody.color = color;
            }
            
            yield return null;
        }
        color.a = endValue;
        _characterBody.color = color;
    }
}

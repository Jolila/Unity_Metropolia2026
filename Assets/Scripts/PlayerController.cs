using UnityEngine;
using static UnityEngine.Input;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D _rb;
    public float movementSpeed;
    [SerializeField] SpriteRenderer _characterBody;
    [SerializeField] private Animator _animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        movementSpeed = 3.5f;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovement();
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
}

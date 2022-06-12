using UnityEngine;

namespace MazeGame.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _movementSpeed;

        private Vector2 _direction;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Sprite skin = SkinsStorage.Instance.CurrentSkin;
            _spriteRenderer.sprite = skin;
        }

        private void Update()
        {
            //Vector3 acceleration = Input.acceleration;
            //_direction = new Vector2(acceleration.x, acceleration.y);

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            _direction = new Vector2(x, y);
        }

        private void FixedUpdate()
        {
            _rigidbody2D.velocity = _direction * _movementSpeed;
        }
    }
}
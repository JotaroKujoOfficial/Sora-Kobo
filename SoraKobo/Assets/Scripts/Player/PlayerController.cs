using UnityEngine;
using Mirror;
using SoraKobo.UI;

namespace SoraKobo.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundCheckRadius = 0.1f;

        [Header("Animation")]
        public Animator animator;
        public SpriteRenderer spriteRenderer;

        [Header("Interaction")]
        public float interactRange = 1.5f;
        public LayerMask interactableLayer;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName = "Player";

        [SyncVar] public int hairIndex;
        [SyncVar] public int outfitIndex;
        [SyncVar] public int accessoryIndex;
        [SyncVar] public Color hairColor   = Color.yellow;
        [SyncVar] public Color skinColor   = new Color(1f, 0.85f, 0.7f);
        [SyncVar] public Color outfitColor = Color.blue;

        private Rigidbody2D _rb;
        private bool _isGrounded;
        private Vector2 _moveInput;
        private bool _facingRight = true;
        private FloatingJoystick _joystick;
        private PlayerCustomization _customization;
        private PlayerNameTag _nameTag;
        private bool _inBuildMode;
        public bool InBuildMode => _inBuildMode;

        void Awake()
        {
            _rb            = GetComponent<Rigidbody2D>();
            _customization = GetComponent<PlayerCustomization>();
            _nameTag       = GetComponentInChildren<PlayerNameTag>();
        }

        public override void OnStartLocalPlayer()
        {
            _joystick = FindObjectOfType<FloatingJoystick>();

            var saved = PlayerPrefs.GetString("PlayerSave", "");
            if (!string.IsNullOrEmpty(saved))
            {
                var data = JsonUtility.FromJson<SoraKobo.Data.PlayerSaveData>(saved);
                CmdSetAppearance(data.playerName, data.hairIndex, data.outfitIndex,
                    data.accessoryIndex, data.hairColor, data.skinColor, data.outfitColor);
            }
            else
            {
                CmdSetAppearance("Player", 0, 0, 0, Color.yellow,
                    new Color(1f, 0.85f, 0.7f), Color.blue);
            }

            var cam = Camera.main;
            if (cam != null)
            {
                var follow = cam.GetComponent<SoraKobo.Camera.CameraFollow>();
                if (follow != null) follow.SetTarget(transform);
            }

            var hud = FindObjectOfType<HUDController>();
            if (hud != null) hud.OnLocalPlayerSpawned(this);
        }

        void Update()
        {
            if (!isLocalPlayer) return;
            if (_inBuildMode) return;
            ReadInput();
            CheckGround();
            HandleAnimation();
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            if (_inBuildMode) { _rb.velocity = Vector2.zero; return; }
            Move();
        }

        void ReadInput()
        {
            float h = _joystick != null ? _joystick.Horizontal : Input.GetAxisRaw("Horizontal");
            _moveInput = new Vector2(h, 0f);
            if (h > 0.01f && !_facingRight) Flip();
            else if (h < -0.01f && _facingRight) Flip();
        }

        void Move() => _rb.velocity = new Vector2(_moveInput.x * moveSpeed, _rb.velocity.y);

        public void Jump()
        {
            if (_isGrounded)
                _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        void CheckGround()
        {
            if (groundCheck == null) return;
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        void HandleAnimation()
        {
            if (animator == null) return;
            animator.SetFloat("Speed",   Mathf.Abs(_moveInput.x));
            animator.SetBool("Grounded", _isGrounded);
        }

        void Flip()
        {
            _facingRight = !_facingRight;
            transform.localScale = new Vector3(_facingRight ? 1 : -1, 1, 1);
        }

        public void TryInteract()
        {
            if (!isLocalPlayer) return;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);
            float closest = float.MaxValue;
            Interaction.InteractableObject target = null;
            foreach (var h in hits)
            {
                var obj = h.GetComponent<Interaction.InteractableObject>();
                if (obj == null) continue;
                float dist = Vector2.Distance(transform.position, h.transform.position);
                if (dist < closest) { closest = dist; target = obj; }
            }
            target?.Interact(this);
        }

        public void SetBuildMode(bool active)
        {
            _inBuildMode = active;
            if (active) _rb.velocity = Vector2.zero;
        }

        [Command]
        public void CmdSetAppearance(string name, int hair, int outfit, int acc,
            Color hColor, Color sColor, Color oColor)
        {
            playerName   = name;
            hairIndex    = hair;
            outfitIndex  = outfit;
            accessoryIndex = acc;
            hairColor    = hColor;
            skinColor    = sColor;
            outfitColor  = oColor;
        }

        void OnNameChanged(string _, string newVal)
        {
            if (_nameTag != null) _nameTag.SetName(newVal);
        }

        // ── Emotes ────────────────────────────────────────────────────────
        // Client calls Command → server validates → RpcPlayEmote on all clients

        public void PlayEmote(int emoteId) => CmdPlayEmote(emoteId);

        [Command]
        void CmdPlayEmote(int emoteId)
        {
            RpcPlayEmote(emoteId);
        }

        [ClientRpc]
        public void RpcPlayEmote(int emoteId)
        {
            if (animator != null) animator.SetTrigger("Emote" + emoteId);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (groundCheck != null)
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}

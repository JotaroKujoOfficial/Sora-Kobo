using UnityEngine;
using Mirror;
using SoraKobo.Player;

namespace SoraKobo.Interaction
{
    public class VehicleObject : InteractableObject
    {
        [Header("Vehicle Settings")]
        public float vehicleSpeed = 8f;
        public Transform driverSeat;

        [SyncVar] private bool _hasDriver = false;
        private Player.PlayerController _driver;
        private Rigidbody2D _rb;
        private UI.FloatingJoystick _joystick;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            interactType = InteractableType.Vehicle;
            interactPrompt = "Enter vehicle";
        }

        public override void Interact(Player.PlayerController player)
        {
            if (!player.isLocalPlayer) return;

            if (_driver == player)
            {
                // Exit vehicle
                ExitVehicle(player);
            }
            else if (!_hasDriver)
            {
                // Enter vehicle
                EnterVehicle(player);
            }
        }

        void EnterVehicle(Player.PlayerController player)
        {
            _driver = player;
            player.enabled = false;
            player.GetComponent<Rigidbody2D>().simulated = false;
            player.transform.SetParent(transform);
            if (driverSeat != null) player.transform.localPosition = driverSeat.localPosition;

            _joystick = FindObjectOfType<UI.FloatingJoystick>();
            CmdSetDriver(true, player.netId);
        }

        void ExitVehicle(Player.PlayerController player)
        {
            player.transform.SetParent(null);
            player.transform.position = transform.position + Vector3.right * 1.5f;
            player.enabled = true;
            player.GetComponent<Rigidbody2D>().simulated = true;
            _driver = null;
            _joystick = null;
            CmdSetDriver(false, 0);
        }

        [Command(requiresAuthority = false)]
        void CmdSetDriver(bool hasDriver, uint driverId)
        {
            _hasDriver = hasDriver;
        }

        void FixedUpdate()
        {
            if (_driver == null || !_driver.isLocalPlayer || _rb == null) return;
            float h = _joystick != null ? _joystick.Horizontal : Input.GetAxisRaw("Horizontal");
            _rb.velocity = new Vector2(h * vehicleSpeed, _rb.velocity.y);
            if (h > 0.01f) transform.localScale = new Vector3(1, 1, 1);
            else if (h < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

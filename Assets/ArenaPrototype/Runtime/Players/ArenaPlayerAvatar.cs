using System.Collections;
using UnityEngine;

namespace ArenaPrototype.Runtime.Players
{
    [RequireComponent(typeof(ArenaPlayerMotor))]
    public sealed class ArenaPlayerAvatar : MonoBehaviour
    {
        [SerializeField] private int playerIndex;

        private Rigidbody2D _rigidbody2D;
        private ArenaPlayerMotor _motor;
        private Behaviour[] _behaviours;
        private Collider2D[] _colliders;
        private Renderer[] _renderers;

        public int PlayerIndex => playerIndex;
        public bool IsRespawning { get; private set; }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _motor = GetComponent<ArenaPlayerMotor>();
            _behaviours = GetComponents<Behaviour>();
            _colliders = GetComponentsInChildren<Collider2D>(true);
            _renderers = GetComponentsInChildren<Renderer>(true);
        }

        public IEnumerator RespawnRoutine(Vector3 spawnPosition, float delaySeconds)
        {
            IsRespawning = true;
            SetGameplayEnabled(false);

            if (_rigidbody2D != null)
            {
                _rigidbody2D.simulated = false;
            }

            yield return new WaitForSeconds(delaySeconds);

            transform.position = spawnPosition;
            if (_motor != null)
            {
                _motor.ResetMotion();
            }

            if (_rigidbody2D != null)
            {
                _rigidbody2D.simulated = true;
            }

            SetGameplayEnabled(true);
            IsRespawning = false;
        }

        public void SetGameplayEnabled(bool enabled)
        {
            for (var i = 0; i < _behaviours.Length; i++)
            {
                var behaviour = _behaviours[i];
                if (behaviour == this)
                {
                    continue;
                }

                behaviour.enabled = enabled;
            }

            for (var i = 0; i < _colliders.Length; i++)
            {
                _colliders[i].enabled = enabled;
            }

            for (var i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = enabled;
            }
        }
    }
}

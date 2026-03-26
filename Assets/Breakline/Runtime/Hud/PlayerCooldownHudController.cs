using Breakline.Runtime.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Breakline.Runtime.Hud
{
    // Optional HUD module for local prototype readability.
    // Connect Image fills for radial/bar cooldowns and optional text labels.
    public sealed class PlayerCooldownHudController : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Image dashCooldownFill;
        [SerializeField] private Image attackCooldownFill;
        [SerializeField] private Text dashLabel;
        [SerializeField] private Text attackLabel;

        private void OnEnable()
        {
            if (player != null)
            {
                player.CombatHudChanged += OnCombatHudChanged;
            }
        }

        private void OnDisable()
        {
            if (player != null)
            {
                player.CombatHudChanged -= OnCombatHudChanged;
            }
        }

        private void OnCombatHudChanged(PlayerCombatHudSnapshot snapshot)
        {
            if (dashCooldownFill != null)
            {
                dashCooldownFill.fillAmount = 1f - snapshot.DashCooldownNormalized;
            }

            if (attackCooldownFill != null)
            {
                attackCooldownFill.fillAmount = 1f - snapshot.AttackCooldownNormalized;
            }

            if (dashLabel != null)
            {
                dashLabel.text = snapshot.DashReady ? "DASH" : "DASH...";
            }

            if (attackLabel != null)
            {
                attackLabel.text = snapshot.AttackReady ? "ATK" : "ATK...";
            }
        }
    }
}

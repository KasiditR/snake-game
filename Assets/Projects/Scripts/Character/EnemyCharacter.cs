using SnakeGame.Data;
using UnityEngine;

namespace SnakeGame.Character
{
    public class EnemyCharacter : BaseCharacter
    {
        [SerializeField] private float _scoreDrop;
        [SerializeField] private SpriteRenderer _fillRenderer;

        public float ScoreDrop { get => _scoreDrop; }

        public void Initialize()
        {
            base.Initialize(maxHp, damage);
            FillHealth();
            this.gameObject.SetActive(true);
        }

        public override void TakeDamage(DamageInfo damageInfo)
        {
            currentHp -= damageInfo.damage;

            if (currentHp <= 0 && !isDie)
            {
                isDie = true;
                Die();
            }

            FillHealth();
        }

        private void FillHealth()
        {
            if (maxHp > 0)
            {
                float fillAmount = (float)this.currentHp / maxHp;
                _fillRenderer.size = new Vector2(fillAmount, _fillRenderer.size.y);
            }
            else
            {
                _fillRenderer.size = new Vector2(0, _fillRenderer.size.y);
            }
        }

        public override void Die()
        {
            OnDie?.Invoke(this);
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent<AdventurerCharacter>(out AdventurerCharacter adventure))
            {
                if (!adventure.IsDie)
                {
                    adventure.TakeDamage(new DamageInfo(damage));
                }
            }
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}
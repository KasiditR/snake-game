using System.Collections;
using SnakeGame.Data;
using UnityEngine;

namespace SnakeGame.Character
{
    public class AdventurerCharacter : BaseCharacter
    {
        [SerializeField] private SpriteRenderer _avatarRenderer;
        [SerializeField] private SpriteRenderer _fillRenderer;
        [SerializeField] private Vector2 _direction;
        [SerializeField] private Animator _animator;
        private bool _isAttack;
        private Coroutine _delayAttack;

        public void Initialize()
        {
            base.Initialize(maxHp, damage);
            FillHealth();
            SetMember();
            this.gameObject.SetActive(true);
        }

        public override void TakeDamage(DamageInfo damageInfo)
        {
            this.currentHp -= damageInfo.damage;

            FillHealth();

            if (this.currentHp <= 0 && !isDie)
            {
                isDie = true;
                Die();
            }
        }

        public override void Die()
        {
            OnDie?.Invoke(this);
            if (_delayAttack != null)
            {
                StopCoroutine(_delayAttack);
            }
            this.gameObject.SetActive(false);
        }

        public void SetLeader()
        {
            _avatarRenderer.transform.localScale = new Vector3(12, 12, 1);
            _avatarRenderer.color = Color.red;
        }

        public void SetMember()
        {
            _avatarRenderer.transform.localScale = new Vector3(7, 7, 1);
            _avatarRenderer.color = Color.white;
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

        public void SetDirection(Vector2 direction)
        {
            if (_isAttack)
            {
                return;
            }

            _direction = direction;
            BlendByDirection(_direction);
        }

        public void BlendByDirection(Vector2 direction)
        {
            _animator.SetFloat("X", direction.x);
            _animator.SetFloat("Y", direction.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isDie)
            {
                return;
            }

            if (other.gameObject.TryGetComponent<EnemyCharacter>(out EnemyCharacter enemy))
            {
                if (!enemy.IsDie)
                {
                    SetDirection(_direction * 2);
                    enemy.TakeDamage(new DamageInfo(damage));
                    _delayAttack = StartCoroutine(DelayAnimationAttack());
                }
            }
        }

        private IEnumerator DelayAnimationAttack()
        {
            _isAttack = true;
            yield return new WaitForSeconds(0.4f);
            _isAttack = false;
        }

        public void DieImmediate()
        {
            this.isDie = true;
            this.currentHp = 0;
            this.Die();
        }

        public override void OnPause()
        {
            _animator.speed = 0;
        }

        public override void OnResume()
        {
            _animator.speed = 1;
        }
    }
}
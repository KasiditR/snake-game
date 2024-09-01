using System;
using UnityEngine;
using SnakeGame.Manager;
using SnakeGame.Data;
using SnakeGame.Interface;

namespace SnakeGame.Character
{
    public abstract class BaseCharacter : MonoBehaviour, IDamageable, IPausable
    {
        [SerializeField] protected int currentHp;
        [SerializeField] protected int maxHp;
        [SerializeField] protected int damage;
        protected bool isDie;
        public Action<BaseCharacter> OnDie;

        public bool IsDie { get => isDie; }

        public virtual void Initialize(int hp, int damage)
        {
            this.currentHp = hp;
            this.damage = damage;
            isDie = false;
        }

        protected virtual void OnEnable()
        {
            PauseManager.Instance.RegisterPausable(this);
        }

        protected virtual void OnDisable()
        {
            PauseManager.Instance.UnregisterPausable(this);
        }

        public abstract void TakeDamage(DamageInfo damageInfo);
        public abstract void Die();

        public abstract void OnPause();
        public abstract void OnResume();
    }

}
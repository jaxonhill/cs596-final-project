using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for managing the health of an entity
    [HideMonoScript]
    public class Health : EntityComponent
    {
        // HEADER: FIELDS (and inspector modifiers)
        
        protected override string GetMainValue(){return "Health";}
        protected override string GetMainTooltip(){return "The current amount of health this entity has";}
        
        private int health {
            get => val;
            set => val = value; }
        
        [Tooltip("The max amount of HP this entity can obtain"), SerializeField] 
        private int maxHealth = 3;

        [Tooltip("The amount of frames this entity is invincible for"), SerializeField]
        private int iFrames = 1500;

        /// Whether this entity is currently invincible
        private bool isInvincible;
        
        // HEADER: CONSTRUCTOR
        
        public Health() { health = maxHealth; }
        
        // HEADER: GETTERS / SETTERS
        
        public int GetMaxHealth(){return maxHealth;}
        
        public void SetMaxHealth(int value){maxHealth = value;}
        
        public int GetIFrames(){return iFrames;}
        
        public bool IsInvincible(){return isInvincible;}
        
        public void SetInvincible(bool value = false){isInvincible = value;}
    
        
        // HEADER: EXTRA MODIFIERS
        
        /// Raise Health by the given amount
        public void RaiseHealth(int value){health += value;}

        public void LowerHealth(int value) { health -= value; }

        public virtual void OnDamaged(int value, bool ignoreIFrames = false)
        {
            LowerHealth(value);
            if (!ignoreIFrames) return; 
            isInvincible = true;
            Invoke(nameof(SetInvincible), iFrames);
        }
    }
}
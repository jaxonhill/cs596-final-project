using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for managing the health of an entity
    [HideMonoScript]
    public class Health : Component
    {
        protected override string GetMainValue(){return "Health";}
        protected override string GetMainTooltip(){return "The current amount of health this entity has";}
        
        private int health {
            get => val;
            set => val = value; }

        public Health() { health = maxHealth; }

        [Tooltip("The max amount of HP this entity can obtain"), SerializeField] 
        private int maxHealth = 3;
        
        public int GetMaxHealth(){return maxHealth;}
        
        public void SetMaxHealth(int value){maxHealth = value;}
    
        /// Increase health by 1
        public void IncrementHealth(){health++;}
    
        /// Increase health by 1
        public void DecrementHealth(){health--;}
    }
}
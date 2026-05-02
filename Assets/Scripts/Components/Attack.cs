using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for managing the attack qualities of an entity
    [HideMonoScript]
    public class Attack : Component
    {
        protected override string GetMainValue(){return "Damage";}
        protected override string GetMainTooltip(){return "The amount of health an attack from this entity will take";}
        
        private int damage {
            get => val;
            set => val = value; }
        
        [Tooltip("The distance the attack can reach"), SerializeField] 
        private int range = 3;

        [Tooltip("Time in frames that passes before entity attacks (Animation may occur during this period)"), SerializeField] 
        private int windup = 1000;

        [Tooltip("Time in frames that the attack will last"), SerializeField] 
        private int length = 500;
        
        [Tooltip("Time in frames that passes before entity can attack again (Animation may occur during this period)"), SerializeField] 
        private int cooldown = 1000;

        public Attack(){ damage = 1;}

        /// See <see cref="range">Range</see>
        public int GetRange() { return range; }
        
        /// See <see cref="windup">Windup</see>
        public int GetWindup() { return windup; }
        
        /// See <see cref="cooldown">Cooldown</see>
        public int GetCooldown() { return cooldown; }
    }
}

using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for managing the attack qualities of an entity
    [HideMonoScript]
    public class Attack : EntityComponent
    {
        // HEADER: FIELDS (and inspector modifiers)
        
        protected override string GetMainValue(){return "Damage";}
        protected override string GetMainTooltip(){return "The amount of health an attack from this entity will take";}
        
        private int damage {
            get => val;
            set => val = value; }
        
        [Tooltip("The distance the attack can reach"), SerializeField] 
        private int range = 1;

        [Tooltip("Time in frames that passes before entity attacks (Animation may occur during this period)"), SerializeField] 
        private int windup = 500;

        [Tooltip("Time in frames that the attack will last"), SerializeField] 
        private int length = 500;
        
        [Tooltip("Time in frames that passes before entity can attack again (Animation may occur during this period)"), SerializeField] 
        private int cooldown = 500;
        
        
        // HEADER: CONSTRUCTOR
        
        public Attack(){ damage = 1;}

        
        // HEADER: GETTERS AND SETTERS
        
        /// See <see cref="range">Range</see>
        public int GetRange() { return range; }
        
        /// See <see cref="windup">Windup</see>
        public int GetWindup() { return windup; }
        
        /// See <see cref="cooldown">Cooldown</see>
        public int GetCooldown() { return cooldown; }
        
        public int GetLength() { return length; }
        
        public void SetRange(int value) { range = value; }
        
        public void SetWindup(int value) { windup = value; }
        
        public void SetCooldown(int value) { cooldown = value; }
        
        public void SetLength(int value) { length = value; }
    }
}

using System;
using NPCs;
using TriInspector;
using UnityEngine;

namespace Components
{
    /// Abstract class for defining components used by the player and NPCs
    public abstract class EntityComponent : MonoBehaviour
    {
        // HEADER: FIELDS (and inspector modifiers)
        
        protected abstract string GetMainValue(); protected string mainValue => GetMainValue();
        protected abstract string GetMainTooltip(); protected string mainTooltip => GetMainTooltip();
        
        [PropertyTooltip("$mainTooltip"),
         SerializeField, LabelText("$mainValue")] protected int val;

        
        // HEADER: GETTERS / SETTERS
        
        /// Return the base value of this component
        public int GetValue(){return val;}
    
        /// Set the base value of this component
        public void SetValue(int new_val){val = new_val;}
        
        
        // HEADER: COMPONENTS

        protected Vector3 position => !npc ? transform.position : npc.position;
        
        protected NPC npc;

        protected void Awake() { npc = GetComponent<NPC>(); }
    }
}

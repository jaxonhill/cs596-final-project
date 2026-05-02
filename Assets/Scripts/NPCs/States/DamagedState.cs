using Cysharp.Threading.Tasks;

namespace NPCs.States
{
    public class DamagedState : NPCState
    {
        
        /* * * * * * * * * *
         * NPC Components  *
         * * * * * * * * * */
        private readonly NPC npc;
        
        public DamagedState(NPC new_npc) { npc = new_npc; }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Enter()
        {
            if (npc.health.GetValue() <= 0)
            {
                npc.ChangeToState(NPCStateEnum.Death);
            }
            npc.movement.SetValue(0);
            if (npc.GetTarget() != null)
            {
                npc.ChangeToState(NPCStateEnum.Chasing);
                return;
            }
            npc.ChangeToState(NPCStateEnum.Idle);
        }

        public override UniTask Run() { return UniTask.CompletedTask; }

        public override void Exit()
        { }
    }
}

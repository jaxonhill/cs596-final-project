using NPCs.States.Attack;
using TriInspector;

namespace NPCs.Enemies
{
    /// <summary> Class used to define and instantiate Melee Enemies </summary>
    [HideMonoScript]
    public class MeleeEnemy : BaseEnemy
    {
        public MeleeEnemy()
        {
            attackState = new MeleeAttackState(this);
        }
    }
}

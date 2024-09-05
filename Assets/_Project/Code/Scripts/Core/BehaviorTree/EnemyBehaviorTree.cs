using System.Collections.Generic;

namespace PSGJ15_DCSA.Core.BehaviorTree
{
    public class EnemyBehaviorTree : BehaviorTree
    {
        private Enemy m_enemy;
        public EnemyBehaviorTree(Enemy enemy)
        {
            m_enemy = enemy;
        }

        protected override Node SetupTree()
        {
            //todo if we ever use this version of behavior tree
            return null;
        }

        public override void Tick()
        {
            base.Tick();
        }
    }
}
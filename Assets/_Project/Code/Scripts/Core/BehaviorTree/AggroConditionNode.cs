namespace PSGJ15_DCSA.Core
{
    public class AggroConditionNode : ConditionNode
    {
        private Enemy enemy;

        public AggroConditionNode(Enemy enemy)
        {
            this.enemy = enemy;
        }

        protected override bool CheckCondition()
        {
            return false;
        }
    }
}
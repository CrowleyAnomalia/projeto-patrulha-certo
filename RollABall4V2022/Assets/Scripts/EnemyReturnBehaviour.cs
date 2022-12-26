using UnityEngine;

namespace UnityTemplateProjects.AI.Enemy
{
    public class EnemyReturnBehaviour : StateMachineBehaviour
    {
        private EnemyController _myEnemyController;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            _myEnemyController = animator.gameObject.GetComponent<EnemyController>();
            _myEnemyController.SetSphereRadius(_myEnemyController.FollowDistance);
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            _myEnemyController.SetDestinationToPatrol();
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }
    }
}
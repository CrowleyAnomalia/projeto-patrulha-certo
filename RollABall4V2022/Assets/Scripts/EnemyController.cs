using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyDataSO enemyData;

    public PatrolRouteManager myPatrolRoute;
    
    private float _moveSpeed;

    private int _maxHealthPoints;

    private GameObject _enemyMesh;

    public float FollowDistance => _followDistance;
    public float _followDistance; //x = distancia minima pro inimigo seguir o jogador

    public float ReturnDistance => _returnDistance;
    public float _returnDistance; //z = distancia maxima pro inimigo seguir o jogador

    public float AttackDistance => _attackDistance;
    public float _attackDistance; //y = distancia minima pro inimigo atacar o jogador

    public float GiveUpDistance => _giveUpDistance;
    public float _giveUpDistance; //w = distancia maxima pro inimigo atacar o jogador

    private int _currentHealthPoints;

    private float _currentMoveSpeed;

    private Animator _enemyFSM;

    private NavMeshAgent _navMeshAgent;

    private SphereCollider _sphereCollider;

    private Transform _playerTransform;

    private Transform _currentPatrolPoint;

    private Transform _nextPatrolPoint;

    private int _currentPatrolIndex;
    

    private void Awake()
    {
        _moveSpeed = enemyData.moveSpeed;
        _maxHealthPoints = enemyData.maxHealthPoints;

        //_enemyMesh = Instantiate(enemyData.enemyMesh, transform);

        _followDistance = enemyData.followDistance;
        _returnDistance = enemyData.returnDistance;
        _attackDistance = enemyData.attackDistance;
        _giveUpDistance = enemyData.giveUpDistance;

        _currentHealthPoints = _maxHealthPoints;
        _currentMoveSpeed = _moveSpeed;

        _enemyFSM = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Questao 1
        _currentPatrolIndex = 0;

        _currentPatrolPoint = myPatrolRoute.patrolRoutePoints[_currentPatrolIndex];
    }

    public void Patrulhar()
    {
        if(myPatrolRoute.patrolRoutePoints.Count > 0)
        {
            // muda o destino do objeto navMashAgent para a posi��o guardada na lista de transforms de acordo com cada indice
            // Questao 4
            _navMeshAgent.destination = myPatrolRoute.patrolRoutePoints[_currentPatrolIndex].position;
            // Atualiza o indice para pegar a proxima posi��o da lista
            // Quetao 3
            _currentPatrolIndex++;

            // Verifica se a lista chegou ao fim atravez do indice atual e o tamanho da lista 
            if(_currentPatrolIndex == myPatrolRoute.patrolRoutePoints.Count)
            {
                // Reinicia a lista atraves do indice
                _currentPatrolIndex = 0;
            }
        }
        else
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyFSM.GetCurrentAnimatorStateInfo(0).IsName("Return"))
        {
            _enemyFSM.SetFloat("ReturnDistance",
                Vector3.Distance(transform.position, _currentPatrolPoint.position));
        }
        // verifica a distancia entre o Inimigo e o ponto de patrulha, de for menor que 0.1 a funcao Patrulhar � chamada  
        // Questao 2
        if (_enemyFSM.GetCurrentAnimatorStateInfo(0).IsName("Patrol"))
        {
            if (_navMeshAgent.remainingDistance < 0.1f)
            {
                Patrulhar();
            }
        }
    }

    public void SetSphereRadius(float value)
    {
        _sphereCollider.radius = value;
    }

    public void SetDestinationToPlayer()
    {
        transform.position += (_playerTransform.position - transform.position).normalized * _moveSpeed * Time.deltaTime;
        
        _navMeshAgent.SetDestination(_playerTransform.position);
    }

    public void SetDestinationToPatrol()
    {
        _navMeshAgent.SetDestination(_currentPatrolPoint.position);
        _currentPatrolIndex = 0;
    }

    public void ResetPlayerTransform()
    {
        _playerTransform = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = other.transform;
            _enemyFSM.SetTrigger("OnPlayerEntered");
            Debug.Log("Jogador entrou na area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemyFSM.SetTrigger("OnPlayerExited");
            Debug.Log("Jogador saiu da area");
        }
    }
}

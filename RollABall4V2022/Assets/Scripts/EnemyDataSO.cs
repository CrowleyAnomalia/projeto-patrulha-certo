using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Assets/Enemy", order = 0)]
public class EnemyDataSO : ScriptableObject
{
    public float moveSpeed;

    public int maxHealthPoints;

    public GameObject enemyMesh;

    public float followDistance; //x = distancia minima pro inimigo seguir o jogador

    public float returnDistance; //z = distancia maxima pro inimigo seguir o jogador

    public float attackDistance; //y = distancia minima pro inimigo atacar o jogador

    public float giveUpDistance; //w = distancia maxima pro inimigo atacar o jogador
}

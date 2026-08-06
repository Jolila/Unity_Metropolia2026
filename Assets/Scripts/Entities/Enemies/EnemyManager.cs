using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyPoolManager;

public class EnemyManager : MonoBehaviour
{



    Vector3 cachedPlayerPosition;
    [SerializeField] float retargetDistance = 1.5f;
    [SerializeField] EnemyPoolManager _poolManager;
    [SerializeField] EnemySpawner _spawner;

    [System.Serializable]
    public struct UpdatePass
    {
        public int ratLeaders;
        public int slimeLeaders;
        public int zombieLeaders;

        public int followers;

        public int bats;
        public int ghosts;
    }

    [SerializeField]
    private List<UpdatePass> updateSchedule = new();
    private int currentPass;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { }


    public void OnStartGame()
    {
        cachedPlayerPosition = GameManager.Instance.GetPlayerReference().transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Countdown) _spawner.SpawnCountDownEnemies();
        
        if (GameManager.Instance.GetState() != GameState.Playing) return;
        Vector3 currentPlayerPos = GameManager.Instance.GetPlayerReference().transform.position;
        if (
        (currentPlayerPos - cachedPlayerPosition).sqrMagnitude >
        retargetDistance * retargetDistance)
        {
            cachedPlayerPosition = currentPlayerPos;
        }

        UpdatePass pass = updateSchedule[currentPass];
        UpdateSingularPass(pass);

        currentPass++;

        if (currentPass >= updateSchedule.Count)
            currentPass = 0;

    }



    void UpdateSingularPass(UpdatePass pass)
    {

        Pool pool = EnemyPoolManager.Instance.GetPool(pass.pool);
        int end = Mathf.Min(pass.startIndex + pass.count,
                          pool.objects.Count);


        for (int i = pass.startIndex; i < end; i++)
        {
            GameObject obj = pool.objects[i];

            if (!obj.activeInHierarchy)
                continue;

            IEnemyAI enemy = obj.GetComponent<IEnemyAI>();

            if (enemy == null) continue;

            enemy.Tick(cachedPlayerPosition);

        }

    }





    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(cachedPlayerPosition, retargetDistance);



    }


}

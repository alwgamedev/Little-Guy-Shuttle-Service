using UnityEngine;

namespace LGShuttle.Core
{
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField] GameObject[] toSpawn;

        bool thisIsSpawner;

        static bool hasSpawned;

        private void Awake()
        {
            if (!hasSpawned)
            {
                foreach (var g in toSpawn)
                {
                    Instantiate(g, transform);
                }

                DontDestroyOnLoad(gameObject);
                hasSpawned = true;
                thisIsSpawner = true;
            }
            else if (!thisIsSpawner)
            {
                Destroy(gameObject);
            }
        }
    }
}
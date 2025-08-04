using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LittleGuySpawner : MonoBehaviour
    {
        [SerializeField] LittleGuyController lgPrefab;
        [SerializeField] RandomizableFloat spawnHeightFactor;

        public LittleGuyController[] Spawn(int quantityToSpawn)
        {
            var spawned = new LittleGuyController[quantityToSpawn];

            for (int i = 0; i < quantityToSpawn; i++)
            {
                var p = SkateboardMover.RandomBoardAnchorPosition.LerpValue;
                var lg = Instantiate(lgPrefab);
                lg.transform.position = p + spawnHeightFactor.Value * lg.Mover.Height * Vector2.up;
                lg.SetSortingOrder(i);
                spawned[i] = lg;
            }

            return spawned;
        }
    }
}
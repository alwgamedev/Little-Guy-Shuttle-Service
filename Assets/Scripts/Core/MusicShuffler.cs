using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LGShuttle.Core
{
    public class MusicShuffler : MonoBehaviour
    {
        [SerializeField] AudioClip[] clips;

        List<int> remainingIndices;
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                if (remainingIndices == null || remainingIndices.Count == 0)
                {
                    BeginShuffle();
                }

                int i = MiscTools.rng.Next(remainingIndices.Count());
                audioSource.PlayOneShot(clips[remainingIndices[i]]);
                remainingIndices.RemoveAt(i);
            }
        }

        public void BeginShuffle()
        {
            remainingIndices = Enumerable.Range(0, clips.Length).ToList();
        }
    }
}
using System.Collections.Generic;
using System;
using UnityEngine;

namespace LGShuttle.Core
{
    [Serializable]
    public struct RandomizableColor
    {
        [SerializeField] RandomizableMode mode;
        [SerializeField] Color min;
        [SerializeField] Color max;

        public Color Value
        {
            get
            {
                if (mode == RandomizableMode.Max)
                {
                    return max;
                }

                return MiscTools.RandomColor(min, max);
            }
        }

        public Color Min => min;

        public Color Max => max;

        public static Color RandomColor(float minSaturation, float alpha)
        {
            var x = MiscTools.RandomFloat(0, 1 - minSaturation);
            var y = MiscTools.RandomFloat(x, 1);
            var z = MiscTools.RandomFloat(Mathf.Max(y, x + minSaturation), 1);

            List<int> indices = new() { 0, 1, 2 };
            float[] rgb = new float[] { 0, 0, 0 };

            var i = MiscTools.rng.Next(indices.Count);
            rgb[indices[i]] = x;
            indices.RemoveAt(i);
            i = MiscTools.rng.Next(indices.Count);
            rgb[indices[i]] = y;
            indices.RemoveAt(i);
            rgb[indices[0]] = z;

            return new Color(rgb[0], rgb[1], rgb[2], alpha);
        }
    }
}
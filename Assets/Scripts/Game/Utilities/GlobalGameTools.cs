using UnityEngine;

namespace LGShuttle.Game
{
    public static class GlobalGameTools
    {
        static int? littleGuyLayer;
        static int? skateboardLayer;
        static int? groundLayer;

        public static int LittleGuyLayer
        {
            get
            {
                if (!littleGuyLayer.HasValue)
                {
                    littleGuyLayer = LayerMask.GetMask("Little Guys");
                }

                return littleGuyLayer.Value;
            }
        }
        public static int SkateboardLayer
        {
            get
            {
                if (!skateboardLayer.HasValue)
                {
                    skateboardLayer = LayerMask.GetMask("Skateboard");
                }

                return skateboardLayer.Value;
            }
        }
        public static int GroundLayer
        {
            get
            {
                if (!groundLayer.HasValue)
                {
                    groundLayer = LayerMask.GetMask("Ground");
                }

                return groundLayer.Value;
            }
        }
    }
}
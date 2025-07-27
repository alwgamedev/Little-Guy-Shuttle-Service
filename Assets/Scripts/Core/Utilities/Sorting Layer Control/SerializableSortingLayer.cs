using System;

namespace LGShuttle.Core
{
    [Serializable]
    public struct SerializableSortingLayer
    {
        public int layerNumber;
        //position in the array SortingLayer.layers
        //note that this may be different from layer id
    }
}
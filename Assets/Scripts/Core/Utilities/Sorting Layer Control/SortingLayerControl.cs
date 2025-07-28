using System;
using UnityEngine;

namespace LGShuttle.Core
{
    [ExecuteAlways]
    public class SortingLayerControl : MonoBehaviour, SortingLayerDataSource
    {
        [SerializeField] SerializableSortingLayer sortingLayer;
        [SerializeField] int sortingOrder;

        public event Action DataUpdated;
        public event Action Destroyed;

        public int? SortingLayerID
        {
            get
            {
                var layers = SortingLayer.layers;
                var layerNumber = Mathf.Clamp(sortingLayer.layerNumber, 0, layers.Length - 1);
                return layers[layerNumber].id;

            }
        }
        public int? SortingOrder
        {
            get => sortingOrder;
            set
            {
                if (value.HasValue)
                {
                    sortingOrder = value.Value;
                    UpdateSortingData();
                }

            }
        }

        private void OnValidate()
        {
            UpdateSortingData();
        }

        public void UpdateSortingData()
        {
            DataUpdated?.Invoke();
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke();

            DataUpdated = null;
            Destroyed = null;
        }
    }
}
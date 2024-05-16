using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Barmetler.RoadSystem
{
    public class RoadTrafficDensity : MonoBehaviour
    {
        public int NumberOfCars = 0;
        public float RoadLength = 0;
        public float TrafficDensity = 0;
        public bool HighTraffic = false;
        public bool ShowIndicator = false;
        float divisor = 0;
        void Start()
        {
            UpdateRoadLength();
            divisor = Tag.CompareTags(transform, Tag.Road_Large) ? 4 : 2;
        }

        void Update()
        {
            TrafficDensityCalculationAndIndication();
        }

        private void TrafficDensityCalculationAndIndication()
        {
            TrafficDensity = NumberOfCars / RoadLength / divisor;
            if (ShowIndicator)
            {
                if (TrafficDensity >= 0.05f)
                {
                    HighTraffic = true;
                    transform.GetComponent<Outline>().OutlineColor = Color.red;
                    transform.GetComponent<Outline>().enabled = true;
                }
                else if (TrafficDensity < 0.1f && HighTraffic)
                {
                    HighTraffic = false;
                    transform.GetComponent<Outline>().OutlineColor = Color.white;
                    transform.GetComponent<Outline>().needsUpdate = true;
                    transform.GetComponent<Outline>().enabled = false;
                }
            }
            else
            {
                transform.GetComponent<Outline>().OutlineColor = Color.white;
                transform.GetComponent<Outline>().needsUpdate = true;
                transform.GetComponent<Outline>().enabled = false;
            }

        }

        private void UpdateRoadLength()
        {
            RoadLength = gameObject.GetComponent<Road>().GetLength();
        }

        public void IncreaseCount()
        {
            NumberOfCars++;
        }

        public void DecreaseCount()
        {
            NumberOfCars--;
        }
    }
}


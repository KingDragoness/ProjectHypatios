using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace TestingPurposes
{

    public class ExperimentalAsync : MonoBehaviour
    {

        public struct SimpleJob : IJobParallelFor
        {

            public NativeArray<SimpleData> ResultArray;

            public void Execute(int index)
            {
                var data = ResultArray[index];
                data.Calculate();
                ResultArray[index] = data;
            }
        }

        public struct SimpleData
        {
            public float a;
            public float b;

            public float Result { get; private set; }

            public SimpleData(float a, float b) : this()
            {
                this.a = a;
                this.b = b;
            }

            public void Calculate()
            {
                Result = a + b;
            }

        }

        public Transform _origin;
        public Color color;
        private NativeArray<RaycastCommand> _raycastCommands;
        private NativeArray<RaycastHit> _raycastHits;
        private JobHandle _jobHandle;
        private int count = 1000;
        private NativeArray<SimpleData> SimpleDataArray;
        private SimpleJob simpleJob;

        void Awake()
        {
            _raycastCommands = new NativeArray<RaycastCommand>(1, Allocator.Persistent);
            _raycastHits = new NativeArray<RaycastHit>(1, Allocator.Persistent);
            SimpleDataArray = new NativeArray<SimpleData>(count, Allocator.Persistent);

            for (int i = 0; i < count; i++)
            {
                SimpleDataArray[i] = new SimpleData(Random.Range(1,count), Random.Range(1, count));
            }
            simpleJob = new SimpleJob { ResultArray = SimpleDataArray, };

        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            _jobHandle.Complete();
            _raycastCommands.Dispose();
            _raycastHits.Dispose();
            SimpleDataArray.Dispose();
        }

        private float nextTimer = 0;

        private void Update()
        {
            RunRaycast();
            RunCalculations();
        }

        private void RunCalculations()
        {

            JobHandle jobHandle = simpleJob.Schedule(count, 1);
      
            jobHandle.Complete();
            for (int x = 0; x < count; x++)
            {
                //Debug.Log(simpleJob.ResultArray[x].Result);
            }
        }

        void RunRaycast()
        {
            if (Time.time > nextTimer)
            {
                nextTimer = Time.time  + 1f;
            }
            else
            {
                return;
            }

            // 1. Process raycast from last frame
            _jobHandle.Complete();
            RaycastHit raycastHit = _raycastHits[0];
            bool didHitYa = raycastHit.collider != null;
            Color targetColor = Color.white;
            if (didHitYa)
            {
                if (raycastHit.collider.CompareTag("Player"))
                {
                    targetColor = Color.green;
                }
                else if (raycastHit.collider.CompareTag("Enemy"))
                {
                    targetColor = Color.red;
                }
                Debug.DrawLine(_origin.position, raycastHit.point, targetColor);

            }

            color = targetColor;

            // 2. Schedule new raycast
            _raycastCommands[0] = new RaycastCommand(_origin.position, _origin.forward);
            _jobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 1);
        }

    }
}
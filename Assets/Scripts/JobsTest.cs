using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

namespace CrazyGames
{
    public struct TestJob : IJobFor
    {
        [ReadOnly] private NativeArray<long> _input;
        [WriteOnly] private NativeArray<long> _output;
        private long result;

        public TestJob(NativeArray<long> input, NativeArray<long> output)
        {
            _input = input;
            _output = output;
            result = 0;
        }

        public void Execute(int i)
        {
            result += _input[i] * 2 - 124526;
            _output[0] = result + 123;
        }
    }

    public class JobsTest : MonoBehaviour
    {
        private NativeArray<long> _input;
        private NativeArray<long> _output;
        private JobHandle _testJob;

        private long start1 = 0;
        private long end1 = 0;
        private long start2 = 0;
        private long end2 = 0;

        private void Start()
        {
            Invoke(nameof(StartTest), 5);
        }

        private void StartTest()
        {
            start1 = DateTime.Now.Ticks;
            Debug.Log("NoJob prepare started " + start1);

            long[] nums = new long[33554431];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = i;
            }

            end1 = DateTime.Now.Ticks;
            Debug.Log("NoJob prepare ended " + end1 + " : " + (end1 - start1));

            start1 = DateTime.Now.Ticks;
            Debug.Log("NoJob started " + start1);

            long result = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                result += nums[i] * 2 - 124526;
                result += 123;
            }

            end1 = DateTime.Now.Ticks;
            Debug.Log("NoJob ended " + end1 + " : " + (end1 - start1));

            Debug.Log(result);

            start2 = DateTime.Now.Ticks;
            Debug.Log("Job prepare started " + start2);

            _input = new NativeArray<long>(33554431, Allocator.Persistent);
            _output = new NativeArray<long>(1, Allocator.Persistent);

            for (int i = 0; i < _input.Length; i++)
            {
                _input[i] = i;
            }

            end2 = DateTime.Now.Ticks;
            Debug.Log("Job prepare ended " + end2 + " : " + (end2 - start2));

            var job = new TestJob(_input, _output);

            start2 = DateTime.Now.Ticks;
            Debug.Log("Job started " + start2);

            _testJob = job.Schedule(_input.Length, default);

            StartCoroutine(WaitJobEnding());
        }

        private IEnumerator WaitJobEnding()
        {
            _testJob.Complete();

            while (!_testJob.IsCompleted)
            {
                yield return new WaitForEndOfFrame();
            }

            end2 = DateTime.Now.Ticks;
            Debug.Log("Job ended " + end2 + " : " + (end2 - start2));

            Debug.Log((end1 - start1) + " : " + (end2 - start2));

            Debug.Log(_output[0]);

            _input.Dispose();
            _output.Dispose();
        }

        private void Update()
        {
            
        }
    }
}

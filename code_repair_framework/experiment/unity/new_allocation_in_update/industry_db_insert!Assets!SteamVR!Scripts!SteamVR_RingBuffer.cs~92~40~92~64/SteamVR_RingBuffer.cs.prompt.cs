//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;

namespace Valve.VR
{
    public class SteamVR_RingBuffer<T>
    {
        protected T[] buffer;
        protected int currentIndex;
        protected T lastElement;

        public SteamVR_RingBuffer(int size)
        {
            buffer = new T[size];
            currentIndex = 0;
        }

        public void Add(T newElement)
        {
            buffer[currentIndex] = newElement;

            StepForward();
        }

        public virtual void StepForward()
        {
            lastElement = buffer[currentIndex];

            currentIndex++;
            if (currentIndex >= buffer.Length)
                currentIndex = 0;

            cleared = false;
        }

        public virtual T GetAtIndex(int atIndex)
        {
            if (atIndex < 0)
                atIndex += buffer.Length;

            return buffer[atIndex];
        }

        public virtual T GetLast()
        {
            return lastElement;
        }

        public virtual int GetLastIndex()
        {
            int lastIndex = currentIndex - 1;
            if (lastIndex < 0)
                lastIndex += buffer.Length;

            return lastIndex;
        }

        private bool cleared = false;
        public void Clear()
        {
            if (cleared == true)
                return;

            if (buffer == null)
                return;

            for (int index = 0; index < buffer.Length; index++)
            {
                buffer[index] = default(T);
            }

            lastElement = default(T);

            currentIndex = 0;

            cleared = true;
        }
    }

    public class SteamVR_HistoryBuffer : SteamVR_RingBuffer<SteamVR_HistoryStep>
    {
        public SteamVR_HistoryBuffer(int size) : base(size)
        {

        }

        public void Update(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
                // BUG: Using New() allocation in Update() method.
                // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                //             if (buffer[currentIndex] == null)
                //                 buffer[currentIndex] = new SteamVR_HistoryStep();
                // 
                //             buffer[currentIndex].position = position;
                //             buffer[currentIndex].rotation = rotation;
                //             buffer[currentIndex].velocity = velocity;
                //             buffer[currentIndex].angularVelocity = angularVelocity;
                //             buffer[currentIndex].timeInTicks = System.DateTime.Now.Ticks;
                // 
                //             StepForward();
                //         }

                // FIXED VERSION:


        public float GetVelocityMagnitudeTrend(int toIndex = -1, int fromIndex = -1)
        {
            if (toIndex == -1)
                toIndex = currentIndex - 1;

            if (toIndex < 0)
                toIndex += buffer.Length;

            if (fromIndex == -1)
                fromIndex = toIndex - 1;

            if (fromIndex < 0)
                fromIndex += buffer.Length;

            SteamVR_HistoryStep toStep = buffer[toIndex];
            SteamVR_HistoryStep fromStep = buffer[fromIndex];

            if (IsValid(toStep) && IsValid(fromStep))
            {
                return toStep.velocity.sqrMagnitude - fromStep.velocity.sqrMagnitude;
            }

            return 0;
        }

        public bool IsValid(SteamVR_HistoryStep step)
        {
            return step != null && step.timeInTicks != -1;
        }

        public int GetTopVelocity(int forFrames, int addFrames = 0)
        {
            int topFrame = currentIndex;
            float topVelocitySqr = 0;

            int currentFrame = currentIndex;

            while (forFrames > 0)
            {
                forFrames--;
                currentFrame--;

                if (currentFrame < 0)
                    currentFrame = buffer.Length - 1;

                SteamVR_HistoryStep currentStep = buffer[currentFrame];

                if (IsValid(currentStep) == false)
                    break;

                float currentSqr = buffer[currentFrame].velocity.sqrMagnitude;
                if (currentSqr > topVelocitySqr)
                {
                    topFrame = currentFrame;
                    topVelocitySqr = currentSqr;
                }
            }

            topFrame += addFrames;

            if (topFrame >= buffer.Length)
                topFrame -= buffer.Length;

            return topFrame;
        }

        public void GetAverageVelocities(out Vector3 velocity, out Vector3 angularVelocity, int forFrames, int startFrame = -1)
        {
            velocity = Vector3.zero;
            angularVelocity = Vector3.zero;

            if (startFrame == -1)
                startFrame = currentIndex - 1;

            if (startFrame < 0)
                startFrame = buffer.Length - 1;

            int endFrame = startFrame - forFrames;

            if (endFrame < 0)
                endFrame += buffer.Length;

            Vector3 totalVelocity = Vector3.zero;
            Vector3 totalAngularVelocity = Vector3.zero;
            float totalFrames = 0;
            int currentFrame = startFrame;
            while (forFrames > 0)
            {
                forFrames--;
                currentFrame--;

                if (currentFrame < 0)
                    currentFrame = buffer.Length - 1;

                SteamVR_HistoryStep currentStep = buffer[currentFrame];

                if (IsValid(currentStep) == false)
                    break;

                totalFrames++;

                totalVelocity += currentStep.velocity;
                totalAngularVelocity += currentStep.angularVelocity;
            }

            velocity = totalVelocity / totalFrames;
            angularVelocity = totalAngularVelocity / totalFrames;
        }
    }

    public class SteamVR_HistoryStep
    {
        public Vector3 position;
        public Quaternion rotation;

        public Vector3 velocity;

        public Vector3 angularVelocity;

        public long timeInTicks = -1;
    }
}
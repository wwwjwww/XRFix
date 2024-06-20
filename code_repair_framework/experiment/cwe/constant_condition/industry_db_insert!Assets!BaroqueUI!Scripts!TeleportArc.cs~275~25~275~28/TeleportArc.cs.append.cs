

        //-------------------------------------------------
        public Vector3 GetArcPositionAtTime(float time)
        {
            Vector3 gravity = useGravity ? Physics.gravity : Vector3.zero;

            Vector3 arcPos = startPos + ((projectileVelocity * time) + (0.5f * time * time) * gravity);
            return arcPos;
        }


        //-------------------------------------------------
        private void HideLineSegments(int startSegment, int endSegment)
        {
            if (lineRenderers != null)
            {
                for (int i = startSegment; i < endSegment; ++i)
                {
                    lineRenderers[i].enabled = false;
                }
            }
        }
    }
}

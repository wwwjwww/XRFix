    public void NotifyNearbyBricksToRecalculateMesh()
    {
        BrickMeshRecalculator meshRecalculator = BrickMeshRecalculator.GetInstance();
        List<BrickAttach> objectsToRecalculate = new List<BrickAttach>();
        foreach (GameObject connectorObject in femaleConnectors.Concat(maleConnectors))
        {
            int hits = Physics.OverlapSphereNonAlloc(connectorObject.transform.position, 0.02f, _colliderBuffer);
            for (int i = 0; i < hits; i++)
            {
                BrickAttach attach = _colliderBuffer[i].GetComponentInParent<BrickAttach>();
                if (((object)attach) == null)
                    continue;

                if (!objectsToRecalculate.Contains(attach))
                {
                    objectsToRecalculate.Add(attach);
                    meshRecalculator.AddAttach(attach);
                }
            }
        }
    }


}

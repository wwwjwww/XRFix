    public bool CollidesWithBricks(Quaternion rot, Vector3 pos)
    {
        int mask = LayerMask.GetMask("lego", "placed lego");

        // For some reason, the math here is all 180 degrees off. So rotate the object 180 degrees around it's UP axis
        _transform.RotateAround(_transform.position, _transform.up, 180f);

        foreach (BoxCollider c in colliders)
        {
            if (Physics.CheckBox(c.transform.TransformPoint(c.center),
                Vector3.Scale(c.size / 2, c.transform.lossyScale), c.transform.rotation, mask,
                QueryTriggerInteraction.Ignore))
                return true;
        }

        return false;
    }

    private GameObject[] GetFemaleConnectorsWithConnections()
    {
        return _femaleConnectors.Where(c => ClosestConnectorFromConnector(c)).ToArray();
    }

    private GameObject[] GetMaleConnectorsWithConnections()
    {

        return _maleConnectors.Where(c => ClosestConnectorFromConnector(c)).ToArray();
    }

    // Checks if there is a valid way to orient the connections
    private GameObject[] ValidConnections(GameObject[] connectors)
    {
        if (connectors.Length == 0)
        {
            return new GameObject[]{};
        }

        if (connectors.Length == 1)
        {
            return connectors;
        }

        Dictionary<GameObject, (float, GameObject)> partnerConnectorDistanceMap = new Dictionary<GameObject, (float, GameObject)>();
        // First, filter out femaleConnectors that are contending for the same male connector
        foreach (GameObject connector in connectors)
        {
            GameObject partnerConnector = ClosestConnectorFromConnector(connector);
            float distance = DistanceBetweenConnectors(connector, partnerConnector);

            // If the female connector is not in the map yet, it means no female before has contested for this male connector
            if (!partnerConnectorDistanceMap.ContainsKey(partnerConnector))
            {
                partnerConnectorDistanceMap[partnerConnector] = (distance, connector);
            }
            else
            {
                (float otherDistance, GameObject _) = partnerConnectorDistanceMap[partnerConnector];
                if (distance < otherDistance)
                {
                    partnerConnectorDistanceMap[partnerConnector] = (distance, connector);
                }
            }
        }

        List<GameObject> eligibleFemaleConnectors = new List<GameObject>();
        (float, GameObject)[] keys = partnerConnectorDistanceMap.Values.ToArray();
        foreach ((float distance, GameObject femaleConnector) in keys)
        {
            eligibleFemaleConnectors.Add(femaleConnector);
        }

        return eligibleFemaleConnectors.ToArray();
    }

    private Quaternion GetNewRot(GameObject otherBrick)
    {
        Quaternion rot = transform.rotation;
        Quaternion otherRot = otherBrick.transform.rotation;

        float identityDiff = Quaternion.Angle(otherRot, rot);

        Quaternion ninetyRot = otherRot * Quaternion.Euler(Vector3.up * 90);
        float ninetyDiff = Quaternion.Angle(ninetyRot, rot);

        Quaternion oneEightyRot = otherRot * Quaternion.Euler(Vector3.up * 180);
        float oneEightyDiff = Quaternion.Angle(oneEightyRot, rot);

        Quaternion twoSeventyRot = otherRot * Quaternion.Euler(Vector3.up * 270);
        float twoSeventyDiff = Quaternion.Angle(twoSeventyRot, rot);

        float maxDiff = Mathf.Max(identityDiff, ninetyDiff, oneEightyDiff, twoSeventyDiff);
        if (maxDiff == identityDiff)
        {
            return otherBrick.transform.rotation;
        }
        else if (maxDiff == ninetyDiff)
        {
            return ninetyRot;
        }
        else if (maxDiff == oneEightyDiff)
        {
            return oneEightyRot;
        }
        else if (maxDiff == twoSeventyDiff)
        {
            return twoSeventyRot;
        }
        else
        {
            Debug.Log("SOMETHING AWFUL HAS HAPPENED FUUUUCK");
            return Quaternion.identity;
        }
    }

    private Vector3 GetNewPosWithRot(Quaternion rot, GameObject otherBrick, GameObject[] femaleConnectors, bool connectingDownwards)
    {
        Vector3 otherBrickPos = otherBrick.transform.position;
        Quaternion oldRot = transform.rotation;
        _transform.rotation = rot;

        // TODO: Figure out where the heck these constants come from.
        // This stuff is so jank D:
        float heightDelta = connectingDownwards ? Height() - 0.060f : 0.0478f - Height();
        if (window && !connectingDownwards)
        {
            heightDelta += 0.001f;
        }

        Vector3 newPos = _transform.position;
        newPos.y = ClosestConnectorFromConnector(femaleConnectors[0]).transform.position.y + heightDelta;
        newPos.x = otherBrickPos.x
                   + (femaleConnectors[0].transform.position.x - newPos.x + (ClosestConnectorFromConnector(femaleConnectors[0]).transform.position.x - otherBrickPos.x));

        newPos.z = otherBrickPos.z
                   + (femaleConnectors[0].transform.position.z - newPos.z + (ClosestConnectorFromConnector(femaleConnectors[0]).transform.position.z - otherBrickPos.z));

        transform.rotation = oldRot;

        return newPos;
    }

    private static GameObject ClosestConnectorFromConnector(GameObject connector)
    {
        return connector.GetComponent<LegoConnectorScript>().ClosestConnector();
    }

    private static float DistanceBetweenConnectors(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static bool AngleApproximatelyZero(float a)
    {
        return Mathf.Abs(a % 360f) < 0.5f || Mathf.Abs(a % 360f) > 359.5f;
    }

    private float Height()
    {
        if (heightOverride != 0) return heightOverride;

        return window ? 0.113f : (tile ? 0.0565f : 0.078736f);
    }

    private bool IsVectorApproximatelyOne(Vector3 vec)
    {
        return ApproximatelyEqual(vec.x, 1f) && ApproximatelyEqual(vec.y, 1f) && ApproximatelyEqual(vec.z, 1f);
    }

    private bool ApproximatelyEqual(float value, float target)
    {
        return Mathf.Abs(value - target) < 0.005f;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        // Vector3 extents = model.GetComponent<MeshFilter>().sharedMesh.bounds.extents;

        // TODO: Instead of dividing by 3, we should subtract the height of the studs. This code won't work with plate pieces.
        // collisionExtents = Vector3.Scale(new Vector3(extents.x, extents.y / 4, extents.z), model.transform.localScale);

        _maleConnectors = new List<GameObject>();
        foreach (Transform child in maleConnectorParent.transform)
        {
            _maleConnectors.Add(child.gameObject);
        }

        _femaleConnectors = new List<GameObject>();
        foreach (Transform child in femaleConnectorParent.transform)
        {
            _femaleConnectors.Add(child.gameObject);
        }
    }
}

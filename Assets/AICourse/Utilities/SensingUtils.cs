using UnityEngine;
using System.Collections.Generic;

public class SensingUtils
{
    private static System.Random random;

    public static GameObject FindInstanceWithinRadius (GameObject me, string tag, float radius) {

		GameObject [] targets = GameObject.FindGameObjectsWithTag(tag);
		if (targets.Length==0) return null;

		float dist = 0;
		GameObject closest = targets[0];
		float minDistance = (closest.transform.position - me.transform.position).magnitude;

		for (int i=1; i<targets.Length; i++) {
			dist = (targets[i].transform.position - me.transform.position).magnitude;
			if (dist<minDistance) {
				minDistance = dist;
				closest = targets[i];
			}
		}
		if (minDistance<radius) return closest;
		else return null;
	}

    public static GameObject FindRandomInstanceWithinRadius(GameObject me, string tag, float radius)
    {

        if (random == null)
            random = new System.Random();

        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        if (targets.Length == 0) return null;

        List<GameObject> list = new List<GameObject>();
        foreach (GameObject gm in targets)
        {
            if (DistanceToTarget(me, gm) <= radius)
                list.Add(gm);
        }

        if (list.Count == 0)
            return null;
        else
            return list[random.Next(list.Count)];

    }

    public static float DistanceToTarget (GameObject me, GameObject target) {
		return (target.transform.position - me.transform.position).magnitude;
	}

}


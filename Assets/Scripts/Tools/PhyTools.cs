using System.Collections.Generic;
using System.Linq;
using GameManaging;
using UnityEngine;
using Vertx.Debugging;

public static class PhyTools
{
    
    /// Returns the transform hit by the Raycast
    private static Transform Raycast(Vector3 position, Vector3 direction, float distance, Color debugColor = default)
    {
        if (GameSettings.Debug) {  Debug.DrawRay(position, direction * distance, debugColor); }
        Physics.Raycast(position, direction, out var hit, distance);
        return hit.transform;
    }
    
    /// Returns true if the hit gameObject has any of the provided tags
    public static bool RaycastForTag(Vector3 position, Vector3 direction, float distance, List<string> tags, Color debugColor = default)
    {
        var hit = Raycast(position, direction, distance, debugColor);
        var transform = hit ? hit.transform : null;
        return transform && tags.Any(transform.CompareTag); // Returns true if the hit object contains any of the tags
    }

    public static bool RaycastForTransform(Vector3 position, Vector3 direction, float distance, Transform entity, Color debugColor = default) {
        var hit = Raycast(position, direction, distance, debugColor);
        return hit.transform && hit.transform == entity;
    }

    public static List<Transform> BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, Color debugColor = default)
    {
        var hits = new RaycastHit[3];
        if (GameSettings.Debug)
        {
            DrawPhysics.BoxCastNonAlloc(center, halfExtents, direction, hits, orientation, maxDistance);
        }
        else
        {
            Physics.BoxCastNonAlloc(center, halfExtents, direction, hits, orientation, maxDistance);
        }
        var transforms = hits.Select(hit => hit.transform).ToList();

        return transforms;
    }

    public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Color debugColor = default)
    {
        Collider[] hits;
        if (GameSettings.Debug)
        {
            hits = DrawPhysics.OverlapBox(center, halfExtents, new Quaternion(0,0,0,1));
        }
        else
        {
            hits = Physics.OverlapBox(center, halfExtents, new Quaternion(0,0,0,1));
        }

        return hits;
    }
}

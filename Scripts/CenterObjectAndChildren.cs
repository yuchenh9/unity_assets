using UnityEngine;

public class CenterObjectAndChildren : MonoBehaviour
{
    public void CenterAndFocus()
    {
        Vector3 center = GetCenter(transform);

        // Move the object to the center
        transform.position = center;

        // Make the main camera look at the centered object
        Camera.main.transform.LookAt(center);
    }

    Vector3 GetCenter(Transform trans)
    {
        // This will calculate the bounds of the parent and all its children
        Renderer[] renderers = trans.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0) return trans.position;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.center;
    }
}

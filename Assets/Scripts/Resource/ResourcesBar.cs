using System.Collections.Generic;
using UnityEngine;

public class ResourcesBar : MonoBehaviour
{
    [SerializeField] private List<ResourceUIView> resources = new List<ResourceUIView>();

    public void UpdateResource(Resource type, int value)
    {
        var view = resources.Find(resource => resource.Type == type);
        view.UpdateValue(value);
    }
}

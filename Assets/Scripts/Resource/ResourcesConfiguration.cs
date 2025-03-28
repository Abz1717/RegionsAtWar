using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resources Configuration", menuName = "Configs/Resources")]
public class ResourcesConfiguration : ScriptableObject
{
    public List<ResourceData> resources;
}

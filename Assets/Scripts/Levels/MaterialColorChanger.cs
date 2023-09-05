using UnityEngine;

public class MaterialColorChanger : MonoBehaviour
{
    public Color PreviousColor { get; private set; }
    public Material Material { get; private set; }

    public void ChangeMaterialColor(Material material, Color color)
    {
        Material = material;
        PreviousColor = new Color(material.color.r, material.color.g, material.color.b);
        material.color = color;
    }
}

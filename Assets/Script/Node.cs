using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isUsable = false;
    private GameObject Crush;

    public void InitializeNode(bool isUsable, GameObject crush)
    {
        this.isUsable = isUsable;
        this.Crush = crush;
    }

    public GameObject GetCrush() => Crush;
    public void SetCrush(GameObject crush) => Crush = crush;
}
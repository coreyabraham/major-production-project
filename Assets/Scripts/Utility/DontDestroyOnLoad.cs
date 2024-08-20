using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static Dictionary<string, GameObject> Instantiated = new();

    private void Awake()
    {
        if (Instantiated.ContainsKey(this.gameObject.name))
        {
            Destroy(this.gameObject); // "Object reference not set to an instance of an object"
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        Instantiated.Add(this.gameObject.name, this.gameObject);
    }
}

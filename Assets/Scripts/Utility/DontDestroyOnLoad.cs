using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static Dictionary<string, GameObject> Instantiated = new();

    private void Awake()
    {
        if (Instantiated.ContainsKey(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instantiated.Add(gameObject.name, gameObject);
    }
}

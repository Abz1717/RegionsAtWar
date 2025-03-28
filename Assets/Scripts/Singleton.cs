using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Attempt to find an existing instance in the scene
                instance = FindObjectOfType<T>();

                // If none exists, create a new one
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // If an instance already exists and it's not this one, destroy this duplicate.
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}

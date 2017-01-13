using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UndoTester : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            // Create a sphere primitive at origin
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // Move it so we can see when new spheres are created
            sphere.transform.position = Random.onUnitSphere;

            // Create an undo object. This is all we need to do to register
            // an object with the undo system.
            new UndoInstantiate(sphere);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            UndoManager.Instance.Undo();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            UndoManager.Instance.Redo();
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            // Get a random sphere we've spawned
            var spheres = FindObjectsOfType<SphereCollider>();

            if (spheres.Length == 0) return;
            
            var sphere = spheres[Random.Range(0, spheres.Length)].transform;

            var oldPos = sphere.transform.position;
            sphere.Translate(Random.onUnitSphere);
            var newPos = sphere.transform.position;

            new UndoTranslate(sphere, oldPos, newPos);
        }
    }
}
<div align="center" id="title"><strong>Unity Undo System Tutorial</strong></div>
<p>&nbsp;&emsp;&emsp;In this tutorial, we will implement a barebones but easily extensible undo system. You are assumed to understand the basics of <a href="https://unity3d.com/learn/tutorials/topics/scripting/inheritance?playlist=17117">OOP</a>, including method <a href="https://unity3d.com/learn/tutorials/topics/scripting/overriding?playlist=17117">overriding</a> and <a href="https://unity3d.com/learn/tutorials/topics/scripting/polymorphism?playlist=17117" target="_blank">polymorphism</a>. <br /><br /> &emsp;&emsp;The core of our undo system will consist of two classes: an undo manager, and an undo action base class. First, we create the <code>UndoManager</code> class and add the input methods we need, as well as a list for storing the undo actions.</p>
<br /><strong>UndoManager.cs</strong>

```csharp
using UnityEngine;
using System.Collections.Generic; // Required for using the List<T> type
 
public class UndoManager : MonoBehaviour
{
    // This list will hold all the undo actions in our history
    List<UndoAction> history = new List<UndoAction>();
    
    // Undoes an action in the history
    public void Undo() { }
    
    // Redoes an action in the history
    public void Redo() { }
    
    // Adds an action to the history
    public void AddAction(UndoAction action) { }
}
```

<p>&emsp;&emsp;Then we set up our basic undo object. Classes extending <code>UndoAction</code> will represent the various actions the user can perform.</p>
<br /><strong>UndoAction.cs</strong>
```csharp
public abstract class UndoAction
{
    public abstract void Undo();
    public abstract void Redo();
}
```

<p><br /> &emsp;&emsp;The <code>abstract</code> prefix of our methods means child classes <em>promise</em> to implement them, allowing the manager to call them on any class extending <code>UndoAction</code>. Making the class <code>abstract</code> means it cannot be used on its own and must first be extended into a child class. By extending this class and implementing the <code>Undo()</code> and <code>Redo()</code> methods, we can create any behaviors we want. These new behaviors will work seamlessly with our undo manager without additional code in the manager script.</p>
<p>&emsp;&emsp;Simple enough, right? Now we implement the <code>UndoManager</code> methods we specified. The manager will go up and down the history list, calling the proper methods on every <code>UndoAction</code> as it goes by.</p>
<br /><strong>UndoManager.cs</strong>
```csharp
using UnityEngine;
using System.Collections.Generic;

public class UndoManager : MonoBehaviour
{
    List<UndoAction> history = new List<UndoAction>();
    int currentIndex = -1;
    
    public void Undo()
    {
        // -1 means we are as far back in the history
        // as possible, so we can't go further.
        if (currentIndex < 0) return;
        
        // Undo the action and move down the history list
        history[currentIndex].Undo();
        currentIndex--;
    }
    
    public void Redo()
    {        
        // Avoid redoing past the last history item
        if (currentIndex >= history.Count - 1) return;

        // Move up the history list and redo the action.
        currentIndex++;
        history[currentIndex].Redo();
    }
    
    public void AddAction(UndoAction action)
    {
        // Add the action and set the index to the end of the history
        history.Add(action);
        currentIndex = history.Count - 1;
    }
}
```
<br /> &emsp;&emsp;Finally, we need a way for the <code>UndoActions</code> to find the <code>UndoManager</code>. To do this, we will add a static reference to the manager. For simplicity, we will use a stripped down <a href="http://wiki.unity3d.com/index.php/Singleton" target="_blank">Singleton</a> implementation.
<br /><strong>UndoManager.cs</strong>
```csharp
using UnityEngine;
using System.Collections.Generic;

public class UndoManager : MonoBehaviour
{
    // A stored instance of an UndoManager
    private static UndoManager _instance;
    // Accessing UndoManager.Instance will execute get{}, which returns an UndoManager.
    public static UndoManager Instance
    {        
        get
        {
            // If we do not have a stored instance, we find one in the scene and store it.
            if (_instance == null)
                _instance = FindObjectOfType<UndoManager>();
            // Return the instance we have stored.
            return _instance;
        }
    }
    
    List<UndoAction> history = new List<UndoAction>();
    int currentIndex = -1;
    
    public void Undo()
    {
        if (currentIndex < 0) return;
        
        // Undo the action and move down the history list
        history[currentIndex].Undo();
        currentIndex--;
    }
    
    public void Redo()
    {        
        if (currentIndex >= history.Count - 1) return;

        // Move up the history list and redo the action.
        currentIndex++;
        history[currentIndex].Redo();
    }
    
    public void AddAction(UndoAction action)
    {
        // Add the action and set the index to the end of the history
        history.Add(action);
        currentIndex = history.Count - 1;
    }
}
```
<br />
<p>&emsp;&emsp; We now have a working core of an undo system! Now we just need to implement a proper <code>UndoAction</code> to see it in action. <br /><br /></p>
<h1>Undo Instantiation</h1>
<p>&emsp;&emsp;Let&rsquo;s extend <code>UndoAction</code> to handle object instantiation, and make a simple script to test it out. <br /><br /> &emsp;&emsp;The new <code>UndoAction</code> will use its constructor to automatically register with the <code>UndoManager</code>. This will make creating and registering new <code>UndoAction</code> objects very simple. All you need to do is call <code>new UndoInstantiate(myGameObject);</code> and it will handle the rest. For simplicity, we will bind the actions to keyboard buttons.</p>
<br /><strong>UndoInstantiate.cs</strong>

```csharp
using UnityEngine;

// The new UndoInstantiate class extends UndoAction
public class UndoInstantiate : UndoAction {

    // Reference to the object we will instantiate
    GameObject instantiatedObject;

    // Take the object we instantiate and
    // add self to the undo manager.
    public UndoInstantiate(GameObject obj)
    {
        // We store a reference to the object we instantiated 
        // so that we can manipulate it later.
        instantiatedObject = obj;
        
        // Here is the static reference we created earlier.
        // The object uses it to add itself to the history via AddAction()
        UndoManager.Instance.AddAction(this);
    }

    // Instead of destroying the object, we simply hide it.
    // This will make redoing it much simpler.
    public override void Undo()
    {
        instantiatedObject.SetActive(false);
    }
    
    // Simply re-enable the object to "reinstantiate" it.
    public override void Redo()
    {
        instantiatedObject.SetActive(true);
    }
}
```

<p><br /><br /> &emsp;&emsp;And finally the test script.</p>
<br /><strong>UndoTester.cs</strong>
```csharp
using UnityEngine;

public class UndoTester : MonoBehaviour 
{
    void Update ()
    {
	if(Input.GetKeyDown(KeyCode.Space))
        {
            // Create a sphere primitive at origin
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // Move it so we can see when new spheres are created
            sphere.transform.position = Random.onUnitSphere * 10f;

            // Create an undo object. This is all we need to do to create
            // and register an UndoAction with the UndoManager.
            new UndoInstantiate(sphere);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            // Call the undo method on our manager using the static reference
            UndoManager.Instance.Undo();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Call the redo method on our manager using the static reference
            UndoManager.Instance.Redo();
        }
    }
}
```
<br />
<p><br /> &emsp;&emsp;Now place the UndoManager and UndoTester onto a gameobject, then use the space, Z, and Y keys to test out the system!</p>
<br /><br />
<div align="center">
<p><img src="https://68.media.tumblr.com/7ea656271c0ebc80215de605c7331a8e/tumblr_inline_oeqcux1ObP1t2g1uk_500.gif" /></p>
</div>
<br /><br /><br />
<h1>Ready For Use?</h1>
<p>&emsp;&emsp;The system may look like it works brilliantly as it is, but in reality there are a couple of big flaws in our system. <br /><br /> &emsp;&emsp;Firstly, the system&nbsp;stores history back indefinitely. This is not a good thing as performance will degrade over time, and the system will eventually run out of memory.</p>
<p>&emsp;&emsp;Secondly, when we create a new undo action while part way down the undo history, we do not clear the actions above it. This can cause a wide variety of implementation-dependent problems by breaking chronology, but we will not discuss that in detail.</p>
<p>&emsp;&emsp;To fix these issues, we will add a couple of extra things to the manager and undo action class. First, the UndoAction will get a <code>Cull()</code> method which will clean up when the undo action can no longer be redone. Not all cases require culling when being cleaned up, so we mark it as virtual, making implementation optional.</p>
<br /><strong>UndoAction.cs</strong>
```csharp
public abstract class UndoAction
{
    public abstract void Undo();
    public abstract void Redo();
    
    public virtual void Cull() { }
}
```
<p><br /><br /> &emsp;&emsp;Then we have our UndoInstantiate class implement <code>Cull()</code> and have it destroy the object properly. We only want to destroy the object when the action is destroyed while undone, so we add an <code>undone</code> boolean to keep track of that as well.</p>
<br /><strong>UndoInstantiate.cs</strong>
```csharp
using UnityEngine;

public class UndoInstantiate : UndoAction {

    // Reference to the stored object
    GameObject instantiatedObject;
    // Has our object been hidden?
    bool undone = false;

    public UndoInstantiate(GameObject obj)
    {
        instantiatedObject = obj;
        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        instantiatedObject.SetActive(true);
        // No longer hidden
        undone = false;
    }

    public override void Undo()
    {
        instantiatedObject.SetActive(false);
        // Object is hidden
        undone = true;
    }

    public override void Cull()
    {
        //  If our object has been hidden by the undo system,
        if (undone)
        {   
            // Destroy it properly
            Object.Destroy(instantiatedObject);
        }
    }
}
```
<p><br /><br />&emsp;&emsp;Now we update the <code>UndoManager</code> class to properly handle a limited-length history. We will also update the <code>AddAction()</code> method to clean up &ldquo;future&rdquo; actions so that we don&rsquo;t leave an alternate timeline in our history.</p>
<br /> &emsp;&emsp;First we add a serialized <code>maxHistoryLength</code> variable so that we can adjust the max length in the editor. Next, we add the <code>Trim()</code> and <code>Cull()</code> methods which will address the problems listed above. <br />
<strong>UndoManager.cs</strong>
```csharp
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    private static UndoManager _instance;
    public static UndoManager Instance
    {        
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UndoManager>();
            return _instance;
        }
    }

    [SerializeField]
    int maxHistoryLength = 5;

    public List<UndoAction> history = new List<UndoAction>();
    public int currentIndex = -1;

    public void Undo()
    {
        if (currentIndex < 0) return;

        // Undo the action and move down the history list
        history[currentIndex].Undo();
        currentIndex--;
    }

    public void Redo()
    {
        if (currentIndex >= history.Count - 1) return;

        // Move up the history list and redo the action.
        currentIndex++;
        history[currentIndex].Redo();
    }


    public void AddAction(UndoAction action)
    {
        // If we have undone part way back into the history, we
        // want to erase everything we have undone, creating
        // a new history thread starting at our current point.
        if (currentIndex != history.Count - 1)
        {
            Cull(currentIndex + 1, history.Count - currentIndex - 1);
        }

        history.Add(action);
        currentIndex = history.Count - 1;
        // Call Trim() to make sure our history does not exceed the max length
        Trim();
    }

    // Removes the oldest object in the history if the history
    // has reached its maximum length.
    // Called every time history is modified
    void Trim()
    {
        if (history.Count > maxHistoryLength)
        {
            Cull(0, 1);
            currentIndex--;
        }
    }

    // Culls a given range of the history
    void Cull(int start, int count)
    {
        // Call Cull() on each object we are about to remove
        for (int i = start; i < start + count; i++)
        {
            history[i].Cull();
        }
        // Then remove them from our history list
        history.RemoveRange(start, count);
    }
}
```
<p><br /><br />&emsp;&emsp;Now the system should delete old undo actions and clean up old timelines as a new one is created.</p>
<h1>Translation</h1>
<p>&emsp;&emsp;To make sure we understand how to use the system, let&rsquo;s implement another undo action. This time, we&rsquo;ll undo and redo the translation of an object. First, create a new class and name it <code>UndoTranslate</code>. <br /><br /> &emsp;&emsp;We will use the &ldquo;before&rdquo; and &ldquo;after&rdquo; positions of the object in this instance, but using the relative translation vector is also a valid option. This time, we do not need the <code>Cull()</code> method as nothing needs cleaning up.</p>
<br />
<strong>UndoTranslate.cs</strong>
```csharp
using UnityEngine;

public class UndoTranslate : UndoAction
{
    Transform target;
    Vector3 startPos;
    Vector3 endPos;

    public UndoTranslate(Transform target, Vector3 startPos, Vector3 endPos)
    {
        this.target = target;
        this.startPos = startPos;
        this.endPos = endPos;

        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        target.transform.position = endPos;
    }

    public override void Undo()
    {
        target.transform.position = startPos;
    }
}
```
<p><br /><br />&emsp;&emsp;Then we add it to our test script.</p>
<br /><strong>UndoTester.cs</strong>
```csharp
using UnityEngine;

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
            var spheres = FindObjectsOfType();

            // If no spheres exist, return to prevent errors
            if (spheres.Length == 0) return;
            
            // Get a random sphere
            var sphere = spheres[Random.Range(0, spheres.Length)].transform;

            // Store its positions before and after translating.
            var oldPos = sphere.transform.position;
            sphere.Translate(Random.onUnitSphere);
            var newPos = sphere.transform.position;

            // Create a new UndoTranslate object and pass the object,
	    // its position before translating, and its position after.
            new UndoTranslate(sphere, oldPos, newPos);
        }
    }
}
```
<p><br /><br />&emsp;&emsp;That&rsquo;s it! Now you can use the M button on your keyboard to randomly translate an object, and the usual Z/Y keys to undo/redo.</p>
<div align="center">
<p><img src="https://68.media.tumblr.com/77ade2d9b2fa3694b8355ee764eefbaf/tumblr_inline_oeqcuhxXD01t2g1uk_500.gif" /></p>
</div>
<p><br /><br /></p>
<h1>Bonus: Editor Extension</h1>
<p>&emsp;&emsp;This part is not strictly necessary, but can be very useful for debugging more advanced undo behaviors. To visualize the history, we can create a simple editor script to display our history list in the inspector. Create a new script named <code>UndoManagerEditor</code> and place it in a folder with the name Editor.</p>
<br /><strong>UndoManagerEditor.cs</strong>
```csharp
using UnityEditor;

[CustomEditor(typeof(UndoManager))]
public class UndoManagerEditor : Editor {
    
    UndoManager manager;
    bool toggle = false;

    void OnEnable()
    {
        manager = (UndoManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Add a toggle so that it won't clutter up our inspector
        toggle = EditorGUILayout.Foldout(toggle, "History View");
        if (toggle)
        {
            // Print every element in our history
            for (int i = 0; i &lt; manager.history.Count; i++)
            {
                var label = i + ": " + 
                    manager.history[i].GetType().ToString();
                if (i == manager.currentIndex)
                    label += "  ◀";
                EditorGUILayout.LabelField(label);
            }
        }
    }
}
```
<p><br /><br /> Now the history will be displayed in the inspector in realtime!</p>
<br /><br />
<div align="center">
<p><img src="https://68.media.tumblr.com/39ed212eb94043eadcfee8f0f084bfd0/tumblr_inline_oeqctu8z2S1t2g1uk_500.gif" /></p>
</div>

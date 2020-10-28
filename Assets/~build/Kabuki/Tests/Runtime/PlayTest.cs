using System; using System.Collections; using System.Linq;
using System.Collections.Generic; using Ex = System.Exception;
using M = System.Runtime.CompilerServices.CallerMemberNameAttribute;
using UnityEngine; using NUnit.Framework;
using Active.Core; using static Active.Core.status; using Active.Core.Details;

namespace Kabuki.Test{
public class PlayTest{

    GameObject       ground, sun, cam;
    List<GameObject> objects       = new List<GameObject>();
    float       savedTimeScale = 0;
    int       frame;

    protected virtual float baseTimeScale => 1;

    protected virtual string[] skip{ get; }

    // Assertions ---------------------------------------------------

    protected void o (bool arg) => Assert.That(arg);

    protected void o (object x, object y) => Assert.That(x, Is.EqualTo(y));

    // Runners ------------------------------------------------------

    public IEnumerator Complete( Func <status> act, float timeout, int timeScale = 1 ){
        OverrideTimeScale(timeScale * baseTimeScale);
        var t0 = Time.time ;
        while ( Time.time - t0 < timeout ) { if (act().complete) break; yield return null; }
        if ( Time.time - t0 >= timeout )
            throw new Ex($"Timeout: {timeout}s");
        RestoreTimeScale();
    }

    public IEnumerator Run( Func <status> act, float duration, int timeScale = 1 ){
        OverrideTimeScale(timeScale * baseTimeScale);
        var t0 = Time.time ;
        while ( Time.time - t0 < duration ) {
            var s = act();
            if (!s.running) throw
                new Ex($"'cont' expected, {s} found @{Time.frameCount}");
            yield return null;
        }
        RestoreTimeScale();
    }

    public IEnumerator Fail( Func<status> act, float timeout, int timeScale = 1 ){
        OverrideTimeScale(timeScale * baseTimeScale);
        var t0 = Time.time ;
        while ( Time.time - t0 < timeout ) { if (act().failing) break; yield return null; }
        if ( Time.time - t0 >= timeout )
            throw new Ex($"Timeout: {timeout}s");
        RestoreTimeScale();
    }

    // Filtering ----------------------------------------------------

    protected bool Skip([M] string member="") => skip?.Contains(member) ?? false;

    // Utilities ----------------------------------------------------

    public GameObject Create(string name, Vector3? pos=null){
        var go = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(name));
        go.name = name;
        objects.Add(go);
        go.transform.position = pos ?? Vector3.zero;
        return go;
    }

    public Transform Ball(float size, string name = "Ball", Transform parent = null){
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        var θ = go.transform;
        θ.localScale = Vector3.one * size;
        θ.SetParent(parent);
        if (parent != null) go.SetActive(false);
        objects.Add(go);
        return θ;
    }

    public Transform Box(float x, float z, float size, string name = "Box"){
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        var θ = go.transform;
        θ.localScale = Vector3.one * size;
        θ.position = new Vector3(x, size/2, z);
        objects.Add(go);
        return θ;
    }

    public Transform CreateEmpty(string name, Vector3 pos){
        var θ = CreateEmpty(pos);
        θ.gameObject.name = name;
        return θ;
    }

    public Transform CreateEmpty(Vector3 pos){
        var go = new GameObject("Empty");
        go.transform.position = pos;
        objects.Add(go);
        return go.transform;
    }

    protected void Print(object arg) => UnityEngine.Debug.Log(arg);

    // --------------------------------------------------------------

    void OverrideTimeScale(float x){
        savedTimeScale = Time.timeScale;
        Time.timeScale = x;
    }

    void RestoreTimeScale() => Time.timeScale = savedTimeScale;

    [SetUp] public void Setup(){
        Time.timeScale = 1;
        cam = GameObject.Find("Camera");
        if (!cam){
            // Create a camera
            cam = new GameObject("Camera");
            cam.gameObject.AddComponent<Camera>().backgroundColor = Color.white;
            cam.transform.position = Vector3.back * 2 + Vector3.up * 1;
            cam.GetComponent<Camera>().tag = "MainCamera";
            // Create a light
            sun = new GameObject("Sun");
            sun.transform.localEulerAngles = new Vector3(30, 30, 0);
            var light = sun.gameObject.AddComponent<Light>();
            light.type = LightType.Directional;
            // Remove skybox
            RenderSettings.skybox = null;
            // Create ground
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.transform.localScale *= 12;
        }
        Active.Core.Details.RoR.Enter(this, frame);
    }

    [TearDown] public void Teardown(){
        foreach (var k in objects) GameObject.DestroyImmediate(k);
        objects = new List <GameObject>();
        Time.timeScale = 1;
        Active.Core.Details.RoR.Exit(this, ref frame);
    }

}}

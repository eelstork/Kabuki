using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace Kabuki.Test{
public class PlayTest{

    GameObject ground, sun, cam;
    List<GameObject> objects = new List<GameObject>();

    // Assertions ---------------------------------------------------

    protected void o (bool arg) => Assert.That(arg);

    protected void o (object x, object y) => Assert.That(x, Is.EqualTo(y));

    public GameObject Create(string name, Vector3? pos=null){
        var go = Object.Instantiate(Resources.Load<GameObject>(name));
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
    }

    [TearDown] public void Teardown(){
        foreach (var k in objects) GameObject.DestroyImmediate(k);
        objects = new List <GameObject>();
        Time.timeScale = 1;
    }

}}

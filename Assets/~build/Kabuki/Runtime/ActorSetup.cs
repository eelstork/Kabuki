using Ex = System.Exception; using UnityEngine;

namespace Activ.Kabuki{
public static class ActorSetup{

    public static Transform Setup(GameObject go, float size){
        var actor = go.gameObject.AddComponent<Actor>();
        actor.transform.localScale = Vector3.one * size;
        actor.leftHold  = actor.transform.Find("hand", "l");
        actor.rightHold = actor.transform.Find("hand", "r");
        if (actor.leftHold == null) throw new Ex("left hold not found");
        else Debug.Log($"actor left hold: {actor.leftHold}");
        actor.pushingBones = new Transform[]{
            actor.θ.Find("hand", "r"),
            actor.θ.Find("hand", "l")
        };
        actor.gameObject.AddComponent<SpeechBox>();
        return go.transform;
    }

}}

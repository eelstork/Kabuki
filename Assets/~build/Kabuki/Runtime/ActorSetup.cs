using UnityEngine;
namespace Activ.Kabuki{
public static class ActorSetup{

    public static Transform Setup(GameObject go){
        var actor = go.gameObject.AddComponent<Actor>();
        actor.leftHold  = actor.transform.Find("hand", "l");
        actor.rightHold = actor.transform.Find("hand", "r");
        actor.pushingBones = new Transform[]{
            actor.θ.Find("hand", "r"),
            actor.θ.Find("hand", "l")
        };
        actor.GetComponent<Animator>().applyRootMotion = false;
        actor.gameObject.AddComponent<SpeechBox>();
        return go.transform;
    }

}}

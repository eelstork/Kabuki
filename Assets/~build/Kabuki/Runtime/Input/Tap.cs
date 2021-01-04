using UnityEngine;
using Inp = UnityEngine.Input;

namespace Active.Input{
public class Tap : MonoBehaviour{

    public Target target;

    void Update(){
        if (!Inp.GetMouseButtonUp(0)) return ;
        RaycastHit hit;
        bool didHit = Physics.Raycast(Camera.main.ScreenPointToRay(
                                   Inp.mousePosition), out hit);
        if (didHit) target = new Target(){
                            position = hit.point,
                            @object  = hit.collider.transform };
    }

    public class Target{

        public Vector3 position;
        public Transform @object;

    }

}}

using UnityEngine;
using Inp = UnityEngine.Input;

namespace Active.Input{
public class Tap : MonoBehaviour{

    public Vector3? target;

    void Update(){
        if (!Inp.GetMouseButtonUp(0)) return ;
        RaycastHit hit;
        bool didHit = Physics.Raycast(Camera.main.ScreenPointToRay(
                                   Inp.mousePosition), out hit);
        if (didHit) target = hit.point;
    }

}}

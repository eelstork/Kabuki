using UnityEngine;
using Active.Core; using static Active.Status;

namespace Active.Loco{
public interface Locomotion{

    status MoveTo(Vector3 target, float speed);
    status MoveTowards(Vector3 target, float dist, float speed);
    status MoveTowards(Transform target, float dist, float speed);
    status Move(Vector3 u, float speed, float los);
    status Idle();

}}

# Physical locomotion notes

## Regression: rotation management with kinematic Karaptor.



## Regression: Karaptor does not play attack animation [unsolved]

Can confirm that the strike action will play, but seeing no attack animation.
- Animation conflict as the 'Idle' task should not play an idle animation.
- `Idle()` should return `cont`

## Regression: Kinematic Karaptor does not play walk animation

Yea of course, because this was done inside `Actor` then removed.
Not 100% straightforward because KinematicLocomotion does not currently have access to the animation driver; not a big deal either.

## Bug: evading Karaptor may run in place

Conflict between locomotion API and client [fixed]

ISSUE: when using physics we should be able to detect attempts at directly writing to the transform.

## Bug: rotation jitter when idling (physical locomotion test)

This happens while "Homing.Step" is on target.
After fumbling a while found homing updated rotor twice, including at the beginning of `Step` before even checking the goal condition.

ISSUE: pseudo-parallelism and associate resource conflicts clearly are still happening even with BTs.

With that, homing should not mutate anything on 'done'.

## Bug: rotational drift when idle

Idle must invoke the locomotion system, otherwise residual moment will cause this to happen.

## Bug: with "reach" action in Karaptor life, sudden lurch forward when target is near.

From what I can judge this happens because Unity is glitching frame rate when loading the UI graphic (for the "drink" label); not happening a second or third time etc. Taking a pass.

## Bug: with "reach" action in Karaptor life, slowing down much when target is near.

Homing dithers between 'Move' and 'Stall'. Likely reason is that raycast hits the target object (water patch).

## Bug: Unclean "reach" action in Karaptor life

Symptom here is getting close, but then some dithering between (apparently) walking and idle, with itching closer slowly.

Sequence is not consistent; because I notice dithering between `Face` and the loco action (which are inside an ordered sequence) I remove the `Face action` (which when using physical locomotion is redundant).

However relying only on `loco` causes the walk animation to stop, which is unexpected. Immediate cause is removing `Playing(x, task)` from `Actor`. But this animation call is not wanted here.

I've been wanting to see output from both agent and physics agent and, my bad. This feature is not missing, just needs enabling.

The walk animation is definitely called from physics action. In passing I note that how animations are selected is prone to reassigning an animation several times, and this does not feel great.

The real issue here though, may be that both Actor and PhysicalLocomotion implement XTask, and how that implements the "Play" command; we should really have only ONE animation driver here, but we have two.

Added a patch to XTask.Start() where we check for other XTask instances, and share the animation driver if possible.

## Milestone: what more?

Some avoidance is happening, but still seeing the raptor looking stuck occasionally.
- Limit motor output
- Handling slopes
- Support for holes in the ground
- No

## 5. Integrating avoidance

In the current implementation, avoidance is done via a memory sensitive cache. Perhaps this is okay but how this integrate with `Locomotion` is looking verbose significant, where the following must be considered:
- That this vector may be null (expired)
- That no valid direction may be found
- That the avoidance vector, although defined, is now out of date.

Avoidance is course correction. So we need to direction vector to enforce this. In `PhysicalLocomotion` let's have a look at places where this course correction might be applied.

First we note that `Steer` and `MoveTo` are expected to be called on a frame (not physics frame) basis. For avoidance that is more than enough because the avoidance vector would change perhaps every 0.1 to 1 seconds.

## 4. Binding alternative inputs

There are several, potential non equivalent ways that the locomotion component may be used. For example:
- reaching a location
- reaching a transform/actor
- steering towards a given direction

This becomes a problem when we need to store the input, both because it may create confusion, and adversely affect performance.

This problem is also not specific to AL. It occurs because the game loop and physics loop interact. So this is a case where the loop running at a faster frame rate needs to manage transient data.

With apperception this works in reverse, but of course storage still occurs. AP runs and stores its output data, then control fetches the data to operate correctly. However, it does feel cleaner somehow.

We benefit much less from storing apperception results than from storing control instructions. Apperception results are actually expensive to evaluate, whereas control by construction is relatively cheap.

## 3. Binding a frame update API

Binding a frame update API requires a little extra work since the calling tasks run slower than the callees.

We define a `status Steer()` function; although we do need to return a status, at the time this function is called no return status may be available.

So we proceed as follows:

1) The interfacing function must check whether any of the provided parameters have changed. If so the 'dirty' flag is set; if the dirty flag is not set, we return the last known state; otherwise return `cont`.

2) The actual step function stores its returning state.

## 2. Managing orientation

Now that we know a bit about steering, let's figure how to manage orientation.

Essentially if we want to align orientation with a vector u, we apply a torque t = (w ᐧ 𝝇 ᐧ s - w0) ᐧ 𝓽 where:

w  : rotation axis (normalized)
𝝇  : nominal rotation speed
s  : a scalar used to decay rotation (avoid jitter)
w0 : the current angular velocity
𝓽  : a multiplier; when this is small we get a spring effect,
     otherwise rotation is tighter.

### Note: a quick fix using the cross product

If we have a reference direction u, and the current forward vector v, then the cross product has interesting properties we might use:

```
Angle(u, v)  |Cross product|
0            0
45           0.707
90           1
135          0.707
180          0
```

## 1.2 Contact and grounded state

When the actor are not grounded, they cannot generate a force. How do we define "grounded"? Common options are found here:

https://answers.unity.com/questions/196381/how-do-i-check-if-my-rigidbody-player-is-grounded.html

For starters, any of this will do.

Animation: play an idle or flail when not grounded. Play "walk" otherwise.

### 1.1 The effect of friction

If the target speed is 1, in a simple test the output might be ~0.8. Friction accounts for the difference. We confirm this by disabling gravity and moving the capsule above ground.

Note: In a simple setup we do not specify friction (no physics material on either surface); the target plane also does not have a rigid body attached. So there are details of how friction is applied which are undefined.

## 1. Moving forward

For starters we'd like to move a physical capsule evenly on a plane. In this case we apply a *corrective* force at every fixed update:

```howl
∙ δ = 𝝇 ᐧ direction - body.𝓋;
body.AddForce(δ ᐧ body.𝓂 ᐧ 𝓽);
```

With this approach we approximate the target speed 𝝇.

Animation: for now just make it walk.

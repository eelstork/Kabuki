# Kabuki dev notes

## Issues with ingest

An annoyance here is the actor are eating with their left hand, whereas most handed actions use the right hand.

Related:
- It seems like "After" still fires at intervals. With RoR this should
not be the case

## Issues with give and take

Right now give_accept is timing out, and this should mean that the recipient are not holding the ball.

## Throwing a ball

Initial implementation:

```
Hold(⧕) ∧ Face(dir) ∧ Play("Throw") ∧ Impel(⧕, dir);
```

Works but we want to release the projectile before the animation is complete:

```
‒ ⑂ Throw(エ ⧕, メ dir)
    → Hold(⧕) ∧ Face(dir) ∧ Play("Throw")
    + After(0.5f)?[Impel(⧕, dir)];
```

This kind of works but won't `Impel` (which should run only once) repeat adding the force while the animation is completing? Well, no. `After` is designed to run the target action once, until control aways and returns.

Been seeing weird throws in weird directions and finally realized the hand bones (used in pushing) are responsible for this. Let's remove these, of course.

## Push

First implementation (draft):

```howl
‒ ⑂ Push(エ ⧕) → Reach(⧕) ∧ Play("Push")
                          % Do( ⧼Animator⧽.applyRootMotion = ✓ );
```

This is incomplete perhaps because there is nothing linking the block's position to the actor's motion. Regardless, testing this we can see that the push animation fudges. Kind of starts and stops. The reason is because the position of the actor changes (while the block does not move yet), and this may cause `Reach` to re-iterate.

Let's take care of the physics; to the left/right hand, we add:
- A small radius sphere collider
- A kinematic rigid body
Now the colliders are going to push on the block.

Then we rewrite our implementation:

```
‒ ⑂ Push(エ ⧕){
    ⤴ (!pushing){
        ∙ s = Reach(⧕);
        ⤴ (s.complete){
            pushing = ✓;
            ⮐ - UseRootMotion(✓);
        }
        ⮐ s;
    }⤵
        ⮐ Play("Push") ∧ Do( pushing = ✗ ) ∧ UseRootMotion(✗);
}
```

Once we've done so, the action works. Still we'd like to ask, are we really getting out of range while pushing? If not we could simplify this...

```
‒ ⑂ Push(エ ⧕)
    → Reach(⧕)
    ∧ UseRootMotion(✓)
    ∧ Play("Push")
    ∧ UseRootMotion(✗);
```

Unfortunately this really breaks against a boundary condition here. Quite possibly one way to fix this would be in how we implement the `Reach` action. Still, we can have a fairly simple sequence by tweaking this a little:

```
‒ ⑂ Push(エ ⧕)
    → !⦿["Push"] ? Reach(⧕) ∧ Play("Push")
    : UseRootMotion(✓)
    ∧ Play("Push")
    ∧ UseRootMotion(✗);
```

How does this work? Instead of keeping a flag for 'pushing' we query the animation state. Also notice how `Play("Push")` appears twice. The first time to start the animation (or we don't enter the second sequence); the second time we are only calling `Play` to figure whether the animation has stopped playing.

An ordered sequence will also work here:

```
‒ ⑂ Push(エ ⧕) → Sequence()[
      and ? Reach(⧕)
    : and ? UseRootMotion(✓)
    : and ? Play("Push")
    : and ? UseRootMotion(✗) : end
];
```

We have no qualms about mixing ordered and stateless composites:

```
‒ ⑂ Push(エ ⧕) → Sequence()[
      and ? Reach(⧕)
    : and ? UseRootMotion(✓) ∧ Play("Push") ∧ UseRootMotion(✗)
    : end
];
```

Finally, we might use the 'Once' decorator:

```
‒ ⑂ Push_withDecorator(エ ⧕)
    → (⑂)Once()?[ Reach(⧕) ]
    ∧ UseRootMotion(✓)
    ∧ Play("Push")
    ∧ UseRootMotion(✗);
```

Surprisingly however, although looking good this does not return.

### Creating colliders programmatically

This is a better approach mostly because we can't have these colliders enabled at all times or they may conflict with physics at other times.

## Giving

The 'Give' action is interesting because giving is transactional and as such, two agents are involved.
The sequence is like this:

1) Reach another agent.
2) Cue the agent. Implemented as waiting for the other agent to attend (look at) the first agent.
3) Present the gift - play an animation while holding the object.
4) Evaluate the result. On the 'giving' side there isn't anything
to do (the other agent are expected to transfer ownership)

If ownership has transferred, the action is successful; otherwise it has failed.

### Rejected offer (ignore case)

In a simple case, the recipient do not respond to a cue. This is the 'ignore' case because there is no response at all; in this
case `Give` returns `cont` and the give animation does not start.

### Accepted offer

*Initially only failed because the timeout value was too low*

*At end of task, other agent does not have the object*
This is an off-by-one error, as the test completes right before transferring ownership.

Still a bit of flakiness here as I don't see a frame where the give action is complete, and ownership is simultaneously correct. Will get back to this.

## Debriefing notes

To help debugging actions, a quick summary of state evolution over time would be initially more helpful than assertions.

## Grabbing

One thing that came out while implementing the `Grab` action is
that although stateless sequences work well when prior steps do not undo subsequent steps, a complete action should not mutate the agent, and the component actions which make up `Grab` did not comply to this.
More specifically, `X % Play(anim)` is an anti-pattern, because
`Play(anim)` is called over and over after X is complete.
This is awkward, as we need to tie the `cont` state to a driving task, and AL does not have an operator for this. Well, there is no avail of an operator for doing this because it would have to support shorting, and seq/sel operators already use this.
The solution, however, is simple. `Playing(anim, x)` first evaluates x, then plays conditionally (if x is running) and returns the value of x.

## LookAt

In its most simple form:

```
LookAt(T target)
```

## Let's get even more idle

For this to actually I work I need some preparation.
My actor needs a state machine. Let's see whether I can export the one frome Shizen.
Yes, that works.
Added: MecanimDriver

## An idle action; testing

In Shizen, the idle action is a component defined via XTask; frankly this is overkill. We don't want a component for idling; in fact, we perhaps do not want a solid UTask, even for stateful actions; Task may suffice.

Initially we really only want to call either mecanim or legacy animation, and ask them to idle.

Well. I had some trouble writing even a simple play mode test. Which perhaps is no wonder.

- One small question might be why do we need play mode testing for this? I figure that an editor mode test would break down trying to play animations. Still, may be nice to confirm.
- A thing that consistently trips me is creating an assembly next to a howl file, I usually forget to select "use howl".

Now, essentially a play mode test is a co-routine, like so:

```cs
[UnityTest] public IEnumerator Test(){

}
```

And my test looks like this:

```
⏚ Test(){
    ∙ actor = ⌢ Actor();
    for(ᆞ i = 0; i < 5; i++){
        Assert.That( actor.Idle.running, Is.EqualTo(✓) );
        yield ⏂;
    }
}
```

Okay so. Let's add my favorite o function and that'll be a thing for now

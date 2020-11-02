# Kabuki dev notes

## Sequences vs Selectors

There's a couple of things that we're going to cover here:

2) Getting to the bottom of the `Once` RoR issue.
1) Why selectors are a better default than sequences.
3) Delegation

## Is `Once` broken?

The `Once` decorator does not behave *exactly* like the memory node described in Ögren et al.; specifically, Once does not reset "when exiting the parent composite. It resets upon finding a temporal discontinuity. As a result, placed in first position in a stateless composite, `Once` does not reset when looping over:

`Once(A) && B && C` does not reset on loop

`A && Once(B) && C` resets on loop, unless A completes immediately.

This may be considered a bug. There's a couple of ways this can be fixed, however probably not without adding an API. The best candidate is this:

`A && Once(B) && C && @break()`

In this case, what we may offer is a function level, explicit *reset*; this cannot be implemented right away. Currently decorators hash on a line basis. So we'll have to also store them on a member basis (via diagnostics) to implement this feature.

An alternative here is the standard, ordered `Sequence`; however no luck here (could be a genuine 'bug').

## Selectors are better than sequences

Based on what 

## Roaming - failing gracefully

When we implemented roaming we decided to condone failures - such as when avoidance is not enough, or the target is inside another object.

We'd like to do something more colorful than just picking another target. Let's have the raptor flail when stuck. This is a simple change; before:

```
⮐ (~ Reach(target))ʾ ∧ Do( target = ∅ );
```

After:

```
⮐ (Reach(target) ∨ ⦿["Flail"]) ∧ Do( target = ∅ );
```

## Avoidance

Essentially, avoidance provides a function that transforms a raw direction vector into a corrected, clear LOS (line of sight) vector.

Candidly, we integrate this with a `MoveTo` function:

```
‒̥ ⑂ MoveToWithAvoidance(⦿ エ x, メ y, ㅅ speed){
    ⤴ (x˙ ☰ y) ◇̠
    ㅅ d = PlanarDist(x, y);
    ㅅ δ = Mathf.Min(𝛿𝚝 ᐧ 𝝇, d);
    シ  u0 = x.PlanarDir(y);
    シ? u1 = Avoidance.Clear(x˙, u0, maxDistance: d);
    ⤴ (u1 ☰ ∅) ⮐ ■;
    ⮐ Run(x˙ += u1 ᐧ δ);
}
```

The result is actually, well. Fairly disappointing. Kind of works, but we notice two things at first:
- The agent are *slowing down*.
- The rotation is wrong

The issue originates part upstream (in the `Reach`) function, part here, in our first integration.
- Correcting the direction vector means the actor are no longer facing towards the target.
- The actor's orientation also does not match the direction they are moving towards.

As to why this causes the actor to slow down, that's because `Reach` is implemented like this:

```
‒ ⑂ Reach(シ? ⧕) → ⧕.HasValue
? Face(⧕ᖾ) ∧ Playing("Walk", み.MoveToWithAvoidance(⧕ᖾ, speed))
: ◇;
```

If, then, the orientation of the actor is lost, `Face()` shorts locomotion until orientation is corrected, and we're effectively moving at half speed, every odd frame (tested).

We could use a `Once` node (`Once(Face) && MoveTo`). The problem though, is that `Once` resets on discontinuity, and there is no discontinuity in looping the `Roam` task:

```
⁺‒ ⑂ Step() → Reach(target) ∧ Do( giz˙ = target = Target() );
```

Although a bit clunky, we're going to fix this in a simple way:

```
⁺‒ ⑂ Step(){
    ⤴ (target.HasValue) ⮐ Reach(target) ∧ Do( target = ∅ );
    ⤵ {
        target = Target();
        giz˙ = targetᖾ; ☡̱
    }
```

An issue which arises, is obstacle avoidance may fail. For now we'll just condone this failure:

```
(~Reach(target))ʾ
```

Now that this is out of the way let's fix the orientation of the bird while moving. First we just align the transform with the walking direction `x.⫫ = u1ᖾ`; but the avoidance vector does not vary smoothly, so `シ.Lerp(x.⫫, u1ᖾ, 0.1f)`.

Note we only smooth the orientation, not position. This is not perfect, but if we start smoothing positions that will open edge cases where avoidance does not work; leaving that for later.

## Legacy animation 2

If we want smoothly chained animations, we cannot wait
for the previous animation to end - we have to start a cross-fade before this happens, then, for the purpose of smoothing animations, `Play` should complete earlier.

However this depends on the wrap mode; 'loop', 'pingpong' and 'clampForever' always return 'cont'. Only 'once' animations may ever complete.

One question though, what if we want to cross fade with an overlap other than the 0.3f default?

## Legacy animation

For legacy animation, I want to test pretty much the same actions as through the mecanim driver.
So what changes?
- Animation driver
- Actor

One problem is, how do I handle missing animations? I'm not sure.
WTF do I even know what I am trying to do here?
What I want is a working legacy animation driver.

Okay... `Strike` is failing, which is a little surprising. I have animations for this. It is not actually failing, just never getting done.
Well, this may be the case since Strike does not have a timeout.
`Strike`, however, has wrap mode Once. It should complete.
`Play` does succeed. Uses the same Strike action. Now what?
... logic error in the drafted driver. Check.

### `NowPlaying` vs `IsNext`

There's a number of tests failing because testing relies on a (too) subtle distinction between what's playing, and what is next.
I don't think legacy animation is going to support this. As in, not even having the concept.
One reason `IsNext` ever was used: In `LookAt`, the initial state is probably `Idle`, however `Idle` breaks away from the test loop.
Well, a simple condition *should* fix this.
Logic error in `Actor`... check

### Validating the result

I am satisfied with which tests are playing, and which are not. However there are two problems;
- Failing tests are a no go. I want to condone/excuse these tests for 'Raptor'.
- How do I run these tests for both Raptor and Dummy?

Let's try an abstraction first.

## 'Throw' using the Once decorator

This works well with an ordered sequence; using the Once decorator, it does not.
Seems I got why not, Once behaves like this:

- On the first pass, Once() ? [ exp ] evaluates `exp` and returns the value of `exp`; this repeatedly until `exp` has completed.
- Thereafter, `Once` returns *failing*.

This behavior does not feel intuitive. Once should continue returning 'done' if the task was successful, and 'failing' if the task was not successful. Resolving upstream because this is not a simple fix.

### Logging woes

Logging without the logging API is tedious. The expression bodied member needs to be unwrapped; in theory inserting a call to `action Log(msg)` would be a simple alternative; however this has the disadvantage of adding logic operations (could live with this for now) and also, it doesn't really work because of type conversion issues.

# Kabuki dev notes

## The failing placeholder animation

Dunno about that. Fixed itself?

## Ordered sequences... what else?

Bizarrely, when we replace `Reach(target)` with
`Reach(target) ∧ ⦿["Eat"]`, the raptor will get stuck on the "eat" action. Yea that is probably a bug.
- Goes away when removing the `Wait` call in `this[anim]`.
- Observed with both `Wait` and `Timeout()?[]`.

The likely reason is temporal discontinuity so that's an `RoR` conflict... probably?

This has to do with a bad interaction between RoR and ordered composites.
Essentially, an ordered composite can only execute ONE task per frame.
Stateless composites are greedy. For practical purposes they behave like a fallthrough case.
In contrast with the ternary pattern, once the task check passes, later nodes are ignored.
So with how `Reach` is now implemented, over-exercising inserts a delay. The delay is probably one frame.

In this case, for practical purposes, switching the RoR offset from 1 to 5, or just 3... Yea for practical purposes, that fixes the issue.

It doesn't *feel* safe but not sure I can offer anything better right now. Or rather, I arrived at a compromise, as follows:
- When we traverse an ordered composite subtask, add +2 to RoR leniency.
- When we enter the RoR context, -1 to leniency.
- Keep leniency in the +1 - MAX range, where MAX is a small value (using 8)

I'd rather have something that works than not.

## Back to ordered sequences

My conclusion for now is that until a `@break` statement can be implemented, ordered sequences should be used instead.

Well, a quick test finds no fault with ordered sequences.

In `loop` mode a sequence does not complete, and the raptor will aim towards the first target, then walk in place.
In fact this should be expected. `loop` ensures that the sequence will *never* return `done`... does it?

So what if we run in `end` mode?
Then we see the other symptom, where the target once reached, starts blinking all over the map.

Let's run through this step by step.

`Sequence()` returns the `iterator` associated with a stored sequence.
Indeed, the sequence class itself only stores a `SeqIterator`.
`SeqIterator` is a kind of `Iterator`; it maintains the iterative node index `i`. That is NOT indexing the current task, it is only used to traverse the ternary structure.

When `end` is called onto `UTask` (UTaskComposites.cs), the *current* `iterator` is retrieved. Now that's because a `UTask` only refers ONE current iterator at any time (not thread safe), and said `iterator` is nulled (no longer current, so we null the reference). This is *not* trying to GC the iterator. It's only saying "end of composite, the iterator is no longer current".

Then via `SeqIterator.end`, the status +2 (overdone) is returned.

Why +2? That's because control then re-accesses the SAME iterator through `this[in status x]` and this will then return the (-x) status value. So:
- Where a subtask finishes it returns `done`, and because the iterator may run more tasks, the status must be `cont`.
- Where the `end` statement is encountered, we return `done + 1` and this becomes `done`; the `κ.index` value is not modified.
- Where the `loop` statement is encountered, we return `done` and this becomes `cont`; the `κ.index` value is set to -1.

As to said `κ`, that is the `Composite` object, and the `index` value represents the current task.

So uhm, that settles the case:
- in `loop` mode we never get a `done` signal. Cause looping. The ordered sequence resets, but the caller doesn't know that.
- in `end` mode we do get a `done` signal. However the ordered sequence is NOT reset, so it won't ever re-run.

This is a design issue. What's happening here is the client want to loop-over, but they also want to process the `done` signal.

## Animation placeholder

If we look at the raptor, it is lacking an "eat" animation. Although we can duplicate another animation, this is not informative.
Let's just see how playing a non-existent state fails. And, well, that creates an error but no exception.

With legacy animation, we can query a state to see if it actually exists. When the state does not exist, we display a text box instead; we also want to hold a default animation for a while.

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

An alternative here is the standard, ordered `Sequence`; however no luck here (real 'bug'?).

## Selectors are better than sequences

Based on what we've seen, selectors (teleoreactive form) should be used, if possible. The obstacle avoidance example shows why:

```
A && B  // works in v0.1
A && B' // broken because B' conflicts with A
```

The problem is that placing A and B in a sequence creates hidden dependencies. Whenever adding another node, we need to check whether it might conflict with all prior nodes. This is perhaps enough hassle that only ordered sequences should be used.

## Delegation

For obstacle avoidance support, we hacked into the `TransformExt` class. Right here and then, that is fine. A delegation pattern, however, may be useful here.

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

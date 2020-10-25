# Kabuki dev notes

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

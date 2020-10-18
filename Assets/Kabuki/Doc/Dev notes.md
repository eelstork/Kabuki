# Kabuki dev notes

## 'Throw' using the Once decorator

This works well with an ordered sequence; using the Once decorator, it does not.
Seems I got why not, Once behaves like this:

- On the first pass, Once() ? [ exp ] evaluates `exp` and returns the value of `exp`; this repeatedly until `exp` has completed.
- Thereafter, `Once` returns *failing*.

This behavior does not feel intuitive. Once should continue returning 'done' if the task was successful, and 'failing' if the task was not successful. Resolving upstream because this is not a simple fix.

### Logging woes

Logging without the logging API is tedious. The expression bodied member needs to be unwrapped; in theory inserting a call to `action Log(msg)` would be a simple alternative; however this has the disadvantage of adding logic operations (could live with this for now) and also, it doesn't really work because of type conversion issues.

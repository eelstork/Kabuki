# Implementating locomotion - Workflow notes

In implementing physical locomotion several things become apparent

**Lack of solution for logging and visualizing vectors**

Debug.DrawLine/Debug.DrawRay produce transient output. Better than nothing but, still almost nothing. Where complex spatial problems are involved we want:
- output to be logged and retained
- configuring what we want to see dynamically; this is because we quickly end up with too much visual output
- navigating frames, so when something looks odd we can scrub and verify the output

**Fuzziness on what kind of testing would help**

Right now, work with physical locomotion is happening with zero tests, which feels wrong.

**A design issue with standing orders**

**AL or a lack thereof**

We should be wanting two BTs here, one for physics and one for frame updates. This we can get, but visual history does not clearly support this.

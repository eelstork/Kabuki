# Untitled

## A scene

I setup a simple scene that consists in, well; a plane and a spotlight. There is also a character who are roaming.
I combined the roaming with a point and click style navigation method. Finding this interesting because the player do not really control the character; instead they *interfere*.

I defined an exit trigger, so that we exit the scene when staying under the spotlight.

Tip: we don't want Raycast to hit triggers. Under Project Settings > Physics, uncheck [Queries hit triggers]

Technical note: we can separate the trigger implementation from what it actually does trigger. But since I don't need this now, that would be over-architecting.

## What is it all about?

Already I want to ask: where is this going? Are we telling a story or not? For this project (or phase) the right answer is going to be: we are not trying to tell a story; it is a case of narrative exploration, combined with a -

- wish to share simple scripts that realise mechanics.
- wish to explore concepts which are relevant to diegetic design, but without the burdening ambition of actually defining a story.
- dash of poetry

As to what I want to explore. Specifically *existence* is the central topic. Interaction (meaning between the player and the scene) is important but there is also a sense that, like the player, the creatures and phenomena which are being considered are categorical in nature. In this sense each scene is a case study or experiment, and we expect a phenomenon or creature to remain inherently the same throughout.

In other words we'll assume that the sets are just that, but the phenomonology is existential. In the same way that, you can put a cat on a stage, and it is still a cat.

I am also not going to stress creating full blown organisms or anything like that. Instead I'll be focusing on creating automata, purporting to investigate a few things.

One assumption is that motion is expensive, and dynamics are troublesome. We are going to challenge this.

I also want to focus on relationships. Tempted to say, relationships with the player, but with the roaming player character we already broke this framework. Relationships essentially should be categorical, which also fits the goal of creating "existence". We also don't know to what extent the player character is going to be one or many, or what the player may interact with.

So we can have simple relationships. Like moving away, moving closer, circling. Eating and being eaten. Talking, and so forth.

And we see that by adding categorical relationships we set something in motion, dynamically. Then we'll just have to do with whatever that is, and offer a narrative, playful experience (probably not a story).

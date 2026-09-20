# Trust No One

25 cards. 5 bombs. One AI that lies straight to your face.

Play it in your browser → kabirutreja.itch.io/trust-no-one Devlogs → [Stardance project]stardance.hackclub.com/projects/55116

# What is this?

Trust No One is a small psychological card game I built solo in 3 days for Brackeys Game Jam 2026.2. The jam theme was "Trust No One", and I really didn't want to make another story about betrayal. I wanted the mechanic itself to be the thing you can't trust.

So: you're sitting across from an AI opponent. Every round it tells you something about the board. Sometimes it's telling the truth. Sometimes it's lying. You never get to find out for free — you either act on what it said, ignore it completely, or call it a liar and put the whole run on the line.

One wrong guess. One good bluff. That's the game.

# How it plays

You get 25 cards laid face-up at the start. 20 are diamonds. 5 are bombs. Look hard, because they're about to shuffle.

Each round goes like this:

Cards shuffle. They physically swap positions on the board — and they get faster every round, so your mental map falls apart sooner than you'd like.
Your turn. Pick a card.
Diamond → you're safe, keep going.
Bomb → that's the run.
The AI's turn. It secretly picks a card too (it also has to survive), then publicly marks one card number as its pick.
That mark might be a total lie. If the AI is still sitting there alive but marked a card you're fairly sure was a bomb... something doesn't add up.

Instead of picking a card, you can hit CHALLENGE and call the bluff:

The mark was a lie (it was actually a bomb) → you win.
The mark was honest (it really was a diamond) → you lose, and you called it wrong.

And here's the part that makes late rounds nasty: the AI gets less honest and less careful the longer you survive. Early on it's mostly straight with you. Deep into a run, that mark is basically noise. Trust less the further you go.

There's also a sneaky way out, if you've completely lost track of where the bombs are, you can just hope the AI picks one itself and blows up. Winning by waiting is still winning.

# Controls

Mouse only. Click a card to pick it, click CHALLENGE to call out the AI. That's genuinely it.

# What's under the hood

Everything is Unity (C#). Some of the bits I'm happiest with:

# Gameplay

25-card board with 5 bomb positions, regenerated and reshuffled every round
A shuffle system where cards visibly swap places, speeding up each round to ramp difficulty
An AI driven by two separate probabilities: how often it avoids bombs, and how often its public mark is honest, both degrade as the run continues
The Challenge system, which turns every round into a real gamble instead of a guess

# Polish / juice

Custom particle effects for diamond pickups and bomb explosions
Screen shake on game end (win or lose)
Squash-and-stretch animation on card flips, plus button feedback
A full audio system
Dual-character turn indicator (blue = you, pink = the AI), driven by Animator bools synced to whoever's turn it is
Smooth mouse-follow camera with boundaries
Circle-wipe scene transitions between the main menu and the game

# Stuff I'd add with more time

Three days is three days, so a few ideas didn't make the cut:

A "Walk Away" option so you can bank a run instead of being forced to push until you eventually lose. Right now the only exit is losing.
A visible (but abstracted) trust meter, so you can feel how honest the AI is instead of reading it purely off vibes and round count.
A "reveal" ability, so card-tracking stays meaningful past the first couple of rounds — after that the shuffle speed outpaces human memory pretty hard.

There's also good feedback from the jam about adding clearer visual cues for when it's actually your turn to act. Fair point, and it's on the list.

# Credits & assets

Big thanks to Brackeys, not just for running the jam that this game exists because of, but because the art in Trust No One comes from his free asset pack. Massive respect for putting that much good stuff out for free; a huge number of us learned Unity from his videos and then shipped our first jam games on his sprites.

Art assets: Free 2D Mega Pack by Brackeys
Background music: Retro Game Arcade from Pixabay
Engine: Unity
Everything else (code, design, the AI's terrible honesty) by me

No generative AI was used to make this game.

# Jam info
Jam: Brackeys Game Jam 2026.2
Theme: Trust No One
Built in: 3 days, solo
Platform: HTML5 (plays right in the browser)
Rate page: itch.io/jam/brackeys-16/rate/4952146
# Say hi

Made by Kabir Utreja. Feedback is always welcome - if you play it and something feels unfair or unclear, I actually want to hear it.

itch.io: kabirutreja.itch.io
Instagram: @kabir_utreja

Good luck. Don't believe the mark.

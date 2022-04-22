# Slime-MonoGame
A slime simulation that can simulate millions of agents that follow simple swarming rules.

The simulation is run on a GPU using OpenCL (will default to CPU if no GPU detcted).
- Each agent leaves a trail and tries to follow trails left by otheres.
- Each agent has 3 sensors allowing it to sense the trail desity at the 3 sensor points then moves towards stongest density detected.
- Sensor angles and turn stength can be changed during simlation by adjusting the sliders onscreen.

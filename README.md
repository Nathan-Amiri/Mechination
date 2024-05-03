Mechination is a Turing-complete cellular automata--a sandbox where you can create machines, computers, and more using only three simple blocks, called 'Cells'

Created in Unity and coded in Visual Studio 2022

Check out the game's code [here](https://github.com/Nathan-Amiri/Mechination/tree/main/Assets/Scripts)

Play the game [here](https://machine-box.itch.io/mechination)

Gameplay consists of two phases: Edit mode, in which players create designs by placing Cells onto a grid, and Play mode, in which players watch as the Cells act autonomously based on their positions.
Each mode has a separate manager class.

Since nearly all player interface occurs in Edit mode, EditModeManager handles all UI logic. It also handles all Cell spawning/despawning, since Cells cannot spawn or despawn in Play mode.

All Cell behavior logic is contained within the Cell and Gadget classes.

There are three types of Cells: Pulsers, Magnets, and Nodes.
Any logic necessary for all three types of Cells is contained in the Cell class.
Pulsers and Magnets have very similar logic, (Pulsers push and Magnets pull) so they have no need for separate specific Classes, and instead share the Gadget class.
The game rules state that a Node, upon coming into contact with a Gadget, (a Pulser or Magnet) swaps the Pulser to a Magnet or vice versa.
In code, Gadgets detect when non-Gadgets come into contact, then swap themselves.
As a result, there is no need for a separate Node class--Nodes are simply default Cells.

In addition to each Cell's unique behavior, Mechination has three 'fail conditions,' which can prevent Gadgets from activating in certain situations. These conditions are found in the ActivateGadget function in the Gadget class.

In play mode, Gadget activation is ordered such that execution order is never a factor. The fail conditions help ensure this, as well as the Cycle logic found in the PlayModeManager's CycleTick function.

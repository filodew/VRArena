Head Gesture Detector
(c)2014 QLC

Thank you for purchasing 'Head Gesture Detector', the simple head-tracking system
which will inform your app when the user nods, shakes, or tilts their head.
Your app can ask yes/no questions via any means (GUI, voice, etc.), and your
script will receive messages about how the person's head is moving.

With this, you can interpret a 'full nod' as 'yes', a 'full shake' as 'no' and
a full side-nod as 'I don't know'. You can also know about a simple tilt or turn,
and a 'half nod' such as a brief tilt of the head up, then back to neutral.
When indicating full nods, shakes, and side-nods, an int parameter contains how many of those happened.
That way you can respond to 1, 2, or more nods (for example) differently.

Drive GUI's, respond to NPC's or live players, use for navigation, whatever you like!

HOW TO USE:

To get started, add the 'HeadGestureDetector' script to an object. There are two methods:
1. Simply drop the 'HeadGestureDetector' script onto the object which represents the player's 'head'.
In the case of an Oculus Rift controller, this would be one of the cameras nested in the OVR
controller. (This is because at run-time, the cameras move with the HMD rotation.) The script
tracks the rotation of the object, and detects gestures on it.
2. Drop the 'HeadGestureDetector' script onto any object. In this case, you will then use the Inspector
to drag/drop the object which actually moves as the player (HMD-driven), on to the 'Tracked Head' field.

The default behavior is to print messages to the Console. You can use this for testing.
To get this attached to your app, your script(s) will need to implement functions for the message you
care to interpret:

	HeadTiltUp ()			Player's head has tilted up
	HeadTiltDown ()			Player's head has tilted down
	HeadNodUp ()			Player's head has tilted up then returned to neutral
	HeadNodDown ()			Player's head has tilted down then returned to neutral
	HeadFullNod (int)		Player's head has tilted up AND down, param says how many times
	
	HeadTurnLeft ()			Player's head has turned left
	HeadTurnRight ()		Player's head has turned right
	HeadShakeLeft ()		Player's head has turned left then returned to neutral
	HeadShakeRight ()		Player's head has turned right then returned to neutral
	HeadFullShake (int)		Player's head has turned left AND right, param says how many times
	
	HeadLeanLeft ()			Player's head has leaned left
	HeadLeanRight ()		Player's head has leaned right
	HeadNodLeft ()			Player's head has leaned left then returned to neutral
	HeadNodRight ()			Player's head has leaned right then returned to neutral
	HeadFullSideNod (int)	Player's head has leaned right AND left, param says how many times

If you have implemented the full set of functions, you will receive such messages as HeadTiltUp and HeadTiltDown before
receiving HeadNodUp and HeadNodDown. HeadFullNod will follow those.

If the player turns or tilts his head for an amount of time without returning to neutral, that position becomes the new neutral.
This allows the player to turn his body, or generally be looking in some direction, and still be able to respond using head gestures.

BONUS:
There is a simple script for testing on mobile devices. It does not use the gyros because not all phone have them, but uses
compass and accelerometers. It's not meant to be a great comprehensive mobile head tracker, but it should be a proof-of-concept.

EXAMPLE

The TestHeadGestures scene is very simple, and does NOT include other libraries such as Oculus or Durovis (as I have no right to include them).
You can try it, however, in the Editor, by running the scene with both Scene and Console windows visible, select the 'Head' object, and
rotate it manually in the Scene window. Make it nod (rotate on X axis) to say 'yes', shake (rotate on Y) to say 'no', and 'shrug' (rotate
on Z) to say 'I don't know'.  Check the TestUI script to see an example of how to receive and interpret such gestures. Note that this is
only one way of coding it, feel free to use your own style!

FIELDS:

	Nod Sensitivity = 10		How many degrees up or down the player's head must move to be detected as 'tilt'
	Shake Sensitivity = 10		How many degrees right or left the player's head must move to be detected as 'turn'
	Lean Sensitivity = 10		How many degrees right or left the player's head must lean to be detected as 'lean'
	Nod Neutral Zone = 5		How many degrees from the original position is the zone to be considered 'neutral'
	Shake Neutral Zone = 5		How many degrees from the original position is the zone to be considered 'neutral'
	Lean Neutral Zone = 5		How many degrees from the original position is the zone to be considered 'neutral'
	TimeOut = 3					How many seconds to wait for a tilt, turn, or lean before assuming the user has shifted
									to a new 'base-line' position, or time between full nods, shakes, and sideNods

MORE INFO

Please visit http://qlcomp.com for more information about this product, and others too:
Polyhedra Collection, Natural Eyes, Simple Sync for lip-sync, Advanced Footstep System!
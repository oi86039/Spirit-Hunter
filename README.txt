Spirit Hunter Midterm Readme - Omar Ilyas - 11/2/18

	Controls (Also presented in-game):
		- Left Arrow Key = Move Left
		- Right Arrow Key = Move Right
		
		- Up Arrow Key = Jump
			- Hold the Up Arrow Key to jump higher
			- Press the Up Arrow Key on a wall to wall jump.
		
		- Z - Dash while moving
			- Press Z in the air to air dash
		
		- X - Shoot
			- Press and hold the X button to charge up. Release the X button when fully charged to fire a charge shot.
			
		- Miscellaneous:
			-You can charge and jump at the same time.
			-You can charge and dash at the same time.
			-You can charge and air dash at the same time.
			-You can dash and shoot at the same time.
			-You can air dash and shoot at the same time.
		
	Deliverable list:
		Fully Completed:
			Specific
				+ 8 Enemies
				+ 6 AI Patterns
				+ 1 Multistage boss
				+ Minimum 3x3x2 branching level
			Common
				+ Github Repository with Regular Commits
				+ Exe Demo Build
				+ Readme - Controls and Deliverable Testing
		
		Partially Completed:
			- 3 methods of defeating enemies (currently only 2)
			- Game Design Document (Posesses unresolved comments)
		
		Not Completed:
			! Stretch Goal: Weapon Editor
			
	Testing All Deliverables:
		- 2 Methods of defeating enemies
			*To test: Click on the 'Load Level' button and press X on the keyboard to fire a small shot. Use 4 small shots to kill the first enemy in the game, the roller.
			Next, Press and hold X on the keyboard for 2 seconds to charge up. Release X after 2 seconds to fire a charge shot. Use the charge shot to kill another enemy in the game world.*
			
	
		- 8 Enemies
				*To test: Click the 'Load Level' button and traverse through the level to observe all enemy types and their behaviors.*
				
				The following is a list of all enemies encountered in the game, in order of appearance.
				
					1. Rollers - Dull Blue Circle enemies that roll in one direction at constant speed
					2. Owlmen - Bright Red Box enemies that are 1 player tall and fire projectiles
					3. Masks - Brown Floating Box enemies that travel left in a sine wave motion
					4. Aeromen - Golden Floating Triangle enemies that hone in to the right of the player's position
					5. Blowfish - Green Triangle enemies that fire many bullets upward rapidly
					6. WallTurrets - Dark-Purple Box enemies that cling to walls and travel based on the player's vertical position
					7. Ninjamen - Bright Pink Box enemies that act faster than owlmen and can jump in an arc
					8. Heavymen - Large Dark-Red Box enemies that act slower than owlmen and fire large projectiles.
			
		- 6 AI Patterns
				*To test: Click the 'Load Level' button and traverse through the level until an enemy appears on-screen to observe their behaviors.*
				
				All observable enemy AI patterns are listed below:
				
				1. Rollers
					- Rollers roll to the left or right at a constant velocity. Their direction is set at the start of the game, and can change if a roller hits a wall.
					- If the roller hits a wall, it will change directions and travel at a constant velocity in the opposite direction. For example, if the roller was traveling to the left and it hits a wall, it will then travel to the right.
					
				2. Owlmen and Heavymen
					 - Owlmen and Heavymen will choose a random number the first time they are on-screen. Depending on the number, they will either walk towards the player or fire projectile(s) in the x direction. The enemy then waits for a number of seconds before performing another randomly chosen move, and the process repeats for as long as the enemy is on-screen.
						- Owlmen will wait for 2 seconds between performing actions. They can either walk towards the player at a moderate speed, or can fire 3 projectiles in the direction of the player on the x axis. These projectiles travel at half the speed of the player's projectiles, and are spaced out by roughly 2 player units between each of them.
						- Heavymen will wait for 1.6 seconds between performing actions. They can either walk towards the player at a very slow speed, or can fire 1 large projectile in the direction of the player on the x axis. These projectiles travel at 1/4th the speed of the player's projectiles.
						
				3. Masks
					- Masks travel in a sine wave motion towards the left at a constant velocity. Their direction is set at the start of the game, and has the ability to change, but none do in the demo as of 11/2/18.
					
				4. Aeromen
					- Aeromen travel towards the player's x position and hover when they are within a certain range to the right of the player. Aeromen will also choose a random number to decide whether to fire 2 diagonal projectiles toward the player or not. Because Aeromen travel at a slower speed than the player, the player can dash or run to the right of them. If they go enough to the player's left, they will stop pursuing the player and fly horizontally to the left off-screen.
					
				5. Blowfish
					- Blowfish are completely stationary and fire on a timed loop. They will fire 20 projectiles upward in the span of 1.8 seconds, then stop firing for 1 second. This repeats in a loop, and pattern resets itself whenever they are off-screen.
					
				6. WallTurrets
					- WallTurrets can only move on the y axis, and they move towards a player's y position. They also cannot turn to face the player. They fire 3 moderate speed projectiles every 2.8 seconds. These projectiles are about half the speed of the player's projectiles.
					
				7. Ninjamen and The Primate (boss)
					- Ninjamen and The Primate act similar to Owlmen and Heavymen, but can also jump in an arc towards the player. They choose a random number and use that number to decide whether to walk towards the player, fire projectiles at the player, or jump towards the player. They will then wait for a number of seconds before performing another random action.
						- Ninjamen will wait for 1.1 seconds between performing actions. They can either walk towards the player at a fast speed, fire 3 projectiles in the direction of the player on the x axis, or jump in an arch towards the player. These projectiles travel at half the speed of the player's projectiles, and are spaced out by roughly 0.5 player units between each of them.
						- (Primate beavior explained below. See '1 Multistage boss')
						
		- 1 Multistage boss
				*To test: Click the 'Load Boss' button to fight the boss and observe its behavior. Attack the boss and get its health to be less than 35 in order to get it into its second phase.*
				Boss behavior is described below:
			- The Primate
				- The Primate operates with the same AI of the ninjaman, but with a few distinct differences.
						- If The Primate's health is greater than or equal to 35, it will wait for 2.1 seconds between performing actions. It can either walk towards the player at a fast speed, fire 2 projectiles in the direction of the player on the x axis, or jump in an arch towards the player. These projectiles travel at 4/5ths the speed of the player's projectiles, and will boomerang back to the primate at the same speed after traveling at a certain distance. These projectiles can also be destroyed by the player's projectiles.
						- If The Primate's health is less than 35, it will turn red and wait for 1.8 seconds between performing actions. It will increase its speed when walking forward, and will attack with faster projectiles.
						
		- Minimum 3x3x2 branching level
				*To test: Click the 'Load Level" button and play through the game. All different branches are labeled in game according to the following diagram:*
				
					B1		 D1
					||		 ||	      F1
				A - B2 - C - D2 - E - || - Rest - Boss
					||		 ||	      F2
					B3		 D3
				
				*A better way to grade this would be to open the scene 'Level.unity' using the unity editor and zooming out to view the overall map.
		
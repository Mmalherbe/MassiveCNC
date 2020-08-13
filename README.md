# MassiveCNC
A opensource G-code generator built in Unity2019.4.1f1, currently working with an EdingCNC controller. But open for any other controllers to be added.
Originally built for the EdingCNC controllers, to ease out the use of generating Gcode and sending it to the controller. 
The project can currently:
- Generate Gcode from Text (Windows fonts or SVG fonts) *
- Generate Gcode from SVG including layers
- Process and edit pregenerated Gcode

* Not all Windows fonts may work propperly due to the way they are built and converted using GraphicsPath.AddString()

Requirements:
- Unity 2019.4
- EdingCNC Version 4.03.51 (installer included in Repo).



# MassiveCNC

MassiveCNC is an utility that allows CNC machine users to do generate G-code from text, SVG or precompiled G-Code. Or interact with their machine in RealTime using Unity3D.

Currently only works with EdingCNC controllers, but open to other controllers (Will need to be implemented).

## Sidenote
The project can currently:
- Generate Gcode from Text (Windows fonts or SVG fonts) *
- Generate Gcode from SVG including layers
- Process and edit pregenerated Gcode

* Not all Windows fonts may work propperly due to the way they are built and converted using GraphicsPath.AddString()

## TODO
- Fix windows fonts not working properly
- Bugfix the connection between EdingCNC, being unable to connect when starting the project
- Find a gorgeous UI 

## Prerequisites

Before you begin, ensure you have met the following requirements:
<!--- These are just example requirements. Add, duplicate or remove as required --->
* You have installed the Unity3D ( Tested and developed on 2019.4, may work on others).
* You have a Windows or Mac machine, Windows is needed in order to interact with the EdingCNC software.


## Installing MassiveCNC

To install MassiveCNC, follow these steps:

Linux and macOS:
```
Clone this repo and open the project in Unity3D

or ( even better )
Fork and help this utility grow by creating pull requests!
```

Windows:
```
Clone this repo and open the project in Unity3D
(optional) install EdingCNC4.03 ( setup included in repo).

or ( even better )
Fork and help this utility grow by creating pull requests!
```
## Using MassiveCNC

To use MassiveCNC, follow these steps:

```
Open the cloned or forked version in Unity3D.

(optional) for the connection with EdingCNC
Make sure to install EdingCNC 4.03
In the hierarchy under controllers select the cncapi_controller and set the EdingCNCPath correctly ( To the cnc.ini in the installed location of EdingCNC4.03)

For example : C:\Users\M\Desktop\CNC4.03\cnc.ini
 Start the EdingCNC4.03 as administrator and select the blue start button. Afterwards you should be able to connect to EdingCNC.


```

## Contributing to MassiveCNC
<!--- If your README is long or you have some specific process or steps you want contributors to follow, consider creating a separate CONTRIBUTING.md file--->
To contribute to MassiveCNC, follow these steps:

1. Fork this repository.
2. Create a branch: `git checkout -b <branch_name>`.
3. Make your changes and commit them: `git commit -m '<commit_message>'`
4. Push to the original branch: `git push origin MassiveCNC/<location>`
5. Create the pull request.

Alternatively see the GitHub documentation on [creating a pull request](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

## Contributors

Thanks to the following people who have contributed to this project:

* [@scottydocs](https://github.com/scottydocs) 📖 (Creating the perfect README format)
* [@mmalherbe](https://github.com/mmalherbe) 🧩
* [@farooqaziz20](https://github.com/farooqaziz20) 🐛
* [@hamzach276](https://github.com/hamzach276) 🐛



## Contact

If you want to contact me you can reach me at info@mprmalherbe.com.

## License
<!--- If you're not sure which open license to use see https://choosealicense.com/--->

This project uses the following license: [GPL-3.0 License](https://github.com/Mmalherbe/MassiveCNC/blob/master/LICENSE).

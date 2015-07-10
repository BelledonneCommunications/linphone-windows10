The documentation is directly included in the code using Intellisense,
but a .xml file with the C++ API documentation is also generated at the 
compilation as Linphone.Core.xml.

The solution is composed of two projects:
* Linphone.Native: contains the C++ API wrapper for Linphone Core
* Linphone: the interface project

YOU CAN'T PASS A NULL STRING FROM C# TO C++/CX (it has to be empty) AND YOU CAN'T RECEIVE A NULL STRING FROM C++/CX (It will always be an empty string).
See http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs



In order to compile Linphone, you need:
* A computer with Windows 10
* Visual Studio 14 (2015)
* gawk and wget from the GnuWin32 project and put them in your PATH environment variable (http://gnuwin32.sourceforge.net/)
* The Java Runtime Environment (JRE). The 'java' command must be in your PATH environment variable.
* The 'git' command must also be in your PATH environment variable.
* The sources of the project. /!\ You can't have a folder that contains a space character in the path of the sources. /!\

To import the solution, open the Linphone.sln file at the root of the project.

To build, choose the architecture you want to build for (ARM, x64 or x86) and click "Build > Build Solution" (or use Ctrl+Shift+B).
To run the built app, press the green triangle button. It will default to run on the computer for the x64 and x86 architecture and on the plugged in Windows Phone 10 for the ARM architecture.

/!\ Please check the Linphone project in the solution is set as startup project.
(The startup project is in bold in the project list)
If not, right click on Linphone project and choose the menu "Set as StartUp project".

The documentation is directly included in the code using Intellisense,
but a .xml file with the C++ API documentation is also generated at the 
compilation as Linphone.Core.xml.

The solution is composed of four projects:
* Agents: contains the background agents for the VoIP application
* Core: contains the C++ API wrapper for Linphone Core
* CoreProxyStub: contains only generated files allowing 
the Linphone project to make calls to the C++ API
/!\ Do not touch anything in this project!
* Linphone: the interface project

If you have a FileNotFoundException is mscorlib during debug phase, 
open DEBUG->Exceptions and uncheck Common Language Runtime Exceptions->System.IO.FileNotFoundException

YOU CAN'T PASS A NULL STRING FROM C# TO C++/CX (it has to be empty) AND YOU CAN'T RECEIVE A NULL STRING FROM C++/CX (It will always be an empty string).
See http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs

The logs are stored in a file in the application isolated storage. To retrieve it, you need to install this tool on your computer : http://wptools.codeplex.com/

If the application crashes during a call, you may loose the sound in linphone for a while. A device reboot fix this issue.



In order to compile Linphone, you need:
* A computer with Windows 8 (Pro if you want to use the emulator)
* Visual Studio 11 (2012)
* The Windows Phone 8 SDK
* wget, awk, unzip, grep and sed from the GnuWin32 project and put them in your path (http://gnuwin32.sourceforge.net/) for belle-sip and some mediastreamer2 codecs
* The Java Runtime Environment (JRE)

To import the solution, open the Linphone.sln file at the root of the project.

To compile and run it, plug a windows phone 8 device or choose an emulator 
and run the build/install (using F5 or the green triangle). The app will start
automatically once built.

/!\ Please check the Linphone project in the solution is set as startup project.
(The startup project is in bold in the project list)
If not, right click on Linphone project and choose the menu "Set as StartUp project".

To enable network log collect, add the following lines under the [app] section of your linphonerc:

LogDestination=TCPRemote
LogOption=ip:port

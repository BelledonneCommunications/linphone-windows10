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

If the application crashes during a call, you may loose the sound in linphone for a while. A device rebbot fix this issue.
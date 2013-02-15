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
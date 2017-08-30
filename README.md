Linphone is a free VoIP and video softphone based on the SIP protocol.

# Getting started

Here's how to launch Linphone for Windows 10 (more details below):

1. Make sure you are using a Windows 10 computer.
2. Install Visual Studio 15 (2017) from Microsoft.
3. Install CMake (version 3.7 is required at least).
4. Install Python 2.7 (the version 2.7 is required).
5. You also need to have the following tools in the PATH (install them if they're not installed already):
	* The Java Runtime Environment (JRE). The 'java' command must be in your PATH environment variable.
	* The 'git' command must also be in your PATH environment variable.
6. Build SDK (see below for options and explanations):
	* In a Windows command prompt (cmd.exe), run:
 		`python prepare.py -C`
 		`python prepare.py`
 	* Open the generated SDK.sln Visual Studio Solution and build it (`Ctrl+Shift+B`).
7. Open Linphone.sln in Visual Studio and build it


# Building the SDK

Linphone for Windows 10 depends on liblinphone SDK. This SDK is generated from Visual Studio with some to (namely Python and CMake) to ease the configuration of what to build.

 To generate the liblinphone multi-arch SDK in GPL mode, simply invoke in a Windows command prompt:

        python prepare.py [options]

 This generates a SDK.sln Visual Studio Solution. Open it and build it.
 The result is a NuGet package that is used in the Linphone application to embed the liblinphone features.

**The resulting SDK is located in `OUTPUT/` root directory.**

## Licensing: GPL third parties versus non GPL third parties

This SDK can be generated in 2 flavors:

* GPL third parties enabled means that liblinphone includes GPL third parties like FFmpeg. If you choose this flavor, your final application **must comply with GPL in any case**. This is the default mode.

* NO GPL third parties means that Linphone will only use non GPL code except for `liblinphone`, `mediastreamer2`, `oRTP` and `belle-sip`. If you choose this flavor, your final application is **still subject to GPL except if you have a [commercial license for the mentioned libraries](http://www.belledonne-communications.com/products.html)**.
 To generate the liblinphone multi arch SDK without GPL third parties, invoke:

        python prepare.py --disable-gpl-third-parties [other options]

## Customizing features

You can enable non-free codecs by using `--enable-non-free-codecs` and `-DENABLE_<codec>=ON`. To get a list of all features, the simplest way is to invoke `prepare.py` with `--list-features`:

        python prepare.py --list-features

You can for instance enable OpenH264 by using:

        python prepare.py --enable-non-free-codecs -DENABLE_OPENH264=ON [other options]

## Built architectures

3 architectures currently exists on Windows 10:

- 32 bits ARM for mobile devices.
- 64 bits x64 for 64 bits desktop systems.
- 32 bits x86 for 32 bits desktop systems.

 Note: The 3 architectures are built by default but you can build only one by passing its name (ARM, x64 or x86) to the `prepare.py` script.

## Upgrading your SDK

Simply re-building the SDK.sln Visual Studio solution should update your SDK.
If compilation fails, you may need to rebuilding everything by invoking:

        python prepare.py -C
        python prepare.py [options]

And then build the SDK.sln Visual Studio solution again.

# Building the application

After the SDK is built, just open the Linphone.sln Visual Studio solution and build it.

## Note regarding third party components subject to license

 The liblinphone SDK is compiled with third parties code that are subject to patent license, specially: AMR, SILK, G729 and H264 codecs.
 You can enable them using `prepare.py` script (see `--enable-non-free-codecs` option). Before embedding these 4 codecs in the final
 application, **make sure to have the right to do so**.

# Limitations and known bugs

* The Linphone application is not a full feature application yet and the SDK is fully functional.

# Debugging the SDK

Sometime it can be useful to step into liblinphone SDK functions. To allow Visual Studio to enable breakpoint within liblinphone, SDK must be built with debug symbols by using option `--debug`:

        python prepare.py --debug [other options]


# Note for developpers

YOU CAN'T PASS A NULL STRING FROM C# TO C++/CX (it has to be empty) AND YOU CAN'T RECEIVE A NULL STRING FROM C++/CX (It will always be an empty string).
See http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs

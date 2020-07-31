[![pipeline status](https://gitlab.linphone.org/BC/public/linphone-windows10/badges/master/pipeline.svg)](https://gitlab.linphone.org/BC/public/linphone-windows10/commits/master)

Linphone is a free VoIP and video softphone based on the SIP protocol.

# Get Linphone SDK

## From packages

Get LinphoneSDK from nuget packager or directly from our website https://www.linphone.org

## From Source

Build the Nuget package from  https://gitlab.linphone.org/BC/public/linphone-sdk by following the readme in Windows UWP section.
The build options for this Windows10 project is:
`cmake.exe .. -G "Visual Studio 15 2017" -DLINPHONESDK_PLATFORM=Desktop -DENABLE_CSHARP_WRAPPER=YES -DCMAKE_BUILD_TYPE=Release -DENABLE_LIME_X3DH=NO -DENABLE_GPL_THIRD_PARTIES=YES -DENABLE_NON_FREE_CODECS=YES -DENABLE_OPENH264=YES -DCMAKE_TOOLCHAIN_FILE=../cmake-builder/toolchains/toolchain-windows-store.cmake`

# Building the application

Just open the Linphone.sln Visual Studio solution.
Install the nuget package on the linphone project.
Build for x86.


# Limitations and known bugs

* The Linphone application is not a full feature application yet and the SDK is fully functional.

# Note for developpers

YOU CAN'T PASS A NULL STRING FROM C# TO C++/CX (it has to be empty) AND YOU CAN'T RECEIVE A NULL STRING FROM C++/CX (It will always be an empty string).
See http://stackoverflow.com/questions/12980915/exception-when-trying-to-read-null-string-in-c-sharp-winrt-component-from-winjs

# Application permissions

Since Windows Fall Creator update, at the first use, Linphone will require your permission before it can access your contacts, your microphone and your camera which you can accept or not.
But sometimes you might want to run automatic tests on Linphone and can't be around to manually accept these permissions.
If that's the case, you can force authorizations of applications. To do so, you have to open `gpedit.msc` and go to:

        Computer Configuration\Administrative Templates\Windows Components\App Privacy

In the right pane, double click on the concerned permissions (camera, contacts and microphone for Linphone), select **Enabled** and select **Force authorization** in options.

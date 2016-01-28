Here is how the openh264 prebuilt libraries have been generated from the tag v1.5.0 of the git repository (https://github.com/cisco/openh264.git) with the 0001-Add-compilation-and-link-flags-for-Windows-10-build.patch applied.
These instructions are mainly based on those to build ffmpeg from https://trac.ffmpeg.org/wiki/CompilationGuide/WinRT.



x86:

In the VS2015 x64 x86 Cross Tools Command Prompt:

SET LIB=%VSINSTALLDIR%VC\lib\store;%VSINSTALLDIR%VC\atlmfc\lib;%UniversalCRTSdkDir%lib\%UCRTVersion%\ucrt\x86;;%UniversalCRTSdkDir%lib\%UCRTVersion%\um\x86;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\lib\um\x86;;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\Lib\um\x86
SET LIBPATH=%VSINSTALLDIR%VC\vcpackages;%FrameworkDir%\%FrameworkVersion%;%VSINSTALLDIR%VC\lib;%VSINSTALLDIR%VC\atlmfc\lib;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;\Microsoft.VCLibs\14.0\References\CommonConfiguration\neutral;
SET INCLUDE=%VSINSTALLDIR%VC\include;%VSINSTALLDIR%VC\atlmfc\include;%UniversalCRTSdkDir%Include\%UCRTVersion%\ucrt;%UniversalCRTSdkDir%Include\%UCRTVersion%\um;%UniversalCRTSdkDir%Include\%UCRTVersion%\shared;%UniversalCRTSdkDir%Include\%UCRTVersion%\winrt;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\Include\um;
C:\msys64\msys2_shell.bat

Then, in the just opened msys64 shell (in the source directory of openh264):

mingw32-make.exe libraries PREFIX=win10-x86 OS=msvc-app ARCH=i386
mingw32-make.exe install-shared PREFIX=win10-x86 OS=msvc-app ARCH=i386
mingw32-make.exe libraries PREFIX=win10-x86 OS=msvc-app ARCH=i386 clean



x64:

In the VS2015 x64 Native Tools Command Prompt:

SET LIB=%VSINSTALLDIR%VC\lib\store\amd64;%VSINSTALLDIR%VC\atlmfc\lib\amd64;%UniversalCRTSdkDir%lib\%UCRTVersion%\ucrt\x64;;%UniversalCRTSdkDir%lib\%UCRTVersion%\um\x64;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\lib\um\x64;;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\Lib\um\x64
SET LIBPATH=%VSINSTALLDIR%VC\vcpackages;%FrameworkDir%\%FrameworkVersion%;%VSINSTALLDIR%VC\lib\amd64;%VSINSTALLDIR%VC\atlmfc\lib\amd64;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;\Microsoft.VCLibs\14.0\References\CommonConfiguration\neutral;
SET INCLUDE=%VSINSTALLDIR%VC\include;%VSINSTALLDIR%VC\atlmfc\include;%UniversalCRTSdkDir%Include\%UCRTVersion%\ucrt;%UniversalCRTSdkDir%Include\%UCRTVersion%\um;%UniversalCRTSdkDir%Include\%UCRTVersion%\shared;%UniversalCRTSdkDir%Include\%UCRTVersion%\winrt;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\Include\um;
C:\msys64\msys2_shell.bat

Then, in the just opened msys64 shell (in the source directory of openh264):

mingw32-make.exe libraries PREFIX=win10-x64 OS=msvc-app ARCH=x86_64
mingw32-make.exe install-shared PREFIX=win10-x64 OS=msvc-app ARCH=x86_64
mingw32-make.exe libraries PREFIX=win10-x64 OS=msvc-app ARCH=x86_64 clean



ARM:

In the VS2015 x64 ARM Cross Tools Command Prompt:

SET LIB=%VSINSTALLDIR%VC\lib\store\ARM;%VSINSTALLDIR%VC\atlmfc\lib\ARM;%UniversalCRTSdkDir%lib\%UCRTVersion%\ucrt\arm;;%UniversalCRTSdkDir%lib\%UCRTVersion%\um\arm;
SET LIBPATH=%VSINSTALLDIR%VC\vcpackages;%FrameworkDir%\%FrameworkVersion%;%VSINSTALLDIR%VC\lib\ARM;%VSINSTALLDIR%VC\atlmfc\lib\ARM;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;\Microsoft.VCLibs\14.0\References\CommonConfiguration\neutral;
SET INCLUDE=%VSINSTALLDIR%VC\include;%VSINSTALLDIR%VC\atlmfc\include;%UniversalCRTSdkDir%Include\%UCRTVersion%\ucrt;%UniversalCRTSdkDir%Include\%UCRTVersion%\um;%UniversalCRTSdkDir%Include\%UCRTVersion%\shared;%UniversalCRTSdkDir%Include\%UCRTVersion%\winrt;C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6\Include\um;
C:\msys64\msys2_shell.bat

Then, in the just opened msys64 shell (in the source directory of openh264):

mingw32-make.exe libraries PREFIX=win10-arm OS=msvc-app ARCH=armv7
mingw32-make.exe install-shared PREFIX=win10-arm OS=msvc-app ARCH=armv7
mingw32-make.exe libraries PREFIX=win10-arm OS=msvc-app ARCH=armv7 clean

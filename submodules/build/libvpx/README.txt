Here is how the libvpx prebuilt libraries have been generated.
The patch 0001-Support-build-with-Visual-Studio-14.patch needs to be applied to revision commit f8b869f573cd2503847f80043bfd4751a78a091b of libvpx for these instructions to be successful.

The commands are to be executed in a msys shell.


x86:

export PATH="/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE/CommonExtensions/Microsoft/TestWindow:/c/Program Files (x86)/MSBuild/14.0/bin/amd64:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/BIN/amd64_x86:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/BIN/amd64:/c/WINDOWS/Microsoft.NET/Framework64/v4.0.30319:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/VCPackages:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/Tools:/c/Program Files (x86)/HTML Help Workshop:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Team Tools/Performance Tools/x64:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Team Tools/Performance Tools:/c/Program Files (x86)/Windows Kits/8.1/bin/x64:/c/Program Files (x86)/Windows Kits/8.1/bin/x86:/c/Program Files (x86)/Microsoft SDKs/Windows/v8.1A/bin/NETFX 4.5.1 Tools/x64:$PATH"
export INCLUDE="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\INCLUDE;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\INCLUDE;C:\Program Files (x86)\Windows Kits\10\\include\10.0.10056.0\ucrt;C:\Program Files (x86)\Windows Kits\8.1\include\shared;C:\Program Files (x86)\Windows Kits\8.1\include\um;C:\Program Files (x86)\Windows Kits\8.1\include\winrt;"
export LIB="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\LIB;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\LIB;C:\Program Files (x86)\Windows Kits\10\\lib\10.0.10056.0\ucrt\x86;C:\Program Files (x86)\Windows Kits\8.1\lib\winv6.3\um\x86;"
export LIBPATH="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcpackages;C:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\LIB;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\LIB;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;\Microsoft.VCLibs\14.0\References\CommonConfiguration\neutral;"
mkdir build-x86
cd build-x86
../configure "--enable-error-concealment" "--enable-multithread" "--enable-realtime-only" "--enable-spatial-resampling" "--enable-vp8" "--disable-vp9" "--enable-libs" "--disable-install-docs" "--disable-debug-libs" "--disable-examples" "--disable-unit-tests" "--as=yasm" "--target=x86-win32-vs14"
make V=1



x64:

export PATH="/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE/CommonExtensions/Microsoft/TestWindow:/c/Program Files (x86)/MSBuild/14.0/bin/amd64:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/BIN/amd64:/c/WINDOWS/Microsoft.NET/Framework64/v4.0.30319:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/VCPackages:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/Tools:/c/Program Files (x86)/HTML Help Workshop:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Team Tools/Performance Tools/x64:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Team Tools/Performance Tools:/c/Program Files (x86)/Windows Kits/8.1/bin/x64:/c/Program Files (x86)/Windows Kits/8.1/bin/x86:/c/Program Files (x86)/Microsoft SDKs/Windows/v8.1A/bin/NETFX 4.5.1 Tools/x64:$PATH"
export INCLUDE="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\INCLUDE;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\INCLUDE;C:\Program Files (x86)\Windows Kits\10\\include\10.0.10056.0\ucrt;C:\Program Files (x86)\Windows Kits\8.1\include\shared;C:\Program Files (x86)\Windows Kits\8.1\include\um;C:\Program Files (x86)\Windows Kits\8.1\include\winrt"
export LIB="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\LIB\amd64;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\LIB\amd64;C:\Program Files (x86)\Windows Kits\10\\lib\10.0.10056.0\ucrt\x64;C:\Program Files (x86)\Windows Kits\8.1\lib\winv6.3\um\x64"
export LIBPATH="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcpackages;C:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\LIB\amd64;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\LIB\amd64;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;\Microsoft.VCLibs\14.0\References\CommonConfiguration\neutral;"
mkdir build-x64
cd build-x64
../configure "--enable-error-concealment" "--enable-multithread" "--enable-realtime-only" "--enable-spatial-resampling" "--enable-vp8" "--disable-vp9" "--enable-libs" "--disable-install-docs" "--disable-debug-libs" "--disable-examples" "--disable-unit-tests" "--as=yasm" "--target=x86_64-win64-vs14"
make V=1



ARM:

export PATH="/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE/CommonExtensions/Microsoft/TestWindow:/c/Program Files (x86)/MSBuild/14.0/bin/amd64:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/BIN/amd64_arm:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/BIN/amd64:/c/WINDOWS/Microsoft.NET/Framework64/v4.0.30319:/c/Program Files (x86)/Microsoft Visual Studio 14.0/VC/VCPackages:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/IDE:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Common7/Tools:/c/Program Files (x86)/HTML Help Workshop:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Team Tools/Performance Tools/x64:/c/Program Files (x86)/Microsoft Visual Studio 14.0/Team Tools/Performance Tools:/c/Program Files (x86)/Windows Kits/8.1/bin/x64:/c/Program Files (x86)/Windows Kits/8.1/bin/x86:/c/Program Files (x86)/Microsoft SDKs/Windows/v8.1A/bin/NETFX 4.5.1 Tools/x64:$PATH"
export INCLUDE="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\INCLUDE;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\INCLUDE;C:\Program Files (x86)\Windows Kits\10\\include\10.0.10056.0\ucrt;C:\Program Files (x86)\Windows Kits\8.1\include\shared;C:\Program Files (x86)\Windows Kits\8.1\include\um;C:\Program Files (x86)\Windows Kits\8.1\include\winrt;"
export LIB="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\LIB\ARM;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\LIB\ARM;C:\Program Files (x86)\Windows Kits\10\\lib\10.0.10056.0\ucrt\ARM;C:\Program Files (x86)\Windows Kits\8.1\lib\winv6.3\um\ARM;"
export LIBPATH="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\vcpackages;C:\WINDOWS\Microsoft.NET\Framework64\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\LIB\ARM;C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\ATLMFC\LIB\ARM;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;\Microsoft.VCLibs\14.0\References\CommonConfiguration\neutral;"
mkdir build-arm
cd build-arm
../configure "--enable-error-concealment" "--enable-multithread" "--enable-realtime-only" "--enable-spatial-resampling" "--enable-vp8" "--disable-vp9" "--enable-libs" "--disable-install-docs" "--disable-debug-libs" "--disable-examples" "--disable-unit-tests" "--as=yasm" "--target=armv7-win32-vs14"
make V=1

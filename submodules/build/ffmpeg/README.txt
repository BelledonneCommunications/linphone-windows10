Basically, follow instructions from https://trac.ffmpeg.org/wiki/CompilationGuide/WinRT but replace the configure options by the following ones (built tag n2.8.4 from the git repository git://source.ffmpeg.org/ffmpeg.git):


x86:

../../../configure --toolchain=msvc --disable-zlib --disable-bzlib --disable-mmx --disable-ffplay --disable-ffprobe --disable-ffserver --disable-avdevice --disable-avfilter --disable-network --disable-avformat --disable-everything --enable-decoder=mjpeg --enable-encoder=mjpeg --disable-vda --disable-vaapi --disable-vdpau --enable-decoder=h263 --enable-encoder=h263 --enable-encoder=h263p --enable-decoder=mpeg4 --enable-encoder=mpeg4 --arch=x86 --enable-shared --enable-cross-compile --target-os=win32 --extra-cflags="-MD -DWINAPI_FAMILY=WINAPI_FAMILY_APP -D_WIN32_WINNT=0x0A00" --extra-ldflags="-APPCONTAINER WindowsApp.lib" --prefix=../../../Build/Windows10/x86


x64:

../../../configure --toolchain=msvc --disable-zlib --disable-bzlib --disable-mmx --disable-ffplay --disable-ffprobe --disable-ffserver --disable-avdevice --disable-avfilter --disable-network --disable-avformat --disable-everything --enable-decoder=mjpeg --enable-encoder=mjpeg --disable-vda --disable-vaapi --disable-vdpau --enable-decoder=h263 --enable-encoder=h263 --enable-encoder=h263p --enable-decoder=mpeg4 --enable-encoder=mpeg4 --arch=x86_64 --enable-shared --enable-cross-compile --target-os=win32 --extra-cflags="-MD -DWINAPI_FAMILY=WINAPI_FAMILY_APP -D_WIN32_WINNT=0x0A00" --extra-ldflags="-APPCONTAINER WindowsApp.lib" --prefix=../../../Build/Windows10/x64


ARM:

../../../configure --toolchain=msvc --disable-zlib --disable-bzlib --disable-mmx --disable-ffplay --disable-ffprobe --disable-ffserver --disable-avdevice --disable-avfilter --disable-network --disable-avformat --disable-everything --enable-decoder=mjpeg --enable-encoder=mjpeg --disable-vda --disable-vaapi --disable-vdpau --enable-decoder=h263 --enable-encoder=h263 --enable-encoder=h263p --enable-decoder=mpeg4 --enable-encoder=mpeg4 --arch=arm --as=armasm --cpu=armv7 --enable-thumb --enable-shared --enable-cross-compile --target-os=win32 --extra-cflags="-MD -DWINAPI_FAMILY=WINAPI_FAMILY_APP -D_WIN32_WINNT=0x0A00 -D__ARM_PCS_VFP" --extra-ldflags="-APPCONTAINER WindowsApp.lib" --prefix=../../../Build/Windows10/ARM

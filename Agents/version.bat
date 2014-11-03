@ECHO off

SET gitlog=
FOR /f "delims=" %%a IN ('git log -1 "--pretty=format:%%H" %1\Customs.cs') DO SET gitlog=%%a

IF [%gitlog%] == [] GOTO UnknownGitVersion

FOR /f "delims=" %%a IN ('git describe --always') DO SET gitdescribe=%%a
GOTO End

:UnknownGitVersion
SET gitdescribe=unknown

:End
ECHO namespace Linphone { public sealed class Version { public static string Number = "%gitdescribe%"; } } > Version.cs

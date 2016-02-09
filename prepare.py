#!/usr/bin/env python

############################################################################
# prepare.py
# Copyright (C) 2015  Belledonne Communications, Grenoble France
#
############################################################################
#
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
#
############################################################################

import argparse
import os
import re
import shutil
import tempfile
import subprocess
import sys
import urllib
import uuid
from logging import error, warning, info, INFO, basicConfig
from distutils.spawn import find_executable
from subprocess import Popen, PIPE
sys.dont_write_bytecode = True
sys.path.insert(0, 'submodules/cmake-builder')
try:
    import prepare
except Exception as e:
    error(
        "Could not find prepare module: {}, probably missing submodules/cmake-builder? Try running:\n"
        "git submodule sync && git submodule update --init --recursive".format(e))
    exit(1)


class Win10Target(prepare.Target):

    def __init__(self, arch, generator_suffix = ''):
        prepare.Target.__init__(self, 'win10-' + arch)
        current_path = os.path.dirname(os.path.realpath(__file__))
        current_path = current_path.replace('\\', '/')
        self.config_file = 'configs/config-win10.cmake'
        self.output = 'OUTPUT/win10-' + arch
        self.additional_args = [
            '-GVisual Studio 14 2015' + generator_suffix,
            '-DCMAKE_SYSTEM_NAME=WindowsStore',
            '-DCMAKE_SYSTEM_VERSION=10.0',
            '-DCMAKE_SYSTEM_PROCESSOR=' + arch,
            '-DCMAKE_INSTALL_MESSAGE=LAZY',
            '-DCMAKE_VS_INCLUDE_INSTALL_TO_DEFAULT_BUILD=TRUE',
            '-DLINPHONE_BUILDER_GROUP_EXTERNAL_SOURCE_PATH_BUILDERS=YES',
            '-DLINPHONE_BUILDER_EXTERNAL_SOURCE_PATH=' + current_path + '/submodules'
        ]

    def clean(self):
        if os.path.isdir('WORK'):
            shutil.rmtree(
                'WORK', ignore_errors=False, onerror=self.handle_remove_read_only)
        if os.path.isdir('OUTPUT'):
            shutil.rmtree(
                'OUTPUT', ignore_errors=False, onerror=self.handle_remove_read_only)


class Win10X86Target(Win10Target):

    def __init__(self):
        Win10Target.__init__(self, 'x86')


class Win10X64Target(Win10Target):

    def __init__(self):
        Win10Target.__init__(self, 'x64', ' Win64')


class Win10ARMTarget(Win10Target):

    def __init__(self):
        Win10Target.__init__(self, 'ARM', ' ARM')


targets = {
    'x86': Win10X86Target(),
    'x64': Win10X64Target(),
    'ARM': Win10ARMTarget(),
}
platforms = ['all', 'ARM', 'x64', 'x86']
builder_targets = ['linphone', 'ms2', 'belle-sip']


class PlatformListAction(argparse.Action):
    def __call__(self, parser, namespace, values, option_string=None):
        if values:
            for value in values:
                if value not in platforms:
                    message = ("invalid platform: {0!r} (choose from {1})".format(
                        value, ', '.join([repr(platform) for platform in platforms])))
                    raise argparse.ArgumentError(self, message)
            setattr(namespace, self.dest, values)

class TargetListAction(argparse.Action):
    def __call__(self, parser, namespace, value, option_string=None):
        if value:
            if value not in builder_targets:
                message = ("Invalid target: {0!r} (choose from {1}".format(
                    value, ', '.join([repr(target) for target in builder_targets])))
                raise argparse.ArgumentError(self, message)
            setattr(namespace, self.dest, value)


def gpl_disclaimer(platforms):
    cmakecache = 'WORK/win10-{arch}/cmake/CMakeCache.txt'.format(arch=platforms[0])
    gpl_third_parties_enabled = "ENABLE_GPL_THIRD_PARTIES:BOOL=YES" in open(
        cmakecache).read() or "ENABLE_GPL_THIRD_PARTIES:BOOL=ON" in open(cmakecache).read()

    if gpl_third_parties_enabled:
        warning("\n***************************************************************************"
                "\n***************************************************************************"
                "\n***** CAUTION, this liblinphone SDK is built using 3rd party GPL code *****"
                "\n***** Even if you acquired a proprietary license from Belledonne      *****"
                "\n***** Communications, this SDK is GPL and GPL only.                   *****"
                "\n***** To disable 3rd party gpl code, please use:                      *****"
                "\n***** $ ./prepare.py [previous options] -DENABLE_GPL_THIRD_PARTIES=NO *****"
                "\n***************************************************************************"
                "\n***************************************************************************")
    else:
        warning("\n***************************************************************************"
                "\n***************************************************************************"
                "\n***** Linphone SDK without 3rd party GPL software                     *****"
                "\n***** If you acquired a proprietary license from Belledonne           *****"
                "\n***** Communications, this SDK can be used to create                  *****"
                "\n***** a proprietary linphone-based application.                       *****"
                "\n***************************************************************************"
                "\n***************************************************************************")


missing_dependencies = {}


def check_is_installed(binary, prog=None, warn=True):
    if not find_executable(binary):
        if warn:
            error("Could not find {}. Please install {}.".format(binary, prog))
        return False
    return True


def check_tools():
    ret = 0

    ret |= not check_is_installed('cmake')
    ret |= not check_is_installed('git')

    if not os.path.isdir("submodules/linphone/mediastreamer2/src") or not os.path.isdir("submodules/linphone/oRTP/src"):
        error("Missing some git submodules. Did you run:\n\tgit submodule update --init --recursive")
        ret = 1

    return ret


def download_nuget():
    if not os.path.isdir('WORK'):
        os.makedirs("WORK")
    if not os.path.isfile('WORK/nuget.exe'):
        urllib.urlretrieve("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", "WORK/nuget.exe")


def list_features_with_args(debug, additional_args):
    tmpdir = tempfile.mkdtemp(prefix="linphone-windows10")
    tmptarget = Win10X86Target()
    tmptarget.abs_cmake_dir = tmpdir

    option_regex = re.compile("ENABLE_(.*):(.*)=(.*)")
    options = {}
    ended = True
    build_type = 'Debug' if debug else 'Release'

    for line in Popen(tmptarget.cmake_command(build_type, False, True, additional_args, verbose=False),
                      cwd=tmpdir, shell=False, stdout=PIPE).stdout.readlines():
        match = option_regex.match(line)
        if match is not None:
            (name, typeof, value) = match.groups()
            options["ENABLE_{}".format(name)] = value
            ended &= (value == 'ON')
    shutil.rmtree(tmpdir)
    return (options, ended)


def list_features(debug, args):
    additional_args = args
    options = {}
    info("Searching for available features...")
    # We have to iterate multiple times to activate ALL options, so that options depending
    # of others are also listed (cmake_dependent_option macro will not output options if
    # prerequisite is not met)
    while True:
        (options, ended) = list_features_with_args(debug, additional_args)
        if ended:
            break
        else:
            additional_args = []
            # Activate ALL available options
            for k in options.keys():
                additional_args.append("-D{}=ON".format(k))

    # Now that we got the list of ALL available options, we must correct default values
    # Step 1: all options are turned off by default
    for x in options.keys():
        options[x] = 'OFF'
    # Step 2: except options enabled when running with default args
    (options_tmp, ended) = list_features_with_args(debug, args)
    final_dict = dict(options.items() + options_tmp.items())

    notice_features = "Here are available features:"
    for k, v in final_dict.items():
        notice_features += "\n\t{}={}".format(k, v)
    info(notice_features)
    info("To enable some feature, please use -DENABLE_SOMEOPTION=ON (example: -DENABLE_OPUS=ON)")
    info("Similarly, to disable some feature, please use -DENABLE_SOMEOPTION=OFF (example: -DENABLE_OPUS=OFF)")


def git_version(path):
    proc = subprocess.Popen(["git", "describe", "--always"], cwd=path, shell=False, stdout=subprocess.PIPE)
    out, err = proc.communicate()
    out = out.strip()
    pos = out.find('-g')
    if pos == -1:
        return out
    else:
        return out[:out.find('-g')].replace('-', '.')


def generate_solution(debug, selected_platforms, builder_target):
    current_path = os.path.dirname(os.path.realpath(__file__))
    current_path = current_path.replace('\\', '/')
    guids = {}
    sln_projects = ""
    sln_confs = ""
    other_sdks = []
    build_type = 'Debug' if debug else 'Release'
    if builder_target == 'linphone':
        linphone_version = git_version('submodules/linphone')
        builder_target = [
            ('LinphoneTesterSDK', linphone_version),
            ('LinphoneSDK', linphone_version),
        ]
    elif builder_target == 'ms2-plugins':
        ms2_version = git_version('submodules/linphone/mediastreamer2')
        builder_target = [
            ('MS2TesterSDK', ms2_version),
        ]
    elif builder_target == 'belle-sip':
        bellesip_version = git_version('submodules/belle-sip')
        builder_target = [
            ('BelleSipTesterSDK', bellesip_version),
        ]
    else:
        return

    vcxproj_platforms = {}
    for platform in selected_platforms:
        if platform == 'x86':
            vcxproj_platforms[platform] = 'Win32'
        else:
            vcxproj_platforms[platform] = platform

    # Generate Visual Studio project to build the SDK for each platform
    for platform in selected_platforms:
        guid = '{' + str(uuid.uuid4()).upper() + '}'
        guids[platform] = guid
        vcxproj = """<?xml version="1.0" encoding="UTF-8"?>
<Project DefaultTargets="Build" ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <ItemGroup Label="ProjectConfigurations">
    <ProjectConfiguration Include="{build_type}|{vcxproj_platform}">
      <Configuration>{build_type}</Configuration>
      <Platform>{vcxproj_platform}</Platform>
    </ProjectConfiguration>
  </ItemGroup>
  <PropertyGroup Label="Globals">
    <ProjectGUID>{guid}</ProjectGUID>
    <ApplicationType>Windows Store</ApplicationType>
    <DefaultLanguage>en-US</DefaultLanguage>
    <ApplicationTypeRevision>10.0</ApplicationTypeRevision>
    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>
    <WindowsTargetPlatformVersion>10.0.10586.0</WindowsTargetPlatformVersion>
    <WindowsTargetPlatformMinVersion>10.0.10586.0</WindowsTargetPlatformMinVersion>
    <Keyword>Win32Proj</Keyword>
    <Platform>{vcxproj_platform}</Platform>
    <ProjectName>SDK_{platform}</ProjectName>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.Default.props" />
  <PropertyGroup Label="Configuration">
    <ConfigurationType>Utility</ConfigurationType>
    <UseOfMfc>false</UseOfMfc>
    <CharacterSet>Unicode</CharacterSet>
    <PlatformToolset>v140</PlatformToolset>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.props" />
  <ImportGroup Label="ExtensionSettings">
  </ImportGroup>
  <ImportGroup Label="PropertySheets">
    <Import Project="$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props" Condition="exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')" Label="LocalAppDataPlatform" />
  </ImportGroup>
  <PropertyGroup Label="UserMacros" />
  <PropertyGroup>
    <IntDir>$(Platform)\$(Configuration)\$(ProjectName)\</IntDir>
  </PropertyGroup>
  <ItemGroup>
    <CustomBuild Include="{current_path}/WORK/win10-{platform}/SDK_{platform}.rule">
      <Message>Building {platform} SDK</Message>
      <Command>setlocal
cd {current_path}/WORK/win10-{platform}/cmake
if %errorlevel% neq 0 goto :cmEnd
C:
if %errorlevel% neq 0 goto :cmEnd
cmake.exe --build . --config $(Configuration)
if %errorlevel% neq 0 goto :cmEnd
:cmEnd
endlocal &amp; call :cmErrorLevel %errorlevel% &amp; goto :cmDone
:cmErrorLevel
exit /b %1
:cmDone
if %errorlevel% neq 0 goto :VCEnd</Command>
      <AdditionalInputs>%(AdditionalInputs)</AdditionalInputs>
      <Outputs>{current_path}/WORK/win10-{platform}/Stamp/SDK-build</Outputs>
      <LinkObjects>false</LinkObjects>
    </CustomBuild>
  </ItemGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.targets" />
  <ImportGroup Label="ExtensionTargets">
  </ImportGroup>
</Project>
""".format(platform=platform, build_type=build_type, vcxproj_platform=vcxproj_platforms[platform], current_path=current_path, guid=guid)
        f = open("WORK/win10-{0}/SDK_{0}.vcxproj".format(platform), 'w')
        f.write(vcxproj)
        f.close()
        f = open("WORK/win10-{0}/SDK_{0}.rule".format(platform), 'w')
        f.close()
        sln_projects += \
"""Project("{{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}}") = "SDK_{platform}", "WORK\win10-{platform}\SDK_{platform}.vcxproj", "{project_guid}"
EndProject
""".format(platform=platform, project_guid=guids[platform])
        sln_confs += """\t\t{project_guid}.{build_type}|Win32.ActiveCfg = {build_type}|{vcxproj_platform}
\t\t{project_guid}.{build_type}|Win32.Build.0 = {build_type}|{vcxproj_platform}
""".format(project_guid=guids[platform], platform=platform, build_type=build_type, vcxproj_platform=vcxproj_platforms[platform])

    # Generate Visual Studio projects to create a nuget packages
    for target, version in builder_target:
        guid = '{' + str(uuid.uuid4()).upper() + '}'
        vcxproj = """<?xml version="1.0" encoding="UTF-8"?>
<Project DefaultTargets="Build" ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <ItemGroup Label="ProjectConfigurations">
    <ProjectConfiguration Include="{build_type}|Win32">
      <Configuration>{build_type}</Configuration>
      <Platform>Win32</Platform>
    </ProjectConfiguration>
  </ItemGroup>
  <PropertyGroup Label="Globals">
    <ProjectGUID>{guid}</ProjectGUID>
    <ApplicationType>Windows Store</ApplicationType>
    <DefaultLanguage>en-US</DefaultLanguage>
    <ApplicationTypeRevision>10.0</ApplicationTypeRevision>
    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>
    <WindowsTargetPlatformVersion>10.0.10586.0</WindowsTargetPlatformVersion>
    <WindowsTargetPlatformMinVersion>10.0.10586.0</WindowsTargetPlatformMinVersion>
    <Keyword>Win32Proj</Keyword>
    <Platform>Win32</Platform>
    <ProjectName>Nuget{target}</ProjectName>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.Default.props" />
  <PropertyGroup Label="Configuration">
    <ConfigurationType>Utility</ConfigurationType>
    <UseOfMfc>false</UseOfMfc>
    <CharacterSet>Unicode</CharacterSet>
    <PlatformToolset>v140</PlatformToolset>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.props" />
  <ImportGroup Label="ExtensionSettings">
  </ImportGroup>
  <ImportGroup Label="PropertySheets">
    <Import Project="$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props" Condition="exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')" Label="LocalAppDataPlatform" />
  </ImportGroup>
  <PropertyGroup Label="UserMacros" />
  <PropertyGroup>
    <IntDir>$(Platform)\$(Configuration)\$(ProjectName)\</IntDir>
  </PropertyGroup>
  <ItemGroup>
    <CustomBuild Include="{current_path}/WORK/NuGet{target}.rule">
      <Message>Generating NuGet package for {target}</Message>
      <Command>setlocal
cd {current_path}
if %errorlevel% neq 0 goto :cmEnd
C:
if %errorlevel% neq 0 goto :cmEnd
python.exe submodules/build/nuget.py -s OUTPUT -w WORK/NuGet{target} -v {version} -t {target} {platforms}
if %errorlevel% neq 0 goto :cmEnd
cd {current_path}/OUTPUT
if %errorlevel% neq 0 goto :cmEnd
{current_path}/WORK/nuget.exe pack {current_path}/WORK/NuGet{target}/{target}.nuspec
:cmEnd
endlocal &amp; call :cmErrorLevel %errorlevel% &amp; goto :cmDone
:cmErrorLevel
exit /b %1
:cmDone
if %errorlevel% neq 0 goto :VCEnd</Command>
      <AdditionalInputs>%(AdditionalInputs)</AdditionalInputs>
      <Outputs>{current_path}/WORK/NuGet-build</Outputs>
      <LinkObjects>false</LinkObjects>
    </CustomBuild>
  </ItemGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.targets" />
  <ImportGroup Label="ExtensionTargets">
  </ImportGroup>
</Project>
""".format(platforms=' '.join(selected_platforms), target=target, build_type=build_type, version=version, current_path=current_path, guid=guid)
        f = open("WORK/NuGet{target}.vcxproj".format(target=target), 'w')
        f.write(vcxproj)
        f.close()
        f = open("WORK/NuGet{target}.rule".format(target=target), 'w')
        f.close()
        project_dependencies = ""
        for platform in selected_platforms:
            project_dependencies += """\t\t{project_guid} = {project_guid}
""".format(project_guid=guids[platform])
        for sdk in other_sdks:
            project_dependencies += """\t\t{project_guid} = {project_guid}
""".format(project_guid=guids[sdk])
        other_sdks += [target]
        guids[target] = guid
        sln_projects += \
"""Project("{{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}}") = "Nuget{target}", "WORK\Nuget{target}.vcxproj", "{project_guid}"
\tProjectSection(ProjectDependencies) = postProject
{project_dependencies}\tEndProjectSection
EndProject
""".format(target=target, project_guid=guid, project_dependencies=project_dependencies)
        sln_confs += \
"""\t\t{project_guid}.{build_type}|Win32.ActiveCfg = {build_type}|Win32
\t\t{project_guid}.{build_type}|Win32.Build.0 = {build_type}|Win32
""".format(project_guid=guid, build_type=build_type)

    # Generate Visual Studio solution to build the SDK
    sln = """Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 14
VisualStudioVersion = 14.0.24720.0
MinimumVisualStudioVersion = 10.0.40219.1
{sln_projects}Global
\tGlobalSection(SolutionConfigurationPlatforms) = preSolution
\t\t{build_type}|Win32 = {build_type}|Win32
\tEndGlobalSection
\tGlobalSection(ProjectConfigurationPlatforms) = postSolution
{sln_confs}\tEndGlobalSection
\tGlobalSection(SolutionProperties) = preSolution
\t\tHideSolutionNode = FALSE
\tEndGlobalSection
EndGlobal
""".format(sln_projects=sln_projects, sln_confs=sln_confs, build_type=build_type)
    f = open('SDK.sln', 'w')
    f.write(sln)
    f.close()


def main(argv=None):
    basicConfig(format="%(levelname)s: %(message)s", level=INFO)

    if argv is None:
        argv = sys.argv
    argparser = argparse.ArgumentParser(
        description="Prepare build of Linphone and its dependencies.")
    argparser.add_argument(
        '-ac', '--build-all-codecs', help="Build all codecs including non-free. Final application must comply with their respective license (see README.md).", action='store_true')
    argparser.add_argument(
        '-c', '-C', '--clean', help="Clean a previous build instead of preparing a build.", action='store_true')
    argparser.add_argument(
        '-d', '--debug', help="Prepare a debug build, eg. add debug symbols and use no optimizations.", action='store_true')
    argparser.add_argument(
        '--disable-gpl-third-parties', help="Disable GPL third parties such as FFMpeg, x264.", action='store_true')
    argparser.add_argument(
        '-dv', '--debug-verbose', help="Activate ms_debug logs.", action='store_true')
    argparser.add_argument(
        '--enable-non-free-codecs', help="Enable non-free codecs such as OpenH264, MPEG4, etc.. Final application must comply with their respective license (see README.md).", action='store_true')
    argparser.add_argument(
        '-f', '--force', help="Force preparation, even if working directory already exist.", action='store_true')
    argparser.add_argument(
        '-lf', '--list-features', help="List optional features and their default values.", action='store_true', dest='list_features')
    argparser.add_argument(
        '-L', '--list-cmake-variables', help="(debug) List non-advanced CMake cache variables.", action='store_true', dest='list_cmake_variables')
    argparser.add_argument(
        '--target', action=TargetListAction, default='linphone', help="The target to build (default is 'linphone'). Can be one of: {0}.".format(', '.join([repr(target) for target in builder_targets])))
    argparser.add_argument(
        '-t', '--tunnel', help="Enable Tunnel.", action='store_true')
    argparser.add_argument(
        'platform', nargs='*', action=PlatformListAction, default=['all'], help="The platform to build for (default is 'all'). Space separated architectures in list: {0}.".format(', '.join([repr(platform) for platform in platforms])))

    args, additional_args2 = argparser.parse_known_args()

    additional_args = []

    if check_tools() != 0:
        return 1

    if args.debug_verbose is True:
        additional_args += ["-DENABLE_DEBUG_LOGS=YES"]
    if args.enable_non_free_codecs is True:
        additional_args += ["-DENABLE_NON_FREE_CODECS=YES"]
    if args.build_all_codecs is True:
        additional_args += ["-DENABLE_GPL_THIRD_PARTIES=YES"]
        additional_args += ["-DENABLE_NON_FREE_CODECS=YES"]
        additional_args += ["-DENABLE_AMRNB=YES"]
        additional_args += ["-DENABLE_AMRWB=YES"]
        additional_args += ["-DENABLE_G729=YES"]
        additional_args += ["-DENABLE_GSM=YES"]
        additional_args += ["-DENABLE_ILBC=YES"]
        additional_args += ["-DENABLE_ISAC=YES"]
        additional_args += ["-DENABLE_OPUS=YES"]
        additional_args += ["-DENABLE_SILK=YES"]
        additional_args += ["-DENABLE_SPEEX=YES"]
        additional_args += ["-DENABLE_FFMPEG=YES"]
        additional_args += ["-DENABLE_H263=YES"]
        additional_args += ["-DENABLE_H263P=YES"]
        additional_args += ["-DENABLE_MPEG4=YES"]
        additional_args += ["-DENABLE_OPENH264=YES"]
        additional_args += ["-DENABLE_VPX=YES"]
    if args.disable_gpl_third_parties is True:
        additional_args += ["-DENABLE_GPL_THIRD_PARTIES=NO"]

    if args.tunnel:
        if not os.path.isdir("submodules/tunnel"):
            info("Tunnel wanted but not found yet, trying to clone it...")
            p = Popen("git clone gitosis@git.linphone.org:tunnel.git submodules/tunnel".split(" "))
            p.wait()
            if p.returncode != 0:
                error("Could not clone tunnel. Please see http://www.belledonne-communications.com/voiptunnel.html")
                return 1
        warning("Tunnel enabled, disabling GPL third parties.")
        additional_args += ["-DENABLE_TUNNEL=ON", "-DENABLE_GPL_THIRD_PARTIES=OFF"]

    if args.target == 'ms2':
        args.target = 'ms2-plugins'
    additional_args += ["-DLINPHONE_BUILDER_TARGET=" + args.target]

    # User's options are priority upon all automatic options
    additional_args += additional_args2

    if args.list_features:
        list_features(args.debug, additional_args)
        return 0

    selected_platforms_dup = []
    for platform in args.platform:
        if platform == 'all':
            selected_platforms_dup += ['ARM', 'x64', 'x86']
        else:
            selected_platforms_dup += [platform]
    # Unify platforms but keep provided order
    selected_platforms = []
    for x in selected_platforms_dup:
        if x not in selected_platforms:
            selected_platforms.append(x)

    if os.path.isdir('WORK') and not args.clean and not args.force:
        warning("Working directory WORK already exists. Please remove it (option -C or -c) before re-executing CMake "
                "to avoid conflicts between executions, or force execution (option -f) if you are aware of consequences.")
        if os.path.isfile('Makefile'):
            Popen("make help-prepare-options".split(" "))
        return 0

    if not args.clean:
        download_nuget()

    for platform in selected_platforms:
        target = targets[platform]

        if args.clean:
            target.clean()
        else:
            retcode = prepare.run(target, args.debug, False, args.list_cmake_variables, args.force, additional_args)
            if retcode != 0:
                return retcode

    if args.clean:
        if os.path.isfile('SDK.sln'):
            os.remove('SDK.sln')
        if os.path.isfile('SDK.sdf'):
            os.remove('SDK.sdf')
    elif selected_platforms:
        generate_solution(args.debug, selected_platforms, args.target)
        gpl_disclaimer(selected_platforms)
        info("You can now build the SDK.sln Visual Studio Solution.")

    return 0

if __name__ == "__main__":
    sys.exit(main())

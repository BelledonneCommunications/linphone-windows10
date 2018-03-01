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
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
#
############################################################################

import argparse
import os
import re
import subprocess
import sys
import urllib
import uuid
from logging import error, warning, info
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

    def __init__(self, arch, generator_platform = None):
        prepare.Target.__init__(self, 'win10-' + arch)
        current_path = os.path.dirname(os.path.realpath(__file__))
        current_path = current_path.replace('\\', '/')
        self.generator = 'Visual Studio 15 2017'
        self.platform_name = generator_platform
        self.config_file = 'configs/config-win10.cmake'
        self.output = 'OUTPUT/win10-' + arch
        self.external_source_path = os.path.join(current_path, 'submodules')
        external_builders_path = os.path.join(current_path, 'cmake_builder')
        self.additional_args = [
                "-DLINPHONE_BUILDER_EXTERNAL_BUILDERS_PATH=" + external_builders_path
                ]
        self.additional_args += ['-DCMAKE_CROSSCOMPILING=YES']
        self.additional_args += ['-DCMAKE_SYSTEM_NAME=WindowsStore']
        self.additional_args += ['-DCMAKE_SYSTEM_VERSION=10.0']
        self.additional_args += ['-DCMAKE_SYSTEM_PROCESSOR=' + arch]
        self.additional_args += ['-DCMAKE_VS_INCLUDE_INSTALL_TO_DEFAULT_BUILD=TRUE']


class Win10X86Target(Win10Target):

    def __init__(self):
        Win10Target.__init__(self, 'x86')


class Win10X64Target(Win10Target):

    def __init__(self):
        Win10Target.__init__(self, 'x64', 'x64')


class Win10ARMTarget(Win10Target):

    def __init__(self):
        Win10Target.__init__(self, 'ARM', 'ARM')


windows10_targets = {
    'x86': Win10X86Target(),
    'x64': Win10X64Target(),
    'ARM': Win10ARMTarget()
}
linphone_builder_targets = ['linphone', 'ms2', 'bellesip']


class ComponentListAction(argparse.Action):
    def __call__(self, parser, namespace, value, option_string=None):
        if value:
            if value not in linphone_builder_targets:
                message = ("Invalid target: {0!r} (choose from {1}".format(
                    value, ', '.join([repr(target) for target in linphone_builder_targets])))
                raise argparse.ArgumentError(self, message)
            setattr(namespace, self.dest, value)


class Windows10Preparator(prepare.Preparator):

    def __init__(self, targets=windows10_targets):
        prepare.Preparator.__init__(self, targets)
        self.veryclean = True
        self.argparser.add_argument('-ac', '--all-codecs', help="Enable all codecs, including the non-free ones", action='store_true')
        self.argparser.add_argument('--component', action=ComponentListAction, default='linphone', help="The component to build (default is 'linphone'). Can be one of: {0}.".format(', '.join([repr(target) for target in linphone_builder_targets])))


    def parse_args(self):
        prepare.Preparator.parse_args(self)

        if self.args.all_codecs:
            self.additional_args += ["-DENABLE_GPL_THIRD_PARTIES=YES"]
            self.additional_args += ["-DENABLE_NON_FREE_CODECS=YES"]
            self.additional_args += ["-DENABLE_AMRNB=YES"]
            self.additional_args += ["-DENABLE_AMRWB=YES"]
            self.additional_args += ["-DENABLE_G729=YES"]
            self.additional_args += ["-DENABLE_GSM=YES"]
            self.additional_args += ["-DENABLE_ILBC=YES"]
            self.additional_args += ["-DENABLE_ISAC=YES"]
            self.additional_args += ["-DENABLE_OPUS=YES"]
            self.additional_args += ["-DENABLE_SILK=YES"]
            self.additional_args += ["-DENABLE_SPEEX=YES"]
            self.additional_args += ["-DENABLE_FFMPEG=YES"]
            self.additional_args += ["-DENABLE_H263=YES"]
            self.additional_args += ["-DENABLE_H263P=YES"]
            self.additional_args += ["-DENABLE_MPEG4=YES"]
            self.additional_args += ["-DENABLE_OPENH264=YES"]
            self.additional_args += ["-DENABLE_VPX=YES"]
            self.additional_args += ["-DENABLE_X264=NO"]

        self.linphone_builder_target = self.args.component
        if self.linphone_builder_target == 'ms2':
            self.linphone_builder_target = 'ms2plugins'
        self.additional_args += ["-DLINPHONE_BUILDER_TARGET=" + self.linphone_builder_target]

    def clean(self):
        prepare.Preparator.clean(self)
        if os.path.isfile('SDK.sln'):
            os.remove('SDK.sln')
        if os.path.isfile('SDK.sdf'):
            os.remove('SDK.sdf')
        if os.path.isdir('WORK') and not os.listdir('WORK'):
            os.rmdir('WORK')
        if os.path.isdir('OUTPUT') and not os.listdir('OUTPUT'):
            os.rmdir('OUTPUT')

    def prepare(self):
        self.download_nuget()
        return prepare.Preparator.prepare(self)

    def nuget_download_reporthook(self, count, block_size, total_size):
        percent = int(count * block_size * 100 / total_size)
        sys.stdout.write("\r-- Downloading nuget: %2d%%" % percent)
        if percent == 100:
            sys.stdout.write('\n')
        sys.stdout.flush()

    def download_nuget(self):
        if not os.path.isdir('WORK'):
            os.makedirs("WORK")
        if not os.path.isfile('WORK/nuget.exe'):
            urllib.urlretrieve("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", "WORK/nuget.exe", reporthook = self.nuget_download_reporthook)

    def git_version(self, path):
        proc = subprocess.Popen(["git", "describe", "--always"], cwd=path, shell=False, stdout=subprocess.PIPE)
        out, err = proc.communicate()
        out = out.strip()
        pos = out.find('-g')
        if pos == -1:
            return out
        else:
            return out[:out.find('-g')].replace('-', '.')

    def generate_vs_solution(self):
        current_path = os.path.dirname(os.path.realpath(__file__))
        current_path = current_path.replace('\\', '/')
        guids = {}
        sln_projects = ""
        sln_confs = ""
        other_sdks = []
        build_type = 'Debug' if self.args.debug else 'Release'
        builder_target = []
        if self.linphone_builder_target == 'linphone':
            linphone_version = self.git_version('submodules/linphone')
            builder_target = [
                ('LinphoneTesterSDK', linphone_version),
                ('LinphoneSDK', linphone_version),
            ]
        elif self.linphone_builder_target == 'ms2plugins':
            ms2_version = self.git_version('submodules/mediastreamer2')
            builder_target = [
                ('MS2TesterSDK', ms2_version),
            ]
        elif self.linphone_builder_target == 'bellesip':
            bellesip_version = self.git_version('submodules/belle-sip')
            builder_target = [
                ('BelleSipTesterSDK', bellesip_version),
            ]
        else:
            return

        vcxproj_platforms = {}
        for platform in self.args.target:
            if platform == 'x86':
                vcxproj_platforms[platform] = 'Win32'
            else:
                vcxproj_platforms[platform] = platform

        # Generate Visual Studio project to build the SDK for each platform
        for platform in self.args.target:
            guid = '{' + str(uuid.uuid4()).upper() + '}'
            guids[platform] = guid
            f = open("WORK/win10-{0}/cmake/ALL_BUILD.vcxproj".format(platform), 'r')
            all_build_content = f.read()
            f.close()
            m = re.search("ToolsVersion=\"(.*)\" xmlns", all_build_content)
            tools_version = m.group(1)
            m = re.search("<WindowsTargetPlatformVersion>(.*)</WindowsTargetPlatformVersion>", all_build_content)
            target_platform_version = m.group(1)
            m = re.search("<WindowsTargetPlatformMinVersion>(.*)</WindowsTargetPlatformMinVersion>", all_build_content)
            target_platform_min_version = m.group(1)
            m = re.search("<PlatformToolset>(.*)</PlatformToolset>", all_build_content)
            platform_toolset = m.group(1)
            arm = '\n    <WindowsSDKDesktopARMSupport>true</WindowsSDKDesktopARMSupport>' if platform == 'ARM' else ''
            vcxproj = """<?xml version="1.0" encoding="UTF-8"?>
<Project DefaultTargets="Build" ToolsVersion="{tools_version}" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
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
    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>{arm}
    <WindowsTargetPlatformVersion>{target_platform_version}</WindowsTargetPlatformVersion>
    <WindowsTargetPlatformMinVersion>{target_platform_min_version}</WindowsTargetPlatformMinVersion>
    <Keyword>Win32Proj</Keyword>
    <Platform>{vcxproj_platform}</Platform>
    <ProjectName>SDK_{platform}</ProjectName>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.Default.props" />
  <PropertyGroup Label="Configuration">
    <ConfigurationType>Utility</ConfigurationType>
    <UseOfMfc>false</UseOfMfc>
    <CharacterSet>Unicode</CharacterSet>
    <PlatformToolset>{platform_toolset}</PlatformToolset>
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
""".format(platform=platform, build_type=build_type, vcxproj_platform=vcxproj_platforms[platform], current_path=current_path, guid=guid, tools_version=tools_version, target_platform_version=target_platform_version, target_platform_min_version=target_platform_min_version, platform_toolset=platform_toolset, arm=arm)
            f = open("WORK/win10-{0}/SDK_{0}.vcxproj".format(platform), 'w')
            f.write(vcxproj)
            f.close()
            f = open("WORK/win10-{0}/SDK_{0}.rule".format(platform), 'w')
            f.close()
            sln_projects += \
"""Project("{{E8FB6309-B31E-4380-992C-BB1609B3EA00}}") = "SDK_{platform}", "WORK\win10-{platform}\SDK_{platform}.vcxproj", "{project_guid}"
EndProject
""".format(platform=platform, project_guid=guids[platform])
            sln_confs += """\t\t{project_guid}.{build_type}|Win32.ActiveCfg = {build_type}|{vcxproj_platform}
\t\t{project_guid}.{build_type}|Win32.Build.0 = {build_type}|{vcxproj_platform}
""".format(project_guid=guids[platform], platform=platform, build_type=build_type, vcxproj_platform=vcxproj_platforms[platform])

        # Generate Visual Studio projects to create a nuget packages
        for target, version in builder_target:
            guid = '{' + str(uuid.uuid4()).upper() + '}'
            vcxproj = """<?xml version="1.0" encoding="UTF-8"?>
<Project DefaultTargets="Build" ToolsVersion="{tools_version}" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
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
    <WindowsTargetPlatformVersion>{target_platform_version}</WindowsTargetPlatformVersion>
    <WindowsTargetPlatformMinVersion>{target_platform_min_version}</WindowsTargetPlatformMinVersion>
    <Keyword>Win32Proj</Keyword>
    <Platform>Win32</Platform>
    <ProjectName>Nuget{target}</ProjectName>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.Default.props" />
  <PropertyGroup Label="Configuration">
    <ConfigurationType>Utility</ConfigurationType>
    <UseOfMfc>false</UseOfMfc>
    <CharacterSet>Unicode</CharacterSet>
    <PlatformToolset>{platform_toolset}</PlatformToolset>
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
python.exe submodules/build/nuget.py -s OUTPUT -w WORK/NuGet{target} -cs CsWrapper -v {version} -t {target} {platforms}
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
""".format(platforms=' '.join(self.args.target), target=target, build_type=build_type, version=version, current_path=current_path, guid=guid, tools_version=tools_version, target_platform_version=target_platform_version, target_platform_min_version=target_platform_min_version, platform_toolset=platform_toolset)
            f = open("WORK/NuGet{target}.vcxproj".format(target=target), 'w')
            f.write(vcxproj)
            f.close()
            f = open("WORK/NuGet{target}.rule".format(target=target), 'w')
            f.close()
            project_dependencies = ""
            for platform in self.args.target:
                project_dependencies += """\t\t{project_guid} = {project_guid}
""".format(project_guid=guids[platform])
            project_dependencies += """\t\t{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC} = {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
"""
            for sdk in other_sdks:
                project_dependencies += """\t\t{project_guid} = {project_guid}
""".format(project_guid=guids[sdk])
            other_sdks += [target]
            guids[target] = guid
            sln_projects += \
"""Project("{{E8FB6309-B31E-4380-992C-BB1609B3EA00}}") = "Nuget{target}", "WORK\Nuget{target}.vcxproj", "{project_guid}"
\tProjectSection(ProjectDependencies) = postProject
{project_dependencies}\tEndProjectSection
EndProject
""".format(target=target, project_guid=guid, project_dependencies=project_dependencies)
            sln_confs += \
"""\t\t{project_guid}.{build_type}|Win32.ActiveCfg = {build_type}|Win32
\t\t{project_guid}.{build_type}|Win32.Build.0 = {build_type}|Win32
""".format(project_guid=guid, build_type=build_type)
            sln_confs += \
"""\t\t{project_guid}.{build_type}|Win32.ActiveCfg = {build_type}|Any CPU
\t\t{project_guid}.{build_type}|Win32.Build.0 = {build_type}|Any CPU
""".format(project_guid="{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", build_type=build_type)

        # Generate Visual Studio solution to build the SDK
        sln = """Microsoft Visual Studio Solution File, Format Version 12.00
MinimumVisualStudioVersion = 10.0.40219.1
{sln_projects}Project("{{E8FB6309-B31E-4380-992C-BB1609B3EA00}}") = "CsWrapper", "CsWrapper\CsWrapper.csproj", "{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"
\tProjectSection(ProjectDependencies) = postProject
{project_dependencies}\tEndProjectSection
EndProject
Global
\tGlobalSection(SolutionConfigurationPlatforms) = preSolution
\t\t{build_type}|Win32 = {build_type}|Win32
\tEndGlobalSection
\tGlobalSection(ProjectConfigurationPlatforms) = postSolution
{sln_confs}\tEndGlobalSection
\tGlobalSection(SolutionProperties) = preSolution
\t\tHideSolutionNode = FALSE
\tEndGlobalSection
EndGlobal
""".format(sln_projects=sln_projects, sln_confs=sln_confs, build_type=build_type, project_dependencies=project_dependencies)
        f = open('SDK.sln', 'w')
        f.write(sln)
        f.close()

        info("You can now build the SDK.sln Visual Studio Solution.")



def main():
    preparator = Windows10Preparator()
    preparator.parse_args()
    if preparator.check_environment() != 0:
        preparator.show_environment_errors()
        return 1
    return preparator.run()

if __name__ == "__main__":
    sys.exit(main())

@echo off
goto START

-------------------------------------------------------
 MakeReleaseZips.bat

 Makes the zips, msi's, and chm for the DotNetZip release content.

 created: Thu, 19 Jun 2008  22:17

 Time-stamp: <2010-January-07 01:33:43>

-------------------------------------------------------


:START

setlocal

set zipit=c:\dinoch\bin\zipit.exe
set stamp=%DATE% %TIME%
set stamp=%stamp:/=-%
set stamp=%stamp: =-%
set stamp=%stamp::=%


@REM get the version:
for /f "delims==" %%I in ('type SolutionInfo.cs ^| c:\utils\grep AssemblyVersion ^| c:\utils\sed -e "s/^.*(.\(.*\).).*/\1 /"') do set longversion=%%I

set version=%longversion:~0,3%
echo version is %version%

c:\.net3.5\msbuild.exe DotNetZip.sln /p:Configuration=Debug
c:\.net3.5\msbuild.exe DotNetZip.sln /p:Configuration=Release

call :CheckSign
if ERRORLEVEL 1 (exit /b %ERRORLEVEL%)

echo making release dir ..\releases\v%version%-%stamp%
mkdir ..\releases\v%version%-%stamp%

call :MakeHelpFile

call :MakeIntegratedHelpMsi

call :MakeDevelopersRedist

call :MakeRuntimeRedist

call :MakeZipUtils

call :MakeUtilsMsi

call :MakeRuntimeMsi

c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe .\clean.ps1

call :MakeSrcZip


goto :END



--------------------------------------------
:CheckSign

  @REM check the digital sig on the various DLLs

  SETLOCAL EnableExtensions EnableDelayedExpansion
  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Checking signatures...
  echo.

    set verbose=1
    set rcode=0
    set ccount=0
    set okcount=0
    set notsigned=0
    for /R %%D in (*.dll) do (
        call :BACKTICK pubkey c:\netsdk2.0\bin\sn.exe -q -T "%%D"
        set /a ccount=!ccount!+1
        If "!pubkey:~-44!"=="does not represent a strongly named assembly" (
            set /a notsigned=!notsigned!+1
            if %verbose% GTR 0 (echo !pubkey!)
        ) else (
            if %verbose% GTR 0 (echo %%D  !pubkey!)
            If /i "!pubkey:~-16!"=="edbe51ad942a3f5c" (
                set /a okcount=!okcount!+1
            ) else (
                set /a rcode=!rcode!+1
            )
        )
    )

    if %verbose% GTR 0 (
      echo Checked !ccount! files
      echo !notsigned! were not signed
      echo !okcount! were signed, with the correct key
      echo !rcode! were signed, with the wrong key
    )

    if !rcode! GTR 0 (
      echo.
      echo Some of the assemblies are incorrectly signed.
      exit /b !rcode!
    )
    if !okcount! LSS 67 (
      echo.
      echo There are not enough correctly signed assemblies.
      exit /b !okcount!
    )

  echo.
  echo.

  endlocal

goto :EOF
-------------------------------------------------------



--------------------------------------------
:MakeHelpFile

  @REM example output hedklp file name:  DotNetZipLib-v1.5.chm

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Invoking Sandcastle HFB to make the Compiled Help File
  echo.

  @REM "C:\Program Files\EWSoftware\Sandcastle Help File Builder\SandcastleBuilderConsole.exe" DotNetZip.shfb

  c:\.net3.5\msbuild.exe  /p:Configuration=Release   Help\Dotnetzip.shfbproj
  move Help\out\DotNetZipLib-v*.chm ..\releases\v%version%-%stamp%

goto :EOF
--------------------------------------------




--------------------------------------------
:MakeDevelopersRedist

  @REM example output zipfile name:  DotNetZipLib-DevKit-v1.5.zip

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Developer's redistributable zip...
  echo.

  set zipfile=DotNetZipLib-DevKit-v%version%.zip
  for %%f in (..\releases\v%version%-%stamp%\%zipfile%) do set rzipfile=%%~ff
  echo zipfile is %rzipfile%

  %zipit% %rzipfile%  -s Contents.txt "This is the Developer's Kit package for DotNetZip v%version%.  This package was packed %stamp%.  In this zip you will find Debug and Release DLLs for the various versions of the Ionic.Zip class library and the Ionic.Zlib class library.  There is a separate top-level folder for each distinct version of the DLL, and within those top-level folders there are Debug and Release folders.  In the Debug folders you will find a DLL, a PDB, and an XML file for the given library, while the Release folder will have just a DLL.  The DLL is the actual library (either Debug or Release flavor), the PDB is the debug information, and the XML file is the intellisense doc for use within Visual Studio.  There is also a .chm file, which is a viewable help file.  In addition you will find the MSI file for the VS2008-integrated help.  If you have any questions, please check the forums on http://www.codeplex.com/DotNetZip"  -s PleaseDonate.txt  "Don't forget: DotNetZip is donationware.  Please donate. It's for a good cause. http://cheeso.members.winisp.net/DotNetZipDonate.aspx"   Readme.txt License.txt

  %zipit% %rzipfile%  -d DotNetZip-v%version%   -s Readme.txt "DotNetZip Library Developer's Kit package,  v%version% packed %stamp%.  This is the DotNetZip library.  It includes the classes in the Ionic.Zip namespace as well as the classes in the Ionic.Zlib namespace. Use this library if you want to manipulate ZIP files within .NET applications."

  %zipit% %rzipfile%  -d DotNetZip-v%version%\Debug   -D "Zip Full DLL\bin\Debug"    Ionic.Zip.dll Ionic.Zip.XML Ionic.Zip.pdb
  %zipit% %rzipfile%  -d DotNetZip-v%version%\Release -D "Zip Full DLL\bin\Release"  Ionic.Zip.dll

  %zipit% %rzipfile%  -d DotNetZip-v%version%-Reduced  -s Readme.txt "DotNetZip Reduced Library, Developer's Kit package, v%version% packed %stamp%.   This is the reduced version of the DotNetZip library.  It includes the classes in the Ionic.Zip namespace as well as the classes in the Ionic.Zlib namespace.  The reduced library differs from the full library in that it lacks the ability to save self-Extracting archives (aka SFX files), and is much smaller than the full library."

  %zipit% %rzipfile%  -d DotNetZip-v%version%-Reduced\Debug   -D "Zip Reduced\bin\Debug"   Ionic.Zip.Reduced.dll Ionic.Zip.Reduced.pdb Ionic.Zip.Reduced.XML
  %zipit% %rzipfile%  -d DotNetZip-v%version%-Reduced\Release -D "Zip Reduced\bin\Release" Ionic.Zip.Reduced.dll

  %zipit% %rzipfile%  -d DotNetZip-v%version%-CompactFramework  -s Readme.txt  "DotNetZip CF Library v%version% packed %stamp%. This assembly is built for the Compact Framework v2.0 or later, and includes all the classes in the Ionic.Zip namespace, as well as all the classes in the Ionic.Zlib namespace. Use this library if you want to manipulate ZIP files in smart-device applications, and if you want to use ZLIB compression directly, or if you want to use the compressing stream classes like GZipStream, DeflateStream, or ZlibStream."


  %zipit% %rzipfile%  -d DotNetZip-v%version%-CompactFramework\Debug    -D "Zip CF Full DLL\bin\Debug"   Ionic.Zip.CF.dll Ionic.Zip.CF.pdb Ionic.Zip.CF.XML
  %zipit% %rzipfile%  -d DotNetZip-v%version%-CompactFramework\Release  -D "Zip CF Full DLL\bin\Release" Ionic.Zip.CF.dll

  %zipit% %rzipfile%  -d Zlib-v%version%  -s Readme.txt  "DotNetZlib v%version% packed %stamp%.  This is the Ionic.Zlib assembly; it includes only the classes in the Ionic.Zlib namespace. Use this library if you want to take advantage of ZLIB compression directly, or if you want to use the compressing stream classes like GZipStream, DeflateStream, or ZlibStream."


  %zipit% %rzipfile%  -d Zlib-v%version%\Debug    -D "Zlib\bin\Debug"    Ionic.Zlib.dll Ionic.Zlib.pdb Ionic.Zlib.XML
  %zipit% %rzipfile%  -d Zlib-v%version%\Release  -D "Zlib\bin\Release"  Ionic.Zlib.dll

  %zipit% %rzipfile%  -d Zlib-v%version%-CompactFramework  -s Readme.txt  "DotNetZlib CF v%version% packed %stamp%. This is the Ionic.Zlib library packaged for the .NET Compact Framework v2.0 or later.  Use this library if you want to take advantage of ZLIB compression directly from within Smart device applications, or if you want to use the compressing stream classes like GZipStream, DeflateStream, or ZlibStream."

  %zipit% %rzipfile%  -d Zlib-v%version%-CompactFramework\Debug    -D "Zlib CF\bin\Debug"    Ionic.Zlib.CF.dll Ionic.Zlib.CF.pdb Ionic.Zlib.CF.XML
  %zipit% %rzipfile%  -d Zlib-v%version%-CompactFramework\Release  -D "Zlib CF\bin\Release"  Ionic.Zlib.CF.dll

  %zipit% %rzipfile%  -d Examples\WScript -D "Zip Tests\resources"  VbsCreateZip-DotNetZip.vbs  VbsUnZip-DotNetZip.vbs  TestCheckZip.js

  %zipit% %rzipfile%  -d VS2008-IntegratedHelp  -s Readme.txt  "This MSI installs the DotNetZip help content into the VisualStudio Integrated help system. After installing this MSI, pressing F1 within Visual Studio, with your cursor on a type defined within the DotNetZip assembly, will open the appropriate help within Visual Studio."   -D Help-VS-Integrated\HelpIntegration\Debug DotNetZip-HelpIntegration.msi

  %zipit% %rzipfile%  -d Examples  -D "Examples"  -r+  "name != *.cache and name != *.*~ and name != *.suo and name != *.user and name != #*.*# and name != *.vspscc and name != Examples\*\*\bin\*.* and name != Examples\*\*\obj\*.* and name != Examples\*\bin\*.* and name != Examples\*\obj\*.*"

  cd ..\releases\v%version%-%stamp%
  for %%V in ("*.chm") do   %zipit% %zipfile%  %%V
  cd ..\..\DotNetZip

goto :EOF
--------------------------------------------



--------------------------------------------
:MakeRuntimeRedist

  @REM example output zipfile name:  DotNetZipLib-Runtime-v1.5.zip

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the user's redistributable zip...
  echo.

  set zipfile=DotNetZipLib-Runtime-v%version%.zip
  for %%f in (..\releases\v%version%-%stamp%\%zipfile%) do set rzipfile=%%~ff

  echo zipfile is %rzipfile%
  %zipit% %rzipfile%    -s Contents.txt "This is the redistributable package for DotNetZip v%version%.  Packed %stamp%. In this zip you will find a separate folder for each separate version of the DLL. In each folder there is a RELEASE build DLL, suitable for redistribution with your app. If you have any questions, please check the forums on http://www.codeplex.com/DotNetZip "   -s PleaseDonate.txt  "Don't forget: DotNetZip is donationware.  Please donate. It's for a good cause. http://cheeso.members.winisp.net/DotNetZipDonate.aspx"   Readme.txt License.txt

  %zipit% %rzipfile%  -d DotNetZip-v%version% -D "Zip Full DLL\bin\Release" -s Readme.txt  "DotNetZip Redistributable Library v%version% packed %stamp%"  Ionic.Zip.dll

  %zipit% %rzipfile%  -d DotNetZip-Reduced-v%version% -D "Zip Reduced\bin\Release" -s Readme.txt  "DotNetZip Reduced Redistributable Library v%version% packed %stamp%"  Ionic.Zip.Reduced.dll


  %zipit% %rzipfile%  -d zlib-v%version% -D "Zlib\bin\Release" -s Readme.txt  "DotNetZlib Redistributable Library v%version% packed %stamp%"  Ionic.Zlib.dll

  %zipit% %rzipfile%  -d DotNetZip-v%version%-CompactFramework -D "Zip CF Full DLL\bin\Release" -s Readme.txt "DotNetZip Library for .NET Compact Framework v%version% packed %stamp%"  Ionic.Zip.CF.dll

  %zipit% %rzipfile%   -d Zlib-v%version%-CompactFramework -D "Zlib CF\bin\Release"  -s Readme.txt  "DotNetZlib Library for .NET Compact Framework v%version% packed %stamp%"   Ionic.Zlib.CF.dll


goto :EOF
--------------------------------------------




--------------------------------------------
:MakeZipUtils

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Zip Utils zip...
  echo.

    set zipfile=DotNetZipUtils-v%version%.zip
    for %%f in (..\releases\v%version%-%stamp%\%zipfile%) do set rzipfile=%%~ff

    %zipit% %rzipfile%    -s Contents.txt "These are the DotNetZip utilities and tools, for DotNetZip v%version%.  Packed %stamp%."   -s PleaseDonate.txt  "Don't forget: DotNetZip is donationware.  Please donate. It's for a good cause. http://cheeso.members.winisp.net/DotNetZipDonate.aspx"   License.txt

    %zipit% %rzipfile%  -zc "Zip utilities v%version% packed %stamp%" -D Tools\ZipIt\bin\Release  Zipit.exe Ionic.Zip.dll
    %zipit% %rzipfile%  -D Tools\Unzip\bin\Release            Unzip.exe
    %zipit% %rzipfile%  -D Tools\ConvertZipToSfx\bin\Release  ConvertZipToSfx.exe
    %zipit% %rzipfile%  -D Tools\WinFormsApp\bin\Release      DotNetZip-WinFormsTool.exe


goto :EOF
--------------------------------------------



--------------------------------------------
:MakeUtilsMsi

  @REM example output zipfile name:   DotNetZipUtils-v1.8.msi

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Utils MSI...
  echo.

  c:\vs2008\Common7\ide\devenv.exe DotNetZip.sln /build release /project "Zip Utilities MSI"
  echo waiting for Setup\release\DotNetZipUtils.msi
  c:\dinoch\dev\dotnet\AwaitFile Setup\Release\DotNetZipUtils.msi
  move Setup\Release\DotNetZipUtils.msi ..\releases\v%version%-%stamp%\DotNetZipUtils-v%version%.msi

goto :EOF
--------------------------------------------




--------------------------------------------
:MakeIntegratedHelpMsi

  @REM example output zipfile name:  DotNetZip-HelpIntegration.msi

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Integrated help MSI...
  echo.

  c:\vs2008\Common7\ide\devenv.exe Help-VS-Integrated\HelpIntegration.sln  /build Debug  /project HelpIntegration
  echo waiting for Help-VS-Integrated\HelpIntegration\Debug\DotNetZip-HelpIntegration.msi
  c:\dinoch\dev\dotnet\AwaitFile Help-VS-Integrated\HelpIntegration\Debug\DotNetZip-HelpIntegration.msi
  @REM move  Help-VS-Integrated\HelpIntegration\Debug\DotNetZip-HelpIntegration.msi  ..\releases\v%version%-%stamp%\DotNetZip-HelpIntegration.msi

goto :EOF
--------------------------------------------



--------------------------------------------
:MakeRuntimeMsi

  @REM example output zipfile name:   DotNetZip-Runtime-v1.8.msi

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Runtime MSI...
  echo.

  c:\vs2008\Common7\ide\devenv.exe DotNetZip.sln /build release /project "RuntimeSetup MSI"
  echo waiting for RuntimeSetup\release\DotNetZipLib-Runtime.msi
  c:\dinoch\dev\dotnet\AwaitFile RuntimeSetup\release\DotNetZipLib-Runtime.msi
  move RuntimeSetup\release\DotNetZipLib-Runtime.msi ..\releases\v%version%-%stamp%\DotNetZipLib-Runtime-v%version%.msi

goto :EOF
--------------------------------------------




--------------------------------------------
:MakeSrcZip

  echo.
  echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++
  echo.
  echo Making the Source Zip...
  echo.

    @REM set zipfile=DotNetZip-src-v%version%.zip

    cd..
    @REM Delete any existing files
    for %%f in (DotNetZip-src-v*.zip) do del %%f

    c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe  DotNetZip\ZipSrc.ps1

    @REM edit in place to remove Ionic.pfx and Ionic.snk from the csproj files

    for %%f in (DotNetZip-src-v*.zip) do set actualFilename=%%f

    DotNetZip\EditCsproj.exe -z %actualFileName%

    @REM move DotNetZip-src-v*.zip  releases\v%version%-%stamp%
    move %actualFileName%  releases\v%version%-%stamp%

    cd DotNetZip

goto :EOF
--------------------------------------------


--------------------------------------------
:BACKTICK
    call :GET_CMDLINE %*
    set varspec=%1
    setlocal EnableDelayedExpansion
    for /f "usebackq delims==" %%I in (`%CMDLINE%`) do set output=%%I
    endlocal & set %varspec%=%output%
    goto :EOF

--------------------------------------------

--------------------------------------------
:GET_CMDLINE
    @REM given a set of params [0..n], sets CMDLINE to
    @REM the join of params[1..n]
    setlocal enableextensions EnableDelayedExpansion
    set PRIOR=
    set PARAMS=
    shift
    :GET_PARAMs_LOOP
    if [%1]==[] goto GET_PARAMS_DONE
    set PARAMS=%PARAMS% %1
    shift
    goto GET_PARAMS_LOOP
    :GET_PARAMS_DONE
    REM strip the first space
    set PARAMS=%PARAMS:~1%
    endlocal & set CMDLINE=%PARAMS%
    goto :EOF
--------------------------------------------



:END
@if exist c:\dinoch\dev\dotnet\pronounceword.exe (c:\dinoch\dev\dotnet\pronounceword.exe All Done > nul)
echo release zips are in releases\v%version%-%stamp%

endlocal




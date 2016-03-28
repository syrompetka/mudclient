REM @echo off
goto START

-------------------------------------------------------
 Deploy.cmd

 Copy the zip tools from the build output directory to the \bin directory.

 Sat, 21 Feb 2009  21:58

-------------------------------------------------------


:START
setlocal

set BinDirectory=\dinoch\bin
copy /y Examples\WinFormsApp\bin\debug\DotNetZip-WinFormsTool.exe        %BinDirectory%
copy /y Examples\WinFormsApp\bin\debug\DotNetZip-WinFormsTool.pdb        %BinDirectory%
copy /y Examples\Zipit\bin\debug\Zipit.exe                               %BinDirectory%
copy /y Examples\Zipit\bin\debug\Zipit.pdb                               %BinDirectory%
copy /y Examples\UnZip\bin\debug\UnZip.exe                               %BinDirectory%
copy /y Examples\UnZip\bin\debug\UnZip.pdb                               %BinDirectory%
copy /y Examples\UnZip\bin\debug\Ionic.Zip.dll                           %BinDirectory%
copy /y Examples\UnZip\bin\debug\Ionic.Zip.pdb                           %BinDirectory%

endlocal
:END




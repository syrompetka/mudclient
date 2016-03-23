@echo off
goto START
=======================================================
 Uninstall.cmd

 This is part of DotNetZip.

 Run this to uninstall the DotNetZip Utilities and Tools

 Sun, 30 Aug 2009  20:14
=======================================================

:START
@REM The uuid is the "ProductCode" in the Visual Studio setup project
@REM for the DotNetZip Utilities v1.9.1.x
%windir%\system32\msiexec /x {B03A4C9E-7387-40A5-A3F5-B37E8FBE0281}

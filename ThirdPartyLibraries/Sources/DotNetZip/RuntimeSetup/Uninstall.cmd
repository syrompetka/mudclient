@echo off
goto START
=======================================================
 Uninstall.cmd

 This is part of DotNetZip.

 Run this to uninstall the DotNetZip Runtime.

 Sun, 30 Aug 2009  20:14
=======================================================

START:
@REM {A9F6335C-4B6A-4A6A-BE3C-4FA0843319E2} is the "ProductCode" in the Visual Studio setup project
@REM for the DotNetZip Runtime
%windir%\system32\msiexec /x {A9F6335C-4B6A-4A6A-BE3C-4FA0843319E2}

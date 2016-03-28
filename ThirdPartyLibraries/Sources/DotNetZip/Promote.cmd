@echo off
goto START
-------------------------------------------------------
 Promote.cmd

 promotes the most recent release directory as a "Versioned" release directory.

 Fri, 07 Aug 2009  21:53

-------------------------------------------------------
:START
SETLOCAL 
echo %~p0
set origPath=%~p0
cd ..\releases
for /d %%d in (v*-*) do call :MaybePromoteDir  %%d

cd %origPath%
goto END



:MaybePromoteDir
  set arg=%1
  CHOICE /C YN /D Y /T 12 /M "Promote %arg% ?"
  if errorlevel 1 if not errorlevel 122 goto PROMOTE

  :SKIPIT1
  goto :EOF
  :PROMOTE
  
  set shortdir=%arg:~0,4%
  if not exist %shortdir% then goto RENAMEDIR
  :REMOVEJUNCTION
  junction -d %shortdir%

  :RENAMEDIR
  for %%x in (%arg%\DotNetZip-src-*.zip) do set tpath=%%~nx

  set version=%tpath:~14,9%
  echo %version%

  :NEWJUNCTION
  ren %arg% %version%
  junction %shortdir%  %version%

goto  :EOF

:END
ENDLOCAL

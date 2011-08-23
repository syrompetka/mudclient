@echo off
goto START

-------------------------------------------------------
 ChkPubKey.bat

 checks the public key on the various DLLs 

 Created Wed Sep 23 17:12:27 2009 

-------------------------------------------------------

:START
SETLOCAL EnableExtensions EnableDelayedExpansion
set verbose=0
if  _%1==_ goto MAIN

if "%1"=="-v" (
 set verbose=1
)
shift
if NOT _%1==_ goto USAGE


goto MAIN


-------------------------------------------------------
:MAIN
set rcode=0
set ccount=0
set notsigned=0
for /R %%D in (*.dll) do (
    call :BACKTICK pubkey c:\netsdk2.0\bin\sn.exe -q -T "%%D"
    set /a ccount=!ccount!+1
    If "!pubkey:~-44!"=="does not represent a strongly named assembly" (
        set /a notsigned=!notsigned!+1
        if %verbose% GTR 0 (echo !pubkey!)
    ) else (
        if %verbose% GTR 0 (echo %%D  !pubkey!)
        If /i NoT "!pubkey:~-16!"=="edbe51ad942a3f5c" (
            set /a rcode=!rcode!+1
        )
    )
)

if %verbose% GTR 0 (
  echo Checked !ccount! files 
  echo !notsigned! were not signed
  echo !rcode! were signed, with the wrong key
)
goto ALL_DONE

-------------------------------------------------------


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



--------------------------------------------
:USAGE
  echo usage:   ChkPubKey ^<arg^> [^<optionalarg^>]
  echo blah blah blah
  goto ALL_DONE

--------------------------------------------


:ALL_DONE

ENDLOCAL & exit /b %rcode%




// StoreExistingZipAssociation.js
//
// Store the existing default file association for .zip files, if it exists. 
// This is called from within the context of a Windows installer session.
//
// Sun, 30 Aug 2009  18:36
//
// ResetAssociations.js
//
// Reset file associations for .zip files, as necessary, when 
// DotNetZip is being uninstalled.
//
// Sun, 30 Aug 2009  18:36
//

/************************************************/
/* Message level                                */
/************************************************/
var msiMessageTypeFatalExit = 0x00000000;
var msiMessageTypeError = 0x01000000;
var msiMessageTypeWarning = 0x02000000;
var msiMessageTypeUser = 0x03000000;
var msiMessageTypeInfo = 0x04000000;
var msiMessageTypeActionStart = 0x08000000;
var msiMessageTypeProgress = 0x0A000000;
var msiMessageTypeActionData = 0x09000000;

/************************************************/
/* Button styles                                */
/************************************************/
var msiMessageTypeOk = 0;
var msiMessageTypeOkCancel = 1;
var msiMessageTypeAbortRetryIgnore = 2;
var msiMessageTypeYesNoCancel = 3;
var msiMessageTypeYesNo = 4;
var msiMessageTypeRetryCancel = 5;

/************************************************/
/* Default button                               */
/************************************************/
var msiMessageTypeDefault1 = 0x000;
var msiMessageTypeDefault2 = 0x100;
var msiMessageTypeDefault3 = 0x200;

/************************************************/
/* Return values                                */
/************************************************/
var msiMessageStatusError = -1;
var msiMessageStatusNone = 0;
var msiMessageStatusOk = 1;
var msiMessageStatusCancel = 2;
var msiMessageStatusAbort = 3;
var msiMessageStatusRetry = 4;
var msiMessageStatusIgnore = 5;
var msiMessageStatusYes = 6;
var msiMessageStatusNo = 7;


var verbose = false;

function DisplayMessageBox(message, options)
{
    if (options == null)
    {
        options = msiMessageTypeUser + msiMessageTypeOk + msiMessageTypeDefault1;
    }

    if (typeof(Session) == undefined)
    {
        WScript.Echo(message);
        if ((options & 0xF) == 1)
        {
            // ask: cancel?
        }
        return 0;
    }

    var record = Session.Installer.CreateRecord(1);
    record.StringData(0) = "[1]";
    record.StringData(1) = message;

    return Session.Message(options, record);
}


function DisplayDiagnostic(message)
{
    if (verbose== false) return 0;

    if (typeof(Session) == undefined)
    {
        WScript.Echo(message);
        return 0;
    }

    var options = msiMessageTypeUser + msiMessageTypeOk + msiMessageTypeDefault1;
    var record = Session.Installer.CreateRecord(1);
    record.StringData(0) = "[1]";
    record.StringData(1) = message;

    return Session.Message(options, record);
}


function mytrace(arg)
{
    if (verbose == false) return;
    // This just causes a regRead to be logged.
    // Then in PerfMon or RegMon, you can use it as a "trace"
    try
    {
        var junkTest = WSHShell.RegRead(regValue2 + arg);
    }
    catch (e2b)
    {
    }
}


// ==================================================================


function PreserveFileAssociation()
{
    // get and store the existing association for zip files, if any
    var WSHShell = new ActiveXObject("WScript.Shell");
    var regValue1 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Dino Chiesa\\DotNetZip Tools v1.9\\PriorZipAssociation";
    var regValue2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\CLASSES\\.zip\\";
    var selfId = "DotNetZip.zip.1";
    try 
    {
        var association = WSHShell.RegRead(regValue2);
        if (association != "")
        {
            DisplayDiagnostic("StoreExisting: Current assoc for .zip: " + association);
            if (association != selfId)
            {
                DisplayDiagnostic("StoreExisting: it is NOT DotNetZip.");
                // there is an association, and it is not DotNetZip
                WSHShell.RegWrite(regValue1, association);
            }
            else
            {
                DisplayDiagnostic("StoreExisting: it is DotNetZip.");
                // the existing association is for DotNetZip
                try
                {
                    var existing = WSHShell.RegRead(regValue1);
                    if (existing == "" || existing == selfId)
                    {
                        DisplayDiagnostic("StoreExisting: defaulting (0)");
                        WSHShell.RegWrite(regValue1, "CompressedFolder0");
                    }
                    else
                    {
                        // there already is a stored prior association.
                        // don't change it. 
                    }
                }
                catch (e1a)
                {
                    DisplayDiagnostic("StoreExisting: exception: " + e1a.message);
                    DisplayDiagnostic("StoreExisting: defaulting (1)");
                    WSHShell.RegWrite(regValue1, "CompressedFolder1");
                }
            }
        }
        else
        {
            // there is no default association for .zip files
            WSHShell.RegWrite(regValue1, "CompressedFolder2");
        }
    }
    catch (e1)
    {
        // the key doesn't exist (no app for .zip files at all)
        WSHShell.RegWrite(regValue1, "CompressedFolder3");
    }
}


function DeleteSelf()
{
    // all done - try to delete myself.
    try 
    {
        var fso = new ActiveXObject("Scripting.FileSystemObject");
        var scriptName = targetDir + "storeExistingZipAssociation.js";
        if (fso.FileExists(scriptName))
        {
            fso.DeleteFile(scriptName);
        }
    }
    catch (e2)
    {
    }
}



var parameters = Session.Property("CustomActionData").split("|"); 
var targetDir = parameters[0];
var checkBoxState = parameters[1];

DisplayDiagnostic("Checkbox state; " +  checkBoxState);

PreserveFileAssociation();
DeleteSelf();

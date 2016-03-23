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




function RestoreRegistry()
{
    var WSHShell = new ActiveXObject("WScript.Shell");

    // get the stored association for zip files, if any
    var regValue1 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Dino Chiesa\\DotNetZip Tools v1.9\\PriorZipAssociation";
    var regValue2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\.zip\\";
    var parkingLot = "__DeleteThis";

    try 
    {
        var currentAssociation = WSHShell.RegRead(regValue2);

        DisplayDiagnostic("Current assoc for .zip: " + currentAssociation);
        var phase = ""; 


        if (currentAssociation == "DotNetZip.zip.1")
            phase = "1";
        else if (currentAssociation == "")
            phase = "2";

        DisplayDiagnostic("phase " + phase);

        if (phase == "1" || phase=="2")
        {
            var priorAssociation = null;
            if (phase=="1")
                priorAssociation= WSHShell.RegRead(regValue1);
            else
                priorAssociation= WSHShell.RegRead(regValue2 + parkingLot);

            DisplayDiagnostic("prior assoc for .zip: " + priorAssociation);
            if (priorAssociation != "")
            {
                mytrace("A"+phase);
                try
                {
                    mytrace("B"+phase);
                    var stillInstalled = WSHShell.RegRead(regValue2 + "OpenWithProgIds\\" + priorAssociation);
                    // the value will be the empty string
                    DisplayDiagnostic("the prior app is still installed.");
                    mytrace("C");
                    if (phase=="1")
                        WSHShell.RegWrite(regValue2 + parkingLot, priorAssociation );
                    else 
                    {
                        WSHShell.RegWrite(regValue2, priorAssociation );
                        WSHShell.RegDelete(regValue2 + parkingLot);
                    }
                }
                catch (e2a)
                {
                    mytrace("F");
                    DisplayDiagnostic("the prior app is NOT still installed.");
                    WSHShell.RegWrite(regValue2, "CompressedFolder");
                }
            }
            else
            {
                mytrace("G");
                DisplayDiagnostic("the prior assoc is empty?");
                WSHShell.RegWrite(regValue2, "CompressedFolder");
            }
        }
        else
        {
            DisplayDiagnostic("the associated app has changed.");
            // The association has been changed since install of DotNetZip.
            // We won't try to reset it. 
        }
    }
    catch (e1)
    {
        DisplayDiagnostic("there is no associated app.");
        WSHShell.RegWrite(regValue2, "CompressedFolder");
    }
}



RestoreRegistry();

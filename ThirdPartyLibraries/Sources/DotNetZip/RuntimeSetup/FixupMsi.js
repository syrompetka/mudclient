// FixupMsi.js
//
// This is run as a post-build step on a Setup project, to  fix up the MSI, make the 
// text more human, larger fonts, etc. 
//
// usage: 
//   FixupMsi.js <msi-file>
//
// Thu, 10 Sep 2009  01:16
//


function UpdateText(formName, text, textName)
{
    sql = "UPDATE `Control` SET `Control`.`Text` = '" + text + "' "  +
        "WHERE `Control`.`Dialog_`='" + formName + "' AND `Control`.`Control`='" + textName + "'"
    view = database.OpenView(sql);
    view.Execute();
    view.Close();
}


// Constant values from Windows Installer
var msiOpenDatabaseModeTransact = 1;

var msiViewModifyInsert         = 1;
var msiViewModifyUpdate         = 2;
var msiViewModifyAssign         = 3;
var msiViewModifyReplace        = 4;
var msiViewModifyDelete         = 6;


if (WScript.Arguments.Length != 1)
{
    WScript.StdErr.WriteLine(WScript.ScriptName + ": Updates an MSI to fix up the text in the install wizard");
    WScript.StdErr.WriteLine("Usage: ");
    WScript.StdErr.WriteLine("  " + WScript.ScriptName + " <file>");
    WScript.Quit(1);
}

var filespec = WScript.Arguments(0);
WScript.Echo(WScript.ScriptName + " " + filespec);
var WshShell = new ActiveXObject("WScript.Shell");

var database = null;
try
{
    var installer = new ActiveXObject("WindowsInstaller.Installer");
    database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);
    // this will fail if Orca.exe has the same MSI already opened
}
catch (e1)
{
    WScript.Echo("Error: " + e1.message);
    for (var x in e1)
        WScript.Echo("e[" + x + "] = " + e1[x]);
}

if (database==null) 
{
    WScript.Quit(1);
}

var sql;
var view;
var record;

try
{
    WScript.Echo("Updating the Text on the wizard...");

    // TextStyle - make regular text larger
    // this is for buttons ?  I think
    sql = "UPDATE `TextStyle` SET `TextStyle`.`Size` = 10 "  +
        "WHERE `TextStyle`.`TextStyle`='VSI_MS_Sans_Serif13.0_0_0' "
    view = database.OpenView(sql);
    view.Execute();
    view.Close();

    // this is for dialog box text, I think
    sql = "UPDATE `TextStyle` SET `TextStyle`.`Size` = 10 "  +
        "WHERE `TextStyle`.`TextStyle`='VSI_MS_Shell_Dlg13.0_0_0' "
    view = database.OpenView(sql);
    view.Execute();
    view.Close();



    // The welcome screen
    UpdateText("WelcomeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\nthe [ProductName]", 
               "BannerText");
    UpdateText("ResumeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\nthe [ProductName]", 
               "BannerText");
    UpdateText("AdminResumeForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\nthe [ProductName]", 
               "BannerText");
    UpdateText("MaintenanceForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\nthe [ProductName]", 
               "BannerText");
    UpdateText("AdminMaintenanceForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Welcome to the Setup Wizard for\r\nthe [ProductName]", 
               "BannerText");

    UpdateText("WelcomeForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}This wizard will guide you through the steps required to install the [ProductName] on your computer.",
               "WelcomeText");



    // Maintenance form
    UpdateText("MaintenanceForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}Select whether you want to repair or remove the [ProductName].",
               "BodyText");

    UpdateText("AdminMaintenanceForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}Select whether you want to repair or remove the [ProductName].",
               "BodyText");



    // update the text for the radio buttons
    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}&Repair the [ProductName]' "  +
        "WHERE `RadioButton`.`Property`='MaintenanceForm_Action'  AND " +
        " `RadioButton`.`Value`='Repair'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();
    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}Re&move the [ProductName]' "  +
        "WHERE `RadioButton`.`Property`='MaintenanceForm_Action'  AND " +
        " `RadioButton`.`Value`='Remove'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();
    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}&Repair the [ProductName]' "  +
        "WHERE `RadioButton`.`Property`='AdminMaintenanceForm_Action'  AND " +
        " `RadioButton`.`Value`='Repair'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();
    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}Re&move the [ProductName]' "  +
        "WHERE `RadioButton`.`Property`='AdminMaintenanceForm_Action'  AND " +
        " `RadioButton`.`Value`='Remove'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();


    // Confirm installation 
    UpdateText("ConfirmInstallForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Ready to install...",
               "BannerText");

    UpdateText("ConfirmInstallForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}The installer is ready to install the [ProductName] on your computer.\r\n\r\nClick \"Next\" to start the installation.",
               "BodyText1");

    // Confirm removal
    UpdateText("ConfirmRemoveDialog",
               "{\\VSI_MS_Sans_Serif13.0_0_0}You have chosen to remove the [ProductName] from your computer. Are you sure you want to remove it?",

               "BodyText");


    // user exit 
    UpdateText("UserExitForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}You interrupted the installation...",
               "BannerText");

    UpdateText("UserExitForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}The installation was interrupted before the [ProductName] could be installed. You need to restart the installer to try again.", 
               "BodyTextInstall");

    // EULA
    UpdateText("EulaForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}Here`s the DotNetZip License.\r\nYou have to accept it to proceed.", 
              "BannerText");


    UpdateText("EulaForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}Please take a moment to read the license agreement now. If you accept the terms below, click \"I Accept\", then \"Next\". Otherwise click \"Cancel\".",
               "BodyText");

    // update the text for the radio buttons
    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}I &Accept' "  +
        "WHERE `RadioButton`.`Property`='EulaForm_Property'  AND " +
        " `RadioButton`.`Value`='Yes'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();

    sql = "UPDATE `RadioButton` SET `RadioButton`.`Text` = '{\\VSI_MS_Sans_Serif13.0_0_0}I &Do Not Accept' "  +
        "WHERE `RadioButton`.`Property`='EulaForm_Property'  AND " +
        " `RadioButton`.`Value`='No'";
    view = database.OpenView(sql);
    view.Execute();
    view.Close();



    // Folder Selection
    UpdateText("FolderForm", "{\\VSI_MS_Sans_Serif16.0_1_0}Select the Installation Folder...", 
               "BannerText");

    UpdateText("FolderForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}The installer will install the [ProductName] to the following folder.\r\n\r\nTo install in this folder, click \"Next\". To install to a different folder, enter it below or click \"Browse\".",
               "Body");


    // ProgressForm
    UpdateText("ProgressForm",
               "{\\VSI_MS_Sans_Serif16.0_1_0}Installing the [ProductName]",
               "InstalledBannerText");

    UpdateText("ProgressForm",
               "{\\VSI_MS_Sans_Serif16.0_1_0}Removing the [ProductName]",
               "RemoveBannerText");

    UpdateText("ProgressForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}This will take just a moment...",
               "InstalledBody");

    UpdateText("ProgressForm",
               "{\\VSI_MS_Sans_Serif13.0_0_0}This will take just a moment...",
               "RemoveedBody");



    // Complete  
    UpdateText("FinishedForm", 
               "{\\VSI_MS_Sans_Serif16.0_1_0}The installer for the\r\n[ProductName] is complete.",
               "BannerText");


    UpdateText("FinishedForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}The [ProductName] has been successfully removed.\r\n\r\nClick \"Close\" to exit.",
               "BodyTextRemove");

    UpdateText("FinishedForm", 
               "{\\VSI_MS_Sans_Serif13.0_0_0}The [ProductName] has been successfully installed.\r\n\r\nClick \"Close\" to exit.",
               "BodyText");



    database.Commit();

    WScript.Echo("done.");
}
catch(e)
{
    WScript.StdErr.WriteLine("Exception");
    for (var x in e)
        WScript.StdErr.WriteLine("e[" + x + "] = " + e[x]);
    WScript.Quit(1);
}



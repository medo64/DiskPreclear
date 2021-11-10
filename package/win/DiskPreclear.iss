#define AppName        GetStringFileInfo('..\..\bin\DiskPreclear.exe', 'ProductName')
#define AppVersion     GetStringFileInfo('..\..\bin\DiskPreclear.exe', 'ProductVersion')
#define AppFileVersion GetStringFileInfo('..\..\bin\DiskPreclear.exe', 'FileVersion')
#define AppCompany     GetStringFileInfo('..\..\bin\DiskPreclear.exe', 'CompanyName')
#define AppCopyright   GetStringFileInfo('..\..\bin\DiskPreclear.exe', 'LegalCopyright')
#define AppBase        LowerCase(StringChange(AppName, ' ', ''))
#define AppSetupFile   AppBase + '-' + AppVersion


[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppCompany}
AppPublisherURL=https://www.medo64.com/{#AppBase}/
AppCopyright={#AppCopyright}
VersionInfoProductVersion={#AppVersion}
VersionInfoProductTextVersion={#AppVersion}
VersionInfoVersion={#AppFileVersion}
DefaultDirName={userpf}\{#AppCompany}\{#AppName}
OutputBaseFilename={#AppSetupFile}
SourceDir=..\..\bin
OutputDir=..\dist
AppId=Medo64_DiskPreclear
CloseApplications="yes"
RestartApplications="no"
AppMutex=Medo64_DiskPreclear
UninstallDisplayIcon={app}\DiskPreclear.exe
AlwaysShowComponentsList=no
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
MergeDuplicateFiles=yes
MinVersion=0,6.0
PrivilegesRequired=lowest
ShowLanguageDialog=no
SolidCompression=yes
ChangesAssociations=no
DisableWelcomePage=yes
LicenseFile=../package/win/License.rtf


[Messages]
SetupAppTitle={#AppName} {#AppVersion}
SetupWindowTitle={#AppName} {#AppVersion}
BeveledLabel=medo64.com

[Files]
Source: "DiskPreclear.exe";                            DestDir: "{app}";  Flags: ignoreversion;
Source: "DiskPreclear.pdb";                            DestDir: "{app}";  Flags: ignoreversion;
Source: "..\README.md";      DestName: "README.txt";   DestDir: "{app}";  Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;
Source: "..\LICENSE.md";     DestName: "LICENSE.txt";  DestDir: "{app}";  Flags: overwritereadonly uninsremovereadonly;  Attribs: readonly;

[Icons]
Name: "{userstartmenu}\DiskPreclear";  Filename: "{app}\DiskPreclear.exe"

[Run]
Description: "Launch application now";  Filename: "{app}\DiskPreclear.exe";  Flags: postinstall nowait skipifsilent runasoriginaluser shellexec
Description: "View ReadMe.txt";         Filename: "{app}\ReadMe.txt";        Flags: postinstall nowait skipifsilent runasoriginaluser shellexec unchecked


[Code]

procedure InitializeWizard;
begin
  WizardForm.LicenseAcceptedRadio.Checked := True;
end;


function PrepareToInstall(var NeedsRestart: Boolean): String;
var
    ResultCode: Integer;
begin
    Exec(ExpandConstant('{sys}\taskkill.exe'), '/f /im DiskPreclear.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Result := Result;
end;

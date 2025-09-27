[Setup]
AppName=VlcTracker
AppVersion=1.0.0
AppPublisher=Marcin Skic
AppPublisherURL=https://skic.me
DefaultDirName={autopf}\VlcTracker
DefaultGroupName=VlcTracker
SetupIconFile=Assets\logo.ico
UninstallDisplayIcon={app}\VlcTracker.exe
OutputBaseFilename=VlcTrackerInstaller
Compression=lzma
SolidCompression=yes
WizardStyle=modern

PrivilegesRequired=admin

; Add this flag if more than one file will be created in the future recursesubdirs
[Files]
Source="bin\Release\net9.0\win-x64\publish\VlcTracker.Service.exe"; DestDir="{app}"; DestName="VlcTracker.Exe" Flags: ignoreversion

[Icons]
; Start Menu shortcut
Name="{group}\VlcTracker"; Filename="{app}\VlcTracker.exe"; Tasks: groupicon
; Desktop shortcut (optional)
Name="{commondesktop}\VlcTracker"; Filename="{app}\VlcTracker.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "groupicon"; Description: "Create a &start menu shortcut"; GroupDescription: "Additional icons:";

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "VlcTracker"; ValueData: """{app}\VlcTracker.exe"""; Flags: uninsdeletevalue

[Run]
; Launch app after install (optional)
Filename="{app}\VlcTracker.exe"; Description="Launch VlcTracker"; Flags: nowait postinstall skipifsilent
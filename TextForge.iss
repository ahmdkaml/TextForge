#define MyAppName "TextForge"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "ahmdkaml"
#define MyAppExeName "TextForge.Desktop.exe"

[Setup]
AppId={{92466b42-df61-4261-a6bb-e91c8085c702}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\Programs\TextForge
DefaultGroupName=TextForge
PrivilegesRequired=lowest
OutputDir=publish
OutputBaseFilename=TextForgeSetup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}
SetupIconFile=Assets\icons\favicon.ico

[Files]
Source: "dist\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{autoprograms}\TextForge"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\TextForge"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"

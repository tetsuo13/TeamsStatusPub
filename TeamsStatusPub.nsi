; Set in the CI build.
!ifndef VERSION
!define VERSION 1.0.0
!endif

!ifndef PUBLISH_DIR
!define PUBLISH_DIR publish
!endif

!ifndef SETUP_DIR
!define SETUP_DIR output
!endif

!echo "Creating subdirectory '${SETUP_DIR}' for installer file..."
!system "MKDIR ${SETUP_DIR}"

Unicode true
SetCompressor /SOLID /FINAL lzma
ManifestDPIAware true
RequestExecutionLevel user

; Used to calculate application size.
!include "FileFunc.nsh"

!define APPNAME "TeamsStatusPub"
!define COMPANY_NAME "Andrei Nicholson"
!define /date TODAY "%Y%m%d"

Name "${APPNAME}"
OutFile "${SETUP_DIR}\${APPNAME}-${VERSION}-setup.exe"
InstallDir $LOCALAPPDATA\Programs\${APPNAME}

VIAddVersionKey "ProductName" "${APPNAME}"
VIAddVersionKey "CompanyName" "${COMPANY_NAME}"
VIAddVersionKey "LegalCopyright" "Copyright (c) 2022-2024 ${COMPANY_NAME}"
VIAddVersionKey "FileDescription" "${APPNAME} ${VERSION} Installer"
VIAddVersionKey "FileVersion" "${VERSION}"
VIAddVersionKey "ProductVersion" "${VERSION}"

VIProductVersion "${VERSION}"
VIFileVersion "${VERSION}"

Page directory
Page instfiles

Section Install
    SetOutPath $INSTDIR

    !echo "Stop program if running..."
    nsExec::Exec "taskkill /F /IM ${APPNAME}.exe"

    !echo "Wait for program to stop running..."
    Sleep 500

    File "${PUBLISH_DIR}\${APPNAME}.exe"
    File "${PUBLISH_DIR}\av_libglesv2.dll"
    File "${PUBLISH_DIR}\libHarfBuzzSharp.dll"
    File "${PUBLISH_DIR}\libSkiaSharp.dll"

    ; Don't overwrite settings file when upgrading.
    SetOverwrite off
    File "src\${APPNAME}\appsettings.json"
    SetOverwrite on

    WriteUninstaller $INSTDIR\uninstall.exe

    ; Create Start Menu launcher
    CreateShortCut "$SMPROGRAMS\${APPNAME}.lnk" "$INSTDIR\${APPNAME}.exe"

    ; Uninstall information to Add/Remove Programs
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallDate" "${TODAY}"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$\"$INSTDIR\${APPNAME}.exe$\""
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "${COMPANY_NAME}"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "HelpLink" "https://github.com/tetsuo13/TeamsStatusPub/issues"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLUpdateInfo" "https://github.com/tetsuo13/TeamsStatusPub/releases"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "https://github.com/tetsuo13/TeamsStatusPub"
    WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" "${VERSION}"
    WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" "1"
    WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" "1"

    ; Get total application size for displaying in Control Panel
    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
    WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "EstimatedSize" "$0"
SectionEnd

Section Uninstall
    ; Delete Start Menu launcher.
    Delete "$SMPROGRAMS\${APPNAME}.lnk"

    ; Delete all application files.
    Delete $INSTDIR\${APPNAME}.exe
    Delete $INSTDIR\*.dll
    Delete $INSTDIR\appsettings.json
    Delete $INSTDIR\uninstall.exe
    Delete $INSTDIR\*.log

    RMDir $INSTDIR

    DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
SectionEnd

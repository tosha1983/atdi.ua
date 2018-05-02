unit uAbout;

interface

uses Windows, Classes, Graphics, Forms, Controls, StdCtrls,
  Buttons, ExtCtrls;

type
  TdlgAboutBox = class(TForm)
    Panel1: TPanel;
    OKButton: TButton;
    ProgramIcon: TImage;
    lblProductName: TLabel;
    lblCopyright: TLabel;
    panTeam: TPanel;
    Bevel1: TBevel;
    lblMemory: TLabel;
    lblOsVer: TLabel;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label6: TLabel;
    Label7: TLabel;
    Label8: TLabel;
    Label9: TLabel;
    lblComments: TLabel;
    lblProductVersion: TLabel;
    Label10: TLabel;
    lblFileVersion: TLabel;
    procedure FormShow(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure OKButtonClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  dlgAboutBox: TdlgAboutBox;

implementation

uses SysUtils;

const

  notDetected = '[не определено]';

{$R *.DFM}

procedure TdlgAboutBox.FormShow(Sender: TObject);
var
    OSversion: OSVERSIONINFO;
    memStat: MEMORYSTATUS;
    BuffSize: cardinal;
    VISize:   cardinal;
    VIBuff:   pointer;
    trans:    pointer;
    temp: integer;
    str: pchar;
    fn: array[0..2048] of char;
    LangCharSet: string;
    LanguageInfo: string;

  function GetStringValue(const From: string): string;
  begin
    VerQueryValue(VIBuff,pchar('\StringFileInfo\'+LanguageInfo+'\'+From), pointer(str),
                  buffsize);
    if buffsize > 0 then Result := str else Result := 'n/a';
  end;
  
begin

    GetModuleFileName(0, fn, 2048);

    VISize := GetFileVersionInfoSize(fn,buffsize);
    if VISize >= 1 then begin
        VIBuff := AllocMem(VISize);
        GetFileVersionInfo(fn,cardinal(0),VISize,VIBuff);

        VerQueryValue(VIBuff,'\VarFileInfo\Translation',Trans,buffsize);
        if buffsize >= 4 then begin
            temp:=0;
            StrLCopy(@temp, pchar(Trans), 2);
            LangCharSet:=IntToHex(temp, 4);
            StrLCopy(@temp, pchar(Trans)+2, 2);
            LanguageInfo := LangCharSet+IntToHex(temp, 4);
        end;

        lblProductName.Caption := GetStringValue('ProductName');
        lblFileVersion.Caption := GetStringValue('FileVersion');
        lblProductVersion.Caption := 'Версия ' + GetStringValue('ProductVersion');
        lblCopyright.Caption := GetStringValue('CompanyName') + ' ' + GetStringValue('LegalCopyright');
        lblComments.Caption := GetStringValue('Comments');

        FreeMem(VIBuff,VISize);
    end;

    OSversion.dwOSVersionInfoSize := SizeOf(OSVERSIONINFO);
    if GetVersionEx(OSversion) then begin
        case (OSversion.dwPlatformId) of
            VER_PLATFORM_WIN32s:
                lblOsVer.Caption := lblOsVer.Caption + 'Windows 3.1 ';
            VER_PLATFORM_WIN32_WINDOWS:
                lblOsVer.Caption := lblOsVer.Caption + 'Windows 95 [' +
                          IntToStr(LOWORD(OSversion.dwBuildNumber)) + '] ';
            VER_PLATFORM_WIN32_NT:
                lblOsVer.Caption := lblOsVer.Caption + 'Windows NT ' +
                          IntToStr(OSversion.dwMajorVersion) + '.' +
                          IntToStr(OSversion.dwMinorVersion) +
                          ' Build ' + IntToStr(OSversion.dwBuildNumber) + ' ';
        end;
        if OSversion.szCSDVersion[0] <> #0 then
            lblOsVer.Caption := lblOsVer.Caption +
                          '(' + OSversion.szCSDVersion + ')';
    end else
        lblOsVer.Caption := lblOsVer.Caption + notDetected;
    memStat.dwLength := SizeOf(memStat);
    GlobalMemoryStatus(memStat);
    Label5.Caption := IntToStr(memStat.dwMemoryLoad) +'%';
    Label6.Caption := IntToStr(memStat.dwTotalPhys div 1024) +' K';
    Label7.Caption := IntToStr(memStat.dwAvailPhys div 1024) +' K';
    Label8.Caption := IntToStr(memStat.dwTotalPageFile div 1024) +' K';
    Label9.Caption := IntToStr(memStat.dwAvailPageFile div 1024) +' K';
end;

procedure TdlgAboutBox.FormClose(Sender: TObject; var Action: TCloseAction);
begin
//  Action := caFree;
end;

procedure TdlgAboutBox.OKButtonClick(Sender: TObject);
begin
  Close;
end;

end.


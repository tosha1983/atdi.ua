unit uEditBases;

interface

uses Windows, SysUtils, Classes, Graphics, Forms, Controls, StdCtrls, 
  Buttons, ExtCtrls, Dialogs;

type
  TdlgEditBase = class(TForm)
    OKBtn: TButton;
    CancelBtn: TButton;
    Label1: TLabel;
    Label2: TLabel;
    edtPath: TEdit;
    edtTitle: TEdit;
    Button1: TButton;
    OpenDialog1: TOpenDialog;
    Panel1: TPanel;
    Label3: TLabel;
    edtUser_name: TEdit;
    Label4: TLabel;
    edtPassword: TEdit;
    procedure Button1Click(Sender: TObject);
    procedure OKBtnClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  dlgEditBase: TdlgEditBase;

implementation

{$R *.DFM}

procedure TdlgEditBase.Button1Click(Sender: TObject);
begin
    if OpenDialog1.Execute then
        edtPath.Text := OpenDialog1.FileName;
end;

procedure TdlgEditBase.OKBtnClick(Sender: TObject);
begin
    if edtTitle.Text = '' then begin
        ModalResult := mrNone;
        edtTitle.SetFocus;
        raise Exception.Create('”кажите псевдоним базы');
    end;
end;

end.


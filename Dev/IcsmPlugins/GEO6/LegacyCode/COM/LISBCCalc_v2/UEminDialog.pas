unit UEminDialog;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Math;

type
  TEminDialog = class(TForm)
    Ok: TButton;
    Button2: TButton;
    ListBox1: TListBox;
    Edit1: TEdit;
    Label1: TLabel;
    Label2: TLabel;
    Label3: TLabel;
    CheckBox1: TCheckBox;
    CheckBox2: TCheckBox;
    procedure ListBox1Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure CheckBox2Click(Sender: TObject);
    procedure CheckBox1Click(Sender: TObject);
    procedure Edit1Exit(Sender: TObject);
  private
    { Private declarations }
    _emin: double;
    _f: double;
    procedure SetEminPlusF(emin: double);
    procedure SetEmin(emin: double);
    procedure SetFreq(f: double);
    procedure Recalc;

  public
    { Public declarations }
    property Emin: double read _emin;
    property Frequency: double read _f write SetFreq;
  end;



var
  EminDialog: TEminDialog;

implementation

{$R *.dfm}

procedure TEminDialog.ListBox1Click(Sender: TObject);
var emin: double;
begin
  case ListBox1.ItemIndex of
    0: emin := 46;
    1: emin := 49;
    2: emin := 12;
    3: emin := 17;
    4: emin := 21;
    5: emin := 23;
    6: emin := 25;
  else
    emin := 46;
  end;
  Edit1.Text := FloatToStr(emin);
  Recalc;
end;

procedure TEminDialog.SetEmin(emin: double);
begin
  _emin := emin;
  Recalc;
end;

procedure TEminDialog.SetEminPlusF(emin: double);
begin
end;

procedure TEminDialog.SetFreq(f: double);
begin
  _f := f;
  Recalc;
end;

procedure TEminDialog.FormCreate(Sender: TObject);
begin
  _f := 420;
  _emin := 46;
  Edit1.Text := FloatToStr(_emin);
  Recalc;
end;

procedure TEminDialog.Recalc;
var e: double;
    k: double;
begin
  e := StrToFloat(Edit1.Text);
  if CheckBox2.Checked then k := 30 else k := 20;

  if CheckBox1.Checked then
  begin
   if _f >= 470
       then _emin := e + k * Log10(_f/650)
       else _emin := e + k * Log10(_f/200)
  end else _emin := e;

  Label3.Caption := FormatFloat('#.##', _emin);
end;

procedure TEminDialog.CheckBox2Click(Sender: TObject);
begin
  Recalc;
end;

procedure TEminDialog.CheckBox1Click(Sender: TObject);
begin
 Recalc;
end;

procedure TEminDialog.Edit1Exit(Sender: TObject);
begin
  Recalc;
end;

end.

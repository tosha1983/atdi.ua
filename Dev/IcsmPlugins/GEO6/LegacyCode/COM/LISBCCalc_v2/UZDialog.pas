unit UZDialog;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, UZRoutine, UShare;

type
  TZDialog = class(TForm)
    Memo1: TMemo;
    Edit1: TEdit;
    Edit2: TEdit;
    Button1: TButton;
    Button2: TButton;
    ListBox1: TListBox;
    Edit3: TEdit;
    Edit4: TEdit;
    Button3: TButton;
    Edit5: TEdit;
    Button4: TButton;
    Label1: TLabel;
    Label2: TLabel;
    Edit6: TEdit;
    Edit7: TEdit;
    Label3: TLabel;
    Label4: TLabel;
    CheckBox1: TCheckBox;
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure FormActivate(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button4Click(Sender: TObject);
    procedure ListBox1KeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
  private
    { Private declarations }
    _z: TDVBZone;
    
  public
    procedure SetZ(z: TDVBZone);
    procedure Refresh;
  published


    { Public declarations }
  end;


implementation

{$R *.dfm}

procedure TZDialog.Button1Click(Sender: TObject);
var l: TStringList;
    i: integer;
begin
  l := TStringList.Create;
  for i := 0 to Memo1.Lines.Count-1 do l.Add(Memo1.Lines[i]);
  _z.ClearPoints;
  _z.LoadFromStringList(l);

  _z.ClearChannels(ckAssigned);

  l.Clear;
  for i := 0 to ListBox1.Items.Count-1 do l.Add(ListBox1.Items[i]);
  _z.LoadChannelsFromStringList(l, ckAssigned);

//  _z.ClearChannels(ckPlanned);

//  _z.AddChannel(StrToInt(Edit1.text), ckAssigned);
//  _z.AddChannel(StrToInt(Edit2.text), ckPlanned);


  _z.Name := Edit6.Text;
  _z.MaxDist := StrToInt(Edit7.Text);
  _z.AvailableForPlanning := CheckBox1.Checked;
  l.Free;


  ModalResult := mrOk;
end;

procedure TZDialog.Button2Click(Sender: TObject);
begin
  ModalResult := mrCancel;
end;

procedure TZDialog.Refresh;
var i: integer;
begin
  for i := 0 to _z.GetPointsCount-1 do
  begin
    Memo1.Lines.Add(FloatToStrDecimalPoint(_z.GetLon(i)) + ' ' + FloatToStrDecimalPoint(_z.GetLat(i)));
  end;

  for i := 0 to _z.GetChannelsCount(ckAssigned)-1 do
  begin
//    ListBox1.Items.Add(IntToStr(_z.GetAssignedChannel(i)));
    ListBox1.Items.Add(_z.GetAssignedChannel(i));
  end;

//  Edit1.Text := IntToStr(_z.GetChannel(0, ckAssigned));
//  Edit2.Text := IntToStr(_z.GetChannel(0, ckPlanned));
  Edit6.text := _z.name;
  Edit7.Text := IntToStr(_z.MaxDist);
  CheckBox1.Checked := _z.AvailableForPlanning;

end;

procedure TZDialog.SetZ(z: TDVBZone);
begin
  _z := z;
end;

procedure TZDialog.FormActivate(Sender: TObject);
begin
  Refresh;
end;

procedure TZDialog.Button3Click(Sender: TObject);
begin
  Memo1.Lines.Add(Edit3.Text + ' ' + Edit4.Text);
end;

procedure TZDialog.Button4Click(Sender: TObject);
var ch: TChannel2;
begin
  ch := Edit5.Text;
  ListBox1.Items.Add(ch);
{
  ch := ch + 1;
  if ch > 69 then ch :=69;
  Edit5.Text := IntToStr(ch);
 } 
end;

procedure TZDialog.ListBox1KeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
begin
  if Key = VK_DELETE then
  begin
    if ListBox1.ItemIndex > -1 then ListBox1.Items.Delete(ListBox1.ItemIndex);

  end;
end;

end.

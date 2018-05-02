unit uConnect;

interface

uses Windows, SysUtils, Classes, Graphics, Forms, Controls, StdCtrls,
  Buttons, ComCtrls, IBDatabase;

type
  TdlgConnect = class(TForm)
    Label1: TLabel;
    edtPassword: TEdit;
    btnOk: TButton;
    btnCancel: TButton;
    lbxBases: TListBox;
    Label2: TLabel;
    btnAdd: TButton;
    btnDelete: TButton;
    btnChange: TButton;
    btnCreate: TButton;
    edtName: TEdit;
    edtRole: TEdit;
    Label3: TLabel;
    Label4: TLabel;
    StatusBar1: TStatusBar;
    procedure FormCreate(Sender: TObject);
    procedure lbxBasesClick(Sender: TObject);
    procedure btnAddClick(Sender: TObject);
    procedure btnDeleteClick(Sender: TObject);
    procedure btnChangeClick(Sender: TObject);
    procedure btnCreateClick(Sender: TObject);
    procedure btnOkClick(Sender: TObject);
    procedure FormShow(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
  private
    { Private declarations }
    TargetDB: TIBDatabase;
  public
    { Public declarations }
    sDatabaseRegPath: string;
    procedure Init(AppRegPath: string; DB: TIBDatabase);
  end;

var
  dlgConnect: TdlgConnect;

implementation

uses Registry, uEditBases;

{$R *.DFM}

procedure TdlgConnect.FormCreate(Sender: TObject);
begin
    Caption := Application.Title + ' - подключение к БД';
end;

procedure TdlgConnect.lbxBasesClick(Sender: TObject);
var
    reg: TRegistry;
begin
    reg := TRegistry.Create;
    StatusBar1.SimpleText := '';
    btnDelete.Enabled := False;
    btnChange.Enabled := False;

    if (lbxBases.Items.Count > 0) and (lbxBases.ItemIndex <> -1) then try
        btnDelete.Enabled := True;
        btnChange.Enabled := True;
        reg.Access := KEY_READ;
        reg.RootKey := HKEY_LOCAL_MACHINE;
        if reg.OpenKey(sDatabaseRegPath, False) then begin
            StatusBar1.SimpleText := reg.ReadString(lbxBases.Items[lbxBases.ItemIndex]);
            reg.CloseKey;
        end;
    finally
        reg.Free;
    end;
end;

procedure TdlgConnect.btnAddClick(Sender: TObject);
var
    reg: TRegistry;
begin
    if dlgEditBase = nil then
        dlgEditBase := TdlgEditBase.Create(Application);

    dlgEditBase.Caption := 'Новое описание базы';
    dlgEditBase.edtPath.Text := '';
    dlgEditBase.edtTitle.Text := '';
    dlgEditBase.Height := 150;
    dlgEditBase.edtPath.Enabled := True;
    dlgEditBase.Button1.Enabled := True;

    if dlgEditBase.ShowModal = mrOk then begin
        reg := TRegistry.Create;
        try
            reg.RootKey := HKEY_LOCAL_MACHINE;
            if reg.OpenKey(sDatabaseRegPath, true) then begin
                reg.WriteString(dlgEditBase.edtTitle.Text, dlgEditBase.edtPath.Text);
                reg.CloseKey;
            end else
                raise Exception.Create('Не удаётся открыть ключ реестра');
        finally
            reg.Free;
        end;
        lbxBases.Items.Add(dlgEditBase.edtTitle.Text);
        lbxBases.ItemIndex := lbxBases.Items.Count - 1;
        lbxBasesClick(Sender);
    end;
end;

procedure TdlgConnect.btnDeleteClick(Sender: TObject);
var
    reg: TRegistry;
    OldIndex: Integer;
begin
    if Application.MessageBox(PChar('Удалить описание базы ''' + lbxBases.Items[lbxBases.ItemIndex] + '''?'),
                                    PChar(Application.Title),
                                    MB_ICONQUESTION or MB_YESNO) = IDYES then begin
        OldIndex := lbxBases.ItemIndex;
        reg := TRegistry.Create;
        try
            reg.RootKey := HKEY_LOCAL_MACHINE;
            if reg.OpenKey(sDatabaseRegPath, true) then begin
                reg.DeleteValue(lbxBases.Items[lbxBases.ItemIndex]);
                reg.CloseKey;
                lbxBases.Items.Delete(lbxBases.ItemIndex);
                if lbxBases.Items.Count > 0 then
                    if OldIndex < lbxBases.Items.Count then
                        lbxBases.ItemIndex := OldIndex
                    else
                        lbxBases.ItemIndex := OldIndex - 1;
                lbxBasesClick(Sender);
            end else
                Application.MessageBox('Не удаётся открыть ключ реестра', PChar(Application.Title), MB_ICONHAND);
        finally
            reg.Free;
        end;
    end;
end;

procedure TdlgConnect.btnChangeClick(Sender: TObject);
var
    reg: TRegistry;
begin
    if dlgEditBase = nil then
        dlgEditBase := TdlgEditBase.Create(Application);

    dlgEditBase.Caption := 'Редактирование описания базы';
    dlgEditBase.edtPath.Text := StatusBar1.SimpleText;
    dlgEditBase.Height := 150;
    dlgEditBase.edtTitle.Text := lbxBases.Items[lbxBases.ItemIndex];
    dlgEditBase.edtPath.Enabled := True;
    dlgEditBase.Button1.Enabled := True;

    if dlgEditBase.ShowModal = mrOk then begin
        reg := TRegistry.Create;
        try
            reg.RootKey := HKEY_LOCAL_MACHINE;
            if reg.OpenKey(sDatabaseRegPath, true) then begin
                reg.DeleteValue(lbxBases.Items[lbxBases.ItemIndex]);
                reg.WriteString(dlgEditBase.edtTitle.Text, dlgEditBase.edtPath.Text);
                reg.CloseKey;
            end else
                raise Exception.Create('Не удаётся открыть ключ реестра');
        finally
            reg.Free;
        end;
        lbxBases.Items[lbxBases.ItemIndex] := dlgEditBase.edtTitle.Text;
        lbxBases.ItemIndex := lbxBases.Items.Count - 1;
        lbxBasesClick(Sender);
    end;
end;

procedure TdlgConnect.btnCreateClick(Sender: TObject);
var
    reg: TRegistry;
begin
    if dlgEditBase = nil then
        dlgEditBase := TdlgEditBase.Create(Application);

    dlgEditBase.Caption := 'Создание базы';
    dlgEditBase.edtPath.Text := '';
    dlgEditBase.edtPath.Enabled := True;
    dlgEditBase.Button1.Enabled := True;
    dlgEditBase.edtUser_name.Text := edtName.Text;
    dlgEditBase.edtPassword.Text := edtPassword.Text;
    dlgEditBase.Height := 210;
    dlgEditBase.Panel1.Visible := True;

    if dlgEditBase.ShowModal = mrOk then begin
        if TargetDB.Connected then
            TargetDB.Connected := False;

        TargetDB.Params.Clear;
        TargetDB.DatabaseName := dlgEditBase.edtPath.Text;
        TargetDB.Params.Add('USER '+#39+dlgEditBase.edtUser_name.Text+#39);
        TargetDB.Params.Add('PASSWORD '+#39+dlgEditBase.edtPassword.Text+#39);
        TargetDB.Params.Add('PAGE_SIZE 4096');
        TargetDB.Params.Add('DEFAULT CHARACTER SET WIN1251');
        TargetDB.SQLDialect := 3;
        TargetDB.CreateDatabase();
        TargetDB.Close();

        TargetDB.Params.Clear();
        TargetDB.Params.Add('password='+dlgEditBase.edtPassword.Text);
        TargetDB.Params.Add('user_name='+dlgEditBase.edtUser_name.Text);
        TargetDB.Params.Add('lc_ctype=WIN1251');

        edtPassword.Text := dlgEditBase.edtPassword.Text;
        edtName.Text := dlgEditBase.edtUser_name.Text;

        reg := TRegistry.Create;
        try
            reg.RootKey := HKEY_LOCAL_MACHINE;
            if reg.OpenKey(sDatabaseRegPath, true) then begin
                reg.WriteString(dlgEditBase.edtTitle.Text, dlgEditBase.edtPath.Text);
                reg.CloseKey;
            end else
                raise Exception.Create('Не удаётся открыть ключ реестра');
        finally
            reg.Free;
        end;
        lbxBases.Items.Add(dlgEditBase.edtTitle.Text);
        lbxBases.ItemIndex := lbxBases.Items.Count - 1;
        lbxBasesClick(Sender);
        dlgEditBase.Panel1.Visible := false;
        dlgEditBase.ClientHeight := 120;
        btnOkClick(Sender);
        ModalResult := mrYes;
    end
    else begin
        dlgEditBase.Panel1.Visible := false;
        dlgEditBase.ClientHeight := 120;
    end;
end;

procedure TdlgConnect.btnOkClick(Sender: TObject);
var
    reg: TRegistry;
begin
    if TargetDB.Connected then
        TargetDB.Connected := False;
    TargetDB.DatabaseName := StatusBar1.SimpleText;
    TargetDB.Params.Clear;
    TargetDB.Params.Add('user_name='+edtName.Text);
    TargetDB.Params.Add('lc_ctype=WIN1251');
    TargetDB.Params.Add('password='+edtPassword.Text);
    if edtRole.Text <> '' then
        TargetDB.Params.Add('sql_role_name='+edtRole.Text);
    try
        TargetDB.Connected := True;
        TargetDB.DefaultTransaction.StartTransaction;
    except
        on E: Exception do begin
            ModalResult := mrNone;
            edtPassword.Text := '';
            raise Exception.Create('Ошибка подключения к БД'+#13+E.Message);
        end
    end;

    reg := TRegistry.Create;
    try
        reg.RootKey := HKEY_LOCAL_MACHINE;
        if reg.OpenKey(sDatabaseRegPath + '\Defaults', True) then begin
            reg.WriteString('LastTitle', lbxBases.Items[lbxBases.ItemIndex]);
            reg.WriteString('LastUser', edtName.Text);
            reg.WriteString('LastRole', edtRole.Text);
            reg.CloseKey;
        end;
    finally
        reg.Free;
    end;
end;

procedure TdlgConnect.FormShow(Sender: TObject);
begin
    lbxBasesClick(Sender);
    if edtName.Text <> '' then begin
        edtPassword.SetFocus;
        edtPassword.Text := '';
    end;
end;

procedure TdlgConnect.FormClose(Sender: TObject; var Action: TCloseAction);
begin
    if Assigned (dlgEditBase) then
        dlgEditBase.Free;
    dlgEditBase := nil;
end;

procedure TdlgConnect.Init(AppRegPath: string; DB: TIBDatabase);
var
    reg: TRegistry;
    names: TStringList;
    i: Integer;
begin
    sDatabaseRegPath := AppRegPath + '\Databases';
    TargetDB := DB;

    lbxBases.Items.Clear;
    btnDelete.Enabled := False;
    btnChange.Enabled := False;
    edtName.Text := '';
    edtRole.Text := '';
    edtPassword.Text := '';
    reg := TRegistry.Create;
    names := TStringList.Create;
    try
        reg.Access := KEY_READ;
        reg.RootKey := HKEY_LOCAL_MACHINE;
        if reg.OpenKey(sDatabaseRegPath, False) then begin
            reg.GetValueNames(names);
            for i := 0 to names.Count - 1 do begin
                lbxBases.Items.Add(names[i]);
            end;
            reg.CloseKey;
            if lbxBases.Items.Count > 0 then
                lbxBases.ItemIndex := 0;
            lbxBasesClick(Self);
            if reg.OpenKey(sDatabaseRegPath + '\Defaults', False) then begin
                lbxBases.ItemIndex := lbxBases.Items.IndexOf(reg.ReadString('LastTitle'));
                edtName.Text := reg.ReadString('LastUser');
                edtRole.Text := reg.ReadString('LastRole');
                reg.CloseKey;
            end;
        end;
    finally
        reg.Free;
        names.Free;
    end;
end;

end.


object dlgEminAndNote: TdlgEminAndNote
  Left = 276
  Top = 135
  ActiveControl = edtEmin
  BorderStyle = bsDialog
  Caption = #1055#1072#1088#1072#1084#1077#1090#1088#1099' '#1085#1086#1074#1086#1075#1086' '#1082#1086#1085#1090#1091#1088#1072' '#1087#1086#1084#1077#1093
  ClientHeight = 123
  ClientWidth = 356
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    356
    123)
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 28
    Top = 19
    Width = 54
    Height = 13
    Caption = 'E min, dBu:'
  end
  object Label2: TLabel
    Left = 16
    Top = 59
    Width = 66
    Height = 13
    Caption = #1055#1088#1080#1084#1077#1095#1072#1085#1080#1077':'
  end
  object btnOk: TButton
    Left = 167
    Top = 92
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    Default = True
    TabOrder = 0
    OnClick = btnOkClick
  end
  object btnCancel: TButton
    Left = 255
    Top = 92
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 1
  end
  object edtEmin: TEdit
    Left = 88
    Top = 16
    Width = 81
    Height = 21
    ParentColor = True
    ReadOnly = True
    TabOrder = 2
  end
  object edtNote: TEdit
    Left = 88
    Top = 56
    Width = 249
    Height = 21
    TabOrder = 3
  end
end

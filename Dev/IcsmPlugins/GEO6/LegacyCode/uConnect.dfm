object dlgConnect: TdlgConnect
  Left = 138
  Top = 609
  BorderStyle = bsDialog
  Caption = 'Password Dialog'
  ClientHeight = 276
  ClientWidth = 353
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  OnClose = FormClose
  OnCreate = FormCreate
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 40
    Top = 197
    Width = 38
    Height = 13
    Caption = #1055'&'#1072#1088#1086#1083#1100
    FocusControl = edtPassword
  end
  object Label2: TLabel
    Left = 16
    Top = 8
    Width = 27
    Height = 13
    Caption = '&'#1041#1072#1079#1099
    FocusControl = lbxBases
  end
  object Label3: TLabel
    Left = 8
    Top = 149
    Width = 73
    Height = 13
    Caption = '&'#1055#1086#1083#1100#1079#1086#1074#1072#1090#1077#1083#1100
    FocusControl = edtName
  end
  object Label4: TLabel
    Left = 56
    Top = 173
    Width = 25
    Height = 13
    Caption = '&'#1056#1086#1083#1100
    FocusControl = edtRole
  end
  object edtPassword: TEdit
    Left = 88
    Top = 192
    Width = 161
    Height = 21
    PasswordChar = '*'
    TabOrder = 3
  end
  object btnOk: TButton
    Left = 152
    Top = 224
    Width = 97
    Height = 25
    Caption = #1055#1086#1076#1082#1083#1102#1095#1080#1090#1100#1089#1103
    Default = True
    ModalResult = 1
    TabOrder = 8
    OnClick = btnOkClick
  end
  object btnCancel: TButton
    Left = 264
    Top = 224
    Width = 75
    Height = 25
    Cancel = True
    Caption = #1054#1090#1084#1077#1085#1072
    ModalResult = 2
    TabOrder = 9
  end
  object lbxBases: TListBox
    Left = 8
    Top = 24
    Width = 241
    Height = 113
    ItemHeight = 13
    TabOrder = 0
    OnClick = lbxBasesClick
  end
  object btnAdd: TButton
    Left = 264
    Top = 24
    Width = 75
    Height = 25
    Caption = '&'#1044#1086#1073#1072#1074#1080#1090#1100
    TabOrder = 4
    OnClick = btnAddClick
  end
  object btnDelete: TButton
    Left = 264
    Top = 56
    Width = 75
    Height = 25
    Caption = '&'#1059#1076#1072#1083#1080#1090#1100
    TabOrder = 5
    OnClick = btnDeleteClick
  end
  object btnChange: TButton
    Left = 264
    Top = 88
    Width = 75
    Height = 25
    Caption = '&'#1048#1079#1084#1077#1085#1080#1090#1100
    TabOrder = 6
    OnClick = btnChangeClick
  end
  object btnCreate: TButton
    Left = 264
    Top = 140
    Width = 75
    Height = 25
    Caption = '&'#1053#1086#1074#1072#1103' '#1041#1044
    TabOrder = 7
    OnClick = btnCreateClick
  end
  object edtName: TEdit
    Left = 88
    Top = 144
    Width = 161
    Height = 21
    CharCase = ecUpperCase
    TabOrder = 1
  end
  object edtRole: TEdit
    Left = 88
    Top = 168
    Width = 161
    Height = 21
    TabOrder = 2
  end
  object StatusBar1: TStatusBar
    Left = 0
    Top = 257
    Width = 353
    Height = 19
    Panels = <>
    SimplePanel = True
  end
end

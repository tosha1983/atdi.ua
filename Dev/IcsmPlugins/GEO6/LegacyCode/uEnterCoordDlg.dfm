object dlgEnterCoord: TdlgEnterCoord
  Left = 245
  Top = 108
  BorderStyle = bsDialog
  Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1090#1080
  ClientHeight = 132
  ClientWidth = 304
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 12
    Top = 12
    Width = 197
    Height = 13
    Caption = #1059#1082#1072#1078#1110#1090#1100' '#1082#1086#1086#1088#1076#1080#1085#1072#1090#1080' '#1072#1085#1072#1083#1110#1079#1091#1108#1084#1086#1111' '#1090#1086#1095#1082#1080
  end
  object Label2: TLabel
    Left = 16
    Top = 36
    Width = 43
    Height = 13
    Caption = #1044#1086#1074#1075#1086#1090#1072
  end
  object Label3: TLabel
    Left = 164
    Top = 36
    Width = 38
    Height = 13
    Caption = #1064#1080#1088#1086#1090#1072
  end
  object OKBtn: TButton
    Left = 127
    Top = 96
    Width = 75
    Height = 25
    Caption = 'OK'
    ModalResult = 1
    TabOrder = 2
  end
  object CancelBtn: TButton
    Left = 207
    Top = 96
    Width = 75
    Height = 25
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 3
  end
  object edtLon: TEdit
    Left = 12
    Top = 52
    Width = 121
    Height = 21
    TabOrder = 0
    OnExit = edtLonExit
  end
  object edtLat: TEdit
    Left = 160
    Top = 52
    Width = 121
    Height = 21
    TabOrder = 1
    OnExit = edtLatExit
  end
end

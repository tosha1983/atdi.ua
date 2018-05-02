object dlgConstrSet: TdlgConstrSet
  Left = 314
  Top = 177
  BorderStyle = bsDialog
  Caption = #1054#1073#1084#1077#1078#1077#1085#1085#1103' i'#1084#1087#1086#1088#1090#1091
  ClientHeight = 214
  ClientWidth = 315
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  OnShow = FormShow
  DesignSize = (
    315
    214)
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 299
    Height = 161
    Anchors = [akLeft, akTop, akRight, akBottom]
    Shape = bsFrame
  end
  object Label1: TLabel
    Left = 24
    Top = 51
    Width = 97
    Height = 13
    Caption = #1052'i'#1085'i'#1084#1072#1083#1100#1085#1072' '#1096#1080#1088#1086#1090#1072
  end
  object Label2: TLabel
    Left = 24
    Top = 27
    Width = 100
    Height = 13
    Caption = #1052'i'#1085'i'#1084#1072#1083#1100#1085#1072' '#1076#1086#1074#1075#1086#1090#1072
  end
  object Label3: TLabel
    Left = 24
    Top = 99
    Width = 111
    Height = 13
    Caption = #1052#1072#1082#1089#1080#1084#1072#1083#1100#1085#1072' '#1096#1080#1088#1086#1090#1072
  end
  object Label4: TLabel
    Left = 24
    Top = 75
    Width = 114
    Height = 13
    Caption = #1052#1072#1082#1089#1080#1084#1072#1083#1100#1085#1072' '#1076#1086#1074#1075#1086#1090#1072
  end
  object OKBtn: TButton
    Left = 121
    Top = 180
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object CancelBtn: TButton
    Left = 209
    Top = 180
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 1
  end
  object edtMinLat: TEdit
    Left = 160
    Top = 48
    Width = 121
    Height = 21
    TabOrder = 3
    Text = 'edtMinLat'
    OnExit = edtMinLatExit
  end
  object edtMinLon: TEdit
    Left = 160
    Top = 24
    Width = 121
    Height = 21
    TabOrder = 2
    Text = 'edtMinLon'
    OnExit = edtMinLonExit
  end
  object edtMaxLat: TEdit
    Left = 160
    Top = 96
    Width = 121
    Height = 21
    TabOrder = 5
    Text = 'edtMaxLat'
    OnExit = edtMaxLatExit
  end
  object edtMaxLon: TEdit
    Left = 160
    Top = 72
    Width = 121
    Height = 21
    TabOrder = 4
    Text = 'edtMaxLon'
    OnExit = edtMaxLonExit
  end
  object chbOnlyIfContExist: TCheckBox
    Left = 24
    Top = 128
    Width = 257
    Height = 17
    Caption = #1058'i'#1083#1100#1082#1080' '#1087#1088#1080' '#1085#1072#1103#1074#1085#1086#1089#1090'i '#1082#1086#1085#1090#1091#1088'i'#1074
    TabOrder = 6
  end
end

object dgSelectStations: TdgSelectStations
  Left = 420
  Top = 219
  BorderStyle = bsDialog
  Caption = #1043#1088#1091#1087#1072' '#1086#1073#39#1108#1082#1090'i'#1074
  ClientHeight = 378
  ClientWidth = 239
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    239
    378)
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 10
    Top = 315
    Width = 82
    Height = 13
    Caption = #1042#1080#1073#1088#1072#1090#1080' '#1089#1090#1072#1085#1094'i'#1111':'
  end
  object OKBtn: TButton
    Left = 61
    Top = 344
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1044#1072
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object CancelBtn: TButton
    Left = 149
    Top = 344
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = #1054#1090#1084#1077#1085#1072
    ModalResult = 2
    TabOrder = 1
  end
  object lbList: TCheckListBox
    Left = 8
    Top = 8
    Width = 225
    Height = 297
    ItemHeight = 13
    TabOrder = 2
  end
  object btAll: TButton
    Left = 96
    Top = 312
    Width = 65
    Height = 21
    Caption = #1042#1089'i'
    TabOrder = 3
    OnClick = btAllClick
  end
  object btNone: TButton
    Left = 168
    Top = 312
    Width = 59
    Height = 21
    Caption = #1046#1086#1076#1085#1086#1111
    TabOrder = 4
    OnClick = btNoneClick
  end
end

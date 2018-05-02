object dlgSaveServer: TdlgSaveServer
  Left = 655
  Top = 345
  BorderStyle = bsDialog
  Caption = #1057#1086#1093#1088#1072#1085#1080#1090#1100' '#1076#1072#1085#1085#1099#1081' '#1089#1077#1088#1074#1077#1088'?'
  ClientHeight = 164
  ClientWidth = 358
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    358
    164)
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 345
    Height = 118
    Anchors = [akLeft, akTop, akBottom]
    Shape = bsFrame
  end
  object Label1: TLabel
    Left = 35
    Top = 20
    Width = 30
    Height = 13
    Caption = 'GUID:'
  end
  object Label2: TLabel
    Left = 14
    Top = 46
    Width = 53
    Height = 13
    Anchors = [akLeft, akBottom]
    Caption = #1054#1087#1080#1089#1072#1085#1080#1077':'
  end
  object Label3: TLabel
    Left = 40
    Top = 47
    Width = 25
    Height = 13
    Caption = #1048#1084#1103':'
    Visible = False
  end
  object OKBtn: TButton
    Left = 176
    Top = 133
    Width = 82
    Height = 25
    Anchors = [akLeft, akBottom]
    Caption = #1057#1086#1093#1088#1072#1085#1080#1090#1100
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object CancelBtn: TButton
    Left = 264
    Top = 132
    Width = 82
    Height = 26
    Anchors = [akLeft, akBottom]
    Cancel = True
    Caption = #1053#1077' '#1089#1086#1093#1088#1072#1085#1103#1090#1100
    ModalResult = 2
    TabOrder = 1
  end
  object edGuid: TEdit
    Left = 72
    Top = 16
    Width = 273
    Height = 21
    TabOrder = 2
  end
  object edServName: TEdit
    Left = 72
    Top = 45
    Width = 273
    Height = 21
    TabOrder = 3
    Visible = False
  end
  object memDescr: TMemo
    Left = 72
    Top = 45
    Width = 273
    Height = 73
    Anchors = [akLeft, akBottom]
    TabOrder = 4
  end
end

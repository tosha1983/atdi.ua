object dlgImportDigLayer: TdlgImportDigLayer
  Left = 245
  Top = 108
  BorderStyle = bsDialog
  Caption = #1030#1084#1087#1086#1088#1090' '#1094#1080#1092#1088#1086#1074#1080#1093' '#1074#1080#1076#1110#1083#1077#1085#1100
  ClientHeight = 367
  ClientWidth = 313
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 297
    Height = 313
    Shape = bsFrame
  end
  object OKBtn: TButton
    Left = 135
    Top = 332
    Width = 75
    Height = 25
    Caption = #1030#1084#1087#1086#1088#1090'!'
    Default = True
    ModalResult = 1
    TabOrder = 0
    OnClick = OKBtnClick
  end
  object CancelBtn: TButton
    Left = 223
    Top = 332
    Width = 75
    Height = 25
    Cancel = True
    Caption = #1042#1110#1076#1084#1110#1085#1080#1090#1080
    ModalResult = 2
    TabOrder = 1
  end
  object lbxLayers: TCheckListBox
    Left = 16
    Top = 16
    Width = 281
    Height = 265
    ItemHeight = 13
    TabOrder = 2
  end
  object rbDab: TRadioButton
    Left = 24
    Top = 296
    Width = 73
    Height = 17
    Caption = #1071#1082' T-DAB'
    TabOrder = 3
  end
  object rbDvb: TRadioButton
    Left = 104
    Top = 296
    Width = 81
    Height = 17
    Caption = #1071#1082' DVB-T'
    TabOrder = 4
  end
  object cbxCountry: TComboBox
    Left = 192
    Top = 293
    Width = 105
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    TabOrder = 5
  end
end

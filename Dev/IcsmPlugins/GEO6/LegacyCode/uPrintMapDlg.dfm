object dlgPrintMap: TdlgPrintMap
  Left = 299
  Top = 215
  BorderStyle = bsDialog
  Caption = #1047#1072#1087#1080#1089' '#1074' '#1092#1072#1081#1083' '#1092#1088#1072#1075#1084#1077#1085#1090#1091' '#1082#1072#1088#1090#1080
  ClientHeight = 118
  ClientWidth = 422
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 16
    Top = 4
    Width = 393
    Height = 77
    Shape = bsFrame
  end
  object lblFileName: TLabel
    Left = 36
    Top = 20
    Width = 53
    Height = 13
    Caption = #1030#1084#39#1103' '#1092#1072#1081#1083#1091
  end
  object lblQuality: TLabel
    Left = 24
    Top = 52
    Width = 57
    Height = 13
    Caption = #1071#1082#1110#1089#1090#1100' (1-5)'
  end
  object lblRange: TLabel
    Left = 176
    Top = 44
    Width = 137
    Height = 26
    Caption = #1050#1086#1077#1092#1080#1094#1110#1108#1085#1090' '#1088#1086#1079#1096#1080#1088#1077#1085#1085#1103' '#1086#1093#1086#1087#1083#1077#1085#1085#1103' '#1074#1110#1076#1085#1086#1089#1085#1086' '#1094#1077#1085#1090#1088#1091
    Visible = False
    WordWrap = True
  end
  object lblX: TLabel
    Left = 332
    Top = 52
    Width = 5
    Height = 13
    Caption = 'x'
    Visible = False
  end
  object OKBtn: TButton
    Left = 131
    Top = 88
    Width = 75
    Height = 25
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 0
    OnClick = OKBtnClick
  end
  object CancelBtn: TButton
    Left = 211
    Top = 88
    Width = 75
    Height = 25
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 1
    OnClick = CancelBtnClick
  end
  object edtFileName: TEdit
    Left = 96
    Top = 16
    Width = 277
    Height = 21
    TabOrder = 2
  end
  object btnGetFileName: TButton
    Left = 376
    Top = 16
    Width = 25
    Height = 21
    Caption = '...'
    TabOrder = 3
    OnClick = btnGetFileNameClick
  end
  object udQuality: TUpDown
    Left = 129
    Top = 48
    Width = 15
    Height = 21
    Associate = edtQuality
    Min = 1
    Max = 5
    Position = 1
    TabOrder = 4
    Wrap = False
  end
  object edtQuality: TEdit
    Left = 96
    Top = 48
    Width = 33
    Height = 21
    TabOrder = 5
    Text = '1'
  end
  object edtRange: TEdit
    Left = 340
    Top = 48
    Width = 33
    Height = 21
    TabOrder = 6
    Text = '1'
    Visible = False
  end
  object upRange: TUpDown
    Left = 373
    Top = 48
    Width = 15
    Height = 21
    Associate = edtRange
    Min = 1
    Max = 10
    Position = 1
    TabOrder = 7
    Visible = False
    Wrap = False
  end
  object SaveDialog1: TSaveDialog
    Filter = #1060#1072#1081#1083#1080' *.bmp  (*.bmp) | *.BMP'
    Options = [ofHideReadOnly, ofPathMustExist, ofEnableSizing]
    OnSelectionChange = SaveDialog1SelectionChange
    Left = 304
    Top = 84
  end
end

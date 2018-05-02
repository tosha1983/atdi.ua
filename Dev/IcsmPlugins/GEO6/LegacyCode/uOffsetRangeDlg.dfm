object OffsetRangeDlg: TOffsetRangeDlg
  Left = 477
  Top = 374
  BorderStyle = bsDialog
  Caption = #1044#1110#1072#1087#1072#1079#1086#1085
  ClientHeight = 146
  ClientWidth = 247
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 233
    Height = 97
    Shape = bsFrame
  end
  object Label1: TLabel
    Left = 48
    Top = 27
    Width = 65
    Height = 13
    Caption = #1053#1080#1078#1085#1103' '#1084#1077#1078#1072
  end
  object Label2: TLabel
    Left = 48
    Top = 63
    Width = 67
    Height = 13
    Caption = #1042#1077#1088#1093#1085#1103' '#1084#1077#1078#1072
  end
  object OKBtn: TButton
    Left = 71
    Top = 116
    Width = 75
    Height = 25
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 0
    OnClick = OKBtnClick
  end
  object CancelBtn: TButton
    Left = 151
    Top = 116
    Width = 75
    Height = 25
    Cancel = True
    Caption = #1042#1110#1076#1084#1110#1085#1080#1090#1080
    ModalResult = 2
    TabOrder = 1
  end
  object edtDownRange: TEdit
    Left = 128
    Top = 24
    Width = 45
    Height = 21
    TabOrder = 2
    Text = '-5'
  end
  object udDown: TUpDown
    Left = 173
    Top = 24
    Width = 15
    Height = 21
    Associate = edtDownRange
    Min = -20
    Max = 20
    Position = -5
    TabOrder = 3
    Wrap = False
  end
  object edtUpRange: TEdit
    Left = 128
    Top = 60
    Width = 45
    Height = 21
    TabOrder = 4
    Text = '5'
  end
  object udUp: TUpDown
    Left = 173
    Top = 60
    Width = 15
    Height = 21
    Associate = edtUpRange
    Min = -20
    Max = 20
    Position = 5
    TabOrder = 5
    Wrap = False
  end
end

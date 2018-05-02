object dgCreateChannelPlan: TdgCreateChannelPlan
  Left = 662
  Top = 157
  BorderStyle = bsDialog
  Caption = #1053#1086#1074#1072#1103' '#1089#1077#1090#1082#1072' '#1095#1072#1089#1090#1086#1090' '#1087#1083#1072#1085
  ClientHeight = 425
  ClientWidth = 296
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    296
    425)
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 282
    Height = 369
    Anchors = [akLeft, akTop, akRight, akBottom]
    Shape = bsFrame
  end
  object Label1: TLabel
    Left = 48
    Top = 29
    Width = 121
    Height = 13
    Caption = #1053#1072#1079#1074#1072#1085#1080#1077' '#1089#1077#1090#1082#1080' '#1095#1072#1089#1090#1086#1090':'
  end
  object Label2: TLabel
    Left = 51
    Top = 157
    Width = 118
    Height = 13
    Caption = 'M'#1077#1078#1076#1091' '#1082#1072#1085#1072#1083#1072#1084#1080', MHz:'
  end
  object Label3: TLabel
    Left = 60
    Top = 125
    Width = 109
    Height = 13
    Caption = #1064#1080#1088#1080#1085#1072' '#1082#1072#1085#1072#1083#1072', MHz:'
  end
  object Label4: TLabel
    Left = 49
    Top = 188
    Width = 120
    Height = 13
    Caption = #1053#1086#1084#1077#1088' '#1087#1077#1088#1074#1086#1075#1086' '#1082#1072#1085#1072#1083#1072':'
  end
  object Label5: TLabel
    Left = 51
    Top = 221
    Width = 118
    Height = 13
    Caption = #1057#1084#1077#1097#1077#1085#1080#1077' '#1074#1080#1076#1077#1086', MHz:'
  end
  object Label6: TLabel
    Left = 52
    Top = 252
    Width = 117
    Height = 13
    Caption = #1057#1084#1077#1097#1077#1085#1080#1077' '#1079#1074#1091#1082#1072', MHz:'
  end
  object Label7: TLabel
    Left = 85
    Top = 285
    Width = 84
    Height = 13
    Caption = #1055#1088#1077#1092#1080#1082#1089' '#1080#1084#1077#1085#1080':'
  end
  object Label8: TLabel
    Left = 85
    Top = 317
    Width = 84
    Height = 13
    Caption = #1057#1091#1092#1092#1080#1082#1089' '#1080#1084#1077#1085#1080':'
  end
  object Label9: TLabel
    Left = 28
    Top = 349
    Width = 141
    Height = 13
    Caption = #1050#1086#1083#1080#1095#1077#1089#1090#1074#1086' '#1094#1080#1092#1088' '#1074' '#1085#1086#1084#1077#1088#1077':'
  end
  object Label10: TLabel
    Left = 41
    Top = 61
    Width = 128
    Height = 13
    Caption = #1053#1072#1095#1072#1083#1100#1085#1072#1103' '#1095#1072#1089#1090#1086#1090#1072', MHz:'
  end
  object btOk: TButton
    Left = 126
    Top = 391
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1044#1072
    Default = True
    TabOrder = 12
    OnClick = btOkClick
  end
  object btCancel: TButton
    Left = 214
    Top = 391
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = #1054#1090#1084#1077#1085#1072
    ModalResult = 2
    TabOrder = 13
  end
  object FirstFreq: TEdit
    Left = 176
    Top = 56
    Width = 105
    Height = 21
    TabOrder = 1
    OnExit = CheckFreqOnExit
  end
  object LastFreq: TEdit
    Left = 176
    Top = 88
    Width = 105
    Height = 21
    TabOrder = 3
  end
  object FreqGridName: TEdit
    Left = 176
    Top = 24
    Width = 105
    Height = 21
    TabOrder = 0
  end
  object Spacing: TEdit
    Left = 176
    Top = 152
    Width = 105
    Height = 21
    TabOrder = 5
  end
  object ChanWidth: TEdit
    Left = 176
    Top = 120
    Width = 105
    Height = 21
    TabOrder = 4
  end
  object FirstChanNum: TEdit
    Left = 176
    Top = 184
    Width = 105
    Height = 21
    TabOrder = 6
    Text = '1'
  end
  object VideoRem: TEdit
    Left = 176
    Top = 216
    Width = 105
    Height = 21
    TabOrder = 7
  end
  object SoundRem: TEdit
    Left = 176
    Top = 248
    Width = 105
    Height = 21
    TabOrder = 8
  end
  object NamePref: TEdit
    Left = 176
    Top = 280
    Width = 105
    Height = 21
    TabOrder = 9
  end
  object NameSuf: TEdit
    Left = 176
    Top = 312
    Width = 105
    Height = 21
    TabOrder = 10
  end
  object NumQuant: TEdit
    Left = 176
    Top = 344
    Width = 105
    Height = 21
    TabOrder = 11
  end
  object LastOrNumb: TComboBox
    Left = 48
    Top = 88
    Width = 121
    Height = 21
    Style = csDropDownList
    DropDownCount = 2
    ItemHeight = 13
    ItemIndex = 0
    TabOrder = 2
    Text = #1050#1086#1085#1077#1095#1085#1072#1103' '#1095#1072#1089#1090#1086#1090#1072
    Items.Strings = (
      #1050#1086#1085#1077#1095#1085#1072#1103' '#1095#1072#1089#1090#1086#1090#1072
      #1050#1086#1083#1080#1095#1077#1089#1090#1074#1086' '#1082#1072#1085#1072#1083#1086#1074)
  end
end

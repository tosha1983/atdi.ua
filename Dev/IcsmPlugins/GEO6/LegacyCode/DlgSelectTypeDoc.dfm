inherited SelectTypeDoc: TSelectTypeDoc
  Left = 414
  Top = 317
  Caption = #1064#1072#1073#1083#1086#1085
  ClientHeight = 260
  ClientWidth = 610
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel [0]
    Left = 30
    Top = 76
    Width = 29
    Height = 13
    Caption = #1060#1072#1081#1083
  end
  object Label2: TLabel [1]
    Left = 35
    Top = 11
    Width = 25
    Height = 13
    Caption = #1050#1086#1076': '
  end
  object Label3: TLabel [2]
    Left = 22
    Top = 43
    Width = 38
    Height = 13
    Caption = #1053#1072#1079#1074#1072': '
  end
  inherited pnButtons: TPanel
    Top = 220
    Width = 610
    TabOrder = 6
    inherited btOk: TBitBtn
      Left = 240
      Width = 86
    end
    inherited btApply: TBitBtn
      Left = 332
      Width = 86
    end
    inherited btRefresh: TBitBtn
      Left = 424
      Width = 86
    end
    inherited btClose: TBitBtn
      Left = 516
      Width = 86
    end
  end
  object RadioGroup1: TRadioGroup [4]
    Left = 256
    Top = 104
    Width = 145
    Height = 73
    BiDiMode = bdLeftToRight
    Caption = ' '#1058#1080#1087' '#1096#1072#1073#1083#1086#1085#1091' '
    Color = clBtnFace
    Columns = 2
    Ctl3D = True
    ItemIndex = 0
    Items.Strings = (
      'PiFolio'
      'Excel Report'
      #1030#1085#1096#1077)
    ParentBiDiMode = False
    ParentColor = False
    ParentCtl3D = False
    TabOrder = 2
    OnClick = RadioGroup1Click
  end
  object edtFileName: TEdit [5]
    Left = 64
    Top = 72
    Width = 337
    Height = 21
    TabOrder = 4
  end
  object btnLoadDoc: TButton [6]
    Left = 404
    Top = 72
    Width = 21
    Height = 21
    Caption = '<<'
    TabOrder = 3
    OnClick = btnLoadDocClick
  end
  object edTempCode: TEdit [7]
    Left = 64
    Top = 8
    Width = 93
    Height = 21
    TabOrder = 0
    OnEnter = edChange
  end
  object edTempName: TEdit [8]
    Left = 64
    Top = 40
    Width = 337
    Height = 21
    TabOrder = 1
    OnEnter = edChange
  end
  object btEditTempl: TButton [9]
    Left = 276
    Top = 184
    Width = 105
    Height = 25
    Caption = ' '#1056#1077#1076#1072#1075#1091#1074#1072#1090#1080'...'
    TabOrder = 5
    OnClick = btEditTemplClick
  end
  object tmpContainer: TOleContainer [10]
    Left = 228
    Top = 4
    Width = 109
    Height = 57
    AllowInPlace = False
    AllowActiveDoc = False
    AutoActivate = aaManual
    AutoVerbMenu = False
    Caption = 'tmpContainer'
    OldStreamFormat = True
    TabOrder = 7
    TabStop = False
    Visible = False
  end
  object rgDT: TRadioGroup [11]
    Left = 432
    Top = 12
    Width = 161
    Height = 189
    Caption = ' '#1058#1080#1087' '#1076#1086#1082#1091#1084#1077#1085#1090#1091
    Items.Strings = (
      #1051#1080#1089#1090#1080' '#1074' '#1043#1064
      #1051#1080#1089#1090#1080' '#1074' '#1053#1056
      #1057#1083#1091#1078#1073#1086#1074#1110' '#1079#1072#1087#1080#1089#1082#1080
      #1042#1080#1089#1085#1086#1074#1086#1082' '#1045#1052#1057
      #1044#1086#1079#1074#1110#1083' '#1085#1072' '#1077#1082#1089#1087#1083#1091#1072#1090#1072#1094#1110#1102
      #1030#1085#1096#1110)
    TabOrder = 8
  end
  object rgRadtech: TRadioGroup [12]
    Left = 64
    Top = 104
    Width = 161
    Height = 105
    Caption = #1056#1110#1076#1110#1086#1090#1077#1093#1085#1086#1083#1086#1075#1110#1103
    Columns = 2
    Items.Strings = (
      #1040#1056#1052
      #1062#1056#1052
      #1040#1058#1041
      #1062#1058#1041)
    TabOrder = 9
  end
  inherited ActionList1: TActionList
    Left = 328
    Top = 12
  end
  inherited tr: TIBTransaction
    Left = 404
    Top = 12
  end
  inherited dscObj: TDataSource
    Left = 288
    Top = 4
  end
  object OpenDialog1: TOpenDialog
    Left = 364
    Top = 12
  end
end

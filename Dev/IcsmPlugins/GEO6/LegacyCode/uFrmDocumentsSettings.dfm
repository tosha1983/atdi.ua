inherited frmDocumentsSettings: TfrmDocumentsSettings
  Left = 486
  Top = 257
  Caption = #1053#1086#1074#1080#1081' '#1076#1086#1082#1091#1084#1077#1085#1090
  ClientHeight = 246
  ClientWidth = 520
  Constraints.MaxHeight = 580
  Constraints.MaxWidth = 700
  FormStyle = fsStayOnTop
  OldCreateOrder = True
  Position = poScreenCenter
  OnCreate = FormCreate
  OnKeyDown = FormKeyDown
  PixelsPerInch = 96
  TextHeight = 13
  object ldlDocType: TLabel [0]
    Left = 184
    Top = 100
    Width = 39
    Height = 13
    Caption = #1064#1072#1073#1083#1086#1085
  end
  object lblNum: TLabel [1]
    Left = 184
    Top = 22
    Width = 34
    Height = 13
    Caption = #1053#1086#1084#1077#1088
  end
  object lblDocName: TLabel [2]
    Left = 244
    Top = 168
    Width = 15
    Height = 13
    Caption = '-----'
  end
  object lblDN: TLabel [3]
    Left = 180
    Top = 168
    Width = 54
    Height = 13
    Caption = #1044#1086#1082#1091#1084#1077#1085#1090':'
  end
  object lblData: TLabel [4]
    Left = 356
    Top = 22
    Width = 26
    Height = 13
    Caption = #1044#1072#1090#1072
  end
  object lblAccStat: TLabel [5]
    Left = 188
    Top = 60
    Width = 24
    Height = 13
    Caption = #1057#1090#1072#1085
  end
  inherited pnButtons: TPanel
    Top = 206
    Width = 520
    TabOrder = 1
    inherited btOk: TBitBtn
      Left = 70
    end
    inherited btApply: TBitBtn
      Left = 182
    end
    inherited btRefresh: TBitBtn
      Left = 294
    end
    inherited btClose: TBitBtn
      Left = 406
    end
  end
  object tmpContainer: TOleContainer [7]
    Left = 381
    Top = 60
    Width = 42
    Height = 29
    AllowInPlace = False
    AllowActiveDoc = False
    AutoActivate = aaManual
    AutoVerbMenu = False
    Caption = 'tmpContainer'
    OldStreamFormat = True
    TabOrder = 0
    Visible = False
  end
  object btnInDocImage: TBitBtn [8]
    Left = 232
    Top = 128
    Width = 100
    Height = 21
    Caption = #1042#1093#1110#1076#1085#1080#1081' '#1076#1086#1082'...'
    TabOrder = 2
    OnClick = btnInDocImageClick
    Glyph.Data = {
      36030000424D3603000000000000360000002800000010000000100000000100
      1800000000000003000000000000000000000000000000000000FF00FFFF00FF
      FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00
      FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
      00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
      FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00
      FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
      00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
      000000000000000000000000000000000000000000000000000000000000FF00
      FFFF00FFFF00FFFF00FFFF00FF00000000000000FFFFC0C0C000FFFFC0C0C000
      FFFFC0C0C000FFFFC0C0C000FFFF000000FF00FFFF00FFFF00FFFF00FF000000
      FFFFFF00000000FFFFC0C0C000FFFFC0C0C000FFFFC0C0C000FFFFC0C0C000FF
      FF000000FF00FFFF00FFFF00FF00000000FFFFFFFFFF00000000FFFFC0C0C000
      FFFFC0C0C000FFFFC0C0C000FFFFC0C0C000FFFF000000FF00FFFF00FF000000
      FFFFFF00FFFFFFFFFF0000000000000000000000000000000000000000000000
      00000000000000FF00FFFF00FF00000000FFFFFFFFFF00FFFFFFFFFF00FFFFFF
      FFFF00FFFFFFFFFF00FFFF000000FF00FFFF00FFFF00FFFF00FFFF00FF000000
      FFFFFF00FFFFFFFFFF00FFFFFFFFFF00FFFFFFFFFF00FFFFFFFFFF000000FF00
      FFFF00FFFF00FFFF00FFFF00FF00000000FFFFFFFFFF00FFFFFFFFFF00FFFFFF
      FFFF000000000000000000FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
      00000000FFFFFFFFFF00FFFFFFFFFF000000FF00FFFF00FFFF00FFFF00FFFF00
      FFFF00FFFF00FFFF00FFFF00FFFF00FF80808000000000000000000000000080
      8080FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
      FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00
      FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
      00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
  end
  object rgDoctype: TRadioGroup [9]
    Left = 8
    Top = 12
    Width = 161
    Height = 189
    Caption = ' '#1058#1080#1087' '
    Items.Strings = (
      #1051#1080#1089#1090#1080' '#1074' '#1043#1064
      #1051#1080#1089#1090#1080' '#1074' '#1053#1056
      #1057#1083#1091#1078#1073#1086#1074#1110' '#1079#1072#1087#1080#1089#1082#1080
      #1042#1080#1089#1085#1086#1074#1086#1082' '#1045#1052#1057
      #1044#1086#1079#1074#1110#1083' '#1085#1072' '#1077#1082#1089#1087#1083#1091#1072#1090#1072#1094#1110#1102
      #1030#1085#1096#1110)
    TabOrder = 3
    OnClick = rgDoctypeClick
  end
  object edtNum: TEdit [10]
    Left = 232
    Top = 18
    Width = 113
    Height = 21
    MaxLength = 16
    TabOrder = 4
    OnChange = edtNumChange
  end
  object dtpDocDate: TDateTimePicker [11]
    Left = 392
    Top = 18
    Width = 121
    Height = 21
    CalAlignment = dtaLeft
    Date = 37831.6994116204
    Time = 37831.6994116204
    DateFormat = dfShort
    DateMode = dmComboBox
    Kind = dtkDate
    ParseInput = False
    TabOrder = 5
    OnChange = dtpDocDateChange
  end
  object cbxDocType: TComboBox [12]
    Left = 232
    Top = 96
    Width = 281
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    TabOrder = 6
    OnChange = cbxDocTypeChange
  end
  object cbxAccountState: TComboBox [13]
    Left = 232
    Top = 56
    Width = 113
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    TabOrder = 7
    OnChange = cbxAccountStateChange
  end
  object btOpenDoc: TButton [14]
    Left = 352
    Top = 128
    Width = 100
    Height = 21
    Caption = #1054#1090#1082#1088#1099#1090#1100' '#1076#1086#1082'.'
    TabOrder = 8
    OnClick = btOpenDocClick
  end
  object rbInOut: TRadioGroup [15]
    Left = 356
    Top = 48
    Width = 149
    Height = 37
    Caption = ' '#1058#1080#1087' '
    Columns = 2
    ItemIndex = 1
    Items.Strings = (
      #1042#1080#1093#1110#1076#1085#1077
      #1042#1093#1110#1076#1085#1077)
    TabOrder = 9
    OnClick = rbInOutClick
  end
  inherited ActionList1: TActionList
    Left = 336
    Top = 160
  end
  inherited tr: TIBTransaction
    Left = 416
    Top = 160
  end
  inherited dscObj: TDataSource
    Left = 456
    Top = 160
  end
  object OpenDialog1: TOpenDialog
    OnSelectionChange = OpenDialog1SelectionChange
    Left = 296
    Top = 160
  end
end

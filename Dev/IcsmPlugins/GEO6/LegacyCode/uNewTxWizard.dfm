object frmNewTxWizard: TfrmNewTxWizard
  Left = 173
  Top = 159
  Width = 754
  Height = 524
  Caption = #1042#1074#1077#1076#1077#1085#1085#1103' '#1085#1086#1074#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  DesignSize = (
    746
    497)
  PixelsPerInch = 96
  TextHeight = 13
  object btnPrev: TButton
    Left = 363
    Top = 464
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = '< '#1053#1072#1079#1072#1076
    Enabled = False
    TabOrder = 1
    OnClick = btnPrevClick
  end
  object btnNext: TButton
    Left = 459
    Top = 464
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1044#1072#1083#1110' >'
    TabOrder = 2
    OnClick = btnNextClick
  end
  object btnFinish: TButton
    Left = 555
    Top = 464
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1042#1074#1077#1089#1090#1080
    Enabled = False
    TabOrder = 3
    OnClick = btnFinishClick
  end
  object Cancel: TButton
    Left = 651
    Top = 464
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = #1042#1110#1076#1084#1110#1085#1072
    TabOrder = 4
    OnClick = CancelClick
  end
  object panTech: TPanel
    Left = 104
    Top = 24
    Width = 592
    Height = 330
    TabOrder = 8
    Visible = False
    object Label14: TLabel
      Left = 68
      Top = 67
      Width = 109
      Height = 13
      Alignment = taRightJustify
      Caption = #1042#1090#1088#1072#1090#1080' '#1074' '#1092#1110#1076#1077#1088#1110', '#1076#1041'/'#1084
    end
    object Label15: TLabel
      Left = 96
      Top = 35
      Width = 81
      Height = 13
      Alignment = taRightJustify
      Caption = #1055#1086#1090#1091#1078#1085#1110#1089#1090#1100', '#1082#1042#1090
    end
    object Label16: TLabel
      Left = 285
      Top = 99
      Width = 124
      Height = 13
      Alignment = taRightJustify
      Caption = #1042#1080#1089#1086#1090#1072' '#1087#1110#1076#1074#1110#1089#1091' '#1072#1085#1090#1077#1085#1080', '#1084
    end
    object Label22: TLabel
      Left = 337
      Top = 131
      Width = 72
      Height = 13
      Alignment = taRightJustify
      Caption = #1050#1055' '#1072#1085#1090#1077#1085#1080', '#1076#1041
    end
    object Label23: TLabel
      Left = 265
      Top = 35
      Width = 144
      Height = 13
      Alignment = taRightJustify
      Caption = #1055#1086#1090#1091#1078#1085#1110#1089#1090#1100' '#1074#1110#1076#1077#1086'/'#1079#1074#1091#1082', '#1088#1072#1079#1080
    end
    object lblPolarization: TLabel
      Left = 113
      Top = 99
      Width = 64
      Height = 13
      Alignment = taRightJustify
      Caption = #1055#1086#1083#1103#1088#1080#1079#1072#1094#1110#1103
    end
    object Label24: TLabel
      Left = 311
      Top = 67
      Width = 98
      Height = 13
      Alignment = taRightJustify
      Caption = #1044#1086#1074#1078#1080#1085#1072' '#1092#1110#1076#1077#1088#1072', '#1084
    end
    object lblDirect: TLabel
      Left = 102
      Top = 131
      Width = 75
      Height = 13
      Caption = #1053#1072#1087#1088#1072#1074#1083#1077#1085#1110#1089#1090#1100
    end
    object Label25: TLabel
      Left = 32
      Top = 163
      Width = 145
      Height = 13
      Alignment = taRightJustify
      Caption = #1050#1055' '#1072#1085#1090#1077#1085#1080' '#1087#1086' '#1085#1072#1087#1088#1103#1084#1082#1072#1093', '#1076#1041
    end
    object edtFeederLoss: TEdit
      Left = 184
      Top = 64
      Width = 60
      Height = 21
      TabOrder = 1
      Text = '0'
      OnExit = edtFeederLossExit
    end
    object edtPower: TEdit
      Left = 184
      Top = 32
      Width = 60
      Height = 21
      TabOrder = 0
      Text = '0'
      OnExit = edtPowerExit
    end
    object edtAntHeight: TEdit
      Left = 416
      Top = 96
      Width = 60
      Height = 21
      TabOrder = 3
      Text = '0'
      OnExit = edtAntHeightExit
    end
    object edtAntennaGain: TEdit
      Left = 416
      Top = 128
      Width = 60
      Height = 21
      TabOrder = 2
      Text = '0'
    end
    object edtVideoSoundRatio: TEdit
      Left = 416
      Top = 32
      Width = 60
      Height = 21
      TabOrder = 4
      Text = '10'
    end
    object cbxPolarization: TComboBox
      Left = 184
      Top = 96
      Width = 60
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 5
      Items.Strings = (
        'V'
        'H'
        'M')
    end
    object edtFeederLen: TEdit
      Left = 416
      Top = 64
      Width = 60
      Height = 21
      TabOrder = 6
      Text = '0'
      OnExit = edtFeederLossExit
    end
    object cbxDirect: TComboBox
      Left = 184
      Top = 128
      Width = 60
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 7
      OnChange = cbxDirectChange
      Items.Strings = (
        'D'
        'ND')
    end
    object sgrAntennaGain: TStringGrid
      Left = 184
      Top = 160
      Width = 100
      Height = 153
      ColCount = 2
      DefaultColWidth = 40
      DefaultRowHeight = 14
      RowCount = 36
      FixedRows = 0
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
      TabOrder = 8
      OnSetEditText = sgrAntennaGainSetEditText
      RowHeights = (
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14
        14)
    end
    object panAntGainGraph: TPanel
      Left = 288
      Top = 164
      Width = 149
      Height = 149
      TabOrder = 9
      object Label27: TLabel
        Left = 72
        Top = 1
        Width = 6
        Height = 13
        Caption = '0'
        Color = cl3DLight
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clGreen
        Font.Height = -7
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        ParentColor = False
        ParentFont = False
      end
    end
  end
  object panSystem: TPanel
    Left = 132
    Top = 64
    Width = 592
    Height = 330
    TabOrder = 7
    Visible = False
    object Label9: TLabel
      Left = 120
      Top = 104
      Width = 97
      Height = 13
      Caption = #1057#1080#1089#1090#1077#1084#1072' '#1084#1086#1074#1083#1077#1085#1085#1103
    end
    object Label10: TLabel
      Left = 240
      Top = 104
      Width = 31
      Height = 13
      Caption = #1050#1072#1085#1072#1083
    end
    object Label11: TLabel
      Left = 368
      Top = 104
      Width = 92
      Height = 13
      Caption = #1053#1077#1089#1091#1095#1072' '#1074#1110#1076#1077#1086', '#1052#1043#1094
    end
    object Label12: TLabel
      Left = 240
      Top = 152
      Width = 81
      Height = 13
      Caption = #1063#1072#1089#1090#1086#1090#1085#1080#1081' '#1073#1083#1086#1082
    end
    object Label13: TLabel
      Left = 368
      Top = 152
      Width = 94
      Height = 13
      Caption = #1053#1077#1089#1091#1095#1072' '#1079#1074#1091#1082#1091', '#1052#1043#1094
    end
    object Label18: TLabel
      Left = 80
      Top = 152
      Width = 51
      Height = 13
      Caption = #1042#1080#1076#1110#1083#1077#1085#1085#1103
    end
    object Label26: TLabel
      Left = 80
      Top = 104
      Width = 67
      Height = 13
      Caption = #1057#1110#1090#1082#1072' '#1082#1072#1085#1072#1083#1110#1074
    end
    object cbxSystem: TComboBox
      Left = 120
      Top = 120
      Width = 97
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 0
      OnChange = cbxSystemChange
    end
    object cbxChannel: TComboBox
      Left = 240
      Top = 120
      Width = 97
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 1
      OnChange = cbxChannelChange
    end
    object edtVideoCarrier: TEdit
      Left = 368
      Top = 120
      Width = 97
      Height = 21
      ReadOnly = True
      TabOrder = 2
    end
    object cbxAllotment: TComboBox
      Left = 80
      Top = 168
      Width = 137
      Height = 21
      Style = csDropDownList
      DropDownCount = 20
      ItemHeight = 13
      TabOrder = 3
      OnChange = cbxAllotmentChange
    end
    object edtSoundCarrier: TEdit
      Left = 368
      Top = 168
      Width = 97
      Height = 21
      TabOrder = 4
      OnExit = edtSoundCarrierExit
    end
    object edtBlock: TEdit
      Left = 240
      Top = 168
      Width = 97
      Height = 21
      ReadOnly = True
      TabOrder = 5
    end
    object panDVBParams: TPanel
      Left = 60
      Top = 20
      Width = 385
      Height = 73
      BevelOuter = bvNone
      TabOrder = 6
      Visible = False
      object Label20: TLabel
        Left = 24
        Top = 16
        Width = 44
        Height = 13
        Caption = #1057#1080#1089#1090#1077#1084#1072
      end
      object Label21: TLabel
        Left = 200
        Top = 16
        Width = 57
        Height = 13
        Caption = #1055#1072#1088#1072#1084#1077#1090#1088#1080
      end
      object cbxDVBSystem: TComboBox
        Left = 24
        Top = 32
        Width = 145
        Height = 21
        Style = csDropDownList
        ItemHeight = 13
        TabOrder = 0
      end
      object cbxDVBParams: TComboBox
        Left = 200
        Top = 32
        Width = 145
        Height = 21
        Style = csDropDownList
        ItemHeight = 13
        TabOrder = 1
      end
    end
    object cbxChannelGrid: TComboBox
      Left = 80
      Top = 120
      Width = 97
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 7
      OnChange = cbxChannelGridChange
    end
  end
  object panCoord: TPanel
    Left = -464
    Top = 64
    Width = 592
    Height = 330
    TabOrder = 5
    Visible = False
    object Label1: TLabel
      Left = 184
      Top = 104
      Width = 38
      Height = 13
      Caption = #1064#1080#1088#1086#1090#1072
    end
    object Label2: TLabel
      Left = 184
      Top = 152
      Width = 43
      Height = 13
      Caption = #1044#1086#1074#1075#1086#1090#1072
    end
    object lblID: TLabel
      Left = 216
      Top = 196
      Width = 11
      Height = 13
      Caption = 'ID'
      Visible = False
    end
    object edtLat: TEdit
      Left = 240
      Top = 100
      Width = 121
      Height = 21
      TabOrder = 0
      OnExit = edtLatExit
    end
    object edtLon: TEdit
      Left = 240
      Top = 148
      Width = 121
      Height = 21
      TabOrder = 1
      OnExit = edtLonExit
    end
    object edtAdminId: TMaskEdit
      Left = 240
      Top = 192
      Width = 118
      Height = 21
      MaxLength = 4
      TabOrder = 2
      Visible = False
      OnExit = edtAdminIdExit
    end
    object chbGenerateID: TCheckBox
      Left = 240
      Top = 216
      Width = 137
      Height = 17
      Caption = #1047#1075#1077#1085#1077#1088#1091#1074#1072#1090#1080' '#1085#1086#1074#1080#1081' ID '
      Checked = True
      State = cbChecked
      TabOrder = 3
      Visible = False
      OnClick = chbGenerateIDClick
    end
  end
  object panType: TPanel
    Left = -444
    Top = 164
    Width = 592
    Height = 330
    TabOrder = 0
    object rgrSystemCast: TRadioGroup
      Left = 144
      Top = 56
      Width = 313
      Height = 217
      Caption = #1058#1080#1087' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      TabOrder = 0
    end
  end
  object panStand: TPanel
    Left = 96
    Top = 32
    Width = 592
    Height = 330
    TabOrder = 6
    Visible = False
    object Label3: TLabel
      Left = 52
      Top = 204
      Width = 38
      Height = 13
      Caption = #1064#1080#1088#1086#1090#1072
    end
    object Label4: TLabel
      Left = 156
      Top = 204
      Width = 43
      Height = 13
      Caption = #1044#1086#1074#1075#1086#1090#1072
    end
    object Label5: TLabel
      Left = 52
      Top = 244
      Width = 32
      Height = 13
      Caption = #1056#1077#1075#1110#1086#1085
    end
    object Label7: TLabel
      Left = 276
      Top = 244
      Width = 87
      Height = 13
      Caption = #1053#1072#1089#1077#1083#1077#1085#1080#1081' '#1087#1091#1085#1082#1090
    end
    object Label8: TLabel
      Left = 52
      Top = 284
      Width = 36
      Height = 13
      Caption = #1042#1091#1083#1080#1094#1103
    end
    object Label6: TLabel
      Left = 276
      Top = 284
      Width = 37
      Height = 13
      Caption = #1040#1076#1088#1077#1089#1072
    end
    object Label17: TLabel
      Left = 276
      Top = 204
      Width = 32
      Height = 13
      Caption = #1053#1072#1079#1074#1072
    end
    object Label19: TLabel
      Left = 412
      Top = 284
      Width = 67
      Height = 13
      Caption = #1042#1080#1089#1086#1090#1072' '#1084#1110#1089#1094#1103
    end
    object lblRadius: TLabel
      Left = 228
      Top = 12
      Width = 26
      Height = 13
      Caption = #1084#1110#1085#1091#1090
    end
    object rbtnExistingStand: TRadioButton
      Left = 32
      Top = 12
      Width = 145
      Height = 13
      Caption = '&'#1030#1089#1085#1091#1102#1095#1072' '#1086#1087#1086#1088#1072' '#1074' '#1088#1072#1076#1110#1091#1089#1110' '
      TabOrder = 0
      OnClick = rbtnExistingStandClick
    end
    object grdExistingStand: TDBGrid
      Left = 52
      Top = 36
      Width = 529
      Height = 125
      DataSource = dsExistingStand
      Options = [dgEditing, dgTitles, dgIndicator, dgColumnResize, dgColLines, dgRowLines, dgConfirmDelete, dgCancelOnExit]
      ReadOnly = True
      TabOrder = 3
      TitleFont.Charset = DEFAULT_CHARSET
      TitleFont.Color = clWindowText
      TitleFont.Height = -11
      TitleFont.Name = 'MS Sans Serif'
      TitleFont.Style = []
      OnCellClick = grdExistingStandCellClick
      OnEnter = grdExistingStandEnter
    end
    object rbtnNewStand: TRadioButton
      Left = 32
      Top = 184
      Width = 89
      Height = 17
      Caption = '&'#1053#1086#1074#1072' '#1086#1087#1086#1088#1072
      Checked = True
      TabOrder = 5
      TabStop = True
      OnClick = rbtnNewStandClick
    end
    object chbCheckInCoord: TCheckBox
      Left = 72
      Top = 164
      Width = 233
      Height = 17
      Caption = #1055#1110#1076#1082#1086#1088#1077#1075#1091#1074#1072#1090#1080' '#1082#1086#1086#1088#1076#1080#1085#1072#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      Checked = True
      State = cbChecked
      TabOrder = 4
      Visible = False
    end
    object edtNewLat: TEdit
      Left = 52
      Top = 220
      Width = 89
      Height = 21
      TabOrder = 6
      Text = 'edtNewLat'
      OnExit = edtNewLatExit
      OnKeyPress = edtNewLatKeyPress
    end
    object edtNewLon: TEdit
      Left = 156
      Top = 220
      Width = 97
      Height = 21
      TabOrder = 7
      Text = 'edtNewLon'
      OnExit = edtNewLonExit
      OnKeyPress = edtNewLonKeyPress
    end
    object edtNewStandAreaNum: TEdit
      Left = 52
      Top = 260
      Width = 49
      Height = 21
      TabOrder = 9
      OnExit = edtNewStandAreaNumExit
      OnKeyPress = edtNewStandAreaNumKeyPress
    end
    object cbxNewStandArea: TComboBox
      Left = 108
      Top = 260
      Width = 161
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 10
      OnChange = cbxNewStandAreaChange
    end
    object cbxNewStandCity: TComboBox
      Left = 276
      Top = 260
      Width = 185
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 11
      OnChange = cbxNewStandCityChange
    end
    object cbxNewStandStreet: TComboBox
      Left = 52
      Top = 300
      Width = 217
      Height = 21
      ItemHeight = 13
      TabOrder = 13
      OnChange = cbxNewStandStreetChange
    end
    object edtNewStandAddress: TEdit
      Left = 276
      Top = 300
      Width = 129
      Height = 21
      TabOrder = 14
      OnExit = edtNewStandAddressExit
    end
    object edtNewStandName: TEdit
      Left = 276
      Top = 220
      Width = 209
      Height = 21
      TabOrder = 8
      Text = #1053#1086#1074#1072' '#1086#1087#1086#1088#1072
    end
    object btnCity: TButton
      Left = 468
      Top = 260
      Width = 20
      Height = 20
      Caption = '...'
      TabOrder = 12
      OnClick = btnCityClick
    end
    object edtSiteHeight: TEdit
      Left = 412
      Top = 300
      Width = 57
      Height = 21
      TabOrder = 15
      Text = '0'
      OnExit = edtSiteHeightExit
    end
    object btnGetHeight: TButton
      Left = 468
      Top = 300
      Width = 20
      Height = 20
      Hint = #1042#1080#1089#1086#1090#1072' '#1079' '#1088#1077#1083#1100#1108#1092#1091
      Caption = '...'
      ParentShowHint = False
      ShowHint = True
      TabOrder = 16
      OnClick = btnGetHeightClick
    end
    object seRadius: TCSpinEdit
      Left = 176
      Top = 8
      Width = 45
      Height = 22
      MaxValue = 60
      MinValue = 1
      TabOrder = 1
      Value = 30
    end
    object btnRadius: TButton
      Left = 268
      Top = 8
      Width = 75
      Height = 22
      Caption = #1042#1080#1073#1088#1072#1090#1080
      TabOrder = 2
      OnClick = btnRadiusClick
    end
  end
  object sqlSystemcast: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, ENUMVAL,  DESCRIPTION, IS_USED from SYSTEMCAST'
      'where Enumval < 7'
      'and is_used is not null and is_used <> 0'
      'order by 2')
    Transaction = dmMain.trMain
    Left = 8
  end
  object dstStand: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterScroll = dstStandAfterScroll
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAMESITE,'
      '  LATITUDE,'
      '  LONGITUDE,'
      '  HEIGHT_SEA,'
      '  MAX_OBST,'
      '  MAX_USE,'
      '  VHF_IS,'
      '  AREA_ID,'
      '  DISTRICT_ID,'
      '  CITY_ID,'
      '  ADDRESS,'
      '  STREET_ID'
      'from STAND '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select * from  SP_SELECT_TX_DISTANCE(:LAT,:LON, :DIF)'
      'order by DISTANCE')
    ModifySQL.Strings = (
      '')
    Left = 56
    object dstStandID: TIntegerField
      FieldName = 'ID'
      Origin = 'STAND.ID'
      Required = True
      Visible = False
    end
    object dstStandLATITUDE: TFloatField
      DisplayLabel = #1064#1080#1088#1086#1090#1072
      FieldName = 'LATITUDE'
      Origin = 'STAND.LATITUDE'
      OnGetText = dstStandLATITUDEGetText
    end
    object dstStandLONGITUDE: TFloatField
      DisplayLabel = #1044#1086#1074#1075#1086#1090#1072
      FieldName = 'LONGITUDE'
      Origin = 'STAND.LONGITUDE'
      OnGetText = dstStandLONGITUDEGetText
    end
    object dstStandNAMESITE: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAMESITE'
      Origin = 'STAND.NAMESITE'
      Size = 32
    end
    object dstStandAREA_ID: TIntegerField
      FieldName = 'AREA_ID'
      Origin = 'STAND.AREA_ID'
      Required = True
      Visible = False
    end
    object dstStandA_NAME: TIBStringField
      DisplayLabel = #1056#1077#1075#1110#1086#1085
      FieldName = 'A_NAME'
      Origin = 'AREA.NAME'
      Size = 32
    end
    object dstStandDISTRICT_ID: TIntegerField
      FieldName = 'DISTRICT_ID'
      Origin = 'STAND.DISTRICT_ID'
      Required = True
      Visible = False
    end
    object dstStandD_NAME: TIBStringField
      DisplayLabel = #1056#1072#1081#1086#1085
      FieldName = 'D_NAME'
      Origin = 'DISTRICT.NAME'
      Size = 32
    end
    object dstStandCITY_ID: TIntegerField
      FieldName = 'CITY_ID'
      Origin = 'STAND.CITY_ID'
      Required = True
      Visible = False
    end
    object dstStandC_NAME: TIBStringField
      DisplayLabel = #1053#1072#1089#1077#1083#1077#1085#1080#1081' '#1087#1091#1085#1082#1090
      FieldName = 'C_NAME'
      Origin = 'CITY.NAME'
      Size = 32
    end
    object dstStandSTREET_ID: TIntegerField
      FieldName = 'STREET_ID'
      Origin = 'STAND.STREET_ID'
      Required = True
      Visible = False
    end
    object dstStandST_NAME: TIBStringField
      DisplayLabel = #1042#1091#1083#1080#1094#1103
      FieldName = 'ST_NAME'
      Origin = 'STREET.NAME'
      Size = 16
    end
    object dstStandADDRESS: TIBStringField
      DisplayLabel = #1040#1076#1088#1077#1089#1072
      DisplayWidth = 7
      FieldName = 'ADDRESS'
      Origin = 'STAND.ADDRESS'
      Required = True
      Size = 128
    end
    object dstStandA_NUMREGION: TIBStringField
      FieldName = 'A_NUMREGION'
      Origin = 'AREA.NUMREGION'
      Size = 4
    end
    object dstStandS_HEIGHT_SEA: TIntegerField
      FieldName = 'S_HEIGHT_SEA'
      Origin = 'STAND.HEIGHT_SEA'
    end
  end
  object sqlGetCS_ID: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID from SYSTEMCAST'
      'where ENUMVAL = :ENUMVAL    ')
    Transaction = dmMain.trMain
    Left = 8
    Top = 40
  end
  object dsExistingStand: TDataSource
    DataSet = dstStand
    Left = 96
  end
  object sqlArea: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select a.ID, a.NAME, a.NUMREGION from AREA a'
      'left join COUNTRY c on (a.COUNTRY_ID = c.id)'
      'order by c.NAME, a.NAME')
    Transaction = dmMain.trMain
    Left = 24
    Top = 456
  end
  object sqlCity: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAME '
      'from CITY'
      'where AREA_ID = :AREA_ID'
      'order by NAME ')
    Transaction = dmMain.trMain
    Left = 72
    Top = 456
  end
  object sqlStreet: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAME '
      'from STREET'
      'where CITY_ID = :CITY_ID '
      'order by NAME ')
    Transaction = dmMain.trMain
    Left = 120
    Top = 456
  end
  object sqlAreaByNum: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID '
      'from AREA'
      'where NUMREGION = :NUMREGION')
    Transaction = dmMain.trMain
    Left = 168
    Top = 456
  end
  object sqlAreaByCity: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select AREA_ID'
      'from CITY'
      'where ID = :ID')
    Transaction = dmMain.trMain
    Left = 216
    Top = 456
  end
  object sqlAreaById: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select NUMREGION'
      'from AREA'
      'where  ID = :ID ')
    Transaction = dmMain.trMain
    Left = 264
    Top = 456
  end
  object sqlNewStreet: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'insert into STREET'
      '(ID, NAME, CITY_ID)'
      'values'
      '(:ID, :NAME, :CITY_ID)')
    Transaction = dmMain.trMain
    Left = 304
    Top = 456
  end
  object sqlSystem: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAMESYSTEM'
      'from ANALOGTELESYSTEM '
      'order by 2')
    Transaction = dmMain.trMain
    Left = 16
    Top = 352
  end
  object sqlChannel: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAMECHANNEL '
      'from CHANNELS '
      
        'where FREQUENCYGRID_ID = (select FREQUENCYGRID_ID from ANALOGTEL' +
        'ESYSTEM where ID = :GRP_ID) '
      'order by NAMECHANNEL')
    Transaction = dmMain.trMain
    Left = 96
    Top = 352
  end
  object sqlAllotment: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, IDENTIFIER '
      'from ALLOTMENTBLOCKDAB'
      'order by 2')
    Transaction = dmMain.trMain
    Left = 136
    Top = 356
  end
  object sqlVideoCarrier: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      
        'select FREQCARRIERVISION, FREQCARRIERSOUND, FREQFROM, FREQTO, FR' +
        'EQCARRIERNICAM  from CHANNELS'
      'where ID = :ID')
    Transaction = dmMain.trMain
    Left = 28
    Top = 400
  end
  object sqlSoundBlock: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select B.NAME, B.CENTREFREQ '
      'from BLOCKDAB B'
      'right outer join ALLOTMENTBLOCKDAB A on (B.ID = A.BLOCKDAB_ID)'
      'where A.ID = :ID')
    Transaction = dmMain.trMain
    Left = 72
    Top = 400
  end
  object sqlNewStand: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'insert into  STAND'
      
        '(ID, NAMESITE, LATITUDE, LONGITUDE, HEIGHT_SEA, AREA_ID, CITY_ID' +
        ', STREET_ID, ADDRESS, VHF_IS, DISTRICT_ID, NAMESITE_ENG)'
      'values'
      
        '(:ID, :NAMESITE, :LATITUDE, :LONGITUDE, :HEIGHT_SEA, :AREA_ID, :' +
        'CITY_ID, :STREET_ID, :ADDRESS, :VHF_IS, :DISTRICT_ID, :NAMESITE_' +
        'ENG )')
    Transaction = dmMain.trMain
    Left = 88
    Top = 112
  end
  object sqlNewTx: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'insert  into TRANSMITTERS'
      '('
      'ID, ADMINISTRATIONID, ORIGINALID, STATUS, STAND_ID,'
      'LATITUDE, LONGITUDE, '
      'SYSTEMCAST_ID, TYPESYSTEM, '
      'CHANNEL_ID, VIDEO_CARRIER, SOUND_CARRIER_PRIMARY, '
      'SOUND_CARRIER_SECOND, '
      'ALLOTMENTBLOCKDAB_ID, BLOCKCENTREFREQ, '
      'POWER_VIDEO, EPR_VIDEO_MAX, EPR_VIDEO_VERT, EPR_VIDEO_HOR,   '
      
        'POWER_SOUND_PRIMARY, POWER_SOUND_SECOND, EPR_SOUND_MAX_PRIMARY, ' +
        'EPR_SOUND_MAX_SECOND,'
      'EPR_SOUND_HOR_PRIMARY, EPR_SOUND_HOR_SECOND,'
      'EPR_SOUND_VERT_PRIMARY, EPR_SOUND_VER_SECOND,'
      'V_SOUND_RATIO_PRIMARY, V_SOUND_RATIO_SECOND,'
      
        'ANTENNAGAIN, HEIGHTANTENNA,HEIGHT_EFF_MAX, DIRECTION, SYSTEMCOLO' +
        'UR,'
      'CARRIER,'
      'BANDWIDTH,'
      'POLARIZATION'
      ')'
      'values'
      '('
      ':ID, :ADMINISTRATIONID, :ORIGINALID,  :STATUS,  :STAND_ID,'
      ':LATITUDE, :LONGITUDE, '
      ':SYSTEMCAST_ID, :TYPESYSTEM, '
      
        ':CHANNEL_ID, :VIDEO_CARRIER, :SOUND_CARRIER_PRIMARY, :SOUND_CARR' +
        'IER_SECOND, '
      ':ALLOTMENTBLOCKDAB_ID, :BLOCKCENTREFREQ, '
      ':POWER_VIDEO, :EPR_VIDEO_MAX,  :EPR_VIDEO_VERT, :EPR_VIDEO_HOR,'
      ':POWER_SOUND_PRIMARY, :POWER_SOUND_SECOND,'
      ':EPR_SOUND_MAX_PRIMARY, :EPR_SOUND_MAX_SECOND,'
      ':EPR_SOUND_HOR_PRIMARY, :EPR_SOUND_HOR_SECOND,'
      ':EPR_SOUND_VERT_PRIMARY, :EPR_SOUND_VER_SECOND,'
      ':V_SOUND_RATIO_PRIMARY, :V_SOUND_RATIO_SECOND,'
      
        ':ANTENNAGAIN, :HEIGHTANTENNA, :HEIGHT_EFF_MAX, :DIRECTION, :SYST' +
        'EMCOLOUR, 0.0, 0.0,'
      ':POLARIZATION'
      ')')
    Transaction = dmMain.trMain
    Left = 128
    Top = 112
  end
  object sqlNewAdminId: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select max(ADMINISTRATIONID)'
      'from TRANSMITTERS t'
      'right outer join STAND s on (t.STAND_ID = s.ID)'
      'where s.AREA_ID = (select AREA_ID from STAND where ID = :ID)')
    Transaction = dmMain.trMain
    Left = 176
    Top = 112
  end
  object sqlRadioSystem: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, CODSYSTEM'
      'from ANALOGRADIOSYSTEM '
      'order by 2')
    Transaction = dmMain.trMain
    Left = 56
    Top = 352
  end
  object sqlChannelGrid: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAME'
      ' from FREQUENCYGRID'
      'order by 2')
    Transaction = dmMain.trMain
    Left = 176
    Top = 352
  end
end

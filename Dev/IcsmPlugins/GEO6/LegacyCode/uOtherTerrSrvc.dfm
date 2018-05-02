inherited frmOtherTerrSrvc: TfrmOtherTerrSrvc
  Left = 369
  Top = 130
  Caption = #1055#1088#1080#1089#1074#1086#1108#1085#1085#1103' i'#1085#1096#1086#1111' '#1087#1077#1088#1074#1080#1085#1085#1086#1111' '#1085#1072#1079#1077#1084#1085#1086#1111' '#1089#1083#1091#1078#1073#1080
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  object gbTransmissionSite: TGroupBox [0]
    Left = 8
    Top = 232
    Width = 257
    Height = 145
    Caption = ' '#1056#1086#1079#1084'i'#1097#1077#1085#1085#1103' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072' '
    TabOrder = 2
    object Label10: TLabel
      Left = 16
      Top = 56
      Width = 38
      Height = 13
      Caption = #1064#1080#1088#1086#1090#1072
      FocusControl = DBEdit10
    end
    object Label8: TLabel
      Left = 16
      Top = 16
      Width = 32
      Height = 13
      Caption = #1053#1072#1079#1074#1072
      FocusControl = DBEdit8
    end
    object Label9: TLabel
      Left = 120
      Top = 96
      Width = 34
      Height = 13
      Caption = #1050#1088#1072#1111#1085#1072
    end
    object Label11: TLabel
      Left = 120
      Top = 56
      Width = 43
      Height = 13
      Caption = #1044#1086#1074#1075#1086#1090#1072
      FocusControl = DBEdit11
    end
    object Label12: TLabel
      Left = 16
      Top = 96
      Width = 59
      Height = 13
      Caption = #1056#1072#1076'i'#1091#1089' '#1079#1086#1085#1080
      FocusControl = DBEdit12
    end
    object DBEdit8: TDBEdit
      Left = 16
      Top = 32
      Width = 233
      Height = 21
      DataField = 'TX_LOCATION'
      DataSource = dscObj
      TabOrder = 0
    end
    object DBEdit10: TDBEdit
      Left = 16
      Top = 72
      Width = 90
      Height = 21
      DataField = 'TX_LAT'
      DataSource = dscObj
      TabOrder = 1
    end
    object DBEdit11: TDBEdit
      Left = 120
      Top = 72
      Width = 90
      Height = 21
      DataField = 'TX_LON'
      DataSource = dscObj
      TabOrder = 2
    end
    object DBEdit12: TDBEdit
      Left = 16
      Top = 112
      Width = 90
      Height = 21
      DataField = 'TX_RADIUS'
      DataSource = dscObj
      TabOrder = 3
    end
    object cbTxCtry: TDBComboBox
      Left = 120
      Top = 112
      Width = 70
      Height = 21
      DataField = 'TX_COUNTRY'
      DataSource = dscObj
      DropDownCount = 30
      ItemHeight = 13
      TabOrder = 4
    end
  end
  object gbGeneral: TGroupBox [2]
    Left = 8
    Top = 0
    Width = 257
    Height = 225
    Caption = ' '#1047#1072#1075#1072#1083#1100#1085#1077' '
    TabOrder = 1
    object Label1: TLabel
      Left = 16
      Top = 16
      Width = 21
      Height = 13
      Caption = #1040#1076#1084
    end
    object Label2: TLabel
      Left = 112
      Top = 16
      Width = 107
      Height = 13
      Caption = 'I'#1076#1077#1085#1090#1080#1092'i'#1082#1072#1094'i'#1081#1085#1080#1081' '#1082#1086#1076
      FocusControl = DBEdit2
    end
    object Label3: TLabel
      Left = 16
      Top = 56
      Width = 50
      Height = 13
      Caption = #1055#1086#1079#1080#1074#1085#1080#1081
      FocusControl = DBEdit3
    end
    object Label4: TLabel
      Left = 16
      Top = 96
      Width = 69
      Height = 13
      Caption = #1063#1072#1089#1090#1086#1090#1072', '#1052#1043#1094
      FocusControl = DBEdit4
    end
    object Label5: TLabel
      Left = 128
      Top = 96
      Width = 119
      Height = 13
      Caption = #1055#1086#1089#1080#1083#1100#1085#1072' '#1095#1072#1089#1090#1086#1090#1072', '#1052#1043#1094
      FocusControl = DBEdit5
    end
    object Label6: TLabel
      Left = 16
      Top = 136
      Width = 105
      Height = 13
      Caption = #1042#1074#1086#1076' '#1074' '#1077#1082#1089#1087#1083#1091#1072#1090#1072#1094'i'#1102
      FocusControl = DBEdit6
    end
    object Label7: TLabel
      Left = 128
      Top = 136
      Width = 56
      Height = 13
      Caption = #1047#1072#1082'i'#1085#1095#1077#1085#1085#1103
      FocusControl = DBEdit7
    end
    object Label18: TLabel
      Left = 16
      Top = 176
      Width = 62
      Height = 13
      Caption = #1050#1083#1072#1089' '#1089#1090#1072#1085#1094'i'#1111
      FocusControl = DBEdit18
    end
    object Label19: TLabel
      Left = 128
      Top = 176
      Width = 38
      Height = 13
      Caption = #1057#1083#1091#1078#1073#1072
      FocusControl = DBEdit19
    end
    object DBEdit7: TDBEdit
      Left = 128
      Top = 152
      Width = 90
      Height = 21
      DataField = 'DATE_EXPIRE'
      DataSource = dscObj
      TabOrder = 1
    end
    object DBEdit2: TDBEdit
      Left = 112
      Top = 32
      Width = 134
      Height = 21
      DataField = 'ADM_REF_ID'
      DataSource = dscObj
      TabOrder = 2
    end
    object DBEdit3: TDBEdit
      Left = 16
      Top = 72
      Width = 134
      Height = 21
      DataField = 'CALL_SIGN'
      DataSource = dscObj
      TabOrder = 3
    end
    object DBEdit4: TDBEdit
      Left = 16
      Top = 112
      Width = 90
      Height = 21
      DataField = 'FREQ'
      DataSource = dscObj
      TabOrder = 4
    end
    object DBEdit5: TDBEdit
      Left = 128
      Top = 112
      Width = 90
      Height = 21
      DataField = 'REF_FREQ'
      DataSource = dscObj
      TabOrder = 5
    end
    object DBEdit6: TDBEdit
      Left = 16
      Top = 152
      Width = 90
      Height = 21
      DataField = 'DATE_INTO_USE'
      DataSource = dscObj
      TabOrder = 6
    end
    object DBEdit18: TDBEdit
      Left = 16
      Top = 192
      Width = 56
      Height = 21
      DataField = 'STA_CLASS'
      DataSource = dscObj
      TabOrder = 7
    end
    object DBEdit19: TDBEdit
      Left = 128
      Top = 192
      Width = 56
      Height = 21
      DataField = 'SERVICE_NATURE'
      DataSource = dscObj
      TabOrder = 8
    end
    object cbAdm: TDBComboBox
      Left = 16
      Top = 32
      Width = 70
      Height = 21
      DataField = 'ADM'
      DataSource = dscObj
      DropDownCount = 30
      ItemHeight = 13
      TabOrder = 0
    end
  end
  object gbReceptionSite: TGroupBox [3]
    Left = 272
    Top = 232
    Width = 257
    Height = 145
    Caption = ' '#1056#1086#1079#1084'i'#1097#1077#1085#1085#1103' '#1087#1088#1080#1081#1084#1072#1095#1072' '
    TabOrder = 3
    object Label13: TLabel
      Left = 16
      Top = 16
      Width = 32
      Height = 13
      Caption = #1053#1072#1079#1074#1072
      FocusControl = DBEdit13
    end
    object Label14: TLabel
      Left = 120
      Top = 96
      Width = 34
      Height = 13
      Caption = #1050#1088#1072#1111#1085#1072
    end
    object Label15: TLabel
      Left = 16
      Top = 56
      Width = 38
      Height = 13
      Caption = #1064#1080#1088#1086#1090#1072
      FocusControl = DBEdit15
    end
    object Label16: TLabel
      Left = 120
      Top = 56
      Width = 43
      Height = 13
      Caption = #1044#1086#1074#1075#1086#1090#1072
      FocusControl = DBEdit16
    end
    object Label17: TLabel
      Left = 16
      Top = 96
      Width = 59
      Height = 13
      Caption = #1056#1072#1076'i'#1091#1089' '#1079#1086#1085#1080
      FocusControl = DBEdit17
    end
    object DBEdit17: TDBEdit
      Left = 16
      Top = 112
      Width = 90
      Height = 21
      DataField = 'RX_RADIUS'
      DataSource = dscObj
      TabOrder = 3
    end
    object DBEdit13: TDBEdit
      Left = 16
      Top = 32
      Width = 233
      Height = 21
      DataField = 'RX_LOCATION'
      DataSource = dscObj
      TabOrder = 0
    end
    object DBEdit15: TDBEdit
      Left = 16
      Top = 72
      Width = 90
      Height = 21
      DataField = 'RX_LAT'
      DataSource = dscObj
      TabOrder = 1
    end
    object DBEdit16: TDBEdit
      Left = 120
      Top = 72
      Width = 90
      Height = 21
      DataField = 'RX_LON'
      DataSource = dscObj
      TabOrder = 2
    end
    object cbRxCtry: TDBComboBox
      Left = 120
      Top = 112
      Width = 70
      Height = 21
      DataField = 'RX_COUNTRY'
      DataSource = dscObj
      DropDownCount = 30
      ItemHeight = 13
      TabOrder = 4
    end
  end
  object gbSystem: TGroupBox [4]
    Left = 8
    Top = 384
    Width = 257
    Height = 121
    Caption = ' '#1057#1080#1089#1090#1077#1084#1072' '
    TabOrder = 4
    object Label20: TLabel
      Left = 16
      Top = 16
      Width = 19
      Height = 13
      Caption = #1058#1080#1087
      FocusControl = DBEdit20
    end
    object Label21: TLabel
      Left = 88
      Top = 16
      Width = 41
      Height = 13
      Caption = #1055#1086#1090#1091#1078#1085'.'
      FocusControl = DBEdit21
    end
    object Label22: TLabel
      Left = 152
      Top = 16
      Width = 93
      Height = 13
      Caption = #1042#1080#1093'. '#1087#1086#1090#1091#1078#1085'., dBW'
      FocusControl = DBEdit22
    end
    object Label23: TLabel
      Left = 16
      Top = 56
      Width = 113
      Height = 33
      AutoSize = False
      Caption = #1052#1072#1082#1089' '#1087#1083#1086#1090#1085'. '#1087#1086#1090#1091#1078#1085'., dB(W/Hz)'
      FocusControl = DBEdit23
      WordWrap = True
    end
    object Label24: TLabel
      Left = 152
      Top = 72
      Width = 85
      Height = 13
      Caption = #1052#1072#1082#1089'. '#1045#1042#1055', dBW'
      FocusControl = DBEdit24
    end
    object DBEdit20: TDBEdit
      Left = 16
      Top = 32
      Width = 56
      Height = 21
      DataField = 'SYSTEM_TYPE'
      DataSource = dscObj
      TabOrder = 0
    end
    object DBEdit21: TDBEdit
      Left = 88
      Top = 32
      Width = 41
      Height = 21
      DataField = 'TYPE_OF_POWER'
      DataSource = dscObj
      TabOrder = 1
    end
    object DBEdit22: TDBEdit
      Left = 152
      Top = 32
      Width = 90
      Height = 21
      DataField = 'TX_OUT_POWER'
      DataSource = dscObj
      TabOrder = 2
    end
    object DBEdit23: TDBEdit
      Left = 16
      Top = 88
      Width = 90
      Height = 21
      DataField = 'MAX_PWR_DENS'
      DataSource = dscObj
      TabOrder = 3
    end
    object DBEdit24: TDBEdit
      Left = 152
      Top = 88
      Width = 90
      Height = 21
      DataField = 'ERP_MAX'
      DataSource = dscObj
      TabOrder = 4
    end
  end
  object gbAnt: TGroupBox [5]
    Left = 272
    Top = 0
    Width = 257
    Height = 225
    Caption = ' '#1040#1085#1090#1077#1085#1072' '
    TabOrder = 5
    object Label25: TLabel
      Left = 16
      Top = 16
      Width = 101
      Height = 13
      Caption = #1052#1072#1082#1089' '#1050#1059', '#1074'i'#1076#1085#1086#1089#1085#1080#1081
      FocusControl = DBEdit25
    end
    object Label26: TLabel
      Left = 16
      Top = 56
      Width = 20
      Height = 13
      Caption = #1055#1086#1083
    end
    object Label27: TLabel
      Left = 120
      Top = 56
      Width = 74
      Height = 13
      Caption = #1042#1080#1089#1086#1090#1072' '#1072#1085#1090#1077#1085#1080
      FocusControl = DBEdit27
    end
    object Label28: TLabel
      Left = 68
      Top = 56
      Width = 26
      Height = 13
      Caption = #1053#1072#1087#1088
    end
    object Label29: TLabel
      Left = 120
      Top = 16
      Width = 100
      Height = 13
      Caption = #1043#1086#1083#1086#1074#1085#1080#1081' '#1087#1077#1083#1102#1089#1090#1086#1082
      FocusControl = DBEdit29
    end
    object Label30: TLabel
      Left = 16
      Top = 96
      Width = 68
      Height = 13
      Caption = #1043#1086#1088#1080#1079#1086#1085#1090'. '#1050#1059
      FocusControl = DBEdit30
    end
    object Label31: TLabel
      Left = 120
      Top = 96
      Width = 67
      Height = 13
      Caption = #1042#1080#1089#1086#1090#1072' '#1084'i'#1089#1094#1103
      FocusControl = DBEdit31
    end
    object Label33: TLabel
      Left = 16
      Top = 177
      Width = 37
      Height = 13
      Caption = #1040#1079#1080#1084#1091#1090
      FocusControl = edAzm
    end
    object Label34: TLabel
      Left = 120
      Top = 136
      Width = 61
      Height = 13
      Caption = #1053#1072#1095'. '#1072#1079#1080#1084#1091#1090
      FocusControl = edAzmStart
    end
    object Label35: TLabel
      Left = 120
      Top = 176
      Width = 63
      Height = 13
      Caption = #1050'i'#1085#1094'. '#1072#1079#1080#1084#1091#1090
      FocusControl = edAzmEnd
    end
    object DBEdit25: TDBEdit
      Left = 16
      Top = 32
      Width = 90
      Height = 21
      DataField = 'MAX_ANT_GAIN'
      DataSource = dscObj
      TabOrder = 0
    end
    object DBEdit27: TDBEdit
      Left = 120
      Top = 72
      Width = 90
      Height = 21
      DataField = 'ANT_HEIGHT'
      DataSource = dscObj
      TabOrder = 4
    end
    object DBEdit29: TDBEdit
      Left = 120
      Top = 32
      Width = 90
      Height = 21
      DataField = 'MAIN_BEAM'
      DataSource = dscObj
      TabOrder = 1
    end
    object DBEdit30: TDBEdit
      Left = 16
      Top = 112
      Width = 90
      Height = 21
      DataField = 'GAIN'
      DataSource = dscObj
      TabOrder = 5
    end
    object DBEdit31: TDBEdit
      Left = 120
      Top = 112
      Width = 90
      Height = 21
      DataField = 'SITE_ALT'
      DataSource = dscObj
      TabOrder = 6
    end
    object edAzm: TDBEdit
      Left = 16
      Top = 192
      Width = 90
      Height = 21
      DataField = 'AZM'
      DataSource = dscObj
      TabOrder = 8
    end
    object edAzmStart: TDBEdit
      Left = 120
      Top = 152
      Width = 90
      Height = 21
      DataField = 'AZM_MAX'
      DataSource = dscObj
      TabOrder = 9
    end
    object edAzmEnd: TDBEdit
      Left = 120
      Top = 192
      Width = 90
      Height = 21
      DataField = 'AZM_MIN'
      DataSource = dscObj
      TabOrder = 10
    end
    object chRotating: TCheckBox
      Left = 16
      Top = 152
      Width = 97
      Height = 17
      Caption = #1050#1088#1091#1090#1080#1090#1100#1089#1103
      TabOrder = 7
      OnClick = chRotatingClick
    end
    object DBComboBox1: TDBComboBox
      Left = 16
      Top = 72
      Width = 41
      Height = 21
      Style = csDropDownList
      DataField = 'POL'
      DataSource = dscObj
      ItemHeight = 13
      Items.Strings = (
        'H'
        'V'
        'M'
        'U')
      TabOrder = 2
    end
    object DBComboBox2: TDBComboBox
      Left = 64
      Top = 72
      Width = 41
      Height = 21
      Style = csDropDownList
      DataField = 'DIR'
      DataSource = dscObj
      ItemHeight = 13
      Items.Strings = (
        'D'
        'ND')
      TabOrder = 3
    end
  end
  object gbEah: TGroupBox [6]
    Left = 536
    Top = 0
    Width = 145
    Height = 377
    Caption = ' '#1069#1092#1077#1082#1090#1080#1074#1085'i '#1074#1080#1089#1086#1090#1080' '#1072#1085#1090#1077#1085#1080' '
    TabOrder = 6
    object Label32: TLabel
      Left = 8
      Top = 16
      Width = 58
      Height = 13
      Caption = 'Max '#1045#1042#1040', '#1084
      FocusControl = DBEdit32
    end
    object Label36: TLabel
      Left = 8
      Top = 56
      Width = 72
      Height = 13
      Caption = #1055#1086' '#1085#1072#1087#1088#1103#1084#1082#1072#1093
    end
    object DBEdit32: TDBEdit
      Left = 8
      Top = 32
      Width = 65
      Height = 21
      DataField = 'MAX_EAH'
      DataSource = dscObj
      TabOrder = 0
    end
    object sgEah: TStringGrid
      Left = 8
      Top = 72
      Width = 129
      Height = 297
      ColCount = 2
      DefaultColWidth = 50
      DefaultRowHeight = 16
      RowCount = 37
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
      TabOrder = 1
      OnDrawCell = sgEahDrawCell
      OnExit = sgEahExit
      OnSetEditText = sgEahSetEditText
      RowHeights = (
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16
        16)
    end
    object btAllMax: TButton
      Left = 82
      Top = 32
      Width = 54
      Height = 21
      Caption = #1042#1089'i '#1084#1072#1082#1089
      TabOrder = 2
      OnClick = btAllMaxClick
    end
  end
  object gbOther: TGroupBox [7]
    Left = 272
    Top = 384
    Width = 409
    Height = 121
    Caption = ' '#1056'i'#1079#1085#1077' '
    TabOrder = 7
    object Label37: TLabel
      Left = 8
      Top = 16
      Width = 73
      Height = 13
      Caption = 'Start time of Op'
      FocusControl = DBEdit36
    end
    object Label38: TLabel
      Left = 112
      Top = 16
      Width = 70
      Height = 13
      Caption = 'End time of Op'
      FocusControl = DBEdit37
    end
    object Label39: TLabel
      Left = 8
      Top = 67
      Width = 49
      Height = 13
      Caption = #1054#1087#1077#1088#1072#1090#1086#1088
      FocusControl = DBEdit38
    end
    object Label40: TLabel
      Left = 16
      Top = 91
      Width = 37
      Height = 13
      Caption = #1040#1076#1088#1077#1089#1072
      FocusControl = DBEdit39
    end
    object DBEdit36: TDBEdit
      Left = 8
      Top = 32
      Width = 90
      Height = 21
      DataField = 'OP_HH_FR'
      DataSource = dscObj
      TabOrder = 0
    end
    object DBEdit37: TDBEdit
      Left = 112
      Top = 32
      Width = 90
      Height = 21
      DataField = 'OP_HH_TO'
      DataSource = dscObj
      TabOrder = 1
    end
    object DBCheckBox1: TDBCheckBox
      Left = 264
      Top = 16
      Width = 137
      Height = 17
      Caption = #1055#1086#1090#1088'i'#1073#1085#1072' '#1082#1086#1086#1088#1076#1080#1085#1072#1094'i'#1103
      DataField = 'COORD_REQ'
      DataSource = dscObj
      TabOrder = 2
      ValueChecked = '1'
      ValueUnchecked = '0'
    end
    object DBCheckBox2: TDBCheckBox
      Left = 264
      Top = 40
      Width = 113
      Height = 17
      Caption = #1055'i'#1076#1090#1074#1077#1088#1078#1076#1077#1085#1085#1103
      DataField = 'SIGNED_COMM'
      DataSource = dscObj
      TabOrder = 3
      ValueChecked = '1'
      ValueUnchecked = '0'
    end
    object DBEdit38: TDBEdit
      Left = 64
      Top = 64
      Width = 337
      Height = 21
      DataField = 'OPR_AGENCY'
      DataSource = dscObj
      TabOrder = 4
    end
    object DBEdit39: TDBEdit
      Left = 64
      Top = 88
      Width = 337
      Height = 21
      DataField = 'ADM_ADDRESS'
      DataSource = dscObj
      TabOrder = 5
    end
  end
  inherited dstObj: TIBDataSet
    AfterOpen = dstObjAfterOpen
    OnNewRecord = dstObjNewRecord
    InsertSQL.Strings = (
      'insert into OTHER_PRIM_TERR'
      
        '  (ID, ADM, ADM_REF_ID, CALL_SIGN, FREQ, REF_FREQ, DATE_INTO_USE' +
        ', '
      'DATE_EXPIRE, '
      
        '   TX_LOCATION, TX_COUNTRY, TX_LAT, TX_LON, TX_RADIUS, RX_LOCATI' +
        'ON, '
      'RX_COUNTRY, '
      '   RX_LAT, RX_LON, RX_RADIUS, STA_CLASS, SERVICE_NATURE, '
      'EMISSION_CLASS, '
      '   SYSTEM_TYPE, TYPE_OF_POWER, TX_OUT_POWER, MAX_PWR_DENS, '
      'ERP_MAX, MAX_ANT_GAIN, '
      
        '   POL, ANT_HEIGHT, DIR, MAIN_BEAM, GAIN, SITE_ALT, MAX_EAH, AZM' +
        ', '
      'AZM_MAX, '
      '   AZM_MIN, OP_HH_FR, OP_HH_TO, COORD_REQ, SIGNED_COMM, '
      'OPR_AGENCY, ADM_ADDRESS, '
      '   REMARKS, EAH)'
      'values'
      
        '  (:ID, :ADM, :ADM_REF_ID, :CALL_SIGN, :FREQ, :REF_FREQ, :DATE_I' +
        'NTO_USE,'
      ':DATE_EXPIRE,'
      '   :TX_LOCATION, :TX_COUNTRY, :TX_LAT, :TX_LON, :TX_RADIUS,'
      ':RX_LOCATION,'
      '   :RX_COUNTRY, :RX_LAT, :RX_LON, :RX_RADIUS, :STA_CLASS,'
      ':SERVICE_NATURE,'
      '   :EMISSION_CLASS, :SYSTEM_TYPE, :TYPE_OF_POWER, :TX_OUT_POWER,'
      ':MAX_PWR_DENS,'
      
        '   :ERP_MAX, :MAX_ANT_GAIN, :POL, :ANT_HEIGHT, :DIR, :MAIN_BEAM,' +
        ' :GAIN,'
      
        '   :SITE_ALT, :MAX_EAH, :AZM, :AZM_MAX, :AZM_MIN, :OP_HH_FR, :OP' +
        '_HH_TO,'
      
        '   :COORD_REQ, :SIGNED_COMM, :OPR_AGENCY, :ADM_ADDRESS, :REMARKS' +
        ', null)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  ADM,'
      '  ADM_REF_ID,'
      '  CALL_SIGN,'
      '  FREQ,'
      '  REF_FREQ,'
      '  DATE_INTO_USE,'
      '  DATE_EXPIRE,'
      '  TX_LOCATION,'
      '  TX_COUNTRY,'
      '  TX_LAT,'
      '  TX_LON,'
      '  TX_RADIUS,'
      '  RX_LOCATION,'
      '  RX_COUNTRY,'
      '  RX_LAT,'
      '  RX_LON,'
      '  RX_RADIUS,'
      '  STA_CLASS,'
      '  SERVICE_NATURE,'
      '  EMISSION_CLASS,'
      '  SYSTEM_TYPE,'
      '  TYPE_OF_POWER,'
      '  TX_OUT_POWER,'
      '  MAX_PWR_DENS,'
      '  ERP_MAX,'
      '  MAX_ANT_GAIN,'
      '  POL,'
      '  ANT_HEIGHT,'
      '  DIR,'
      '  MAIN_BEAM,'
      '  GAIN,'
      '  SITE_ALT,'
      '  MAX_EAH,'
      '  EAH,'
      '  AZM,'
      '  AZM_MAX,'
      '  AZM_MIN,'
      '  OP_HH_FR,'
      '  OP_HH_TO,'
      '  COORD_REQ,'
      '  SIGNED_COMM,'
      '  OPR_AGENCY,'
      '  ADM_ADDRESS,'
      '  REMARKS'
      'from OTHER_PRIM_TERR '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select * from OTHER_PRIM_TERR'
      'where ID = :OBJ_ID')
    ModifySQL.Strings = (
      'update OTHER_PRIM_TERR'
      'set'
      '  ADM = :ADM,'
      '  ADM_REF_ID = :ADM_REF_ID,'
      '  CALL_SIGN = :CALL_SIGN,'
      '  FREQ = :FREQ,'
      '  REF_FREQ = :REF_FREQ,'
      '  DATE_INTO_USE = :DATE_INTO_USE,'
      '  DATE_EXPIRE = :DATE_EXPIRE,'
      '  TX_LOCATION = :TX_LOCATION,'
      '  TX_COUNTRY = :TX_COUNTRY,'
      '  TX_LAT = :TX_LAT,'
      '  TX_LON = :TX_LON,'
      '  TX_RADIUS = :TX_RADIUS,'
      '  RX_LOCATION = :RX_LOCATION,'
      '  RX_COUNTRY = :RX_COUNTRY,'
      '  RX_LAT = :RX_LAT,'
      '  RX_LON = :RX_LON,'
      '  RX_RADIUS = :RX_RADIUS,'
      '  STA_CLASS = :STA_CLASS,'
      '  SERVICE_NATURE = :SERVICE_NATURE,'
      '  EMISSION_CLASS = :EMISSION_CLASS,'
      '  SYSTEM_TYPE = :SYSTEM_TYPE,'
      '  TYPE_OF_POWER = :TYPE_OF_POWER,'
      '  TX_OUT_POWER = :TX_OUT_POWER,'
      '  MAX_PWR_DENS = :MAX_PWR_DENS,'
      '  ERP_MAX = :ERP_MAX,'
      '  MAX_ANT_GAIN = :MAX_ANT_GAIN,'
      '  POL = :POL,'
      '  ANT_HEIGHT = :ANT_HEIGHT,'
      '  DIR = :DIR,'
      '  MAIN_BEAM = :MAIN_BEAM,'
      '  GAIN = :GAIN,'
      '  SITE_ALT = :SITE_ALT,'
      '  MAX_EAH = :MAX_EAH,'
      '  AZM = :AZM,'
      '  AZM_MAX = :AZM_MAX,'
      '  AZM_MIN = :AZM_MIN,'
      '  OP_HH_FR = :OP_HH_FR,'
      '  OP_HH_TO = :OP_HH_TO,'
      '  COORD_REQ = :COORD_REQ,'
      '  SIGNED_COMM = :SIGNED_COMM,'
      '  OPR_AGENCY = :OPR_AGENCY,'
      '  ADM_ADDRESS = :ADM_ADDRESS,'
      '  REMARKS = :REMARKS'
      'where'
      '  ID = :OLD_ID')
    object dstObjID: TIntegerField
      FieldName = 'ID'
      Origin = 'OTHER_PRIM_TERR.ID'
      Required = True
    end
    object dstObjADM: TIBStringField
      DisplayLabel = 'ITU symbol of the notifying administration'
      DisplayWidth = 4
      FieldName = 'ADM'
      Origin = 'OTHER_PRIM_TERR.ADM'
      Size = 3
    end
    object dstObjADM_REF_ID: TIBStringField
      DisplayLabel = 'Indentification code'
      FieldName = 'ADM_REF_ID'
      Origin = 'OTHER_PRIM_TERR.ADM_REF_ID'
      Size = 30
    end
    object dstObjCALL_SIGN: TIBStringField
      DisplayLabel = 'Call sign'
      FieldName = 'CALL_SIGN'
      Origin = 'OTHER_PRIM_TERR.CALL_SIGN'
      Size = 10
    end
    object dstObjFREQ: TFloatField
      DisplayLabel = 'Assigned frequency'
      FieldName = 'FREQ'
      Origin = 'OTHER_PRIM_TERR.FREQ'
    end
    object dstObjREF_FREQ: TFloatField
      DisplayLabel = 'The reference frequency'
      FieldName = 'REF_FREQ'
      Origin = 'OTHER_PRIM_TERR.REF_FREQ'
    end
    object dstObjDATE_INTO_USE: TDateField
      DisplayLabel = 'Date of bringing into use'
      FieldName = 'DATE_INTO_USE'
      Origin = 'OTHER_PRIM_TERR.DATE_INTO_USE'
    end
    object dstObjDATE_EXPIRE: TDateField
      DisplayLabel = 'Expiration date'
      FieldName = 'DATE_EXPIRE'
      Origin = 'OTHER_PRIM_TERR.DATE_EXPIRE'
    end
    object dstObjTX_LOCATION: TIBStringField
      DisplayLabel = 'Name of the location of the transmitting station'
      FieldName = 'TX_LOCATION'
      Origin = 'OTHER_PRIM_TERR.TX_LOCATION'
      Size = 128
    end
    object dstObjTX_COUNTRY: TIBStringField
      DisplayLabel = 'Country or geographical area'
      FieldName = 'TX_COUNTRY'
      Origin = 'OTHER_PRIM_TERR.TX_COUNTRY'
      Size = 3
    end
    object dstObjTX_LAT: TFloatField
      DisplayLabel = 'Latitude of transmitting site (center of transmitting area)'
      FieldName = 'TX_LAT'
      Origin = 'OTHER_PRIM_TERR.TX_LAT'
      OnGetText = CoordFieldGetText
      OnSetText = CoordFieldSetText
    end
    object dstObjTX_LON: TFloatField
      DisplayLabel = 'Longitude of transmitting site (center of transmitting area)'
      FieldName = 'TX_LON'
      Origin = 'OTHER_PRIM_TERR.TX_LON'
      OnGetText = CoordFieldGetText
      OnSetText = CoordFieldSetText
    end
    object dstObjTX_RADIUS: TFloatField
      DisplayLabel = 'Nominal radius of the circular area'
      FieldName = 'TX_RADIUS'
      Origin = 'OTHER_PRIM_TERR.TX_RADIUS'
    end
    object dstObjRX_LOCATION: TIBStringField
      DisplayLabel = 'Name of the location of the receiving station'
      FieldName = 'RX_LOCATION'
      Origin = 'OTHER_PRIM_TERR.RX_LOCATION'
      Size = 128
    end
    object dstObjRX_COUNTRY: TIBStringField
      DisplayLabel = 'Country or geographical area of reception'
      FieldName = 'RX_COUNTRY'
      Origin = 'OTHER_PRIM_TERR.RX_COUNTRY'
      Size = 3
    end
    object dstObjRX_LAT: TFloatField
      DisplayLabel = 'Latitude of receiving site (center of receiving area)'
      FieldName = 'RX_LAT'
      Origin = 'OTHER_PRIM_TERR.RX_LAT'
      OnGetText = CoordFieldGetText
      OnSetText = CoordFieldSetText
    end
    object dstObjRX_LON: TFloatField
      DisplayLabel = 'Longitude of receiving site (center of receiving area)'
      FieldName = 'RX_LON'
      Origin = 'OTHER_PRIM_TERR.RX_LON'
      OnGetText = CoordFieldGetText
      OnSetText = CoordFieldSetText
    end
    object dstObjRX_RADIUS: TFloatField
      DisplayLabel = 'Nominal radius of the circular receiving area'
      FieldName = 'RX_RADIUS'
      Origin = 'OTHER_PRIM_TERR.RX_RADIUS'
    end
    object dstObjSTA_CLASS: TIBStringField
      DisplayLabel = 'Class of station'
      FieldName = 'STA_CLASS'
      Origin = 'OTHER_PRIM_TERR.STA_CLASS'
      Size = 4
    end
    object dstObjSERVICE_NATURE: TIBStringField
      DisplayLabel = 'Nature of service performed'
      FieldName = 'SERVICE_NATURE'
      Origin = 'OTHER_PRIM_TERR.SERVICE_NATURE'
      Size = 4
    end
    object dstObjEMISSION_CLASS: TIBStringField
      DisplayLabel = 'Class of emission,'
      FieldName = 'EMISSION_CLASS'
      Origin = 'OTHER_PRIM_TERR.EMISSION_CLASS'
      FixedChar = True
      Size = 9
    end
    object dstObjSYSTEM_TYPE: TIBStringField
      DisplayLabel = 'System type code'
      FieldName = 'SYSTEM_TYPE'
      Origin = 'OTHER_PRIM_TERR.SYSTEM_TYPE'
      Size = 4
    end
    object dstObjTYPE_OF_POWER: TIBStringField
      DisplayLabel = 'The type of power'
      FieldName = 'TYPE_OF_POWER'
      Origin = 'OTHER_PRIM_TERR.TYPE_OF_POWER'
      FixedChar = True
      Size = 1
    end
    object dstObjTX_OUT_POWER: TFloatField
      DisplayLabel = 'The transmitter output power (dBW)'
      FieldName = 'TX_OUT_POWER'
      Origin = 'OTHER_PRIM_TERR.TX_OUT_POWER'
    end
    object dstObjMAX_PWR_DENS: TFloatField
      DisplayLabel = 
        'Maximum power density (dB(W/Hz)) averaged over the worst 4 kHz b' +
        'and supplied to the antenna transmission line'
      FieldName = 'MAX_PWR_DENS'
      Origin = 'OTHER_PRIM_TERR.MAX_PWR_DENS'
    end
    object dstObjERP_MAX: TFloatField
      DisplayLabel = 'The maximum effective radiated power (dBW)'
      FieldName = 'ERP_MAX'
      Origin = 'OTHER_PRIM_TERR.ERP_MAX'
    end
    object dstObjMAX_ANT_GAIN: TFloatField
      DisplayLabel = 'Maximum antenna gain, relative to a half-wave dipole'
      FieldName = 'MAX_ANT_GAIN'
      Origin = 'OTHER_PRIM_TERR.MAX_ANT_GAIN'
    end
    object dstObjPOL: TIBStringField
      DisplayLabel = 'Polarization'
      FieldName = 'POL'
      Origin = 'OTHER_PRIM_TERR.POL'
      FixedChar = True
      Size = 1
    end
    object dstObjANT_HEIGHT: TIntegerField
      DisplayLabel = 'Height of antenna above ground level'
      FieldName = 'ANT_HEIGHT'
      Origin = 'OTHER_PRIM_TERR.ANT_HEIGHT'
    end
    object dstObjDIR: TIBStringField
      DisplayLabel = 'Antenna directivity'
      FieldName = 'DIR'
      Origin = 'OTHER_PRIM_TERR.DIR'
      FixedChar = True
      Size = 2
    end
    object dstObjMAIN_BEAM: TFloatField
      DisplayLabel = 'The total angular width of the radiation main lobe'
      FieldName = 'MAIN_BEAM'
      Origin = 'OTHER_PRIM_TERR.MAIN_BEAM'
    end
    object dstObjGAIN: TFloatField
      DisplayLabel = 'Antenna gain towards the local horizon'
      FieldName = 'GAIN'
      Origin = 'OTHER_PRIM_TERR.GAIN'
    end
    object dstObjSITE_ALT: TIntegerField
      DisplayLabel = 'Altitude of the site above sea level'
      FieldName = 'SITE_ALT'
      Origin = 'OTHER_PRIM_TERR.SITE_ALT'
    end
    object dstObjMAX_EAH: TIntegerField
      DisplayLabel = 'Maximum effective antenna height'
      FieldName = 'MAX_EAH'
      Origin = 'OTHER_PRIM_TERR.MAX_EAH'
    end
    object dstObjAZM: TFloatField
      DisplayLabel = 'Azimuth of maximum radiation of not rotating antenna'
      FieldName = 'AZM'
      Origin = 'OTHER_PRIM_TERR.AZM'
    end
    object dstObjAZM_MAX: TFloatField
      DisplayLabel = 'Start azimuth of rotating antenna'
      FieldName = 'AZM_MAX'
      Origin = 'OTHER_PRIM_TERR.AZM_MAX'
    end
    object dstObjAZM_MIN: TFloatField
      DisplayLabel = 'End azimuth of rotating antenna'
      FieldName = 'AZM_MIN'
      Origin = 'OTHER_PRIM_TERR.AZM_MIN'
    end
    object dstObjOP_HH_FR: TTimeField
      DisplayLabel = 'Regular hours (UTC) of operation - start time'
      FieldName = 'OP_HH_FR'
      Origin = 'OTHER_PRIM_TERR.OP_HH_FR'
    end
    object dstObjOP_HH_TO: TTimeField
      DisplayLabel = 'Regular hours (UTC) of operation - stop time'
      FieldName = 'OP_HH_TO'
      Origin = 'OTHER_PRIM_TERR.OP_HH_TO'
    end
    object dstObjCOORD_REQ: TSmallintField
      DisplayLabel = 'coordination is necessary'
      FieldName = 'COORD_REQ'
      Origin = 'OTHER_PRIM_TERR.COORD_REQ'
    end
    object dstObjSIGNED_COMM: TSmallintField
      DisplayLabel = 'Signed commitment from the notifying administration'
      FieldName = 'SIGNED_COMM'
      Origin = 'OTHER_PRIM_TERR.SIGNED_COMM'
    end
    object dstObjOPR_AGENCY: TIBStringField
      DisplayLabel = 'Operating agency'
      FieldName = 'OPR_AGENCY'
      Origin = 'OTHER_PRIM_TERR.OPR_AGENCY'
      Size = 64
    end
    object dstObjADM_ADDRESS: TIBStringField
      DisplayLabel = 'Address of the administration'
      FieldName = 'ADM_ADDRESS'
      Origin = 'OTHER_PRIM_TERR.ADM_ADDRESS'
      Size = 128
    end
    object dstObjREMARKS: TIBStringField
      DisplayLabel = 
        'Any comment designed to assist the Bureau in processing the noti' +
        'ce'
      FieldName = 'REMARKS'
      Origin = 'OTHER_PRIM_TERR.REMARKS'
      Size = 512
    end
  end
end

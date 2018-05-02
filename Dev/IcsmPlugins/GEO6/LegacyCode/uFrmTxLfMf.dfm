inherited frmTxLfMf: TfrmTxLfMf
  Left = 296
  Top = 178
  Caption = 'frmTxLfMf'
  ClientHeight = 513
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited lblTxType: TLabel
      Left = 8
      Width = 46
      Height = 23
      Caption = #1057#1061'/'#1044#1061
      Font.Name = 'Arial Narrow'
    end
  end
  inherited pcData: TPageControl
    Height = 423
    inherited tshCommon: TTabSheet
      inherited pnlMaster: TPanel
        Top = 231
        TabOrder = 3
      end
      inherited gbxCoordination: TGroupBox
        Top = 192
        TabOrder = 2
      end
      object pnEmission: TPanel
        Left = 0
        Top = 0
        Width = 772
        Height = 41
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 0
        object lbFreq: TLabel
          Left = 6
          Top = 18
          Width = 66
          Height = 13
          Caption = #1063#1072#1089#1090#1086#1090#1072', '#1082#1043#1094
        end
        object lbGndCond: TLabel
          Left = 165
          Top = 18
          Width = 75
          Height = 13
          Caption = #1055#1088#1086#1074#1086#1076'., mS/m'
        end
        object lbNoiseZone: TLabel
          Left = 359
          Top = 18
          Width = 67
          Height = 13
          Caption = #1064#1091#1084#1086#1074#1072' '#1079#1086#1085#1072
        end
        object edFreq: TNumericEdit
          Left = 80
          Top = 14
          Width = 69
          Height = 21
          TabOrder = 0
          Text = 'edtFreq'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtFreq'
          OnValueChange = edFreqValueChange
        end
        object edGndCond: TNumericEdit
          Left = 247
          Top = 14
          Width = 42
          Height = 21
          TabOrder = 1
          Text = '0'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = '0'
          OnValueChange = edGndCondValueChange
        end
        object edNoiseZone: TNumericEdit
          Left = 433
          Top = 14
          Width = 24
          Height = 21
          TabOrder = 3
          Text = '1'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = '1'
          OnValueChange = edNoiseZoneValueChange
        end
        object edGndCondCalc: TNumericEdit
          Left = 314
          Top = 14
          Width = 37
          Height = 21
          ParentColor = True
          TabOrder = 4
          Text = '0'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = '0'
        end
        object btSetGndCond: TButton
          Left = 291
          Top = 14
          Width = 21
          Height = 21
          Caption = '<-'
          TabOrder = 2
          OnClick = btSetGndCondClick
        end
        object edNoiseZoneCalc: TNumericEdit
          Left = 482
          Top = 14
          Width = 24
          Height = 21
          ParentColor = True
          TabOrder = 5
          Text = '0'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = '0'
        end
        object btSetNoiseZone: TButton
          Left = 459
          Top = 14
          Width = 21
          Height = 21
          Caption = '<-'
          TabOrder = 6
          OnClick = btSetNoiseZoneClick
        end
        object pnSfn: TPanel
          Left = 510
          Top = 5
          Width = 257
          Height = 32
          BevelOuter = bvNone
          TabOrder = 7
          object lblSynchr: TLabel
            Left = 177
            Top = 6
            Width = 42
            Height = 26
            Caption = #1057#1080#1085#1093#1088#1086', '#1084#1082#1089
            WordWrap = True
          end
          object lblOChS: TLabel
            Left = 4
            Top = 12
            Width = 25
            Height = 13
            Caption = #1054#1063#1052
          end
          object edtSfn: TDBEdit
            Left = 31
            Top = 8
            Width = 106
            Height = 21
            DataField = 'SYNHRONETID'
            ReadOnly = True
            TabOrder = 0
          end
          object btnSetSfn: TButton
            Left = 136
            Top = 8
            Width = 19
            Height = 20
            Caption = '...'
            TabOrder = 1
            OnClick = btnSetSfnClick
          end
          object btnDropSfnId: TButton
            Left = 155
            Top = 8
            Width = 19
            Height = 20
            Caption = 'X'
            TabOrder = 2
            OnClick = btnDropSfnIdClick
          end
          object edSynchro: TNumericEdit
            Left = 220
            Top = 8
            Width = 34
            Height = 21
            TabOrder = 3
            Text = 'edtSynchronization'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtSynchronization'
            OnValueChange = edSynchroValueChange
          end
        end
      end
      object pnLfmf: TPanel
        Left = 0
        Top = 41
        Width = 772
        Height = 151
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 1
        object pnDayNight: TPanel
          Left = 0
          Top = 0
          Width = 81
          Height = 151
          Align = alLeft
          BevelOuter = bvNone
          TabOrder = 0
          object lbOp: TLabel
            Left = 8
            Top = 8
            Width = 36
            Height = 13
            Caption = #1056#1086#1073#1086#1090#1072
          end
          object lbMode: TLabel
            Left = 8
            Top = 80
            Width = 35
            Height = 13
            Caption = #1056#1077#1078#1080#1084
          end
          object chDay: TCheckBox
            Left = 8
            Top = 24
            Width = 97
            Height = 17
            Caption = #1044#1077#1085#1100
            TabOrder = 0
            OnClick = chDayClick
          end
          object chNight: TCheckBox
            Left = 8
            Top = 48
            Width = 97
            Height = 17
            Caption = #1053'i'#1095
            TabOrder = 1
            OnClick = chNightClick
          end
          object rbDay: TRadioButton
            Left = 8
            Top = 96
            Width = 113
            Height = 17
            Caption = #1044#1077#1085#1100
            TabOrder = 2
            OnClick = rbDayNightClick
          end
          object rbNight: TRadioButton
            Left = 8
            Top = 120
            Width = 113
            Height = 17
            Caption = #1053'i'#1095
            TabOrder = 3
            OnClick = rbDayNightClick
          end
        end
        object gbEmission: TGroupBox
          Left = 520
          Top = 0
          Width = 249
          Height = 151
          Caption = ' '#1042#1080#1087#1088#1086#1084'i'#1085#1102#1074#1072#1085#1085#1103' '
          TabOrder = 4
          object lbMonoStereo: TLabel
            Left = 6
            Top = 44
            Width = 77
            Height = 13
            Caption = #1052#1086#1085#1086' / '#1057#1090#1077#1088#1077#1086' '
          end
          object lbEmissionClass: TLabel
            Left = 55
            Top = 75
            Width = 25
            Height = 13
            Caption = #1050#1083#1072#1089
          end
          object lbBw: TLabel
            Left = 18
            Top = 123
            Width = 62
            Height = 13
            Caption = #1055#1086#1083#1086#1089#1072', '#1082#1043#1094
          end
          object lbAdjRat: TLabel
            Left = 38
            Top = 93
            Width = 45
            Height = 26
            Alignment = taRightJustify
            Caption = #1057#1091#1084'i'#1078#1085#1077' '#1074'i'#1076#1085', '#1076#1041
            WordWrap = True
          end
          object lbSystem: TLabel
            Left = 36
            Top = 20
            Width = 44
            Height = 13
            Caption = #1057#1080#1089#1090#1077#1084#1072
          end
          object lbModType: TLabel
            Left = 28
            Top = 44
            Width = 52
            Height = 13
            Caption = #1052#1086#1076#1091#1083#1103#1094'i'#1103
          end
          object lbProtLevl: TLabel
            Left = 163
            Top = 44
            Width = 35
            Height = 13
            Caption = #1047#1072#1093#1080#1089#1090
          end
          object cbMonoStereo: TComboBox
            Left = 88
            Top = 40
            Width = 70
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 1
            OnChange = cbMonoStereoChange
            Items.Strings = (
              #1052#1086#1085#1086
              #1057#1090#1077#1088#1077#1086)
          end
          object btEmissionClass: TBitBtn
            Left = 166
            Top = 72
            Width = 19
            Height = 20
            Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
            ParentShowHint = False
            ShowHint = True
            TabOrder = 5
            OnClick = btEmissionClassClick
            Glyph.Data = {
              A6030000424DA603000000000000A60200002800000010000000100000000100
              08000000000000010000120B0000120B00009C0000009C00000000000000FFFF
              FF006E66A5006D64A10088739400FF00FF00977D92009D8397009C746900A174
              5F00AE602A00AF7F5600F8C9A20060554C0047413C005C5147005A514900CA8A
              4800C78D5500FCC79400F5C19000EEC09300FFD0A000D1AC8600EDC29900E9BF
              9700F5CAA000D4AF8C00584E440063584D00534A4100433E3900B97B3700B679
              3700CE904B00EFB06A00FEC07D00CAA27600B3916C008A715500D1AC8500FED1
              A200BC9B7800D1AD8600FFD4A5004F463C00C5853800E9A65700E3A45B00D39D
              5B00D3A16500A17B4D00D2A26800FDC47F00C0976800CDA574008B704F00D1AA
              7B00EE9B2C00FCAF4600E9A14200CA8D3D00BA833800EFAA4D00F6AF5200B988
              4700B3864900D69F5800FFBD6A00FBBD6D00E4AE6700FCC57B00DCAB6B00EAB7
              7700866A4600CFA46E00FFCF9200FFD1950052433000E08D1200EB931700F298
              1B00D2851A00F69F2000E3911E00F3A02800E89B2700C1822500FCAA3200F3A3
              3300D5902F00FCAB3C00E79D3700EDA54000DC9C3E00AD7A3100FFB74F00B581
              3900F1AE4F00B5843F00FFBD6000FFBD6100624A280055442B0054452F005043
              3000FF9F0000FC990000FB980000FE9D0100FB980100FE9D0200F6970200FF9F
              0400FB9A0500FFA20B00F89D1300FB9F1400FFA71600FEA31700FFA92200E697
              1F00FFAC2400E79F3100E9A43A00EAA94500EEB04F00FFBF5D00F2B65A00F2BA
              6200DAA03900E9B24D00F0B84D00EEB64D00EDB33900EFB53E00E7AC1D00E4A7
              0A00E6D39F00DED5A800E3DAB100E1DCB900DEDBBC00E1DEC000E2E0C1006CD7
              CE006BD3CA008DE0D9006CD7CF0066D7D1003047D3003148D200263EE3003448
              CC003649CC002E42DE0005050505050505050505050505050505050505050505
              05050505050505050505050A0A0A0A0A0A0A0A0A0A0A0A0A0A05824D49466462
              5E587875726C6E6B6A0A88151C1E314A1F3E5A69684E6766700A881625364543
              415D3C0B2E4F21206F0A88180D0F34382D63069B0311969A500A88292A263532
              333F2F04127708096D0A88191D1037270E42079802229799540A882C13144C24
              236560305B7A3A516A0A881A8F908E8D8C8B8A7F5B595374710A881B92919194
              949593473D615F57730A870C172B2B28394B4844403B5C55760A058986848583
              81807E7D7C7B5679520505050505050505050505050505050505050505050505
              05050505050505050505}
          end
          object edEmissionClass: TDBEdit
            Left = 88
            Top = 72
            Width = 70
            Height = 21
            DataSource = dsLfMfOper
            TabOrder = 4
          end
          object edBw: TNumericEdit
            Left = 88
            Top = 120
            Width = 70
            Height = 21
            TabOrder = 7
            Text = 'edtFreq'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtFreq'
            OnValueChange = edBwValueChange
          end
          object edAdjRat: TNumericEdit
            Left = 88
            Top = 96
            Width = 70
            Height = 21
            TabOrder = 6
            Text = 'edtGain'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtGain'
            OnValueChange = edAdjRatValueChange
          end
          object cbModType: TComboBox
            Left = 88
            Top = 40
            Width = 70
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 2
            OnChange = cbModTypeChange
            Items.Strings = (
              'QAM-16'
              'QAM-64')
          end
          object cbSys: TComboBox
            Left = 88
            Top = 16
            Width = 70
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 0
            OnChange = cbSysChange
          end
          object cbProtLevl: TComboBox
            Left = 200
            Top = 40
            Width = 41
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 3
            OnChange = cbProtLevlChange
            Items.Strings = (
              '0'
              '1'
              '2'
              '3')
          end
        end
        object gbPower: TGroupBox
          Left = 80
          Top = 0
          Width = 201
          Height = 81
          Caption = ' '#1055#1086#1090#1091#1078#1085'i'#1089#1090#1100' '
          TabOrder = 1
          object lbPwrKw: TLabel
            Left = 12
            Top = 19
            Width = 80
            Height = 13
            Caption = #1055#1086#1090#1091#1078#1085'i'#1089#1090#1100', kW'
          end
          object lbEmrp: TLabel
            Left = 25
            Top = 51
            Width = 67
            Height = 13
            Caption = 'EMRP, dBkW'
          end
          object edPwrKw: TNumericEdit
            Left = 96
            Top = 16
            Width = 49
            Height = 21
            TabOrder = 0
            Text = '0'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = '0'
            OnValueChange = edPwrKwValueChange
          end
          object edEmrp: TNumericEdit
            Left = 96
            Top = 48
            Width = 49
            Height = 21
            TabOrder = 1
            Text = '0'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = '0'
            OnValueChange = edEmrpValueChange
          end
          object btErp: TButton
            Left = 166
            Top = 48
            Width = 21
            Height = 21
            Caption = '...'
            TabOrder = 3
            Visible = False
            OnClick = btErpClick
          end
          object btSetEmrp: TButton
            Left = 145
            Top = 48
            Width = 21
            Height = 21
            Caption = '<-'
            TabOrder = 2
            OnClick = btSetEmrpClick
          end
        end
        object gbAntenna: TGroupBox
          Left = 288
          Top = 0
          Width = 225
          Height = 151
          Caption = ' '#1040#1085#1090#1077#1085#1072' '
          TabOrder = 3
          object lbAntType: TLabel
            Left = 35
            Top = 20
            Width = 57
            Height = 13
            Caption = #1058#1080#1087' '#1072#1085#1090#1077#1085#1080
          end
          object lbAgl: TLabel
            Left = 33
            Top = 51
            Width = 59
            Height = 13
            Caption = #1042#1080#1089#1086#1090#1072' '#1072#1085#1090'.'
          end
          object lbAzmMax: TLabel
            Left = 33
            Top = 109
            Width = 59
            Height = 26
            Alignment = taRightJustify
            Caption = #1040#1079#1080#1084#1091#1090' '#1084#1072#1082#1089'. '#1074#1080#1087#1088'.'
            WordWrap = True
          end
          object lbGainH: TLabel
            Left = 54
            Top = 83
            Width = 70
            Height = 13
            Caption = #1050#1059' '#1075#1086#1088'. '#1084#1072#1082#1089'.'
          end
          object btClearGainH: TButton
            Left = 187
            Top = 80
            Width = 21
            Height = 21
            Caption = 'X'
            TabOrder = 4
            OnClick = btClearGainHClick
          end
          object cbAntType: TComboBox
            Left = 96
            Top = 16
            Width = 112
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 0
            OnChange = cbAntTypeChange
            Items.Strings = (
              'A - '#1055#1088#1086#1089#1090#1072#1103' '#1074#1077#1088#1090#1080#1082#1072#1083#1100#1085#1072#1103
              'B - '#1044#1088#1091#1075#1072#1103)
          end
          object edtAgl: TNumericEdit
            Left = 96
            Top = 48
            Width = 70
            Height = 21
            TabOrder = 1
            Text = '0'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = '0'
            OnValueChange = edtAglValueChange
          end
          object edAzmMax: TDBEdit
            Left = 96
            Top = 112
            Width = 70
            Height = 21
            DataField = 'AZIMUTH'
            DataSource = dsLfMfOper
            TabOrder = 5
          end
          object edMaxGainH: TNumericEdit
            Left = 128
            Top = 80
            Width = 38
            Height = 21
            ParentColor = True
            ReadOnly = True
            TabOrder = 2
            Text = '0'
            Alignment = taRightJustify
            OldValue = '0'
          end
          object btGainH: TButton
            Left = 166
            Top = 80
            Width = 21
            Height = 21
            Caption = '...'
            TabOrder = 3
            OnClick = btGainHClick
          end
        end
        object pnOpTm: TPanel
          Left = 112
          Top = 82
          Width = 145
          Height = 65
          BevelOuter = bvNone
          TabOrder = 2
          object Label6: TLabel
            Left = 14
            Top = 12
            Width = 45
            Height = 13
            Caption = #1056#1086#1073#1086#1090#1072' '#1079
          end
          object Label7: TLabel
            Left = 9
            Top = 44
            Width = 51
            Height = 13
            Caption = #1056#1086#1073#1086#1090#1072' '#1076#1086
          end
          object edOpFrom: TDBEdit
            Left = 64
            Top = 8
            Width = 73
            Height = 21
            DataField = 'START_TIME'
            DataSource = dsLfMfOper
            TabOrder = 0
          end
          object edOpTo: TDBEdit
            Left = 64
            Top = 40
            Width = 73
            Height = 21
            DataField = 'STOP_TIME'
            DataSource = dsLfMfOper
            TabOrder = 1
          end
        end
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Height = 192
        inherited dbgOrganizations: TDBGrid
          Height = 175
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 192
      end
      inherited gbDoc: TGroupBox
        Height = 192
      end
      inherited gbOrganization: TGroupBox
        Height = 192
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 397
        inherited sb: TStatusBar
          Top = 378
        end
        inherited bmf: TBaseMapFrame
          Height = 349
        end
        inherited tb: TToolBar
          Images = cmf.bmf.iml
          inherited tb1: TToolButton
            Action = cmf.bmf.actNone
          end
          inherited tb2: TToolButton
            Action = cmf.bmf.actPan
          end
          inherited tb3: TToolButton
            Action = cmf.bmf.actZoomInTwice
          end
          inherited tb4: TToolButton
            Action = cmf.bmf.actZoomOutTwice
          end
          inherited tb5: TToolButton
            Action = cmf.bmf.actConf
          end
          inherited tb6: TToolButton
            Action = cmf.bmf.actArrows
          end
        end
      end
    end
  end
  inherited pnlForAllBottom: TPanel
    Top = 477
  end
  inherited pmIntoBeforeBase: TPopupMenu
    Left = 608
  end
  inherited tr: TIBTransaction
    Left = 488
    Top = 56
  end
  object dsLfMfOper: TDataSource
    DataSet = dstLfMfOper
    Left = 640
    Top = 176
  end
  object dstLfMfOper: TIBDataSet
    Database = dmMain.dbMain
    Transaction = tr
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'Select '
      '  STA_ID,'
      '  DAYNIGHT,'
      '  AZIMUTH,'
      '  ANGLE_ELEV,'
      '  SERV,'
      '  BDWDTH,'
      '  ADJ_RATIO,'
      '  PWR_KW,'
      '  AGL,'
      '  E_RMS,'
      '  E_MAX,'
      '  ANT_TYPE,'
      '  PTRN_TYPE,'
      '  Q_FACT,'
      '  START_TIME,'
      '  STOP_TIME,'
      '  ERP,'
      '  GAIN_AZM'
      'from LFMF_OPER '
      'where'
      '  STA_ID = :STA_ID and'
      '  DAYNIGHT = :DAYNIGHT')
    SelectSQL.Strings = (
      'select * from LFMF_OPER'
      'where STA_ID = :ID and DAYNIGHT = :DAYNIGHT')
    ModifySQL.Strings = (
      'update LFMF_OPER'
      'set'
      '  AZIMUTH = :AZIMUTH,'
      '  START_TIME = :START_TIME,'
      '  STOP_TIME = :STOP_TIME'
      'where'
      '  STA_ID = :OLD_STA_ID and'
      '  DAYNIGHT = :OLD_DAYNIGHT')
    Left = 672
    Top = 176
    object dstLfMfOperSTA_ID: TIntegerField
      FieldName = 'STA_ID'
      Origin = 'LFMF_OPER.STA_ID'
      Required = True
    end
    object dstLfMfOperDAYNIGHT: TIBStringField
      FieldName = 'DAYNIGHT'
      Origin = 'LFMF_OPER.DAYNIGHT'
      Required = True
      FixedChar = True
      Size = 2
    end
    object dstLfMfOperAZIMUTH: TFloatField
      FieldName = 'AZIMUTH'
      Origin = 'LFMF_OPER.AZIMUTH'
    end
    object dstLfMfOperSTART_TIME: TIBBCDField
      FieldName = 'START_TIME'
      Origin = 'LFMF_OPER.START_TIME'
      OnGetText = dstLfMfOpeTIMEGetText
      OnSetText = dstLfMfOperTIMESetText
      Precision = 4
      Size = 2
    end
    object dstLfMfOperSTOP_TIME: TIBBCDField
      FieldName = 'STOP_TIME'
      Origin = 'LFMF_OPER.STOP_TIME'
      OnGetText = dstLfMfOpeTIMEGetText
      OnSetText = dstLfMfOperTIMESetText
      Precision = 4
      Size = 2
    end
  end
end

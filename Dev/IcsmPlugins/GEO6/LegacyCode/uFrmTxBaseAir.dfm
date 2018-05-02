inherited frmTxBaseAir: TfrmTxBaseAir
  Left = 406
  Top = 104
  Caption = 'frmTxBaseAir'
  ClientHeight = 573
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited pcData: TPageControl
    Height = 481
    inherited tshCommon: TTabSheet
      object pnlTech: TPanel [0]
        Left = 0
        Top = 0
        Width = 772
        Height = 148
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 0
        object gbPower: TGroupBox
          Left = 0
          Top = 0
          Width = 236
          Height = 148
          Align = alLeft
          Caption = #1055#1086#1090#1091#1078#1085#1110#1089#1090#1100
          TabOrder = 0
          object lblEBPmax: TLabel
            Left = 20
            Top = 24
            Width = 81
            Height = 13
            Caption = 'EB'#1055' max, '#1076#1041#1082#1042#1090
          end
          object lblEBPG: TLabel
            Left = 20
            Top = 48
            Width = 72
            Height = 13
            Caption = 'EB'#1055' '#39#1043#39', '#1076#1041#1082#1042#1090
          end
          object lblEBPV: TLabel
            Left = 20
            Top = 72
            Width = 73
            Height = 13
            Caption = 'EB'#1055' '#39#1042#39', '#1076#1041#1082#1042#1090
          end
          object lblPower: TLabel
            Left = 20
            Top = 96
            Width = 57
            Height = 13
            Caption = #1056' '#1087#1088#1076#1095', '#1082#1042#1090
          end
          object edtEPRmaxAudio1: TNumericEdit
            Left = 116
            Top = 20
            Width = 69
            Height = 21
            TabOrder = 0
            Text = 'edtEPRmaxAudio1'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtEPRmaxAudio1'
            OnValueChange = edtEPRmaxAudio1ValueChange
          end
          object edtEPRGAudio1: TNumericEdit
            Left = 116
            Top = 44
            Width = 69
            Height = 21
            TabOrder = 1
            Text = 'edtEPRGAudio1'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtEPRGAudio1'
            OnValueChange = edtEPRGAudio1ValueChange
          end
          object edtEPRVAudio1: TNumericEdit
            Left = 116
            Top = 68
            Width = 69
            Height = 21
            TabOrder = 3
            Text = 'edtEPRVAudio1'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtEPRVAudio1'
            OnValueChange = edtEPRVAudio1ValueChange
          end
          object edtPowerAudio1: TNumericEdit
            Left = 116
            Top = 92
            Width = 69
            Height = 21
            TabOrder = 5
            Text = 'edtPowerAudio1'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtPowerAudio1'
            OnValueChange = edtPowerAudio1ValueChange
          end
          object btnEBPG1: TButton
            Left = 182
            Top = 44
            Width = 19
            Height = 20
            Caption = '...'
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clWindowText
            Font.Height = -11
            Font.Name = 'MS Sans Serif'
            Font.Style = [fsBold]
            ParentFont = False
            TabOrder = 2
            OnClick = btnEBPG1Click
          end
          object btnEBPV1: TButton
            Left = 182
            Top = 68
            Width = 19
            Height = 20
            Caption = '...'
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clWindowText
            Font.Height = -11
            Font.Name = 'MS Sans Serif'
            Font.Style = [fsBold]
            ParentFont = False
            TabOrder = 4
            OnClick = btnEBPV1Click
          end
          object CheckBoxERP: TCheckBox
            Left = 3
            Top = 22
            Width = 15
            Height = 17
            TabOrder = 6
            OnClick = CheckBoxERPClick
          end
          object CheckBoxPower: TCheckBox
            Left = 3
            Top = 94
            Width = 15
            Height = 17
            TabOrder = 7
            OnClick = CheckBoxPowerClick
          end
        end
        object gbAntenna: TGroupBox
          Left = 236
          Top = 0
          Width = 536
          Height = 148
          Align = alClient
          Caption = #1055#1077#1088#1077#1076#1072#1074#1072#1083#1100#1085#1072' '#1072#1085#1090#1077#1085#1072
          TabOrder = 1
          OnExit = gbAntennaExit
          object lblDirect: TLabel
            Left = 192
            Top = 20
            Width = 75
            Height = 13
            Caption = #1053#1072#1087#1088#1072#1074#1083#1077#1085#1110#1089#1090#1100
          end
          object lblPolarization: TLabel
            Left = 202
            Top = 42
            Width = 64
            Height = 13
            Caption = #1055#1086#1083#1103#1088#1080#1079#1072#1094#1110#1103
          end
          object lblHeight: TLabel
            Left = 24
            Top = 22
            Width = 86
            Height = 13
            Caption = #1042#1080#1089#1086#1090#1072' '#1087#1110#1076#1074#1110#1089#1091', '#1084
          end
          object lblAngle: TLabel
            Left = 428
            Top = 40
            Width = 10
            Height = 13
            Caption = #1042':'
          end
          object lblEmplif: TLabel
            Left = 36
            Top = 100
            Width = 77
            Height = 13
            Alignment = taRightJustify
            Caption = #1055#1110#1076#1089#1080#1083#1077#1085#1085#1103', '#1076#1041
            WordWrap = True
          end
          object lblFiderLoss: TLabel
            Left = 333
            Top = 76
            Width = 104
            Height = 13
            Alignment = taRightJustify
            Caption = #1042#1090#1088#1072#1090#1080' '#1092#1110#1076#1077#1088#1072', '#1076#1041'/'#1084
            WordWrap = True
          end
          object lblFiderLength: TLabel
            Left = 339
            Top = 98
            Width = 98
            Height = 13
            Alignment = taRightJustify
            Caption = #1044#1086#1074#1078#1080#1085#1072' '#1092#1110#1076#1077#1088#1072', '#1084
            WordWrap = True
          end
          object LblHeff: TLabel
            Left = 220
            Top = 72
            Width = 42
            Height = 13
            Caption = #1053' '#1077#1092'., '#1084
          end
          object lblAbgl2: TLabel
            Left = 368
            Top = 22
            Width = 57
            Height = 26
            Caption = #1050#1091#1090' '#1085#1072#1093#1080#1083#1091', '#1075#1088#1072#1076':'
            WordWrap = True
          end
          object lblHeffMax: TLabel
            Left = 52
            Top = 76
            Width = 59
            Height = 13
            Caption = 'H eff max, '#1084
          end
          object lblAzimuth: TLabel
            Left = 44
            Top = 38
            Width = 67
            Height = 26
            Caption = #1040#1079#1080#1084#1091#1090' '#1084#1072#1093' '#1074#1080#1087#1088#1086#1084', '#1075#1088#1072#1076
            WordWrap = True
          end
          object lblG: TLabel
            Left = 428
            Top = 20
            Width = 9
            Height = 13
            Caption = #1043':'
          end
          object lblAntDiscr: TLabel
            Left = 195
            Top = 99
            Width = 78
            Height = 27
            AutoSize = False
            Caption = #1055#1086#1089#1083#1072#1073#1083#1077#1085#1085#1103', '#1076#1041
            WordWrap = True
          end
          object lblSummAtten: TLabel
            Left = 14
            Top = 124
            Width = 110
            Height = 13
            Align = alCustom
            Caption = #1042#1090#1088#1072#1090#1080' '#1074' '#1089#1091#1084#1072#1090#1086#1088#1110', '#1076#1041
          end
          object edtHeight: TNumericEdit
            Left = 116
            Top = 18
            Width = 67
            Height = 21
            TabOrder = 0
            Text = 'edtHeight'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtHeight'
            OnValueChange = edtHeightValueChange
          end
          object edtFiderLoss: TNumericEdit
            Left = 440
            Top = 72
            Width = 77
            Height = 21
            TabOrder = 12
            Text = 'edtFiderLoss'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtFiderLoss'
            OnValueChange = edtFiderLossValueChange
          end
          object edtFiderLength: TNumericEdit
            Left = 440
            Top = 94
            Width = 77
            Height = 21
            TabOrder = 13
            Text = 'edtFiderLength'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtFiderLength'
            OnValueChange = edtFiderLengthValueChange
          end
          object cbxDirect: TComboBox
            Left = 268
            Top = 16
            Width = 57
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 5
            OnChange = cbxDirectChange
            Items.Strings = (
              'D'
              'ND')
          end
          object cbxPolarization: TComboBox
            Left = 268
            Top = 38
            Width = 57
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 6
            OnChange = cbxPolarizationChange
            Items.Strings = (
              'V'
              'H'
              'M')
          end
          object edtAngle: TNumericEdit
            Left = 440
            Top = 36
            Width = 77
            Height = 21
            TabOrder = 11
            Text = 'edtAngle'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtAngle'
            OnValueChange = edtAngleValueChange
          end
          object edtGain: TNumericEdit
            Left = 116
            Top = 96
            Width = 67
            Height = 21
            TabOrder = 3
            Text = 'edtGain'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtGain'
            OnValueChange = edtGainValueChange
          end
          object btnAntPattH: TButton
            Left = 268
            Top = 94
            Width = 19
            Height = 20
            Caption = 'H'
            TabOrder = 9
            OnClick = btnAntPattClick
          end
          object btnHeff: TButton
            Left = 268
            Top = 68
            Width = 19
            Height = 20
            Caption = '...'
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clWindowText
            Font.Height = -11
            Font.Name = 'MS Sans Serif'
            Font.Style = [fsBold]
            ParentFont = False
            TabOrder = 7
            OnClick = btnHeffClick
          end
          object edtAngle2: TNumericEdit
            Left = 440
            Top = 14
            Width = 77
            Height = 21
            TabOrder = 10
            Text = 'edtAngle'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtAngle'
            OnValueChange = edtAngle2ValueChange
          end
          object edtHeffMax: TNumericEdit
            Left = 116
            Top = 72
            Width = 67
            Height = 21
            TabOrder = 2
            Text = 'edtHeffMax'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtHeffMax'
            OnValueChange = edtHeffMaxValueChange
          end
          object edtAzimuth: TDBEdit
            Left = 116
            Top = 42
            Width = 67
            Height = 21
            DataField = 'AZIMUTHMAXRADIATION'
            DataSource = dsAir
            TabOrder = 1
          end
          object btnCalcHeff: TBitBtn
            Left = 288
            Top = 68
            Width = 19
            Height = 20
            Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
            ParentShowHint = False
            ShowHint = True
            TabOrder = 8
            OnClick = btnCalcHeffClick
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
          object edtSummAtten: TDBEdit
            Left = 132
            Top = 121
            Width = 51
            Height = 21
            DataField = 'SUMMATORATTENUATION'
            DataSource = dsAir
            TabOrder = 4
          end
          object btnAntPattV: TButton
            Left = 268
            Top = 118
            Width = 19
            Height = 20
            Caption = 'V'
            TabOrder = 14
            OnClick = btnAntPattClick
          end
        end
      end
      inherited pnlMaster: TPanel
        Top = 289
        TabOrder = 2
        DesignSize = (
          772
          166)
        inherited btnTRK: TButton
          OnEnter = btnTRKEnter
        end
        inherited btnOperator: TButton
          OnEnter = btnOperatorEnter
        end
        inherited cbProgramm: TDBComboBox
          OnEnter = cbProgrammEnter
        end
        inherited cbStateInCode: TDBLookupComboBox
          Left = 291
          Anchors = [akLeft]
        end
        inherited cbStateOutCode: TDBLookupComboBox
          Anchors = [akLeft]
        end
      end
      inherited gbxCoordination: TGroupBox
        Top = 250
        Caption = #1059#1079#1075#1086#1076#1078#1077#1085#1086' '#1090#1072' '#1082#1086#1086#1088#1076#1080#1085#1086#1074#1072#1085#1086
        object edtCoord: TDBEdit
          Left = 8
          Top = 13
          Width = 753
          Height = 21
          DataField = 'COORD'
          DataSource = dsStantionsBase
          TabOrder = 0
        end
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Height = 250
        inherited dbgOrganizations: TDBGrid
          Height = 233
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 250
      end
      inherited gbDoc: TGroupBox
        Height = 250
      end
      inherited gbOrganization: TGroupBox
        Height = 250
      end
    end
    inherited tshEquipment: TTabSheet
      inherited gbEquipment: TGroupBox
        Top = 101
        Height = 304
        inherited dbgEquipment: TDBGrid
          Height = 287
        end
      end
      object pnlSummator: TPanel [1]
        Left = 0
        Top = 40
        Width = 861
        Height = 61
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 1
        object gbSummator: TGroupBox
          Left = 0
          Top = 0
          Width = 861
          Height = 56
          Align = alTop
          TabOrder = 0
          Visible = False
          object lblRange: TLabel
            Left = 8
            Top = 23
            Width = 58
            Height = 13
            Caption = #1057#1084#1091#1075#1072', '#1052#1043#1094
          end
          object lblMinus1: TLabel
            Left = 120
            Top = 12
            Width = 8
            Height = 29
            Caption = '-'
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clWindowText
            Font.Height = -24
            Font.Name = 'MS Sans Serif'
            Font.Style = []
            ParentFont = False
          end
          object lblSummPow: TLabel
            Left = 192
            Top = 23
            Width = 81
            Height = 13
            Caption = #1055#1086#1090#1091#1078#1085#1110#1089#1090#1100', '#1082#1042#1090
          end
          object lblMinus2: TLabel
            Left = 332
            Top = 12
            Width = 8
            Height = 29
            Caption = '-'
            Font.Charset = DEFAULT_CHARSET
            Font.Color = clWindowText
            Font.Height = -24
            Font.Name = 'MS Sans Serif'
            Font.Style = []
            ParentFont = False
          end
          object lblMinFreq: TLabel
            Left = 414
            Top = 15
            Width = 104
            Height = 26
            Align = alCustom
            Caption = #1052#1110#1085#1110#1084#1072#1083#1100#1085#1072' '#1088#1110#1079#1085#1080#1094#1103'                '#1095#1072#1089#1090#1086#1090', '#1052#1043#1094
            WordWrap = True
          end
          object lblLoss: TLabel
            Left = 606
            Top = 23
            Width = 87
            Height = 13
            Align = alCustom
            Caption = #1055#1086#1089#1083#1072#1073#1083#1077#1085#1085#1103', '#1076#1041
            WordWrap = True
          end
          object edtSummFreqTo: TDBEdit
            Left = 132
            Top = 20
            Width = 49
            Height = 21
            DataField = 'SUMMATORFREQTO'
            DataSource = dsAir
            TabOrder = 0
          end
          object edtSummFreqFrom: TDBEdit
            Left = 68
            Top = 20
            Width = 49
            Height = 21
            DataField = 'SUMMATOFREQFROM'
            DataSource = dsAir
            TabOrder = 1
          end
          object edtSummPowerFrom: TDBEdit
            Left = 280
            Top = 20
            Width = 49
            Height = 21
            DataField = 'SUMMATORPOWERFROM'
            DataSource = dsAir
            TabOrder = 2
          end
          object edtSummPowerTo: TDBEdit
            Left = 344
            Top = 20
            Width = 49
            Height = 21
            DataField = 'SUMMATORPOWERTO'
            DataSource = dsAir
            TabOrder = 3
          end
          object edtSumMinFreq: TDBEdit
            Left = 540
            Top = 20
            Width = 49
            Height = 21
            DataField = 'SUMMATORMINFREQS'
            DataSource = dsAir
            TabOrder = 4
          end
          object edtSummLoss: TDBEdit
            Left = 700
            Top = 20
            Width = 49
            Height = 21
            DataField = 'SUMMATORATTENUATION'
            DataSource = dsAir
            TabOrder = 5
          end
        end
        object chbSummator: TCheckBox
          Left = 9
          Top = -1
          Width = 69
          Height = 17
          Caption = 'C'#1091#1084#1072#1090#1086#1088
          TabOrder = 1
          OnClick = chbSummatorClick
        end
      end
      object pnlFreqShift: TPanel [2]
        Left = 0
        Top = 0
        Width = 861
        Height = 40
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 2
        object lblSideLevel: TLabel
          Left = 71
          Top = 4
          Width = 102
          Height = 26
          Alignment = taRightJustify
          Caption = #1056#1110#1074#1077#1085#1100' '#1073#1086#1082#1086#1074#1086#1075#1086' '#1074#1080#1087#1088#1086#1110#1085#1102#1074#1072#1085#1085#1103', '#1084#1042#1090
          WordWrap = True
        end
        object lblFreqShift: TLabel
          Left = 393
          Top = 11
          Width = 116
          Height = 13
          Alignment = taRightJustify
          Caption = #1042#1110#1076#1093#1080#1083#1077#1085#1085#1103' '#1095#1072#1089#1090#1086#1090#1080', '#1043#1094
          WordWrap = True
        end
        object edtLevelSideRadiation: TDBEdit
          Left = 184
          Top = 7
          Width = 77
          Height = 21
          DataField = 'LEVELSIDERADIATION'
          DataSource = dsAir
          TabOrder = 0
        end
        object edtFreqShift: TDBEdit
          Left = 520
          Top = 7
          Width = 77
          Height = 21
          DataField = 'FREQSHIFT'
          DataSource = dsAir
          TabOrder = 1
        end
      end
      inherited pnlEquipButton: TPanel
        Top = 405
        TabOrder = 3
      end
    end
    inherited tshLicenses: TTabSheet
      DesignSize = (
        772
        455)
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Height = 447
        inherited dbgTestpoints: TDBGrid
          Height = 430
        end
      end
    end
    inherited tshChangeLog: TTabSheet
      inherited dbgGhangeLog: TDBGrid
        Height = 381
      end
      inherited panList: TPanel
        Height = 381
        inherited dgrList: TDBGrid
          Height = 356
        end
        inherited panSearch: TPanel
          Top = 356
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 455
        inherited sb: TStatusBar
          Top = 436
        end
        inherited bmf: TBaseMapFrame
          Height = 407
          Visible = True
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
    Top = 535
    Height = 38
  end
  inherited ibdsStantionsBase: TIBDataSet
    RefreshSQL.Strings = (
      'select   TRN.ID,'
      '            TRN.STAND_ID, '
      '            TRN.ADMINISTRATIONID,'
      '            TRN.DATECREATE,'
      '            TRN.DATECHANGE,'
      '            TRN.OWNER_ID, '
      '            TRN.RESPONSIBLEADMIN,'
      '            TRN.ACCOUNTCONDITION_IN,  '
      '            TRN.ACCOUNTCONDITION_OUT, '
      '            TRN.SYSTEMCAST_ID,'
      '            TRN.CLASSWAVE, '
      '            TRN.TIMETRANSMIT,'
      '            TRN.NAMEPROGRAMM,'
      '            TRN.USERID, '
      '            TRN.ORIGINALID,'
      '            TRN.NUMREGISTRY,'
      '            TRN.TYPEREGISTRY,'
      'TRN.VIDEO_EMISSION,'
      'TRN.SOUND_EMISSION_PRIMARY,'
      'TRN.SOUND_EMISSION_SECOND,'
      #9'TRN.REMARKS,'
      #9'AREA.NUMREGION AREA_NUMREGION,'
      #9'TRN.OPERATOR_ID,'
      #9'OWNER.NAMEORGANIZATION OPERATOR_NAME,'
      'TRN.COORD'
      'from  TRANSMITTERS TRN'
      'left outer join STAND on(TRN.STAND_ID = STAND.ID)'
      'left outer join AREA on(AREA.ID = STAND.AREA_ID)'
      'left outer join OWNER on(OWNER.ID = TRN.OPERATOR_ID)'
      'where TRN.ID = :ID')
    SelectSQL.Strings = (
      'select   '#9'TRN.ID,'
      #9'TRN.STAND_ID, '
      #9'TRN.ADMINISTRATIONID,'#9
      #9'TRN.DATECREATE,'
      #9'TRN.DATECHANGE,'
      #9'TRN.OWNER_ID, '
      #9'TRN.RESPONSIBLEADMIN,'
      #9'TRN.ACCOUNTCONDITION_IN,  '
      #9'TRN.ACCOUNTCONDITION_OUT, '
      #9'TRN.SYSTEMCAST_ID,'
      #9'TRN.CLASSWAVE, '
      #9'TRN.TIMETRANSMIT,'
      #9'TRN.NAMEPROGRAMM,'
      #9'TRN.USERID, '
      #9'TRN.ORIGINALID,'
      #9'TRN.NUMREGISTRY,'
      #9'TRN.TYPEREGISTRY,'#9
      #9'TRN.REMARKS,'
      #9'TRN.OPERATOR_ID,'
      #9'TRN.STATUS,'
      '                TRN.REMARKS_ADD,'
      'TRN.VIDEO_EMISSION,'
      'TRN.SOUND_EMISSION_PRIMARY,'
      'TRN.SOUND_EMISSION_SECOND,'
      'TRN.COORD'
      ''
      'from  TRANSMITTERS TRN'
      'where TRN.ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS'
      'set       '
      '            STAND_ID = '#9':STAND_ID, '
      '            DATECHANGE = '#9':DATECHANGE,'
      '            OWNER_ID    = '#9':OWNER_ID, '
      '            RESPONSIBLEADMIN          =  :RESPONSIBLEADMIN,'
      '            ACCOUNTCONDITION_IN =     :ACCOUNTCONDITION_IN,  '
      '            ACCOUNTCONDITION_OUT = :ACCOUNTCONDITION_OUT, '
      '            SYSTEMCAST_ID = '#9#9':SYSTEMCAST_ID,'
      '            CLASSWAVE = '#9':CLASSWAVE,'
      '            TIMETRANSMIT = '#9':TIMETRANSMIT,'
      '            NAMEPROGRAMM = :NAMEPROGRAMM,'
      '            USERID = '#9#9':USERID, '
      '            ORIGINALID = '#9':ORIGINALID, '
      '            NUMREGISTRY = '#9':NUMREGISTRY,'
      '            TYPEREGISTRY = '#9':TYPEREGISTRY,'
      '            REMARKS ='#9':REMARKS,'
      '            REMARKS_ADD = :REMARKS_ADD,'
      #9'OPERATOR_ID = :OPERATOR_ID,'
      'VIDEO_EMISSION = :VIDEO_EMISSION,'
      'SOUND_EMISSION_PRIMARY = :SOUND_EMISSION_PRIMARY,'
      'SOUND_EMISSION_SECOND = :SOUND_EMISSION_SECOND,'
      'COORD = :COORD'
      'where ID = :ID'
      '                ')
    object ibdsStantionsBaseVIDEO_EMISSION: TStringField
      FieldName = 'VIDEO_EMISSION'
      Origin = 'TRANSMITTERS.VIDEO_EMISSION'
    end
    object ibdsStantionsBaseSOUND_EMISSION_PRIMARY: TStringField
      FieldName = 'SOUND_EMISSION_PRIMARY'
    end
    object ibdsStantionsBaseSOUND_EMISSION_SECOND: TStringField
      FieldName = 'SOUND_EMISSION_SECOND'
    end
    object ibdsStantionsBaseCOORD: TIBStringField
      FieldName = 'COORD'
      Origin = 'TRANSMITTERS.COORD'
      Size = 64
    end
  end
  inherited ibdsLicenses: TIBDataSet
    Left = 680
    Top = 348
  end
  inherited dsLicenses: TDataSource
    Left = 724
    Top = 288
  end
  inherited ibqAccCondNameIn: TIBQuery
    Left = 128
    Top = 364
  end
  inherited ibqAccCondNameOut: TIBQuery
    Left = 160
    Top = 364
  end
  inherited dsStand: TDataSource
    Left = 8
    Top = 348
  end
  inherited ibqUserName: TIBQuery
    Left = 192
    Top = 348
  end
  inherited ibqTRKName: TIBQuery
    Left = 232
    Top = 360
  end
  inherited ibqSystemCastName: TIBQuery
    Left = 288
    Top = 364
  end
  inherited ibdsTelecomOrg: TIBDataSet
    Left = 12
    Top = 400
  end
  inherited dsTelecomOrg: TDataSource
    Left = 16
    Top = 428
  end
  inherited dsDocuments: TDataSource
    Left = 620
    Top = 356
  end
  inherited ibdsDocuments: TIBDataSet
    Left = 592
    Top = 354
  end
  inherited ibqDocType: TIBQuery
    Left = 428
    Top = 364
  end
  inherited dsAccCondNameOut: TDataSource
    Left = 160
    Top = 392
  end
  inherited dsAccCondNameIn: TDataSource
    Left = 128
    Top = 392
  end
  inherited ibdsEquipment: TIBDataSet
    Left = 480
    Top = 360
  end
  inherited dsEquipment: TDataSource
    Left = 512
    Top = 360
  end
  inherited dsTestpoints: TDataSource
    Left = 92
  end
  inherited ibdsTestpoint: TIBDataSet
    Left = 96
    Top = 400
  end
  inherited ActionList1: TActionList
    Left = 760
    Top = 8
  end
  inherited pmIntoBeforeBase: TPopupMenu
    Left = 640
    Top = 208
  end
  inherited ImageList1: TImageList
    Left = 764
    Top = 132
  end
  object ibqTypeSystemName: TIBQuery [30]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    Left = 324
    Top = 364
  end
  object ibdsAir: TIBDataSet [31]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsAirAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select '#9'TR.ID,'
      #9'TR.LICENSE_RFR_ID,'
      #9'L.NUMLICENSE, '
      #9'TR.NUMPERMBUILD, '
      #9'TR.DATEPERMBUILDFROM, '
      #9'TR.DATEPERMBUILDTO,'
      #9'SUMMATORPOWERS, '
      #9'SUMMATOFREQFROM, '
      #9'SUMMATORATTENUATION, '
      #9'SUMMATORFREQTO, '
      #9'SUMMATORMINFREQS, '
      #9'SUMMATORPOWERFROM, '
      #9'SUMMATORPOWERTO,'
      #9'AZIMUTHMAXRADIATION,'
      #9'LEVELSIDERADIATION, '
      #9'FREQSHIFT'
      'from TRANSMITTERS TR'
      'left outer join LICENSE L on (L.ID = TR.LICENSE_RFR_ID)'
      'where TR.ID = :ID')
    SelectSQL.Strings = (
      'select '#9'TR.ID,'
      #9'TR.LICENSE_RFR_ID,'
      #9'L.NUMLICENSE, '
      #9'TR.NUMPERMBUILD, '
      #9'TR.DATEPERMBUILDFROM, '
      #9'TR.DATEPERMBUILDTO,'
      #9'SUMMATORPOWERS, '
      #9'SUMMATOFREQFROM, '
      #9'SUMMATORATTENUATION, '
      #9'SUMMATORFREQTO, '
      #9'SUMMATORMINFREQS, '
      #9'SUMMATORPOWERFROM, '
      #9'SUMMATORPOWERTO,'
      #9'AZIMUTHMAXRADIATION,'
      #9'LEVELSIDERADIATION, '
      #9'FREQSHIFT'
      'from TRANSMITTERS TR'
      'left outer join LICENSE L on (L.ID = TR.LICENSE_RFR_ID)'
      'where TR.ID = :ID')
    ModifySQL.Strings = (
      'update '#9'TRANSMITTERS set'
      ' '#9'LICENSE_RFR_ID = :LICENSE_RFR_ID,'
      #9'NUMPERMBUILD = :NUMPERMBUILD, '
      #9'DATEPERMBUILDFROM = :DATEPERMBUILDFROM, '
      #9'DATEPERMBUILDTO = :DATEPERMBUILDTO,'
      #9'SUMMATORPOWERS = :SUMMATORPOWERS, '
      #9'SUMMATOFREQFROM = :SUMMATOFREQFROM, '
      #9'SUMMATORATTENUATION = :SUMMATORATTENUATION, '
      #9'SUMMATORFREQTO = :SUMMATORFREQTO, '
      #9'SUMMATORMINFREQS = :SUMMATORMINFREQS, '
      #9'SUMMATORPOWERFROM = :SUMMATORPOWERFROM, '
      #9'SUMMATORPOWERTO = :SUMMATORPOWERTO,'
      #9'AZIMUTHMAXRADIATION = :AZIMUTHMAXRADIATION,'
      #9'LEVELSIDERADIATION = :LEVELSIDERADIATION, '
      #9'FREQSHIFT = :FREQSHIFT'
      'where ID = :ID'
      '')
    Left = 680
    Top = 376
    object ibdsAirLICENSE_RFR_ID: TIntegerField
      FieldName = 'LICENSE_RFR_ID'
      Origin = 'TRANSMITTERS.LICENSE_RFR_ID'
    end
    object ibdsAirNUMLICENSE: TIBStringField
      FieldName = 'NUMLICENSE'
      Origin = 'LICENSE.NUMLICENSE'
      Size = 64
    end
    object ibdsAirNUMPERMBUILD: TIBStringField
      FieldName = 'NUMPERMBUILD'
      Origin = 'TRANSMITTERS.NUMPERMBUILD'
      Size = 64
    end
    object ibdsAirDATEPERMBUILDFROM: TDateField
      FieldName = 'DATEPERMBUILDFROM'
      Origin = 'TRANSMITTERS.DATEPERMBUILDFROM'
    end
    object ibdsAirDATEPERMBUILDTO: TDateField
      FieldName = 'DATEPERMBUILDTO'
      Origin = 'TRANSMITTERS.DATEPERMBUILDTO'
    end
    object ibdsAirID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object ibdsAirSUMMATORPOWERS: TSmallintField
      FieldName = 'SUMMATORPOWERS'
      Origin = 'TRANSMITTERS.SUMMATORPOWERS'
    end
    object ibdsAirSUMMATOFREQFROM: TFloatField
      FieldName = 'SUMMATOFREQFROM'
      Origin = 'TRANSMITTERS.SUMMATOFREQFROM'
    end
    object ibdsAirSUMMATORATTENUATION: TFloatField
      FieldName = 'SUMMATORATTENUATION'
      Origin = 'TRANSMITTERS.SUMMATORATTENUATION'
      OnChange = ibdsAirSUMMATORATTENUATIONChange
    end
    object ibdsAirSUMMATORFREQTO: TFloatField
      FieldName = 'SUMMATORFREQTO'
      Origin = 'TRANSMITTERS.SUMMATORFREQTO'
    end
    object ibdsAirSUMMATORMINFREQS: TFloatField
      FieldName = 'SUMMATORMINFREQS'
      Origin = 'TRANSMITTERS.SUMMATORMINFREQS'
    end
    object ibdsAirSUMMATORPOWERFROM: TFloatField
      FieldName = 'SUMMATORPOWERFROM'
      Origin = 'TRANSMITTERS.SUMMATORPOWERFROM'
    end
    object ibdsAirSUMMATORPOWERTO: TFloatField
      FieldName = 'SUMMATORPOWERTO'
      Origin = 'TRANSMITTERS.SUMMATORPOWERTO'
    end
    object ibdsAirAZIMUTHMAXRADIATION: TFloatField
      FieldName = 'AZIMUTHMAXRADIATION'
      Origin = 'TRANSMITTERS.AZIMUTHMAXRADIATION'
    end
    object ibdsAirLEVELSIDERADIATION: TIntegerField
      FieldName = 'LEVELSIDERADIATION'
      Origin = 'TRANSMITTERS.LEVELSIDERADIATION'
    end
    object ibdsAirFREQSHIFT: TIntegerField
      FieldName = 'FREQSHIFT'
      Origin = 'TRANSMITTERS.FREQSHIFT'
    end
  end
  object dsAir: TDataSource [32]
    DataSet = ibdsAir
    Left = 712
    Top = 376
  end
  inherited ibqStand: TIBQuery
    Left = 48
    Top = 424
  end
end

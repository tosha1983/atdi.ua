inherited frmTxDVB: TfrmTxDVB
  Left = 326
  Top = 111
  BorderStyle = bsToolWindow
  Caption = 'frmTxDVB'
  ClientHeight = 614
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited lblTxType: TLabel
      Width = 41
      Caption = #1062#1058#1041
    end
  end
  inherited pcData: TPageControl
    Height = 522
    inherited tshCommon: TTabSheet
      inherited pnlTech: TPanel
        Top = 81
        TabOrder = 1
      end
      object pnlForDVB: TPanel [1]
        Left = 0
        Top = 0
        Width = 772
        Height = 81
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 0
        object lblChannel: TLabel
          Left = 84
          Top = 16
          Width = 30
          Height = 13
          Caption = #1056#1072#1089#1090#1088
        end
        object lblFreqCentre: TLabel
          Left = 84
          Top = 41
          Width = 66
          Height = 26
          Caption = #1062#1077#1085#1090#1088#1072#1083#1100#1085#1072' '#1095#1072#1089#1090#1086#1090#1072', '#1052#1043#1094
          WordWrap = True
        end
        object lblClassRadiation: TLabel
          Left = 594
          Top = 28
          Width = 118
          Height = 13
          Caption = #1050#1083#1072#1089#1089' '#1074#1080#1087#1088#1086#1084#1110#1085#1102#1074#1072#1085#1085#1103
          WordWrap = True
        end
        object pnDvbt: TPanel
          Left = 216
          Top = 0
          Width = 329
          Height = 89
          Caption = 'pnDvbt'
          TabOrder = 6
          object lblSystemCast: TLabel
            Left = 12
            Top = 9
            Width = 50
            Height = 26
            Alignment = taRightJustify
            Caption = #1057#1080#1089#1090#1077#1084#1072' '#1084#1086#1074#1083#1077#1085#1085#1103
            WordWrap = True
          end
          object lbGiFftSize: TLabel
            Left = 14
            Top = 40
            Width = 48
            Height = 26
            Alignment = taRightJustify
            Caption = #1047#1072#1093#1080#1089#1085#1080#1081' '#1110#1085#1090#1077#1088#1074#1072#1083
            WordWrap = True
          end
          object cbxTypeSysName: TComboBox
            Left = 68
            Top = 12
            Width = 52
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 0
            OnChange = cbxTypeSysNameChange
            OnDropDown = cbxTypeSysNameDropDown
          end
          object cbGiFftSize: TComboBox
            Left = 68
            Top = 44
            Width = 52
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 1
            OnChange = cbGiFftSizeChange
            OnDropDown = cbxTypeSysNameDropDown
          end
          object edDvbtSystemInfo: TEdit
            Left = 128
            Top = 12
            Width = 161
            Height = 21
            ParentColor = True
            ReadOnly = True
            TabOrder = 2
            Text = 'edDvbtSystemInfo'
          end
          object edDvbtGiInfo: TEdit
            Left = 128
            Top = 44
            Width = 161
            Height = 21
            ParentColor = True
            ReadOnly = True
            TabOrder = 3
            Text = 'edDvbtSystemInfo'
          end
        end
        object edtChannel: TNumericEdit
          Left = 116
          Top = 12
          Width = 53
          Height = 21
          ReadOnly = True
          TabOrder = 2
          Text = '0'
          Alignment = taRightJustify
          OldValue = '0'
        end
        object edtFreqCentre: TNumericEdit
          Left = 152
          Top = 44
          Width = 57
          Height = 21
          ReadOnly = True
          TabOrder = 5
          Text = 'edtFreqCentre'
          Alignment = taRightJustify
          OldValue = 'edtFreqCentre'
        end
        object btnChannel: TButton
          Left = 168
          Top = 13
          Width = 19
          Height = 20
          Caption = '...'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -11
          Font.Name = 'MS Sans Serif'
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 3
          OnClick = btnChannelClick
        end
        object btnNullChannel: TButton
          Left = 187
          Top = 13
          Width = 20
          Height = 20
          Caption = 'X'
          TabOrder = 4
          OnClick = btnNullChannelClick
        end
        object edtClassRadiationVideo: TEdit
          Left = 592
          Top = 56
          Width = 121
          Height = 21
          TabOrder = 12
          Text = 'edtClassRadiationVideo'
          Visible = False
        end
        object edtVideoEmission: TDBEdit
          Left = 598
          Top = 44
          Width = 91
          Height = 21
          DataField = 'VIDEO_EMISSION'
          DataSource = dsStantionsBase
          TabOrder = 10
        end
        object btnVideoEmission: TBitBtn
          Left = 690
          Top = 44
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 11
          OnClick = btnVideoEmissionClick
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
        object rbDvbt: TRadioButton
          Left = 8
          Top = 16
          Width = 65
          Height = 17
          Caption = 'DVB-T'
          TabOrder = 0
          TabStop = True
          OnClick = rbDvbtClick
        end
        object rbDvbt2: TRadioButton
          Left = 8
          Top = 40
          Width = 65
          Height = 17
          Caption = 'DVB-T2'
          TabOrder = 1
          TabStop = True
          OnClick = rbDvbt2Click
        end
        object chRotatedCnstls: TCheckBox
          Left = 600
          Top = 11
          Width = 49
          Height = 17
          Caption = 'RC'
          TabOrder = 8
          OnClick = chRotatedCnstlsClick
        end
        object chModeOfExtnts: TCheckBox
          Left = 656
          Top = 11
          Width = 49
          Height = 17
          Caption = 'ME'
          TabOrder = 9
          OnClick = chModeOfExtntsClick
        end
        object pnDvbt2: TPanel
          Left = 224
          Top = 24
          Width = 369
          Height = 81
          Caption = 'pnDvbt2'
          TabOrder = 7
          object lbCodeRate: TLabel
            Left = 12
            Top = 41
            Width = 53
            Height = 26
            Caption = #1064#1074#1080#1076#1082#1110#1089#1090#1100' '#1082#1086#1076#1091#1074#1072#1085#1085#1103
            WordWrap = True
          end
          object lblModulation: TLabel
            Left = 12
            Top = 16
            Width = 52
            Height = 13
            Caption = #1052#1086#1076#1091#1083#1103#1094#1110#1103
          end
          object lbDiversity: TLabel
            Left = 265
            Top = 41
            Width = 37
            Height = 26
            Alignment = taRightJustify
            Caption = #1056#1086#1079#1085#1086#1089' '#1072#1085#1090#1077#1085
            WordWrap = True
          end
          object lbPilotPattern: TLabel
            Left = 267
            Top = 9
            Width = 35
            Height = 26
            Alignment = taRightJustify
            Caption = #1055#1110#1083#1086#1090' '#1089#1080#1075#1085#1072#1083
            WordWrap = True
          end
          object lbGuardInterval: TLabel
            Left = 142
            Top = 40
            Width = 48
            Height = 26
            Alignment = taRightJustify
            Caption = #1047#1072#1093#1080#1089#1085#1080#1081' '#1110#1085#1090#1077#1088#1074#1072#1083
            WordWrap = True
          end
          object lbFftSize: TLabel
            Left = 144
            Top = 9
            Width = 46
            Height = 26
            Alignment = taRightJustify
            Caption = #1050#1110#1083#1100#1082#1110#1089#1090#1100' '#1085#1077#1089#1091#1095#1080#1093
            WordWrap = True
          end
          object cbModulation: TComboBox
            Left = 68
            Top = 12
            Width = 69
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 0
            OnChange = cbModulationChange
          end
          object cbCodeRate: TComboBox
            Left = 68
            Top = 44
            Width = 69
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 1
            OnChange = cbCodeRateChange
          end
          object cbPilotPattern: TComboBox
            Left = 308
            Top = 12
            Width = 56
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 4
            OnChange = cbPilotPatternChange
            Items.Strings = (
              ''
              'PP1'
              'PP2'
              'PP3'
              'PP4'
              'PP5'
              'PP6'
              'PP7'
              'PP8')
          end
          object cbDiversity: TComboBox
            Left = 308
            Top = 44
            Width = 56
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 5
            OnChange = cbDiversityChange
            Items.Strings = (
              ''
              'SISO'
              'MISO'
              'SIMO'
              'MIMO')
          end
          object cbGuardInterval: TComboBox
            Left = 196
            Top = 44
            Width = 60
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 3
            OnChange = cbGuardIntervalChange
            OnDropDown = cbxTypeSysNameDropDown
          end
          object cbFftSize: TComboBox
            Left = 196
            Top = 12
            Width = 60
            Height = 21
            Style = csDropDownList
            ItemHeight = 13
            TabOrder = 2
            OnChange = cbFftSizeChange
            OnDropDown = cbxTypeSysNameDropDown
          end
        end
      end
      inherited pnlForDigital: TPanel
        Top = 229
        Height = 62
        TabOrder = 2
      end
      inherited pnlMaster: TPanel
        Top = 330
        TabOrder = 4
        inherited cbStateIn: TDBLookupComboBox
          Left = 348
          Width = 133
        end
        inherited cbStateInCode: TDBLookupComboBox
          Left = 294
        end
        inherited cbStateOutCode: TDBLookupComboBox
          Left = 35
        end
      end
      inherited gbxCoordination: TGroupBox
        Top = 291
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Left = 121
        Height = 328
        inherited dbgOrganizations: TDBGrid
          Height = 311
          Columns = <
            item
              Expanded = False
              FieldName = 'CODE'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'NAME'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'AC_NAME'
              Visible = True
            end>
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 328
        Height = 168
        inherited dbgDocuments: TDBGrid
          Height = 151
        end
      end
      inherited gbDoc: TGroupBox
        Left = 589
        Width = 183
        Height = 328
      end
      inherited gbOrganization: TGroupBox
        Width = 121
        Height = 328
        inherited btnNewTelecomOrg: TButton
          Left = 12
        end
        inherited btnDelOrg: TButton
          Left = 12
        end
      end
    end
    inherited tshEquipment: TTabSheet
      inherited gbEquipment: TGroupBox
        Width = 802
        Height = 321
        inherited dbgEquipment: TDBGrid
          Width = 798
          Height = 304
        end
      end
      inherited pnlSummator: TPanel
        Width = 802
        inherited gbSummator: TGroupBox
          Width = 802
        end
      end
      inherited pnlFreqShift: TPanel
        Width = 802
      end
      inherited pnlEquipButton: TPanel
        Top = 422
        Width = 802
      end
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Width = 764
        Height = 330
        inherited dbgTestpoints: TDBGrid
          Width = 760
          Height = 313
        end
      end
    end
    inherited tshChangeLog: TTabSheet
      inherited dbgGhangeLog: TDBGrid
        Width = 764
        Height = 330
      end
      inherited panList: TPanel
        Width = 764
        Height = 330
        inherited dgrList: TDBGrid
          Width = 764
          Height = 305
        end
        inherited panSearch: TPanel
          Top = 305
          Width = 764
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 496
        inherited sb: TStatusBar
          Top = 477
        end
        inherited bmf: TBaseMapFrame
          Height = 448
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
    Top = 576
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
      'TRN.REMARKS_ADD,'
      #9'AREA.NUMREGION AREA_NUMREGION,'
      #9'TRN.OPERATOR_ID,'
      #9'OWNER.NAMEORGANIZATION OPERATOR_NAME,'
      'TRN.COORD'
      'from  TRANSMITTERS TRN'
      'left outer join STAND on(TRN.STAND_ID = STAND.ID)'
      'left outer join AREA on(AREA.ID = STAND.AREA_ID)'
      'left outer join OWNER on(OWNER.ID = TRN.OPERATOR_ID)'
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
      '            CLASSWAVE = '#9':CLASSWAVE,'
      '            TIMETRANSMIT = '#9':TIMETRANSMIT,'
      '            NAMEPROGRAMM = :NAMEPROGRAMM,'
      '            USERID = '#9#9':USERID, '
      '            ORIGINALID = '#9':ORIGINALID, '
      '            NUMREGISTRY = '#9':NUMREGISTRY,'
      '            TYPEREGISTRY = '#9':TYPEREGISTRY,'
      '            REMARKS ='#9':REMARKS,'
      'REMARKS_ADD = :REMARKS_ADD,'
      #9'OPERATOR_ID = :OPERATOR_ID,'
      'VIDEO_EMISSION = :VIDEO_EMISSION,'
      'SOUND_EMISSION_PRIMARY = :SOUND_EMISSION_PRIMARY,'
      'SOUND_EMISSION_SECOND = :SOUND_EMISSION_SECOND,'
      'COORD = :COORD'
      'where ID = :ID')
    Left = 360
    Top = 32
  end
  inherited dsStantionsBase: TDataSource
    Left = 398
    Top = 32
  end
  inherited ibdsLicenses: TIBDataSet
    Left = 824
  end
  inherited dsLicenses: TDataSource
    Left = 824
  end
  inherited ibqAccCondNameIn: TIBQuery
    Left = 112
    Top = 424
  end
  inherited ibqAccCondNameOut: TIBQuery
    Left = 144
    Top = 424
  end
  inherited dsStand: TDataSource
    Left = 360
    Top = 424
  end
  inherited ibqUserName: TIBQuery
    Left = 184
    Top = 424
  end
  inherited ibqTRKName: TIBQuery
    Left = 224
    Top = 424
  end
  inherited ibqSystemCastName: TIBQuery
    Left = 288
    Top = 424
  end
  inherited ibqDocType: TIBQuery
    Left = 396
    Top = 420
  end
  inherited ActionList1: TActionList
    Left = 736
  end
  inherited ImageList1: TImageList
    Left = 732
  end
  inherited ibqTypeSystemName: TIBQuery
    Left = 324
    Top = 424
  end
  inherited sqlNewAdminId: TIBSQL
    Left = 704
    Top = 32
  end
  inherited ibqStand: TIBQuery
    Left = 8
  end
  inherited imlMap: TImageList
    Left = 740
    Top = 59
  end
  inherited tr: TIBTransaction
    Left = 328
    Top = 32
  end
  object ibdsDVB: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsDVBAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      ' select'
      '    TR.ID,'
      '    TR.TYPESYSTEM,'
      '    TR.GUARDINTERVAL_ID,'
      '    GI.NAMECURRIERGUARD,'
      '    TR.CHANNEL_ID,'
      '    CH.NAMECHANNEL'
      'from TRANSMITTERS TR'
      
        'left outer join CARRIERGUARDINTERVAL GI on (TR.GUARDINTERVAL_ID ' +
        '= GI.ID)'
      'left outer join CHANNELS CH on (TR.CHANNEL_ID = CH.ID)'
      'where TR.ID = :ID')
    SelectSQL.Strings = (
      ' select'
      '    TR.ID,'
      '    TR.TYPESYSTEM,'
      '    TR.GUARDINTERVAL_ID,'
      '    GI.NAMECURRIERGUARD,'
      '    TR.CHANNEL_ID,'
      '    CH.NAMECHANNEL'
      'from TRANSMITTERS TR'
      
        'left outer join CARRIERGUARDINTERVAL GI on (TR.GUARDINTERVAL_ID ' +
        '= GI.ID)'
      'left outer join CHANNELS CH on (TR.CHANNEL_ID = CH.ID)'
      'where TR.ID = :ID')
    ModifySQL.Strings = (
      'update  TRANSMITTERS '
      'set '
      '    GUARDINTERVAL_ID = :GUARDINTERVAL_ID'
      'where ID = :ID')
    Left = 736
    Top = 156
    object ibdsDVBID: TIntegerField
      FieldName = 'ID'
      Origin = 'DIGITALTELESYSTEM.ID'
    end
    object ibdsDVBGUARDINTERVAL_ID: TSmallintField
      FieldName = 'GUARDINTERVAL_ID'
      Origin = 'TRANSMITTERS.GUARDINTERVAL_ID'
    end
    object ibdsDVBNAMECHANNEL: TIBStringField
      FieldName = 'NAMECHANNEL'
      Origin = 'CHANNELS.NAMECHANNEL'
      Size = 4
    end
    object ibdsDVBCHANNEL_ID: TIntegerField
      FieldName = 'CHANNEL_ID'
      Origin = 'TRANSMITTERS.CHANNEL_ID'
    end
    object ibdsDVBNAMECURRIERGUARD: TIBStringField
      FieldName = 'NAMECURRIERGUARD'
      Origin = 'CARRIERGUARDINTERVAL.NAMECURRIERGUARD'
      Size = 4
    end
    object ibdsDVBTYPESYSTEM: TSmallintField
      FieldName = 'TYPESYSTEM'
      Origin = 'TRANSMITTERS.TYPESYSTEM'
    end
  end
  object dsDVB: TDataSource
    DataSet = ibdsDVB
    Left = 736
    Top = 184
  end
  object ibqDigTSys: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      
        'select ID, MODULATION, CODERATE, NAMESYSTEM, ENUMVAL from DIGITA' +
        'LTELESYSTEM')
    Left = 640
    Top = 232
    object ibqDigTSysID: TIntegerField
      FieldName = 'ID'
      Origin = 'DIGITALTELESYSTEM.ID'
      Required = True
    end
    object ibqDigTSysMODULATION: TIBStringField
      FieldName = 'MODULATION'
      Origin = 'DIGITALTELESYSTEM.MODULATION'
      Size = 8
    end
    object ibqDigTSysCODERATE: TIBStringField
      FieldName = 'CODERATE'
      Origin = 'DIGITALTELESYSTEM.CODERATE'
      Size = 4
    end
    object ibqDigTSysNAMESYSTEM: TIBStringField
      FieldName = 'NAMESYSTEM'
      Origin = 'DIGITALTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object ibqDigTSysENUMVAL: TIntegerField
      FieldName = 'ENUMVAL'
      Origin = 'DIGITALTELESYSTEM.ENUMVAL'
      Required = True
    end
  end
  object dsDigTSys: TDataSource
    DataSet = ibqDigTSys
    Left = 672
    Top = 232
  end
end

inherited frmTxDAB: TfrmTxDAB
  Left = 310
  Top = 225
  BorderStyle = bsToolWindow
  Caption = 'frmTxDAB'
  ClientHeight = 576
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited lblTxType: TLabel
      Width = 44
      Caption = #1062#1056#1052
    end
  end
  inherited pcData: TPageControl
    Height = 484
    inherited tshCommon: TTabSheet
      inherited pnlTech: TPanel
        Top = 41
        TabOrder = 1
      end
      inherited pnlForDigital: TPanel
        Top = 189
        TabOrder = 2
      end
      object pnlForDAB: TPanel [2]
        Left = 0
        Top = 0
        Width = 772
        Height = 41
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 0
        object lblFreqCentre: TLabel
          Left = 234
          Top = 9
          Width = 70
          Height = 26
          Caption = #1062#1077#1085#1090#1088#1072#1083#1100#1085#1072#1103' '#1095#1072#1089#1090#1086#1090#1072', '#1052#1043#1094
          WordWrap = True
        end
        object lblRangeFreq: TLabel
          Left = 408
          Top = 15
          Width = 94
          Height = 13
          Caption = #1057#1084#1091#1075#1072' '#1095#1072#1089#1090#1086#1090', '#1052#1043#1094
        end
        object Label2: TLabel
          Left = 574
          Top = 12
          Width = 5
          Height = 20
          Caption = '-'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -16
          Font.Name = 'MS Sans Serif'
          Font.Style = []
          ParentFont = False
        end
        object Label3: TLabel
          Left = 10
          Top = 14
          Width = 50
          Height = 13
          Caption = #1041#1083#1086#1082' DAB'
        end
        object lblClassRadiation: TLabel
          Left = 658
          Top = 0
          Width = 72
          Height = 13
          Caption = #1050#1083#1072#1089#1089' '#1074#1080#1087#1088#1086#1084
          WordWrap = True
        end
        object edtFreqCentre: TNumericEdit
          Left = 312
          Top = 12
          Width = 81
          Height = 21
          ReadOnly = True
          TabOrder = 2
          Text = 'edtFreqCentre'
          Alignment = taRightJustify
          OldValue = 'edtFreqCentre'
        end
        object edtFreqFrom: TDBEdit
          Left = 508
          Top = 12
          Width = 60
          Height = 21
          DataField = 'BD_FREQFROM'
          DataSource = dsDAB
          ReadOnly = True
          TabOrder = 3
        end
        object btnBlock: TButton
          Left = 198
          Top = 11
          Width = 19
          Height = 20
          Caption = '...'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -11
          Font.Name = 'MS Sans Serif'
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 1
          OnClick = btnBlockClick
        end
        object edtFreqTo: TDBEdit
          Left = 584
          Top = 12
          Width = 60
          Height = 21
          DataField = 'BD_FREQTO'
          DataSource = dsDAB
          ReadOnly = True
          TabOrder = 4
        end
        object edtBlockDAB: TDBEdit
          Left = 62
          Top = 11
          Width = 137
          Height = 21
          DataField = 'BLOCK_NAME'
          DataSource = dsDAB
          ReadOnly = True
          TabOrder = 0
        end
        object edtClassRadiationVideo: TEdit
          Left = 664
          Top = 22
          Width = 89
          Height = 21
          ReadOnly = True
          TabOrder = 5
          Text = 'edtClassRadiationVideo'
          Visible = False
        end
        object edtSoundEmissionPrimary: TDBEdit
          Left = 654
          Top = 12
          Width = 82
          Height = 21
          DataField = 'SOUND_EMISSION_PRIMARY'
          DataSource = dsStantionsBase
          TabOrder = 6
        end
        object btnSoundEmissionPrimary: TBitBtn
          Left = 738
          Top = 13
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          OnClick = btnSoundEmissionPrimaryClick
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
      end
      inherited pnlMaster: TPanel
        Top = 292
        TabOrder = 4
        inherited cbStateIn: TDBLookupComboBox
          Left = 348
          Width = 133
        end
        inherited cbStateInCode: TDBLookupComboBox
          Left = 293
        end
      end
      inherited gbxCoordination: TGroupBox
        Top = 253
        inherited edtCoord: TDBEdit
          Width = 751
        end
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Left = 121
        Height = 298
        inherited dbgOrganizations: TDBGrid
          Height = 281
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 298
        Width = 768
        Height = 160
        inherited dbgDocuments: TDBGrid
          Width = 764
          Height = 143
        end
      end
      inherited gbDoc: TGroupBox
        Left = 589
        Width = 179
        Height = 298
      end
      inherited gbOrganization: TGroupBox
        Width = 121
        Height = 298
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
        Width = 761
        Height = 280
        inherited dbgEquipment: TDBGrid
          Width = 757
          Height = 263
        end
      end
      inherited pnlSummator: TPanel
        Width = 761
        inherited gbSummator: TGroupBox
          Width = 761
        end
      end
      inherited pnlFreqShift: TPanel
        Width = 761
      end
      inherited pnlEquipButton: TPanel
        Top = 381
        Width = 761
      end
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Width = 764
        inherited dbgTestpoints: TDBGrid
          Width = 760
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 458
        inherited sb: TStatusBar
          Top = 439
        end
        inherited bmf: TBaseMapFrame
          Height = 410
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
    Top = 538
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
    Top = 32
  end
  inherited dsStantionsBase: TDataSource
    Top = 32
  end
  inherited ibdsLicenses: TIBDataSet
    Left = 740
    Top = 128
  end
  inherited dsLicenses: TDataSource
    Left = 740
    Top = 160
  end
  object ibdsDAB: TIBDataSet [19]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsDABAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select TR.ID, '
      '    ALLOTMENTBLOCKDAB_ID,  '
      '    BD.FREQFROM   BD_FREQFROM,'
      '    BD.FREQTO        BD_FREQTO,'
      '    BD.NAME             BLOCK_NAME'
      'from TRANSMITTERS TR'
      'left outer join BLOCKDAB BD on (BD.ID = TR.ALLOTMENTBLOCKDAB_ID)'
      'where TR.ID = :ID')
    SelectSQL.Strings = (
      'select TR.ID, '
      '    ALLOTMENTBLOCKDAB_ID,  '
      '    BD.FREQFROM   BD_FREQFROM,'
      '    BD.FREQTO        BD_FREQTO,'
      '    BD.NAME             BLOCK_NAME'
      'from TRANSMITTERS TR'
      'left outer join BLOCKDAB BD on (BD.ID = TR.ALLOTMENTBLOCKDAB_ID)'
      'where TR.ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS set '
      #9'ALLOTMENTBLOCKDAB_ID = :ALLOTMENTBLOCKDAB_ID '
      'where  ID = :ID')
    Left = 684
    Top = 124
    object ibdsDABID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object ibdsDABALLOTMENTBLOCKDAB_ID: TIntegerField
      FieldName = 'ALLOTMENTBLOCKDAB_ID'
      Origin = 'TRANSMITTERS.ALLOTMENTBLOCKDAB_ID'
    end
    object ibdsDABBD_FREQFROM: TFloatField
      FieldName = 'BD_FREQFROM'
      Origin = 'BLOCKDAB.FREQFROM'
      DisplayFormat = '0.###'
    end
    object ibdsDABBD_FREQTO: TFloatField
      FieldName = 'BD_FREQTO'
      Origin = 'BLOCKDAB.FREQTO'
      DisplayFormat = '0.###'
    end
    object ibdsDABBLOCK_NAME: TIBStringField
      FieldName = 'BLOCK_NAME'
      Origin = 'BLOCKDAB.NAME'
      Size = 4
    end
  end
  object dsDAB: TDataSource [20]
    DataSet = ibdsDAB
    Left = 684
    Top = 144
  end
  inherited sqlNewAdminId: TIBSQL
    Left = 596
    Top = 40
  end
end

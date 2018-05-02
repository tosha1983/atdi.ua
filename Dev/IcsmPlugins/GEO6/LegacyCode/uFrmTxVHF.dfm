inherited frmTxVHF: TfrmTxVHF
  Left = 222
  Top = 75
  BorderStyle = bsToolWindow
  Caption = 'frmTxVHF'
  ClientHeight = 519
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited lblTxType: TLabel
      Width = 43
      Caption = #1040#1056#1052
    end
    inherited edtID: TDBEdit
      Left = 726
    end
  end
  inherited pcData: TPageControl
    Height = 427
    inherited tshCommon: TTabSheet
      inherited pnlTech: TPanel
        Top = 45
        TabOrder = 1
      end
      inherited pnlMaster: TPanel
        Top = 235
        TabOrder = 2
        inherited cbStateIn: TDBLookupComboBox
          Left = 348
          Width = 133
        end
        inherited cbStateInCode: TDBLookupComboBox
          Left = 294
        end
      end
      object pnlForVHF: TPanel [2]
        Left = 0
        Top = 0
        Width = 772
        Height = 45
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 0
        object lblFreq: TLabel
          Left = 6
          Top = 18
          Width = 72
          Height = 13
          Caption = #1063#1072#1089#1090#1086#1090#1072', '#1052#1043#1094';'
        end
        object lbMonoStereo: TLabel
          Left = 344
          Top = 18
          Width = 72
          Height = 13
          Caption = #1042#1080#1076' '#1084#1086#1074#1083#1077#1085#1085#1103
        end
        object lblSystemCast: TLabel
          Left = 154
          Top = 18
          Width = 97
          Height = 13
          Caption = #1057#1080#1089#1090#1077#1084#1072' '#1084#1086#1074#1083#1077#1085#1085#1103
        end
        object lblClassRadiation: TLabel
          Left = 525
          Top = 18
          Width = 118
          Height = 13
          Caption = #1050#1083#1072#1089#1089' '#1074#1080#1087#1088#1086#1084#1110#1085#1102#1074#1072#1085#1085#1103
        end
        object edtFreq: TNumericEdit
          Left = 80
          Top = 14
          Width = 69
          Height = 21
          TabOrder = 0
          Text = 'edtFreq'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtFreq'
          OnValueChange = edtFreqValueChange
        end
        object cbMonoStereo: TComboBox
          Left = 418
          Top = 14
          Width = 89
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 2
          OnChange = cbMonoStereoChange
          Items.Strings = (
            #1052#1086#1085#1086
            #1057#1090#1077#1088#1077#1086)
        end
        object cbxTypeSysName: TComboBox
          Left = 254
          Top = 14
          Width = 73
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 1
          OnChange = cbxTypeSysNameChange
        end
        object edtClassRadiationVideo: TEdit
          Left = 648
          Top = 16
          Width = 105
          Height = 21
          ReadOnly = True
          TabOrder = 3
          Text = 'edtClassRadiationVideo'
          Visible = False
        end
        object edtSoundEmissionPrimary: TDBEdit
          Left = 654
          Top = 14
          Width = 82
          Height = 21
          DataField = 'SOUND_EMISSION_PRIMARY'
          DataSource = dsStantionsBase
          TabOrder = 4
        end
        object btnSoundEmissionPrimary: TBitBtn
          Left = 738
          Top = 14
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
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
      inherited gbxCoordination: TGroupBox
        Top = 196
        TabOrder = 3
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Left = 121
        Height = 237
        inherited dbgOrganizations: TDBGrid
          Height = 220
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 237
        Height = 164
        inherited dbgDocuments: TDBGrid
          Height = 147
        end
      end
      inherited gbDoc: TGroupBox
        Left = 589
        Width = 183
        Height = 237
      end
      inherited gbOrganization: TGroupBox
        Width = 121
        Height = 237
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
        Width = 763
        Height = 258
        inherited dbgEquipment: TDBGrid
          Width = 759
          Height = 241
        end
      end
      inherited pnlSummator: TPanel
        Width = 763
        inherited gbSummator: TGroupBox
          Width = 763
        end
      end
      inherited pnlFreqShift: TPanel
        Width = 763
      end
      inherited pnlEquipButton: TPanel
        Top = 359
        Width = 763
      end
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Width = 763
        Height = 401
        inherited dbgTestpoints: TDBGrid
          Width = 759
          Height = 384
        end
      end
    end
    inherited tshChangeLog: TTabSheet
      inherited dbgGhangeLog: TDBGrid
        Width = 763
        Height = 401
      end
      inherited panList: TPanel
        Width = 763
        Height = 401
        inherited dgrList: TDBGrid
          Width = 763
          Height = 376
        end
        inherited panSearch: TPanel
          Top = 376
          Width = 763
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 401
        inherited sb: TStatusBar
          Top = 382
        end
        inherited bmf: TBaseMapFrame
          Height = 353
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
    Top = 481
  end
  inherited ibdsStantionsBase: TIBDataSet
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
    Top = 40
  end
  inherited dsStantionsBase: TDataSource
    Top = 40
  end
  inherited ibdsLicenses: TIBDataSet
    Left = 684
    Top = 84
  end
  inherited dsLicenses: TDataSource
    Left = 732
    Top = 48
  end
  inherited ibqAccCondNameIn: TIBQuery
    Top = 424
  end
  inherited ibqAccCondNameOut: TIBQuery
    Top = 424
  end
  inherited dsStand: TDataSource
    Top = 424
  end
  inherited ibqUserName: TIBQuery
    Top = 424
  end
  inherited ibqTRKName: TIBQuery
    Top = 424
  end
  inherited ibqSystemCastName: TIBQuery
    Top = 424
  end
  inherited dsDocuments: TDataSource
    Left = 588
    Top = 368
  end
  inherited ibdsDocuments: TIBDataSet
    Left = 708
    Top = 420
  end
  inherited ActionList1: TActionList
    Left = 728
    Top = 16
  end
  inherited ibdsRetranslate: TIBDataSet
    Top = 420
  end
  inherited dsRetranslate: TDataSource
    Top = 424
  end
  inherited ibqTypeSystemName: TIBQuery
    SQL.Strings = (
      'select ID, CODSYSTEM from ANALOGRADIOSYSTEM')
    Top = 424
  end
  inherited ibqStand: TIBQuery
    Top = 424
  end
end

inherited frmTxFxm: TfrmTxFxm
  Caption = 'I'#1085#1096#1072' '#1087#1077#1088#1074#1080#1085#1085#1072' '#1089#1083#1091#1078#1073#1072
  ClientHeight = 525
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited lblTxType: TLabel
      Width = 42
      Caption = 'FXM'
    end
  end
  inherited pcData: TPageControl
    Height = 433
    inherited tshCommon: TTabSheet
      inherited pnlTech: TPanel
        Top = 41
      end
      inherited pnlMaster: TPanel
        Top = 241
        TabOrder = 4
      end
      inherited gbxCoordination: TGroupBox
        Top = 202
      end
      object pnEmission: TPanel
        Left = 0
        Top = 0
        Width = 772
        Height = 41
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 2
        object lbFreq: TLabel
          Left = 6
          Top = 11
          Width = 66
          Height = 13
          Caption = #1063#1072#1089#1090#1086#1090#1072', '#1082#1043#1094
        end
        object lbBw: TLabel
          Left = 410
          Top = 11
          Width = 62
          Height = 13
          Caption = #1055#1086#1083#1086#1089#1072', '#1082#1043#1094
        end
        object lbEmissionClass: TLabel
          Left = 607
          Top = 11
          Width = 25
          Height = 13
          Caption = #1050#1083#1072#1089
        end
        object lbServ: TLabel
          Left = 176
          Top = 11
          Width = 90
          Height = 13
          Caption = #1057#1080#1089#1090#1077#1084#1072' ('#1089#1083#1091#1078#1073#1072')'
        end
        object edFreq: TNumericEdit
          Left = 80
          Top = 8
          Width = 65
          Height = 21
          TabOrder = 0
          Text = 'edtFreq'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtFreq'
          OnValueChange = edFreqValueChange
        end
        object btEmissionClass: TBitBtn
          Left = 718
          Top = 8
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          Visible = False
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
        object edBw: TNumericEdit
          Left = 480
          Top = 8
          Width = 70
          Height = 21
          TabOrder = 2
          Text = '0'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = '0'
          OnValueChange = edBwValueChange
        end
        object edEmissionClass: TDBEdit
          Left = 640
          Top = 8
          Width = 70
          Height = 21
          TabOrder = 4
          Visible = False
        end
        object cbServ: TComboBox
          Left = 272
          Top = 8
          Width = 87
          Height = 21
          Style = csDropDownList
          DropDownCount = 40
          ItemHeight = 13
          TabOrder = 1
          OnChange = cbServChange
          Items.Strings = (
            'D'
            'ND')
        end
      end
      object pnFxm: TPanel
        Left = 0
        Top = 189
        Width = 772
        Height = 13
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 3
      end
    end
    inherited tshEquipment: TTabSheet
      inherited gbEquipment: TGroupBox
        Width = 772
        Height = 367
        inherited dbgEquipment: TDBGrid
          Width = 768
          Height = 350
        end
      end
      inherited pnlEquipButton: TPanel
        Top = 367
        Width = 772
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 407
        inherited sb: TStatusBar
          Top = 388
        end
        inherited bmf: TBaseMapFrame
          Height = 359
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
    Top = 487
  end
  object dsFxm: TIBDataSet
    Database = dmMain.dbMain
    Transaction = tr
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select AZIMUTHMAXRADIATION from TRANSMITTERS'
      'where ID = :ID')
    DataSource = dsStantionsBase
    Left = 600
    Top = 112
  end
  object srcFxm: TDataSource
    DataSet = dsFxm
    Left = 552
    Top = 112
  end
end

inherited frmTxTVA: TfrmTxTVA
  Left = 300
  Top = 162
  Caption = 'frmTxTVA'
  ClientHeight = 565
  ClientWidth = 792
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    Width = 792
    inherited lblTxType: TLabel
      Width = 40
      Caption = #1040#1058#1041
    end
    inherited edtID: TDBEdit
      Left = 736
    end
  end
  inherited pcData: TPageControl
    Width = 792
    Height = 473
    inherited tshCommon: TTabSheet
      inherited pnlTech: TPanel
        Top = 97
        Width = 784
        Height = 144
        TabOrder = 1
        inherited gbPower: TGroupBox
          Width = 265
          Height = 144
          inherited lblEBPmax: TLabel
            Left = 19
            Top = 25
          end
          inherited lblEBPG: TLabel
            Left = 19
            Top = 49
          end
          inherited lblEBPV: TLabel
            Left = 19
            Top = 73
          end
          inherited lblPower: TLabel
            Left = 19
            Top = 97
          end
          object lblVRatio: TLabel [4]
            Left = 8
            Top = 113
            Width = 130
            Height = 26
            Caption = #1042#1110#1076#1085#1086#1096#1077#1085#1085#1103' '#1087#1086#1090#1091#1078#1085#1086#1089#1090#1077#1081' '#1074#1110#1076#1077#1086' '#1076#1086' '#1079#1074#1091#1082#1091
            WordWrap = True
          end
          object lblPowerVideo: TLabel [5]
            Left = 88
            Top = 7
            Width = 26
            Height = 13
            Caption = #1074#1110#1076#1077#1086
          end
          object lblPowerSound1: TLabel [6]
            Left = 183
            Top = 7
            Width = 38
            Height = 13
            Caption = '     '#1079#1074#1091#1082
          end
          object lblPowerSound2: TLabel [7]
            Left = 240
            Top = 7
            Width = 29
            Height = 13
            Caption = #1079#1074#1091#1082'2'
            Visible = False
          end
          inherited edtEPRmaxAudio1: TNumericEdit
            Left = 183
            Top = 21
            Width = 50
            TabOrder = 15
          end
          inherited edtEPRGAudio1: TNumericEdit
            Left = 183
            Top = 45
            Width = 50
            TabOrder = 16
          end
          inherited edtEPRVAudio1: TNumericEdit
            Left = 183
            Top = 69
            Width = 50
            TabOrder = 17
          end
          inherited edtPowerAudio1: TNumericEdit
            Left = 183
            Top = 93
            Width = 50
            TabOrder = 8
          end
          inherited btnEBPG1: TButton
            Left = 149
            Top = 45
          end
          inherited btnEBPV1: TButton
            Left = 149
            Top = 69
          end
          object edtEPRmaxVideo: TNumericEdit
            Left = 95
            Top = 21
            Width = 53
            Height = 21
            TabOrder = 0
            Text = 'edtEBPmax'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtEBPmax'
            OnValueChange = edtEPRmaxVideoValueChange
          end
          object edtEPRGVideo: TNumericEdit
            Left = 95
            Top = 45
            Width = 53
            Height = 21
            TabOrder = 1
            Text = 'edtEBPG'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtEBPG'
            OnValueChange = edtEPRGVideoValueChange
          end
          object edtEPRVVideo: TNumericEdit
            Left = 95
            Top = 69
            Width = 53
            Height = 21
            TabOrder = 3
            Text = 'edtEBPV'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtEBPV'
            OnValueChange = edtEPRVVideoValueChange
          end
          object edtPowerVideo: TNumericEdit
            Left = 95
            Top = 93
            Width = 53
            Height = 21
            TabOrder = 5
            Text = 'edtPower'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtPower'
            OnValueChange = edtPowerVideoValueChange
          end
          object edtEPRmaxAudio2: TNumericEdit
            Left = 236
            Top = 21
            Width = 45
            Height = 21
            TabOrder = 10
            Text = 'edtEBPmax'
            Visible = False
            OldValue = 'edtEBPmax'
          end
          object edtEPRGAudio2: TNumericEdit
            Left = 236
            Top = 45
            Width = 45
            Height = 21
            TabOrder = 11
            Text = 'edtEBPG'
            Visible = False
            OldValue = 'edtEBPG'
          end
          object edtEPRVAudio2: TNumericEdit
            Left = 236
            Top = 69
            Width = 45
            Height = 21
            TabOrder = 12
            Text = 'edtEBPV'
            Visible = False
            OldValue = 'edtEBPV'
          end
          object edtPowerAudio2: TNumericEdit
            Left = 236
            Top = 93
            Width = 45
            Height = 21
            TabOrder = 13
            Text = 'edtPower'
            Visible = False
            OldValue = 'edtPower'
          end
          object edtVSoundRatio1: TNumericEdit
            Left = 183
            Top = 117
            Width = 50
            Height = 21
            TabOrder = 9
            Text = 'edtVSoundRatio1'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = 'edtVSoundRatio1'
            OnValueChange = edtVSoundRatio1ValueChange
          end
          object edtVSoundRatio2: TNumericEdit
            Left = 236
            Top = 117
            Width = 45
            Height = 21
            TabOrder = 14
            Text = 'edtVSoundRatio2'
            Visible = False
            OldValue = 'edtVSoundRatio2'
          end
        end
        inherited gbAntenna: TGroupBox
          Left = 265
          Width = 519
          Height = 144
          inherited lblAngle: TLabel
            Left = 420
          end
          inherited lblFiderLoss: TLabel
            Left = 325
          end
          inherited lblFiderLength: TLabel
            Left = 331
          end
          inherited lblAbgl2: TLabel
            Left = 360
          end
          inherited lblG: TLabel
            Left = 420
          end
          inherited edtFiderLoss: TNumericEdit
            Left = 432
          end
          inherited edtFiderLength: TNumericEdit
            Left = 432
          end
          inherited edtAngle: TNumericEdit
            Left = 432
            TabOrder = 10
          end
          inherited btnAntPattH: TButton
            TabOrder = 11
          end
          inherited edtAngle2: TNumericEdit
            Left = 432
            TabOrder = 9
          end
        end
      end
      object pnlVideoOffsetHerz: TPanel [1]
        Left = 0
        Top = 0
        Width = 784
        Height = 97
        Align = alTop
        BevelOuter = bvNone
        TabOrder = 0
        object lblChannel: TLabel
          Left = 24
          Top = 76
          Width = 31
          Height = 13
          Caption = #1050#1072#1085#1072#1083
        end
        object lblClassRadiation: TLabel
          Left = 354
          Top = 6
          Width = 118
          Height = 13
          Caption = #1050#1083#1072#1089#1089' '#1074#1080#1087#1088#1086#1084#1110#1085#1102#1074#1072#1085#1085#1103
          WordWrap = True
        end
        object lblVideo: TLabel
          Left = 216
          Top = 24
          Width = 53
          Height = 13
          Caption = #1074#1110#1076#1077#1086', '#1052#1043#1094
        end
        object lblAudio: TLabel
          Left = 220
          Top = 48
          Width = 50
          Height = 13
          Caption = #1079#1074#1091#1082', '#1052#1043#1094
        end
        object lblFreq: TLabel
          Left = 284
          Top = 6
          Width = 42
          Height = 13
          Caption = #1063#1072#1089#1090#1086#1090#1080
        end
        object lblShift: TLabel
          Left = 480
          Top = 48
          Width = 47
          Height = 13
          Caption = #1047#1053#1063', '#1082#1043#1094
        end
        object lblTip: TLabel
          Left = 484
          Top = 24
          Width = 43
          Height = 13
          Caption = #1090#1080#1087' '#1047#1053#1063
        end
        object Label2: TLabel
          Left = 212
          Top = 72
          Width = 56
          Height = 13
          Caption = #1079#1074#1091#1082'2, '#1052#1043#1094
          Enabled = False
          Visible = False
        end
        object Label3: TLabel
          Left = 484
          Top = 72
          Width = 45
          Height = 13
          Caption = #1047#1053#1063' '#1083#1110#1085#1110#1111
        end
        object Label4: TLabel
          Left = 656
          Top = 8
          Width = 96
          Height = 26
          Caption = #1058#1080#1087' '#1085#1077#1089#1090#1072#1073#1110#1083#1100#1085#1086#1089#1090#1110' '#1095#1072#1089#1090#1086#1090#1080
          WordWrap = True
        end
        object lblSystemCast: TLabel
          Left = 8
          Top = 44
          Width = 50
          Height = 26
          Caption = '  '#1057#1080#1089#1090#1077#1084#1072' '#1084#1086#1074#1083#1077#1085#1085#1103
          WordWrap = True
        end
        object Label1: TLabel
          Left = 12
          Top = 24
          Width = 47
          Height = 13
          Caption = #1057#1090#1072#1085#1076#1072#1088#1090
          WordWrap = True
        end
        object cbxChannel: TComboBox
          Left = 64
          Top = 72
          Width = 73
          Height = 21
          ItemHeight = 13
          TabOrder = 2
          OnChange = cbxChannelChange
        end
        object edtFreqVideo: TNumericEdit
          Left = 272
          Top = 22
          Width = 65
          Height = 21
          Enabled = False
          ReadOnly = True
          TabOrder = 5
          Text = 'edtFreqVideo'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtFreqVideo'
        end
        object edtFreqAudio1: TNumericEdit
          Left = 272
          Top = 46
          Width = 65
          Height = 21
          Enabled = False
          ReadOnly = True
          TabOrder = 7
          Text = 'edtFreqAudio1'
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtFreqAudio1'
        end
        object edtFreqAudio2: TNumericEdit
          Left = 272
          Top = 70
          Width = 65
          Height = 21
          Enabled = False
          ReadOnly = True
          TabOrder = 9
          Text = 'edtFreqAudio'
          Visible = False
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtFreqAudio'
        end
        object cbSystemcolor: TComboBox
          Left = 64
          Top = 48
          Width = 73
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 0
          OnChange = cbSystemcolorChange
          Items.Strings = (
            #1085#1077#1080#1079#1074
            'PAL'
            'SECAM'
            'NTSC')
        end
        object chbMonoStereo: TCheckBox
          Left = 144
          Top = 27
          Width = 61
          Height = 17
          Caption = #1057#1090#1077#1088#1077#1086
          Enabled = False
          TabOrder = 4
          Visible = False
          OnClick = chbMonoStereoClick
        end
        object cbxVideoOffsetLine: TComboBox
          Left = 534
          Top = 66
          Width = 99
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 13
          OnChange = cbxVideoOffsetLineChange
        end
        object dbcbTypeOffset: TDBComboBox
          Left = 534
          Top = 20
          Width = 99
          Height = 21
          Style = csDropDownList
          DataField = 'TYPEOFFSET'
          DataSource = dsTVA
          ItemHeight = 13
          Items.Strings = (
            #1053#1086#1088#1084#1072#1083#1100#1085#1077
            #1058#1086#1095#1085#1077
            #1057#1080#1085#1093#1088#1086#1085#1110#1079#1086#1074#1072#1085#1077
            #1053#1077#1074#1080#1079#1085#1072#1095#1077#1085#1077)
          TabOrder = 11
        end
        object cbxFreqStability: TDBComboBox
          Left = 656
          Top = 40
          Width = 93
          Height = 21
          Style = csDropDownList
          DataField = 'FREQSTABILITY'
          DataSource = dsTVA
          ItemHeight = 13
          Items.Strings = (
            'RELAXED'
            'NORMAL'
            'PRECISION')
          TabOrder = 14
        end
        object cbxTypeSysName: TComboBox
          Left = 64
          Top = 24
          Width = 73
          Height = 21
          ItemHeight = 13
          TabOrder = 1
          OnChange = cbxTypeSysNameChange
        end
        object cbxVideoOffsetHerz: TComboBox
          Left = 534
          Top = 42
          Width = 99
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 12
          OnChange = cbxVideoOffsetHerzChange
        end
        object btnNullChannel: TButton
          Left = 136
          Top = 72
          Width = 20
          Height = 20
          Caption = 'X'
          TabOrder = 3
          OnClick = btnNullChannelClick
        end
        object edtClassRadiationVideo: TEdit
          Left = 344
          Top = 30
          Width = 121
          Height = 21
          ReadOnly = True
          TabOrder = 6
          Visible = False
        end
        object edtClassRadiationAudio1: TEdit
          Left = 344
          Top = 54
          Width = 121
          Height = 21
          ReadOnly = True
          TabOrder = 8
          Visible = False
        end
        object edtClassRadiationAudio2: TEdit
          Left = 344
          Top = 78
          Width = 121
          Height = 21
          ReadOnly = True
          TabOrder = 10
          Visible = False
        end
        object edtVideoEmission: TDBEdit
          Left = 342
          Top = 22
          Width = 91
          Height = 21
          DataField = 'VIDEO_EMISSION'
          DataSource = dsStantionsBase
          TabOrder = 15
        end
        object edtSoundEmissionPrimary: TDBEdit
          Left = 342
          Top = 46
          Width = 91
          Height = 21
          DataField = 'SOUND_EMISSION_PRIMARY'
          DataSource = dsStantionsBase
          TabOrder = 16
        end
        object edtSoundEmissionSecond: TDBEdit
          Left = 342
          Top = 70
          Width = 91
          Height = 21
          DataField = 'SOUND_EMISSION_SECOND'
          DataSource = dsStantionsBase
          TabOrder = 17
          Visible = False
        end
        object btnVideoEmission: TBitBtn
          Left = 434
          Top = 22
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 18
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
        object btnSoundEmissionPrimary: TBitBtn
          Left = 434
          Top = 46
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 19
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
        object btnSoundEmissionSecond: TBitBtn
          Left = 434
          Top = 70
          Width = 19
          Height = 20
          Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
          ParentShowHint = False
          ShowHint = True
          TabOrder = 20
          Visible = False
          OnClick = btnSoundEmissionSecondClick
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
        Top = 281
        Width = 784
        TabOrder = 2
        inherited lblTypeRegistry: TDBText [10]
        end
        inherited lblOut: TLabel [11]
        end
        inherited lblIn: TLabel [12]
        end
        inherited lblClassWave: TLabel [13]
        end
        inherited lblTimeTransmit: TLabel [14]
        end
        inherited cbStateOut: TDBLookupComboBox [20]
        end
        inherited cbStateIn: TDBLookupComboBox [21]
        end
        inherited cbStateInCode: TDBLookupComboBox [22]
        end
        inherited cbStateOutCode: TDBLookupComboBox [23]
        end
        inherited dbedNumRegistry: TDBEdit [24]
        end
        inherited edtClassWave: TDBEdit [25]
        end
        inherited pcRemark: TPageControl [26]
          Width = 784
          inherited tshNote: TTabSheet
            inherited DBMemo1: TDBMemo
              Width = 776
            end
          end
        end
        inherited edtTimeTransmit: TDBEdit [27]
        end
      end
      inherited gbxCoordination: TGroupBox
        Top = 242
        Width = 784
        TabOrder = 3
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Height = 242
        inherited dbgOrganizations: TDBGrid
          Height = 225
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 242
        Width = 784
        inherited dbgDocuments: TDBGrid
          Width = 780
        end
      end
      inherited gbDoc: TGroupBox
        Width = 179
        Height = 242
      end
      inherited gbOrganization: TGroupBox
        Height = 242
      end
    end
    inherited tshEquipment: TTabSheet
      inherited gbEquipment: TGroupBox
        Width = 784
        Height = 338
        inherited dbgEquipment: TDBGrid
          Width = 780
          Height = 321
        end
      end
      inherited pnlSummator: TPanel
        Width = 784
        inherited gbSummator: TGroupBox
          Width = 784
        end
      end
      inherited pnlFreqShift: TPanel
        Width = 784
      end
      inherited pnlEquipButton: TPanel
        Top = 439
        Width = 784
      end
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Width = 779
        Height = 444
        inherited dbgTestpoints: TDBGrid
          Width = 775
          Height = 427
        end
      end
    end
    inherited tshChangeLog: TTabSheet
      inherited dbgGhangeLog: TDBGrid
        Width = 779
        Height = 444
      end
      inherited panList: TPanel
        Width = 779
        Height = 444
        inherited dgrList: TDBGrid
          Width = 779
          Height = 419
        end
        inherited panSearch: TPanel
          Top = 419
          Width = 779
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Width = 784
        Height = 447
        inherited sb: TStatusBar
          Top = 428
          Width = 784
        end
        inherited bmf: TBaseMapFrame
          Width = 784
          Height = 399
        end
        inherited tb: TToolBar
          Width = 784
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
    Top = 527
    Width = 792
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
      '            TRN.TYPEREGISTRY,'#9
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
      ''
      ''
      'where ID = :ID')
  end
  inherited ibdsLicenses: TIBDataSet
    Left = 704
    Top = 112
  end
  inherited dsLicenses: TDataSource
    DataSet = frmTxBase.ibdsLicenses
    Left = 732
    Top = 112
  end
  inherited ibqAccCondNameIn: TIBQuery
    Left = 60
    Top = 352
  end
  inherited ibqAccCondNameOut: TIBQuery
    Left = 88
    Top = 352
  end
  inherited dsStand: TDataSource
    Left = 40
    Top = 324
  end
  inherited ibqUserName: TIBQuery
    Left = 116
    Top = 352
  end
  inherited ibqTRKName: TIBQuery
    Left = 176
    Top = 352
  end
  inherited ibqSystemCastName: TIBQuery
    Left = 240
    Top = 352
  end
  inherited ibdsTelecomOrg: TIBDataSet
    Top = 300
  end
  inherited dsTelecomOrg: TDataSource
    Left = 12
    Top = 324
  end
  inherited dsDocuments: TDataSource
    Left = 508
    Top = 348
  end
  inherited ibdsDocuments: TIBDataSet
    Left = 452
    Top = 346
  end
  inherited ibqDocType: TIBQuery
    Left = 356
    Top = 296
  end
  inherited dsAccCondNameOut: TDataSource
    Top = 300
  end
  inherited dsAccCondNameIn: TDataSource
    Left = 132
    Top = 300
  end
  inherited ibdsEquipment: TIBDataSet
    Left = 332
    Top = 348
  end
  inherited dsEquipment: TDataSource
    Left = 360
    Top = 348
  end
  inherited dsTestpoints: TDataSource
    Left = 96
    Top = 324
  end
  inherited ibdsTestpoint: TIBDataSet
    Left = 96
    Top = 300
  end
  inherited ibqNewTx: TIBQuery
    Left = 512
    Top = 296
  end
  inherited ImageList1: TImageList
    Left = 764
  end
  inherited ibdsRetranslate: TIBDataSet
    Left = 332
    Top = 296
  end
  inherited dsRetranslate: TDataSource
    Left = 384
    Top = 296
  end
  inherited dsUserActLog: TDataSource
    Left = 388
    Top = 348
  end
  object dsTVA: TDataSource [31]
    DataSet = ibdsTVA
    Left = 732
    Top = 140
  end
  object ibqAnalogTeleSystemName: TIBQuery [32]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAMESYSTEM from ANALOGTELESYSTEM')
    Left = 412
    Top = 296
  end
  object ibdsTVA: TIBDataSet [33]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsTVAAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select '#9'ID,'
      #9'VIDEO_EMISSION,'
      #9'SOUND_EMISSION_PRIMARY, '
      #9'SOUND_EMISSION_SECOND,'
      #9'FREQSTABILITY,'
      #9'TYPESYSTEM,'
      #9'TYPEOFFSET'
      'from TRANSMITTERS'
      'where ID = :ID')
    SelectSQL.Strings = (
      'select      ID,'
      #9'VIDEO_EMISSION,'
      #9'SOUND_EMISSION_PRIMARY, '
      #9'SOUND_EMISSION_SECOND,'
      #9'FREQSTABILITY,'
      #9'TYPESYSTEM,'
      #9'TYPEOFFSET'
      'from TRANSMITTERS'
      'where ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS set'
      #9'VIDEO_EMISSION = :VIDEO_EMISSION,'
      #9'SOUND_EMISSION_PRIMARY = :SOUND_EMISSION_PRIMARY, '
      #9'SOUND_EMISSION_SECOND = :SOUND_EMISSION_SECOND,'
      #9'FREQSTABILITY = :FREQSTABILITY,'
      #9'TYPESYSTEM = :TYPESYSTEM,'
      #9'TYPEOFFSET = :TYPEOFFSET'
      'where ID = :ID')
    UniDirectional = True
    Left = 704
    Top = 140
    object ibdsTVAVIDEO_EMISSION: TIBStringField
      FieldName = 'VIDEO_EMISSION'
      Origin = 'TRANSMITTERS.VIDEO_EMISSION'
      Size = 16
    end
    object ibdsTVASOUND_EMISSION_PRIMARY: TIBStringField
      FieldName = 'SOUND_EMISSION_PRIMARY'
      Origin = 'TRANSMITTERS.SOUND_EMISSION_PRIMARY'
      Size = 16
    end
    object ibdsTVASOUND_EMISSION_SECOND: TIBStringField
      FieldName = 'SOUND_EMISSION_SECOND'
      Origin = 'TRANSMITTERS.SOUND_EMISSION_SECOND'
      Size = 16
    end
    object ibdsTVAFREQSTABILITY: TIBStringField
      FieldName = 'FREQSTABILITY'
      Origin = 'TRANSMITTERS.FREQSTABILITY'
      OnGetText = ibdsTVAFREQSTABILITYGetText
      OnSetText = ibdsTVAFREQSTABILITYSetText
      Size = 16
    end
    object ibdsTVATYPESYSTEM: TSmallintField
      FieldName = 'TYPESYSTEM'
      Origin = 'TRANSMITTERS.TYPESYSTEM'
    end
    object ibdsTVATYPEOFFSET: TIBStringField
      FieldName = 'TYPEOFFSET'
      Origin = 'TRANSMITTERS.TYPEOFFSET'
      OnGetText = ibdsTVATYPEOFFSETGetText
      OnSetText = ibdsTVATYPEOFFSETSetText
      Size = 16
    end
  end
  inherited ibdsUserActLog: TIBDataSet
    Left = 480
    Top = 348
  end
  inherited ibqTypeSystemName: TIBQuery
    SQL.Strings = (
      'select ID, NAMESYSTEM from ANALOGTELESYSTEM')
    Left = 268
    Top = 352
  end
  inherited ibdsAir: TIBDataSet
    Left = 456
    Top = 296
  end
  inherited dsAir: TDataSource
    Left = 484
    Top = 296
  end
  inherited ibqOwner: TIBQuery
    Left = 544
    Top = 348
  end
  inherited ibqStand: TIBQuery
    Left = 44
    Top = 284
  end
end

object frmPoint: TfrmPoint
  Left = 183
  Top = 248
  Width = 878
  Height = 281
  BorderStyle = bsSizeToolWin
  Caption = 'frmPoint'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsStayOnTop
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  OnResize = FormResize
  PixelsPerInch = 96
  TextHeight = 13
  object pnButtons: TPanel
    Left = 0
    Top = 229
    Width = 870
    Height = 25
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 0
    object btToExcel: TSpeedButton
      Left = 2
      Top = 2
      Width = 23
      Height = 22
      Glyph.Data = {
        E2040000424DE204000000000000A20200002800000018000000180000000100
        08000000000040020000120B0000120B00009B0000009B00000000000000FFFF
        FF00FF00FF00FAF8F900B9838500B9838400B8838400A7868700FFFEFE00FEFD
        FD00FDFCFC00B4807E00C29E9C00A8868300A87D7800AB817C00AB817D00C09A
        9600AB817B00AD898400AD847D00B1878000C19A9300C0999300B68B8200BA8F
        8400B78F8400BE938600A88378009E8B8500C1877300C1968700A08A8200F7F1
        EF00C3907B00C59A8900FFFAF800C99D8A00D8B9AB00D19F8700D2A48D00CDA1
        8C00FEFAF800FEFCFB00C4937700D7A88E00F7EEE900D0926D00D0987700DAA4
        8200DBA68500DFAF9100DFAF9200DAAC9000DFA98600D3A48100FAF2EC00FDF6
        F100FFFBF800D2A48000F7EDE400FBF3EC00FDF6F000DB9E6000DBA77400DCAC
        7C00FAEFE400F9EFE500FBF3EB00FDF7F100FAF5F000FFFBF700FEFAF600FCF9
        F600FFFEFD00FEFDFC00F7E5D100F8E7D400F7E6D300F5E5D300F8E9D900F9EC
        DE00FBF0E400FAF0E500FBF3EA00F4DBBC00F5DEC200F5E2CC00F6E4CE00F8E9
        D800F9EAD900F6E8D700F8EADA00FAEDDE00FBF1E500FCF6EF00FCF7F100EC9F
        3900F1D3AA00F2D6B000F3D9B600F4DBBB00F4DCBC00F5DEC100F5DFC200F4DE
        C200E7D3B800F5E1C700F5E1C800F7E3CA00F8E5CD00F6E4CD00F7E7D300F8EA
        D800FAEEDE00F9EDDE00FBF0E200F7ECDE00F8EFE300FBF4EB00F9F2E900FCF6
        EE00FEFBF700F3C17C00F3D9B500FAE3C200FAE7CB00F5E2C700F9EAD400FAED
        DB00FBF2E500FBF3E800FCF5EB00FCF7F000FFCA7400F6D8A700F7DDB300F8E0
        B900C9BAA300F8E8CF00FFBD4E00F6DBAD00FCE9C900DFD5C300FFF5E300FDFA
        F400FFFEFC00C6BEAD00FBF2DD00FFFCF500BBB5A400FFFFFD00FFFFFE000084
        0000848484000202020202020202999A999A999A999A999A999A999A999A0202
        0202020202029A999A999A999A999A999A999A999A990E04040506060606999A
        010101010101010101010101999A127E8E7D89888D879A990101010101010101
        010101019A990F576D56557C6362999A019999990101010199999901999A0F4F
        8B6C566564639A990101999999010199999901019A990F5B80586B676664999A
        010101999999999999010101999A1075814D6F6C68669A990101010199999999
        010101019A991476745C4E586C69999A010101019999999901010101999A1578
        837359706F6B9A990101019999999999990101019A99184679535150706F999A
        010199999901019999990101999A1949913D435D714E9A990199999901010101
        999999019A991B2B92453853725A999A010101010101010101010101999A1F4B
        013A3E445E5D9A990101010101010101010101019A99230A010824457753999A
        999A999A999A999A999A999A999A250A0101084760779A999A999A999A999A99
        9A999A999A99294B010101987A39445273594C6E7F6A8A20020202020202284B
        01010101982A8554427390948F93961D0202020202022D4B01010101014A7A5F
        8482261A130D071C02020202020235090101010101019748953C162C403F611E
        0202020202023309010101010101014A972E17377B8C2F020202020202023309
        01010101010101010121113B8630020202020202020234010101010101010101
        01030C4127020202020202020202313232323232323232323236220B02020202
        020202020202}
      OnClick = btToExcelClick
    end
  end
  object stringGrid: TStringGrid
    Left = 0
    Top = 36
    Width = 870
    Height = 164
    Align = alClient
    ColCount = 18
    Ctl3D = False
    DefaultColWidth = 40
    DefaultRowHeight = 14
    RowCount = 37
    Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goDrawFocusSelected, goRowSelect]
    ParentColor = True
    ParentCtl3D = False
    ScrollBars = ssVertical
    TabOrder = 1
    OnDrawCell = stringGridDrawCell
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
      14
      14)
  end
  object pnlCpReq: TPanel
    Left = 0
    Top = 0
    Width = 870
    Height = 36
    Align = alTop
    TabOrder = 2
    Visible = False
    object lblCPNumber: TLabel
      Left = 8
      Top = 4
      Width = 61
      Height = 13
      Caption = 'lblCPNumber'
    end
    object lblCPDistance: TLabel
      Left = 8
      Top = 20
      Width = 66
      Height = 13
      Caption = 'lblCPDistance'
    end
    object lblCPESum: TLabel
      Left = 8
      Top = 36
      Width = 52
      Height = 13
      Caption = 'lblCPESum'
    end
  end
  object pnSum: TPanel
    Left = 0
    Top = 200
    Width = 870
    Height = 29
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 3
    Visible = False
    object Label1: TLabel
      Left = 0
      Top = 8
      Width = 97
      Height = 13
      Caption = #1045#1074#1080#1082' '#1073#1077#1079' '#1047#1040#1042#1040#1044#1048':'
    end
    object lbEu: TLabel
      Left = 104
      Top = 8
      Width = 21
      Height = 13
      Caption = 'lbEu'
    end
    object Label3: TLabel
      Left = 192
      Top = 8
      Width = 94
      Height = 13
      Caption = #1045#1074#1080#1082' '#1079' '#1047#1040#1042#1040#1044#1054#1070':'
    end
    object lbEuIntfr: TLabel
      Left = 296
      Top = 8
      Width = 39
      Height = 13
      Caption = 'lbEuIntfr'
    end
    object Label5: TLabel
      Left = 376
      Top = 8
      Width = 42
      Height = 13
      Caption = #1056#1110#1079#1085#1080#1094#1103':'
    end
    object lbDiff: TLabel
      Left = 424
      Top = 8
      Width = 24
      Height = 13
      Caption = 'lbDiff'
    end
    object Label7: TLabel
      Left = 496
      Top = 0
      Width = 105
      Height = 52
      Caption = #1057#1091#1084#1072#1088#1085#1072' '#1079#1072#1074#1072#1076#1072' '#1074'i'#1076' '#1062#1058#1041' '#1090#1086#1075#1086' '#1078' '#1082#1072#1085#1072#1083#1091': '
      WordWrap = True
    end
    object lbSumIntfr: TLabel
      Left = 608
      Top = 8
      Width = 21
      Height = 13
      Caption = '-999'
    end
    object Label2: TLabel
      Left = 664
      Top = 0
      Width = 86
      Height = 26
      Caption = #1057#1091#1084#1072#1088#1085#1072' '#1079#1072#1074#1072#1076#1072' '#1074'i'#1076' '#1074#1080#1073#1088#1072#1085#1080#1093': '
      WordWrap = True
    end
    object lbSumSelected: TLabel
      Left = 760
      Top = 8
      Width = 21
      Height = 13
      Caption = '-999'
    end
  end
end

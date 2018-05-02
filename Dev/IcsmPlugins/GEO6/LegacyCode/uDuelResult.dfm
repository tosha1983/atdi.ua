object frmDuelResult: TfrmDuelResult
  Left = 346
  Top = 106
  Width = 838
  Height = 742
  BorderIcons = [biSystemMenu]
  Caption = #1044#1091#1077#1083#1100
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  KeyPreview = True
  OldCreateOrder = False
  Position = poMainFormCenter
  OnDeactivate = FormDeactivate
  OnKeyDown = FormKeyDown
  PixelsPerInch = 96
  TextHeight = 13
  object Splitter1: TSplitter
    Left = 0
    Top = 310
    Width = 830
    Height = 3
    Cursor = crVSplit
    Align = alTop
  end
  object panData: TPanel
    Left = 0
    Top = 580
    Width = 830
    Height = 135
    Align = alBottom
    BevelInner = bvLowered
    BevelOuter = bvNone
    TabOrder = 0
    OnResize = panDataResize
    DesignSize = (
      830
      135)
    object lblA: TLabel
      Left = 8
      Top = 8
      Width = 17
      Height = 13
      Caption = 'lblA'
    end
    object lblB: TLabel
      Left = 8
      Top = 24
      Width = 17
      Height = 13
      Caption = 'lblB'
    end
    object lblEminA: TLabel
      Left = 8
      Top = 40
      Width = 40
      Height = 13
      Caption = 'lblEminA'
    end
    object lblEminB: TLabel
      Left = 8
      Top = 56
      Width = 40
      Height = 13
      Caption = 'lblEminB'
    end
    object lblEa: TLabel
      Left = 152
      Top = 40
      Width = 23
      Height = 13
      Caption = 'lblEa'
    end
    object lblEb: TLabel
      Left = 152
      Top = 56
      Width = 23
      Height = 13
      Caption = 'lblEb'
    end
    object lblAData: TLabel
      Left = 32
      Top = 8
      Width = 40
      Height = 13
      Caption = 'lblAData'
    end
    object lblBData: TLabel
      Left = 32
      Top = 24
      Width = 40
      Height = 13
      Caption = 'lblBData'
    end
    object lblEminAData: TLabel
      Left = 48
      Top = 40
      Width = 63
      Height = 13
      Caption = 'lblEminAData'
    end
    object lblEminBData: TLabel
      Left = 48
      Top = 56
      Width = 63
      Height = 13
      Caption = 'lblEminBData'
    end
    object grdPoints: TStringGrid
      Left = 4
      Top = 42
      Width = 821
      Height = 88
      Anchors = [akLeft, akRight, akBottom]
      DefaultColWidth = 110
      DefaultRowHeight = 16
      FixedCols = 0
      FixedRows = 0
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine]
      ParentColor = True
      ScrollBars = ssNone
      TabOrder = 0
      OnDrawCell = grdPointsDrawCell
    end
  end
  object panGraph: TPanel
    Left = 0
    Top = 0
    Width = 830
    Height = 310
    Align = alTop
    BevelInner = bvLowered
    BevelOuter = bvNone
    TabOrder = 1
    OnMouseMove = panGraphMouseMove
    OnResize = panGraphResize
  end
  object panRelief: TPanel
    Left = 0
    Top = 313
    Width = 830
    Height = 267
    Align = alClient
    BevelInner = bvLowered
    BevelOuter = bvNone
    TabOrder = 2
    DesignSize = (
      830
      267)
    inline fmProfileView: TfmProfileView
      Left = 170
      Top = 1
      Width = 477
      Height = 265
      Anchors = [akTop, akBottom]
      AutoScroll = False
      Ctl3D = True
      ParentCtl3D = False
      TabOrder = 0
      inherited txtHeight: TStaticText
        Top = 247
      end
      inherited chbEarthCurve: TCheckBox
        Top = 246
      end
      inherited txtDist: TStaticText
        Left = 424
        Top = 247
      end
    end
  end
end

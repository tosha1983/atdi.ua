object LisMapX: TLisMapX
  Left = 585
  Top = 189
  Width = 502
  Height = 435
  Caption = 'LisMapX'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  inline cmf: TCustomMapFrame
    Left = 0
    Top = 0
    Width = 494
    Height = 408
    Align = alClient
    ParentShowHint = False
    ShowHint = True
    TabOrder = 0
    inherited sb: TStatusBar
      Top = 389
      Width = 494
    end
    inherited bmf: TBaseMapFrame
      Width = 494
      Height = 360
    end
    inherited tb: TToolBar
      Width = 494
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

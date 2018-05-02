object frmCheckPosition: TfrmCheckPosition
  Left = 507
  Top = 186
  Width = 610
  Height = 523
  Caption = 'frmCheckPosition'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIChild
  OldCreateOrder = False
  Position = poMainFormCenter
  Visible = True
  OnClose = FormClose
  PixelsPerInch = 96
  TextHeight = 13
  inline cmf: TCustomMapFrame
    Left = 0
    Top = 0
    Width = 602
    Height = 496
    Align = alClient
    TabOrder = 0
    inherited sb: TStatusBar
      Top = 477
      Width = 602
    end
    inherited bmf: TBaseMapFrame
      Width = 602
      Height = 448
    end
    inherited tb: TToolBar
      Width = 602
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
    end
  end
end

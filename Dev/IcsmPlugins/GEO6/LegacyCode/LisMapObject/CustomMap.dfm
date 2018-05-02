object CustomMapFrame: TCustomMapFrame
  Left = 0
  Top = 0
  Width = 595
  Height = 421
  ParentShowHint = False
  ShowHint = True
  TabOrder = 0
  OnMouseWheelDown = FrameMouseWheelDown
  OnMouseWheelUp = FrameMouseWheelUp
  object sb: TStatusBar
    Left = 0
    Top = 402
    Width = 595
    Height = 19
    Panels = <
      item
        Text = #1050#1086#1086#1088#1076#1080#1085#1072#1090#1099
        Width = 150
      end
      item
        Text = #1052#1072#1089#1096#1090#1072#1073
        Width = 70
      end
      item
        Text = #1042#1099#1089#1086#1090#1072
        Width = 70
      end
      item
        Text = #1055#1086#1082#1088#1099#1090#1080#1077
        Width = 70
      end
      item
        Text = #1053#1072#1087#1088#1103#1078#1105#1085#1085#1086#1089#1090#1100
        Width = 70
      end
      item
        Text = #1044#1080#1089#1090#1072#1085#1094#1080#1103
        Width = 70
      end
      item
        Width = 50
      end>
    SimplePanel = False
  end
  inline bmf: TBaseMapFrame
    Left = 0
    Top = 29
    Width = 595
    Height = 373
    Align = alClient
    TabOrder = 1
    Visible = False
    inherited al: TActionList
      inherited actZoomInTwice: TAction
        OnExecute = bmfactZoomInTwiceExecute
      end
      inherited actZoomOutTwice: TAction
        OnExecute = bmfactZoomOutTwiceExecute
      end
      inherited actZoomDefault: TAction
        OnExecute = bmfactZoomDefaultExecute
      end
      inherited actZoomCust: TAction
        OnExecute = bmfactZoomCustExecute
      end
    end
  end
  object tb: TToolBar
    Left = 0
    Top = 0
    Width = 595
    Height = 29
    Caption = 'tb'
    Flat = True
    Images = bmf.iml
    TabOrder = 2
    object tb1: TToolButton
      Left = 0
      Top = 0
      Action = bmf.actNone
      Grouped = True
      Style = tbsCheck
    end
    object tb2: TToolButton
      Left = 23
      Top = 0
      Action = bmf.actPan
      Down = True
      Grouped = True
      Style = tbsCheck
    end
    object tb3: TToolButton
      Left = 46
      Top = 0
      Action = bmf.actZoomInTwice
    end
    object tb4: TToolButton
      Left = 69
      Top = 0
      Action = bmf.actZoomOutTwice
    end
    object tb5: TToolButton
      Left = 92
      Top = 0
      Action = bmf.actConf
    end
    object tb6: TToolButton
      Left = 115
      Top = 0
      Action = bmf.actArrows
      Style = tbsCheck
    end
  end
end

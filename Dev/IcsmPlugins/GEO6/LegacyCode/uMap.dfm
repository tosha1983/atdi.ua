object fmMap: TfmMap
  Left = 0
  Top = 0
  Width = 731
  Height = 396
  ParentShowHint = False
  ShowHint = True
  TabOrder = 0
  object sbMap: TStatusBar
    Left = 0
    Top = 375
    Width = 731
    Height = 21
    Panels = <
      item
        Width = 140
      end
      item
        Width = 0
      end
      item
        Width = 100
      end
      item
        Width = 120
      end
      item
        Width = 50
      end
      item
        Width = 50
      end
      item
        Width = 140
      end
      item
        Width = 200
      end
      item
        Width = 50
      end>
    SimplePanel = False
  end
  object theMap: TMapUniversalX
    Left = 0
    Top = 0
    Width = 731
    Height = 375
    Align = alClient
    TabOrder = 1
    OnMouseDown = theMapMouseDown
    OnMouseUp = theMapMouseUp
    OnMouseMove = theMapMouseMove
    OnSelectedZone = theMapSelectedZone
    ControlData = {
      545046300D5444585363726F6C6C4472617700044C656674020003546F700200
      05576964746803DB020648656967687403770106437572736F72020205436F6C
      6F72070B636C5363726F6C6C4261720B506172656E74436F6C6F72080000}
  end
  object pmnTx: TPopupMenu
    Left = 208
    Top = 112
    object mniTxEdit: TMenuItem
    end
    object mniAnalyze: TMenuItem
    end
    object mniUseInCalc: TMenuItem
    end
    object mniShowTestPoints: TMenuItem
    end
  end
end

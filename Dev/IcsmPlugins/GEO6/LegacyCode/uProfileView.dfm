object fmProfileView: TfmProfileView
  Left = 0
  Top = 0
  Width = 508
  Height = 101
  AutoScroll = False
  Ctl3D = True
  ParentCtl3D = False
  TabOrder = 0
  OnMouseMove = FrameMouseMove
  OnResize = FrameResize
  DesignSize = (
    508
    101)
  object txtHeight: TStaticText
    Left = 132
    Top = 83
    Width = 173
    Height = 17
    Anchors = [akLeft, akBottom]
    AutoSize = False
    Caption = #1042#1080#1089#1086#1090#1072
    Constraints.MinWidth = 80
    TabOrder = 1
  end
  object chbEarthCurve: TCheckBox
    Left = 4
    Top = 82
    Width = 97
    Height = 17
    Anchors = [akLeft, akBottom]
    Caption = #1050#1088#1080#1074#1110#1079#1085#1072' '#1047#1077#1084#1083#1110' '
    Checked = True
    State = cbChecked
    TabOrder = 0
    OnClick = chbEarthCurveClick
  end
  object txtDist: TStaticText
    Left = 456
    Top = 83
    Width = 48
    Height = 17
    Alignment = taRightJustify
    Anchors = [akRight, akBottom]
    AutoSize = False
    Caption = #1042#1110#1076#1089#1090#1072#1085#1100
    TabOrder = 2
  end
end

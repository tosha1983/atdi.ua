object fmWhereCriteria: TfmWhereCriteria
  Left = 0
  Top = 0
  Width = 476
  Height = 242
  TabOrder = 0
  object edit: TEdit
    Left = 240
    Top = 192
    Width = 121
    Height = 21
    TabOrder = 1
    Text = 'edit'
  end
  object tvwCriteria: TTreeView
    Left = 0
    Top = 0
    Width = 476
    Height = 242
    Align = alClient
    Indent = 19
    PopupMenu = pmnWhere
    ReadOnly = True
    RowSelect = True
    TabOrder = 0
    OnKeyDown = tvwCriteriaKeyDown
    OnMouseDown = tvwCriteriaMouseDown
  end
  object pmnWhere: TPopupMenu
    Left = 32
    Top = 120
    object mniAddCondition: TMenuItem
      Caption = '&'#1044#1086#1076#1072#1090#1080' '#1082#1088#1110#1090#1077#1088#1110#1081
      OnClick = mniAddConditionClick
    end
    object mniAddComposeCondition: TMenuItem
      Caption = #1044#1086#1076#1072#1090#1080' &'#1057#1086#1089#1090#1072#1074#1085#1080#1081' '#1082#1088#1110#1090#1077#1088#1110#1081
      OnClick = mniAddComposeConditionClick
    end
    object mniMakeComposeCondition: TMenuItem
      Caption = '&'#1047#1088#1086#1073#1080#1090#1080' '#1082#1088#1110#1090#1077#1088#1110#1081' '#1089#1086#1089#1090#1072#1074#1085#1080#1084#1102
      OnClick = mniMakeComposeConditionClick
    end
    object DeleteCondition: TMenuItem
      Caption = '&'#1042#1080#1076#1072#1083#1080#1090#1080' '#1082#1088#1110#1090#1077#1088#1110#1081
      OnClick = DeleteConditionClick
    end
  end
end

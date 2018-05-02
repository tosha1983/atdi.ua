object frmSelectColumns: TfrmSelectColumns
  Left = 357
  Top = 281
  VertScrollBar.Visible = False
  BorderStyle = bsDialog
  ClientHeight = 314
  ClientWidth = 556
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 8
    Width = 60
    Height = 13
    Caption = #1044#1086#1089#1090#1091#1087#1085#1099#1077':'
  end
  object Label2: TLabel
    Left = 256
    Top = 8
    Width = 62
    Height = 13
    Caption = #1042#1099#1073#1088#1072#1085#1085#1099#1077':'
  end
  object lbAccessibleFields: TListBox
    Left = 4
    Top = 24
    Width = 196
    Height = 285
    ItemHeight = 13
    MultiSelect = True
    TabOrder = 0
    OnDblClick = btnAddClick
  end
  object lbSelectedFields: TListBox
    Left = 248
    Top = 24
    Width = 197
    Height = 285
    ItemHeight = 13
    MultiSelect = True
    TabOrder = 1
    OnDblClick = btnRemoveClick
  end
  object btnAdd: TButton
    Left = 212
    Top = 72
    Width = 25
    Height = 25
    Caption = '>'
    TabOrder = 2
    OnClick = btnAddClick
  end
  object btnAddAll: TButton
    Left = 212
    Top = 101
    Width = 25
    Height = 25
    Caption = '>>'
    TabOrder = 3
    OnClick = btnAddAllClick
  end
  object btnRemove: TButton
    Left = 212
    Top = 142
    Width = 25
    Height = 25
    Caption = '<'
    TabOrder = 4
    OnClick = btnRemoveClick
  end
  object btnRemoveAll: TButton
    Left = 212
    Top = 171
    Width = 25
    Height = 25
    Caption = '<<'
    TabOrder = 5
    OnClick = btnRemoveAllClick
  end
  object btnUp: TButton
    Left = 464
    Top = 164
    Width = 75
    Height = 25
    Caption = #1042#1074#1077#1088#1093
    TabOrder = 6
    OnClick = btnUpClick
  end
  object btnDown: TButton
    Left = 464
    Top = 196
    Width = 75
    Height = 25
    Caption = #1042#1085#1080#1079
    TabOrder = 7
    OnClick = btnDownClick
  end
  object btnOk: TButton
    Left = 464
    Top = 240
    Width = 75
    Height = 25
    Caption = #1054#1050
    TabOrder = 8
    OnClick = btnOkClick
  end
  object btnCancel: TButton
    Left = 464
    Top = 272
    Width = 75
    Height = 25
    Caption = #1054#1090#1084#1077#1085#1072
    TabOrder = 9
    OnClick = btnCancelClick
  end
end

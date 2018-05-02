object frmRrc06Export: TfrmRrc06Export
  Left = 545
  Top = 218
  BorderStyle = bsDialog
  Caption = #1069#1082#1089#1087#1086#1088#1090
  ClientHeight = 416
  ClientWidth = 453
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    453
    416)
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 16
    Top = 12
    Width = 59
    Height = 13
    Caption = 'I'#1084#39#1103' '#1092#1072#1081#1083#1091': '
  end
  object Label2: TLabel
    Left = 16
    Top = 40
    Width = 53
    Height = 13
    Caption = #1050#1086#1084#1077#1085#1090#1072#1088':'
  end
  object Label3: TLabel
    Left = 16
    Top = 112
    Width = 84
    Height = 13
    Caption = #1057#1077#1082#1094'i'#1103' <HEAD> :'
  end
  object Label4: TLabel
    Left = 111
    Top = 219
    Width = 50
    Height = 13
    Caption = 't_action = '
  end
  object Label5: TLabel
    Left = 7
    Top = 243
    Width = 168
    Height = 13
    Caption = #1071#1082#1097#1086' '#1085#1077' '#1074#1082#1072#1079#1072#1085#1086', t_is_pub_req = '
  end
  object Label6: TLabel
    Left = 10
    Top = 275
    Width = 160
    Height = 13
    Caption = #1044#1083#1103' Art.5 t_remark_conds_met = '
  end
  object Label7: TLabel
    Left = 62
    Top = 299
    Width = 109
    Height = 13
    Caption = #1044#1083#1103' Art.5 t_is_resub = '
  end
  object Label8: TLabel
    Left = 9
    Top = 323
    Width = 163
    Height = 13
    Caption = #1044#1083#1103' Art.5 t_signed_commitment = '
  end
  object edtFileName: TEdit
    Left = 80
    Top = 8
    Width = 323
    Height = 21
    Anchors = [akLeft, akTop, akRight]
    TabOrder = 2
  end
  object btnFile: TButton
    Left = 408
    Top = 8
    Width = 21
    Height = 21
    Anchors = [akTop, akRight]
    Caption = '...'
    TabOrder = 3
    OnClick = btnFileClick
  end
  object btnImport: TButton
    Left = 282
    Top = 383
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1045#1082#1089#1087#1086#1088#1090'!'
    ModalResult = 1
    TabOrder = 0
    OnClick = btnImportClick
  end
  object btnCancel: TButton
    Left = 370
    Top = 383
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1047#1072#1082#1088#1080#1090#1080
    ModalResult = 2
    TabOrder = 1
  end
  object memComment: TMemo
    Left = 80
    Top = 40
    Width = 353
    Height = 65
    Anchors = [akLeft, akTop, akRight]
    Lines.Strings = (
      'memComment')
    TabOrder = 4
  end
  object sgHead: TStringGrid
    Left = 112
    Top = 112
    Width = 320
    Height = 97
    Anchors = [akLeft, akTop, akRight]
    ColCount = 2
    DefaultColWidth = 120
    DefaultRowHeight = 16
    FixedCols = 0
    RowCount = 4
    Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goEditing]
    TabOrder = 5
  end
  object cbxAction: TComboBox
    Left = 176
    Top = 216
    Width = 81
    Height = 21
    ItemHeight = 13
    TabOrder = 6
    Text = 'MODIFY'
    Items.Strings = (
      'ADD'
      'MODIFY'
      'SUPPRESS')
  end
  object chbOpenFile: TCheckBox
    Left = 112
    Top = 352
    Width = 201
    Height = 17
    Caption = #1042'i'#1076#1082#1088#1080#1090#1080' '#1092#1072#1081#1083' '#1087'i'#1089#1083#1103' '#1092#1086#1088#1084#1091#1074#1072#1085#1085#1103
    TabOrder = 7
  end
  object rgArt: TRadioGroup
    Left = 352
    Top = 216
    Width = 81
    Height = 57
    Caption = ' '#1057#1090#1072#1090#1090#1103' '
    ItemIndex = 0
    Items.Strings = (
      'Art. 4'
      'Art. 5')
    TabOrder = 8
  end
  object cbIsPubReq: TComboBox
    Left = 176
    Top = 240
    Width = 81
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    ItemIndex = 0
    TabOrder = 9
    Text = 'TRUE'
    Items.Strings = (
      'TRUE'
      'FALSE')
  end
  object cbRemarkCondsMet: TComboBox
    Left = 176
    Top = 272
    Width = 81
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    ItemIndex = 0
    TabOrder = 10
    Text = 'TRUE'
    Items.Strings = (
      'TRUE'
      'FALSE')
  end
  object cbIsResub: TComboBox
    Left = 176
    Top = 296
    Width = 81
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    ItemIndex = 0
    TabOrder = 11
    Text = 'TRUE'
    Items.Strings = (
      'TRUE'
      'FALSE')
  end
  object cbSignedCommitment: TComboBox
    Left = 176
    Top = 320
    Width = 81
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    ItemIndex = 0
    TabOrder = 12
    Text = 'TRUE'
    Items.Strings = (
      'TRUE'
      'FALSE')
  end
end

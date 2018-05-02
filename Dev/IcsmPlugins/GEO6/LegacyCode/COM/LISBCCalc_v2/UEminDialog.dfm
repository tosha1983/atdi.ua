object EminDialog: TEminDialog
  Left = 329
  Top = 154
  BorderStyle = bsDialog
  Caption = #1052#1110#1085#1110#1084#1072#1083#1100#1085#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100
  ClientHeight = 312
  ClientWidth = 359
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 8
    Width = 82
    Height = 13
    Caption = #1045#1084#1110#1085'. ('#1076#1041#1084#1082#1042'/'#1084')'
  end
  object Label2: TLabel
    Left = 8
    Top = 224
    Width = 55
    Height = 13
    Caption = #1056#1077#1079#1091#1083#1100#1090#1072#1090':'
  end
  object Label3: TLabel
    Left = 72
    Top = 216
    Width = 30
    Height = 24
    Caption = '___'
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -19
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
  end
  object Ok: TButton
    Left = 96
    Top = 280
    Width = 75
    Height = 25
    Caption = 'Ok'
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object Button2: TButton
    Left = 176
    Top = 280
    Width = 75
    Height = 25
    Caption = #1042#1110#1076#1084#1110#1085#1072
    ModalResult = 2
    TabOrder = 1
  end
  object ListBox1: TListBox
    Left = 8
    Top = 24
    Width = 345
    Height = 121
    Color = clBtnFace
    ItemHeight = 13
    Items.Strings = (
      '46 '#1076#1041'- '#1079#1072#1093#1080#1089#1090' '#1074#1080#1076#1110#1083#1077#1085#1100' ('#1076#1083#1103' '#1089#1091#1093#1086#1087#1091#1090#1085#1080#1093' '#1090#1088#1072#1089')'
      '49 '#1076#1041'- '#1079#1072#1093#1080#1089#1090' '#1074#1080#1076#1110#1083#1077#1085#1100' ('#1076#1083#1103' '#1084#1086#1088#1089#1100#1082#1080#1093' '#1090#1088#1072#1089')'
      '12 '#1076#1041'- '#1090#1088#1080#1075#1075#1077#1088#1085#1077' '#1079#1085#1072#1095#1077#1085#1085#1103' '#1076#1083#1103' '#1062#1056#1052' '
      '17 '#1076#1041'- '#1090#1088#1080#1075#1075#1077#1088#1085#1077' '#1079#1085#1072#1095#1077#1085#1085#1103' '#1076#1083#1103' '#1062#1058#1042' (III '#1076#1110#1072#1087#1072#1079#1086#1085' '#1058#1042#1050' 5-12)'
      '21 '#1076#1041'- '#1090#1088#1080#1075#1075#1077#1088#1085#1077' '#1079#1085#1072#1095#1077#1085#1085#1103' '#1076#1083#1103' '#1062#1058#1042' (IV '#1076#1110#1072#1087#1072#1079#1086#1085'  '#1058#1042#1050' 21-34)'
      '23 '#1076#1041'- '#1090#1088#1080#1075#1075#1077#1088#1085#1077' '#1079#1085#1072#1095#1077#1085#1085#1103' '#1076#1083#1103' '#1062#1058#1042' (V '#1076#1110#1072#1087#1072#1079#1086#1085' '#1058#1042#1050' 35-51) '
      '25 '#1076#1041'- '#1090#1088#1080#1075#1075#1077#1088#1085#1077' '#1079#1085#1072#1095#1077#1085#1085#1103' '#1076#1083#1103' '#1062#1058#1042' (V '#1076#1110#1072#1087#1072#1079#1086#1085' '#1058#1042#1050' 52-69)')
    TabOrder = 2
    OnClick = ListBox1Click
  end
  object Edit1: TEdit
    Left = 64
    Top = 152
    Width = 81
    Height = 24
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -13
    Font.Name = 'MS Sans Serif'
    Font.Style = [fsBold]
    ParentFont = False
    TabOrder = 3
    Text = '46'
    OnExit = Edit1Exit
  end
  object CheckBox1: TCheckBox
    Left = 152
    Top = 152
    Width = 153
    Height = 17
    Caption = #1063#1072#1089#1090#1086#1090#1085#1072' '#1110#1085#1090#1077#1088#1087#1086#1083#1103#1094#1110#1103
    Checked = True
    State = cbChecked
    TabOrder = 4
    OnClick = CheckBox1Click
  end
  object CheckBox2: TCheckBox
    Left = 152
    Top = 168
    Width = 201
    Height = 17
    Caption = #1052#1086#1073#1110#1083#1100#1085#1080#1081' '#1072#1073#1086' '#1087#1086#1088#1090#1072#1090#1080#1074#1085#1080#1081' '#1087#1088#1080#1081#1086#1084
    TabOrder = 5
    OnClick = CheckBox2Click
  end
end

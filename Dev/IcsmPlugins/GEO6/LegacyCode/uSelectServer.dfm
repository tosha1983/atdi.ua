object dlgSelectServer: TdlgSelectServer
  Left = 527
  Top = 472
  BorderStyle = bsDialog
  Caption = #1044#1086#1073#1072#1074#1080#1090#1100' '#1089#1077#1088#1074#1077#1088
  ClientHeight = 232
  ClientWidth = 593
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    593
    232)
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 577
    Height = 185
    Anchors = [akLeft, akTop, akRight, akBottom]
    Shape = bsFrame
  end
  object Label1: TLabel
    Left = 16
    Top = 16
    Width = 385
    Height = 13
    Caption = 
      #1057#1085#1072#1095#1072#1083#1072' '#1074#1099#1073#1077#1088#1080#1090#1077' '#1092#1072#1081#1083', '#1072' '#1074' '#1085#1105#1084' '#1086#1076#1080#1085' '#1080#1083#1080' '#1085#1077#1089#1082#1086#1083#1100#1082#1086' '#1089#1077#1088#1074#1077#1088#1086#1074' ('#1077#1089#1083#1080 +
      ' '#1077#1089#1090#1100'):'
  end
  object OKBtn: TButton
    Left = 397
    Top = 201
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 0
    OnClick = OKBtnClick
  end
  object CancelBtn: TButton
    Left = 485
    Top = 201
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 1
  end
  object edFileName: TEdit
    Left = 16
    Top = 32
    Width = 534
    Height = 21
    Anchors = [akLeft, akTop, akRight]
    ParentColor = True
    TabOrder = 2
  end
  object btFile: TButton
    Left = 553
    Top = 32
    Width = 21
    Height = 21
    Anchors = [akTop, akRight]
    Caption = '...'
    TabOrder = 3
    OnClick = btFileClick
  end
  object ServerList: TListBox
    Left = 16
    Top = 56
    Width = 561
    Height = 129
    ItemHeight = 13
    MultiSelect = True
    ScrollWidth = 5
    TabOrder = 4
  end
  object AddNewServ: TButton
    Left = 304
    Top = 201
    Width = 83
    Height = 25
    Caption = #1053#1086#1074#1099#1081' '#1089#1077#1088#1074#1077#1088
    TabOrder = 5
    OnClick = AddNewServClick
  end
  object OpenDialog1: TOpenDialog
    Filter = 
      #1060#1072#1081#1083#1099' '#1073#1080#1073#1083#1080#1086#1090#1077#1082' (*.dll;*.ocx;*.exe)|*.dll;*.ocx;*.exe|'#1042#1089#1077' '#1092#1072#1081#1083#1099' ' +
      '(*.*)|*.*'
    Left = 488
  end
end

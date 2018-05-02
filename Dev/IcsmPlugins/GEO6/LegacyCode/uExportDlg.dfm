object dlgExport: TdlgExport
  Left = 415
  Top = 281
  BorderStyle = bsDialog
  Caption = #1045#1082#1089#1087#1086#1088#1090' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
  ClientHeight = 206
  ClientWidth = 313
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    313
    206)
  PixelsPerInch = 96
  TextHeight = 13
  object OKBtn: TButton
    Left = 135
    Top = 176
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
    Left = 223
    Top = 176
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 1
  end
  object Panel1: TPanel
    Left = 0
    Top = 0
    Width = 313
    Height = 169
    Anchors = [akLeft, akTop, akRight, akBottom]
    TabOrder = 2
    DesignSize = (
      313
      169)
    object Label1: TLabel
      Left = 20
      Top = 112
      Width = 29
      Height = 13
      Caption = #1060#1072#1081#1083
    end
    object rgList: TRadioGroup
      Left = 8
      Top = 16
      Width = 297
      Height = 37
      Anchors = [akLeft, akTop, akRight]
      Caption = #1057#1087#1080#1089#1086#1082' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
      Columns = 2
      ItemIndex = 0
      Items.Strings = (
        #1042#1089#1110
        #1042#1080#1076#1110#1083#1077#1085#1110)
      TabOrder = 0
    end
    object rgFormat: TRadioGroup
      Left = 8
      Top = 60
      Width = 297
      Height = 37
      Caption = #1060#1086#1088#1084#1072#1090
      Columns = 3
      ItemIndex = 0
      Items.Strings = (
        'TVA'
        'TVD'
        'TVA && TVD')
      TabOrder = 1
      OnClick = rgFormatClick
    end
    object edtFilename: TEdit
      Left = 12
      Top = 128
      Width = 269
      Height = 21
      Anchors = [akLeft, akTop, akRight]
      TabOrder = 2
    end
    object btnFilename: TButton
      Left = 284
      Top = 128
      Width = 21
      Height = 21
      Anchors = [akTop, akRight]
      Caption = '...'
      TabOrder = 3
      OnClick = btnFilenameClick
    end
  end
  object opdFile: TOpenDialog
    Filter = #1042#1089#1110' '#1092#1072#1081#1083#1080' ()|*.*|'#1060#1072#1081#1083#1080' TVA ()|*.TVA'
    Options = [ofHideReadOnly, ofPathMustExist, ofNoReadOnlyReturn, ofEnableSizing]
    Left = 256
    Top = 8
  end
end

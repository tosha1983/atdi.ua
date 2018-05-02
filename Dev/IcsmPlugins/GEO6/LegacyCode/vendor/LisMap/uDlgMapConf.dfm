object dlgMapConf: TdlgMapConf
  Left = 321
  Top = 170
  BorderStyle = bsDialog
  Caption = #1050#1086#1085#1092#1080#1075#1091#1088#1072#1094#1080#1103
  ClientHeight = 358
  ClientWidth = 355
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  DesignSize = (
    355
    358)
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 211
    Width = 50
    Height = 13
    Caption = #1053#1072#1079#1074#1072#1085#1080#1077
  end
  object btnOk: TButton
    Left = 177
    Top = 324
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1044#1072
    Default = True
    ModalResult = 1
    TabOrder = 0
    OnClick = btnOkClick
  end
  object btnCancel: TButton
    Left = 265
    Top = 324
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = #1054#1090#1084#1077#1085#1072
    ModalResult = 2
    TabOrder = 1
  end
  object lbxParams: TListBox
    Left = 8
    Top = 16
    Width = 306
    Height = 121
    Anchors = [akLeft, akTop, akRight]
    ItemHeight = 13
    TabOrder = 2
    OnClick = lbxParamsClick
    OnKeyDown = lbxParamsKeyDown
    OnKeyUp = lbxParamsKeyUp
  end
  object edtFilePath: TEdit
    Left = 8
    Top = 144
    Width = 306
    Height = 21
    Anchors = [akLeft, akTop, akRight]
    TabOrder = 3
  end
  object btnUp: TBitBtn
    Left = 321
    Top = 32
    Width = 25
    Height = 25
    Anchors = [akTop, akRight]
    TabOrder = 4
    OnClick = btnUpClick
    Glyph.Data = {
      76010000424D7601000000000000760000002800000020000000100000000100
      04000000000000010000120B0000120B00001000000000000000000000000000
      800000800000008080008000000080008000808000007F7F7F00BFBFBF000000
      FF0000FF000000FFFF00FF000000FF00FF00FFFF0000FFFFFF00333333000333
      3333333333777F33333333333309033333333333337F7F333333333333090333
      33333333337F7F33333333333309033333333333337F7F333333333333090333
      33333333337F7F33333333333309033333333333FF7F7FFFF333333000090000
      3333333777737777F333333099999990333333373F3333373333333309999903
      333333337F33337F33333333099999033333333373F333733333333330999033
      3333333337F337F3333333333099903333333333373F37333333333333090333
      33333333337F7F33333333333309033333333333337373333333333333303333
      333333333337F333333333333330333333333333333733333333}
    NumGlyphs = 2
  end
  object btnDown: TBitBtn
    Left = 321
    Top = 64
    Width = 25
    Height = 25
    Anchors = [akTop, akRight]
    TabOrder = 5
    OnClick = btnDownClick
    Glyph.Data = {
      76010000424D7601000000000000760000002800000020000000100000000100
      04000000000000010000120B0000120B00001000000000000000000000000000
      800000800000008080008000000080008000808000007F7F7F00BFBFBF000000
      FF0000FF000000FFFF00FF000000FF00FF00FFFF0000FFFFFF00333333303333
      333333333337F33333333333333033333333333333373F333333333333090333
      33333333337F7F33333333333309033333333333337373F33333333330999033
      3333333337F337F33333333330999033333333333733373F3333333309999903
      333333337F33337F33333333099999033333333373333373F333333099999990
      33333337FFFF3FF7F33333300009000033333337777F77773333333333090333
      33333333337F7F33333333333309033333333333337F7F333333333333090333
      33333333337F7F33333333333309033333333333337F7F333333333333090333
      33333333337F7F33333333333300033333333333337773333333}
    NumGlyphs = 2
  end
  object btnSelectFilePath: TButton
    Left = 321
    Top = 144
    Width = 21
    Height = 21
    Anchors = [akTop, akRight]
    Caption = '...'
    TabOrder = 6
    OnClick = btnSelectFilePathClick
  end
  object gbxZoom: TGroupBox
    Left = 8
    Top = 264
    Width = 337
    Height = 41
    Caption = #1052#1072#1089#1096#1090#1072#1073
    TabOrder = 7
    object Label2: TLabel
      Left = 16
      Top = 16
      Width = 21
      Height = 13
      Caption = #1052#1080#1085
    end
    object Label3: TLabel
      Left = 168
      Top = 16
      Width = 27
      Height = 13
      Caption = #1052#1072#1082#1089
    end
    object edtMinZoom: TEdit
      Left = 56
      Top = 13
      Width = 70
      Height = 21
      TabOrder = 0
      OnChange = edtMinZoomChange
    end
    object edtMaxZoom: TEdit
      Left = 208
      Top = 13
      Width = 70
      Height = 21
      TabOrder = 1
      OnChange = edtMaxZoomChange
    end
  end
  object chbVisible: TCheckBox
    Left = 72
    Top = 240
    Width = 105
    Height = 17
    Caption = #1042#1080#1076#1080#1084#1099#1081
    TabOrder = 8
    OnClick = chbVisibleClick
  end
  object edtName: TEdit
    Left = 72
    Top = 208
    Width = 241
    Height = 21
    TabOrder = 9
    OnChange = edtNameChange
  end
  object chbAutoLabels: TCheckBox
    Left = 176
    Top = 240
    Width = 97
    Height = 17
    Caption = #1052#1077#1090#1082#1080
    TabOrder = 10
    OnClick = chbAutoLabelsClick
  end
  object btnReplace: TButton
    Left = 8
    Top = 175
    Width = 75
    Height = 21
    Caption = #1047#1072#1084#1077#1085#1080#1090#1100
    TabOrder = 11
    OnClick = btnReplaceClick
  end
  object btnAdd: TButton
    Left = 96
    Top = 175
    Width = 75
    Height = 21
    Caption = #1044#1086#1073#1072#1074#1080#1090#1100
    TabOrder = 12
    OnClick = btnAddClick
  end
  object btnDelete: TButton
    Left = 184
    Top = 175
    Width = 75
    Height = 21
    Caption = #1059#1076#1072#1083#1080#1090#1100
    TabOrder = 13
    OnClick = btnDeleteClick
  end
  object opd: TOpenDialog
    DefaultExt = 'tab'
    Filter = 'TAB-'#1092#1072#1081#1083#1099' (*.tab)|*.tab|'#1042#1089#1077' '#1092#1072#1081#1083#1099' (*.*)|*.*'
    Left = 32
    Top = 32
  end
end

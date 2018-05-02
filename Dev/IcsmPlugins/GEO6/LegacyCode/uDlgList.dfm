object dlgList: TdlgList
  Left = 628
  Top = 602
  BorderStyle = bsDialog
  Caption = 'Dialog'
  ClientHeight = 342
  ClientWidth = 274
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  OnShow = FormShow
  DesignSize = (
    274
    342)
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 8
    Width = 258
    Height = 289
    Anchors = [akLeft, akTop, akRight, akBottom]
    Shape = bsFrame
  end
  object lblComment: TLabel
    Left = 16
    Top = 268
    Width = 241
    Height = 26
    AutoSize = False
    WordWrap = True
  end
  object btnOk: TButton
    Left = 88
    Top = 308
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 1
    OnClick = btnOkClick
  end
  object btnCancel: TButton
    Left = 176
    Top = 308
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 2
  end
  object lb: TListBox
    Left = 16
    Top = 16
    Width = 242
    Height = 249
    Anchors = [akLeft, akTop, akRight, akBottom]
    ItemHeight = 13
    TabOrder = 0
    OnDblClick = lbDblClick
  end
end

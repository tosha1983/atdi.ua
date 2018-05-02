object frmRelief: TfrmRelief
  Left = 68
  Top = 63
  BorderIcons = [biSystemMenu]
  BorderStyle = bsDialog
  Caption = 'frmRelief'
  ClientHeight = 101
  ClientWidth = 678
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsStayOnTop
  OldCreateOrder = False
  Position = poOwnerFormCenter
  OnDeactivate = FormDeactivate
  PixelsPerInch = 96
  TextHeight = 13
  inline fmProfileView1: TfmProfileView
    Left = 0
    Top = 0
    Width = 678
    Height = 101
    Align = alClient
    AutoScroll = False
    Ctl3D = True
    ParentCtl3D = False
    TabOrder = 0
    DesignSize = (
      678
      101)
    inherited txtDist: TStaticText
      Left = 628
    end
  end
end

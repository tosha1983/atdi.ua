object frmTable36: TfrmTable36
  Left = 254
  Top = 110
  BorderStyle = bsToolWindow
  Caption = 'frmTable36'
  ClientHeight = 509
  ClientWidth = 519
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsStayOnTop
  KeyPreview = True
  OldCreateOrder = False
  Position = poDefault
  ShowHint = True
  Visible = True
  OnClose = FormClose
  OnCreate = FormCreate
  OnDeactivate = FormDeactivate
  OnKeyDown = FormKeyDown
  OnPaint = FormPaint
  DesignSize = (
    519
    509)
  PixelsPerInch = 96
  TextHeight = 13
  object Shape1: TShape
    Left = 120
    Top = 493
    Width = 21
    Height = 9
    Anchors = [akRight, akBottom]
    Brush.Color = clBlue
    Visible = False
  end
  object Label1: TLabel
    Left = 144
    Top = 489
    Width = 62
    Height = 13
    Anchors = [akRight, akBottom]
    Caption = #1056#1086#1079#1088#1072#1093#1086#1074#1072#1085#1110
    Visible = False
  end
  object Shape2: TShape
    Left = 456
    Top = 493
    Width = 21
    Height = 9
    Anchors = [akRight, akBottom]
    Brush.Color = clRed
    Visible = False
  end
  object Label2: TLabel
    Left = 480
    Top = 489
    Width = 26
    Height = 13
    Anchors = [akRight, akBottom]
    Caption = #1042' '#1041#1044
    Visible = False
  end
  object sgTable36: TStringGrid
    Left = 60
    Top = 0
    Width = 60
    Height = 509
    Align = alLeft
    ColCount = 2
    DefaultColWidth = 27
    DefaultRowHeight = 13
    RowCount = 36
    FixedRows = 0
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -5
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing]
    ParentFont = False
    ScrollBars = ssNone
    TabOrder = 0
    OnDrawCell = sgTable36DrawCell
    OnKeyDown = sgTable36KeyDown
    OnKeyPress = sgTable36KeyPress
    OnSetEditText = sgTable36SetEditText
    RowHeights = (
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13)
  end
  object sgTable36old: TStringGrid
    Left = 0
    Top = 0
    Width = 60
    Height = 509
    Align = alLeft
    ColCount = 2
    DefaultColWidth = 27
    DefaultRowHeight = 13
    RowCount = 36
    FixedRows = 0
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -5
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
    ScrollBars = ssNone
    TabOrder = 1
    Visible = False
    OnDrawCell = sgTable36DrawCell
    OnSetEditText = sgTable36SetEditText
    RowHeights = (
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      12
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13
      13)
  end
  object btnSaveNew: TButton
    Left = 443
    Top = 8
    Width = 73
    Height = 25
    Hint = #1055#1110#1076#1089#1090#1072#1074#1080#1090#1080' '#1088#1086#1079#1088#1072#1093#1086#1074#1072#1085#1110' '#1079#1085#1072#1095#1077#1085#1085#1103' '#1074' '#1041#1044
    Anchors = [akTop, akRight]
    Caption = #1055#1110#1076#1089#1090#1072#1074#1080#1090#1080
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clRed
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
    ParentShowHint = False
    ShowHint = True
    TabOrder = 2
    Visible = False
    OnClick = btnSaveNewClick
  end
end

object ZDialog: TZDialog
  Left = 201
  Top = 151
  BorderStyle = bsDialog
  Caption = 'ZDialog'
  ClientHeight = 288
  ClientWidth = 371
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  OnActivate = FormActivate
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 184
    Top = 56
    Width = 84
    Height = 13
    Caption = 'Assigned channel'
    Visible = False
  end
  object Label2: TLabel
    Left = 184
    Top = 88
    Width = 80
    Height = 13
    Caption = 'Planned channel'
    Visible = False
  end
  object Label3: TLabel
    Left = 168
    Top = 120
    Width = 71
    Height = 13
    Caption = 'CoChannel dist'
  end
  object Label4: TLabel
    Left = 297
    Top = 121
    Width = 14
    Height = 13
    Caption = 'km'
  end
  object Memo1: TMemo
    Left = 8
    Top = 56
    Width = 151
    Height = 193
    ScrollBars = ssVertical
    TabOrder = 0
    WordWrap = False
  end
  object Edit1: TEdit
    Left = 280
    Top = 56
    Width = 25
    Height = 21
    TabOrder = 1
    Text = '0'
    Visible = False
  end
  object Edit2: TEdit
    Left = 280
    Top = 88
    Width = 25
    Height = 21
    TabOrder = 2
    Text = '0'
    Visible = False
  end
  object Button1: TButton
    Left = 200
    Top = 256
    Width = 75
    Height = 25
    Caption = 'Ok'
    Default = True
    TabOrder = 3
    OnClick = Button1Click
  end
  object Button2: TButton
    Left = 288
    Top = 256
    Width = 75
    Height = 25
    Caption = 'Cancel'
    TabOrder = 4
    OnClick = Button2Click
  end
  object ListBox1: TListBox
    Left = 317
    Top = 56
    Width = 50
    Height = 193
    Hint = 'cClick me to add channel'
    ItemHeight = 13
    TabOrder = 5
    OnKeyDown = ListBox1KeyDown
  end
  object Edit3: TEdit
    Left = 8
    Top = 8
    Width = 70
    Height = 21
    TabOrder = 6
    Text = '30'
  end
  object Edit4: TEdit
    Left = 88
    Top = 8
    Width = 70
    Height = 21
    TabOrder = 7
    Text = '50'
  end
  object Button3: TButton
    Left = 72
    Top = 32
    Width = 25
    Height = 17
    Hint = 'Click me to add point'
    Caption = 'v'
    ParentShowHint = False
    ShowHint = True
    TabOrder = 8
    OnClick = Button3Click
  end
  object Edit5: TEdit
    Left = 317
    Top = 8
    Width = 50
    Height = 21
    TabOrder = 9
    Text = '21'
  end
  object Button4: TButton
    Left = 328
    Top = 32
    Width = 25
    Height = 17
    Caption = 'v'
    ParentShowHint = False
    ShowHint = True
    TabOrder = 10
    OnClick = Button4Click
  end
  object Edit6: TEdit
    Left = 176
    Top = 8
    Width = 121
    Height = 21
    TabOrder = 11
  end
  object Edit7: TEdit
    Left = 248
    Top = 120
    Width = 46
    Height = 21
    TabOrder = 12
    Text = '100'
  end
  object CheckBox1: TCheckBox
    Left = 168
    Top = 152
    Width = 145
    Height = 17
    Caption = 'Available for planning'
    TabOrder = 13
  end
end

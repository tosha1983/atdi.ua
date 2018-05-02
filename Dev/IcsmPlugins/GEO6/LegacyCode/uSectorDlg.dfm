object dlgSector: TdlgSector
  Left = 435
  Top = 193
  BorderStyle = bsDialog
  Caption = #1042#1080#1073#1110#1088' '#1089#1077#1082#1090#1086#1088#1091
  ClientHeight = 205
  ClientWidth = 386
  Color = clBtnFace
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  OnCreate = FormCreate
  OnShow = FormShow
  DesignSize = (
    386
    205)
  PixelsPerInch = 96
  TextHeight = 13
  object OKBtn: TButton
    Left = 200
    Top = 175
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object CancelBtn: TButton
    Left = 288
    Top = 175
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = #1042#1110#1076#1084#1110#1085#1072
    ModalResult = 2
    TabOrder = 1
  end
  object panSector: TPanel
    Left = 0
    Top = 0
    Width = 386
    Height = 168
    Align = alTop
    Anchors = [akLeft, akTop, akRight, akBottom]
    TabOrder = 2
    object Label1: TLabel
      Left = 16
      Top = 8
      Width = 226
      Height = 13
      Caption = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1088#1086#1079#1087#1086#1076#1110#1083' '#1085#1072#1087#1088#1091#1078#1077#1085#1086#1089#1090#1110' '#1087#1086#1083#1103' '#1074#1110#1076
    end
    object lblTxName: TLabel
      Left = 16
      Top = 24
      Width = 50
      Height = 13
      Caption = 'lblTxName'
    end
    object Label8: TLabel
      Left = 150
      Top = 132
      Width = 131
      Height = 26
      Alignment = taRightJustify
      Caption = #1052#1110#1085#1110#1084#1072#1083#1100#1085#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1076#1083#1103' '#1074#1110#1076#1086#1073#1088#1072#1078#1077#1085#1085#1103' ('#1045#1084#1110#1085')'
      WordWrap = True
    end
    object Label9: TLabel
      Left = 344
      Top = 140
      Width = 31
      Height = 13
      Caption = #1076#1041#1082#1042#1090
    end
    object GroupBox1: TGroupBox
      Left = 8
      Top = 39
      Width = 369
      Height = 89
      Caption = #1042' '#1089#1077#1082#1090#1086#1088#1110
      TabOrder = 0
      DesignSize = (
        369
        89)
      object Label2: TLabel
        Left = 8
        Top = 25
        Width = 81
        Height = 13
        Anchors = [akLeft, akBottom]
        Caption = #1072#1079#1110#1084#1091#1090', '#1075#1088#1072#1076', '#1074#1110#1076
      end
      object Label3: TLabel
        Left = 184
        Top = 25
        Width = 12
        Height = 13
        Anchors = [akLeft, akBottom]
        Caption = #1076#1086
      end
      object Label5: TLabel
        Left = 184
        Top = 57
        Width = 12
        Height = 13
        Anchors = [akLeft, akBottom]
        Caption = #1076#1086
      end
      object Label4: TLabel
        Left = 8
        Top = 57
        Width = 74
        Height = 13
        Anchors = [akLeft, akBottom]
        Caption = #1088#1072#1076#1110#1091#1089', '#1082#1084',  '#1074#1110#1076
      end
      object Label6: TLabel
        Left = 280
        Top = 25
        Width = 19
        Height = 13
        Anchors = [akLeft, akBottom]
        Caption = #1096#1072#1075
      end
      object Label7: TLabel
        Left = 280
        Top = 57
        Width = 19
        Height = 13
        Anchors = [akLeft, akBottom]
        Caption = #1096#1072#1075
      end
      object cseAzBeg: TCSpinEdit
        Left = 104
        Top = 22
        Width = 65
        Height = 22
        Anchors = [akLeft, akBottom]
        MaxValue = 360
        MinValue = -180
        TabOrder = 0
      end
      object cseAzEnd: TCSpinEdit
        Left = 200
        Top = 22
        Width = 65
        Height = 22
        Anchors = [akLeft, akBottom]
        MaxValue = 360
        TabOrder = 1
        Value = 360
      end
      object cseRadEnd: TCSpinEdit
        Left = 200
        Top = 54
        Width = 65
        Height = 22
        Anchors = [akLeft, akBottom]
        MaxValue = 1000
        TabOrder = 4
      end
      object cseRadBeg: TCSpinEdit
        Left = 104
        Top = 54
        Width = 65
        Height = 22
        Anchors = [akLeft, akBottom]
        MaxValue = 1000
        TabOrder = 3
      end
      object edtRadStep: TEdit
        Left = 304
        Top = 54
        Width = 49
        Height = 21
        Anchors = [akLeft, akBottom]
        TabOrder = 5
      end
      object edtAzStep: TEdit
        Left = 304
        Top = 22
        Width = 49
        Height = 21
        Anchors = [akLeft, akBottom]
        TabOrder = 2
      end
    end
    object chbGradient: TCheckBox
      Left = 8
      Top = 136
      Width = 129
      Height = 17
      Caption = #1043#1088#1072#1076#1110#1108#1085#1090#1085#1077' '#1079#1072#1083#1080#1090#1090#1103
      Checked = True
      State = cbChecked
      TabOrder = 1
    end
    object edtEmin: TEdit
      Left = 288
      Top = 135
      Width = 49
      Height = 21
      TabOrder = 2
    end
  end
end

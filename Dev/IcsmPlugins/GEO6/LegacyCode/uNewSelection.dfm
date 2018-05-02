object dlgNewSelection: TdlgNewSelection
  Left = 266
  Top = 232
  BorderIcons = [biSystemMenu]
  BorderStyle = bsDialog
  Caption = #1053#1086#1074#1072' '#1074#1080#1073#1110#1088#1082#1072
  ClientHeight = 360
  ClientWidth = 524
  Color = clBtnFace
  ParentFont = True
  FormStyle = fsStayOnTop
  OldCreateOrder = True
  Position = poMainFormCenter
  Visible = True
  OnClose = FormClose
  OnShow = FormShow
  DesignSize = (
    524
    360)
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 8
    Top = 337
    Width = 55
    Height = 13
    Anchors = [akLeft, akBottom]
    Caption = #1055#1088#1086#1094#1077#1076#1091#1088#1072
    Visible = False
  end
  object btnOk: TButton
    Left = 331
    Top = 330
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1042#1080#1073#1088#1072#1090#1080
    Default = True
    ModalResult = 1
    TabOrder = 1
    OnClick = btnOkClick
  end
  object btnCancel: TButton
    Left = 419
    Top = 330
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = #1042#1110#1076#1084#1110#1085#1072
    ModalResult = 2
    TabOrder = 2
    OnClick = btnCancelClick
  end
  object panParams: TPanel
    Left = 0
    Top = 0
    Width = 524
    Height = 325
    Align = alTop
    Anchors = [akLeft, akTop, akRight, akBottom]
    BevelOuter = bvNone
    TabOrder = 0
    DesignSize = (
      524
      325)
    object lblTxName: TLabel
      Left = 24
      Top = 4
      Width = 480
      Height = 35
      Anchors = [akLeft, akTop, akRight]
      AutoSize = False
      Caption = 'lblTxName'
      WordWrap = True
    end
    object pcSelectionCriteria: TPageControl
      Left = 1
      Top = 72
      Width = 522
      Height = 252
      ActivePage = tshCommon
      Anchors = [akLeft, akTop, akRight, akBottom]
      TabIndex = 0
      TabOrder = 4
      object tshCommon: TTabSheet
        Caption = #1047#1072#1075#1072#1083#1100#1085#1110
        DesignSize = (
          514
          224)
        object chbImage: TCheckBox
          Left = 272
          Top = 172
          Width = 185
          Height = 17
          Anchors = [akLeft, akBottom]
          Caption = #1042#1080#1073#1080#1088#1072#1090#1080' '#1076#1079#1077#1088#1082#1072#1083#1100#1085#1110' '#1082#1072#1085#1072#1083#1080
          Checked = True
          State = cbChecked
          TabOrder = 5
          Visible = False
        end
        object chbAdjanced: TCheckBox
          Left = 272
          Top = 156
          Width = 201
          Height = 17
          Anchors = [akLeft, akBottom]
          Caption = #1042#1080#1073#1080#1088#1072#1090#1080' '#1089#1091#1089#1110#1076#1085#1110' '#1082#1072#1085#1072#1083#1080
          Checked = True
          State = cbChecked
          TabOrder = 4
          Visible = False
        end
        object gbxCond: TGroupBox
          Left = 8
          Top = 88
          Width = 241
          Height = 105
          Caption = #1054#1073#1083#1110#1082#1086#1074#1110' '#1089#1090#1072#1085#1080
          TabOrder = 2
          DesignSize = (
            241
            105)
          object lbxCond: TListBox
            Left = 8
            Top = 16
            Width = 193
            Height = 81
            Anchors = [akLeft, akTop, akRight, akBottom]
            ItemHeight = 13
            TabOrder = 0
          end
          object btnCondAdd: TButton
            Left = 208
            Top = 16
            Width = 20
            Height = 20
            Hint = #1044#1086#1076#1072#1090#1080
            Anchors = [akTop, akRight]
            Caption = '...'
            TabOrder = 1
            OnClick = btnCondAddClick
          end
          object btnCondRm: TButton
            Left = 208
            Top = 44
            Width = 20
            Height = 20
            Hint = #1042#1080#1076#1072#1083#1080#1090#1080
            Anchors = [akTop, akRight]
            Caption = 'X'
            TabOrder = 2
            OnClick = btnCondRmClick
          end
          object btnCondRmAll: TButton
            Left = 208
            Top = 72
            Width = 20
            Height = 20
            Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1074#1089#1110
            Anchors = [akTop, akRight]
            Caption = 'XX'
            TabOrder = 3
            OnClick = btnCondRmAllClick
          end
        end
        object gbxRegions: TGroupBox
          Left = 264
          Top = 88
          Width = 241
          Height = 105
          Caption = #1056#1077#1075#1110#1086#1085#1080
          TabOrder = 3
          DesignSize = (
            241
            105)
          object lbxRegions: TListBox
            Left = 8
            Top = 16
            Width = 193
            Height = 81
            Anchors = [akLeft, akTop, akRight, akBottom]
            ItemHeight = 13
            TabOrder = 0
          end
          object btnRegionsAdd: TButton
            Left = 208
            Top = 16
            Width = 20
            Height = 20
            Hint = #1044#1086#1076#1072#1090#1080
            Anchors = [akTop, akRight]
            Caption = '...'
            TabOrder = 1
            OnClick = btnRegionsAddClick
          end
          object btnRegionsRm: TButton
            Left = 208
            Top = 44
            Width = 20
            Height = 20
            Hint = #1042#1080#1076#1072#1083#1080#1090#1080
            Anchors = [akTop, akRight]
            Caption = 'X'
            TabOrder = 2
            OnClick = btnRegionsRmClick
          end
          object btnRegionsRmAll: TButton
            Left = 208
            Top = 72
            Width = 20
            Height = 20
            Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1074#1089#1110
            Anchors = [akTop, akRight]
            Caption = 'XX'
            TabOrder = 3
            OnClick = btnRegionsRmAllClick
          end
        end
        object chbOnlyRoot: TCheckBox
          Left = 272
          Top = 199
          Width = 209
          Height = 17
          Anchors = [akLeft, akBottom]
          Caption = #1042#1080#1073#1080#1088#1072#1090#1080' '#1090#1110#1083#1100#1082#1110' '#1082#1086#1088#1085#1100#1086#1074#1110' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110
          TabOrder = 6
        end
        object panChBlockGrid: TPanel
          Left = 8
          Top = 148
          Width = 241
          Height = 65
          Anchors = [akLeft, akBottom]
          TabOrder = 8
          Visible = False
          DesignSize = (
            241
            65)
          object lblChFrom: TLabel
            Left = 8
            Top = 35
            Width = 46
            Height = 13
            Anchors = [akLeft, akBottom]
            Caption = #1050#1072#1085#1072#1083#1080' '#1079
          end
          object lblChTo: TLabel
            Left = 144
            Top = 35
            Width = 12
            Height = 13
            Anchors = [akLeft, akBottom]
            Caption = #1087#1086
          end
          object lblChannelGrid: TLabel
            Left = 8
            Top = 11
            Width = 67
            Height = 13
            Anchors = [akLeft, akBottom]
            Caption = #1057#1110#1090#1082#1072' '#1082#1072#1085#1072#1083#1110#1074
            Visible = False
          end
          object cbxChFrom: TComboBox
            Left = 64
            Top = 32
            Width = 70
            Height = 21
            Style = csDropDownList
            Anchors = [akLeft, akBottom]
            DropDownCount = 20
            ItemHeight = 13
            TabOrder = 0
          end
          object cbxChTo: TComboBox
            Left = 163
            Top = 32
            Width = 70
            Height = 21
            Style = csDropDownList
            Anchors = [akLeft, akBottom]
            DropDownCount = 20
            ItemHeight = 13
            TabOrder = 1
          end
          object cbxChannelGrid: TComboBox
            Left = 80
            Top = 8
            Width = 153
            Height = 21
            Style = csDropDownList
            Anchors = [akLeft, akBottom]
            DropDownCount = 20
            ItemHeight = 13
            TabOrder = 2
            Visible = False
            OnChange = cbxChannelGridChange
          end
        end
        object panFreqGrid: TPanel
          Left = 8
          Top = 148
          Width = 241
          Height = 69
          Anchors = [akLeft, akBottom]
          TabOrder = 7
          Visible = False
          DesignSize = (
            241
            69)
          object lblFmDiapason: TLabel
            Left = 6
            Top = 15
            Width = 58
            Height = 13
            Anchors = [akLeft, akBottom]
            Caption = #1057#1084#1091#1075#1072', '#1052#1043#1094
            FocusControl = edtDiapason
          end
          object edtDiapason: TEdit
            Left = 68
            Top = 12
            Width = 93
            Height = 21
            Anchors = [akLeft, akBottom]
            TabOrder = 0
            Text = 'edtDiapason'
          end
          object rgrGrid: TRadioGroup
            Left = 1
            Top = 33
            Width = 160
            Height = 34
            Anchors = [akLeft, akTop, akRight, akBottom]
            Caption = ' '#1064#1072#1075' '#1089#1110#1090#1082#1080' '#1095#1072#1089#1090#1086#1090', kHz '
            Columns = 2
            Items.Strings = (
              '30 '#1082#1043#1094
              '100 '#1082#1043#1094)
            TabOrder = 1
            OnClick = rgrGridClick
          end
          object rgBand: TRadioGroup
            Left = 168
            Top = 8
            Width = 69
            Height = 59
            Caption = ' '#1044'i'#1072#1087#1072#1079#1086#1085' '
            ItemIndex = 0
            Items.Strings = (
              #1044#1061
              #1057#1061)
            TabOrder = 2
            OnClick = rgBandClick
          end
        end
        object chbSelectBrIfic: TCheckBox
          Left = 16
          Top = 8
          Width = 233
          Height = 17
          Caption = #1042#1080#1073#1080#1088#1072#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1080' '#1079' '#8470' '#1088#1077#1075#1110#1086#1085#1072' '#39'xxBR'#39
          TabOrder = 0
        end
        object lbDbSection: TCheckListBox
          Left = 272
          Top = 8
          Width = 193
          Height = 77
          ItemHeight = 13
          TabOrder = 1
        end
      end
      object tshAddition: TTabSheet
        Caption = #1044#1086#1076#1072#1090#1082#1086#1074#1086
        ImageIndex = 1
        inline fmWhereCriteria1: TfmWhereCriteria
          Left = 0
          Top = 0
          Width = 514
          Height = 224
          Align = alClient
          TabOrder = 0
          inherited tvwCriteria: TTreeView
            Width = 514
            Height = 224
          end
        end
      end
    end
    object edtLon: TEdit
      Left = 16
      Top = 44
      Width = 97
      Height = 21
      TabOrder = 0
      Text = '0'
      OnExit = edtLonExit
    end
    object edtLat: TEdit
      Left = 136
      Top = 44
      Width = 97
      Height = 21
      TabOrder = 1
      Text = '0'
      OnExit = edtLatExit
    end
    object chbMaxRadius: TCheckBox
      Left = 269
      Top = 44
      Width = 148
      Height = 17
      Caption = #1052#1072#1082#1089#1110#1084#1072#1083#1100#1085#1080#1081' '#1088#1072#1076#1110#1091#1089', '#1082#1084
      Checked = True
      State = cbChecked
      TabOrder = 2
      OnClick = chbMaxRadiusClick
    end
    object edtMaxRadius: TNumericEdit
      Left = 424
      Top = 43
      Width = 73
      Height = 21
      TabOrder = 3
      Text = '500'
      OnExit = edtMaxRadiusExit
      OldValue = '500'
    end
  end
  object cbxProcName: TComboBox
    Left = 72
    Top = 334
    Width = 177
    Height = 21
    Style = csDropDownList
    Anchors = [akLeft, akBottom]
    ItemHeight = 13
    ItemIndex = 3
    TabOrder = 3
    Text = 'SP_TX_SELECTION3'
    Visible = False
    Items.Strings = (
      'SP_CREATE_SELECTION'
      'SP_TX_SELECTION'
      'SP_TX_SELECTION2'
      'SP_TX_SELECTION3')
  end
  object sqlGetSelection: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select OUT_TX_ID, OUT_DISTANCE  from SP_CREATE_SELECTION ('
      '    :ID,'
      '    :TX_ID,'
      '    :RADIUS,'
      '    :LON,'
      '    :LAT,'
      '    :USE_CONDITIONS,'
      '    :USE_AREAS,'
      '    :USE_ADJANCED,'
      '    :USE_IMAGE,'
      '    :ONLY_ROOT,'
      '    :CARRIER'
      ')')
    Transaction = dmMain.trMain
    Left = 328
    Top = 16
  end
  object sqlAddState: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select CODE, NAME from ACCOUNTCONDITION'
      'where ID = :ID')
    Transaction = dmMain.trMain
    Left = 144
    Top = 168
  end
  object sqlAddRegion: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select NAME from AREA'
      'where ID = :ID ')
    Transaction = dmMain.trMain
    Left = 320
    Top = 168
  end
  object sqlDbSection: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, SECTIONNAME from DATABASESECTION')
    Transaction = dmMain.trMain
    Left = 320
    Top = 136
  end
end

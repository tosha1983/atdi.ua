object frmTxBase: TfrmTxBase
  Left = 593
  Top = 239
  BorderIcons = [biSystemMenu]
  BorderStyle = bsSingle
  Caption = 'frmTxBase'
  ClientHeight = 565
  ClientWidth = 780
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIChild
  OldCreateOrder = False
  Position = poDefaultPosOnly
  ShowHint = True
  Visible = True
  OnClose = FormClose
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object pnlForAllTop: TPanel
    Left = 0
    Top = 0
    Width = 780
    Height = 54
    Align = alTop
    BevelOuter = bvNone
    TabOrder = 0
    DesignSize = (
      780
      54)
    object lblTxType: TLabel
      Left = 12
      Top = 24
      Width = 37
      Height = 22
      Alignment = taCenter
      Caption = 'ALL'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clGreen
      Font.Height = -19
      Font.Name = 'Arial'
      Font.Style = [fsBold]
      ParentFont = False
    end
    object lblCode: TLabel
      Left = 64
      Top = 9
      Width = 19
      Height = 13
      Caption = #1050#1086#1076
      Visible = False
    end
    object lblNumber: TLabel
      Left = 58
      Top = 29
      Width = 11
      Height = 13
      Caption = #8470
    end
    object lblRegion: TLabel
      Left = 272
      Top = 7
      Width = 43
      Height = 13
      Caption = #1054#1073#1083#1072#1089#1090#1100
    end
    object lblPoint: TLabel
      Left = 448
      Top = 8
      Width = 51
      Height = 13
      Caption = #1053#1072#1089'.'#1087#1091#1085#1082#1090
    end
    object lblHPoint: TLabel
      Left = 628
      Top = 7
      Width = 53
      Height = 13
      Caption = 'H '#1084#1110#1089#1094#1103', '#1084
    end
    object lblLat: TLabel
      Left = 145
      Top = 29
      Width = 38
      Height = 13
      Caption = #1064#1080#1088#1086#1090#1072
    end
    object lblStand: TLabel
      Left = 280
      Top = 29
      Width = 32
      Height = 13
      Caption = #1054#1087#1086#1088#1072
    end
    object lblNumRegion: TLabel
      Left = 11
      Top = 7
      Width = 58
      Height = 13
      Caption = #1050#1086#1076' '#1088#1077#1075#1110#1086#1085#1091
    end
    object lblEditing: TLabel
      Left = 628
      Top = 24
      Width = 86
      Height = 26
      Caption = #1061#1072#1088#1072#1082#1090#1077#1088#1080#1089#1090#1080#1082#1080' '#1086#1073#39#1108#1082#1090#1091' '#1079#1084#1110#1085#1077#1085#1110
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clRed
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentFont = False
      Visible = False
      WordWrap = True
    end
    object Label5: TLabel
      Left = 460
      Top = 31
      Width = 37
      Height = 13
      Caption = #1040#1076#1088#1077#1089#1072
    end
    object lblLong: TLabel
      Left = 140
      Top = 7
      Width = 43
      Height = 13
      Caption = #1044#1086#1074#1075#1086#1090#1072
    end
    object edtID: TDBEdit
      Left = 724
      Top = 4
      Width = 51
      Height = 21
      Anchors = [akTop, akRight]
      AutoSize = False
      BiDiMode = bdLeftToRight
      BorderStyle = bsNone
      DataField = 'ID'
      DataSource = dsStantionsBase
      ParentBiDiMode = False
      ParentColor = True
      ReadOnly = True
      TabOrder = 10
    end
    object edtCityName: TDBEdit
      Left = 500
      Top = 4
      Width = 121
      Height = 21
      DataField = 'CITY_NAME'
      DataSource = dsStand
      ReadOnly = True
      TabOrder = 7
    end
    object edtHPoint: TDBEdit
      Left = 684
      Top = 4
      Width = 37
      Height = 21
      DataField = 'HEIGHT_SEA'
      DataSource = dsStand
      ReadOnly = True
      TabOrder = 9
    end
    object edtLong: TEdit
      Left = 188
      Top = 4
      Width = 69
      Height = 21
      TabOrder = 2
      Text = 'edtLong'
      OnExit = edtLongExit
    end
    object edtLat: TEdit
      Left = 188
      Top = 26
      Width = 69
      Height = 21
      TabOrder = 3
      Text = 'edtLat'
      OnExit = edtLatExit
    end
    object edtStand: TDBEdit
      Left = 316
      Top = 26
      Width = 121
      Height = 21
      DataField = 'NAMESITE'
      DataSource = dsStand
      ReadOnly = True
      TabOrder = 5
    end
    object btnStand: TButton
      Left = 436
      Top = 27
      Width = 19
      Height = 20
      Caption = '...'
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = [fsBold]
      ParentFont = False
      TabOrder = 6
      OnClick = btnStandClick
    end
    object edtAreaName: TDBEdit
      Left = 316
      Top = 4
      Width = 121
      Height = 21
      DataField = 'AREA_NAME'
      DataSource = dsStand
      ReadOnly = True
      TabOrder = 4
    end
    object edtCode: TDBEdit
      Left = 72
      Top = 26
      Width = 59
      Height = 21
      DataField = 'ADMINISTRATIONID'
      DataSource = dsStantionsBase
      ReadOnly = True
      TabOrder = 1
    end
    object edtNumRegion: TDBEdit
      Left = 72
      Top = 4
      Width = 59
      Height = 21
      DataField = 'NUMREG'
      DataSource = dsStand
      ReadOnly = True
      TabOrder = 0
    end
    object edtAdress: TDBEdit
      Left = 500
      Top = 28
      Width = 121
      Height = 21
      DataField = 'FULL_ADDR'
      DataSource = dsStand
      ReadOnly = True
      TabOrder = 8
    end
  end
  object pcData: TPageControl
    Left = 0
    Top = 54
    Width = 780
    Height = 475
    ActivePage = tshCommon
    Align = alClient
    Anchors = [akLeft, akRight]
    TabIndex = 0
    TabOrder = 1
    TabPosition = tpBottom
    OnChange = pcDataChange
    object tshCommon: TTabSheet
      Caption = #1058#1077#1093#1085#1110#1095#1085#1110' '#1093#1072#1088#1072#1082#1090#1077#1088#1080#1089#1090#1080#1082#1080
      object pnlMaster: TPanel
        Left = 0
        Top = 283
        Width = 772
        Height = 166
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 0
        object lblTRC: TLabel
          Left = 12
          Top = 39
          Width = 21
          Height = 13
          Caption = #1058#1056#1050
        end
        object lblMaster: TLabel
          Left = 495
          Top = 39
          Width = 49
          Height = 13
          Caption = #1054#1087#1077#1088#1072#1090#1086#1088
        end
        object lblProgram: TLabel
          Left = 239
          Top = 39
          Width = 51
          Height = 13
          Caption = #1055#1088#1086#1075#1088#1072#1084#1072
        end
        object lblDateEnter: TLabel
          Left = 6
          Top = 68
          Width = 24
          Height = 13
          Caption = #1042#1074#1110#1076':'
        end
        object lblUserEnter: TDBText
          Left = 150
          Top = 68
          Width = 57
          Height = 13
          DataField = 'USER_NAME'
          DataSource = dsStantionsBase
        end
        object lblEdit: TLabel
          Left = 256
          Top = 68
          Width = 32
          Height = 13
          Caption = #1047#1084#1110#1085#1072':'
        end
        object lblUserEdit: TDBText
          Left = 406
          Top = 68
          Width = 50
          Height = 13
          DataField = 'USER_NAME'
          DataSource = dsStantionsBase
        end
        object lblReestr: TLabel
          Left = 492
          Top = 16
          Width = 53
          Height = 13
          Caption = #1056#1077#1108#1089#1090#1088' '#8470':'
        end
        object lblDateChange: TDBText
          Left = 294
          Top = 68
          Width = 70
          Height = 13
          DataField = 'DATECHANGE'
          DataSource = dsStantionsBase
        end
        object lblDateCreate: TDBText
          Left = 38
          Top = 68
          Width = 64
          Height = 13
          DataField = 'DATECREATE'
          DataSource = dsStantionsBase
        end
        object lblClassWave: TLabel
          Left = 608
          Top = 96
          Width = 57
          Height = 13
          Caption = #1050#1083#1072#1089' '#1093#1074#1080#1083#1100
          Visible = False
        end
        object lblTimeTransmit: TLabel
          Left = 612
          Top = 120
          Width = 50
          Height = 13
          Caption = #1063#1072#1089' '#1077#1092#1110#1088#1091
          Visible = False
        end
        object lblTypeRegistry: TDBText
          Left = 633
          Top = 16
          Width = 65
          Height = 13
          DataField = 'TYPEREGISTRY'
          DataSource = dsStantionsBase
        end
        object lblOut: TLabel
          Left = 4
          Top = 16
          Width = 29
          Height = 13
          Caption = #1052#1077#1078#1076
        end
        object lblIn: TLabel
          Left = 261
          Top = 16
          Width = 29
          Height = 13
          Caption = #1042#1085#1091#1090#1088
        end
        object edtTRK: TDBEdit
          Left = 36
          Top = 36
          Width = 173
          Height = 21
          DataField = 'TRK_NAME'
          DataSource = dsStantionsBase
          ReadOnly = True
          TabOrder = 4
        end
        object btnTRK: TButton
          Left = 209
          Top = 36
          Width = 19
          Height = 20
          Caption = '...'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -11
          Font.Name = 'MS Sans Serif'
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 5
          OnClick = btnTRKClick
        end
        object edtOperator: TDBEdit
          Left = 547
          Top = 36
          Width = 173
          Height = 21
          DataField = 'OPERATOR_NAME'
          DataSource = dsStantionsBase
          ReadOnly = True
          TabOrder = 7
        end
        object btnOperator: TButton
          Left = 720
          Top = 36
          Width = 19
          Height = 20
          Caption = '...'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -11
          Font.Name = 'MS Sans Serif'
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 8
          OnClick = btnOperatorClick
        end
        object cbProgramm: TDBComboBox
          Left = 293
          Top = 36
          Width = 188
          Height = 21
          DataField = 'NAMEPROGRAMM'
          DataSource = dsStantionsBase
          ItemHeight = 13
          TabOrder = 6
        end
        object edtTimeTransmit: TDBEdit
          Left = 668
          Top = 116
          Width = 105
          Height = 21
          DataField = 'TIMETRANSMIT'
          DataSource = dsStantionsBase
          TabOrder = 9
          Visible = False
        end
        object edtClassWave: TDBEdit
          Left = 668
          Top = 92
          Width = 105
          Height = 21
          DataField = 'CLASSWAVE'
          DataSource = dsStantionsBase
          ReadOnly = True
          TabOrder = 10
          Visible = False
        end
        object cbStateOut: TDBLookupComboBox
          Left = 88
          Top = 12
          Width = 141
          Height = 21
          DataField = 'ACCOUNTCONDITION_OUT'
          DataSource = dsStantionsBase
          DropDownRows = 15
          KeyField = 'ID'
          ListField = 'NAME'
          ListSource = dsAccCondNameOut
          TabOrder = 1
        end
        object cbStateIn: TDBLookupComboBox
          Left = 344
          Top = 12
          Width = 137
          Height = 21
          DataField = 'ACCOUNTCONDITION_IN'
          DataSource = dsStantionsBase
          DropDownRows = 15
          KeyField = 'ID'
          ListField = 'NAME'
          ListSource = dsAccCondNameIn
          TabOrder = 3
        end
        object pcRemark: TPageControl
          Left = 0
          Top = 82
          Width = 772
          Height = 84
          ActivePage = tshNote
          Align = alBottom
          TabIndex = 0
          TabOrder = 11
          object tshNote: TTabSheet
            Caption = #1055#1088#1080#1084#1110#1090#1082#1080
            object DBMemo1: TDBMemo
              Left = 0
              Top = 0
              Width = 764
              Height = 56
              Align = alClient
              DataField = 'REMARKS'
              DataSource = dsStantionsBase
              TabOrder = 0
            end
          end
          object tshPrivateNote: TTabSheet
            Caption = #1057#1083#1091#1078#1073#1086#1074#1072' '#1110#1085#1092#1086#1088#1084#1072#1094#1110#1103
            ImageIndex = 1
            object DBMemo2: TDBMemo
              Left = 0
              Top = 0
              Width = 853
              Height = 39
              Align = alClient
              DataField = 'REMARKS_ADD'
              DataSource = dsStantionsBase
              TabOrder = 0
            end
          end
        end
        object cbStateInCode: TDBLookupComboBox
          Left = 292
          Top = 12
          Width = 53
          Height = 21
          DataField = 'ACCOUNTCONDITION_IN'
          DataSource = dsStantionsBase
          DropDownRows = 15
          KeyField = 'ID'
          ListField = 'CODE'
          ListSource = dsAccCondNameIn
          TabOrder = 2
        end
        object cbStateOutCode: TDBLookupComboBox
          Left = 36
          Top = 12
          Width = 53
          Height = 21
          DataField = 'ACCOUNTCONDITION_OUT'
          DataSource = dsStantionsBase
          DropDownRows = 15
          KeyField = 'ID'
          ListField = 'CODE'
          ListSource = dsAccCondNameOut
          TabOrder = 0
        end
        object dbedNumRegistry: TDBEdit
          Left = 547
          Top = 12
          Width = 74
          Height = 21
          DataField = 'NUMREGISTRY'
          DataSource = dsStantionsBase
          TabOrder = 12
        end
      end
      object gbxCoordination: TGroupBox
        Left = 0
        Top = 244
        Width = 772
        Height = 39
        Align = alBottom
        Caption = ' '#1059#1079#1075#1086#1076#1078#1077#1085#1085#1103' '#1090#1072' '#1082#1086#1086#1088#1076#1080#1085#1072#1094#1110#1103' '
        Color = clBtnFace
        ParentColor = False
        TabOrder = 1
      end
    end
    object tshCoordination: TTabSheet
      Caption = #1044#1086#1082#1091#1084#1077#1085#1090#1080
      ImageIndex = 1
      object gbOrganizations: TGroupBox
        Left = 137
        Top = 0
        Width = 468
        Height = 244
        Align = alLeft
        Caption = ' '#1054#1088#1075#1072#1085#1080#1079#1072#1094#1110#1111' '
        TabOrder = 0
        object dbgOrganizations: TDBGrid
          Left = 2
          Top = 15
          Width = 464
          Height = 227
          Align = alClient
          DataSource = dsTelecomOrg
          TabOrder = 0
          TitleFont.Charset = DEFAULT_CHARSET
          TitleFont.Color = clWindowText
          TitleFont.Height = -11
          TitleFont.Name = 'MS Sans Serif'
          TitleFont.Style = []
        end
      end
      object gbDocuments: TGroupBox
        Left = 0
        Top = 244
        Width = 772
        Height = 205
        Align = alBottom
        Caption = #1044#1086#1082#1091#1084#1077#1085#1090#1080
        TabOrder = 1
        object dbgDocuments: TDBGrid
          Left = 2
          Top = 15
          Width = 768
          Height = 188
          Align = alClient
          DataSource = dsDocuments
          ReadOnly = True
          TabOrder = 0
          TitleFont.Charset = DEFAULT_CHARSET
          TitleFont.Color = clWindowText
          TitleFont.Height = -11
          TitleFont.Name = 'MS Sans Serif'
          TitleFont.Style = []
          OnDblClick = dbgDocumentsDblClick
          OnEditButtonClick = dbgDocumentsEditButtonClick
          Columns = <
            item
              Alignment = taCenter
              Expanded = False
              FieldName = 'TYPELETTER'
              PickList.Strings = (
                #1042#1093
                #1042#1080#1093)
              Width = 38
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'AC_NAME'
              Width = 81
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'CREATEDATEIN'
              Width = 59
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'CREATEDATEOUT'
              Width = 58
              Visible = True
            end
            item
              Alignment = taLeftJustify
              Expanded = False
              FieldName = 'NUMIN'
              Width = 68
              Visible = True
            end
            item
              Alignment = taLeftJustify
              Expanded = False
              FieldName = 'NUMOUT'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'ANSWERDATE'
              Visible = True
            end
            item
              ButtonStyle = cbsNone
              Expanded = False
              FieldName = 'DT_NAME'
              Width = 173
              Visible = True
            end
            item
              Alignment = taCenter
              Expanded = False
              FieldName = 'ANSWERIS'
              PickList.Strings = (
                #1085#1077#1075#1072#1090#1080#1074#1085#1072
                #1087#1086#1079#1080#1090#1080#1074#1085#1072)
              Width = 83
              Visible = True
            end>
        end
      end
      object gbDoc: TGroupBox
        Left = 605
        Top = 0
        Width = 167
        Height = 244
        Align = alClient
        Caption = #1044#1086#1082#1091#1084#1077#1085#1090
        TabOrder = 2
        object btnDocCreate: TButton
          Left = 14
          Top = 18
          Width = 71
          Height = 25
          Hint = #1057#1090#1074#1086#1088#1080#1085#1080' '#1085#1086#1074#1080#1081' '#1076#1086#1082#1091#1084#1077#1085#1090
          Caption = #1057#1090#1074#1086#1088#1080#1090#1080
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = btnDocCreateClick
        end
        object btnDocEdit: TButton
          Left = 14
          Top = 46
          Width = 71
          Height = 25
          Hint = #1047#1084#1110#1085#1080#1090#1080' '#1087#1086#1090#1086#1095#1085#1080#1081' '#1076#1086#1082#1091#1084#1077#1085#1090
          Caption = #1047#1084#1110#1085#1080#1090#1080
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = btnDocEditClick
        end
        object btnDocAnswer: TButton
          Left = 14
          Top = 74
          Width = 71
          Height = 25
          Hint = #1042#1110#1076#1087#1086#1074#1080#1089#1090#1080' '#1085#1072' '#1087#1086#1090#1086#1095#1085#1080#1081' '#1076#1086#1082#1091#1084#1077#1085#1090
          Caption = #1042#1110#1076#1087#1086#1074#1110#1089#1090#1080
          ParentShowHint = False
          ShowHint = True
          TabOrder = 2
          OnClick = btnDocAnswerClick
        end
        object btnDocDel: TButton
          Left = 14
          Top = 102
          Width = 71
          Height = 25
          Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1087#1086#1090#1086#1095#1085#1080#1081' '#1076#1086#1082#1091#1084#1077#1085#1090
          Caption = #1042#1080#1076#1072#1083#1080#1090#1080
          ParentShowHint = False
          ShowHint = True
          TabOrder = 3
          OnClick = btnDocDelClick
        end
        object btnDocSave: TButton
          Left = 94
          Top = 18
          Width = 71
          Height = 25
          Hint = #1047#1073#1077#1088#1110#1075#1090#1080' '#1079#1084#1110#1085#1080
          Caption = #1047#1073#1077#1088#1110#1075#1090#1080
          ParentShowHint = False
          ShowHint = True
          TabOrder = 4
          OnClick = btnDocSaveClick
        end
        object btnGotoLast: TButton
          Left = 94
          Top = 46
          Width = 71
          Height = 25
          Hint = #1055#1077#1088#1077#1081#1090#1080' '#1085#1072' '#1087#1086#1087#1077#1088#1077#1076#1085#1110#1081' '#1076#1086#1082#1091#1084#1077#1085#1090' ('#1074' '#1083#1072#1085#1094#1110' '#1083#1080#1089#1090'-'#1074#1110#1076#1087#1086#1074#1110#1076#1100')'
          Caption = #1055#1086#1087#1077#1088#1077#1076#1085#1110#1081
          ParentShowHint = False
          ShowHint = True
          TabOrder = 5
          OnClick = btnGotoLastClick
        end
        object btnGotoNext: TButton
          Left = 94
          Top = 74
          Width = 71
          Height = 25
          Hint = #1055#1077#1088#1077#1081#1090#1080' '#1085#1072' '#1085#1072#1089#1090#1091#1087#1085#1080#1081' '#1076#1086#1082#1091#1084#1077#1085#1090' ('#1074' '#1083#1072#1085#1094#1110' '#1083#1080#1089#1090'-'#1074#1110#1076#1087#1086#1074#1110#1076#1100')'
          Caption = #1053#1072#1089#1090#1091#1087#1085#1080#1081
          ParentShowHint = False
          ShowHint = True
          TabOrder = 6
          OnClick = btnGotoNextClick
        end
        object btnReplyRequired: TButton
          Left = 94
          Top = 102
          Width = 71
          Height = 25
          Hint = #1042#1082#1083'/'#1074#1080#1082#1083' '#1092#1110#1083#1100#1090#1088#1072' '#1076#1083#1103' '#1083#1080#1089#1090#1110#1074', '#1097#1086' '#1087#1086#1090#1088#1077#1073#1091#1102#1090#1100' '#1074#1110#1076#1087#1086#1074#1110#1076#1110'"'
          Caption = #1041#1077#1079' '#1074#1110#1076#1087#1086#1074#1110#1076#1110
          ParentShowHint = False
          ShowHint = True
          TabOrder = 7
          OnClick = btnReplyRequiredClick
        end
        object btnFieldList: TButton
          Left = 14
          Top = 144
          Width = 71
          Height = 25
          Hint = #1057#1087#1080#1089#1086#1082' '#1076#1086#1089#1090#1091#1087#1085#1099#1093' '#1087#1086#1083#1077#1081' '#1086#1090#1095#1105#1090#1072' ('#1076#1086#1082#1091#1084#1077#1085#1090#1072')'
          Caption = #1055#1086#1083#1103'...'
          TabOrder = 8
          OnClick = btnFieldListClick
        end
      end
      object gbOrganization: TGroupBox
        Left = 0
        Top = 0
        Width = 137
        Height = 244
        Align = alLeft
        Caption = #1054#1088#1075#1072#1085#1110#1079#1072#1094#1110#1103
        TabOrder = 3
        object btnNewTelecomOrg: TButton
          Left = 20
          Top = 20
          Width = 101
          Height = 25
          Hint = #1044#1086#1073#1072#1074#1080#1090#1080' '#1086#1088#1075#1072#1085#1110#1079#1072#1094#1110#1102' '#1074' '#1089#1087#1080#1089#1086#1082' '#1076#1086#1082#1091#1084#1077#1085#1090#1110#1074' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
          Caption = #1053#1086#1074#1072
          ParentShowHint = False
          ShowHint = True
          TabOrder = 0
          OnClick = btnNewTelecomOrgClick
        end
        object btnDelOrg: TButton
          Left = 20
          Top = 52
          Width = 101
          Height = 25
          Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1086#1088#1075#1072#1085#1110#1079#1072#1094#1110#1102' '#1079#1110' '#1089#1087#1080#1089#1082#1072' '#1076#1086#1082#1091#1084#1077#1085#1090#1110#1074' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
          Caption = #1042#1080#1076#1072#1083#1080#1090#1080
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          OnClick = btnDelOrgClick
        end
      end
    end
    object tshEquipment: TTabSheet
      Caption = #1059#1089#1090#1072#1090#1082#1091#1074#1072#1085#1085#1103
      ImageIndex = 2
      OnShow = tshEquipmentShow
      object gbEquipment: TGroupBox
        Left = 0
        Top = 0
        Width = 861
        Height = 407
        Align = alClient
        Caption = #1059#1089#1090#1072#1090#1082#1091#1074#1072#1085#1085#1103' '
        TabOrder = 0
        object dbgEquipment: TDBGrid
          Left = 2
          Top = 15
          Width = 857
          Height = 390
          Align = alClient
          DataSource = dsEquipment
          TabOrder = 0
          TitleFont.Charset = DEFAULT_CHARSET
          TitleFont.Color = clWindowText
          TitleFont.Height = -11
          TitleFont.Name = 'MS Sans Serif'
          TitleFont.Style = []
          OnEnter = dbgEquipmentEnter
          Columns = <
            item
              Expanded = False
              FieldName = 'EQUIPMENT_ID'
              Visible = False
            end
            item
              Expanded = False
              FieldName = 'TRANSMITTERS_ID'
              Visible = False
            end
            item
              Expanded = False
              FieldName = 'NAME'
              Width = 138
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'TYPEEQUIPMENT'
              Width = 114
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'MANUFACTURE'
              Width = 128
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'NUMFACTORY'
              Width = 104
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'NUMSTANDCERTIFICATE'
              Width = 165
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'DATESTANDCERTIFICATE'
              Width = 87
              Visible = True
            end>
        end
      end
      object pnlEquipButton: TPanel
        Left = 0
        Top = 407
        Width = 861
        Height = 42
        Align = alBottom
        BevelOuter = bvNone
        TabOrder = 1
        object btnEqAdd: TButton
          Left = 596
          Top = 9
          Width = 75
          Height = 25
          Caption = #1044#1086#1073#1072#1074#1080#1090#1080
          TabOrder = 0
          OnClick = btnEqAddClick
        end
        object btnEqDel: TButton
          Left = 676
          Top = 9
          Width = 75
          Height = 25
          Caption = #1042#1080#1076#1072#1083#1080#1090#1080
          TabOrder = 1
          OnClick = btnEqDelClick
        end
      end
    end
    object tshLicenses: TTabSheet
      Caption = #1051#1110#1094#1077#1085#1079#1110#1111
      ImageIndex = 3
      DesignSize = (
        772
        449)
      object lblLicenseChannel: TLabel
        Left = 324
        Top = 4
        Width = 60
        Height = 13
        Caption = #1051#1110#1094#1077#1085#1079#1110#1103' '#1053#1056
      end
      object lblPermUse: TLabel
        Left = 16
        Top = 192
        Width = 121
        Height = 13
        Caption = #1044#1086#1079#1074#1110#1083' '#1085#1072' '#1077#1082#1089#1087#1083#1091#1072#1090#1072#1094#1110#1102
      end
      object lblDatePermUseBeg: TLabel
        Left = 176
        Top = 192
        Width = 64
        Height = 13
        Caption = #1044#1072#1090#1072' '#1074#1080#1076#1072#1095#1080
      end
      object lblDatePermUseEnd: TLabel
        Left = 312
        Top = 192
        Width = 56
        Height = 13
        Caption = #1044#1110#1081#1089#1085#1080#1081' '#1076#1086
      end
      object lblNumStdCertification: TLabel
        Left = 33
        Top = 300
        Width = 125
        Height = 13
        Caption = #1057#1077#1088#1090#1080#1092'i'#1082#1072#1090' '#1074'i'#1076#1087#1086#1074#1110#1076#1085#1086#1089#1090#1110
      end
      object lblDateCertification: TLabel
        Left = 308
        Top = 300
        Width = 90
        Height = 13
        Caption = #1044#1072#1090#1072' '#1089#1077#1088#1090#1080#1092'i'#1082#1072#1090#1091
      end
      object lblFactoryNum: TLabel
        Left = 524
        Top = 300
        Width = 96
        Height = 13
        Caption = #1047#1072#1074#1086#1076#1089#1100#1082#1080#1081' '#1085#1086#1084#1077#1088
      end
      object lblNrReqNo: TLabel
        Left = 20
        Top = 4
        Width = 55
        Height = 13
        Caption = #1047#1072#1103#1074#1082#1072' '#1053#1056
      end
      object lblNrReqDate: TLabel
        Left = 184
        Top = 4
        Width = 26
        Height = 13
        Caption = #1044#1072#1090#1072
      end
      object lblNrConclNo: TLabel
        Left = 20
        Top = 48
        Width = 67
        Height = 13
        Caption = #1042#1080#1089#1085#1086#1074#1086#1082' '#1053#1056
      end
      object lblNrConclDate: TLabel
        Left = 184
        Top = 48
        Width = 26
        Height = 13
        Caption = #1044#1072#1090#1072
      end
      object lblNrApplNo: TLabel
        Left = 20
        Top = 92
        Width = 62
        Height = 13
        Caption = #1055#1086#1076#1072#1085#1085#1103' '#1053#1056
      end
      object lblNrApplDate: TLabel
        Left = 184
        Top = 92
        Width = 26
        Height = 13
        Caption = #1044#1072#1090#1072
      end
      object lblEmsNo: TLabel
        Left = 16
        Top = 152
        Width = 105
        Height = 13
        Caption = #1042#1080#1089#1085#1086#1074#1086#1082' '#1097#1086#1076#1086' '#1045#1052#1057
      end
      object lblEmsDateBeg: TLabel
        Left = 176
        Top = 152
        Width = 64
        Height = 13
        Caption = #1044#1072#1090#1072' '#1074#1080#1076#1072#1095#1080
      end
      object lblEmsDateEnd: TLabel
        Left = 312
        Top = 152
        Width = 56
        Height = 13
        Caption = #1044#1110#1081#1089#1085#1080#1081' '#1076#1086
      end
      object bev1: TBevel
        Left = 8
        Top = 144
        Width = 752
        Height = 9
        Anchors = [akLeft, akTop, akRight]
        Shape = bsTopLine
      end
      object bev2: TBevel
        Left = 8
        Top = 240
        Width = 752
        Height = 9
        Anchors = [akLeft, akTop, akRight]
        Shape = bsTopLine
      end
      object lblJcsCond: TLabel
        Left = 532
        Top = 152
        Width = 116
        Height = 13
        Caption = #1059#1084#1086#1074#1080' '#1087#1086#1075#1086#1076#1078#1077#1085#1085#1103' '#1043#1064
      end
      object lblPermOwner: TLabel
        Left = 532
        Top = 192
        Width = 87
        Height = 13
        Caption = #1042#1083#1072#1089#1085#1080#1082' '#1076#1086#1079#1074#1086#1083#1091
      end
      object lbllResLic: TLabel
        Left = 20
        Top = 248
        Width = 82
        Height = 13
        Caption = #1051'i'#1094#1077#1085#1079'i'#1103' '#1085#1072' '#1056#1063#1056
      end
      object lbResLicDateBeg: TLabel
        Left = 176
        Top = 248
        Width = 64
        Height = 13
        Caption = #1044#1072#1090#1072' '#1074#1080#1076#1072#1095#1080
      end
      object lblResLicDateEnd: TLabel
        Left = 312
        Top = 248
        Width = 50
        Height = 13
        Caption = #1044#1110#1081#1089#1085#1072' '#1076#1086
      end
      object lblResLicOwner: TLabel
        Left = 532
        Top = 248
        Width = 82
        Height = 13
        Caption = #1042#1083#1072#1089#1085#1080#1082' '#1083'i'#1094#1077#1085#1079'ii'
      end
      object dtpResLicDateEnd: TDateTimePicker
        Left = 312
        Top = 264
        Width = 101
        Height = 21
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Enabled = False
        Kind = dtkDate
        ParseInput = False
        TabOrder = 36
        OnChange = dtpResLicDateEndChange
      end
      object dtpResLicDateBeg: TDateTimePicker
        Left = 176
        Top = 264
        Width = 101
        Height = 21
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Enabled = False
        Kind = dtkDate
        ParseInput = False
        TabOrder = 35
        OnChange = dtpResLicDateBegChange
      end
      object dtpEmsDateEnd: TDateTimePicker
        Left = 312
        Top = 168
        Width = 101
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 34
        OnChange = dtpEmsDateEndChange
      end
      object dtpEmsDateBeg: TDateTimePicker
        Left = 176
        Top = 168
        Width = 101
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 33
        OnChange = dtpEmsDateBegChange
      end
      object dtpNrAppl: TDateTimePicker
        Left = 176
        Top = 108
        Width = 101
        Height = 21
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 32
        OnChange = dtpNrApplChange
      end
      object dtpNrConcl: TDateTimePicker
        Left = 176
        Top = 64
        Width = 101
        Height = 21
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 31
        OnChange = dtpNrConclChange
      end
      object dtpNrReq: TDateTimePicker
        Left = 176
        Top = 20
        Width = 101
        Height = 21
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 30
        OnChange = dtpNrReqChange
      end
      object edtPermUse: TDBEdit
        Left = 16
        Top = 208
        Width = 121
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        DataField = 'NUMPERMUSE'
        DataSource = dsLicenses
        TabOrder = 11
      end
      object cbxNumStdCertification: TDBComboBox
        Left = 168
        Top = 296
        Width = 121
        Height = 21
        DataField = 'NUMSTANDCERTIFICATE'
        DataSource = dsLicenses
        ItemHeight = 13
        TabOrder = 24
      end
      object edtFactoryNum: TDBEdit
        Left = 624
        Top = 296
        Width = 105
        Height = 21
        DataField = 'NUMFACTORY'
        DataSource = dsLicenses
        TabOrder = 26
      end
      object dtpPermUseBeg: TDateTimePicker
        Left = 176
        Top = 208
        Width = 101
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 27
        OnChange = dtpPermUseBegChange
      end
      object dtpPermUseEnd: TDateTimePicker
        Left = 312
        Top = 208
        Width = 101
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        CalAlignment = dtaLeft
        Date = 37843.5564868981
        Time = 37843.5564868981
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 28
        OnChange = dtpPermUseEndChange
      end
      object dtpDatCertificate: TDateTimePicker
        Left = 408
        Top = 296
        Width = 101
        Height = 21
        CalAlignment = dtaLeft
        Date = 37843.5524871528
        Time = 37843.5524871528
        DateFormat = dfShort
        DateMode = dmComboBox
        Kind = dtkDate
        ParseInput = False
        TabOrder = 29
        OnChange = dtpDatCertificateChange
      end
      object edtPermUseBeg: TDBEdit
        Left = 176
        Top = 208
        Width = 85
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        DataField = 'DATEPERMUSEFROM'
        DataSource = dsLicenses
        TabOrder = 12
      end
      object edtPermUseEnd: TDBEdit
        Left = 312
        Top = 208
        Width = 85
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        DataField = 'DATEPERMUSETO'
        DataSource = dsLicenses
        TabOrder = 13
      end
      object edtDateCertificate: TDBEdit
        Left = 408
        Top = 296
        Width = 85
        Height = 21
        DataField = 'DATESTANDCERTIFICATE'
        DataSource = dsLicenses
        TabOrder = 25
      end
      object edNrReqNo: TDBEdit
        Left = 16
        Top = 20
        Width = 121
        Height = 21
        DataField = 'NR_REQ_NO'
        DataSource = dsLicenses
        TabOrder = 0
      end
      object edNrReqDate: TDBEdit
        Left = 176
        Top = 20
        Width = 85
        Height = 21
        DataField = 'NR_REQ_DATE'
        DataSource = dsLicenses
        TabOrder = 1
      end
      object edNrConclNo: TDBEdit
        Left = 16
        Top = 64
        Width = 121
        Height = 21
        DataField = 'NR_CONCL_NO'
        DataSource = dsLicenses
        TabOrder = 2
      end
      object edNrConclDate: TDBEdit
        Left = 176
        Top = 64
        Width = 85
        Height = 21
        DataField = 'NR_CONCL_DATE'
        DataSource = dsLicenses
        TabOrder = 3
      end
      object edNrApplNo: TDBEdit
        Left = 16
        Top = 108
        Width = 121
        Height = 21
        DataField = 'NR_APPL_NO'
        DataSource = dsLicenses
        TabOrder = 4
      end
      object edNrApplDate: TDBEdit
        Left = 176
        Top = 108
        Width = 85
        Height = 21
        DataField = 'NR_APPL_DATE'
        DataSource = dsLicenses
        TabOrder = 6
      end
      object edEmsNo: TDBEdit
        Left = 16
        Top = 168
        Width = 121
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        DataField = 'EMC_CONCL_NUM'
        DataSource = dsLicenses
        TabOrder = 8
      end
      object edEmsDateBeg: TDBEdit
        Left = 176
        Top = 168
        Width = 85
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        DataField = 'EMC_CONCL_FROM'
        DataSource = dsLicenses
        TabOrder = 9
      end
      object edEmsDateEnd: TDBEdit
        Left = 312
        Top = 168
        Width = 85
        Height = 21
        Hint = 
          #1055#1086#1083#1077' '#1079#1072#1087#1086#1083#1085#1103#1077#1090#1089#1103' '#1087#1088#1080' '#1089#1086#1079#1076#1072#1085#1080#1080' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1077#1075#1086' '#1076#1086#1082#1091#1084#1077#1085#1090#1072' '#1085#1072' '#1079#1072#1082#1083 +
          #1072#1076#1082#1077' "'#1044#1086#1082#1091#1084#1077#1085#1090#1099'"'
        DataField = 'EMC_CONCL_TO'
        DataSource = dsLicenses
        TabOrder = 10
      end
      object btNrAppl: TButton
        Left = 136
        Top = 108
        Width = 21
        Height = 21
        Caption = '...'
        TabOrder = 5
      end
      object edJcsCond: TDBEdit
        Left = 528
        Top = 168
        Width = 209
        Height = 21
        DataSource = dsLicenses
        TabOrder = 14
      end
      object btJcsCond: TButton
        Left = 736
        Top = 168
        Width = 21
        Height = 21
        Caption = '...'
        TabOrder = 15
        Visible = False
      end
      object edPermOwner: TDBEdit
        Left = 528
        Top = 208
        Width = 209
        Height = 21
        DataField = 'PERMUSE_O_NAME'
        DataSource = dsLicenses
        ReadOnly = True
        TabOrder = 16
      end
      object btPermOwner: TButton
        Left = 736
        Top = 208
        Width = 21
        Height = 21
        Caption = '...'
        TabOrder = 17
        OnClick = btPermOwnerClick
      end
      object edResLic: TDBEdit
        Left = 16
        Top = 264
        Width = 121
        Height = 21
        DataField = 'L_RFR_NUMLICENSE'
        DataSource = dsLicenses
        TabOrder = 18
      end
      object btResLic: TButton
        Left = 136
        Top = 264
        Width = 21
        Height = 21
        Caption = '...'
        TabOrder = 19
        OnClick = btResLicClick
      end
      object edResLicDateBeg: TDBEdit
        Left = 176
        Top = 264
        Width = 85
        Height = 21
        DataField = 'L_RFR_DATEFROM'
        DataSource = dsLicenses
        ReadOnly = True
        TabOrder = 20
      end
      object edResLicDateEnd: TDBEdit
        Left = 312
        Top = 264
        Width = 85
        Height = 21
        DataField = 'L_RFR_DATETO'
        DataSource = dsLicenses
        ReadOnly = True
        TabOrder = 21
      end
      object edlResLicOwner: TDBEdit
        Left = 528
        Top = 264
        Width = 209
        Height = 21
        DataField = 'L_RFR_O_NAME'
        DataSource = dsLicenses
        ReadOnly = True
        TabOrder = 22
      end
      object btlResLicOwner: TButton
        Left = 736
        Top = 264
        Width = 21
        Height = 21
        Caption = '...'
        Enabled = False
        TabOrder = 23
      end
      object btnAttach: TButton
        Left = 592
        Top = 2
        Width = 75
        Height = 21
        Caption = #1044#1086#1073#1072#1074#1080#1090#1100'...'
        TabOrder = 37
        OnClick = btnAttachClick
      end
      object btnDetach: TButton
        Left = 680
        Top = 2
        Width = 75
        Height = 21
        Caption = #1054#1089#1074#1086#1073#1086#1076#1080#1090#1100
        TabOrder = 38
        OnClick = btnDetachClick
      end
      object gbHoursOfOp: TGroupBox
        Left = 312
        Top = 328
        Width = 177
        Height = 73
        Caption = ' '#1063#1072#1089#1080' '#1088#1086#1073#1086#1090#1080' '
        TabOrder = 40
        object lbOpStart: TLabel
          Left = 16
          Top = 24
          Width = 42
          Height = 13
          Caption = #1055#1086#1095#1072#1090#1086#1082
        end
        object lbOpEnd: TLabel
          Left = 16
          Top = 48
          Width = 33
          Height = 13
          Caption = #1050'i'#1085#1077#1094#1100
        end
        object edOpStart: TDBEdit
          Left = 80
          Top = 20
          Width = 81
          Height = 21
          DataField = 'OP_HH_FR'
          DataSource = dsLicenses
          TabOrder = 0
        end
        object edOpEnd: TDBEdit
          Left = 80
          Top = 44
          Width = 80
          Height = 21
          DataField = 'OP_HH_TO'
          DataSource = dsLicenses
          TabOrder = 1
        end
      end
      object gbUseDates: TGroupBox
        Left = 16
        Top = 328
        Width = 217
        Height = 73
        Caption = ' '#1045#1082#1089#1087#1083#1091#1072#1090#1072#1094'i'#1103' '
        TabOrder = 39
        object lbIntoUse: TLabel
          Left = 16
          Top = 24
          Width = 49
          Height = 13
          Caption = #1042#1074#1077#1076#1077#1085#1085#1103
        end
        object lbExpired: TLabel
          Left = 16
          Top = 48
          Width = 27
          Height = 13
          Caption = #1042#1080#1074'i'#1076
        end
        object dtIntoUse: TDateTimePicker
          Left = 80
          Top = 20
          Width = 122
          Height = 21
          CalAlignment = dtaLeft
          Date = 39504.9415110185
          Time = 39504.9415110185
          DateFormat = dfShort
          DateMode = dmComboBox
          Kind = dtkDate
          ParseInput = False
          TabOrder = 0
          TabStop = False
          OnChange = dtIntoUseChange
        end
        object dtExpired: TDateTimePicker
          Left = 80
          Top = 44
          Width = 122
          Height = 21
          CalAlignment = dtaLeft
          Date = 39504.9415777546
          Time = 39504.9415777546
          DateFormat = dfShort
          DateMode = dmComboBox
          Kind = dtkDate
          ParseInput = False
          TabOrder = 1
          TabStop = False
          OnChange = dtExpiredChange
        end
        object edDateBegUse: TDBEdit
          Left = 80
          Top = 20
          Width = 105
          Height = 21
          DataField = 'DATEINTENDUSE'
          DataSource = dsLicenses
          TabOrder = 2
        end
        object edExpiry: TDBEdit
          Left = 80
          Top = 44
          Width = 105
          Height = 21
          DataField = 'D_EXPIRY'
          DataSource = dsLicenses
          TabOrder = 3
        end
      end
      inline objGrdLic: TLisObjectGrid
        Left = 315
        Top = 25
        Width = 437
        Height = 104
        Anchors = [akLeft, akTop, akRight]
        TabOrder = 7
        inherited dg: TStringGrid
          Width = 437
          Height = 85
          BorderStyle = bsSingle
          PopupMenu = nil
        end
        inherited hd: THeaderControl
          Width = 437
          BorderWidth = 1
        end
      end
    end
    object tshTestpoint: TTabSheet
      Caption = #1050#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080
      ImageIndex = 4
      OnShow = tshTestpointShow
      object gbTestpoints: TGroupBox
        Left = 0
        Top = 0
        Width = 861
        Height = 449
        Align = alClient
        Caption = #1050#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080
        TabOrder = 0
        object dbgTestpoints: TDBGrid
          Left = 2
          Top = 15
          Width = 857
          Height = 432
          Align = alClient
          DataSource = dsTestpoints
          TabOrder = 0
          TitleFont.Charset = DEFAULT_CHARSET
          TitleFont.Color = clWindowText
          TitleFont.Height = -11
          TitleFont.Name = 'MS Sans Serif'
          TitleFont.Style = []
          OnEnter = dbgTestpointsEnter
          Columns = <
            item
              Expanded = False
              FieldName = 'NAME'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'LATITUDE'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'LONGITUDE'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'TESTPOINT_TYPE'
              PickList.Strings = (
                #1082#1088#1091#1075#1086#1074#1072
                #1088#1077#1090#1088'-'#1090#1086#1088
                #1082#1086#1088#1076#1086#1085)
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'BEARING'
              Title.Caption = #1040#1079#1080#1084#1091#1090
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'DISTANCE'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'USEBLEFIELD'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'PROTECTEDFIELD'
              Visible = True
            end>
        end
      end
    end
    object tshChangeLog: TTabSheet
      Caption = #1046#1091#1088#1085#1072#1083' '#1079#1084#1110#1085
      ImageIndex = 5
      object dbgGhangeLog: TDBGrid
        Left = 0
        Top = 0
        Width = 861
        Height = 449
        Align = alClient
        TabOrder = 0
        TitleFont.Charset = DEFAULT_CHARSET
        TitleFont.Color = clWindowText
        TitleFont.Height = -11
        TitleFont.Name = 'MS Sans Serif'
        TitleFont.Style = []
      end
      object panList: TPanel
        Left = 0
        Top = 0
        Width = 861
        Height = 449
        Align = alClient
        BevelOuter = bvNone
        Caption = #1046#1091#1088#1085#1072#1083' '#1072#1082#1090#1080#1074#1085#1086#1089#1090#1110' '#1082#1086#1088#1080#1089#1090#1091#1074#1072#1095#1110#1074
        TabOrder = 1
        object dgrList: TDBGrid
          Left = 0
          Top = 0
          Width = 861
          Height = 424
          Align = alClient
          DataSource = dsUserActLog
          Options = [dgEditing, dgTitles, dgIndicator, dgColumnResize, dgColLines, dgRowLines, dgTabs, dgCancelOnExit]
          TabOrder = 0
          TitleFont.Charset = DEFAULT_CHARSET
          TitleFont.Color = clWindowText
          TitleFont.Height = -11
          TitleFont.Name = 'MS Sans Serif'
          TitleFont.Style = []
        end
        object panSearch: TPanel
          Left = 0
          Top = 424
          Width = 861
          Height = 25
          Align = alBottom
          BevelOuter = bvNone
          TabOrder = 1
          Visible = False
          object edtIncSearch: TEdit
            Left = 14
            Top = 1
            Width = 121
            Height = 21
            Ctl3D = True
            ParentCtl3D = False
            TabOrder = 0
          end
        end
      end
    end
    object tshMap: TTabSheet
      Caption = #1050#1072#1088#1090#1072
      ImageIndex = 6
      inline cmf: TCustomMapFrame
        Left = 0
        Top = 0
        Width = 772
        Height = 449
        Align = alClient
        ParentShowHint = False
        ShowHint = True
        TabOrder = 0
        inherited sb: TStatusBar
          Top = 430
          Width = 772
        end
        inherited bmf: TBaseMapFrame
          Width = 772
          Height = 401
        end
        inherited tb: TToolBar
          Width = 772
          Images = cmf.bmf.iml
          inherited tb1: TToolButton
            Action = cmf.bmf.actNone
          end
          inherited tb2: TToolButton
            Action = cmf.bmf.actPan
          end
          inherited tb3: TToolButton
            Action = cmf.bmf.actZoomInTwice
          end
          inherited tb4: TToolButton
            Action = cmf.bmf.actZoomOutTwice
          end
          inherited tb5: TToolButton
            Action = cmf.bmf.actConf
          end
          inherited tb6: TToolButton
            Action = cmf.bmf.actArrows
          end
        end
      end
    end
  end
  object pnlForAllBottom: TPanel
    Left = 0
    Top = 529
    Width = 780
    Height = 36
    Align = alBottom
    BevelInner = bvRaised
    BevelOuter = bvLowered
    TabOrder = 2
    object sbIntoProject: TSpeedButton
      Left = 16
      Top = 7
      Width = 25
      Height = 22
      Hint = #1047#1072#1085#1077#1089#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '#1074' '#1087#1088#1086#1077#1082#1090
      Action = actIntoProject
      Caption = #1042' '#1087#1088#1086#1077#1082#1090
      Glyph.Data = {
        36060000424D3606000000000000360000002800000020000000100000000100
        18000000000000060000C40E0000C40E00000000000000000000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF05710AFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FF636363FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FF05710A05710AFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF63
        6363636363FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FF05710A76F9A705710A05710A05710A05710A0571
        0A05710AFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF63636398
        9898636363636363636363636363636363636363FF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FF05710A76F9A776F9A76FF39E5BE38342CE6128B93F14A8
        2405710AFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF636363B3B3B3AC
        ACAC9F9F9F919191828282727272676767636363FF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FF05710A76F9A705710A05710A05710A05710A0571
        0A05710AFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF636363B3
        B3B3636363636363636363636363636363636363FF00FFFF00FFFF00FF1C78D5
        1C78D51C78D51C78D51C5996FF00FF05710A05710AFF00FFFF00FFE4F0FC1C78
        D51C78D51C78D51C78D5FF00FF828282828282828282828282636363FF00FF63
        6363636363FF00FFFF00FFFAFAFA8282828282828282828282821C78D582C6F9
        57BCFF4EB5FF4DB4FF1C5996FF00FFFF00FF05710AFF00FFFF00FFE4F0FC2A95
        FF369BFF379CFF1C78D5828282C7C7C7B5B5B5B0B0B0B0B0B0636363FF00FFFF
        00FF636363FF00FFFF00FFFAFAFA9E9E9EA4A4A4A5A5A58282821C78D57DC3F7
        56BCFF4EB4FE4DB3FF1C5996FF00FFFF00FFFF00FFFF00FFFF00FFE4F0FC2893
        FF3499FF359AFF1C78D5828282C4C4C4B4B4B4B0B0B0B0B0B0636363FF00FFFF
        00FFFF00FFFF00FFFF00FFFAFAFA9D9D9DA3A3A3A4A4A48282821C78D580C6F9
        5BC1FF53BAFF52B8FF1C5996FF00FFFF00FFFF00FFFF00FFFF00FFE4F0FC1F8E
        FF2B95FF2C96FF1C78D5828282C6C6C6B7B7B7B3B3B3B2B2B2636363FF00FFFF
        00FFFF00FFFF00FFFF00FFFAFAFA9999999F9F9F9F9F9F8282821C78D580C6F9
        5BC1FF53BAFF52B8FF1C5996FF00FFFF00FFFF00FFFF00FFFF00FFE4F0FCE4F0
        FCE4F0FCE4F0FCE4F0FC828282C6C6C6B7B7B7B3B3B3B2B2B2636363FF00FFFF
        00FFFF00FFFF00FFFF00FFFAFAFAFAFAFAFAFAFAFAFAFAFAFAFA1C78D5629DCF
        3589CF3589CF3589CF1C59961C59961C59961C59961C59961C5996FF00FFFF00
        FFFF00FFFF00FFFF00FF828282A2A2A28C8C8C8C8C8C8C8C8C63636363636363
        6363636363636363636363FF00FFFF00FFFF00FFFF00FFFF00FF1C78D586CCF9
        65CBFF5DC3FF5CC4FF3589CF53BAFF53BAFF4EB4FF4DB4FF1C78D5FF00FFFF00
        FFFF00FFFF00FFFF00FF828282C9C9C9BCBCBCB8B8B8B7B7B78C8C8CB3B3B3B3
        B3B3B0B0B0B0B0B0828282FF00FFFF00FFFF00FFFF00FFFF00FF1C78D584C9F7
        65CAFF5EC3FE5EC4FF3589CF53BAFF54BAFF4FB4FE4FB4FF1C78D5FF00FFFF00
        FFFF00FFFF00FFFF00FF828282C7C7C7BCBCBCB8B8B8B8B8B88C8C8CB3B3B3B3
        B3B3B0B0B0B1B1B1828282FF00FFFF00FFFF00FFFF00FFFF00FF1C78D585CBF8
        64CBFF5EC6FF5EC7FF3589CF53BAFF55BDFF50B7FF50B7FF1C78D5FF00FFFF00
        FFFF00FFFF00FFFF00FF828282C8C8C8BBBBBBB8B8B8B8B8B88C8C8CB3B3B3B4
        B4B4B1B1B1B1B1B1828282FF00FFFF00FFFF00FFFF00FFFF00FF1C78D59ECFF5
        92D7FD88D2FC8CD5FD629DCF85CEFD85CEFD80C9FC84CBFD1C78D5FF00FFFF00
        FFFF00FFFF00FFFF00FF828282D3D3D3D1D1D1CCCCCCCECECEA2A2A2CBCBCBCB
        CBCBC8C8C8CACACA828282FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF1C78D5
        1C78D51C78D51C78D51C78D51C78D51C78D51C78D51C78D5FF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF82828282828282828282828282828282828282
        8282828282828282FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
      Layout = blGlyphTop
      Margin = 0
      NumGlyphs = 2
    end
    object sbIntoBase: TSpeedButton
      Left = 44
      Top = 7
      Width = 25
      Height = 22
      Hint = #1055#1077#1088#1077#1085#1077#1089#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '#1074' '#1041#1040#1047#1059
      Action = actIntoBase
      Glyph.Data = {
        36060000424D3606000000000000360000002800000020000000100000000100
        18000000000000060000C40E0000C40E00000000000000000000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF08750D08750D08750D0875
        0DFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FF8C8C8C8C8C8C8C8C8C8C8C8CFF00FFFF00FFFF00FFFF00FFFF00FF
        0E80AA0E80AA0E80AA0E80AA0E80AA0E80AAFF00FF08750D13AA2210A61D0875
        0DFF00FFFF00FFFF00FFFF00FFFF00FF83838383838383838383838383838383
        8383FF00FF8C8C8CA3A3A3A1A1A18C8C8CFF00FFFF00FFFF00FFFF00FF078DBB
        49D5EE23D7FE36D9FE6FE6FF8DE7FA44BADD0E80AA08750D1AB12D16AD260875
        0DFF00FFFF00FFFF00FFFF00FF878787AFAFAFA8A8A8AFAFAFC3C3C3CCCCCCA8
        A8A88383838C8C8CA8A8A8A5A5A58C8C8CFF00FFFF00FFFF00FF078DBB8CFBFE
        59EAFE23D7FE36D8FD6CE0F808750D08750D08750D08750D22B93B1DB5320875
        0D08750D08750D08750D878787CDCDCDBBBBBBA8A8A8AEAEAEC0C0C08C8C8C8C
        8C8C8C8C8C8C8C8CADADADAAAAAA8C8C8C8C8C8C8C8C8C8C8C8C078DBB8CFBFE
        59EAFE23D7FE36D8FD6CE0F808750D3CD46236CF5A30C9522CC34926BE4121B8
        381CB43117AF2A08750D878787CDCDCDBBBBBBA8A8A8AEAEAEC0C0C08C8C8CC0
        C0C0BCBCBCB8B8B8B4B4B4B1B1B1ACACACAAAAAAA6A6A68C8C8C078DBB8CFBFE
        59EAFE23D7FE36D9FE6CE1F908750D44DD703FD8683AD26035CD582FC74F2AC1
        4725BD3E20B83608750D878787CDCDCDBBBBBBA8A8A8AFAFAFC0C0C08C8C8CC6
        C6C6C2C2C2BFBFBFBBBBBBB7B7B7B3B3B3B0B0B0ACACAC8C8C8C078DBBB3FCFE
        B6F6FFC6F5FFE3FAFFE9F9FD08750D08750D08750D08750D3ED76638D15E0875
        0D08750D08750D08750D878787DADADADCDCDCE1E1E1ECECECEDEDED8C8C8C8C
        8C8C8C8C8C8C8C8CC2C2C2BDBDBD8C8C8C8C8C8C8C8C8C8C8C8C078DBBBAEEF6
        30BCDD11A7D2129FCB20A1CA35A7CD2692BF92CEE408750D46DE7341DA6D0875
        0DFF00FFFF00FFFF00FF878787DADADAA1A1A19292929090909595959D9D9D93
        9393C6C6C68C8C8CC7C7C7C4C4C48C8C8CFF00FFFF00FFFF00FF078DBB4AC5DD
        59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E30682B608750D4DE67F49E2790875
        0DFF00FFFF00FFFF00FF878787AAAAAABBBBBBA8A8A8AFAFAFC3C3C3CCCCCCAC
        ACAC8585858C8C8CCCCCCCC9C9C98C8C8CFF00FFFF00FFFF00FF078DBB8CFBFE
        59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E3089DCF08750D08750D08750D0875
        0DFF00FFFF00FFFF00FF878787CDCDCDBBBBBBA8A8A8AFAFAFC3C3C3CCCCCCAC
        ACAC8E8E8E8C8C8C8C8C8C8C8C8C8C8C8CFF00FFFF00FFFF00FF078DBB8CFBFE
        59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E3089CCE0E7FA9FF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FF878787CDCDCDBBBBBBA8A8A8AFAFAFC3C3C3CCCCCCAC
        ACAC8E8E8E838383FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB8CFBFE
        59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E3089DCF0E80AAFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FF878787CDCDCDBBBBBBA8A8A8AFAFAFC3C3C3CCCCCCAC
        ACAC8E8E8E838383FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB91FCFE
        82F8FF6FF8FF7AFEFF97FEFFA0FCFE63DAF50DA2D40E80AAFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FF878787CECECEC9C9C9C3C3C3C7C7C7D1D1D1D4D4D4BB
        BBBB919191838383FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBBFCFFFF
        F4FFFFD3FFFFB4FFFFADFFFFADFFFFA9FFFF72F9FE0E80AAFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FF878787F4F4F4F1F1F1E6E6E6DBDBDBD9D9D9D9D9D9D7
        D7D7C4C4C4838383FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF0C92C0
        F1FBFDE4FFFFC7FFFFAEFFFFA8FFFF9BFBFC1385AFFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8A8A8AF0F0F0ECECECE2E2E2D9D9D9D7D7D7D1
        D1D1878787FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        078DBB078DBB078DBB078DBB078DBB078DBBFF00FFFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FF87878787878787878787878787878787
        8787FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
      Layout = blGlyphTop
      Margin = 0
      NumGlyphs = 2
    end
    object sbIntoArhive: TSpeedButton
      Left = 72
      Top = 7
      Width = 25
      Height = 22
      Hint = #1042#1080#1076#1072#1083#1077#1085#1085#1103' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072' '#1074' '#1040#1056#1061#1030#1042
      Action = actIntoarchives
      Caption = #1042#1080#1076#1072#1083#1077#1085#1085#1103
      Glyph.Data = {
        36060000424D3606000000000000360000002800000020000000100000000100
        18000000000000060000C40E0000C40E00000000000000000000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF00009A00009AFF00FFFF00FF0000
        9A00009AFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FF7E7E7E7E7E7EFF00FFFF00FF7E7E7E7E7E7EFF00FFFF00FFFF00FFFF00FF
        FF00FF0E80AA0E80AA0E80AA0E80AA00009A5480FF041DCD0000B10000B10327
        F80024EF00009AFF00FFFF00FFFF00FFFF00FF8888888888888888888888887E
        7E7EBEBEBE8787877E7E7E7E7E7E9191918D8D8D787878FF00FFFF00FFFF00FF
        078DBB49D5EE23D7FE36D9FE6FE6FF00009A446AFF3765FF051ABC0523E00836
        FF011FC900009AFF00FFFF00FFFF00FF8C8C8CB5B5B5ADADADB4B4B4C8C8C87E
        7E7EADADADB2B2B28383838C8C8CA1A1A18484847E7E7EFF00FFFF00FF078DBB
        8CFBFE59EAFE23D7FE36D9FE6FE6FF8DE7FA00009A4A6EFF3A6CFF0934FF011B
        B700009AFF00FFFF00FFFF00FF8C8C8CD2D2D2C0C0C0ADADADB4B4B4C8C8C8D1
        D1D1777777ADADADB4B4B49B9B9B8080807E7E7EFF00FFFF00FFFF00FF078DBB
        8CFBFE59EAFE23D7FE36D9FE6FE6FF8DE7FA00009A4D71FF72A6FF376AFF011B
        BC00009AFF00FFFF00FFFF00FF8C8C8CD2D2D2C0C0C0ADADADB4B4B4C8C8C8D1
        D1D1777777AEAEAEC7C7C7B4B4B48181817E7E7EFF00FFFF00FFFF00FF078DBB
        8CFBFE59EAFE23D7FE36D9FE6FE6FF00009A4C72FF6092FF405DF64D6CFF3665
        FF0320C600009AFF00FFFF00FF8C8C8CD2D2D2C0C0C0ADADADB4B4B4C8C8C87E
        7E7EB0B0B0C2C2C2A6A6A6ADADADB1B1B18585857E7E7EFF00FFFF00FF078DBB
        B3FCFEB6F6FFC6F5FFE3FAFFEBFBFF00009A83B0FF4C72FF00009A00009A476C
        FF3665FF00009AFF00FFFF00FF8C8C8CDFDFDFE1E1E1E6E6E6F1F1F1F4F4F47E
        7E7ECCCCCCB0B0B07E7E7E7E7E7EADADADB1B1B17E7E7EFF00FFFF00FF078DBB
        BAEEF630BCDD11A7D2129FCB20A2CB35A7CD00009A00009A0D81ABFF00FF0000
        9A00009AFF00FFFF00FFFF00FF8C8C8CDFDFDFA6A6A69797979595959A9A9AA2
        A2A27E7E7E7E7E7E888888FF00FF7E7E7E7E7E7EFF00FFFF00FFFF00FF078DBB
        4AC5DD59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E30682B60A82AFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8C8C8CAFAFAFC0C0C0ADADADB4B4B4C8C8C8D1
        D1D1B1B1B18A8A8A888888FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB
        8CFBFE59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E3089DCF0E80AAFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8C8C8CD2D2D2C0C0C0ADADADB4B4B4C8C8C8D1
        D1D1B1B1B1939393888888FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB
        8CFBFE59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E3089DCF0E80AAFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8C8C8CD2D2D2C0C0C0ADADADB4B4B4C8C8C8D1
        D1D1B1B1B1939393888888FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB
        8CFBFE59EAFE23D7FE36D9FE6FE6FF8DE7FA49C1E3089DCF0E80AAFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8C8C8CD2D2D2C0C0C0ADADADB4B4B4C8C8C8D1
        D1D1B1B1B1939393888888FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB
        91FCFE82F8FF6FF8FF7AFEFF97FEFFA0FCFE63DAF50DA2D40E80AAFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8C8C8CD3D3D3CFCFCFC8C8C8CCCCCCD6D6D6D9
        D9D9C0C0C0969696888888FF00FFFF00FFFF00FFFF00FFFF00FFFF00FF078DBB
        FCFFFFF4FFFFD3FFFFB4FFFFADFFFFADFFFFA9FFFF72F9FE0E80AAFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF8C8C8CF9F9F9F7F7F7EBEBEBE0E0E0DEDEDEDE
        DEDEDDDDDDC9C9C9888888FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        0C92C0F1FBFDE4FFFFC7FFFFAEFFFFA8FFFF9BFBFC1385AFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FF8F8F8FF5F5F5F1F1F1E7E7E7DEDEDEDC
        DCDCD6D6D68C8C8CFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FF078DBB078DBB078DBB078DBB078DBB078DBBFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF8C8C8C8C8C8C8C8C8C8C8C8C8C
        8C8C8C8C8CFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
      Layout = blGlyphTop
      Margin = 0
      NumGlyphs = 2
    end
    object sbIntoBeforeBase: TSpeedButton
      Left = 100
      Top = 7
      Width = 25
      Height = 22
      Hint = #1055#1077#1088#1077#1085#1077#1089#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '#1074' '#1055#1056#1045#1044#1041#1040#1047#1059
      Action = actIntoBeforeBase
      Caption = #1042' '#1087#1077#1088#1077#1076#1073#1072#1079#1091
      Glyph.Data = {
        36060000424D3606000000000000360000002800000020000000100000000100
        18000000000000060000C40E0000C40E00000000000000000000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FF038ABEFF00FFFF00FFFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF9A9A9AFF
        00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FF068EC11D9FCE068EC11588B5038DC0FF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF9C9C9CA9A9A99C
        9C9C9E9E9E9B9B9BFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FF068EC13EB3E32492C17AEAFF8AF3FD78DEEB1A83AF068EC1068E
        C1FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF9C9C9CBCBCBCA7A7A7DB
        DBDBE0E0E0D3D3D39D9D9D9C9C9C9C9C9CFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FF068EC159D4FF2697C9289ACA6DE0FF86F0FE97FEFF9DFFFFA1FFFF7AE0
        EC068EC1068EC10B8CBFFF00FFFF00FFFF00FF9C9C9CD0D0D0AAAAAAACACACD7
        D7D7DFDFDFE5E5E5E8E8E8E9E9E9D5D5D59C9C9C9C9C9C9E9E9EFF00FFFF00FF
        068EC158D1FD55D0FF2295C659D1FA62D8FC7DEAFD94FAFF97FCFF98FEFF96FC
        FD329CC045B8E20F92C4FF00FFFF00FF9C9C9CCECECECECECEA8A8A8CECECED2
        D2D2DCDCDCE4E4E4E5E5E5E5E5E5E4E4E4ACACACBEBEBEA1A1A1FF00FF068EC1
        71E2FF63D8FC5DD2FA23A2D259D1FA59D1FA75E4FC90F7FF9AFFFF8CF1F6329C
        C04BC2F1058DC0FF00FFFF00FF9C9C9CD8D8D8D2D2D2CFCFCFADADADCECECECE
        CECED8D8D8E3E3E3E6E6E6DEDEDEACACACC6C6C69C9C9CFF00FF068EC18FF7FF
        7EEBFE78E6FC7AE0EC0990C255CFFE54CDFBD45252D452527EE4EF329CC04FC7
        F652CCFC058DC0FF00FF9C9C9CE3E3E3DCDCDCDADADAD5D5D59E9E9ECECECECC
        CCCC808080808080D7D7D7ACACACC9C9C9CCCCCC9C9C9CFF00FF068EC1A2FFFF
        90F6FE7AE0EC0A8EC130A4D437ADDCD45252E28989E08484D452526CDEFB6ADD
        FE058DC0FF00FFFF00FF9C9C9CE9E9E9E3E3E3D5D5D59E9E9EB2B2B2B7B7B780
        8080ACACACA8A8A8808080D5D5D5D5D5D59C9C9CFF00FFFF00FF068EC1A4FFFF
        7AE0EC018ABF048BBE0185B7D45252DC7171E8A1A1EAACACDD7777D4525269D8
        F11B9ECBFF00FFFF00FF9C9C9CEAEAEAD5D5D59A9A9A9B9B9B98989880808099
        9999C0C0C0C8C8C89E9E9E808080D0D0D0A8A8A8FF00FFFF00FF068EC17AE0EC
        0089BE0990C20991C3D45252D65959E08383E38E8EE69B9BE9A6A6DA6A6AD452
        52FF00FFFF00FFFF00FF9C9C9CD5D5D59A9A9A9E9E9E9F9F9F808080868686A7
        A7A7B0B0B0BABABAC3C3C3949494808080FF00FFFF00FFFF00FF088FC1068EC1
        0990C20891C30C8BBCD45252D45252D45252DF7E7EE28989D45252D45252D452
        52FF00FFFF00FFFF00FF9D9D9D9C9C9C9E9E9E9E9E9E9D9D9D80808080808080
        8080A3A3A3ADADAD808080808080808080FF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FF0991C30992C40792C4D45252DB6E6EDE7C7CD45252FF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF9F9F9F9F9F9F9E9E9E80
        8080979797A2A2A2808080FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFD45252D86262DB6F6FD45252FF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF80
        80808D8D8D989898808080FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFD45252DC7171D45252FF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FF808080999999808080FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFD45252D96666D86262FF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FF8080809090908D8D8DFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFD45252D45252D452
        52D45252FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FFFF00FF808080808080808080808080FF00FFFF00FF}
      Layout = blGlyphTop
      Margin = 0
      NumGlyphs = 2
    end
    object sbCopy: TSpeedButton
      Left = 128
      Top = 7
      Width = 25
      Height = 22
      Hint = #1057#1090#1074#1086#1088#1080#1090#1080' '#1082#1086#1087#1110#1102' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072' '#1074' '#1055#1056#1045#1044#1041#1040#1047#1030
      Action = actTxCopy
      Glyph.Data = {
        36030000424D3603000000000000360000002800000010000000100000000100
        1800000000000003000000000000000000000000000000000000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FF8400008400008400008400008400008400
        00840000840000840000FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF84
        0000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF840000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FF840000FFFFFF0000000000000000000000
        00000000FFFFFF840000FF00FF00000000000000000000000000000000000084
        0000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF840000FF00FF000000
        FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF840000FFFFFF0000000000000000000000
        00000000FFFFFF840000FF00FF000000FFFFFF00000000000000000000000084
        0000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF840000FF00FF000000
        FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF840000FFFFFF000000000000FFFFFF8400
        00840000840000840000FF00FF000000FFFFFF00000000000000000000000084
        0000FFFFFFFFFFFFFFFFFFFFFFFF840000FFFFFF840000FF00FFFF00FF000000
        FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF840000FFFFFFFFFFFFFFFFFFFFFFFF8400
        00840000FF00FFFF00FFFF00FF000000FFFFFF000000000000FFFFFF00000084
        0000840000840000840000840000840000FF00FFFF00FFFF00FFFF00FF000000
        FFFFFFFFFFFFFFFFFFFFFFFF000000FFFFFF000000FF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FF000000FFFFFFFFFFFFFFFFFFFFFFFF00000000
        0000FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF000000
        000000000000000000000000000000FF00FFFF00FFFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
      Layout = blGlyphTop
      Margin = 0
      Visible = False
    end
    object sbIntoList: TSpeedButton
      Left = 156
      Top = 7
      Width = 25
      Height = 22
      Hint = #1042#1080#1074#1077#1089#1090#1080' '#1089#1087#1080#1089#1086#1082' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
      Action = actIntoList
      Caption = #1059' '#1089#1087#1080#1089#1082#1091
      Glyph.Data = {
        2E020000424D2E020000000000002E0100002800000010000000100000000100
        08000000000000010000120B0000120B00003E0000003E00000000000000FFFF
        FF00FF00FF00FFFDFD00FFFDFC00FFFCFA00FFEFE000FFF9F300FFFBF700FFFE
        FD00FED7AB00FED8AD00FED8AE00FED9B000FEDBB400FFDDB800FEDDB800FEDF
        BC00FEDFBD00FFE1C000FEE3C500FEE3C600FFE6CA00FEE6CB00FFE8D000FEE8
        D000FFEBD500FFEEDB00FEEDDA00FFF0E000FFF2E400FEF2E500FEF4E900FFF7
        EE00FEDAB100FEE1C000FEE2C100FEE4C500FEE4C600FEE6CA00FEE8CF00FEE9
        CF00FEE9D000FEEBD500FFEEDA00FEEEDB00FFF0DF00FEF0DF00FEF0E000FFF3
        E500FFF5E900FEF5EA00FFF9F200FEEEDA00FEF5E900FFF7ED00FFFBF600FEF7
        ED00FEF9F200D6820000FFFDF900009900000202020202020202020202020202
        020202023B3B3B3B3B3B3B3B3B3B3B3B0202023B2C1A29171424110F0E220C0A
        3B02023B1D1C1A2A27252412100E220B3B02023B1E3D3D3D3D16263D3D3D3D0D
        3B02023B331F2F2C1A2917152311100E3B02023B213D3D3D3D2B2A3D3D3D3D10
        3B02023B342136311D1B1A18272613113B02023B083D3D3D3D302C3D3D3D3D23
        3B02023B05380737201E1D2D2B2817263B02023B043D3D3D3D32313D3D3D3D16
        3B02023B01093C3834213231062D2B193B02023B013D3D3D3D3A393D3D3D3D1A
        3B02023B010101033C38342132312E353B0202023B3B3B3B3B3B3B3B3B3B3B3B
        020202020202020202020202020202020202}
      Layout = blGlyphTop
      Margin = 0
    end
    object sbIntoTree: TSpeedButton
      Left = 184
      Top = 7
      Width = 25
      Height = 22
      Hint = #1055#1086#1082#1072#1079#1072#1090#1080' '#1076#1077#1088#1077#1074#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
      Action = actIntoTree
      Caption = #1044#1077#1088#1077#1074#1086
      Glyph.Data = {
        66010000424D6601000000000000660000002800000010000000100000000100
        08000000000000010000120B0000120B00000C0000000C00000000000000FFFF
        FF00FF00FF009B390B009C3A0D009B3A0C009C3B0D00CC670100D06A0000DD88
        2E00EBA45A00FFCF920002020202020202020202020202020202020202020702
        020206060606060202020202020207070708060B0A0906020202020202020702
        0202030604050602020202020202070202020202020202020202020202020702
        020206060606060202020202020207070707060B0A0906020202020202020702
        0202030604050602020202020202070202020202020202020202020202020702
        020206060606060202020202020207070707060B0A0906020202020202020702
        02020306040506020202020206060606060202020202020202020202060B0A09
        0602020202020202020202020306040506020202020202020202020202020202
        02020202020202020202}
      Layout = blGlyphTop
      Margin = 0
    end
    object sbPlanning: TSpeedButton
      Left = 224
      Top = 7
      Width = 25
      Height = 22
      Hint = #1055#1083#1072#1085#1091#1074#1072#1085#1085#1103
      Action = actPlanning
      Caption = #1055#1083#1072#1085#1091#1074#1072#1085#1085#1103
      Glyph.Data = {
        CA030000424DCA03000000000000CA0200002800000010000000100000000100
        08000000000000010000120B0000120B0000A5000000A500000000000000FFFF
        FF005150F1005756F1005F5DF0006A69EF006D6BE7007978D5007E7CDA005E59
        CE007974E1007A77E0006E68E1006C67CF00ABA7E4005A4ED300B1ACE400B4B1
        CF00ADA9C2005949A000A8A0C800B1ACC600A89FC700B4AFC600BDB6C400BCB3
        C40073567700FF00FF00B7818300B6808200B47F8100B17D7F00AA787A009F70
        7200EEEDED00DDCCCA00DBCCCA00B4817600B8857A00B7847A00B78C8300A981
        7900D9C6C000CF9B8600C5876800CD9377008D716300DBA78600D6A38300C99A
        7B00E5E2E000A48B7800E8D9CD00D4CDC700DFD4C900F8EDE200CBC2B800ECE2
        D700E8B27000F6C89200E9D7C100EAD9C500F7E6D200DFD2C200F8EAD900EEE1
        D100F3E6D600D8CFC400ECA54A00C8B39800EAD5BA00C3B29B00E9D6BE00C7B7
        A300F5E2CA00EBDCC800D0C3B200E4D7C600D5CFC700EEE8E000FAC57700F2CE
        9C00F5D2A00084776400ECD7BA00FEF3E300FFF7EB00EDCD9900FFE2B200E8D3
        B100E5D8C300FDEFD900FBEED800C5BBA800C8C7C500FFE7B200FDFAF200D1D0
        CC00F9F0C800C0BA9A0077766800FDFCEC00FEFDD700F4F3D100FFFFDC00F5F5
        D400DDDDCA00FFFFF400FFFFF800EFEFED00C3C3C200C5C6AF00EEEFDC006E6F
        6A00B9BAB700535452009BA49E009CA59F009BA59F009BFFFF009EFFFF00A0FF
        FF00A1FFFF00ACFFFF0095FBFE009CFCFF00ABF9FC0099FBFF0083F0FA008DF6
        FF009EF7FD008DF3FF008FF5FF0092F5FF0094F3FE008DF1FF0094EDF90098ED
        F80078E4F60077E0F3007FE8FB0095EEFD0095EFFD0096E7F500E8F3F5007BE3
        F70064D2EF007CD5EC007FA8B50019A3D4005BA1B9006E9EB000C1C4C500DEE9
        F100E8E9F400E1E2EC002E2EFC006060D2006262CE007474F2007778EF008E8E
        E3008E8FDB00D5D5DE00D8D8D8001B1B1B1B1B1B1B1B1B1B9C9C9C1B1B1B1B1B
        1C1C1D1F211A09071815119D131B1B1B1C5B4A46450DA2675F3B58709E1B1B1B
        25553E3C4706A36866625152179C1B1B2556404B49A0996B69717353129C1B1B
        286037414C9F9B01656F6457199C1B1B296D4F393F0B106C01016A63081B9595
        2E985E354314050E22909AA10F1B9582959595969472160A0304020C201B957E
        787D7F8A9595384D3D4854591E1B95937781838587929536425C5A5D1C1B9595
        8F8886848C91954E342726261C1B957C959595898E8D95612A273A442C1B957A
        777A8095959595A42427502D1B1B957B798B959595976E3223272B1B1B1B1B95
        95957476753331302F261B1B1B1B}
      Margin = 0
      Transparent = False
    end
    object sbExamination: TSpeedButton
      Left = 252
      Top = 7
      Width = 25
      Height = 22
      Hint = #1045#1082#1089#1087#1077#1088#1090#1080#1079#1072
      Caption = #1045#1082#1089#1087#1077#1088#1090#1080#1079#1072
      Glyph.Data = {
        36060000424D3606000000000000360000002800000020000000100000000100
        18000000000000060000C40E0000C40E00000000000000000000FF00FF8C6059
        A47874A47874A47874A47874A47874A47874A47874A47874A47874A478749368
        63FF00FF5F77947C8288FF00FF7272728C8C8C8C8C8C8C8C8C8C8C8C8C8C8C8C
        8C8C8C8C8C8C8C8C8C8C8C8C8C8C7B7B7BFF00FF797979828282FF00FF90635A
        F6E4C4FAE4BCFAE1B3F7DDABF7DAA3F6D79FF6D69CF6D59AF6D59AFADB9E9368
        63637D9E2190EA65A9D4FF00FF757575DDDDDDDBDBDBD6D6D6D1D1D1CDCDCDCA
        CACAC9C9C9C8C8C8C8C8C8CCCCCC7B7B7B8080808585859C9C9CFF00FF93675C
        FFF1D5DF993EDF993EDF993EDF993EDF993EDF993EDF993EDF993EF4D49B6680
        9E228DE854BAFF54ABF1FF00FF777777EAEAEAB0B0B0B0B0B0B0B0B0B0B0B0B0
        B0B0B0B0B0B0B0B0B0B0B0C7C7C7828282858585A9A9A9A2A2A2FF00FF996C5E
        FBF1E3F8EAD3F7E7CBF6E3C4E2CEADAC9C7FAB9879AA9676AD99797881961F86
        DD52B8FF53ABF3FF00FFFF00FF7B7B7BEFEFEFE5E5E5E1E1E1DDDDDDC7C7C795
        95959292929090909393938787877E7E7EA8A8A8A3A3A3FF00FFFF00FF9E7161
        FFFFF4DF993EDF993EC08546BB9187C09C91BD9C8FAB7E7190705A5997CA4FB5
        FB49A9F8FF00FFFF00FFFF00FF7F7F7FF9F9F9B0B0B0B0B0B0838383A1A1A1A8
        A8A8A6A6A68E8E8E757575919191A5A5A5A0A0A0FF00FFFF00FFFF00FFA47863
        FFFFFEFCF8EFDCC8C0BC9994EDE1D0FFFEDFFFFFE4F2E9CEC0A291B79A976573
        8CFF00FFFF00FFFF00FFFF00FF838383FEFEFEF5F5F5CECECEA8A8A8DEDEDEEF
        EFEFF1F1F1E0E0E0A8A8A8A7A7A7787878FF00FFFF00FFFF00FFFF00FFAA7E66
        FFFFFFCB6600B07D65F0E8E4FFFFE6FFFFD5FFFFDCFFF9CFFFEBB8BB92847652
        4EFF00FFFF00FFFF00FFFF00FF888888FFFFFFA1A1A18A8A8AEAEAEAF2F2F2EA
        EAEAEDEDEDE7E7E7DBDBDB9F9F9F626262FF00FFFF00FFFF00FFFF00FFB18369
        FFFFFFCB6600B99D92FDFDE7FFFFD9FFFFD9FFFFDFF9ECC3F9D29BDDBFA1865A
        56FF00FFFF00FFFF00FFFF00FF8D8D8DFFFFFFA1A1A1A5A5A5F2F2F2ECECECEC
        ECECEFEFEFDEDEDECACACABFBFBF6E6E6EFF00FFFF00FFFF00FFFF00FFB7896C
        FFFFFFCB6600BC9E91FFFFE3FFFFD9FFFFDEFFFDDAF3CF9EF6C68BE9D1AE885E
        5AFF00FFFF00FFFF00FFFF00FF919191FFFFFFA1A1A1A6A6A6F1F1F1ECECECEE
        EEEEECECECC8C8C8C0C0C0CBCBCB717171FF00FFFF00FFFF00FFFF00FFBD8F6E
        FFFFFFCB6600B6907FFBF9DFFEFCD7FBF5D0F6DBABF3CB9BFFEFBDD2B19A845B
        57FF00FFFF00FFFF00FFFF00FF959595FFFFFFA1A1A19A9A9AEDEDEDEAEAEAE5
        E5E5D0D0D0C7C7C7DEDEDEB6B6B66D6D6DFF00FFFF00FFFF00FFFF00FFC29470
        FFFFFFFFFFFFC3A6A3DBC6ABFFF1BDF4CA94F6CE96FFFEEEF1E7E1AA86808A60
        5CFF00FFFF00FFFF00FFFF00FF999999FFFFFFFFFFFFB3B3B3C3C3C3DEDEDEC4
        C4C4C6C6C6F6F6F6E9E9E9959595737373FF00FFFF00FFFF00FFFF00FFC69872
        FFFFFFCB6600C76607B1714DCFAE96EED1AAEFDAB2CDB3A9A26E5AD5C7C69368
        63FF00FFFF00FFFF00FFFF00FF9C9C9CFFFFFFC9C9C9A3A3A37F7F7FB2B2B2CC
        CCCCD0D0D0BBBBBBBABABACDCDCD7B7B7BFF00FFFF00FFFF00FFFF00FFC99B73
        FFFFFFCB6600CB6600C86606B66933AC7151A97055AD6638BF6516FFFEFE9368
        63FF00FFFF00FFFF00FFFF00FF9E9E9EFFFFFFC9C9C9A1A1A1A3A3A3B0B0B0BA
        BABABBBBBBAEAEAEA6A6A6FEFEFE7B7B7BFF00FFFF00FFFF00FFFF00FFC99B73
        FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF9368
        63FF00FFFF00FFFF00FFFF00FF9E9E9EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
        FFFFFFFFFFFFFFFFFFFFFFFFFFFF7B7B7BFF00FFFF00FFFF00FFFF00FFC99B73
        DCA887DCA887DCA887DCA887DCA887DCA887DCA887DCA887DCA887DCA887DCA8
        87FF00FFFF00FFFF00FFFF00FF9E9E9EB1B1B1B1B1B1B1B1B1B1B1B1B1B1B1B1
        B1B1B1B1B1B1B1B1B1B1B1B1B1B1B1B1B1FF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
      Layout = blGlyphTop
      Margin = 0
      NumGlyphs = 2
      OnClick = actExaminationExecute
    end
    object btnSave: TBitBtn
      Left = 504
      Top = 6
      Width = 78
      Height = 25
      Action = actApply
      Caption = #1047#1073#1077#1088#1110#1075#1090#1080
      TabOrder = 1
      NumGlyphs = 2
    end
    object btnLoad: TBitBtn
      Left = 592
      Top = 6
      Width = 78
      Height = 25
      Action = actLoad
      Caption = #1054#1073#1085#1086#1074#1080#1090#1080
      TabOrder = 2
      NumGlyphs = 2
    end
    object btnOk: TBitBtn
      Left = 420
      Top = 6
      Width = 73
      Height = 25
      Action = actOk
      Caption = 'Ok'
      TabOrder = 0
      NumGlyphs = 2
    end
    object btnCansel: TBitBtn
      Left = 680
      Top = 6
      Width = 78
      Height = 25
      Action = actClose
      Cancel = True
      Caption = #1042#1080#1081#1090#1080
      TabOrder = 3
      NumGlyphs = 2
    end
  end
  object ibdsStantionsBase: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsStantionsBaseAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select   '#9'TRN.ID,'
      #9'TRN.STAND_ID, '
      #9'TRN.ADMINISTRATIONID,'#9
      #9'TRN.DATECREATE,'
      #9'TRN.DATECHANGE,'
      #9'TRN.OWNER_ID, '
      #9'TRN.RESPONSIBLEADMIN,'
      #9'TRN.ACCOUNTCONDITION_IN,  '
      #9'TRN.ACCOUNTCONDITION_OUT, '
      #9'TRN.SYSTEMCAST_ID,'
      #9'TRN.CLASSWAVE, '
      #9'TRN.TIMETRANSMIT,'
      #9'TRN.NAMEPROGRAMM,'
      #9'TRN.USERID, '
      #9'TRN.ORIGINALID,'
      #9'TRN.NUMREGISTRY,'
      #9'TRN.TYPEREGISTRY,'#9
      #9'TRN.REMARKS,'
      #9'TRN.OPERATOR_ID,'
      #9'TRN.STATUS,'
      '                TRN.REMARKS_ADD'
      'from  TRANSMITTERS TRN'
      'where TRN.ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS'
      'set'
      '    STAND_ID =          :STAND_ID,'
      #9'DATECHANGE =        :DATECHANGE,'
      #9'OWNER_ID =          :OWNER_ID,'
      #9'RESPONSIBLEADMIN =  :RESPONSIBLEADMIN,'
      #9'ACCOUNTCONDITION_IN =  :ACCOUNTCONDITION_IN,'
      #9'ACCOUNTCONDITION_OUT = :ACCOUNTCONDITION_OUT,'
      #9'CLASSWAVE =         :CLASSWAVE,'
      #9'TIMETRANSMIT =      :TIMETRANSMIT,'
      #9'NAMEPROGRAMM  =     :NAMEPROGRAMM,'
      #9'USERID = '#9#9'    :USERID,'
      #9'ORIGINALID = '#9#9':ORIGINALID,'
      #9'NUMREGISTRY = '#9#9':NUMREGISTRY,'
      #9'TYPEREGISTRY = '#9#9':TYPEREGISTRY,'
      #9'REMARKS ='#9#9'    :REMARKS,'
      '    REMARKS_ADD =       :REMARKS_ADD,'
      #9'OPERATOR_ID = '#9#9':OPERATOR_ID'
      ''
      'where ID = :ID')
    Left = 424
    Top = 56
    object ibdsStantionsBaseID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object ibdsStantionsBaseSTAND_ID: TIntegerField
      FieldName = 'STAND_ID'
      Origin = 'TRANSMITTERS.STAND_ID'
    end
    object ibdsStantionsBaseDATECREATE: TDateField
      FieldName = 'DATECREATE'
      Origin = 'TRANSMITTERS.DATECREATE'
    end
    object ibdsStantionsBaseDATECHANGE: TDateField
      FieldName = 'DATECHANGE'
      Origin = 'TRANSMITTERS.DATECHANGE'
    end
    object ibdsStantionsBaseOWNER_ID: TIntegerField
      FieldName = 'OWNER_ID'
      Origin = 'TRANSMITTERS.OWNER_ID'
    end
    object ibdsStantionsBaseSYSTEMCAST_ID: TIntegerField
      FieldName = 'SYSTEMCAST_ID'
      Origin = 'TRANSMITTERS.SYSTEMCAST_ID'
      Required = True
    end
    object ibdsStantionsBaseNAMEPROGRAMM: TIBStringField
      FieldName = 'NAMEPROGRAMM'
      Origin = 'TRANSMITTERS.NAMEPROGRAMM'
      Size = 16
    end
    object ibdsStantionsBaseTIMETRANSMIT: TIBStringField
      FieldName = 'TIMETRANSMIT'
      Origin = 'TRANSMITTERS.TIMETRANSMIT'
      Size = 256
    end
    object ibdsStantionsBaseUSERID: TIntegerField
      FieldName = 'USERID'
      Origin = 'TRANSMITTERS.USERID'
      Required = True
    end
    object ibdsStantionsBaseORIGINALID: TIntegerField
      FieldName = 'ORIGINALID'
      Origin = 'TRANSMITTERS.ORIGINALID'
    end
    object ibdsStantionsBaseNUMREGISTRY: TIBStringField
      FieldName = 'NUMREGISTRY'
      Origin = 'TRANSMITTERS.NUMREGISTRY'
      Size = 16
    end
    object ibdsStantionsBaseTYPEREGISTRY: TIBStringField
      FieldName = 'TYPEREGISTRY'
      Origin = 'TRANSMITTERS.TYPEREGISTRY'
      Size = 16
    end
    object ibdsStantionsBaseREMARKS: TIBStringField
      FieldName = 'REMARKS'
      Origin = 'TRANSMITTERS.REMARKS'
      Size = 512
    end
    object ibdsStantionsBaseUSER_NAME: TStringField
      FieldKind = fkLookup
      FieldName = 'USER_NAME'
      LookupDataSet = ibqUserName
      LookupKeyFields = 'ID'
      LookupResultField = 'NAME'
      KeyFields = 'USERID'
      Size = 32
      Lookup = True
    end
    object ibdsStantionsBaseTRK_NAME: TStringField
      FieldKind = fkLookup
      FieldName = 'TRK_NAME'
      LookupDataSet = ibqTRKName
      LookupKeyFields = 'ID'
      LookupResultField = 'NAMEORGANIZATION'
      KeyFields = 'OWNER_ID'
      Size = 32
      Lookup = True
    end
    object ibdsStantionsBaseRESPONSIBLEADMIN: TIBStringField
      FieldName = 'RESPONSIBLEADMIN'
      Origin = 'TRANSMITTERS.RESPONSIBLEADMIN'
      Size = 4
    end
    object ibdsStantionsBaseCLASSWAVE: TIBStringField
      FieldName = 'CLASSWAVE'
      Origin = 'TRANSMITTERS.CLASSWAVE'
      Size = 4
    end
    object ibdsStantionsBaseSYSTEMCAST_NAME: TStringField
      FieldKind = fkLookup
      FieldName = 'SYSTEMCAST_NAME'
      LookupDataSet = ibqSystemCastName
      LookupKeyFields = 'ID'
      LookupResultField = 'CODE'
      KeyFields = 'SYSTEMCAST_ID'
      Size = 4
      Lookup = True
    end
    object ibdsStantionsBaseACCOUNTCONDITION_IN: TSmallintField
      FieldName = 'ACCOUNTCONDITION_IN'
      Origin = 'TRANSMITTERS.ACCOUNTCONDITION_IN'
      Required = True
    end
    object ibdsStantionsBaseADMINISTRATIONID: TIBStringField
      Alignment = taRightJustify
      FieldName = 'ADMINISTRATIONID'
      Origin = 'TRANSMITTERS.ADMINISTRATIONID'
      Size = 4
    end
    object ibdsStantionsBaseACCOUNTCONDITION_OUT: TSmallintField
      FieldName = 'ACCOUNTCONDITION_OUT'
      Origin = 'TRANSMITTERS.ACCOUNTCONDITION_OUT'
      Required = True
    end
    object ibdsStantionsBaseOPERATOR_ID: TIntegerField
      FieldName = 'OPERATOR_ID'
      Origin = 'TRANSMITTERS.OPERATOR_ID'
    end
    object ibdsStantionsBaseSTATUS: TSmallintField
      FieldName = 'STATUS'
      Origin = 'TRANSMITTERS.STATUS'
    end
    object ibdsStantionsBaseOPERATOR_NAME: TStringField
      FieldKind = fkLookup
      FieldName = 'OPERATOR_NAME'
      LookupDataSet = ibqOwner
      LookupKeyFields = 'ID'
      LookupResultField = 'NAMEORGANIZATION'
      KeyFields = 'OPERATOR_ID'
      Size = 256
      Lookup = True
    end
    object ibdsStantionsBaseREMARKS_ADD: TIBStringField
      FieldName = 'REMARKS_ADD'
      Origin = 'TRANSMITTERS.REMARKS_ADD'
      Size = 512
    end
  end
  object dsStantionsBase: TDataSource
    DataSet = ibdsStantionsBase
    Left = 454
    Top = 56
  end
  object ibdsLicenses: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsLicensesAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select     TX.ID,'
      '    TX.LICENSE_RFR_ID,'
      '    L_RFR.NUMLICENSE L_RFR_NUMLICENSE, '
      '    L_RFR.DATEFROM L_RFR_DATEFROM, '
      '    L_RFR.DATETO L_RFR_DATETO, '
      '    L_RFR_O.NAMEORGANIZATION L_RFR_O_NAME,'
      ''
      '    TX.EMC_CONCL_NUM,'
      '    TX.EMC_CONCL_FROM,'
      '    TX.EMC_CONCL_TO,'
      ''
      '    TX.NUMPERMUSE,'
      '    TX.DATEPERMUSEFROM, '
      '    TX.DATEPERMUSETO, '
      '    TX.PERMUSE_OWNER_ID,'
      '    PERMUSE_O.NAMEORGANIZATION PERMUSE_O_NAME,'
      ''
      '    TX.NUMSTANDCERTIFICATE, '
      '    TX.NUMFACTORY, '
      '    TX.DATEINTENDUSE, '
      '    TX.DATESTANDCERTIFICATE,'
      ''
      '    tx.NR_REQ_NO,'
      '    tx.NR_REQ_DATE,'
      '    tx.NR_CONCL_NO,'
      '    tx.NR_CONCL_DATE,'
      '    tx.NR_APPL_NO,'
      '    tx.NR_APPL_DATE,'
      ''
      '    tx.op_hh_fr,'
      '    tx.op_hh_to,'
      '    tx.D_EXPIRY, '
      '    tx.REMARK_CONDS_MET, '
      '    tx.IS_RESUB, '
      '    tx.SIGNED_COMMITMENT'
      ''
      'from TRANSMITTERS TX'
      'left outer join  LICENSE L_RFR on (TX.LICENSE_RFR_ID = L_RFR.ID)'
      'left outer join  OWNER L_RFR_O on (L_RFR.OWNER_ID = L_RFR_O.ID)'
      
        'left outer join  OWNER PERMUSE_O on (TX.PERMUSE_OWNER_ID = PERMU' +
        'SE_O.ID)'
      'where TX.ID = :ID')
    SelectSQL.Strings = (
      'select     TX.ID,'
      '    TX.LICENSE_RFR_ID,'
      '    L_RFR.NUMLICENSE L_RFR_NUMLICENSE, '
      '    L_RFR.DATEFROM L_RFR_DATEFROM, '
      '    L_RFR.DATETO L_RFR_DATETO, '
      '    L_RFR_O.NAMEORGANIZATION L_RFR_O_NAME,'
      ''
      '    TX.EMC_CONCL_NUM,'
      '    TX.EMC_CONCL_FROM,'
      '    TX.EMC_CONCL_TO,'
      ''
      '    TX.NUMPERMUSE,'
      '    TX.DATEPERMUSEFROM, '
      '    TX.DATEPERMUSETO, '
      '    TX.PERMUSE_OWNER_ID,'
      '    PERMUSE_O.NAMEORGANIZATION PERMUSE_O_NAME,'
      ''
      '    TX.NUMSTANDCERTIFICATE, '
      '    TX.NUMFACTORY, '
      '    TX.DATEINTENDUSE, '
      '    TX.DATESTANDCERTIFICATE,'
      ''
      '    tx.NR_REQ_NO,'
      '    tx.NR_REQ_DATE,'
      '    tx.NR_CONCL_NO,'
      '    tx.NR_CONCL_DATE,'
      '    tx.NR_APPL_NO,'
      '    tx.NR_APPL_DATE,'
      ''
      '    tx.op_hh_fr,'
      '    tx.op_hh_to,'
      '    tx.D_EXPIRY, '
      '    tx.REMARK_CONDS_MET, '
      '    tx.IS_RESUB, '
      '    tx.SIGNED_COMMITMENT'
      ''
      'from TRANSMITTERS TX'
      'left outer join  LICENSE L_RFR on (TX.LICENSE_RFR_ID = L_RFR.ID)'
      'left outer join  OWNER L_RFR_O on (L_RFR.OWNER_ID = L_RFR_O.ID)'
      
        'left outer join  OWNER PERMUSE_O on (TX.PERMUSE_OWNER_ID = PERMU' +
        'SE_O.ID)'
      'where TX.ID = :ID'
      '')
    ModifySQL.Strings = (
      'update'#9'TRANSMITTERS set'
      '    LICENSE_RFR_ID = :LICENSE_RFR_ID,'
      ''
      '    EMC_CONCL_NUM     = :EMC_CONCL_NUM,'
      '    EMC_CONCL_FROM    = :EMC_CONCL_FROM,'
      '    EMC_CONCL_TO      = :EMC_CONCL_TO,'
      ''
      '    PERMUSE_OWNER_ID  = :PERMUSE_OWNER_ID,'
      ''
      '    NR_REQ_NO         = :NR_REQ_NO,'
      '    NR_REQ_DATE       = :NR_REQ_DATE,'
      '    NR_CONCL_NO       = :NR_CONCL_NO,'
      '    NR_CONCL_DATE     = :NR_CONCL_DATE,'
      '    NR_APPL_NO        = :NR_APPL_NO,'
      '    NR_APPL_DATE      = :NR_APPL_DATE,'
      ''
      #9'NUMPERMUSE = :NUMPERMUSE,'
      #9'DATEPERMUSEFROM = :DATEPERMUSEFROM,'
      #9'DATEPERMUSETO = :DATEPERMUSETO,'
      #9'NUMSTANDCERTIFICATE = :NUMSTANDCERTIFICATE,'
      #9'NUMFACTORY = :NUMFACTORY,'
      #9'DATEINTENDUSE = :DATEINTENDUSE,'
      #9'DATESTANDCERTIFICATE  = :DATESTANDCERTIFICATE,'
      ''
      '    op_hh_fr = :op_hh_fr,'
      '    op_hh_to = :op_hh_to,'
      '    D_EXPIRY = :D_EXPIRY, '
      '    REMARK_CONDS_MET = :REMARK_CONDS_MET, '
      '    IS_RESUB = :IS_RESUB, '
      '    SIGNED_COMMITMENT = :SIGNED_COMMITMENT'
      ''
      ''
      'where ID = :ID'
      '')
    Left = 328
    Top = 464
    object ibdsLicensesNUMPERMUSE: TIBStringField
      FieldName = 'NUMPERMUSE'
      Origin = 'TRANSMITTERS.NUMPERMUSE'
      Size = 64
    end
    object ibdsLicensesDATEPERMUSEFROM: TDateField
      FieldName = 'DATEPERMUSEFROM'
      Origin = 'TRANSMITTERS.DATEPERMUSEFROM'
    end
    object ibdsLicensesDATEPERMUSETO: TDateField
      FieldName = 'DATEPERMUSETO'
      Origin = 'TRANSMITTERS.DATEPERMUSETO'
    end
    object ibdsLicensesNUMSTANDCERTIFICATE: TIBStringField
      FieldName = 'NUMSTANDCERTIFICATE'
      Origin = 'TRANSMITTERS.NUMSTANDCERTIFICATE'
      Size = 16
    end
    object ibdsLicensesNUMFACTORY: TIBStringField
      FieldName = 'NUMFACTORY'
      Origin = 'TRANSMITTERS.NUMFACTORY'
      Size = 16
    end
    object ibdsLicensesDATEINTENDUSE: TDateField
      FieldName = 'DATEINTENDUSE'
      Origin = 'TRANSMITTERS.DATEINTENDUSE'
    end
    object ibdsLicensesDATESTANDCERTIFICATE: TDateField
      FieldName = 'DATESTANDCERTIFICATE'
      Origin = 'TRANSMITTERS.DATESTANDCERTIFICATE'
    end
    object ibdsLicensesID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object ibdsLicensesLICENSE_RFR_ID: TIntegerField
      FieldName = 'LICENSE_RFR_ID'
      Origin = 'TRANSMITTERS.LICENSE_RFR_ID'
    end
    object ibdsLicensesL_RFR_NUMLICENSE: TIBStringField
      FieldName = 'L_RFR_NUMLICENSE'
      Origin = 'LICENSE.NUMLICENSE'
      Size = 64
    end
    object ibdsLicensesL_RFR_DATEFROM: TDateField
      FieldName = 'L_RFR_DATEFROM'
      Origin = 'LICENSE.DATEFROM'
    end
    object ibdsLicensesL_RFR_DATETO: TDateField
      FieldName = 'L_RFR_DATETO'
      Origin = 'LICENSE.DATETO'
    end
    object ibdsLicensesL_RFR_O_NAME: TIBStringField
      FieldName = 'L_RFR_O_NAME'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
    object ibdsLicensesEMC_CONCL_NUM: TIBStringField
      FieldName = 'EMC_CONCL_NUM'
      Origin = 'TRANSMITTERS.EMC_CONCL_NUM'
      Size = 16
    end
    object ibdsLicensesEMC_CONCL_FROM: TDateField
      FieldName = 'EMC_CONCL_FROM'
      Origin = 'TRANSMITTERS.EMC_CONCL_FROM'
    end
    object ibdsLicensesEMC_CONCL_TO: TDateField
      FieldName = 'EMC_CONCL_TO'
      Origin = 'TRANSMITTERS.EMC_CONCL_TO'
    end
    object ibdsLicensesPERMUSE_OWNER_ID: TIntegerField
      FieldName = 'PERMUSE_OWNER_ID'
      Origin = 'TRANSMITTERS.PERMUSE_OWNER_ID'
    end
    object ibdsLicensesPERMUSE_O_NAME: TIBStringField
      FieldName = 'PERMUSE_O_NAME'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
    object ibdsLicensesNR_REQ_NO: TIBStringField
      FieldName = 'NR_REQ_NO'
      Origin = 'TRANSMITTERS.NR_REQ_NO'
      Size = 16
    end
    object ibdsLicensesNR_REQ_DATE: TDateField
      FieldName = 'NR_REQ_DATE'
      Origin = 'TRANSMITTERS.NR_REQ_DATE'
    end
    object ibdsLicensesNR_CONCL_NO: TIBStringField
      FieldName = 'NR_CONCL_NO'
      Origin = 'TRANSMITTERS.NR_CONCL_NO'
      Size = 16
    end
    object ibdsLicensesNR_CONCL_DATE: TDateField
      FieldName = 'NR_CONCL_DATE'
      Origin = 'TRANSMITTERS.NR_CONCL_DATE'
    end
    object ibdsLicensesNR_APPL_NO: TIBStringField
      FieldName = 'NR_APPL_NO'
      Origin = 'TRANSMITTERS.NR_APPL_NO'
      Size = 16
    end
    object ibdsLicensesNR_APPL_DATE: TDateField
      FieldName = 'NR_APPL_DATE'
      Origin = 'TRANSMITTERS.NR_APPL_DATE'
    end
    object ibdsLicensesOP_HH_FR: TTimeField
      FieldName = 'OP_HH_FR'
      Origin = 'TRANSMITTERS.OP_HH_FR'
      DisplayFormat = 'hh:nn:ss'
    end
    object ibdsLicensesOP_HH_TO: TTimeField
      FieldName = 'OP_HH_TO'
      Origin = 'TRANSMITTERS.OP_HH_TO'
      DisplayFormat = 'hh:nn:ss'
    end
    object ibdsLicensesD_EXPIRY: TDateField
      FieldName = 'D_EXPIRY'
      Origin = 'TRANSMITTERS.D_EXPIRY'
    end
    object ibdsLicensesREMARK_CONDS_MET: TSmallintField
      FieldName = 'REMARK_CONDS_MET'
      Origin = 'TRANSMITTERS.REMARK_CONDS_MET'
    end
    object ibdsLicensesIS_RESUB: TSmallintField
      FieldName = 'IS_RESUB'
      Origin = 'TRANSMITTERS.IS_RESUB'
    end
    object ibdsLicensesSIGNED_COMMITMENT: TSmallintField
      FieldName = 'SIGNED_COMMITMENT'
      Origin = 'TRANSMITTERS.SIGNED_COMMITMENT'
    end
  end
  object dsLicenses: TDataSource
    DataSet = ibdsLicenses
    Left = 356
    Top = 464
  end
  object ibqAccCondNameIn: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, CODE, NAME from ACCOUNTCONDITION'
      'where TYPECONDITION = 0')
    Left = 112
    Top = 432
    object ibqAccCondNameInID: TIntegerField
      FieldName = 'ID'
      Origin = 'ACCOUNTCONDITION.ID'
      Required = True
    end
    object ibqAccCondNameInCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'ACCOUNTCONDITION.CODE'
      Size = 4
    end
    object ibqAccCondNameInNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'ACCOUNTCONDITION.NAME'
      Size = 32
    end
  end
  object ibqAccCondNameOut: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, CODE, NAME from ACCOUNTCONDITION'
      'where TYPECONDITION = 1')
    Left = 120
    Top = 448
    object ibqAccCondNameOutID: TIntegerField
      FieldName = 'ID'
      Origin = 'ACCOUNTCONDITION.ID'
      Required = True
    end
    object ibqAccCondNameOutNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'ACCOUNTCONDITION.NAME'
      Size = 32
    end
    object ibqAccCondNameOutCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'ACCOUNTCONDITION.CODE'
      Size = 4
    end
  end
  object dsStand: TDataSource
    DataSet = ibqStand
    Left = 4
    Top = 360
  end
  object ibqUserName: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAME from ADMIT')
    Left = 168
    Top = 464
    object ibqUserNameID: TIntegerField
      FieldName = 'ID'
      Origin = 'ADMIT.ID'
      Required = True
    end
    object ibqUserNameNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'ADMIT.NAME'
      Size = 64
    end
  end
  object ibqTRKName: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAMEORGANIZATION from OWNER')
    Left = 204
    Top = 464
    object ibqTRKNameID: TIntegerField
      FieldName = 'ID'
      Origin = 'OWNER.ID'
      Required = True
    end
    object ibqTRKNameNAMEORGANIZATION: TIBStringField
      FieldName = 'NAMEORGANIZATION'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
  end
  object ibqSystemCastName: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, CODE from SYSTEMCAST')
    Left = 240
    Top = 464
    object ibqSystemCastNameID: TIntegerField
      FieldName = 'ID'
      Origin = 'SYSTEMCAST.ID'
      Required = True
    end
    object ibqSystemCastNameCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'SYSTEMCAST.CODE'
      Size = 4
    end
  end
  object ibdsTelecomOrg: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsTelecomOrgAfterEdit
    AfterScroll = ibdsTelecomOrgAfterScroll
    BeforeDelete = ibdsTelecomOrgBeforeDelete
    BeforePost = ibdsTelecomOrgBeforePost
    BufferChunks = 1000
    CachedUpdates = False
    DeleteSQL.Strings = (
      'delete from COORDINATION where ID = :ID')
    InsertSQL.Strings = (
      'INSERT INTO COORDINATION ( ID,'
      #9'TRANSMITTER_ID,'
      #9'TELECOMORG_ID,'
      #9'ACCOUNTCONDITION_ID)'
      'VALUES ('
      #9':ID,'
      #9':TRANSMITTER_ID,'
      #9':TELECOMORG_ID,'
      #9':ACCOUNTCONDITION_ID)')
    RefreshSQL.Strings = (
      'select     '#9'   COO.ID, '
      #9'    COO.TRANSMITTER_ID,'
      '    COO.TELECOMORG_ID,'
      '    COO.ACCOUNTCONDITION_ID,'
      '    ORG.CODE,'
      '    ORG.NAME,'
      '    AC.NAME AC_NAME'
      'from  COORDINATION COO'
      
        'left outer join TELECOMORGANIZATION ORG on (ORG.ID = COO.TELECOM' +
        'ORG_ID)'
      
        'left outer join ACCOUNTCONDITION AC on (AC.ID = COO.ACCOUNTCONDI' +
        'TION_ID)'
      'where COO.TRANSMITTER_ID = :TRANSMITTER_ID')
    SelectSQL.Strings = (
      'select     '#9'   COO.ID, '
      #9'    COO.TRANSMITTER_ID,'
      '    COO.TELECOMORG_ID,'
      '    COO.ACCOUNTCONDITION_ID,'
      '    ORG.CODE,'
      '    ORG.NAME,'
      '    AC.NAME AC_NAME'
      'from  COORDINATION COO'
      
        'left outer join TELECOMORGANIZATION ORG on (ORG.ID = COO.TELECOM' +
        'ORG_ID)'
      
        'left outer join ACCOUNTCONDITION AC on (AC.ID = COO.ACCOUNTCONDI' +
        'TION_ID)'
      'where COO.TRANSMITTER_ID = :TRANSMITTER_ID')
    ModifySQL.Strings = (
      'UPDATE COORDINATION SET '
      #9'TRANSMITTER_ID = :TRANSMITTER_ID,'
      #9'TELECOMORG_ID = :TELECOMORG_ID,'
      #9'ACCOUNTCONDITION_ID = :ACCOUNTCONDITION_ID'
      'WHERE (ID = :ID)')
    Left = 4
    Top = 328
    object ibdsTelecomOrgID: TIntegerField
      FieldName = 'ID'
      Origin = 'COORDINATION.ID'
      Required = True
      Visible = False
    end
    object ibdsTelecomOrgTRANSMITTER_ID: TIntegerField
      FieldName = 'TRANSMITTER_ID'
      Origin = 'COORDINATION.TRANSMITTER_ID'
      Required = True
      Visible = False
    end
    object ibdsTelecomOrgTELECOMORG_ID: TIntegerField
      FieldName = 'TELECOMORG_ID'
      Origin = 'COORDINATION.TELECOMORG_ID'
      Required = True
      Visible = False
    end
    object ibdsTelecomOrgACCOUNTCONDITION_ID: TIntegerField
      FieldName = 'ACCOUNTCONDITION_ID'
      Origin = 'COORDINATION.ACCOUNTCONDITION_ID'
      Required = True
      Visible = False
    end
    object ibdsTelecomOrgCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'TELECOMORGANIZATION.CODE'
      Size = 4
    end
    object ibdsTelecomOrgNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'TELECOMORGANIZATION.NAME'
      Size = 32
    end
    object ibdsTelecomOrgAC_NAME: TIBStringField
      DisplayLabel = #1057#1086#1089#1090#1086#1103#1085#1080#1077
      FieldName = 'AC_NAME'
      Origin = 'ACCOUNTCONDITION.NAME'
      Visible = False
      Size = 32
    end
  end
  object dsTelecomOrg: TDataSource
    DataSet = ibdsTelecomOrg
    Left = 36
    Top = 324
  end
  object dsDocuments: TDataSource
    DataSet = ibdsDocuments
    Left = 252
    Top = 472
  end
  object ibdsDocuments: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterScroll = ibdsDocumentsAfterScroll
    BufferChunks = 1000
    CachedUpdates = False
    DeleteSQL.Strings = (
      'delete from LETTERS where ID = :ID')
    RefreshSQL.Strings = (
      'SELECT l.ID,'
      '             l.TRANSMITTERS_ID,'
      '             l.TYPELETTER,'
      '             l.LETTERS_ID,'
      '             l.ACCOUNTCONDITION_ID,'
      '             l.TELECOMORGANIZATION_ID,'
      '             l.DOCUMENT_ID,'
      '             l.CREATEDATEIN,'
      '             l.NUMIN,'
      '             l.CREATEDATEOUT,'
      '             l.NUMOUT,'
      '             l.ANSWERDATE,'
      '             l.ANSWERIS,'
      '             AC.NAME AC_NAME,'
      '             DT.NAME DT_NAME'
      'FROM LETTERS l'
      'left join ACCOUNTCONDITION ac on l.ACCOUNTCONDITION_ID = ac.ID'
      'left join DOCUMENT dt on l.DOCUMENT_ID = dt.ID'
      'where l.ID = :ID')
    SelectSQL.Strings = (
      'SELECT l.ID,'
      '             l.TRANSMITTERS_ID,'
      '             l.TYPELETTER,'
      '             l.LETTERS_ID,'
      '             l.ACCOUNTCONDITION_ID,'
      '             l.TELECOMORGANIZATION_ID,'
      '             l.DOCUMENT_ID,'
      '             l.CREATEDATEIN,'
      '             l.NUMIN,'
      '             l.CREATEDATEOUT,'
      '             l.NUMOUT,'
      '             l.ANSWERDATE,'
      '             l.ANSWERIS,'
      '             AC.NAME AC_NAME,'
      '             DT.NAME DT_NAME'
      'FROM LETTERS l'
      'left join ACCOUNTCONDITION ac on l.ACCOUNTCONDITION_ID = ac.ID'
      'left join DOCUMENT dt on l.DOCUMENT_ID = dt.ID'
      'where TRANSMITTERS_ID = :TRANSMITTERS_ID and'
      'TELECOMORGANIZATION_ID = :TELECOMORGANIZATION_ID')
    OnFilterRecord = ibdsDocumentsFilterRecord
    Left = 224
    Top = 478
    object ibdsDocumentsID: TIntegerField
      FieldName = 'ID'
      Origin = 'LETTERS.ID'
      Required = True
      Visible = False
    end
    object ibdsDocumentsTELECOMORGANIZATION_ID: TIntegerField
      FieldName = 'TELECOMORGANIZATION_ID'
      Origin = 'LETTERS.TELECOMORGANIZATION_ID'
      Required = True
      Visible = False
    end
    object ibdsDocumentsTRANSMITTERS_ID: TIntegerField
      FieldName = 'TRANSMITTERS_ID'
      Origin = 'LETTERS.LETTERS_ID'
      Required = True
      Visible = False
    end
    object ibdsDocumentsTYPELETTER: TSmallintField
      DisplayLabel = #1058#1080#1087
      DisplayWidth = 7
      FieldName = 'TYPELETTER'
      Origin = 'LETTERS.TYPELETTER'
      OnGetText = ibdsDocumentsTYPELETTERGetText
    end
    object ibdsDocumentsACCOUNTCONDITION_ID: TIntegerField
      FieldName = 'ACCOUNTCONDITION_ID'
      Origin = 'LETTERS.ACCOUNTCONDITION_ID'
      Required = True
      Visible = False
    end
    object ibdsDocumentsCREATEDATEIN: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1074#1093
      DisplayWidth = 13
      FieldName = 'CREATEDATEIN'
      Origin = 'LETTERS.CREATEDATEIN'
    end
    object ibdsDocumentsCREATEDATEOUT: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1074#1080#1093
      DisplayWidth = 15
      FieldName = 'CREATEDATEOUT'
      Origin = 'LETTERS.CREATEDATEOUT'
    end
    object ibdsDocumentsNUMIN: TIntegerField
      DisplayLabel = #8470' '#1074#1093
      DisplayWidth = 10
      FieldName = 'NUMIN'
      Origin = 'LETTERS.DOCUMENT_ID'
    end
    object ibdsDocumentsNUMOUT: TIntegerField
      DisplayLabel = #8470' '#1074#1080#1093
      DisplayWidth = 12
      FieldName = 'NUMOUT'
      Origin = 'LETTERS.NUMOUT'
    end
    object ibdsDocumentsANSWERDATE: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1074#1110#1076#1087
      DisplayWidth = 10
      FieldName = 'ANSWERDATE'
      Origin = 'LETTERS.ANSWERDATE'
    end
    object ibdsDocumentsANSWERIS: TSmallintField
      DisplayLabel = #1042#1110#1076#1087#1086#1074#1110#1076#1100
      DisplayWidth = 18
      FieldName = 'ANSWERIS'
      Origin = 'LETTERS.ANSWERIS'
      OnGetText = ibdsDocumentsANSWERISGetText
    end
    object ibdsDocumentsLETTERS_ID: TIntegerField
      DisplayLabel = #1042#1093#1110#1076#1085#1077
      FieldName = 'LETTERS_ID'
      Origin = 'LETTERS.LETTERS_ID'
    end
    object ibdsDocumentsDOCUMENT_ID: TIntegerField
      FieldName = 'DOCUMENT_ID'
      Origin = 'LETTERS.DOCUMENT_ID'
      Required = True
    end
    object ibdsDocumentsAC_NAME: TIBStringField
      DisplayLabel = #1057#1090#1072#1085
      FieldName = 'AC_NAME'
      Size = 32
    end
    object ibdsDocumentsDT_NAME: TIBStringField
      DisplayLabel = #1064#1072#1073#1083#1086#1085
      FieldName = 'DT_NAME'
      Size = 64
    end
  end
  object ibqDocType: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAME from DOCUMENT')
    Left = 272
    Top = 464
    object ibqDocTypeID: TIntegerField
      FieldName = 'ID'
      Origin = 'DOCUMENT.ID'
      Required = True
    end
    object ibqDocTypeNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'DOCUMENT.NAME'
      Size = 64
    end
  end
  object dsAccCondNameOut: TDataSource
    DataSet = ibqAccCondNameOut
    Left = 8
    Top = 456
  end
  object dsAccCondNameIn: TDataSource
    DataSet = ibqAccCondNameIn
    Left = 36
    Top = 364
  end
  object ibdsEquipment: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsEquipmentAfterEdit
    BeforePost = ibdsEquipmentBeforePost
    BufferChunks = 1000
    CachedUpdates = False
    DeleteSQL.Strings = (
      'delete from TRANSMITTEREQUIPMENT'
      'where '#9'ID = :ID')
    InsertSQL.Strings = (
      'insert into TRANSMITTEREQUIPMENT( '
      #9'ID,'
      #9'EQUIPMENT_ID, '
      #9'TRANSMITTERS_ID, '
      #9'DATESTANDCERTIFICATE, '
      #9'NUMSTANDCERTIFICATE, '
      #9'NUMFACTORY)'
      'values ('
      #9':ID,'
      #9':EQUIPMENT_ID, '
      #9':TRANSMITTERS_ID, '
      #9':DATESTANDCERTIFICATE, '
      #9':NUMSTANDCERTIFICATE, '
      #9':NUMFACTORY)')
    RefreshSQL.Strings = (
      'select '#9'te.ID,'
      #9'te.EQUIPMENT_ID, '
      #9'te.TRANSMITTERS_ID, '
      #9'te.DATESTANDCERTIFICATE, '
      #9'te.NUMSTANDCERTIFICATE, '
      #9'te.NUMFACTORY,'
      #9'eq.NAME, '
      #9'eq.TYPEEQUIPMENT, '
      #9'eq.MANUFACTURE'
      'from TRANSMITTEREQUIPMENT te'
      'left outer join EQUIPMENT eq on (eq.ID = te.EQUIPMENT_ID)'
      'where te.'#9'TRANSMITTERS_ID = :ID  and '
      #9'EQUIPMENT_ID = :EQUIPMENT_ID')
    SelectSQL.Strings = (
      'select '#9'te.ID,'
      #9'te.EQUIPMENT_ID, '
      #9'te.TRANSMITTERS_ID, '
      #9'te.DATESTANDCERTIFICATE, '
      #9'te.NUMSTANDCERTIFICATE, '
      #9'te.NUMFACTORY,'
      #9'eq.NAME, '
      #9'eq.TYPEEQUIPMENT, '
      #9'eq.MANUFACTURE'
      'from TRANSMITTEREQUIPMENT te'
      'left outer join EQUIPMENT eq on (eq.ID = te.EQUIPMENT_ID)'
      'where te.TRANSMITTERS_ID = :ID ')
    ModifySQL.Strings = (
      'update  '#9'TRANSMITTEREQUIPMENT set '
      #9'DATESTANDCERTIFICATE = :DATESTANDCERTIFICATE, '
      #9'NUMSTANDCERTIFICATE = :NUMSTANDCERTIFICATE, '
      #9'NUMFACTORY = :NUMFACTORY'
      'where '#9'ID = :ID ')
    Left = 328
    Top = 496
    object ibdsEquipmentEQUIPMENT_ID: TIntegerField
      FieldName = 'EQUIPMENT_ID'
      Origin = 'TRANSMITTEREQUIPMENT.EQUIPMENT_ID'
      Required = True
    end
    object ibdsEquipmentTRANSMITTERS_ID: TIntegerField
      FieldName = 'TRANSMITTERS_ID'
      Origin = 'TRANSMITTEREQUIPMENT.TRANSMITTERS_ID'
      Required = True
    end
    object ibdsEquipmentDATESTANDCERTIFICATE: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1089#1077#1088#1090#1080#1092#1110#1082#1072#1094#1110#1111
      FieldName = 'DATESTANDCERTIFICATE'
      Origin = 'TRANSMITTEREQUIPMENT.DATESTANDCERTIFICATE'
    end
    object ibdsEquipmentNUMSTANDCERTIFICATE: TIBStringField
      DisplayLabel = #1053#1086#1084#1077#1088' '#1089#1090#1072#1085#1076#1072#1088#1090#1091' '#1089#1077#1088#1090#1080#1092#1080#1082#1072#1094#1110#1111
      FieldName = 'NUMSTANDCERTIFICATE'
      Origin = 'TRANSMITTEREQUIPMENT.NUMSTANDCERTIFICATE'
      Size = 16
    end
    object ibdsEquipmentNUMFACTORY: TIBStringField
      DisplayLabel = #1047#1072#1074#1086#1076#1089#1100#1082#1080#1081' '#1085#1086#1084#1077#1088
      FieldName = 'NUMFACTORY'
      Origin = 'TRANSMITTEREQUIPMENT.NUMFACTORY'
      Size = 16
    end
    object ibdsEquipmentNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'EQUIPMENT.NAME'
      ReadOnly = True
      Size = 32
    end
    object ibdsEquipmentTYPEEQUIPMENT: TIBStringField
      DisplayLabel = #1058#1080#1087
      FieldName = 'TYPEEQUIPMENT'
      Origin = 'EQUIPMENT.TYPEEQUIPMENT'
      ReadOnly = True
      Size = 64
    end
    object ibdsEquipmentMANUFACTURE: TIBStringField
      DisplayLabel = #1042#1080#1088#1086#1073#1085#1080#1082
      FieldName = 'MANUFACTURE'
      Origin = 'EQUIPMENT.MANUFACTURE'
      ReadOnly = True
      Size = 32
    end
    object ibdsEquipmentID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTEREQUIPMENT.ID'
      Required = True
    end
  end
  object dsEquipment: TDataSource
    DataSet = ibdsEquipment
    Left = 356
    Top = 496
  end
  object dsTestpoints: TDataSource
    DataSet = ibdsTestpoint
    Left = 108
    Top = 340
  end
  object ibdsTestpoint: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsTestpointAfterEdit
    AfterInsert = ibdsTestpointAfterInsert
    BeforePost = ibdsTestpointBeforePost
    BufferChunks = 1000
    CachedUpdates = False
    InsertSQL.Strings = (
      'insert into TESTPOINTS ('
      #9'ID, '
      #9'TRANSMITTERS_ID, '
      #9'NAME, '
      #9'LATITUDE, '
      #9'LONGITUDE, '
      #9'TESTPOINT_TYPE,'
      #9'BEARING, '
      #9'DISTANCE, '
      #9'USEBLEFIELD, '
      #9'PROTECTEDFIELD)'
      'values'#9'(:ID, '
      #9':TRANSMITTERS_ID, '
      #9':NAME, '
      #9':LATITUDE, '
      #9':LONGITUDE, '
      #9':TESTPOINT_TYPE,'
      #9':BEARING, '
      #9':DISTANCE, '
      #9':USEBLEFIELD, '
      #9':PROTECTEDFIELD)')
    RefreshSQL.Strings = (
      'select '
      #9'ID, '
      #9'TRANSMITTERS_ID, '
      #9'NAME, '
      #9'LATITUDE, '
      #9'LONGITUDE, '
      #9'TESTPOINT_TYPE,'
      #9'BEARING, '
      #9'DISTANCE, '
      #9'USEBLEFIELD, '
      #9'PROTECTEDFIELD '
      'from TESTPOINTS '
      'where TRANSMITTERS_ID = :TRANSMITTERS_ID'
      'order by TESTPOINT_TYPE, NAME')
    SelectSQL.Strings = (
      'select '
      #9'ID, '
      #9'TRANSMITTERS_ID, '
      #9'NAME, '
      #9'LATITUDE, '
      #9'LONGITUDE, '
      #9'TESTPOINT_TYPE,'
      #9'BEARING, '
      #9'DISTANCE, '
      #9'USEBLEFIELD, '
      #9'PROTECTEDFIELD '
      'from TESTPOINTS '
      'where TRANSMITTERS_ID = :TRANSMITTERS_ID'
      'order by TESTPOINT_TYPE, NAME')
    ModifySQL.Strings = (
      'update TESTPOINTS set'
      #9'NAME = :NAME, '
      #9'LATITUDE = :LATITUDE, '
      #9'LONGITUDE = :LONGITUDE, '
      #9'TESTPOINT_TYPE = :TESTPOINT_TYPE,'
      #9'BEARING = :BEARING, '
      #9'DISTANCE = :DISTANCE, '
      #9'USEBLEFIELD = :USEBLEFIELD, '
      #9'PROTECTEDFIELD = :PROTECTEDFIELD'
      'where ID = :ID'
      '')
    Left = 68
    Top = 344
    object ibdsTestpointID: TIntegerField
      FieldName = 'ID'
      Origin = 'TESTPOINTS.ID'
      Required = True
      Visible = False
    end
    object ibdsTestpointTRANSMITTERS_ID: TIntegerField
      FieldName = 'TRANSMITTERS_ID'
      Origin = 'TESTPOINTS.TRANSMITTERS_ID'
      Required = True
      Visible = False
    end
    object ibdsTestpointNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'TESTPOINTS.NAME'
      Size = 16
    end
    object ibdsTestpointLATITUDE: TFloatField
      DisplayLabel = #1064#1080#1088#1086#1090#1072
      FieldName = 'LATITUDE'
      Origin = 'TESTPOINTS.LATITUDE'
      Required = True
      OnGetText = ibdsTestpointLATITUDEGetText
      OnSetText = ibdsTestpointLATITUDESetText
    end
    object ibdsTestpointLONGITUDE: TFloatField
      DisplayLabel = #1044#1086#1074#1075#1086#1090#1072
      FieldName = 'LONGITUDE'
      Origin = 'TESTPOINTS.LONGITUDE'
      Required = True
      OnGetText = ibdsTestpointLONGITUDEGetText
      OnSetText = ibdsTestpointLATITUDESetText
    end
    object ibdsTestpointTESTPOINT_TYPE: TSmallintField
      DisplayLabel = #1058#1080#1087
      FieldName = 'TESTPOINT_TYPE'
      Origin = 'TESTPOINTS.TESTPOINT_TYPE'
      OnGetText = ibdsTestpointTESTPOINT_TYPEGetText
      OnSetText = ibdsTestpointTESTPOINT_TYPESetText
    end
    object ibdsTestpointBEARING: TFloatField
      DisplayLabel = #1050#1091#1090
      FieldName = 'BEARING'
      Origin = 'TESTPOINTS.BEARING'
    end
    object ibdsTestpointDISTANCE: TFloatField
      DisplayLabel = #1042#1110#1076#1089#1090#1072#1085#1100
      FieldName = 'DISTANCE'
      Origin = 'TESTPOINTS.DISTANCE'
      OnGetText = ibdsTestpointDISTANCEGetText
      EditFormat = '#.###'
    end
    object ibdsTestpointUSEBLEFIELD: TFloatField
      DisplayLabel = #1053#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103
      FieldName = 'USEBLEFIELD'
      Origin = 'TESTPOINTS.USEBLEFIELD'
    end
    object ibdsTestpointPROTECTEDFIELD: TFloatField
      DisplayLabel = #1047#1072#1093#1080#1089#1085#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103
      FieldName = 'PROTECTEDFIELD'
      Origin = 'TESTPOINTS.PROTECTEDFIELD'
    end
  end
  object ActionList1: TActionList
    Left = 640
    Top = 56
    object actOk: TAction
      Caption = 'Ok'
      OnExecute = actOkExecute
      OnUpdate = actOkUpdate
    end
    object actClose: TAction
      Caption = 'actClose'
      OnExecute = actCloseExecute
    end
    object actApply: TAction
      Caption = 'actApply'
      ShortCut = 16467
      OnExecute = actApplyExecute
    end
    object actLoad: TAction
      Caption = 'actLoad'
      OnExecute = actLoadExecute
    end
    object actIntoProject: TAction
      Caption = #1042' '#1087#1088#1086#1108#1082#1090
      OnExecute = actIntoProjectExecute
    end
    object actIntoBase: TAction
      Caption = #1042' '#1073#1072#1079#1091
      OnExecute = actIntoBaseExecute
    end
    object actIntoarchives: TAction
      Caption = #1042' '#1072#1088#1093#1110#1074
      OnExecute = actIntoarchivesExecute
    end
    object actIntoBeforeBase: TAction
      Caption = #1042' '#1087#1088#1077#1076#1073#1072#1079#1091
      OnExecute = actIntoBeforeBaseExecute
    end
    object actTxCopy: TAction
      Caption = #1050#1086#1087#1110#1103
      OnExecute = actTxCopyExecute
    end
    object actIntoList: TAction
      Caption = #1042' '#1089#1087#1080#1089#1082#1091
      OnExecute = actIntoListExecute
    end
    object actIntoTree: TAction
      Caption = 'actIntoTree'
      OnExecute = actIntoTreeExecute
    end
    object actExamination: TAction
      Caption = 'Ok'
      OnExecute = actExaminationExecute
      OnUpdate = actOkUpdate
    end
    object actPlanning: TAction
      Caption = 'actPlanning'
      OnExecute = actPlanningExecute
    end
  end
  object ibqNewTx: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      '  INSERT INTO TRANSMITTERS ('
      '    ID,'
      '    STAND_ID,'
      '    LATITUDE,'
      '    LONGITUDE,'
      '    DATECREATE,'
      '    DATECHANGE,'
      '    OWNER_ID,'
      '    LICENSE_CHANNEL_ID,'
      '    LICENSE_RFR_ID,'
      '    LICENSE_SERVICE_ID,'
      '    NUMPERMBUILD,'
      '    DATEPERMBUILDFROM,'
      '    DATEPERMBUILDTO,'
      '    NUMPERMUSE,'
      '    DATEPERMUSEFROM,'
      '    DATEPERMUSETO,'
      '    REGIONALCOUNCIL,'
      '    NUMPERMREGCOUNCIL,'
      '    DATEPERMREGCOUNCIL,'
      '    NOTICECOUNT,'
      '    NUMSTANDCERTIFICATE,'
      '    DATESTANDCERTIFICATE,'
      '    NUMFACTORY,'
      '    RESPONSIBLEADMIN,'
      '    ADMINISTRATIONID,'
      '    REGIONALAGREEMENT,'
      '    DATEINTENDUSE,'
      '    AREACOVERAGE,'
      '    SYSTEMCAST_ID,'
      '    CLASSWAVE,'
      '    ACCOUNTCONDITION_IN,'
      '    ACCOUNTCONDITION_OUT,'
      '    TIMETRANSMIT,'
      '    TYPESYSTEM,'
      '    CHANNEL_ID,'
      '    VIDEO_CARRIER,'
      '    VIDEO_OFFSET_LINE,'
      '    VIDEO_OFFSET_HERZ,'
      '    FREQSTABILITY,'
      '    TYPEOFFSET,'
      '    SYSTEMCOLOUR,'
      '    VIDEO_EMISSION,'
      '    POWER_VIDEO,'
      '    EPR_VIDEO_MAX,'
      '    EPR_VIDEO_HOR,'
      '    EPR_VIDEO_VERT,'
      '    EFFECTPOWERHOR,'
      '    EFFECTPOWERVER,'
      '    ALLOTMENTBLOCKDAB_ID,'
      '    GUARDINTERVAL_ID,'
      '    IDENTIFIERSFN,'
      '    RELATIVETIMINGSFN,'
      '    BLOCKCENTREFREQ,'
      '    SOUND_CARRIER_PRIMARY,'
      '    SOUND_OFFSET_PRIMARY,'
      '    SOUND_EMISSION_PRIMARY,'
      '    POWER_SOUND_PRIMARY,'
      '    EPR_SOUND_MAX_PRIMARY,'
      '    EPR_SOUND_HOR_PRIMARY,'
      '    EPR_SOUND_VERT_PRIMARY,'
      '    V_SOUND_RATIO_PRIMARY,'
      '    MONOSTEREO_PRIMARY,'
      '    SOUND_CARRIER_SECOND,'
      '    SOUND_OFFSET_SECOND,'
      '    SOUND_EMISSION_SECOND,'
      '    POWER_SOUND_SECOND,'
      '    EPR_SOUND_MAX_SECOND,'
      '    EPR_SOUND_HOR_SECOND,'
      '    EPR_SOUND_VER_SECOND,'
      '    SOUND_SYSTEM_SECOND,'
      '    V_SOUND_RATIO_SECOND,'
      '    HEIGHTANTENNA,'
      '    HEIGHT_EFF_MAX,'
      '    EFFECTHEIGHT,'
      '    POLARIZATION,'
      '    DIRECTION,'
      '    FIDERLOSS,'
      '    FIDERLENGTH,'
      '    ANGLEELEVATION_HOR,'
      '    ANGLEELEVATION_VER,'
      '    ANTENNAGAIN,'
      '    EFFECTANTENNAGAINS,'
      '    TESTPOINTSIS,'
      '    NAMEPROGRAMM,'
      '    USERID,'
      '    ORIGINALID,'
      '    NUMREGISTRY,'
      '    TYPEREGISTRY,'
      '    REMARKS,'
      '    RELAYSTATION_ID,'
      '    OPERATOR_ID,'
      '    TYPERECEIVE_ID,'
      '    LEVELSIDERADIATION,'
      '    FREQSHIFT,'
      '    SUMMATORPOWERS,'
      '    AZIMUTHMAXRADIATION,'
      '    SUMMATOFREQFROM,'
      '    SUMMATORFREQTO,'
      '    SUMMATORPOWERFROM,'
      '    SUMMATORPOWERTO,'
      '    SUMMATORMINFREQS,'
      '    SUMMATORATTENUATION,'
      '    STATUS)'
      '  VALUES ('
      '    :ID,'
      '    :STAND_ID,'
      '    :LATITUDE,'
      '    :LONGITUDE,'
      '    :DATECREATE,'
      '    :DATECHANGE,'
      '    :OWNER_ID,'
      '    :LICENSE_CHANNEL_ID,'
      '    :LICENSE_RFR_ID,'
      '    :LICENSE_SERVICE_ID,'
      '    :NUMPERMBUILD,'
      '    :DATEPERMBUILDFROM,'
      '    :DATEPERMBUILDTO,'
      '    :NUMPERMUSE,'
      '    :DATEPERMUSEFROM,'
      '    :DATEPERMUSETO,'
      '    :REGIONALCOUNCIL,'
      '    :NUMPERMREGCOUNCIL,'
      '    :DATEPERMREGCOUNCIL,'
      '    :NOTICECOUNT,'
      '    :NUMSTANDCERTIFICATE,'
      '    :DATESTANDCERTIFICATE,'
      '    :NUMFACTORY,'
      '    :RESPONSIBLEADMIN,'
      '    :ADMINISTRATIONID,'
      '    :REGIONALAGREEMENT,'
      '    :DATEINTENDUSE,'
      '    :AREACOVERAGE,'
      '    :SYSTEMCAST_ID,'
      '    :CLASSWAVE,'
      '    :ACCOUNTCONDITION_IN,'
      '    :ACCOUNTCONDITION_OUT,'
      '    :TIMETRANSMIT,'
      '    :TYPESYSTEM,'
      '    :CHANNEL_ID,'
      '    :VIDEO_CARRIER,'
      '    :VIDEO_OFFSET_LINE,'
      '    :VIDEO_OFFSET_HERZ,'
      '    :FREQSTABILITY,'
      '    :TYPEOFFSET,'
      '    :SYSTEMCOLOUR,'
      '    :VIDEO_EMISSION,'
      '    :POWER_VIDEO,'
      '    :EPR_VIDEO_MAX,'
      '    :EPR_VIDEO_HOR,'
      '    :EPR_VIDEO_VERT,'
      '    :EFFECTPOWERHOR,'
      '    :EFFECTPOWERVER,'
      '    :ALLOTMENTBLOCKDAB_ID,'
      '    :GUARDINTERVAL_ID,'
      '    :IDENTIFIERSFN,'
      '    :RELATIVETIMINGSFN,'
      '    :BLOCKCENTREFREQ,'
      '    :SOUND_CARRIER_PRIMARY,'
      '    :SOUND_OFFSET_PRIMARY,'
      '    :SOUND_EMISSION_PRIMARY,'
      '    :POWER_SOUND_PRIMARY,'
      '    :EPR_SOUND_MAX_PRIMARY,'
      '    :EPR_SOUND_HOR_PRIMARY,'
      '    :EPR_SOUND_VERT_PRIMARY,'
      '    :V_SOUND_RATIO_PRIMARY,'
      '    :MONOSTEREO_PRIMARY,'
      '    :SOUND_CARRIER_SECOND,'
      '    :SOUND_OFFSET_SECOND,'
      '    :SOUND_EMISSION_SECOND,'
      '    :POWER_SOUND_SECOND,'
      '    :EPR_SOUND_MAX_SECOND,'
      '    :EPR_SOUND_HOR_SECOND,'
      '    :EPR_SOUND_VER_SECOND,'
      '    :SOUND_SYSTEM_SECOND,'
      '    :V_SOUND_RATIO_SECOND,'
      '    :HEIGHTANTENNA,'
      '    :HEIGHT_EFF_MAX,'
      '    :EFFECTHEIGHT,'
      '    :POLARIZATION,'
      '    :DIRECTION,'
      '    :FIDERLOSS,'
      '    :FIDERLENGTH,'
      '    :ANGLEELEVATION_HOR,'
      '    :ANGLEELEVATION_VER,'
      '    :ANTENNAGAIN,'
      '    :EFFECTANTENNAGAINS,'
      '    :TESTPOINTSIS,'
      '    :NAMEPROGRAMM,'
      '    :USERID,'
      '    :ORIGINALID,'
      '    :NUMREGISTRY,'
      '    :TYPEREGISTRY,'
      '    :REMARKS,'
      '    :RELAYSTATION_ID,'
      '    :OPERATOR_ID,'
      '    :TYPERECEIVE_ID,'
      '    :LEVELSIDERADIATION,'
      '    :FREQSHIFT,'
      '    :SUMMATORPOWERS,'
      '    :AZIMUTHMAXRADIATION,'
      '    :SUMMATOFREQFROM,'
      '    :SUMMATORFREQTO,'
      '    :SUMMATORPOWERFROM,'
      '    :SUMMATORPOWERTO,'
      '    :SUMMATORMINFREQS,'
      '    :SUMMATORATTENUATION,'
      '    :STATUS)')
    Left = 204
    Top = 432
    ParamData = <
      item
        DataType = ftUnknown
        Name = 'ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'STAND_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'LATITUDE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'LONGITUDE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATECREATE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATECHANGE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'OWNER_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'LICENSE_CHANNEL_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'LICENSE_RFR_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'LICENSE_SERVICE_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NUMPERMBUILD'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATEPERMBUILDFROM'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATEPERMBUILDTO'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NUMPERMUSE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATEPERMUSEFROM'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATEPERMUSETO'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'REGIONALCOUNCIL'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NUMPERMREGCOUNCIL'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATEPERMREGCOUNCIL'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NOTICECOUNT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NUMSTANDCERTIFICATE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATESTANDCERTIFICATE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NUMFACTORY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'RESPONSIBLEADMIN'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ADMINISTRATIONID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'REGIONALAGREEMENT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DATEINTENDUSE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'AREACOVERAGE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SYSTEMCAST_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'CLASSWAVE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ACCOUNTCONDITION_IN'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ACCOUNTCONDITION_OUT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'TIMETRANSMIT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'TYPESYSTEM'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'CHANNEL_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'VIDEO_CARRIER'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'VIDEO_OFFSET_LINE'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'VIDEO_OFFSET_HERZ'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'FREQSTABILITY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'TYPEOFFSET'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SYSTEMCOLOUR'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'VIDEO_EMISSION'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'POWER_VIDEO'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_VIDEO_MAX'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_VIDEO_HOR'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_VIDEO_VERT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EFFECTPOWERHOR'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EFFECTPOWERVER'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ALLOTMENTBLOCKDAB_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'GUARDINTERVAL_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'IDENTIFIERSFN'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'RELATIVETIMINGSFN'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'BLOCKCENTREFREQ'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_CARRIER_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_OFFSET_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_EMISSION_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'POWER_SOUND_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_SOUND_MAX_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_SOUND_HOR_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_SOUND_VERT_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'V_SOUND_RATIO_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'MONOSTEREO_PRIMARY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_CARRIER_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_OFFSET_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_EMISSION_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'POWER_SOUND_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_SOUND_MAX_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_SOUND_HOR_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EPR_SOUND_VER_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SOUND_SYSTEM_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'V_SOUND_RATIO_SECOND'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'HEIGHTANTENNA'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'HEIGHT_EFF_MAX'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EFFECTHEIGHT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'POLARIZATION'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'DIRECTION'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'FIDERLOSS'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'FIDERLENGTH'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ANGLEELEVATION_HOR'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ANGLEELEVATION_VER'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ANTENNAGAIN'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'EFFECTANTENNAGAINS'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'TESTPOINTSIS'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NAMEPROGRAMM'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'USERID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'ORIGINALID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'NUMREGISTRY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'TYPEREGISTRY'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'REMARKS'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'RELAYSTATION_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'OPERATOR_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'TYPERECEIVE_ID'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'LEVELSIDERADIATION'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'FREQSHIFT'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATORPOWERS'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'AZIMUTHMAXRADIATION'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATOFREQFROM'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATORFREQTO'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATORPOWERFROM'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATORPOWERTO'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATORMINFREQS'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'SUMMATORATTENUATION'
        ParamType = ptUnknown
      end
      item
        DataType = ftUnknown
        Name = 'STATUS'
        ParamType = ptUnknown
      end>
  end
  object pmIntoBeforeBase: TPopupMenu
    Left = 672
    Top = 56
    object mniCopyToDraftAnal: TMenuItem
      Caption = #1047#1088#1086#1073#1080#1090#1080' '#1082#1086#1087#1110#1102' '#1074' '#1087#1088#1077#1076#1073#1072#1079#1110
      OnClick = mniCopyToDraftAnalClick
    end
    object mniCopyToDraftDig: TMenuItem
      Caption = #1047#1088#1086#1073#1080#1090#1080' '#1082#1086#1087#1110#1102' '#1074' '#1087#1088#1077#1076#1073#1072#1079#1110
      OnClick = mniCopyToDraftDigClick
    end
    object mniMoveToDraft: TMenuItem
      Caption = #1055#1077#1088#1077#1085#1077#1089#1090#1080' '#1074' '#1087#1088#1077#1076#1073#1072#1079#1091
      OnClick = mniMoveToDraftClick
    end
  end
  object ImageList1: TImageList
    Left = 576
    Top = 56
    Bitmap = {
      494C010109000E00040010001000FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000400000004000000001002000000000000040
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000087308000873
      0800087308000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000884
      AD000884AD000884AD000884AD000884AD000884AD00000000000873080029BD
      4200087308000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000884AD0021AD
      CE0021ADCE0021C6EF0042CEEF0063C6DE0039ADCE000884AD000873080029BD
      4A00087308000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF000873080008730800087308000873080039CE
      5A00087308000873080008730800087308000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF005AE7840052DE84004ADE7B004AD6730042CE
      630039CE5A0031C6520029BD4A00087308000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF00007308000073080008730800087308004AD6
      7300087308000073080000730800007308000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF0094EFFF006BD6EF00089CCE000873080052DE
      7B00087308000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD00B5EFF7008CDE
      EF005AC6DE00219CC6001894BD0052B5CE006BBDD6008CD6E700087308005AE7
      8400087308000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000088CB5006BC6D60039BD
      DE0029C6E70029D6FF0052DEFF0084DEF70052BDDE00108CBD00087308005AE7
      8400087308000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF0094EFFF006BD6EF00089CCE000094C6000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF0094EFFF006BD6EF00089CCE000094C6000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF0094EFFF006BD6EF00089CCE000094C6000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD0073DEEF0063EF
      FF0039E7FF0029DEFF0052E7FF0094EFFF006BD6EF00089CCE000094C6000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD00C6DEEF00E7FF
      FF00CEFFFF00A5FFFF009CFFFF009CFFFF009CFFFF0073F7FF0039CEEF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000008CBD006BB5CE00F7FF
      FF00DEFFFF00B5FFFF009CFFFF009CFFFF009CFFFF0084F7FF0031ADCE000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000008C
      BD00008CBD00008CBD00008CBD00008CBD00008CBD0000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000848484008484
      8400848484008484840084848400848484000000000000000000000000000000
      000000000000000000000000000000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF0000000000848484000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400848484008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000FFFFFF00000000000000000000000000FFFF
      FF0000000000000000000000000000000000000000000000000000000000FFFF
      FF00848484008484840084848400848484000000000000000000FFFFFF000000
      0000FFFFFF00FFFFFF0000000000000000000000000000000000FFFFFF008484
      840000000000000000008484840084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF0000000000FFFF
      FF00FFFFFF0000000000FFFFFF00848484000000000000000000000000000000
      00000000000000000000000000000000000084848400FFFFFF00848484008484
      8400FFFFFF00848484008484840000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF0000000000FFFFFF00848484000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000FFFFFF00FFFF
      FF0000000000FFFFFF00FFFFFF0084848400FFFFFF000000000000000000FFFF
      FF0000000000000000000000000000000000FFFFFF0084848400848484000000
      000084848400848484008484840084848400000000000000000000FFFF0000FF
      FF00000000000000000000000000000000008484840084848400FFFFFF000000
      0000FFFFFF000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000008484840084848400848484008484
      8400FFFFFF008484840084848400848484000000000000000000FFFFFF000000
      000000000000FFFFFF0000000000000000000000000000000000FFFFFF008484
      840084848400000000008484840084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF0000000000FFFF
      FF00FFFFFF0000000000FFFFFF0084848400000000000000000000FFFF000000
      0000000000000000000000000000000000008484840000000000000000008484
      84000000000000000000000000000000000000FFFF0000FFFF0000FFFF0000FF
      FF000000000000000000FFFFFF00000000008484840084848400848484008484
      8400848484008484840000000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000FFFFFF000000
      000000000000FFFFFF000000000084848400FFFFFF000000000000000000FFFF
      FF0000000000000000000000000000000000FFFFFF0084848400848484000000
      0000848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000848484000000
      00000000000000000000000000000000000000FFFF00848484008484840000FF
      FF00000000000000000000000000000000008484840084848400848484008484
      840084848400848484008484840000000000000000000000000000000000FFFF
      FF00FFFFFF00000000000000000000000000000000000000000084848400FFFF
      FF0000000000848484008484840084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF0000000000FFFF
      FF00FFFFFF0000000000FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000FFFFFF0000FFFF00FFFFFF00FFFFFF00FFFF
      FF000000000000000000000000000000000084848400FFFFFF00000000000000
      00008484840084848400FFFFFF0000000000000000000000000000FFFF000000
      00000000000000000000FFFFFF00000000000000000084848400000000008484
      840084848400848484000000000084848400FFFFFF000000000000000000FFFF
      FF0000000000000000000000000000000000FFFFFF0084848400848484000000
      0000848484008484840084848400848484000000000000000000000000000000
      00000000000000000000000000000000000084848400FFFFFF00848484008484
      8400FFFFFF0084848400848484000000000000FFFF00FFFFFF00FFFFFF00FFFF
      FF000000000000000000000000000000000084848400FFFFFF00000000000000
      00008484840084848400FFFFFF000000000000000000FFFFFF0000000000FFFF
      FF00FFFFFF000000000000000000000000000000000000000000848484000000
      000000000000848484008484840084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF0000000000FFFF
      FF00FFFFFF0000000000FFFFFF0084848400000000000000000000FFFF0000FF
      FF00000000000000000000000000000000008484840084848400FFFFFF000000
      0000FFFFFF0000000000000000000000000000FFFF00FFFFFF00FFFFFF00FFFF
      FF00000000000000FF000000FF000000000084848400FFFFFF00000000000000
      0000848484008484840084848400FFFFFF000000000000FFFF0000FFFF000000
      000000FFFF00FFFFFF00FFFFFF00000000008484840000000000000000008484
      840000000000000000000000000084848400FFFFFF000000000000000000FFFF
      FF0000000000000000000000000000000000FFFFFF0084848400848484000000
      000084848400848484008484840084848400000000000000000000FFFF000000
      0000000000000000000000000000000000008484840000000000000000008484
      84000000000000000000000000000000000000FFFF00FFFFFF00FFFFFF00FFFF
      FF00000000000000FF000000FF000000000084848400FFFFFF00000000000000
      0000848484008484840084848400FFFFFF0000000000FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF00FFFFFF00000000008484840000000000000000000000
      00008484840000000000FFFFFF0084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000848484000000
      00000000000000000000000000000000000000FFFF00FFFFFF00FFFFFF00FFFF
      FF000000FF000000FF000000FF000000FF0084848400FFFFFF00000000000000
      0000848484008484840084848400848484000000000000FFFF0000FFFF0000FF
      FF00FFFFFF00FFFFFF0000000000000000008484840000000000000000000000
      0000FFFFFF00000000008484840084848400FF000000FF000000FF000000FF00
      0000FF000000FF000000FF000000000000008484840084848400848484008484
      8400848484008484840084848400848484000000000000000000000000000000
      00000000000000000000000000000000000084848400FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF00000000000000000000FFFF00FFFFFF00FFFFFF00FFFF
      FF00000000000000FF000000FF000000000084848400FFFFFF00FFFFFF00FFFF
      FF008484840084848400848484000000000000000000FFFFFF00FFFFFF000000
      000000000000FFFFFF00FFFFFF000000000084848400FFFFFF00000000008484
      840084848400FFFFFF00FFFFFF0084848400C6C6C600FF000000FF000000FF00
      0000FF000000FF000000C6C6C60000000000FFFFFF0084848400848484008484
      84008484840084848400FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400FFFFFF008484
      8400848484008484840000000000000000008484840000000000848484000000
      0000000000000000FF000000FF00000000008484840084848400848484008484
      840084848400848484008484840000000000FFFF000000FFFF0000FFFF0000FF
      FF0000FFFF00FFFFFF00FFFFFF000000000084848400FFFFFF00000000000000
      0000FFFFFF0000000000FFFFFF00000000000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400848484008484
      84008484840084848400848484008484840000FFFF0000FFFF00000000000000
      000000000000000000000000000000000000FFFFFF0000000000FFFFFF000000
      00000000000000000000000000000000000000FFFF0084848400000000008484
      8400000000000000FF000000FF000000000084848400FFFFFF00FFFFFF00FFFF
      FF0084848400848484008484840000000000FFFF000000000000FFFFFF00FFFF
      FF0000000000FFFFFF0000000000000000008484840084848400FFFFFF00FFFF
      FF0084848400FFFFFF0084848400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000FFFF0000000000000000000000
      0000000000000000000000000000000000000000000084848400000000000000
      00000000000000000000000000000000000000FFFF00848484000000000000FF
      FF000000FF000000FF0000000000000000008484840084848400FFFFFF008484
      840084848400848484000000000000000000FFFF0000FFFF0000000000000000
      0000000000000000000000000000000000008484840084848400848484008484
      8400848484008484840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000008484840084848400848484008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF000000
      0000000000000000000000000000FFFFFF00FFFFFF0000000000000000000000
      0000000000000000000000000000000000000000000084848400848484008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000084848400848484008484
      840084848400FFFFFF000000000000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000000000000FFFFFF0084848400FFFF
      FF00FFFFFF00FFFFFF000000000084848400000000000000FF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF000000
      000000000000000000008484840084848400FFFFFF00FFFFFF00FF000000FF00
      0000FFFFFF00000000000000FF000000000000000000FFFFFF00000000000000
      000000000000848484000000000084848400000000000000FF000000FF000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      00000000000084848400FFFFFF00000000000000000000000000FFFFFF00FFFF
      FF000000000000000000FFFFFF0000000000FFFFFF0084848400FFFFFF00FFFF
      FF0084848400848484000000000084848400000000000000FF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF00FFFF
      FF000000000000000000848484008484840000000000FFFFFF00000000000000
      0000FFFFFF00000000000000FF00000000000000000000000000848484008484
      840000000000848484000000000084848400000000000000FF000000FF000000
      FF000000FF000000840000000000000000008484840000000000000000000000
      000000000000FFFFFF00000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000084848400FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00848484000000FF000000FF000000FF000000
      000000000000000000000000000000000000848484008484840084848400FFFF
      FF00000000000000000000000000FFFFFF0000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00000000000000FF00000000000000000000000000FFFFFF000000
      0000FFFFFF0084848400FFFFFF00848484000000000000008400000084000000
      84000000840000008400000000000000000084848400FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0084848400FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0000000000000000000000000000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00848484008484840084848400848484000000FF000000FF000000FF000000
      0000000000000000000000000000000000008484840084848400848484000000
      0000000000000000000000000000848484000000000000000000FFFFFF000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000008400000000008484840084848400848484008484
      84008484840084848400FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0084848400000000000000FF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF000000
      0000000000000000000000000000848484000000000000000000FFFFFF008484
      840084848400FFFFFF00FFFFFF00FFFFFF00000000000000000000000000FFFF
      FF0000000000FFFFFF0000000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000008400000000008484840084848400000000000000
      000000000000FFFFFF000000000084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0000000000000000000000000000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0084848400848484008484840084848400000000000000FF00000000000000
      0000000000000000000000000000000000000000000084848400000000000000
      0000000000000000000000000000FFFFFF0000000000000000000000000000FF
      FF0000FFFF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000FFFF
      FF0000000000FFFFFF0000000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000000000000000000084848400000000000000
      0000FFFFFF00FFFFFF008484840084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000008484
      840084848400FFFFFF00FFFFFF00FFFFFF000000000000000000000000000000
      0000FFFFFF0000000000FFFFFF00000000000000000000000000FFFFFF000000
      000000000000C6C6C600FFFFFF00000000000000000084848400FFFFFF008484
      840084848400000000000000000084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF008484840000000000FFFFFF0084848400000000000000000000000000FF00
      0000000000000000000000000000000000000000000000000000000000008484
      8400FFFFFF000000000000000000848484000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF000000000000000000000000008484
      84008484840000000000FFFFFF0000000000000000000000000000000000C6C6
      C600C6C6C600C6C6C600FFFFFF0000000000000000000000000084848400FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0084848400FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF000000000000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00000000008484840084848400000000000000000000000000FF00
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF008484
      8400FFFFFF00FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000FFFFFF000000000000000000FFFFFF0000000000FFFFFF00FFFFFF000000
      0000000000008484840084848400FFFFFF000000000000000000000000000000
      00000000000000000000FFFFFF00000000000000000000000000848484008484
      84008484840084848400FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF008484840000000000FFFFFF0000000000FF000000FF000000FF000000FF00
      0000FF000000000000000000FF00000000008484840084848400848484008484
      840084848400FFFFFF0084848400000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF008484840084848400848484000000
      0000000000008484840084848400FFFFFF00000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000848484000000
      0000FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF000000000000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF008484840000000000FF000000FF000000FF000000FF00
      0000FF000000000000000000FF00000000008484840084848400848484008484
      84008484840000000000848484000000000000000000FFFF0000000000000000
      0000000000000000000000000000000000008484840000000000848484000000
      0000848484008484840084848400848484000000000000000000000000000000
      000000000000FFFFFF0000000000000000000000000000000000848484008484
      840084848400000000008484840000000000FFFFFF00FFFFFF00FFFFFF000000
      000000000000000000000000000000000000FFFFFF00FFFFFF00FFFFFF008484
      840084848400848484000000000000000000000000000000000000000000FF00
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000FFFFFF0000000000FFFF0000000000000000
      0000000000000000000000000000FFFFFF008484840000000000848484000000
      000000000000848484008484840000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF0000000000000000000000000000000000848484000000
      000000000000FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00000000000000
      000000000000000000000000000000000000FFFFFF00FFFFFF00848484008484
      840084848400000000000000000000000000000000000000000000000000FF00
      0000000000000000000000000000000000000000000000000000000000008484
      84000000000000000000848484008484840000000000FFFF0000000000000000
      0000000000000000000000000000FFFFFF0084848400FFFFFF00848484000000
      000000000000848484008484840000000000000000000000000000000000FFFF
      FF0000000000000000000000000000000000000000000000000084848400FFFF
      FF0084848400848484000000000000000000FFFFFF0000000000000000000000
      000000000000000000000000000000000000FFFFFF0084848400848484008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084848400848484000000000000000000000000000000
      0000000000000000000000000000FFFFFF008484840084848400848484000000
      0000000000008484840084848400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF000000000000000000000000000000
      000084848400848484000000000000000000424D3E000000000000003E000000
      2800000040000000400000000100010000000000000200000000000000000000
      000000000000000000000000FFFFFF0000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000FFC7000000000000E047000000000000
      C007000000000000800000000000000080000000000000008000000000000000
      800700000000000080070000000000008007000000000000801F000000000000
      801F000000000000801F000000000000801F000000000000801F000000000000
      801F000000000000E07F000000000000C0C0FF007F7FE0E2C0C000007F42E0E0
      C0CC00244901E084C0C800100F170000C0C400244F6F0103C0DA00105F5F0101
      C0C800247F42033180A200104901033180D800240F170130006E00104F6F0130
      007400005F5F0030007400007F0B01010020000023030101013500003F5F0101
      0101FFFF3FBF03030303FFFF7F7F070FFFC3E0C0FFDE00808783C082BC9C00BA
      83F98002BC8C80CA037B00001F0E80D0010000001E1EC0E001010000BE9EC0EB
      003A0000BFBEE0EB80B00002FEFEE0F580860004EEE6E0E5C0C00004EF81F098
      C1C0010505011818C1D4010105051050C1C50303EFEE1859C3D90707ECEC1819
      C3C30F0FFCFC1819EFEF1F1FFFFFF2F300000000000000000000000000000000
      000000000000}
  end
  object dsUserActLog: TDataSource
    DataSet = ibdsUserActLog
    Left = 432
    Top = 464
  end
  object ibdsUserActLog: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      
        'select av.ID, av.DATECHANGE, ad.LOGIN, av.NAME_TABLE, av.NAME_FI' +
        'ELD, av.NUM_CHANGE, av.TYPECHANGE  '
      'from ACTIVEVIEW av'
      'left join ADMIT ad on (av.ADMIT_ID = ad.ID) '
      'where NUM_CHANGE = :ID'
      'and NAME_TABLE = '#39'TRANSMITTERS'#39
      'order by av.DATECHANGE')
    Left = 404
    Top = 464
    object ibdsUserActLogID: TIntegerField
      FieldName = 'ID'
      Origin = 'ACTIVEVIEW.ID'
      Required = True
      Visible = False
    end
    object ibdsUserActLogDATECHANGE: TDateTimeField
      DisplayLabel = #1044#1072#1090#1072
      FieldName = 'DATECHANGE'
      Origin = 'ACTIVEVIEW.DATECHANGE'
    end
    object ibdsUserActLogLOGIN: TIBStringField
      DisplayLabel = #1050#1086#1088#1080#1089#1090#1091#1074#1072#1095
      FieldName = 'LOGIN'
      Origin = 'ADMIT.LOGIN'
      Size = 16
    end
    object ibdsUserActLogTYPECHANGE: TIBStringField
      DisplayLabel = #1058#1080#1087' '#1079#1084#1110#1085#1080
      FieldName = 'TYPECHANGE'
      Origin = 'ACTIVEVIEW.TYPECHANGE'
      Size = 16
    end
    object ibdsUserActLogNAME_TABLE: TIBStringField
      DisplayLabel = #1058#1072#1073#1083#1080#1094#1103
      DisplayWidth = 20
      FieldName = 'NAME_TABLE'
      Origin = 'ACTIVEVIEW.NAME_TABLE'
      Size = 32
    end
    object ibdsUserActLogNAME_FIELD: TIBStringField
      DisplayLabel = #1055#1086#1083#1077
      DisplayWidth = 32
      FieldName = 'NAME_FIELD'
      Origin = 'ACTIVEVIEW.NAME_FIELD'
      Size = 256
    end
    object ibdsUserActLogNUM_CHANGE: TIntegerField
      DisplayLabel = #1047#1072#1087#1080#1089
      FieldName = 'NUM_CHANGE'
      Origin = 'ACTIVEVIEW.NUM_CHANGE'
    end
  end
  object sqlNewAdminId: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select max(t.ADMINISTRATIONID)'
      'from TRANSMITTERS t'
      'right outer join STAND s on (t.STAND_ID = s.ID)'
      'where s.AREA_ID = (select AREA_ID from STAND where ID = :ID)')
    Transaction = dmMain.trMain
    Left = 608
    Top = 56
  end
  object ibqOwner: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAMEORGANIZATION from OWNER')
    Left = 480
    Top = 464
  end
  object ibqStand: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    OnCalcFields = ibqStandCalcFields
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select '#9'S.ID, '
      #9'S.NAMESITE, '
      #9'S.CITY_ID, '
      #9'S.AREA_ID, '
      #9'S.HEIGHT_SEA, '
      #9'AREA.NAME AREA_NAME, '
      #9'CITY.NAME CITY_NAME, '
      #9'STREET.NAME STREET_NAME, '
      #9'S.ADDRESS, '
      #9'AREA.NUMREGION NUMREG '
      'from STAND S '
      'left outer join AREA on (STAND.AREA_ID = AREA.ID) '
      'left outer join STREET on (STAND.STREET_ID = STREET.ID) '
      'left outer join CITY on (STAND.CITY_ID = CITY.ID) '
      'where S.ID = :ID')
    Left = 40
    Top = 456
    ParamData = <
      item
        DataType = ftUnknown
        Name = 'ID'
        ParamType = ptUnknown
      end>
    object ibqStandID: TIntegerField
      FieldName = 'ID'
      Origin = 'STAND.ID'
      Required = True
    end
    object ibqStandNAMESITE: TIBStringField
      FieldName = 'NAMESITE'
      Origin = 'STAND.NAMESITE'
      Size = 32
    end
    object ibqStandCITY_ID: TIntegerField
      FieldName = 'CITY_ID'
      Origin = 'STAND.CITY_ID'
      Required = True
    end
    object ibqStandAREA_ID: TIntegerField
      FieldName = 'AREA_ID'
      Origin = 'STAND.AREA_ID'
      Required = True
    end
    object ibqStandHEIGHT_SEA: TIntegerField
      FieldName = 'HEIGHT_SEA'
      Origin = 'STAND.HEIGHT_SEA'
    end
    object ibqStandAREA_NAME: TIBStringField
      FieldName = 'AREA_NAME'
      Origin = 'AREA.NAME'
      Size = 32
    end
    object ibqStandCITY_NAME: TIBStringField
      FieldName = 'CITY_NAME'
      Origin = 'CITY.NAME'
      Size = 32
    end
    object ibqStandSTREET_NAME: TIBStringField
      FieldName = 'STREET_NAME'
      Origin = 'STREET.NAME'
      Size = 32
    end
    object ibqStandADDRESS: TIBStringField
      FieldName = 'ADDRESS'
      Origin = 'STAND.ADDRESS'
      Required = True
      Size = 128
    end
    object ibqStandNUMREG: TIBStringField
      FieldName = 'NUMREG'
      Origin = 'AREA.NUMREGION'
      Size = 4
    end
    object ibqStandFULL_ADDR: TStringField
      FieldKind = fkCalculated
      FieldName = 'FULL_ADDR'
      Size = 32
      Calculated = True
    end
  end
  object imlMap: TImageList
    Left = 532
    Top = 75
    Bitmap = {
      494C01011A001D00040010001000FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000400000008000000001002000000000000080
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000084848400000000008484
      8400000000008484840000000000848484000000000084848400000000008484
      8400000000008484840000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000FFFF00848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000008484
      840084848400848484008484840084848400000000000000000000FFFF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000084848400000000000000FF000000
      00000000000000000000FF000000FF0000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF00000000000000FF0000000000000000000000FF0000000000000000000000
      0000000000000000000000000000848484008484840084848400C6C6C600FFFF
      FF000000FF000000FF000000FF00FFFFFF00C6C6C60084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      FF00000000000000FF000000000000000000000000000000FF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF000000
      FF00FFFFFF00C6C6C600FFFFFF000000FF00FFFFFF0084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF0000000000000000000000000000000000000000000000FF000000
      0000000000000000FF0000000000848484000000000084848400C6C6C6000000
      FF00C6C6C600FFFFFF00C6C6C6000000FF00C6C6C60084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      00000000000000000000000000000000000000000000000000000000FF000000
      00000000FF000000000000000000000000000000000084848400FFFFFF000000
      FF00FFFFFF00C6C6C600FFFFFF000000FF00FFFFFF0084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000848484008484840084848400C6C6C600FFFF
      FF000000FF000000FF000000FF00FFFFFF00C6C6C60084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FF00000084000000FF0000008400000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000000000000000000000000000000000
      00000000000000000000848484000000FF008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      000084000000000000000000FF000000FF000000FF0000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000FF00000084000000FF00000084000000FF000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000000000
      00000000000000000000848484000000FF008484840000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      00000084000000FF00000084000000840000008400000084000000FF00000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      FF000000FF0000000000000000000000000000000000FF000000000000000000
      000000000000000000000000FF000000FF00000000007B7B7B00000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF00000000000000000000000000FF000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF00000000000000000000000000848484000000FF008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000084
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B000000FF007B7B7B007B7B7B00FF0000007B7B7B007B7B
      7B000000FF007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B000000FF007B7B7B00FF000000FF000000FF0000007B7B
      7B000000FF007B7B7B007B7B7B007B7B7B00000000000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF00000084000000FF00000084
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000000000000000FF000000000000000000
      00000000FF000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF00FF000000000000000000000000000000FF00
      00000000FF0000000000000000000000000000000000848484000000FF008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000084
      000000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000000000000000FF0000000000FF000000000000000000
      FF0000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000FF0000000000FF000000000000000000000000000000
      FF00FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF000000FF000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FF000000000000000000FF000000FF000000FF000000
      0000FF0000000000000000000000000000008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000084000000FF00000084000000840000008400000084000000FF00000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00FF0000007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B00FF0000007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B00FF0000007B7B7B007B7B7B007B7B7B008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      000000FF00000084000000FF00000084000000FF000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000000000000000000000000000FF00
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      000000000000000000000000000000000000FF000000FF000000FF0000000000
      0000000000000000000000000000000000000000000084000000000000008484
      84000000FF008484840000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      000000000000000000000000000000FF00000084000000FF0000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B000000000000000000000000008484
      84000000FF008484840000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000840000008400000084000000840000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000007B7B
      7B007B7B7B0000000000000000000000000000FF000000000000000000000000
      0000840000008400000000000000000000000000000000FF0000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      00000000000000000000000000000000000000000000000000007B7B7B007B7B
      7B000000000000000000000000000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00000000000000FF0000000000000000
      0000000000000000000000000000000000000000FF0000000000000000000000
      00000000FF00000000000000000000000000000000000000FF00000000000000
      000000000000000000007B7B7B000000000000000000000000009C6300009C63
      0000CE9C000094949400949494009494940094949400949494009C6331009C63
      31009C6331009C6331000000000000000000FF00000000000000FF000000FF00
      000000000000000000000000000000000000000000007B7B7B007B7B7B000000
      00000000000000000000000000000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00000000000000FF0000000000000000
      000000000000000000000000000000000000000000000000FF00000000000000
      00000000FF0000000000000000000000000000000000000000000000FF000000
      0000000000007B7B7B000000FF00000000000000000000000000FFCE6300CE9C
      0000CEFFFF00CEFFFF00CEFFFF009CCECE009CCECE0094ADAD0094ADAD009C63
      3100FFCE63009C6331000000000000000000FF00000000000000000000000000
      0000FF0000000000000000000000000000007B7B7B007B7B7B00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000000FF00008400000000FF00008400
      000084000000840000008400000000FF00008400000000FF0000840000008400
      000084000000840000000000000000000000000000007B7B7B000000FF007B7B
      7B000000FF0000000000000000000000000000000000000000007B7B7B000000
      FF007B7B7B000000FF0000000000000000000000000000000000CE9C0000CEFF
      FF00CEFFFF0000000000000000000000000000000000000000009CCECE0094AD
      AD009C6331009C6331000000000000000000FF000000FF000000000000000000
      0000000000000000000000000000FFFFFF007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00008400000000FF0000840000008400
      00008400000084000000000000000000000000000000000000000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000000000000000000000000000000000000000CEFFFF00CEFF
      FF00CEFFFF0094949400FFFFFF00FFFFFF00CEFFFF00000000009CCECE0094AD
      AD009CCECE0094949400000000000000000000000000FF000000FF000000FF00
      0000FF000000FF00000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00000000000000FF0000840000008400
      00000000000000000000000000000000000000000000000000000000FF000000
      FF00000000000000FF000000000000000000000000000000FF00000000000000
      FF000000FF000000000000000000000000000000000000000000CEFFFF00CEFF
      FF00CEFFFF0094949400FFFFFF00FFFFFF00CEFFFF00000000009CCECE009CCE
      CE0094ADAD009494940000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000000FF000000000000000000000000
      0000840000008400000000FF0000000000000000000000FF0000840000008400
      000000000000000000000000000000FF0000000000000000FF000000FF000000
      FF000000FF0000000000000000000000000000000000000000000000FF000000
      FF000000FF000000FF000000FF00000000000000000000000000FFFFFF00CEFF
      FF00CEFFFF009494940000000000FFFFFF00CEFFFF0000000000CEFFFF0094AD
      AD009CCECE00949494000000000000000000000000000000000000000000FF00
      000000000000FF000000000000000000000000000000FFFFFF00000000000000
      0000FFFFFF0000000000FFFFFF00000000000000000000000000000000000000
      0000840000008400000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000FF00000000000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00CEFFFF0094949400639C9C0000639C0000000000000000009CCECE00CEFF
      FF0094ADAD009494940000000000000000000000000000000000FF0000000000
      000000000000FF000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000000000000000000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      00000000FF000000FF00000000000000000000000000000000009C6300009CCE
      CE00FFFFFF009494940094949400009CCE009CFFFF0000639C0094ADAD00CEFF
      FF009C6331009C63000000000000000000000000000000000000FF0000000000
      000000000000FF000000000000000000000000000000FFFFFF0000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000FF00008400000000FF000084000000840000008400
      00008400000000FF00008400000000FF00000000000000000000000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      00000000FF000000FF0000000000000000000000000000000000CE9C00009C63
      00009CCECE00FFFFFF00CEFFFF00CEFFFF00009CCE0000CEFF00000000009C63
      3100FFCE63009C63000000000000000000000000000000000000FF0000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFCE6300CE9C
      00009C6300009CCECE00FFFFFF00CEFFFF00FFFFFF00009CCE0000639C000000
      0000CE9C00009C6300000000000000000000000000000000000000000000FF00
      0000FF000000FF000000000000000000000000000000FFFFFF0000000000FFFF
      FF0000000000FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000CE9C0000CE9C
      0000CE9C00009C63000094ADAD0094ADAD0094ADAD0094ADAD0000CEFF000063
      9C000000000094ADAD0000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF0000000000000000000000000000000000000000000000
      0000000000000000000000FF0000000000000000000000000000840000008400
      000000000000000000000000000000FF00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000005252
      FF000031CE000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000FF000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000FF000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000FF000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000FF000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000FF000000FF0000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000FF000000FF000000FF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF000000FF000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000FF000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF00000000000000000000000000000000000000000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000008400000084000000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF00000000000000000000000000000000000000000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000084000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000008400000084000000000000000000000000000000000000000000000000
      0000008400000084000000840000008400000084000000840000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000084
      0000008400000084000000840000008400000084000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000008400000084
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000000000000000000000000000000000000008400000084
      0000008400000084000000840000008400000084000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000008400000084
      0000000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000000000000000000000000000000000000008400000084
      0000008400000084000000840000008400000084000000840000008400000084
      0000008400000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000000000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000084
      0000008400000084000000840000008400000084000000840000008400000084
      0000008400000084000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000008400000084000000840000008400000084000000840000008400000084
      0000008400000084000000840000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000084000000840000008400000084000000840000008400000084
      0000008400000084000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000084000000840000000000
      0000000000000000FF000000FF00000000000000000000000000000000000000
      00000000000000000000840000000000000000000000000000000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000840000008400000084000000840000008400000084
      0000008400000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000084000000840000000000
      0000000000000000FF000000FF00000000000000000000000000000000000000
      00000000000000000000000000008400000000000000000000000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000008400000084000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000840000000000FF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000084000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF00FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000BDBDBD000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000BDBDBD000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF000000
      00000000000000FFFF0000000000000000000000000000FFFF00000000000000
      000000FFFF000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF000000000000FFFF000000000000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000FFFF00848484000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF0000000000FFFFFF000000000000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000FFFF00848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      840084848400848484008484840084848400000000000000000000FFFF000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000FFFF0000000000FFFFFF007B7B7B00FFFFFF000000000000FFFF0000FF
      FF0000FFFF000000000000000000000000000000000000000000000000008484
      840084848400848484008484840084848400000000000000000000FFFF000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FFFFFF007B7B7B00FFFFFF0000000000000000000000
      000000000000000000000000000000000000000000000000000084848400FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF008484840000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000000000FFFFFF0000FFFF007B7B7B0000FFFF00FFFFFF000000000000FF
      FF0000FFFF00000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF007B7B7B00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000008484840084848400FFFFFF00C6C6
      C600FFFFFF000000FF00FFFFFF00C6C6C600FFFFFF0084848400848484000000
      00000000000000000000000000000000000000FFFF0000FFFF0000FFFF0000FF
      FF000000000000FFFF00FFFFFF007B7B7B00FFFFFF0000FFFF000000000000FF
      FF0000FFFF0000FFFF0000FFFF00000000008484840084848400C6C6C600FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF00C6C6C60084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF007B7B7B00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000084848400C6C6C600FFFF
      FF00C6C6C6000000FF00C6C6C600FFFFFF00C6C6C60084848400000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000000000FFFFFF0000FFFF00FFFFFF0000FFFF00FFFFFF000000000000FF
      FF0000FFFF000000000000000000000000000000000084848400FFFFFF00C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C600FFFFFF0084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF000000
      FF000000FF000000FF000000FF000000FF00FFFFFF0084848400000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000FFFF0000000000FFFFFF0000FFFF00FFFFFF000000000000FFFF0000FF
      FF0000FFFF000000000000000000000000000000000084848400C6C6C6000000
      FF000000FF000000FF000000FF000000FF00C6C6C60084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      0000000000000000000000000000000000000000000084848400C6C6C600FFFF
      FF00C6C6C6000000FF00C6C6C600FFFFFF00C6C6C60084848400000000000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF0000000000000000000000000000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000000000000084848400FFFFFF00C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C600FFFFFF0084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400FFFFFF00C6C6
      C600FFFFFF000000FF00FFFFFF00C6C6C600FFFFFF0084848400848484000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF0000FFFF0000FFFF0000FFFF0000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000008484840084848400C6C6C600FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF00C6C6C60084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000084848400FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF008484840000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF000000
      00000000000000FFFF0000FFFF0000FFFF0000FFFF0000FFFF00000000000000
      000000FFFF00000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000C6C6C600FFFFFF00C6C6C600C6C6C600C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FFFFFF00C6C6C600FFFFFF00C6C6C600C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF00000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00C6C6C600FFFFFF00FFFFFF00C6C6C600C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FF000000FF00000000000000FFFFFF0000000000FF0000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00C6C6C600FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6C600FFFF
      FF00C6C6C6000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FF0000000000000000000000FFFFFF0000000000FFFFFF0000000000FF00
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00C6C6C600FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6C600FFFFFF00C6C6
      C600C6C6C6000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      000000000000000000000000000000000000000000000000000000000000FF00
      00000000000000000000FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF000000
      0000FF0000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6C600FFFF
      FF00C6C6C600C6C6C60000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      00000000000000000000000000000000000000000000FF000000FF0000000000
      0000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000FFFF
      FF0000000000FF00000000000000000000000000000000000000000000000000
      000000000000FFFFFF000000000000000000FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000000000000C6C6C600FFFF
      FF00C6C6C600FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6
      C600FFFFFF00C6C6C60000000000000000000000000000000000000000008400
      0000840000008400000084000000FF000000FF00000084000000840000008400
      000084000000840000008400000000000000FF0000000000000000000000FFFF
      FF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF000000
      0000FFFFFF0000000000FF000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      00000000000000000000000000000000000000000000C6C6C600FFFFFF00C6C6
      C60000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6C600FFFF
      FF00C6C6C600C6C6C60000000000000000000000000000000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF00
      0000FF000000FF00000084000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF0000000000FF0000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6
      C600C6C6C600FFFFFF00C6C6C600000000000000000000000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF00
      0000FF000000FF000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF0000000000FFFFFF00000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      000000000000C6C6C600C6C6C600FFFFFF00C6C6C600FFFFFF00FFFFFF00C6C6
      C60000000000C6C6C600C6C6C600000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00C6C6C60000000000FFFFFF00C6C6C60000000000FFFFFF00C6C6
      C60000000000FFFFFF00C6C6C600000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00C6C6C60000000000FFFFFF00C6C6C60000000000FFFFFF00C6C6
      C6000000000000000000C6C6C600000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      000000000000000000000000000000000000000000000000000000000000C6C6
      C600FFFFFF000000000000000000FFFFFF00C6C6C60000000000FFFFFF00C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000C6C6
      C600FFFFFF000000000000000000C6C6C600C6C6C60000000000C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF00C6C6C60000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000424D3E000000000000003E000000
      2800000040000000800000000100010000000000000400000000000000000000
      000000000000000000000000FFFFFF0000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000AAAAFFFD000000000001FFF800000000
      BFFCFFF1000000001FF9FFE300000000AFF4FFC70000000033CDE08F00000000
      BC3CC01F000000001CFD803F00000000AB7C001F000000002BBD001F00000000
      B7D8001F000000003FD5001F00000000BFEC001F000000003FFD803F00000000
      BFFCC07F00000000FFFFE0FF00000000FFBFDDDDDDDDFC7FFE1FD555D555F83F
      F81F00000000E011F01FDFBFDFFFD820F01F87BC87FC8C60F80FDBBBDBFB07E0
      F80F0000000007F1F80FDDB7DCE707FDF80F9EAF9CE78FFDF80FDF1FDD177FF1
      F01F000000007FE0F01FDFBFDEEFA3E0F81F9FBF9F1F81E0FE1FDFBFDFFFC1F1
      FFBF00000000C1CFFFFFFFFFFFFFE23FFFFFC0FF8000FFFFFFE7F3FF5555FFFF
      FFC773BF0000C0038F8F52BF76BCC003070052BFB6D8C0033200000386C2C003
      00005203CEE6C0038000528FCAA6C003F900718E86C0C003E100F14A4EF6C003
      C900FD4AE6F2C003C900FC00E6F2C003C300FD4AFEFEC003E300FD4A0000C003
      FF01FDCE5555FFE7FF03FFCF0000FFFFFFFFFFFFFFFFFFFFFFFFFC1FFFFF9FFF
      F80FFBEFFFFF8FFFF80FF7F7FFFFC7FFF80FEFFBFFFFE3FFF8FFEFFBFFFFF1FF
      F8FFDFFDFFFFF8FFF8FFDFFDFE7FFC7FF8FFDFFDFE7FFE3FF8FFEFFBFFFFFF1F
      F8FFEFFBFFFFFF8FF8FFF7F7FFFFFFC7F8FFFBEFFFFFFFE3F8FFFC1FFFFFFFF1
      F8FFFFFFFFFFFFF9FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCFFFFFFF8001
      CF9FF0FFFFFFBFFDCF9FFF0FFFFFBFFDFFFFFFF3F03FBFFDF3FFFFFBE01FBFFD
      F3CFBFF7C00FBFFDFFCFDFF7C007BFFDFFFFEFEFE003BFFDFCFFF7EFF001BFFD
      FCFFFBEFF803BFFDE799FDDFFC07BFFDE799FEDFFE0FBFFDFFFFFF3FFF1FBFFD
      FFFFFFBFFFBF8001FFFFFFFFFFFFFFFFFFFCFEFFFFFDFEFFFFF8FC7FFFF8FC7F
      FFF1FC7FFFF1FC7FFFE3D837FFE3FC7FFFC7E00FFFC7FC7FE08FE00FE08FFC7F
      C01FC007C01FF83F803FC007803FF01F001F0001001FF01F001FC007001FF01F
      001FC007001FF83F001FE00F001FFC7F001FE00F001FFFFF803FD837803FFFFF
      C07FFEFFC07FFFFFE0FFFEFFE0FFFFFFF807FFFFFFFFFFCFF807FF3FFE3FFF87
      F007FE3FF81FFF87E003FE3FF40FF70FC003FE3FE007F30FC001FE3F8003F01F
      8001E0014001F01F0001C0010000F0030000C0030000F0079000FE3F8001F00F
      E000FE3FC003F01FE000FE3FE00FF03FC005FE3FF07FF07FC007FE7FF8FFF0FF
      E40FFFFFFFFFF1FFFE7FFFFFFFFFF3FF00000000000000000000000000000000
      000000000000}
  end
  object tr: TIBTransaction
    Active = False
    DefaultDatabase = dmMain.dbMain
    DefaultAction = TARollback
    Params.Strings = (
      'read_committed'
      'rec_version'
      'nowait')
    AutoStopAction = saNone
    Left = 504
    Top = 112
  end
end

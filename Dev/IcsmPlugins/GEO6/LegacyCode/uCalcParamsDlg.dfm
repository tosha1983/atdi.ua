object dlgCalcParams: TdlgCalcParams
  Left = 593
  Top = 452
  BorderStyle = bsDialog
  Caption = #1055#1072#1088#1072#1084#1077#1090#1088#1080
  ClientHeight = 455
  ClientWidth = 370
  Color = clBtnFace
  Constraints.MinHeight = 380
  Constraints.MinWidth = 330
  ParentFont = True
  OldCreateOrder = True
  Position = poScreenCenter
  OnShow = FormShow
  DesignSize = (
    370
    455)
  PixelsPerInch = 96
  TextHeight = 13
  object pcParams: TPageControl
    Left = 0
    Top = 0
    Width = 370
    Height = 413
    ActivePage = tshExpert
    Align = alTop
    Anchors = [akLeft, akTop, akRight, akBottom]
    MultiLine = True
    TabIndex = 1
    TabOrder = 0
    object tshCalcParams: TTabSheet
      Caption = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082
      DesignSize = (
        362
        385)
      object gbxPropagModel: TGroupBox
        Left = 4
        Top = 88
        Width = 354
        Height = 44
        Anchors = [akLeft, akTop, akRight]
        Caption = #1052#1086#1076#1077#1083#1100' '#1088#1086#1079#1087#1086#1074#1089#1102#1076#1078#1077#1085#1085#1103' '#1045#1052#1061
        TabOrder = 1
        object cbxPropagServer: TComboBox
          Left = 8
          Top = 16
          Width = 145
          Height = 21
          Style = csDropDownList
          ItemHeight = 0
          TabOrder = 0
        end
        object btPpgAdd: TButton
          Left = 160
          Top = 15
          Width = 60
          Height = 21
          Caption = #1044#1086#1076#1072#1090#1080'...'
          TabOrder = 1
          OnClick = btPpgAddClick
        end
        object btPpgRmv: TButton
          Left = 224
          Top = 15
          Width = 60
          Height = 21
          Caption = #1059#1073#1088#1072#1090#1080'...'
          TabOrder = 2
          OnClick = btPpgRmvClick
        end
        object btPrgEdt: TButton
          Left = 288
          Top = 15
          Width = 56
          Height = 21
          Caption = #1056#1077#1076'...'
          TabOrder = 3
          OnClick = btPrgEdtClick
        end
      end
      object gbxReliefModel: TGroupBox
        Left = 4
        Top = 132
        Width = 354
        Height = 85
        Anchors = [akLeft, akTop, akRight]
        Caption = #1052#1086#1076#1077#1083#1100' '#1088#1077#1083#1100#1108#1092#1091
        TabOrder = 2
        DesignSize = (
          354
          85)
        object Label1: TLabel
          Left = 16
          Top = 40
          Width = 73
          Height = 13
          Caption = #1064#1083#1103#1093' '#1076#1086' '#1076#1072#1085#1080#1093
        end
        object lblFilesNum: TLabel
          Left = 280
          Top = 59
          Width = 30
          Height = 13
          Caption = #1052#1072#1082#1089'.'
          WordWrap = True
        end
        object cbxReliefServer: TComboBox
          Left = 8
          Top = 16
          Width = 145
          Height = 21
          Style = csDropDownList
          ItemHeight = 0
          TabOrder = 0
          OnChange = cbxReliefServerChange
        end
        object edtPathData: TEdit
          Left = 8
          Top = 56
          Width = 241
          Height = 21
          Anchors = [akLeft, akTop, akRight]
          TabOrder = 4
          OnExit = edtPathDataExit
        end
        object btnPath: TButton
          Left = 253
          Top = 57
          Width = 20
          Height = 20
          Anchors = [akTop, akRight]
          Caption = '...'
          TabOrder = 5
          OnClick = btnPathClick
        end
        object edtFilesNum: TNumericEdit
          Left = 312
          Top = 56
          Width = 33
          Height = 21
          TabOrder = 6
          Text = '0'
          OnExit = edtFilesNumExit
          Alignment = taRightJustify
          OldValue = '0'
        end
        object btDtmAdd: TButton
          Left = 160
          Top = 16
          Width = 60
          Height = 21
          Caption = #1044#1086#1076'..'
          TabOrder = 1
          OnClick = btDtmAddClick
        end
        object btDtmRmv: TButton
          Left = 224
          Top = 16
          Width = 60
          Height = 21
          Caption = #1059#1073#1088'...'
          TabOrder = 2
          OnClick = btDtmRmvClick
        end
        object btDtmEdt: TButton
          Left = 288
          Top = 16
          Width = 56
          Height = 21
          Caption = #1056#1077#1076'...'
          TabOrder = 3
          OnClick = btDtmEdtClick
        end
      end
      object gbxPathParams: TGroupBox
        Left = 4
        Top = 217
        Width = 354
        Height = 56
        Anchors = [akLeft, akTop, akRight]
        Caption = ' '#1055#1072#1088#1072#1084#1077#1090#1088#1080' '#1090#1088#1072#1089#1080' '
        TabOrder = 3
        object Label2: TLabel
          Left = 236
          Top = 30
          Width = 40
          Height = 13
          Caption = #1064#1072#1075', '#1082#1084
        end
        object chbHeff: TCheckBox
          Left = 16
          Top = 16
          Width = 117
          Height = 17
          Caption = #1045#1092#1077#1082#1090#1080#1074#1085#1072' '#1074#1080#1089#1086#1090#1072
          TabOrder = 0
        end
        object chbTxClearance: TCheckBox
          Left = 16
          Top = 32
          Width = 105
          Height = 17
          Caption = #1050#1091#1090' '#1079#1072#1082#1088'. '#1087#1088#1076#1095
          TabOrder = 1
        end
        object chbRxClearance: TCheckBox
          Left = 132
          Top = 16
          Width = 97
          Height = 17
          Caption = #1050#1091#1090' '#1079#1072#1082#1088' '#1087#1088#1084
          TabOrder = 2
        end
        object chbMorphology: TCheckBox
          Left = 132
          Top = 32
          Width = 98
          Height = 17
          Caption = #1043#1110#1076#1088#1086#1083#1086#1075#1110#1103
          TabOrder = 3
        end
        object edtStep: TNumericEdit
          Left = 284
          Top = 27
          Width = 53
          Height = 21
          TabOrder = 4
          Text = '1'
          OnExit = edtStepExit
          Alignment = taRightJustify
          OldValue = '1'
        end
      end
      object gbxCalcServer: TGroupBox
        Left = 4
        Top = 8
        Width = 197
        Height = 73
        Anchors = [akLeft, akTop, akRight]
        Caption = #1057#1077#1088#1074#1077#1088' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1110#1074
        TabOrder = 0
        object cbxCalcServer: TComboBox
          Left = 8
          Top = 16
          Width = 177
          Height = 21
          Style = csDropDownList
          ItemHeight = 0
          TabOrder = 0
        end
        object btCalcAdd: TButton
          Left = 8
          Top = 40
          Width = 56
          Height = 21
          Caption = #1044#1086#1076#1072#1090#1080'...'
          TabOrder = 1
          OnClick = btCalcAddClick
        end
        object btCalcRmv: TButton
          Left = 68
          Top = 40
          Width = 56
          Height = 21
          Caption = #1059#1073#1088#1072#1090#1080'...'
          TabOrder = 2
          OnClick = btCalcRmvClick
        end
        object btCalcEdt: TButton
          Left = 128
          Top = 40
          Width = 56
          Height = 21
          Caption = #1056#1077#1076'...'
          TabOrder = 3
          OnClick = btCalcEdtClick
        end
      end
      object gbxCalcLog: TGroupBox
        Left = 4
        Top = 334
        Width = 354
        Height = 46
        Anchors = [akLeft, akTop, akRight]
        Caption = #1046#1091#1088#1085#1072#1083' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1110#1074
        TabOrder = 4
        DesignSize = (
          354
          46)
        object edtCalcLog: TEdit
          Left = 8
          Top = 16
          Width = 317
          Height = 21
          Anchors = [akLeft, akTop, akRight]
          TabOrder = 0
        end
        object btnCalcLog: TButton
          Left = 325
          Top = 17
          Width = 20
          Height = 20
          Anchors = [akTop, akRight]
          Caption = '...'
          TabOrder = 1
          OnClick = btnCalcLogClick
        end
      end
      object rgrEminMethod: TRadioGroup
        Left = 208
        Top = 8
        Width = 150
        Height = 73
        Anchors = [akLeft, akTop, akRight]
        Caption = #1052#1077#1090#1086#1076' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091' E'#1074#1080#1082
        Items.Strings = (
          #1057#1091#1084#1110#1088#1091#1074#1072#1085#1085#1103' '
          #1057#1087#1088#1086#1097#1077#1085#1077' '#1084#1085#1086#1078#1077#1085#1085#1103
          #1063#1077#1089#1090#1077#1088' 97')
        TabOrder = 5
      end
      object gbxPathTheo: TGroupBox
        Left = 4
        Top = 273
        Width = 354
        Height = 56
        Anchors = [akLeft, akTop, akRight]
        Caption = ' '#1055#1072#1088#1072#1084#1077#1090#1088#1080' '#1090#1088#1072#1089#1080' '#1076#1083#1103' '#1090#1077#1086#1088'. '#1079#1086#1085' '
        TabOrder = 6
        object Label28: TLabel
          Left = 236
          Top = 30
          Width = 40
          Height = 13
          Caption = #1064#1072#1075', '#1082#1084
        end
        object chbHeffTheo: TCheckBox
          Left = 16
          Top = 16
          Width = 117
          Height = 17
          Caption = #1045#1092#1077#1082#1090#1080#1074#1085#1072' '#1074#1080#1089#1086#1090#1072
          TabOrder = 0
        end
        object chbTxClearanceTheo: TCheckBox
          Left = 16
          Top = 32
          Width = 105
          Height = 17
          Caption = #1050#1091#1090' '#1079#1072#1082#1088'. '#1087#1088#1076#1095
          TabOrder = 1
        end
        object chbRxClearanceTheo: TCheckBox
          Left = 132
          Top = 16
          Width = 97
          Height = 17
          Caption = #1050#1091#1090' '#1079#1072#1082#1088' '#1087#1088#1084
          TabOrder = 2
        end
        object chbMorphologyTheo: TCheckBox
          Left = 132
          Top = 32
          Width = 98
          Height = 17
          Caption = #1043#1110#1076#1088#1086#1083#1086#1075#1110#1103
          TabOrder = 3
        end
        object edtStepTheo: TNumericEdit
          Left = 284
          Top = 27
          Width = 53
          Height = 21
          TabOrder = 4
          Text = '1'
          OnExit = edtStepExit
          Alignment = taRightJustify
          OldValue = '1'
        end
      end
      object chbTheoPathTheSame: TCheckBox
        Left = 240
        Top = 283
        Width = 65
        Height = 17
        Caption = ' '#1058#1072#1082'i '#1078' '
        TabOrder = 7
        OnClick = chbTheoPathTheSameClick
      end
    end
    object tshExpert: TTabSheet
      Caption = #1045#1082#1089#1087#1077#1088#1090#1080#1079#1072
      ImageIndex = 2
      object lblHigherIntNum: TLabel
        Left = 56
        Top = 31
        Width = 165
        Height = 13
        Caption = #1050#1110#1083#1100#1082#1110#1089#1090#1100' '#1089#1090#1072#1088#1096#1080#1093' '#1079#1072#1074#1072#1076' '#1076#1083#1103'  '#1050#1058
      end
      object Label14: TLabel
        Left = 56
        Top = 260
        Width = 152
        Height = 13
        Caption = #1050#1088#1086#1082' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091' '#1079#1086#1085', '#1075#1088#1072#1076#1091#1089#1080
      end
      object Label15: TLabel
        Left = 56
        Top = 304
        Width = 157
        Height = 13
        Caption = #1055#1086#1088#1110#1075' '#1087#1077#1088#1077#1074#1080#1097#1077#1085#1085#1103' ('#1040#1058#1041', '#1062#1058#1041')'
      end
      object Label16: TLabel
        Left = 56
        Top = 328
        Width = 161
        Height = 13
        Caption = #1055#1086#1088#1110#1075' '#1087#1077#1088#1077#1074#1080#1097#1077#1085#1085#1103' ('#1040#1056#1052', '#1062#1056#1052')'
      end
      object Label26: TLabel
        Left = 132
        Top = 363
        Width = 192
        Height = 13
        Caption = #1055#1077#1088#1077#1076#1072#1074#1072#1095#1110' '#1079' '#1085#1077#1079#1073#1077#1088#1077#1078#1077#1085#1080#1084#1080' '#1076#1072#1085#1080#1084#1080
      end
      object edtHigherIntNum: TNumericEdit
        Left = 228
        Top = 28
        Width = 50
        Height = 21
        TabOrder = 2
        Text = '6'
        OnExit = edtStepExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '6'
      end
      object edtMinSelInterf: TNumericEdit
        Left = 228
        Top = 4
        Width = 50
        Height = 21
        TabOrder = 1
        Text = '0'
        OnExit = edtStepExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object chbMapAutoFit: TCheckBox
        Left = 12
        Top = 56
        Width = 201
        Height = 17
        Caption = #1040#1074#1090#1086#1084#1072#1090#1080#1095#1085#1077' '#1087#1086#1079#1080#1094#1110#1102#1074#1072#1085#1085#1103' '#1082#1072#1088#1090#1080
        TabOrder = 3
      end
      object chbDuelAutoRecalc: TCheckBox
        Left = 12
        Top = 140
        Width = 249
        Height = 17
        Caption = #1040#1074#1090#1086#1084#1072#1090#1080#1095#1085#1080#1081' '#1087#1077#1088#1077#1088#1072#1093#1091#1085#1086#1082' '#1076#1091#1077#1083#1100#1085#1080#1093' '#1079#1072#1074#1072#1076
        TabOrder = 4
      end
      object chbSelectionAutotruncation: TCheckBox
        Left = 12
        Top = 7
        Width = 213
        Height = 17
        Caption = #1052#1110#1085#1110#1084#1072#1083#1100#1085#1072' '#1079#1072#1074#1072#1076#1072' '#1074#1080#1073#1110#1088#1082#1080', '#1076#1041#1084#1082#1042'/'#1084
        TabOrder = 0
        OnClick = chbSelectionAutotruncationClick
      end
      object cbxDegreeStep: TComboBox
        Left = 228
        Top = 256
        Width = 50
        Height = 21
        Style = csDropDownList
        ItemHeight = 13
        TabOrder = 7
        Items.Strings = (
          '1'
          '5'
          '10')
      end
      object chbShowCp: TCheckBox
        Left = 12
        Top = 280
        Width = 177
        Height = 17
        Caption = #1055#1086#1082#1072#1079#1091#1074#1072#1090#1080' '#1050#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1058#1086#1095#1082#1080' '
        TabOrder = 8
      end
      object edtTreshVideo: TNumericEdit
        Left = 228
        Top = 300
        Width = 50
        Height = 21
        TabOrder = 9
        Text = '0'
        OnExit = edtTreshVideoExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object edtTreshAudio: TNumericEdit
        Left = 228
        Top = 324
        Width = 50
        Height = 21
        TabOrder = 10
        Text = '0'
        OnExit = edtTreshVideoExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object cbxChangedTxColor: TColorBox
        Left = 11
        Top = 358
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 11
      end
      object cbxDisableReliefAtPlanning: TCheckBox
        Left = 12
        Top = 162
        Width = 269
        Height = 17
        Caption = #1042#1110#1076#1082#1083#1102#1095#1072#1090#1080' '#1088#1077#1083#1100#1108#1092' '#1087#1088#1080' '#1087#1088#1086#1074#1077#1076#1077#1085#1085#1110' '#1087#1083#1072#1085#1091#1074#1072#1085#1085#1103
        TabOrder = 5
      end
      object rgDuelType: TRadioGroup
        Left = 8
        Top = 184
        Width = 265
        Height = 65
        Caption = ' '#1044#1091#1077#1083#1100#1085'i '#1079#1072#1074#1072#1076#1080' '
        Items.Strings = (
          #1042' '#1084'i'#1089#1094#1103#1093' '#1074#1089#1090#1072#1085#1086#1074#1083#1077#1085#1085#1103
          #1053#1072' '#1089#1077#1088#1077#1076#1080#1085'i '#1090#1088#1072#1089#1080
          #1055'i'#1076#1078#1072#1090#1090#1103' '#1079#1086#1085#1080)
        TabOrder = 6
      end
      object gbxMapXProblems: TGroupBox
        Left = 4
        Top = 76
        Width = 345
        Height = 57
        Caption = ' '#1057#1087#1088#1086#1073#1072' '#1086#1073#1110#1081#1090#1080' '#1087#1088#1086#1073#1083#1077#1084#1080' MapX '#1085#1072' Windows 7 '
        TabOrder = 12
        object lbMapInitDelay: TLabel
          Left = 201
          Top = 17
          Width = 6
          Height = 13
          Caption = #1089
        end
        object cbMapInitDelay: TCheckBox
          Left = 8
          Top = 17
          Width = 161
          Height = 17
          Caption = #1047#1072#1090#1088#1080#1084#1082#1072' '#1087#1110#1089#1083#1103' '#1110#1085#1110#1094#1110#1072#1083#1110#1079#1072#1094#1110#1111
          TabOrder = 0
          OnClick = cbMapInitDelayClick
        end
        object cbMapInitInfo: TCheckBox
          Left = 8
          Top = 36
          Width = 321
          Height = 17
          Caption = #1044#1086#1076#1072#1090#1082#1086#1074#1077' '#1087#1086#1074#1110#1076#1086#1084#1083#1077#1085#1085#1103' '#1087#1088#1086' '#1079#1072#1082#1110#1085#1095#1077#1085#1085#1103' '#1110#1085#1110#1094#1110#1072#1083#1110#1079#1072#1094#1110#1111
          TabOrder = 1
        end
        object edtMapInitDelay: TNumericEdit
          Left = 168
          Top = 15
          Width = 30
          Height = 21
          TabOrder = 2
          Text = '6'
          OnExit = edtMapInitDelayExit
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = '6'
        end
      end
    end
    object tshCommon: TTabSheet
      Caption = #1047#1072#1075#1072#1083#1100#1085#1110
      ImageIndex = 3
      object Label18: TLabel
        Left = 72
        Top = 200
        Width = 131
        Height = 13
        Caption = #1058#1086#1074#1097#1080#1085#1072' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1079#1072#1074#1072#1076
      end
      object Label19: TLabel
        Left = 72
        Top = 176
        Width = 127
        Height = 13
        Caption = #1058#1086#1074#1097#1080#1085#1072' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1096#1091#1084#1091
      end
      object Label20: TLabel
        Left = 72
        Top = 152
        Width = 143
        Height = 13
        Caption = #1058#1086#1074#1097#1080#1085#1072' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1087#1086#1082#1088#1110#1090#1090#1103
      end
      object Label21: TLabel
        Left = 136
        Top = 229
        Width = 128
        Height = 13
        Caption = #1050#1086#1083#1110#1088' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1087#1086#1082#1088#1080#1090#1090#1103
      end
      object Label22: TLabel
        Left = 136
        Top = 254
        Width = 108
        Height = 13
        Caption = #1050#1086#1083#1110#1088' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1096#1091#1084#1091
      end
      object Label23: TLabel
        Left = 136
        Top = 279
        Width = 112
        Height = 13
        Caption = #1050#1086#1083#1110#1088' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1079#1072#1074#1072#1076
      end
      object Label24: TLabel
        Left = 136
        Top = 329
        Width = 172
        Height = 13
        Caption = #1050#1086#1083#1110#1088' '#1082#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1080#1093' '#1090#1086#1095#1086#1082' '#1074' '#1079#1086#1085#1110
      end
      object Label25: TLabel
        Left = 136
        Top = 355
        Width = 187
        Height = 13
        Caption = #1050#1086#1083#1110#1088' '#1082#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1080#1093' '#1090#1086#1095#1086#1082' '#1085#1077' '#1074' '#1079#1086#1085#1110
      end
      object Label27: TLabel
        Left = 136
        Top = 303
        Width = 121
        Height = 13
        Caption = #1050#1086#1083#1110#1088' '#1083#1110#1085#1110#1081' '#1079#1086#1085#1080' '#1079#1072#1074#1072#1076' 2'
      end
      object chbQueryOnMainormClose: TCheckBox
        Left = 12
        Top = 12
        Width = 221
        Height = 17
        Caption = #1047#1072#1087#1080#1090' '#1085#1072' '#1079#1072#1082#1088#1080#1090#1090#1103' '#1075#1086#1083#1086#1074#1085#1086#1111' '#1092#1086#1088#1084#1080
        TabOrder = 0
      end
      object chbGetCoordinatesFromBase: TCheckBox
        Left = 12
        Top = 32
        Width = 213
        Height = 17
        Caption = #1041#1088#1072#1090#1080' '#1082#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1110' '#1074#1110#1076#1089#1090#1072#1085#1110' '#1079' '#1073#1072#1079#1080
        TabOrder = 1
        Visible = False
      end
      object chbEarthCurveInRelief: TCheckBox
        Left = 12
        Top = 72
        Width = 209
        Height = 17
        Caption = #1050#1088#1080#1074#1110#1079#1085#1072' '#1047#1077#1084#1083#1110'  '#1085#1072' '#1087#1088#1086#1092#1110#1083#1110' '#1088#1077#1083#1100#1108#1092#1091
        TabOrder = 2
      end
      object chbShowTxNames: TCheckBox
        Left = 12
        Top = 92
        Width = 301
        Height = 17
        Caption = #1042#1110#1076#1086#1073#1088#1072#1078#1072#1090#1080' '#1085#1072#1079#1074#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074' '#1085#1072' '#1082#1072#1088#1090#1110
        TabOrder = 3
      end
      object lineThicknessZoneCover: TNumericEdit
        Left = 12
        Top = 148
        Width = 50
        Height = 21
        TabOrder = 5
        Text = '0'
        OnExit = edtStepExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object lineThicknessZoneNoise: TNumericEdit
        Left = 12
        Top = 172
        Width = 50
        Height = 21
        TabOrder = 6
        Text = '0'
        OnExit = edtStepExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object lineThicknessZoneInterfere: TNumericEdit
        Left = 12
        Top = 196
        Width = 50
        Height = 21
        TabOrder = 7
        Text = '0'
        OnExit = edtStepExit
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object cbxLineColorZoneCover: TColorBox
        Left = 11
        Top = 225
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 8
        OnDblClick = cbxLineColorZoneCoverDblClick
      end
      object cbxLineColorZoneNoise: TColorBox
        Left = 11
        Top = 250
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 9
        OnDblClick = cbxLineColorZoneCoverDblClick
      end
      object cbxLineColorZoneInterfere: TColorBox
        Left = 11
        Top = 275
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 10
        OnDblClick = cbxLineColorZoneCoverDblClick
      end
      object cbxCoordinationPointsInZoneColor: TColorBox
        Left = 11
        Top = 325
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 11
        OnDblClick = cbxLineColorZoneCoverDblClick
      end
      object cbxCoordinationPointsOutZoneColor: TColorBox
        Left = 11
        Top = 351
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 12
        OnDblClick = cbxLineColorZoneCoverDblClick
      end
      object cbxLineColorZoneInterfere2: TColorBox
        Left = 11
        Top = 299
        Width = 112
        Height = 22
        Style = [cbStandardColors, cbExtendedColors, cbSystemColors, cbCustomColor, cbPrettyNames]
        ItemHeight = 16
        TabOrder = 13
        OnDblClick = cbxLineColorZoneCoverDblClick
      end
      object chRpcRxModeLink: TCheckBox
        Left = 12
        Top = 112
        Width = 341
        Height = 17
        Caption = #1047#1074#39#1103#1079#1086#1082' '#1084#1110#1078' RPC '#1090#1072' Rx Mode '#1074' '#1082#1072#1088#1090#1082#1072#1093' '#1062#1058#1041' '#1090#1072' '#1062#1056#1052
        TabOrder = 4
      end
    end
    object tshNewTx: TTabSheet
      Caption = #1053#1086#1074#1080#1081' '#1087#1077#1088#1077#1076#1072#1074#1072#1095
      ImageIndex = 1
      object lblRadius: TLabel
        Left = 12
        Top = 20
        Width = 159
        Height = 13
        Caption = #1056#1072#1076#1110#1091#1089' '#1087#1110#1076#1073#1086#1088#1091' '#1110#1089#1085#1091#1095#1080#1093' '#1086#1087#1086#1088', '#1084#1110#1085
      end
      object Label5: TLabel
        Left = 140
        Top = 56
        Width = 32
        Height = 13
        Caption = #1056#1077#1075#1110#1086#1085
      end
      object Label7: TLabel
        Left = 84
        Top = 84
        Width = 87
        Height = 13
        Caption = #1053#1072#1089#1077#1083#1077#1085#1080#1081' '#1087#1091#1085#1082#1090
      end
      object seRadius: TCSpinEdit
        Left = 180
        Top = 16
        Width = 45
        Height = 22
        MaxValue = 60
        MinValue = 1
        TabOrder = 0
        Value = 30
      end
      object edtNewStandAreaNum: TEdit
        Left = 84
        Top = 52
        Width = 49
        Height = 21
        TabOrder = 1
        Visible = False
      end
      object cbxNewStandArea: TComboBox
        Left = 180
        Top = 52
        Width = 161
        Height = 21
        Style = csDropDownList
        ItemHeight = 0
        TabOrder = 2
        OnChange = cbxNewStandAreaChange
      end
      object cbxNewStandCity: TComboBox
        Left = 180
        Top = 80
        Width = 161
        Height = 21
        Style = csDropDownList
        ItemHeight = 0
        TabOrder = 3
      end
    end
    object tshLisBcCalc: TTabSheet
      Caption = 'LIS BcCalc'
      ImageIndex = 4
      DesignSize = (
        362
        385)
      object Label4: TLabel
        Left = 8
        Top = 20
        Width = 69
        Height = 13
        Caption = 'Emin DVB 200'
      end
      object Label6: TLabel
        Left = 8
        Top = 44
        Width = 69
        Height = 13
        Caption = 'Emin DVB 500'
      end
      object Label8: TLabel
        Left = 8
        Top = 68
        Width = 69
        Height = 13
        Caption = 'Emin DVB 700'
      end
      object Label9: TLabel
        Left = 20
        Top = 104
        Width = 113
        Height = 13
        Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1110' '#1074#1110#1076#1089#1090#1072#1085#1110
      end
      object Label10: TLabel
        Left = 8
        Top = 152
        Width = 272
        Height = 26
        Caption = 
          #1086#1089#1083#1072#1073#1083#1077#1085#1080#1077' '#1087#1086' '#1079#1072#1076#1085#1077#1084#1091' '#1083#1077#1087#1077#1089#1090#1082#1091' '#1087#1088#1080#1077#1084#1085#1086#1081' '#1072#1085#1090#1077#1085#1085#1099' '#1076#1083#1103' '#1054#1042#1063'/'#1063#1052' ('#1084#1086#1085#1086 +
          '), '#1076#1041
        WordWrap = True
      end
      object Label11: TLabel
        Left = 8
        Top = 184
        Width = 272
        Height = 26
        Caption = 
          #1086#1089#1083#1072#1073#1083#1077#1085#1080#1077' '#1087#1086' '#1079#1072#1076#1085#1077#1084#1091' '#1083#1077#1087#1077#1089#1090#1082#1091' '#1087#1088#1080#1077#1084#1085#1086#1081' '#1072#1085#1090#1077#1085#1085#1099' '#1076#1083#1103' '#1054#1042#1063'/'#1063#1052' ('#1089#1090#1077#1088 +
          #1077#1086'), '#1076#1041
        WordWrap = True
      end
      object Label12: TLabel
        Left = 8
        Top = 216
        Width = 272
        Height = 26
        Caption = 
          #1086#1089#1083#1072#1073#1083#1077#1085#1080#1077' '#1087#1086' '#1079#1072#1076#1085#1077#1084#1091' '#1083#1077#1087#1077#1089#1090#1082#1091' '#1087#1088#1080#1077#1084#1085#1086#1081' '#1072#1085#1090#1077#1085#1085#1099' '#1076#1083#1103' '#1040#1058#1042' ('#1076#1080#1072#1087#1072#1079#1086 +
          #1085' II), '#1076#1041
        WordWrap = True
      end
      object Label13: TLabel
        Left = 8
        Top = 248
        Width = 267
        Height = 26
        Caption = #1086#1089#1083#1072#1073#1083#1077#1085#1080#1077' '#1076#1083#1103' '#1086#1088#1090#1086#1075#1086#1085#1072#1083#1100#1085#1086#1081' '#1087#1086#1083#1103#1088#1080#1079#1072#1094#1080#1080' '#1072#1085#1090#1077#1085#1085' '#1054#1042#1063'/'#1063#1052', '#1076#1041
        WordWrap = True
      end
      object Label17: TLabel
        Left = 8
        Top = 308
        Width = 273
        Height = 13
        Caption = #1064#1072#1075' '#1110#1090#1077#1088#1072#1094#1110#1081' '#1087#1088#1080' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091' '#1088#1072#1076#1110#1091#1089#1072' '#1079#1086#1085' '#1087#1086#1082#1088#1080#1090#1090#1103', '#1082#1084
      end
      object edtEmin_dvb_700: TNumericEdit
        Left = 84
        Top = 64
        Width = 50
        Height = 21
        TabOrder = 0
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object edtEmin_dvb_200: TNumericEdit
        Left = 84
        Top = 16
        Width = 50
        Height = 21
        TabOrder = 1
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object edtEmin_dvb_500: TNumericEdit
        Left = 84
        Top = 40
        Width = 50
        Height = 21
        TabOrder = 2
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object cbxQuick_calc_duel_interf: TCheckBox
        Left = 144
        Top = 18
        Width = 213
        Height = 17
        Caption = #1064#1074#1080#1076#1082#1080#1081' '#1088#1086#1079#1088#1072#1093#1091#1085#1086#1082' '#1076#1091#1077#1083#1100#1085#1080#1093' '#1079#1072#1074#1072#1076
        TabOrder = 3
      end
      object cbxQuick_calc_max_dist: TCheckBox
        Left = 144
        Top = 66
        Width = 193
        Height = 17
        Caption = #1064#1074#1080#1076#1082#1080#1081' '#1088#1086#1079#1088#1072#1093#1091#1085#1086#1082' '#1082#1086#1086#1088#1076'. '#1079#1086#1085#1080
        TabOrder = 4
      end
      object edtCoord_dist_ini_file: TEdit
        Left = 12
        Top = 120
        Width = 285
        Height = 21
        TabOrder = 5
      end
      object btCoordDistLoc: TButton
        Left = 301
        Top = 121
        Width = 20
        Height = 20
        Anchors = [akTop, akRight]
        Caption = '...'
        TabOrder = 6
        OnClick = btnDistPathClick
      end
      object edtBackLobeFmMono: TNumericEdit
        Left = 300
        Top = 148
        Width = 50
        Height = 21
        TabOrder = 7
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object edtBackLobeFmStereo: TNumericEdit
        Left = 300
        Top = 180
        Width = 50
        Height = 21
        TabOrder = 8
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object edtBackLobeTvBand2: TNumericEdit
        Left = 300
        Top = 212
        Width = 50
        Height = 21
        TabOrder = 9
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object edtPolarCorrectFm: TNumericEdit
        Left = 300
        Top = 244
        Width = 50
        Height = 21
        TabOrder = 10
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object chbTvSoundStereo: TCheckBox
        Left = 12
        Top = 284
        Width = 237
        Height = 17
        Caption = #1047#1072#1093#1080#1097#1072#1090#1080' '#1079#1074#1091#1082#1086#1074#1080#1081' '#1082#1072#1085#1072#1083' '#1058#1041' '#1103#1082' '#1089#1090#1077#1088#1077#1086
        TabOrder = 11
      end
      object edtStepCalcMaxDist: TNumericEdit
        Left = 300
        Top = 304
        Width = 50
        Height = 21
        TabOrder = 12
        Text = '0'
        Alignment = taRightJustify
        ApplyChanges = acExit
        OldValue = '0'
      end
      object chbRequestForCoordDist: TCheckBox
        Left = 144
        Top = 42
        Width = 209
        Height = 17
        Caption = #1047#1072#1087#1080#1090' '#1085#1072' '#1090#1080#1087' '#1082#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1086#1111' '#1079#1086#1085#1080
        TabOrder = 13
        OnClick = chbRequestForCoordDistClick
      end
    end
  end
  object btnCancel: TButton
    Left = 281
    Top = 423
    Width = 77
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Cancel'
    ModalResult = 2
    TabOrder = 2
    OnClick = btnCancelClick
  end
  object btnOk: TButton
    Left = 196
    Top = 422
    Width = 75
    Height = 26
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    ModalResult = 1
    TabOrder = 1
    OnClick = btnOkClick
  end
  object OpenDialog1: TOpenDialog
    DefaultExt = 'log'
    Filter = #1042#1089#1110' '#1092#1072#1081#1083#1080' (*.*)|*.*|'#1060#1072#1081#1083#1080' '#1078#1091#1088#1085#1072#1083#1072' (*.log)|*.log'
    Left = 320
    Top = 28
  end
  object sqlArea: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAME from AREA'
      'order by 2')
    Transaction = dmMain.trMain
    Left = 316
    Top = 64
  end
  object sqlCity: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select ID, NAME from CITY'
      'where AREA_ID = :AREA_ID'
      'order by 2')
    Transaction = dmMain.trMain
    Left = 304
    Top = 104
  end
  object colorDialog: TColorDialog
    Ctl3D = True
    Left = 336
    Top = 104
  end
end

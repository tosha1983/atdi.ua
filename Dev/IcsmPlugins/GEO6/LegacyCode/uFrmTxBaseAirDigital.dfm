inherited frmTxBaseAirDigital: TfrmTxBaseAirDigital
  Top = 114
  Caption = 'frmTxBaseAirDigital'
  ClientHeight = 535
  PixelsPerInch = 96
  TextHeight = 13
  inherited pcData: TPageControl
    Height = 443
    inherited tshCommon: TTabSheet
      inherited pnlTech: TPanel
        inherited gbAntenna: TGroupBox
          object lblOffset: TLabel [14]
            Left = 390
            Top = 122
            Width = 48
            Height = 13
            Caption = #1047#1089#1091#1074', '#1082#1043#1094
          end
          inherited btnAntPattV: TButton
            TabOrder = 15
          end
          object edtOffset: TNumericEdit
            Left = 440
            Top = 120
            Width = 77
            Height = 21
            TabOrder = 14
            Text = '0'
            Alignment = taRightJustify
            ApplyChanges = acExit
            OldValue = '0'
            OnValueChange = edtOffsetValueChange
          end
        end
      end
      object pnlForDigital: TPanel [1]
        Left = 0
        Top = 148
        Width = 772
        Height = 64
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 1
        object lblOChS: TLabel
          Left = 4
          Top = 12
          Width = 25
          Height = 13
          Caption = #1054#1063#1052
        end
        object lblSynchr: TLabel
          Left = 177
          Top = 6
          Width = 42
          Height = 26
          Caption = #1057#1080#1085#1093#1088#1086', '#1084#1082#1089
          WordWrap = True
        end
        object lblRpc: TLabel
          Left = 265
          Top = 12
          Width = 22
          Height = 13
          Caption = 'RPC'
        end
        object lblRxType: TLabel
          Left = 384
          Top = 12
          Width = 36
          Height = 13
          Caption = #1058#1080#1087' '#1087#1084
        end
        object lblIdRrc06: TLabel
          Left = 560
          Top = 12
          Width = 55
          Height = 13
          Caption = #1048#1076'. RRC06'
        end
        object lblPlanEntry: TLabel
          Left = 4
          Top = 43
          Width = 59
          Height = 13
          Caption = #1050#1086#1076' '#1074' '#1055#1083#1072#1085'i'
        end
        object lblAssgnCode: TLabel
          Left = 120
          Top = 43
          Width = 55
          Height = 13
          Caption = #1050#1086#1076' '#1087#1088#1080#1089#1074'.'
        end
        object lblAssAllotId: TLabel
          Left = 232
          Top = 43
          Width = 46
          Height = 13
          Caption = #1040#1089#1089'. '#1074#1080#1076'.'
        end
        object lblAssSfnId: TLabel
          Left = 408
          Top = 43
          Width = 53
          Height = 13
          Caption = #1040#1089#1089'. '#1054#1063#1052' '
        end
        object lblCallSign: TLabel
          Left = 612
          Top = 43
          Width = 50
          Height = 13
          Caption = #1055#1086#1079#1080#1074#1085#1080#1081
        end
        object lblPolIsol: TLabel
          Left = 488
          Top = 19
          Width = 33
          Height = 13
          Caption = #1087#1084', '#1076#1041
        end
        object lblSm: TLabel
          Left = 328
          Top = 12
          Width = 16
          Height = 13
          Caption = 'SM'
        end
        object edtSfn: TDBEdit
          Left = 31
          Top = 8
          Width = 106
          Height = 21
          DataField = 'SYNHRONETID'
          DataSource = dsTxDigital
          ReadOnly = True
          TabOrder = 0
        end
        object edtSynchronization: TNumericEdit
          Left = 220
          Top = 8
          Width = 40
          Height = 21
          TabOrder = 2
          Text = 'edtSynchronization'
          OnEnter = edtSynchronizationEnter
          Alignment = taRightJustify
          ApplyChanges = acExit
          OldValue = 'edtSynchronization'
          OnValueChange = edtSynchronizationValueChange
        end
        object btnSetSfn: TButton
          Left = 136
          Top = 8
          Width = 19
          Height = 20
          Caption = '...'
          TabOrder = 1
          OnClick = btnSetSfnClick
        end
        object cbxRpc: TComboBox
          Left = 289
          Top = 8
          Width = 36
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 3
          OnChange = cbxRpcChange
        end
        object cbxRxMode: TComboBox
          Left = 424
          Top = 8
          Width = 41
          Height = 21
          Style = csDropDownList
          ItemHeight = 13
          TabOrder = 5
          OnChange = cbxRxModeChange
        end
        object edtRrc06: TDBEdit
          Left = 620
          Top = 8
          Width = 138
          Height = 21
          DataField = 'ADM_REF_ID'
          DataSource = dsTxDigital
          TabOrder = 8
        end
        object cbPlanEntry: TDBComboBox
          Left = 67
          Top = 40
          Width = 44
          Height = 21
          DataField = 'PLAN_ENTRY'
          DataSource = dsTxDigital
          ItemHeight = 13
          Items.Strings = (
            '1'
            '2'
            '3'
            '4'
            '5')
          TabOrder = 9
        end
        object cbAssgnCode: TDBComboBox
          Left = 179
          Top = 40
          Width = 41
          Height = 21
          DataField = 'assgn_code'
          DataSource = dsTxDigital
          ItemHeight = 13
          Items.Strings = (
            'L'
            'C'
            'S')
          TabOrder = 10
          OnChange = cbAssgnCodeChange
        end
        object edAssocAllotId: TDBEdit
          Left = 280
          Top = 40
          Width = 105
          Height = 21
          DataField = 'associated_adm_allot_id'
          DataSource = dsTxDigital
          TabOrder = 11
        end
        object edAssocSfnId: TDBEdit
          Left = 464
          Top = 40
          Width = 137
          Height = 21
          DataField = 'associated_allot_sfn_id'
          DataSource = dsTxDigital
          TabOrder = 13
        end
        object edCallSign: TDBEdit
          Left = 664
          Top = 40
          Width = 94
          Height = 21
          DataField = 'call_sign'
          DataSource = dsTxDigital
          TabOrder = 14
        end
        object btnDropSfnId: TButton
          Left = 155
          Top = 8
          Width = 19
          Height = 20
          Caption = 'X'
          TabOrder = 15
          OnClick = btnDropSfnIdClick
        end
        object edAssgnCode: TDBEdit
          Left = 181
          Top = 43
          Width = 20
          Height = 14
          BorderStyle = bsNone
          DataField = 'ASSGN_CODE'
          DataSource = dsTxDigital
          TabOrder = 16
        end
        object btAssocAllot: TButton
          Left = 384
          Top = 40
          Width = 21
          Height = 21
          Hint = #1042#1099#1079#1086#1074' '#1082#1072#1088#1090#1086#1095#1082#1080' '#1074#1099#1076#1077#1083#1077#1085#1080#1103
          Caption = '->'
          TabOrder = 12
          OnClick = btAssocAllotClick
        end
        object edPolIsol: TNumericEdit
          Left = 528
          Top = 8
          Width = 25
          Height = 21
          ParentColor = True
          TabOrder = 7
          Text = '0'
          Alignment = taRightJustify
          OldValue = '0'
          OnValueChange = edPolIsolValueChange
        end
        object chPolIsol: TCheckBox
          Left = 468
          Top = 4
          Width = 53
          Height = 17
          Caption = #1050#1088#1086#1089#1089
          TabOrder = 6
          OnClick = chPolIsolClick
        end
        object cbSm: TDBComboBox
          Left = 345
          Top = 8
          Width = 36
          Height = 21
          Style = csDropDownList
          DataField = 'SPECT_MASK'
          DataSource = dsTxDigital
          ItemHeight = 13
          TabOrder = 4
        end
      end
      inherited pnlMaster: TPanel
        Top = 251
      end
      inherited gbxCoordination: TGroupBox
        Top = 212
        TabOrder = 3
        DesignSize = (
          772
          39)
        inherited edtCoord: TDBEdit
          Width = 750
          Anchors = [akLeft, akTop, akRight, akBottom]
        end
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Height = 212
        inherited dbgOrganizations: TDBGrid
          Height = 195
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 212
      end
      inherited gbDoc: TGroupBox
        Height = 212
      end
      inherited gbOrganization: TGroupBox
        Height = 212
      end
    end
    inherited tshLicenses: TTabSheet
      DesignSize = (
        772
        417)
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 417
        inherited sb: TStatusBar
          Top = 398
        end
        inherited bmf: TBaseMapFrame
          Height = 369
        end
        inherited tb: TToolBar
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
  inherited pnlForAllBottom: TPanel
    Top = 497
    object sbExpToGs1Gt1: TSpeedButton [9]
      Left = 296
      Top = 7
      Width = 23
      Height = 22
      Glyph.Data = {
        36030000424D3603000000000000360000002800000010000000100000000100
        1800000000000003000000000000000000000000000000000000FF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF0000FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FF0000FF0000FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF0000FF0000FF0000FF00
        00FF0000FF0000FF0000FF0000FF0000FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FF0000FF0000FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF0000FFFF00FFFF00FFFF00
        FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF
        800000800000FF00FFFF00FFFF00FF800000800000FF00FFFF00FF8000008000
        00800000FF00FFFF00FFFF00FF800000FF00FFFF00FF800000FF00FF800000FF
        00FFFF00FF800000FF00FFFF00FF800000FF00FFFF00FFFF00FFFF00FF800000
        FF00FF800000800000FF00FFFF00FFFF00FFFF00FF800000FF00FFFF00FF8000
        00FF00FFFF00FFFF00FFFF00FF800000FF00FFFF00FFFF00FFFF00FFFF00FF80
        0000800000FF00FFFF00FFFF00FF800000FF00FFFF00FFFF00FFFF00FF800000
        FF00FFFF00FFFF00FFFF00FF800000FF00FFFF00FFFF00FFFF00FF8000008000
        00FF00FFFF00FFFF00FFFF00FF800000FF00FFFF00FF800000FF00FF800000FF
        00FFFF00FF800000FF00FFFF00FF800000FF00FFFF00FFFF00FFFF00FFFF00FF
        800000800000FF00FFFF00FFFF00FF800000800000FF00FFFF00FFFF00FF8000
        00FF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF
        00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FFFF00FF}
      OnClick = sbExpToGs1Gt1Click
    end
  end
  inherited ibdsStantionsBase: TIBDataSet
    RefreshSQL.Strings = (
      'select   TRN.ID,'
      '            TRN.STAND_ID, '
      '            TRN.ADMINISTRATIONID,'
      '            TRN.DATECREATE,'
      '            TRN.DATECHANGE,'
      '            TRN.OWNER_ID, '
      '            TRN.RESPONSIBLEADMIN,'
      '            TRN.ACCOUNTCONDITION_IN,  '
      '            TRN.ACCOUNTCONDITION_OUT, '
      '            TRN.SYSTEMCAST_ID,'
      '            TRN.CLASSWAVE, '
      '            TRN.TIMETRANSMIT,'
      '            TRN.NAMEPROGRAMM,'
      '            TRN.USERID, '
      '            TRN.ORIGINALID,'
      '            TRN.NUMREGISTRY,'
      '            TRN.TYPEREGISTRY,'
      'TRN.VIDEO_EMISSION,'
      'TRN.SOUND_EMISSION_PRIMARY,'
      'TRN.SOUND_EMISSION_SECOND,'
      #9'TRN.REMARKS,'
      'TRN.REMARKS_ADD,'
      #9'AREA.NUMREGION AREA_NUMREGION,'
      #9'TRN.OPERATOR_ID,'
      #9'OWNER.NAMEORGANIZATION OPERATOR_NAME'
      'from  TRANSMITTERS TRN'
      'left outer join STAND on(TRN.STAND_ID = STAND.ID)'
      'left outer join AREA on(AREA.ID = STAND.AREA_ID)'
      'left outer join OWNER on(OWNER.ID = TRN.OPERATOR_ID)'
      'where TRN.ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS'
      'set       '
      '            STAND_ID = '#9':STAND_ID, '
      '            DATECHANGE = '#9':DATECHANGE,'
      '            OWNER_ID    = '#9':OWNER_ID, '
      '            RESPONSIBLEADMIN          =  :RESPONSIBLEADMIN,'
      '            ACCOUNTCONDITION_IN =     :ACCOUNTCONDITION_IN,  '
      '            ACCOUNTCONDITION_OUT = :ACCOUNTCONDITION_OUT, '
      '            CLASSWAVE = '#9':CLASSWAVE,'
      '            TIMETRANSMIT = '#9':TIMETRANSMIT,'
      '            NAMEPROGRAMM = :NAMEPROGRAMM,'
      '            USERID = '#9#9':USERID, '
      '            ORIGINALID = '#9':ORIGINALID, '
      '            NUMREGISTRY = '#9':NUMREGISTRY,'
      '            TYPEREGISTRY = '#9':TYPEREGISTRY,'
      '            REMARKS ='#9':REMARKS,'
      'REMARKS_ADD = :REMARKS_ADD,'
      #9'OPERATOR_ID = :OPERATOR_ID,'
      'VIDEO_EMISSION = :VIDEO_EMISSION,'
      'SOUND_EMISSION_PRIMARY = :SOUND_EMISSION_PRIMARY,'
      'SOUND_EMISSION_SECOND = :SOUND_EMISSION_SECOND'
      'where ID = :ID')
  end
  inherited ibqAccCondNameIn: TIBQuery
    Left = 116
    Top = 396
  end
  inherited ibqAccCondNameOut: TIBQuery
    Left = 148
    Top = 396
  end
  inherited dsStand: TDataSource
    Left = 364
    Top = 396
  end
  inherited ibqUserName: TIBQuery
    Top = 396
  end
  inherited ibqTRKName: TIBQuery
    Left = 228
    Top = 396
  end
  inherited ibqSystemCastName: TIBQuery
    Left = 292
    Top = 396
  end
  inherited dsTestpoints: TDataSource
    Left = 100
  end
  inherited pmIntoBeforeBase: TPopupMenu
    Left = 184
    Top = 512
  end
  inherited ibqTypeSystemName: TIBQuery
    Left = 328
    Top = 396
  end
  object ibdsTxDigital: TIBDataSet [37]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsTxDigitalAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select '
      #9'TR.ID TR_ID, '
      #9'TR.IDENTIFIERSFN,'
      #9'SN.SYNHRONETID,'
      '                TR.ADM_REF_ID,'
      '                TR.plan_entry,'
      '                TR.assgn_code,'
      '                TR.associated_adm_allot_id,'
      '                TR.associated_allot_sfn_id,'
      '                TR.call_sign,'
      '                TR.SPECT_MASK '
      'from TRANSMITTERS TR '
      'left outer join SYNHROFREQNET SN on (SN.ID = TR.IDENTIFIERSFN)'
      'where TR.ID = :ID')
    SelectSQL.Strings = (
      'select '
      #9'TR.ID, '
      #9'TR.IDENTIFIERSFN,'
      #9'SN.SYNHRONETID,'
      '                TR.ADM_REF_ID,'
      '                TR.plan_entry,'
      '                TR.assgn_code,'
      '                TR.associated_adm_allot_id,'
      '                TR.associated_allot_sfn_id,'
      '                TR.call_sign,'
      '                TR.SPECT_MASK '
      'from TRANSMITTERS TR '
      'left outer join SYNHROFREQNET SN on (SN.ID = TR.IDENTIFIERSFN)'
      'where TR.ID = :ID'
      '')
    ModifySQL.Strings = (
      'update TRANSMITTERS set'
      #9'IDENTIFIERSFN =  :IDENTIFIERSFN,'
      '                ADM_REF_ID = :ADM_REF_ID,'
      '                plan_entry = :plan_entry,'
      '                assgn_code = :assgn_code,'
      
        '                associated_adm_allot_id = :associated_adm_allot_' +
        'id,'
      
        '                associated_allot_sfn_id = :associated_allot_sfn_' +
        'id,'
      '                call_sign = :call_sign,'
      '                SPECT_MASK = :SPECT_MASK'
      'where ID = :ID')
    Left = 380
    Top = 464
    object ibdsTxDigitalIDENTIFIERSFN: TIntegerField
      FieldName = 'IDENTIFIERSFN'
      Origin = 'TRANSMITTERS.IDENTIFIERSFN'
      OnChange = ibdsTxDigitalIDENTIFIERSFNChange
    end
    object ibdsTxDigitalSYNHRONETID: TIBStringField
      FieldName = 'SYNHRONETID'
      Origin = 'SYNHROFREQNET.SYNHRONETID'
      OnChange = ibdsTxDigitalSYNHRONETIDChange
      Size = 30
    end
    object ibdsTxDigitalID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object ibdsTxDigitalADM_REF_ID: TIBStringField
      FieldName = 'ADM_REF_ID'
    end
    object ibdsTxDigitalPLAN_ENTRY: TIntegerField
      FieldName = 'PLAN_ENTRY'
      Origin = 'TRANSMITTERS.PLAN_ENTRY'
    end
    object ibdsTxDigitalASSGN_CODE: TIBStringField
      FieldName = 'ASSGN_CODE'
      Origin = 'TRANSMITTERS.ASSGN_CODE'
      OnChange = ibdsTxDigitalASSGN_CODEChange
      FixedChar = True
      Size = 1
    end
    object ibdsTxDigitalASSOCIATED_ADM_ALLOT_ID: TIBStringField
      FieldName = 'ASSOCIATED_ADM_ALLOT_ID'
      Origin = 'TRANSMITTERS.ASSOCIATED_ADM_ALLOT_ID'
      OnChange = ibdsTxDigitalASSOCIATED_ADM_ALLOT_IDChange
    end
    object ibdsTxDigitalASSOCIATED_ALLOT_SFN_ID: TIBStringField
      FieldName = 'ASSOCIATED_ALLOT_SFN_ID'
      Origin = 'TRANSMITTERS.ASSOCIATED_ALLOT_SFN_ID'
      OnChange = ibdsTxDigitalASSOCIATED_ALLOT_SFN_IDChange
      Size = 30
    end
    object ibdsTxDigitalCALL_SIGN: TIBStringField
      FieldName = 'CALL_SIGN'
      Origin = 'TRANSMITTERS.CALL_SIGN'
      Size = 10
    end
    object ibdsTxDigitalSPECT_MASK: TIBStringField
      FieldName = 'SPECT_MASK'
      Origin = 'TRANSMITTERS.SPECT_MASK'
      FixedChar = True
      Size = 1
    end
  end
  object dsTxDigital: TDataSource [38]
    DataSet = ibdsTxDigital
    OnDataChange = dsTxDigitalDataChange
    Left = 356
    Top = 464
  end
  object mnShowAllotment: TPopupMenu
    Left = 408
    Top = 248
    object mniShowAllotment: TMenuItem
      OnClick = mniShowAllotmentClick
    end
  end
end

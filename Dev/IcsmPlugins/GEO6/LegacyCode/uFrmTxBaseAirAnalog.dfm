inherited frmTxBaseAirAnalog: TfrmTxBaseAirAnalog
  Left = 547
  Top = 140
  Caption = 'frmTxBaseAirAnalog'
  ClientHeight = 512
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited edtID: TDBEdit
      Left = 734
    end
  end
  inherited pcData: TPageControl
    Height = 420
    inherited tshCommon: TTabSheet
      inherited pnlMaster: TPanel
        Top = 228
        TabOrder = 1
      end
      inherited gbxCoordination: TGroupBox
        Top = 189
        TabOrder = 2
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Height = 189
        inherited dbgOrganizations: TDBGrid
          Height = 172
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 189
      end
      inherited gbDoc: TGroupBox
        Height = 189
      end
      inherited gbOrganization: TGroupBox
        Height = 189
      end
    end
    inherited tshEquipment: TTabSheet
      inherited gbEquipment: TGroupBox
        Width = 782
        Height = 251
        inherited dbgEquipment: TDBGrid
          Width = 778
          Height = 234
        end
      end
      inherited pnlSummator: TPanel
        Width = 782
        inherited gbSummator: TGroupBox
          Width = 782
        end
      end
      inherited pnlFreqShift: TPanel
        Width = 782
      end
      inherited pnlEquipButton: TPanel
        Top = 352
        Width = 782
      end
    end
    inherited tshLicenses: TTabSheet
      DesignSize = (
        772
        394)
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Width = 782
        Height = 425
        inherited dbgTestpoints: TDBGrid
          Width = 778
          Height = 408
        end
      end
    end
    inherited tshChangeLog: TTabSheet
      inherited dbgGhangeLog: TDBGrid
        Width = 782
        Height = 425
      end
      inherited panList: TPanel
        Width = 782
        Height = 425
        inherited dgrList: TDBGrid
          Width = 782
          Height = 400
        end
        inherited panSearch: TPanel
          Top = 400
          Width = 782
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 394
        inherited sb: TStatusBar
          Top = 375
        end
        inherited bmf: TBaseMapFrame
          Height = 346
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
    Top = 474
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
  inherited ibdsLicenses: TIBDataSet
    Top = 364
  end
  inherited dsLicenses: TDataSource
    Top = 364
  end
  inherited ibqAccCondNameIn: TIBQuery
    Left = 108
    Top = 408
  end
  inherited ibqAccCondNameOut: TIBQuery
    Left = 140
    Top = 408
  end
  inherited dsStand: TDataSource
    Left = 356
    Top = 408
  end
  inherited ibqUserName: TIBQuery
    Left = 172
    Top = 408
  end
  inherited ibqTRKName: TIBQuery
    Left = 220
    Top = 408
  end
  inherited ibqSystemCastName: TIBQuery
    Left = 284
    Top = 408
  end
  inherited ibqDocType: TIBQuery
    Left = 456
    Top = 368
  end
  inherited ibdsTestpoint: TIBDataSet
    DeleteSQL.Strings = (
      'delete from TESTPOINTS where TRANSMITTERS_ID = :TRANSMITTERS_ID')
    Left = 100
    Top = 396
  end
  inherited ImageList1: TImageList
    Left = 756
  end
  object ibdsRetranslate: TIBDataSet [28]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsRetranslateAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select      TRIN.ID,'
      #9'TRIN.RELAYSTATION_ID,'
      #9'TR.ADMINISTRATIONID,'
      #9'S.NAMESITE,'
      #9'CH.NAMECHANNEL,'#9
      #9'TRIN.TYPERECEIVE_ID,'
      #9'RT.NAME TYPEREC_NAME'
      'from TRANSMITTERS TRIN'
      
        'left outer join TRANSMITTERS TR on  (TR.ID  = TRIN.RELAYSTATION_' +
        'ID)'
      'left outer join STAND S on  (S.ID = TR.STAND_ID)'
      'left outer join CHANNELS CH on (CH.ID = TR.CHANNEL_ID)'
      
        'left outer join TYPERECEIVE RT on (RT.ID  = TRIN.TYPERECEIVE_ID ' +
        ')'
      'where TRIN.ID = :ID')
    SelectSQL.Strings = (
      'select      TRIN.ID,'
      #9'TRIN.RELAYSTATION_ID,'
      #9'TR.ADMINISTRATIONID,'
      #9'S.NAMESITE,'
      #9'CH.NAMECHANNEL,'#9
      #9'TRIN.TYPERECEIVE_ID,'
      #9'RT.NAME TYPEREC_NAME'
      'from TRANSMITTERS TRIN'
      
        'left outer join TRANSMITTERS TR on  (TR.ID  = TRIN.RELAYSTATION_' +
        'ID)'
      'left outer join STAND S on  (S.ID = TR.STAND_ID)'
      'left outer join CHANNELS CH on (CH.ID = TR.CHANNEL_ID)'
      
        'left outer join TYPERECEIVE RT on (RT.ID  = TRIN.TYPERECEIVE_ID ' +
        ')'
      'where TRIN.ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS set      '
      #9'RELAYSTATION_ID = :RELAYSTATION_ID,'
      #9'TYPERECEIVE_ID = :TYPERECEIVE_ID'
      'where ID = :ID')
    Left = 396
    Top = 408
    object ibdsRetranslateID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
    end
    object ibdsRetranslateADMINISTRATIONID: TIBStringField
      FieldName = 'ADMINISTRATIONID'
      Origin = 'TRANSMITTERS.ADMINISTRATIONID'
      Size = 4
    end
    object ibdsRetranslateNAMESITE: TIBStringField
      FieldName = 'NAMESITE'
      Origin = 'STAND.NAMESITE'
      Size = 32
    end
    object ibdsRetranslateNAMECHANNEL: TIBStringField
      FieldName = 'NAMECHANNEL'
      Origin = 'CHANNELS.NAMECHANNEL'
      Size = 4
    end
    object ibdsRetranslateTYPEREC_NAME: TIBStringField
      FieldName = 'TYPEREC_NAME'
      Origin = 'TYPERECEIVE.NAME'
      Size = 16
    end
    object ibdsRetranslateRELAYSTATION_ID: TIntegerField
      FieldName = 'RELAYSTATION_ID'
      Origin = 'TRANSMITTERS.RELAYSTATION_ID'
    end
    object ibdsRetranslateTYPERECEIVE_ID: TIntegerField
      FieldName = 'TYPERECEIVE_ID'
      Origin = 'TRANSMITTERS.TYPERECEIVE_ID'
    end
  end
  object dsRetranslate: TDataSource [29]
    DataSet = ibdsRetranslate
    Left = 428
    Top = 408
  end
  inherited ibqTypeSystemName: TIBQuery
    Left = 320
    Top = 408
  end
  inherited ibdsAir: TIBDataSet
    Top = 392
  end
  inherited dsAir: TDataSource
    Top = 392
  end
  inherited ibqStand: TIBQuery
    Left = 8
    Top = 408
  end
end

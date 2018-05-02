object dmMain: TdmMain
  OldCreateOrder = False
  OnDestroy = DataModuleDestroy
  Left = 985
  Top = 341
  Height = 442
  Width = 375
  object dbMain: TIBDatabase
    DatabaseName = 'D:\SEV\DataBases\LISBC-6-RRC-LIS.GDB'
    Params.Strings = (
      'user_name=sysdba'
      'password=masterkey'
      'lc_ctype=WIN1251')
    LoginPrompt = False
    DefaultTransaction = trMain
    IdleTimer = 0
    SQLDialect = 3
    TraceFlags = []
    AllowStreamedConnected = False
    AfterConnect = dbMainAfterConnect
    Left = 40
    Top = 20
  end
  object trMain: TIBTransaction
    Active = False
    DefaultDatabase = dbMain
    Params.Strings = (
      'read_committed'
      'rec_version'
      'nowait')
    AutoStopAction = saNone
    Left = 152
    Top = 8
  end
  object sqlUserId: TIBSQL
    Database = dbMain
    ParamCheck = True
    SQL.Strings = (
      'select OUT_ID from SP_GET_USERID')
    Transaction = trMain
    Left = 40
    Top = 72
  end
  object sqlGetNewId: TIBSQL
    Database = dbMain
    ParamCheck = True
    SQL.Strings = (
      'select OUT_ID from SP_GEN_ID')
    Transaction = trMain
    Left = 152
    Top = 64
  end
  object sqlUserLog: TIBSQL
    Database = dbMain
    ParamCheck = True
    SQL.Strings = (
      'execute PROCEDURE SP_ACTIVE_USER(:Action)')
    Transaction = trMain
    Left = 96
    Top = 48
  end
  object ibdsCoordination: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      
        'select OVERLAND, OVERCOLDSEA, OVERWARMSEA, GENERALLYSEA100, GENE' +
        'RALLYSEA20, GENERALLYSEA40, GENERALLYSEA60, GENERALLYSEA80, MEDI' +
        'TERRANEANSEA100, MEDITERRANEANSEA20, MEDITERRANEANSEA40, MEDITER' +
        'RANEANSEA60, MEDITERRANEANSEA80 '
      'from COORDDISTANCE'
      'where SYSTEMCAST_ID = :SYSTEMCAST_ID'
      'and EFFECTRADIATEPOWER = :EFFECTRADIATEPOWER'
      'and HEIGHTANTENNA = :HEIGHTANTENNA')
    Left = 40
    Top = 136
  end
  object ibdsChannelList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAMECHANNEL, FREQUENCYGRID_ID from CHANNELS')
    Left = 184
    Top = 304
    object ibdsChannelListID: TIntegerField
      FieldName = 'ID'
      Origin = 'CHANNELS.ID'
      Required = True
    end
    object ibdsChannelListNAMECHANNEL: TIBStringField
      FieldName = 'NAMECHANNEL'
      Origin = 'CHANNELS.NAMECHANNEL'
      Size = 4
    end
    object ibdsChannelListFREQUENCYGRID_ID: TIntegerField
      FieldName = 'FREQUENCYGRID_ID'
      Origin = 'CHANNELS.FREQUENCYGRID_ID'
      Required = True
    end
  end
  object ibdsAccCondList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, CODE, NAME, TYPECONDITION from ACCOUNTCONDITION')
    Left = 40
    Top = 304
    object ibdsAccCondListID: TIntegerField
      FieldName = 'ID'
      Origin = 'ACCOUNTCONDITION.ID'
      Required = True
    end
    object ibdsAccCondListCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'ACCOUNTCONDITION.CODE'
      Size = 4
    end
    object ibdsAccCondListNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'ACCOUNTCONDITION.NAME'
      Size = 32
    end
    object ibdsAccCondListTYPECONDITION: TSmallintField
      FieldName = 'TYPECONDITION'
      Origin = 'ACCOUNTCONDITION.TYPECONDITION'
    end
  end
  object ibdsScList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, CODE, ENUMVAL from SYSTEMCAST')
    Left = 40
    Top = 208
    object ibdsScListID: TIntegerField
      FieldName = 'ID'
      Origin = 'SYSTEMCAST.ID'
      Required = True
    end
    object ibdsScListCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'SYSTEMCAST.CODE'
      Size = 4
    end
    object ibdsScListENUMVAL: TSmallintField
      FieldName = 'ENUMVAL'
      Origin = 'SYSTEMCAST.ENUMVAL'
      Required = True
    end
  end
  object ibdsAtsList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      
        'select ID, NAMESYSTEM, ENUMVAL, FREQUENCYGRID_ID from ANALOGTELE' +
        'SYSTEM')
    Left = 96
    Top = 208
    object ibdsAtsListID: TIntegerField
      FieldName = 'ID'
      Origin = 'ANALOGTELESYSTEM.ID'
      Required = True
    end
    object ibdsAtsListNAMESYSTEM: TIBStringField
      FieldName = 'NAMESYSTEM'
      Origin = 'ANALOGTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object ibdsAtsListENUMVAL: TSmallintField
      FieldName = 'ENUMVAL'
      Origin = 'ANALOGTELESYSTEM.ENUMVAL'
      Required = True
    end
    object ibdsAtsListFREQUENCYGRID_ID: TIntegerField
      FieldName = 'FREQUENCYGRID_ID'
      Origin = 'ANALOGTELESYSTEM.FREQUENCYGRID_ID'
      Required = True
    end
  end
  object ibdsArsList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, CODSYSTEM, ENUMVAL from ANALOGRADIOSYSTEM')
    Left = 168
    Top = 208
    object ibdsArsListID: TIntegerField
      FieldName = 'ID'
      Origin = 'ANALOGRADIOSYSTEM.ID'
      Required = True
    end
    object ibdsArsListCODSYSTEM: TIBStringField
      FieldName = 'CODSYSTEM'
      Origin = 'ANALOGRADIOSYSTEM.CODSYSTEM'
      Size = 8
    end
    object ibdsArsListENUMVAL: TIntegerField
      FieldName = 'ENUMVAL'
      Origin = 'ANALOGRADIOSYSTEM.ENUMVAL'
      Required = True
    end
  end
  object ibdsDabBlockName: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAME, CENTREFREQ from BLOCKDAB')
    Left = 40
    Top = 264
    object ibdsDabBlockNameID: TIntegerField
      FieldName = 'ID'
      Origin = 'BLOCKDAB.ID'
      Required = True
    end
    object ibdsDabBlockNameNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'BLOCKDAB.NAME'
      Size = 4
    end
    object ibdsDabBlockNameCENTREFREQ: TFloatField
      FieldName = 'CENTREFREQ'
      Origin = 'BLOCKDAB.CENTREFREQ'
    end
  end
  object ibdsDvbSystemList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAMESYSTEM, ENUMVAL from DIGITALTELESYSTEM')
    Left = 240
    Top = 208
    object ibdsDvbSystemListID: TIntegerField
      FieldName = 'ID'
      Origin = 'DIGITALTELESYSTEM.ID'
      Required = True
    end
    object ibdsDvbSystemListNAMESYSTEM: TIBStringField
      FieldName = 'NAMESYSTEM'
      Origin = 'DIGITALTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object ibdsDvbSystemListENUMVAL: TIntegerField
      FieldName = 'ENUMVAL'
      Origin = 'DIGITALTELESYSTEM.ENUMVAL'
      Required = True
    end
  end
  object ibdsCoordDist: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select * from COORDDISTANCE')
    Left = 184
    Top = 256
    object ibdsCoordDistID: TIntegerField
      FieldName = 'ID'
      Origin = 'COORDDISTANCE.ID'
      Required = True
    end
    object ibdsCoordDistSYSTEMCAST_ID: TIntegerField
      FieldName = 'SYSTEMCAST_ID'
      Origin = 'COORDDISTANCE.SYSTEMCAST_ID'
      Required = True
    end
    object ibdsCoordDistEFFECTRADIATEPOWER: TIntegerField
      FieldName = 'EFFECTRADIATEPOWER'
      Origin = 'COORDDISTANCE.EFFECTRADIATEPOWER'
    end
    object ibdsCoordDistHEIGHTANTENNA: TIntegerField
      FieldName = 'HEIGHTANTENNA'
      Origin = 'COORDDISTANCE.HEIGHTANTENNA'
    end
    object ibdsCoordDistOVERLAND: TSmallintField
      FieldName = 'OVERLAND'
      Origin = 'COORDDISTANCE.OVERLAND'
    end
    object ibdsCoordDistOVERCOLDSEA: TSmallintField
      FieldName = 'OVERCOLDSEA'
      Origin = 'COORDDISTANCE.OVERCOLDSEA'
    end
    object ibdsCoordDistOVERWARMSEA: TSmallintField
      FieldName = 'OVERWARMSEA'
      Origin = 'COORDDISTANCE.OVERWARMSEA'
    end
    object ibdsCoordDistGENERALLYSEA20: TSmallintField
      FieldName = 'GENERALLYSEA20'
      Origin = 'COORDDISTANCE.GENERALLYSEA20'
    end
    object ibdsCoordDistGENERALLYSEA40: TSmallintField
      FieldName = 'GENERALLYSEA40'
      Origin = 'COORDDISTANCE.GENERALLYSEA40'
    end
    object ibdsCoordDistGENERALLYSEA60: TSmallintField
      FieldName = 'GENERALLYSEA60'
      Origin = 'COORDDISTANCE.GENERALLYSEA60'
    end
    object ibdsCoordDistGENERALLYSEA80: TSmallintField
      FieldName = 'GENERALLYSEA80'
      Origin = 'COORDDISTANCE.GENERALLYSEA80'
    end
    object ibdsCoordDistGENERALLYSEA100: TSmallintField
      FieldName = 'GENERALLYSEA100'
      Origin = 'COORDDISTANCE.GENERALLYSEA100'
    end
    object ibdsCoordDistMEDITERRANEANSEA20: TSmallintField
      FieldName = 'MEDITERRANEANSEA20'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA20'
    end
    object ibdsCoordDistMEDITERRANEANSEA40: TSmallintField
      FieldName = 'MEDITERRANEANSEA40'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA40'
    end
    object ibdsCoordDistMEDITERRANEANSEA60: TSmallintField
      FieldName = 'MEDITERRANEANSEA60'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA60'
    end
    object ibdsCoordDistMEDITERRANEANSEA80: TSmallintField
      FieldName = 'MEDITERRANEANSEA80'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA80'
    end
    object ibdsCoordDistMEDITERRANEANSEA100: TSmallintField
      FieldName = 'MEDITERRANEANSEA100'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA100'
    end
  end
  object ibdsCheckPoints: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select CP.LATITUDE,'
      '          CP.LONGITUDE,'
      '          CN.COUNTRY_ID,'
      '          CP.NUMBOUND'
      'from COORDPOINTS CP'
      'left outer join COUNTRYPOINTS'
      '    CN on (CP.NUMBOUND = CN.NUMBOUND)')
    Left = 240
    Top = 256
    object ibdsCheckPointsLATITUDE: TFloatField
      FieldName = 'LATITUDE'
      Origin = 'COORDPOINTS.LATITUDE'
    end
    object ibdsCheckPointsLONGITUDE: TFloatField
      FieldName = 'LONGITUDE'
      Origin = 'COORDPOINTS.LONGITUDE'
    end
    object ibdsCheckPointsCOUNTRY_ID: TIntegerField
      FieldName = 'COUNTRY_ID'
      Origin = 'COUNTRYPOINTS.COUNTRY_ID'
    end
    object ibdsCheckPointsNUMBOUND: TIntegerField
      FieldName = 'NUMBOUND'
      Origin = 'COORDPOINTS.NUMBOUND'
    end
  end
  object ibdsCountries: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAME, CODE from COUNTRY')
    Left = 240
    Top = 304
    object ibdsCountriesID: TIntegerField
      FieldName = 'ID'
      Origin = 'COUNTRY.ID'
      Required = True
    end
    object ibdsCountriesNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'COUNTRY.NAME'
      Size = 32
    end
    object ibdsCountriesCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
  end
  object qryTxList: TIBQuery
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select TX.ID, tx.LATITUDE, tx.LONGITUDE, tx.SYSTEMCAST_ID '
      ' , tx.TYPESYSTEM, tx.VIDEO_CARRIER '
      ',tx.VIDEO_OFFSET_LINE, tx.VIDEO_OFFSET_HERZ, tx.TYPEOFFSET '
      ', tx.SYSTEMCOLOUR,  tx.POWER_VIDEO, tx.EPR_VIDEO_MAX '
      ', tx.EPR_VIDEO_HOR, tx.EPR_VIDEO_VERT '
      ', tx.IDENTIFIERSFN, tx.RELATIVETIMINGSFN, tx.CHANNEL_ID '
      ', tx.BLOCKCENTREFREQ, tx.SOUND_CARRIER_PRIMARY   '
      
        ', tx.POWER_SOUND_PRIMARY, tx.EPR_SOUND_MAX_PRIMARY, tx.EPR_SOUND' +
        '_HOR_PRIMARY '
      
        ', tx.EPR_SOUND_VERT_PRIMARY,  tx.V_SOUND_RATIO_PRIMARY, tx.MONOS' +
        'TEREO_PRIMARY '
      ', tx.SOUND_CARRIER_SECOND '
      
        ', tx.POWER_SOUND_SECOND, tx.EPR_SOUND_MAX_SECOND, tx.EPR_SOUND_H' +
        'OR_SECOND '
      
        ', tx.EPR_SOUND_VER_SECOND, tx.SOUND_SYSTEM_SECOND, tx.V_SOUND_RA' +
        'TIO_SECOND '
      
        ', tx.HEIGHTANTENNA, tx.HEIGHT_EFF_MAX, tx.POLARIZATION, tx.DIREC' +
        'TION '
      
        ', tx.FIDERLOSS, tx.FIDERLENGTH, tx.ANGLEELEVATION_HOR, tx.ANGLEE' +
        'LEVATION_VER, tx.ANTENNAGAIN '
      
        ', tx.TESTPOINTSIS, tx.ACCOUNTCONDITION_IN, tx.ACCOUNTCONDITION_O' +
        'UT, tx.ADMINISTRATIONID, tx.STAND_ID '
      
        ', tx.EFFECTPOWERHOR, tx.EFFECTPOWERVER, tx.EFFECTHEIGHT, tx.ANT_' +
        'DIAG_H, tx.ANT_DIAG_V '
      
        ' ,CN.CODE adm_response, CN.CODE adm_sited_in, TX.STATUS status_c' +
        'ode, ST.NAMESITE_ENG station_name '
      
        ', ST.HEIGHT_SEA site_height, CH.NAMECHANNEL channel, TX.DATECHAN' +
        'GE date_of_last_change '
      ', AR.NUMREGION NUMREGION '
      'from TRANSMITTERS TX '
      'LEFT OUTER JOIN STAND ST on (TX.STAND_ID = ST.ID) '
      'LEFT OUTER JOIN AREA AR on (ST.AREA_ID = AR.ID) '
      'LEFT OUTER JOIN COUNTRY CN on (AR.COUNTRY_ID = CN.ID) '
      'LEFT OUTER JOIN CHANNELS CH on (TX.CHANNEL_ID = CH.ID) ')
    Left = 224
    Top = 16
    object qryTxListID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object qryTxListLATITUDE: TFloatField
      FieldName = 'LATITUDE'
      Origin = 'TRANSMITTERS.LATITUDE'
      Required = True
    end
    object qryTxListLONGITUDE: TFloatField
      FieldName = 'LONGITUDE'
      Origin = 'TRANSMITTERS.LONGITUDE'
      Required = True
    end
    object qryTxListSYSTEMCAST_ID: TIntegerField
      FieldName = 'SYSTEMCAST_ID'
      Origin = 'TRANSMITTERS.SYSTEMCAST_ID'
      Required = True
    end
    object qryTxListTYPESYSTEM: TSmallintField
      FieldName = 'TYPESYSTEM'
      Origin = 'TRANSMITTERS.TYPESYSTEM'
    end
    object qryTxListVIDEO_CARRIER: TFloatField
      FieldName = 'VIDEO_CARRIER'
      Origin = 'TRANSMITTERS.VIDEO_CARRIER'
    end
    object qryTxListVIDEO_OFFSET_LINE: TSmallintField
      FieldName = 'VIDEO_OFFSET_LINE'
      Origin = 'TRANSMITTERS.VIDEO_OFFSET_LINE'
    end
    object qryTxListVIDEO_OFFSET_HERZ: TIntegerField
      FieldName = 'VIDEO_OFFSET_HERZ'
      Origin = 'TRANSMITTERS.VIDEO_OFFSET_HERZ'
    end
    object qryTxListTYPEOFFSET: TIBStringField
      FieldName = 'TYPEOFFSET'
      Origin = 'TRANSMITTERS.TYPEOFFSET'
      Size = 16
    end
    object qryTxListSYSTEMCOLOUR: TIBStringField
      FieldName = 'SYSTEMCOLOUR'
      Origin = 'TRANSMITTERS.SYSTEMCOLOUR'
      Size = 8
    end
    object qryTxListPOWER_VIDEO: TFloatField
      FieldName = 'POWER_VIDEO'
      Origin = 'TRANSMITTERS.POWER_VIDEO'
    end
    object qryTxListEPR_VIDEO_MAX: TFloatField
      FieldName = 'EPR_VIDEO_MAX'
      Origin = 'TRANSMITTERS.EPR_VIDEO_MAX'
    end
    object qryTxListEPR_VIDEO_HOR: TFloatField
      FieldName = 'EPR_VIDEO_HOR'
      Origin = 'TRANSMITTERS.EPR_VIDEO_HOR'
    end
    object qryTxListEPR_VIDEO_VERT: TFloatField
      FieldName = 'EPR_VIDEO_VERT'
      Origin = 'TRANSMITTERS.EPR_VIDEO_VERT'
    end
    object qryTxListIDENTIFIERSFN: TIntegerField
      FieldName = 'IDENTIFIERSFN'
      Origin = 'TRANSMITTERS.IDENTIFIERSFN'
    end
    object qryTxListRELATIVETIMINGSFN: TIntegerField
      FieldName = 'RELATIVETIMINGSFN'
      Origin = 'TRANSMITTERS.RELATIVETIMINGSFN'
    end
    object qryTxListCHANNEL_ID: TIntegerField
      FieldName = 'CHANNEL_ID'
      Origin = 'TRANSMITTERS.CHANNEL_ID'
    end
    object qryTxListBLOCKCENTREFREQ: TFloatField
      FieldName = 'BLOCKCENTREFREQ'
      Origin = 'TRANSMITTERS.BLOCKCENTREFREQ'
    end
    object qryTxListSOUND_CARRIER_PRIMARY: TFloatField
      FieldName = 'SOUND_CARRIER_PRIMARY'
      Origin = 'TRANSMITTERS.SOUND_CARRIER_PRIMARY'
    end
    object qryTxListPOWER_SOUND_PRIMARY: TFloatField
      FieldName = 'POWER_SOUND_PRIMARY'
      Origin = 'TRANSMITTERS.POWER_SOUND_PRIMARY'
    end
    object qryTxListEPR_SOUND_MAX_PRIMARY: TFloatField
      FieldName = 'EPR_SOUND_MAX_PRIMARY'
      Origin = 'TRANSMITTERS.EPR_SOUND_MAX_PRIMARY'
    end
    object qryTxListEPR_SOUND_HOR_PRIMARY: TFloatField
      FieldName = 'EPR_SOUND_HOR_PRIMARY'
      Origin = 'TRANSMITTERS.EPR_SOUND_HOR_PRIMARY'
    end
    object qryTxListEPR_SOUND_VERT_PRIMARY: TFloatField
      FieldName = 'EPR_SOUND_VERT_PRIMARY'
      Origin = 'TRANSMITTERS.EPR_SOUND_VERT_PRIMARY'
    end
    object qryTxListV_SOUND_RATIO_PRIMARY: TFloatField
      FieldName = 'V_SOUND_RATIO_PRIMARY'
      Origin = 'TRANSMITTERS.V_SOUND_RATIO_PRIMARY'
    end
    object qryTxListMONOSTEREO_PRIMARY: TSmallintField
      FieldName = 'MONOSTEREO_PRIMARY'
      Origin = 'TRANSMITTERS.MONOSTEREO_PRIMARY'
    end
    object qryTxListSOUND_CARRIER_SECOND: TFloatField
      FieldName = 'SOUND_CARRIER_SECOND'
      Origin = 'TRANSMITTERS.SOUND_CARRIER_SECOND'
    end
    object qryTxListPOWER_SOUND_SECOND: TFloatField
      FieldName = 'POWER_SOUND_SECOND'
      Origin = 'TRANSMITTERS.POWER_SOUND_SECOND'
    end
    object qryTxListEPR_SOUND_MAX_SECOND: TFloatField
      FieldName = 'EPR_SOUND_MAX_SECOND'
      Origin = 'TRANSMITTERS.EPR_SOUND_MAX_SECOND'
    end
    object qryTxListEPR_SOUND_HOR_SECOND: TFloatField
      FieldName = 'EPR_SOUND_HOR_SECOND'
      Origin = 'TRANSMITTERS.EPR_SOUND_HOR_SECOND'
    end
    object qryTxListEPR_SOUND_VER_SECOND: TFloatField
      FieldName = 'EPR_SOUND_VER_SECOND'
      Origin = 'TRANSMITTERS.EPR_SOUND_VER_SECOND'
    end
    object qryTxListSOUND_SYSTEM_SECOND: TIBStringField
      FieldName = 'SOUND_SYSTEM_SECOND'
      Origin = 'TRANSMITTERS.SOUND_SYSTEM_SECOND'
      Size = 8
    end
    object qryTxListV_SOUND_RATIO_SECOND: TFloatField
      FieldName = 'V_SOUND_RATIO_SECOND'
      Origin = 'TRANSMITTERS.V_SOUND_RATIO_SECOND'
    end
    object qryTxListHEIGHTANTENNA: TIntegerField
      FieldName = 'HEIGHTANTENNA'
      Origin = 'TRANSMITTERS.HEIGHTANTENNA'
    end
    object qryTxListHEIGHT_EFF_MAX: TIntegerField
      FieldName = 'HEIGHT_EFF_MAX'
      Origin = 'TRANSMITTERS.HEIGHT_EFF_MAX'
    end
    object qryTxListPOLARIZATION: TIBStringField
      FieldName = 'POLARIZATION'
      Origin = 'TRANSMITTERS.POLARIZATION'
      FixedChar = True
      Size = 1
    end
    object qryTxListDIRECTION: TIBStringField
      FieldName = 'DIRECTION'
      Origin = 'TRANSMITTERS.DIRECTION'
      FixedChar = True
      Size = 2
    end
    object qryTxListFIDERLOSS: TFloatField
      FieldName = 'FIDERLOSS'
      Origin = 'TRANSMITTERS.FIDERLOSS'
    end
    object qryTxListFIDERLENGTH: TIntegerField
      FieldName = 'FIDERLENGTH'
      Origin = 'TRANSMITTERS.FIDERLENGTH'
    end
    object qryTxListANGLEELEVATION_HOR: TSmallintField
      FieldName = 'ANGLEELEVATION_HOR'
      Origin = 'TRANSMITTERS.ANGLEELEVATION_HOR'
    end
    object qryTxListANGLEELEVATION_VER: TSmallintField
      FieldName = 'ANGLEELEVATION_VER'
      Origin = 'TRANSMITTERS.ANGLEELEVATION_VER'
    end
    object qryTxListANTENNAGAIN: TFloatField
      FieldName = 'ANTENNAGAIN'
      Origin = 'TRANSMITTERS.ANTENNAGAIN'
    end
    object qryTxListTESTPOINTSIS: TSmallintField
      FieldName = 'TESTPOINTSIS'
      Origin = 'TRANSMITTERS.TESTPOINTSIS'
    end
    object qryTxListACCOUNTCONDITION_IN: TSmallintField
      FieldName = 'ACCOUNTCONDITION_IN'
      Origin = 'TRANSMITTERS.ACCOUNTCONDITION_IN'
    end
    object qryTxListACCOUNTCONDITION_OUT: TSmallintField
      FieldName = 'ACCOUNTCONDITION_OUT'
      Origin = 'TRANSMITTERS.ACCOUNTCONDITION_OUT'
    end
    object qryTxListADMINISTRATIONID: TIBStringField
      FieldName = 'ADMINISTRATIONID'
      Origin = 'TRANSMITTERS.ADMINISTRATIONID'
      Size = 4
    end
    object qryTxListSTAND_ID: TIntegerField
      FieldName = 'STAND_ID'
      Origin = 'TRANSMITTERS.STAND_ID'
    end
    object qryTxListEFFECTPOWERHOR: TBlobField
      FieldName = 'EFFECTPOWERHOR'
      Origin = 'TRANSMITTERS.EFFECTPOWERHOR'
      Size = 8
    end
    object qryTxListEFFECTPOWERVER: TBlobField
      FieldName = 'EFFECTPOWERVER'
      Origin = 'TRANSMITTERS.EFFECTPOWERVER'
      Size = 8
    end
    object qryTxListEFFECTHEIGHT: TBlobField
      FieldName = 'EFFECTHEIGHT'
      Origin = 'TRANSMITTERS.EFFECTHEIGHT'
      Size = 8
    end
    object qryTxListANT_DIAG_H: TBlobField
      FieldName = 'ANT_DIAG_H'
      Origin = 'TRANSMITTERS.ANT_DIAG_H'
      Size = 8
    end
    object qryTxListANT_DIAG_V: TBlobField
      FieldName = 'ANT_DIAG_V'
      Origin = 'TRANSMITTERS.ANT_DIAG_V'
      Size = 8
    end
    object qryTxListSC_ENUMVAL: TSmallintField
      FieldKind = fkLookup
      FieldName = 'SC_ENUMVAL'
      LookupDataSet = ibdsScList
      LookupKeyFields = 'ID'
      LookupResultField = 'ENUMVAL'
      KeyFields = 'SYSTEMCAST_ID'
      Origin = 'SYSTEMCAST.ENUMVAL'
      Lookup = True
    end
    object qryTxListAT_ENUMVAL: TSmallintField
      FieldKind = fkLookup
      FieldName = 'AT_ENUMVAL'
      LookupDataSet = ibdsAtsList
      LookupKeyFields = 'ID'
      LookupResultField = 'ENUMVAL'
      KeyFields = 'TYPESYSTEM'
      Origin = 'ANALOGTELESYSTEM.ENUMVAL'
      Lookup = True
    end
    object qryTxListDTS_ENUMVAL: TIntegerField
      FieldKind = fkLookup
      FieldName = 'DTS_ENUMVAL'
      LookupDataSet = ibdsDvbSystemList
      LookupKeyFields = 'ID'
      LookupResultField = 'ENUMVAL'
      KeyFields = 'TYPESYSTEM'
      Origin = 'DIGITALTELESYSTEM.ENUMVAL'
      Lookup = True
    end
    object qryTxListARS_ENUMVAL: TIntegerField
      FieldKind = fkLookup
      FieldName = 'ARS_ENUMVAL'
      LookupDataSet = ibdsArsList
      LookupKeyFields = 'ID'
      LookupResultField = 'ENUMVAL'
      KeyFields = 'TYPESYSTEM'
      Origin = 'ANALOGRADIOSYSTEM.ENUMVAL'
      Lookup = True
    end
    object qryTxListADM_RESPONSE: TIBStringField
      FieldName = 'ADM_RESPONSE'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
    object qryTxListADM_SITED_IN: TIBStringField
      FieldName = 'ADM_SITED_IN'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
    object qryTxListSTATUS_CODE: TSmallintField
      FieldName = 'STATUS_CODE'
      Origin = 'TRANSMITTERS.STATUS'
    end
    object qryTxListSTATION_NAME: TIBStringField
      FieldName = 'STATION_NAME'
      Origin = 'STAND.NAMESITE_ENG'
      Size = 32
    end
    object qryTxListSITE_HEIGHT: TIntegerField
      FieldName = 'SITE_HEIGHT'
      Origin = 'STAND.HEIGHT_SEA'
    end
    object qryTxListCHANNEL: TIBStringField
      FieldName = 'CHANNEL'
      Origin = 'CHANNELS.NAMECHANNEL'
      Size = 4
    end
    object qryTxListDATE_OF_LAST_CHANGE: TDateField
      FieldName = 'DATE_OF_LAST_CHANGE'
      Origin = 'TRANSMITTERS.DATECHANGE'
      Required = True
    end
    object qryTxListNUMREGION: TIBStringField
      FieldName = 'NUMREGION'
      Origin = 'AREA.NUMREGION'
      Size = 4
    end
    object qryTxListRPC: TIntegerField
      FieldName = 'RPC'
    end
    object qryTxListRX_MODE: TIntegerField
      FieldName = 'RX_MODE'
    end
    object qryTxListPOL_ISOL: TFloatField
      FieldName = 'POL_ISOL'
    end
    object qryTxListGND_COND: TFloatField
      FieldName = 'GND_COND'
    end
  end
  object ibdsStands: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      
        ' SELECT STAND.ID, STAND.NAMESITE, AREA.NAME AREA_NAME, CITY.NAME' +
        ' CITY_NAME, AREA.NUMREGION, AREA.COUNTRY_ID'
      ' FROM STAND '
      '   LEFT OUTER JOIN AREA ON (STAND.AREA_ID = AREA.ID) '
      '   LEFT OUTER JOIN CITY ON (STAND.CITY_ID = CITY.ID) ')
    Left = 280
    Top = 16
    object ibdsStandsID: TIntegerField
      FieldName = 'ID'
      Origin = 'STAND.ID'
      Required = True
    end
    object ibdsStandsNAMESITE: TIBStringField
      FieldName = 'NAMESITE'
      Origin = 'STAND.NAMESITE'
      Size = 32
    end
    object ibdsStandsAREA_NAME: TIBStringField
      FieldName = 'AREA_NAME'
      Origin = 'AREA.NAME'
      Size = 32
    end
    object ibdsStandsCITY_NAME: TIBStringField
      FieldName = 'CITY_NAME'
      Origin = 'CITY.NAME'
      Size = 32
    end
    object ibdsStandsNUMREGION: TIBStringField
      FieldName = 'NUMREGION'
      Origin = 'AREA.NUMREGION'
      Size = 4
    end
    object ibdsStandsCOUNTRY_ID: TIntegerField
      FieldName = 'COUNTRY_ID'
      Origin = 'AREA.COUNTRY_ID'
    end
  end
  object sqlTxSysCast: TIBSQL
    Database = dbMain
    ParamCheck = True
    SQL.Strings = (
      'select s.enumval from systemcast s'
      'inner join transmitters t on (t.systemcast_id = s.id)'
      'where t.id = :id')
    Transaction = trMain
    Left = 198
    Top = 103
  end
  object ibdsLfMfList: TIBDataSet
    Database = dbMain
    Transaction = trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ENUM, CODE from LFMF_SYSTEM')
    Left = 304
    Top = 192
    object ibdsLfMfListCODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'LFMF_SYSTEM.CODE'
      Size = 8
    end
    object ibdsLfMfListENUM: TIntegerField
      FieldName = 'ENUM'
      Origin = 'LFMF_SYSTEM.ENUM'
      Required = True
    end
  end
  object ibqTxOuery: TIBQuery
    Database = dbMain
    Transaction = trMain
    OnCalcFields = ibqTxOueryCalcFields
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'SELECT'
      ' TX.ID,'
      ' cast('#39'now'#39' as TIMESTAMP) as DOC_CURRENT_TIME,'
      ' cast('#39'today'#39' as TIMESTAMP) as DOC_CURRENT_DATE,'
      ' TX.OP_AGCY,'
      ' TX.ADDR_CODE,'
      ' TX.GND_COND,'
      ' TX.MAX_COORD_DIST,'
      ' TX.stand_id,'
      ' TX.DATECREATE,'
      ' TX.DATECHANGE,'
      ' TX.status,'
      ' TX.OWNER_ID,'
      ' TX.LICENSE_CHANNEL_ID,'
      ' TX.LICENSE_RFR_ID,'
      ' TX.LICENSE_SERVICE_ID,'
      ' TX.NUMPERMBUILD,'
      ' TX.DATEPERMBUILDFROM,'
      ' TX.DATEPERMBUILDTO,'
      ' TX.NUMPERMUSE,'
      ' TX.DATEPERMUSEFROM,'
      ' TX.DATEPERMUSETO,'
      ' TX.REGIONALCOUNCIL,'
      ' TX.NUMPERMREGCOUNCIL,'
      ' TX.DATEPERMREGCOUNCIL,'
      ' TX.NOTICECOUNT,'
      ' TX.NUMSTANDCERTIFICATE,'
      ' TX.DATESTANDCERTIFICATE,'
      ' TX.NUMFACTORY,'
      ' TX.RESPONSIBLEADMIN,'
      ' TX.ADMINISTRATIONID,'
      ' TX.REGIONALAGREEMENT,'
      ' TX.DATEINTENDUSE,'
      ' TX.AREACOVERAGE,'
      ' TX.systemcast_id,'
      ' TX.CLASSWAVE,'
      ' TX.ACCOUNTCONDITION_IN,'
      ' TX.ACCOUNTCONDITION_OUT,'
      ' TX.typesystem,'
      ' TX.CHANNEL_ID,'
      ' TX.TIMETRANSMIT,'
      ' TX.VIDEO_CARRIER,'
      ' TX.VIDEO_OFFSET_LINE,'
      ' TX.VIDEO_OFFSET_HERZ,'
      ' TX.FREQSTABILITY,'
      ' TX.TYPEOFFSET,'
      ' TX.TYPEOFFSET typeoffset2,'
      ' TX.SYSTEMCOLOUR,'
      ' TX.VIDEO_EMISSION,'
      ' TX.POWER_VIDEO,'
      ' TX.EPR_VIDEO_MAX,'
      ' TX.EPR_VIDEO_MAX + 30 EPR_VIDEO_MAX_DBW,'
      ' TX.EPR_VIDEO_HOR,'
      ' TX.EPR_VIDEO_HOR + 30 EPR_VIDEO_HOR_DBW,'
      ' TX.EPR_VIDEO_VERT,'
      ' TX.EPR_VIDEO_VERT + 30 EPR_VIDEO_VERT_DBW,'
      ' TX.EFFECTPOWERHOR,'
      ' TX.EFFECTPOWERVER,'
      ' TX.ALLOTMENTBLOCKDAB_ID,'
      ' TX.BLOCKCENTREFREQ,'
      ' TX.GUARDINTERVAL_ID,'
      ' TX.RELATIVETIMINGSFN,'
      ' TX.SOUND_CARRIER_PRIMARY,'
      ' TX.SOUND_OFFSET_PRIMARY,'
      ' TX.SOUND_EMISSION_PRIMARY,'
      ' TX.POWER_SOUND_PRIMARY,'
      ' TX.EPR_SOUND_MAX_PRIMARY,'
      ' TX.EPR_SOUND_MAX_PRIMARY + 30 EPR_SOUND_MAX_PRIMARY_DBW,'
      ' TX.EPR_SOUND_HOR_PRIMARY,'
      ' TX.EPR_SOUND_HOR_PRIMARY+30 EPR_SOUND_HOR_PRIMARY_DBW,'
      ' TX.EPR_SOUND_VERT_PRIMARY,'
      ' TX.EPR_SOUND_VERT_PRIMARY+30 EPR_SOUND_VERT_PRIMARY_DBW,'
      ' TX.V_SOUND_RATIO_PRIMARY,'
      ' TX.monostereo_primary,'
      ' TX.SOUND_CARRIER_SECOND,'
      ' TX.SOUND_OFFSET_SECOND,'
      ' TX.SOUND_EMISSION_SECOND,'
      ' TX.POWER_SOUND_SECOND,'
      ' TX.EPR_SOUND_MAX_SECOND,'
      ' TX.EPR_SOUND_HOR_SECOND,'
      ' TX.EPR_SOUND_VER_SECOND,'
      ' TX.SOUND_SYSTEM_SECOND,'
      ' TX.V_SOUND_RATIO_SECOND,'
      ' TX.HEIGHTANTENNA,'
      ' TX.HEIGHT_EFF_MAX,'
      ' TX.EFFECTHEIGHT,'
      ' TX.polarization,'
      ' TX.POLARIZATION POLARIZATION2,'
      ' TX.direction,'
      ' TX.DIRECTION DIRECTION2,'
      ' TX.FIDERLOSS,'
      ' TX.FIDERLENGTH,'
      ' TX.ANGLEELEVATION_HOR,'
      ' TX.ANGLEELEVATION_VER,'
      ' TX.ANTENNAGAIN,'
      ' TX.ANT_DIAG_H,'
      ' TX.ANT_DIAG_V,'
      ' TX.TESTPOINTSIS,'
      ' TX.NAMEPROGRAMM,'
      ' TX.USERID,'
      ' TX.ORIGINALID,'
      ' TX.NUMREGISTRY,'
      ' TX.TYPEREGISTRY,'
      ' TX.REMARKS,'
      ' TX.RELAYSTATION_ID,'
      ' TX.OPERATOR_ID,'
      ' TX.IDENTIFIERSFN,'
      ' TX.LEVELSIDERADIATION,'
      ' TX.FREQSHIFT,'
      ' TX.summatorpowers,'
      ' TX.AZIMUTHMAXRADIATION,'
      ' TX.SUMMATOFREQFROM,'
      ' TX.SUMMATORFREQTO,'
      ' TX.SUMMATORPOWERFROM,'
      ' TX.SUMMATORPOWERTO,'
      ' TX.SUMMATORMINFREQS,'
      ' TX.SUMMATORATTENUATION,'
      ' TX.TYPERECEIVE_ID,'
      ' TXSTAND.NAMESITE stand_namesite,'
      ' TXSTAND.NAMESITE_ENG STAND_NAMESITE_ENG,'
      ' TXSTAND.HEIGHT_SEA stand_height_sea,'
      ' TXSTAND.LATITUDE,'
      ' TXSTAND.LONGITUDE LONGITUDE2,'
      ' TXSTAND.LATITUDE LATITUDE2,'
      ' TXSTAND.LONGITUDE,'
      ' TXAREA.NAME area_name,'
      ' TXAREA.NUMREGION AREA_NUMREGION,'
      ' TXCOUNTRY.CODE COUNTRY_CODE,'
      ' TXCOUNTRY.CODE COUNTRY_CODE2,'
      ' TXCITY.NAME CITY_NAME,'
      ' TXSTREET.NAME STREET_NAME,'
      ' TXSTAND.ADDRESS STAND_ADDRESS,'
      ' OWNER1.NAMEORGANIZATION OWNER_NAMEORGANIZATION,'
      ' OWNER1.NUMIDENTYCOD OWNER_NUMIDENTYCOD,'
      ' OWNER1.NUMNDS OWNER_NUMNDS,'
      ' OWNER1.TYPEFINANCE OWNER_TYPEFINANCE,'
      ' OWNER1.ADDRESSJURE OWNER_ADDRESSJURE,'
      ' OWNER1.ADDRESSPHYSICAL OWNER_ADDRESSPHYSICAL,'
      ' OWNER1.NUMSETTLEMENTACCOUNT OWNER_NUMSETTLEMENTACCOUNT,'
      ' BANK.MFO BANK_MFO,'
      ' OWNER1.NUMIDENTYCODACCOUNT OWNER_NUMIDENTYCODACCOUNT,'
      ' BANK.NAME BANK_NAME,'
      ' OWNER1.NAMEBOSS,'
      ' OWNER1.PHONE OWNER_PHONE,'
      ' OWNER1.FAX OWNER_FAX,'
      ' OWNER1.MAIL OWNER_MAIL,'
      ' OWNER2.NAMEBOSS NAMEBOSS2,'
      ' OWNER2.PHONE OWNER_PHONE2,'
      ' OWNER2.FAX OWNER_FAX2,'
      ' OWNER2.MAIL OWNER_MAIL2,'
      ' OPERATOR.NAMEORGANIZATION OPERATOR_NAMEORGANIZATION,'
      ' OPERATOR.NUMIDENTYCOD OPERATOR_NUMIDENTYCOD,'
      ' OPERATOR.ADDRESSPHYSICAL OPERATOR_ADDRESSPHYSICAL,'
      ' OPERATOR.NAMEBOSS OPERATOR_NAMEBOSS,'
      ' OPERATOR.PHONE OPERATOR_PHONE,'
      ' OPERATOR.FAX OPERATOR_FAX,'
      ' OPERATOR.MAIL OPERATOR_MAIL,'
      ' OPERATOR2.NAMEBOSS OPERATOR_NAMEBOSS2,'
      ' OPERATOR2.PHONE OPERATOR_PHONE2,'
      ' OPERATOR2.FAX OPERATOR_FAX2,'
      ' OPERATOR2.MAIL OPERATOR_MAIL2,'
      ' LICENSE_CH.NUMLICENSE LICENSE_CH_NUMLICENSE,'
      ' LICENSE_CH.DATEFROM LICENSE_CH_DATEFROM,'
      ' LICENSE_CH.DATETO LICENSE_CH_DATETO,'
      ' LICENSE_RFR.NUMLICENSE LICENSE_RFR_NUMLICENSE,'
      ' LICENSE_RFR.DATEFROM LICENSE_RFR_DATEFROM,'
      ' LICENSE_RFR.DATETO LICENSE_RFR_DATETO,'
      ' LICENSE_SRV.NUMLICENSE LICENSE_SRV_NUMLICENSE,'
      ' LICENSE_SRV.DATEFROM LICENSE_SRV_DATEFROM,'
      ' ACIN.CODE ACIN_CODE,'
      ' ACOUT.CODE ACOUT_CODE,'
      ' ARS.CODSYSTEM ARS_CODSYSTEM,'
      ' ARS.DEVIATION ARS_DEVIATION,'
      ' ATS.NAMESYSTEM ATS_NAMESYSTEM,'
      ' ATS_TYPE.NAMESYSTEM ATS_TYPE_NAMESYSTEM,'
      ' ATS.CHANNELBAND ATS_CHANNELBAND,'
      ' '#39#39' ABD_NAME,'
      ' BD.NAME BD_NAME,'
      ' CGI.FREQINTERVAL CGI_FREQINTERVAL,'
      ' CGI.CODE CGI_CODE,'
      ' SN.SYNHRONETID SN_SYNHRONETID,'
      ' ADMIT.NAME ADMIT_NAME,'
      ' TXRETR.ADMINISTRATIONID TXRETR_ADMINISTRATIONID,'
      ' AREA_RETR.NAME AREA_RETR_NAME,'
      ' CITY_RETR.NAME CITY_RETR_NAME,'
      ' TXRETR.SOUND_CARRIER_PRIMARY TXRETR_SOUND_CARRIER_PRIMARY,'
      ' TXRETR.VIDEO_CARRIER TXRETR_VIDEO_CARRIER,'
      ' OPER.NAME OPER_MAME,'
      ' TREC.NAME TREC_NAME,'
      ' DTS.NAMESYSTEM DTS_NAMESYSTEM,'
      ' (ARS.DEVIATION*2) ARS_BAND_WIDTH,'
      ' CH.namechannel,'
      ' CH.FREQTO CH_FREQTO,'
      ' CH.FREQFROM CH_FREQFROM,'
      ' (CH.FREQTO + CH.FREQFROM)/2 CH_DEVIATION,'
      ' CH.FREQCARRIERNICAM CH_FREQCARRIERNICAM,'
      ' SC.CODE SC_CODE,'
      ' SC.enumval sc_enumval,'
      ' ATS.DESCR ATS_DESCR,'
      ' ARS.DESCR ARS_DESCR,'
      ' DTS.DESCR DTS_DESCR,'
      ''
      '    TX.EMC_CONCL_NUM,'
      '    TX.EMC_CONCL_FROM,'
      '    TX.EMC_CONCL_TO,'
      ''
      '    tx.NR_REQ_NO,'
      '    tx.NR_REQ_DATE,'
      '    tx.NR_CONCL_NO,'
      '    tx.NR_CONCL_DATE,'
      '    tx.NR_APPL_NO,'
      '    tx.NR_APPL_DATE,'
      '    tx.associated_adm_allot_id'
      'from TRANSMITTERS TX'
      ' LEFT OUTER JOIN STAND TXSTAND ON (TX.STAND_ID = TXSTAND.ID)'
      
        ' LEFT OUTER JOIN STREET TXSTREET ON (TXSTAND.STREET_ID = TXSTREE' +
        'T.ID)'
      ' LEFT OUTER JOIN AREA TXAREA ON (TXSTAND.AREA_ID = TXAREA.ID)'
      
        ' LEFT OUTER JOIN COUNTRY TXCOUNTRY ON (TXAREA.COUNTRY_ID = TXCOU' +
        'NTRY.ID)'
      ' LEFT OUTER JOIN CITY TXCITY ON (TXSTAND.CITY_ID = TXCITY.ID)'
      ' LEFT OUTER JOIN OWNER OWNER1 ON (TX.OWNER_ID = OWNER1.ID)'
      ' LEFT OUTER JOIN OWNER OWNER2 ON (TX.OWNER_ID = OWNER2.ID)'
      
        ' LEFT OUTER JOIN OWNER OPERATOR ON (TX.OPERATOR_ID = OPERATOR.ID' +
        ')'
      
        ' LEFT OUTER JOIN OWNER OPERATOR2 ON (TX.OPERATOR_ID = OPERATOR2.' +
        'ID)'
      ' LEFT OUTER JOIN BANK BANK ON (OWNER1.BANK_ID = BANK.ID)'
      
        ' LEFT OUTER JOIN LICENSE LICENSE_CH ON (TX.LICENSE_CHANNEL_ID = ' +
        'LICENSE_CH.ID)'
      
        ' LEFT OUTER JOIN LICENSE LICENSE_RFR ON (TX.LICENSE_RFR_ID = LIC' +
        'ENSE_RFR.ID)'
      
        ' LEFT OUTER JOIN LICENSE LICENSE_SRV ON (TX.LICENSE_SERVICE_ID =' +
        ' LICENSE_SRV.ID)'
      ' LEFT OUTER JOIN SYSTEMCAST SC ON (TX.SYSTEMCAST_ID = SC.ID)'
      
        ' LEFT OUTER JOIN ACCOUNTCONDITION ACIN ON (TX.ACCOUNTCONDITION_I' +
        'N = ACIN.ID)'
      
        ' LEFT OUTER JOIN ACCOUNTCONDITION ACOUT ON (TX.ACCOUNTCONDITION_' +
        'OUT = ACOUT.ID)'
      
        ' LEFT OUTER JOIN ANALOGRADIOSYSTEM ARS ON (TX.TYPESYSTEM = ARS.I' +
        'D)'
      
        ' LEFT OUTER JOIN ANALOGTELESYSTEM ATS ON (TX.TYPESYSTEM = ATS.ID' +
        ')'
      
        ' LEFT OUTER JOIN DIGITALTELESYSTEM DTS ON (TX.TYPESYSTEM = DTS.I' +
        'D)'
      ' LEFT OUTER JOIN CHANNELS CH ON (TX.CHANNEL_ID = CH.ID)'
      
        ' LEFT OUTER JOIN ANALOGTELESYSTEM ATS_TYPE ON (TX.TYPESYSTEM = A' +
        'TS_TYPE.ID)'
      
        ' LEFT OUTER JOIN BLOCKDAB BD ON (TX.ALLOTMENTBLOCKDAB_ID = BD.ID' +
        ')'
      
        ' LEFT OUTER JOIN CARRIERGUARDINTERVAL CGI ON (TX.GUARDINTERVAL_I' +
        'D = CGI.ID)'
      ' LEFT OUTER JOIN SYNHROFREQNET SN ON (TX.IDENTIFIERSFN = SN.ID)'
      
        ' LEFT OUTER JOIN TRANSMITTERS TXRETR ON (TX.RELAYSTATION_ID = TX' +
        'RETR.ID)'
      
        ' LEFT OUTER JOIN STAND STAND_RETR ON (TXRETR.STAND_ID = STAND_RE' +
        'TR.ID)'
      
        ' LEFT OUTER JOIN AREA AREA_RETR ON (STAND_RETR.AREA_ID = AREA_RE' +
        'TR.ID)'
      
        ' LEFT OUTER JOIN CITY CITY_RETR ON (STAND_RETR.CITY_ID = CITY_RE' +
        'TR.ID)'
      
        ' LEFT OUTER JOIN TELECOMORGANIZATION OPER ON (TX.OPERATOR_ID = O' +
        'PER.ID)'
      
        ' LEFT OUTER JOIN TYPERECEIVE TREC ON (TX.TYPERECEIVE_ID = TREC.I' +
        'D)'
      ' LEFT OUTER JOIN ADMIT ON (TX.USERID = ADMIT.ID)'
      'where TX.ID = :ID')
    Left = 288
    Top = 88
    ParamData = <
      item
        DataType = ftUnknown
        Name = 'ID'
        ParamType = ptUnknown
      end>
    object ibqTxOueryEFFECTPOWERHOR: TBlobField
      FieldName = 'EFFECTPOWERHOR'
      Origin = 'TRANSMITTERS.EFFECTPOWERHOR'
      Size = 8
    end
    object ibqTxOueryEFFECTPOWERVER: TBlobField
      FieldName = 'EFFECTPOWERVER'
      Origin = 'TRANSMITTERS.EFFECTPOWERVER'
      Size = 8
    end
    object ibqTxOueryEFFECTHEIGHT: TBlobField
      FieldName = 'EFFECTHEIGHT'
      Origin = 'TRANSMITTERS.EFFECTHEIGHT'
      Size = 8
    end
    object ibqTxOueryANT_DIAG_H: TBlobField
      FieldName = 'ANT_DIAG_H'
      Origin = 'TRANSMITTERS.ANT_DIAG_H'
      Size = 8
    end
    object ibqTxOueryANT_DIAG_V: TBlobField
      FieldName = 'ANT_DIAG_V'
      Origin = 'TRANSMITTERS.ANT_DIAG_V'
      Size = 8
    end
    object ibqTxOueryID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
    object ibqTxOuerySTAND_ID: TIntegerField
      FieldName = 'STAND_ID'
      Origin = 'TRANSMITTERS.STAND_ID'
    end
    object ibqTxOuerySTAND_NAMESITE: TIBStringField
      FieldName = 'STAND_NAMESITE'
      Origin = 'STAND.NAMESITE'
      Size = 32
    end
    object ibqTxOuerySTAND_HEIGHT_SEA: TIntegerField
      FieldName = 'STAND_HEIGHT_SEA'
      Origin = 'STAND.HEIGHT_SEA'
    end
    object ibqTxOueryAREA_NAME: TIBStringField
      FieldName = 'AREA_NAME'
      Origin = 'AREA.NAME'
      Size = 32
    end
    object ibqTxOueryAREA_NUMREGION: TIBStringField
      FieldName = 'AREA_NUMREGION'
      Origin = 'AREA.NUMREGION'
      Size = 4
    end
    object ibqTxOueryCITY_NAME: TIBStringField
      FieldName = 'CITY_NAME'
      Origin = 'CITY.NAME'
      Size = 32
    end
    object ibqTxOuerySTREET_NAME: TIBStringField
      FieldName = 'STREET_NAME'
      Origin = 'STREET.NAME'
      Size = 32
    end
    object ibqTxOuerySTAND_ADDRESS: TIBStringField
      FieldName = 'STAND_ADDRESS'
      Origin = 'STAND.ADDRESS'
      Size = 128
    end
    object ibqTxOueryDATECREATE: TDateField
      FieldName = 'DATECREATE'
      Origin = 'TRANSMITTERS.DATECREATE'
      Required = True
    end
    object ibqTxOueryDATECHANGE: TDateField
      FieldName = 'DATECHANGE'
      Origin = 'TRANSMITTERS.DATECHANGE'
      Required = True
    end
    object ibqTxOueryOWNER_ID: TIntegerField
      FieldName = 'OWNER_ID'
      Origin = 'TRANSMITTERS.OWNER_ID'
    end
    object ibqTxOueryOWNER_NAMEORGANIZATION: TIBStringField
      FieldName = 'OWNER_NAMEORGANIZATION'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
    object ibqTxOueryOWNER_NUMIDENTYCOD: TIBStringField
      FieldName = 'OWNER_NUMIDENTYCOD'
      Origin = 'OWNER.NUMIDENTYCOD'
      Size = 16
    end
    object ibqTxOueryOWNER_NUMNDS: TIBStringField
      FieldName = 'OWNER_NUMNDS'
      Origin = 'OWNER.NUMNDS'
      Size = 16
    end
    object ibqTxOueryOWNER_TYPEFINANCE: TSmallintField
      FieldName = 'OWNER_TYPEFINANCE'
      Origin = 'OWNER.TYPEFINANCE'
    end
    object ibqTxOueryOWNER_ADDRESSJURE: TIBStringField
      FieldName = 'OWNER_ADDRESSJURE'
      Origin = 'OWNER.ADDRESSJURE'
      Size = 128
    end
    object ibqTxOueryOWNER_ADDRESSPHYSICAL: TIBStringField
      FieldName = 'OWNER_ADDRESSPHYSICAL'
      Origin = 'OWNER.ADDRESSPHYSICAL'
      Size = 128
    end
    object ibqTxOueryOWNER_NUMSETTLEMENTACCOUNT: TIBStringField
      FieldName = 'OWNER_NUMSETTLEMENTACCOUNT'
      Origin = 'OWNER.NUMSETTLEMENTACCOUNT'
      Size = 16
    end
    object ibqTxOueryBANK_MFO: TIBStringField
      FieldName = 'BANK_MFO'
      Origin = 'BANK.MFO'
      Size = 8
    end
    object ibqTxOueryOWNER_NUMIDENTYCODACCOUNT: TIBStringField
      FieldName = 'OWNER_NUMIDENTYCODACCOUNT'
      Origin = 'OWNER.NUMIDENTYCODACCOUNT'
      Size = 16
    end
    object ibqTxOueryBANK_NAME: TIBStringField
      FieldName = 'BANK_NAME'
      Origin = 'BANK.NAME'
      Size = 64
    end
    object ibqTxOueryNAMEBOSS: TIBStringField
      FieldName = 'NAMEBOSS'
      Origin = 'OWNER.NAMEBOSS'
      Size = 64
    end
    object ibqTxOueryOWNER_PHONE: TIBStringField
      FieldName = 'OWNER_PHONE'
      Origin = 'OWNER.PHONE'
      Size = 16
    end
    object ibqTxOueryOWNER_FAX: TIBStringField
      FieldName = 'OWNER_FAX'
      Origin = 'OWNER.FAX'
      Size = 16
    end
    object ibqTxOueryOWNER_MAIL: TIBStringField
      FieldName = 'OWNER_MAIL'
      Origin = 'OWNER.MAIL'
      Size = 32
    end
    object ibqTxOueryNAMEBOSS2: TIBStringField
      FieldName = 'NAMEBOSS2'
      Origin = 'OWNER.NAMEBOSS'
      Size = 64
    end
    object ibqTxOueryOWNER_PHONE2: TIBStringField
      FieldName = 'OWNER_PHONE2'
      Origin = 'OWNER.PHONE'
      Size = 16
    end
    object ibqTxOueryOWNER_FAX2: TIBStringField
      FieldName = 'OWNER_FAX2'
      Origin = 'OWNER.FAX'
      Size = 16
    end
    object ibqTxOueryOWNER_MAIL2: TIBStringField
      FieldName = 'OWNER_MAIL2'
      Origin = 'OWNER.MAIL'
      Size = 32
    end
    object ibqTxOueryOPERATOR_NAMEORGANIZATION: TIBStringField
      FieldName = 'OPERATOR_NAMEORGANIZATION'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
    object ibqTxOueryOPERATOR_NUMIDENTYCOD: TIBStringField
      FieldName = 'OPERATOR_NUMIDENTYCOD'
      Origin = 'OWNER.NUMIDENTYCOD'
      Size = 16
    end
    object ibqTxOueryOPERATOR_ADDRESSPHYSICAL: TIBStringField
      FieldName = 'OPERATOR_ADDRESSPHYSICAL'
      Origin = 'OWNER.ADDRESSPHYSICAL'
      Size = 128
    end
    object ibqTxOueryOPERATOR_NAMEBOSS: TIBStringField
      FieldName = 'OPERATOR_NAMEBOSS'
      Origin = 'OWNER.NAMEBOSS'
      Size = 64
    end
    object ibqTxOueryOPERATOR_PHONE: TIBStringField
      FieldName = 'OPERATOR_PHONE'
      Origin = 'OWNER.PHONE'
      Size = 16
    end
    object ibqTxOueryOPERATOR_FAX: TIBStringField
      FieldName = 'OPERATOR_FAX'
      Origin = 'OWNER.FAX'
      Size = 16
    end
    object ibqTxOueryOPERATOR_MAIL: TIBStringField
      FieldName = 'OPERATOR_MAIL'
      Origin = 'OWNER.MAIL'
      Size = 32
    end
    object ibqTxOueryOPERATOR_NAMEBOSS2: TIBStringField
      FieldName = 'OPERATOR_NAMEBOSS2'
      Origin = 'OWNER.NAMEBOSS'
      Size = 64
    end
    object ibqTxOueryOPERATOR_PHONE2: TIBStringField
      FieldName = 'OPERATOR_PHONE2'
      Origin = 'OWNER.PHONE'
      Size = 16
    end
    object ibqTxOueryOPERATOR_FAX2: TIBStringField
      FieldName = 'OPERATOR_FAX2'
      Origin = 'OWNER.FAX'
      Size = 16
    end
    object ibqTxOueryOPERATOR_MAIL2: TIBStringField
      FieldName = 'OPERATOR_MAIL2'
      Origin = 'OWNER.MAIL'
      Size = 32
    end
    object ibqTxOueryLICENSE_CHANNEL_ID: TIntegerField
      FieldName = 'LICENSE_CHANNEL_ID'
      Origin = 'TRANSMITTERS.LICENSE_CHANNEL_ID'
    end
    object ibqTxOueryLICENSE_CH_NUMLICENSE: TIBStringField
      FieldName = 'LICENSE_CH_NUMLICENSE'
      Origin = 'LICENSE.NUMLICENSE'
      Size = 64
    end
    object ibqTxOueryLICENSE_CH_DATEFROM: TDateField
      FieldName = 'LICENSE_CH_DATEFROM'
      Origin = 'LICENSE.DATEFROM'
    end
    object ibqTxOueryLICENSE_CH_DATETO: TDateField
      FieldName = 'LICENSE_CH_DATETO'
      Origin = 'LICENSE.DATETO'
    end
    object ibqTxOueryLICENSE_RFR_ID: TIntegerField
      FieldName = 'LICENSE_RFR_ID'
      Origin = 'TRANSMITTERS.LICENSE_RFR_ID'
    end
    object ibqTxOueryLICENSE_RFR_NUMLICENSE: TIBStringField
      FieldName = 'LICENSE_RFR_NUMLICENSE'
      Origin = 'LICENSE.NUMLICENSE'
      Size = 64
    end
    object ibqTxOueryLICENSE_RFR_DATEFROM: TDateField
      FieldName = 'LICENSE_RFR_DATEFROM'
      Origin = 'LICENSE.DATEFROM'
    end
    object ibqTxOueryLICENSE_SERVICE_ID: TIntegerField
      FieldName = 'LICENSE_SERVICE_ID'
      Origin = 'TRANSMITTERS.LICENSE_SERVICE_ID'
    end
    object ibqTxOueryLICENSE_SRV_NUMLICENSE: TIBStringField
      FieldName = 'LICENSE_SRV_NUMLICENSE'
      Origin = 'LICENSE.NUMLICENSE'
      Size = 64
    end
    object ibqTxOueryLICENSE_SRV_DATEFROM: TDateField
      FieldName = 'LICENSE_SRV_DATEFROM'
      Origin = 'LICENSE.DATEFROM'
    end
    object ibqTxOueryNUMPERMBUILD: TIBStringField
      FieldName = 'NUMPERMBUILD'
      Origin = 'TRANSMITTERS.NUMPERMBUILD'
      Size = 64
    end
    object ibqTxOueryDATEPERMBUILDFROM: TDateField
      FieldName = 'DATEPERMBUILDFROM'
      Origin = 'TRANSMITTERS.DATEPERMBUILDFROM'
    end
    object ibqTxOueryDATEPERMBUILDTO: TDateField
      FieldName = 'DATEPERMBUILDTO'
      Origin = 'TRANSMITTERS.DATEPERMBUILDTO'
    end
    object ibqTxOueryNUMPERMUSE: TIBStringField
      FieldName = 'NUMPERMUSE'
      Origin = 'TRANSMITTERS.NUMPERMUSE'
      Size = 64
    end
    object ibqTxOueryDATEPERMUSEFROM: TDateField
      FieldName = 'DATEPERMUSEFROM'
      Origin = 'TRANSMITTERS.DATEPERMUSEFROM'
    end
    object ibqTxOueryDATEPERMUSETO: TDateField
      FieldName = 'DATEPERMUSETO'
      Origin = 'TRANSMITTERS.DATEPERMUSETO'
    end
    object ibqTxOueryREGIONALCOUNCIL: TIBStringField
      FieldName = 'REGIONALCOUNCIL'
      Origin = 'TRANSMITTERS.REGIONALCOUNCIL'
      Size = 64
    end
    object ibqTxOueryNUMPERMREGCOUNCIL: TIBStringField
      FieldName = 'NUMPERMREGCOUNCIL'
      Origin = 'TRANSMITTERS.NUMPERMREGCOUNCIL'
      Size = 64
    end
    object ibqTxOueryDATEPERMREGCOUNCIL: TDateField
      FieldName = 'DATEPERMREGCOUNCIL'
      Origin = 'TRANSMITTERS.DATEPERMREGCOUNCIL'
    end
    object ibqTxOueryNOTICECOUNT: TBlobField
      FieldName = 'NOTICECOUNT'
      Origin = 'TRANSMITTERS.NOTICECOUNT'
      Size = 8
    end
    object ibqTxOueryNUMSTANDCERTIFICATE: TIBStringField
      FieldName = 'NUMSTANDCERTIFICATE'
      Origin = 'TRANSMITTERS.NUMSTANDCERTIFICATE'
      Size = 16
    end
    object ibqTxOueryDATESTANDCERTIFICATE: TDateField
      FieldName = 'DATESTANDCERTIFICATE'
      Origin = 'TRANSMITTERS.DATESTANDCERTIFICATE'
    end
    object ibqTxOueryNUMFACTORY: TIBStringField
      FieldName = 'NUMFACTORY'
      Origin = 'TRANSMITTERS.NUMFACTORY'
      Size = 16
    end
    object ibqTxOueryRESPONSIBLEADMIN: TIBStringField
      FieldName = 'RESPONSIBLEADMIN'
      Origin = 'TRANSMITTERS.RESPONSIBLEADMIN'
      Size = 4
    end
    object ibqTxOueryADMINISTRATIONID: TIBStringField
      FieldName = 'ADMINISTRATIONID'
      Origin = 'TRANSMITTERS.ADMINISTRATIONID'
      Size = 4
    end
    object ibqTxOueryREGIONALAGREEMENT: TSmallintField
      FieldName = 'REGIONALAGREEMENT'
      Origin = 'TRANSMITTERS.REGIONALAGREEMENT'
    end
    object ibqTxOueryDATEINTENDUSE: TDateField
      FieldName = 'DATEINTENDUSE'
      Origin = 'TRANSMITTERS.DATEINTENDUSE'
    end
    object ibqTxOueryAREACOVERAGE: TBlobField
      FieldName = 'AREACOVERAGE'
      Origin = 'TRANSMITTERS.AREACOVERAGE'
      Size = 8
    end
    object ibqTxOuerySYSTEMCAST_ID: TIntegerField
      FieldName = 'SYSTEMCAST_ID'
      Origin = 'TRANSMITTERS.SYSTEMCAST_ID'
      Required = True
    end
    object ibqTxOuerySC_CODE: TIBStringField
      FieldName = 'SC_CODE'
      Origin = 'SYSTEMCAST.CODE'
      Size = 4
    end
    object ibqTxOueryCLASSWAVE: TIBStringField
      FieldName = 'CLASSWAVE'
      Origin = 'TRANSMITTERS.CLASSWAVE'
      Size = 4
    end
    object ibqTxOueryACCOUNTCONDITION_IN: TSmallintField
      FieldName = 'ACCOUNTCONDITION_IN'
      Origin = 'TRANSMITTERS.ACCOUNTCONDITION_IN'
    end
    object ibqTxOueryACIN_CODE: TIBStringField
      FieldName = 'ACIN_CODE'
      Origin = 'ACCOUNTCONDITION.CODE'
      Size = 4
    end
    object ibqTxOueryACCOUNTCONDITION_OUT: TSmallintField
      FieldName = 'ACCOUNTCONDITION_OUT'
      Origin = 'TRANSMITTERS.ACCOUNTCONDITION_OUT'
    end
    object ibqTxOueryACOUT_CODE: TIBStringField
      FieldName = 'ACOUT_CODE'
      Origin = 'ACCOUNTCONDITION.CODE'
      Size = 4
    end
    object ibqTxOueryTIMETRANSMIT: TIBStringField
      FieldName = 'TIMETRANSMIT'
      Origin = 'TRANSMITTERS.TIMETRANSMIT'
      Size = 256
    end
    object ibqTxOueryTYPESYSTEM: TSmallintField
      FieldName = 'TYPESYSTEM'
      Origin = 'TRANSMITTERS.TYPESYSTEM'
    end
    object ibqTxOueryARS_CODSYSTEM: TIBStringField
      FieldName = 'ARS_CODSYSTEM'
      Origin = 'ANALOGRADIOSYSTEM.CODSYSTEM'
      Size = 8
    end
    object ibqTxOueryARS_DEVIATION: TFloatField
      FieldName = 'ARS_DEVIATION'
      Origin = 'ANALOGRADIOSYSTEM.DEVIATION'
    end
    object ibqTxOueryATS_NAMESYSTEM: TIBStringField
      FieldName = 'ATS_NAMESYSTEM'
      Origin = 'ANALOGTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object ibqTxOueryATS_TYPE_NAMESYSTEM: TIBStringField
      FieldName = 'ATS_TYPE_NAMESYSTEM'
      Origin = 'ANALOGTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object ibqTxOueryATS_CHANNELBAND: TFloatField
      FieldName = 'ATS_CHANNELBAND'
      Origin = 'ANALOGTELESYSTEM.CHANNELBAND'
    end
    object ibqTxOueryCHANNEL_ID: TIntegerField
      FieldName = 'CHANNEL_ID'
      Origin = 'TRANSMITTERS.CHANNEL_ID'
    end
    object ibqTxOueryNAMECHANNEL: TIBStringField
      FieldName = 'NAMECHANNEL'
      Origin = 'CHANNELS.NAMECHANNEL'
      Size = 4
    end
    object ibqTxOueryCH_FREQTO: TFloatField
      FieldName = 'CH_FREQTO'
      Origin = 'CHANNELS.FREQTO'
    end
    object ibqTxOueryCH_FREQFROM: TFloatField
      FieldName = 'CH_FREQFROM'
      Origin = 'CHANNELS.FREQFROM'
    end
    object ibqTxOueryCH_DEVIATION: TFloatField
      FieldName = 'CH_DEVIATION'
      Origin = '(CH.FREQTO + CH.FREQFROM)/2'
    end
    object ibqTxOueryVIDEO_CARRIER: TFloatField
      FieldName = 'VIDEO_CARRIER'
      Origin = 'TRANSMITTERS.VIDEO_CARRIER'
    end
    object ibqTxOueryVIDEO_OFFSET_LINE: TSmallintField
      FieldName = 'VIDEO_OFFSET_LINE'
      Origin = 'TRANSMITTERS.VIDEO_OFFSET_LINE'
    end
    object ibqTxOueryVIDEO_OFFSET_HERZ: TIntegerField
      FieldName = 'VIDEO_OFFSET_HERZ'
      Origin = 'TRANSMITTERS.VIDEO_OFFSET_HERZ'
    end
    object ibqTxOueryFREQSTABILITY: TIBStringField
      FieldName = 'FREQSTABILITY'
      Origin = 'TRANSMITTERS.FREQSTABILITY'
      Size = 16
    end
    object ibqTxOueryTYPEOFFSET: TIBStringField
      FieldName = 'TYPEOFFSET'
      Origin = 'TRANSMITTERS.TYPEOFFSET'
      Size = 16
    end
    object ibqTxOuerySYSTEMCOLOUR: TIBStringField
      FieldName = 'SYSTEMCOLOUR'
      Origin = 'TRANSMITTERS.SYSTEMCOLOUR'
      Size = 8
    end
    object ibqTxOueryVIDEO_EMISSION: TIBStringField
      FieldName = 'VIDEO_EMISSION'
      Origin = 'TRANSMITTERS.VIDEO_EMISSION'
      Size = 16
    end
    object ibqTxOueryPOWER_VIDEO: TFloatField
      FieldName = 'POWER_VIDEO'
      Origin = 'TRANSMITTERS.POWER_VIDEO'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_VIDEO_MAX: TFloatField
      FieldName = 'EPR_VIDEO_MAX'
      Origin = 'TRANSMITTERS.EPR_VIDEO_MAX'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_VIDEO_HOR: TFloatField
      FieldName = 'EPR_VIDEO_HOR'
      Origin = 'TRANSMITTERS.EPR_VIDEO_HOR'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_VIDEO_VERT: TFloatField
      FieldName = 'EPR_VIDEO_VERT'
      Origin = 'TRANSMITTERS.EPR_VIDEO_VERT'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryALLOTMENTBLOCKDAB_ID: TSmallintField
      FieldName = 'ALLOTMENTBLOCKDAB_ID'
      Origin = 'TRANSMITTERS.ALLOTMENTBLOCKDAB_ID'
    end
    object ibqTxOueryABD_NAME: TIBStringField
      FieldName = 'ABD_NAME'
      Origin = 'ALLOTMENTBLOCKDAB.NAME'
      Size = 8
    end
    object ibqTxOueryBD_NAME: TIBStringField
      FieldName = 'BD_NAME'
      Origin = 'BLOCKDAB.NAME'
      Size = 4
    end
    object ibqTxOueryBLOCKCENTREFREQ: TFloatField
      FieldName = 'BLOCKCENTREFREQ'
      Origin = 'TRANSMITTERS.BLOCKCENTREFREQ'
    end
    object ibqTxOueryGUARDINTERVAL_ID: TSmallintField
      FieldName = 'GUARDINTERVAL_ID'
      Origin = 'TRANSMITTERS.GUARDINTERVAL_ID'
    end
    object ibqTxOueryCGI_FREQINTERVAL: TIntegerField
      FieldName = 'CGI_FREQINTERVAL'
      Origin = 'CARRIERGUARDINTERVAL.FREQINTERVAL'
    end
    object ibqTxOueryIDENTIFIERSFN: TIntegerField
      FieldName = 'IDENTIFIERSFN'
      Origin = 'TRANSMITTERS.IDENTIFIERSFN'
    end
    object ibqTxOuerySN_SYNHRONETID: TIBStringField
      FieldName = 'SN_SYNHRONETID'
      Origin = 'SYNHROFREQNET.SYNHRONETID'
      Size = 30
    end
    object ibqTxOueryRELATIVETIMINGSFN: TIntegerField
      FieldName = 'RELATIVETIMINGSFN'
      Origin = 'TRANSMITTERS.RELATIVETIMINGSFN'
    end
    object ibqTxOuerySOUND_CARRIER_PRIMARY: TFloatField
      FieldName = 'SOUND_CARRIER_PRIMARY'
      Origin = 'TRANSMITTERS.SOUND_CARRIER_PRIMARY'
    end
    object ibqTxOuerySOUND_OFFSET_PRIMARY: TIntegerField
      FieldName = 'SOUND_OFFSET_PRIMARY'
      Origin = 'TRANSMITTERS.SOUND_OFFSET_PRIMARY'
    end
    object ibqTxOuerySOUND_EMISSION_PRIMARY: TIBStringField
      FieldName = 'SOUND_EMISSION_PRIMARY'
      Origin = 'TRANSMITTERS.SOUND_EMISSION_PRIMARY'
      Size = 16
    end
    object ibqTxOueryPOWER_SOUND_PRIMARY: TFloatField
      FieldName = 'POWER_SOUND_PRIMARY'
      Origin = 'TRANSMITTERS.POWER_SOUND_PRIMARY'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_SOUND_MAX_PRIMARY: TFloatField
      FieldName = 'EPR_SOUND_MAX_PRIMARY'
      Origin = 'TRANSMITTERS.EPR_SOUND_MAX_PRIMARY'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_SOUND_HOR_PRIMARY: TFloatField
      FieldName = 'EPR_SOUND_HOR_PRIMARY'
      Origin = 'TRANSMITTERS.EPR_SOUND_HOR_PRIMARY'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_SOUND_VERT_PRIMARY: TFloatField
      FieldName = 'EPR_SOUND_VERT_PRIMARY'
      Origin = 'TRANSMITTERS.EPR_SOUND_VERT_PRIMARY'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryV_SOUND_RATIO_PRIMARY: TFloatField
      FieldName = 'V_SOUND_RATIO_PRIMARY'
      Origin = 'TRANSMITTERS.V_SOUND_RATIO_PRIMARY'
    end
    object ibqTxOueryMONOSTEREO_PRIMARY: TSmallintField
      FieldName = 'MONOSTEREO_PRIMARY'
      Origin = 'TRANSMITTERS.MONOSTEREO_PRIMARY'
    end
    object ibqTxOuerySOUND_CARRIER_SECOND: TFloatField
      FieldName = 'SOUND_CARRIER_SECOND'
      Origin = 'TRANSMITTERS.SOUND_CARRIER_SECOND'
    end
    object ibqTxOuerySOUND_OFFSET_SECOND: TIntegerField
      FieldName = 'SOUND_OFFSET_SECOND'
      Origin = 'TRANSMITTERS.SOUND_OFFSET_SECOND'
    end
    object ibqTxOuerySOUND_EMISSION_SECOND: TIBStringField
      FieldName = 'SOUND_EMISSION_SECOND'
      Origin = 'TRANSMITTERS.SOUND_EMISSION_SECOND'
      Size = 16
    end
    object ibqTxOueryPOWER_SOUND_SECOND: TFloatField
      FieldName = 'POWER_SOUND_SECOND'
      Origin = 'TRANSMITTERS.POWER_SOUND_SECOND'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_SOUND_MAX_SECOND: TFloatField
      FieldName = 'EPR_SOUND_MAX_SECOND'
      Origin = 'TRANSMITTERS.EPR_SOUND_MAX_SECOND'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_SOUND_HOR_SECOND: TFloatField
      FieldName = 'EPR_SOUND_HOR_SECOND'
      Origin = 'TRANSMITTERS.EPR_SOUND_HOR_SECOND'
      DisplayFormat = '0.#'
    end
    object ibqTxOueryEPR_SOUND_VER_SECOND: TFloatField
      FieldName = 'EPR_SOUND_VER_SECOND'
      Origin = 'TRANSMITTERS.EPR_SOUND_VER_SECOND'
      DisplayFormat = '0.#'
    end
    object ibqTxOuerySOUND_SYSTEM_SECOND: TIBStringField
      FieldName = 'SOUND_SYSTEM_SECOND'
      Origin = 'TRANSMITTERS.SOUND_SYSTEM_SECOND'
      Size = 8
    end
    object ibqTxOueryV_SOUND_RATIO_SECOND: TFloatField
      FieldName = 'V_SOUND_RATIO_SECOND'
      Origin = 'TRANSMITTERS.V_SOUND_RATIO_SECOND'
    end
    object ibqTxOueryHEIGHTANTENNA: TIntegerField
      FieldName = 'HEIGHTANTENNA'
      Origin = 'TRANSMITTERS.HEIGHTANTENNA'
    end
    object ibqTxOueryHEIGHT_EFF_MAX: TIntegerField
      FieldName = 'HEIGHT_EFF_MAX'
      Origin = 'TRANSMITTERS.HEIGHT_EFF_MAX'
    end
    object ibqTxOueryPOLARIZATION: TIBStringField
      FieldName = 'POLARIZATION'
      Origin = 'TRANSMITTERS.POLARIZATION'
      FixedChar = True
      Size = 1
    end
    object ibqTxOueryDIRECTION: TIBStringField
      FieldName = 'DIRECTION'
      Origin = 'TRANSMITTERS.DIRECTION'
      FixedChar = True
      Size = 2
    end
    object ibqTxOueryFIDERLOSS: TFloatField
      FieldName = 'FIDERLOSS'
      Origin = 'TRANSMITTERS.FIDERLOSS'
    end
    object ibqTxOueryFIDERLENGTH: TIntegerField
      FieldName = 'FIDERLENGTH'
      Origin = 'TRANSMITTERS.FIDERLENGTH'
    end
    object ibqTxOueryANGLEELEVATION_HOR: TSmallintField
      FieldName = 'ANGLEELEVATION_HOR'
      Origin = 'TRANSMITTERS.ANGLEELEVATION_HOR'
    end
    object ibqTxOueryANGLEELEVATION_VER: TSmallintField
      FieldName = 'ANGLEELEVATION_VER'
      Origin = 'TRANSMITTERS.ANGLEELEVATION_VER'
    end
    object ibqTxOueryANTENNAGAIN: TFloatField
      FieldName = 'ANTENNAGAIN'
      Origin = 'TRANSMITTERS.ANTENNAGAIN'
    end
    object ibqTxOueryTESTPOINTSIS: TSmallintField
      FieldName = 'TESTPOINTSIS'
      Origin = 'TRANSMITTERS.TESTPOINTSIS'
    end
    object ibqTxOueryNAMEPROGRAMM: TIBStringField
      FieldName = 'NAMEPROGRAMM'
      Origin = 'TRANSMITTERS.NAMEPROGRAMM'
      Size = 18
    end
    object ibqTxOueryUSERID: TIntegerField
      FieldName = 'USERID'
      Origin = 'TRANSMITTERS.USERID'
      Required = True
    end
    object ibqTxOueryADMIT_NAME: TIBStringField
      FieldName = 'ADMIT_NAME'
      Origin = 'ADMIT.NAME'
      Size = 64
    end
    object ibqTxOueryORIGINALID: TIntegerField
      FieldName = 'ORIGINALID'
      Origin = 'TRANSMITTERS.ORIGINALID'
    end
    object ibqTxOueryNUMREGISTRY: TIBStringField
      FieldName = 'NUMREGISTRY'
      Origin = 'TRANSMITTERS.NUMREGISTRY'
      Size = 16
    end
    object ibqTxOueryTYPEREGISTRY: TIBStringField
      FieldName = 'TYPEREGISTRY'
      Origin = 'TRANSMITTERS.TYPEREGISTRY'
      Size = 16
    end
    object ibqTxOueryREMARKS: TIBStringField
      FieldName = 'REMARKS'
      Origin = 'TRANSMITTERS.REMARKS'
      Size = 512
    end
    object ibqTxOueryRELAYSTATION_ID: TIntegerField
      FieldName = 'RELAYSTATION_ID'
      Origin = 'TRANSMITTERS.RELAYSTATION_ID'
    end
    object ibqTxOueryTXRETR_ADMINISTRATIONID: TIBStringField
      FieldName = 'TXRETR_ADMINISTRATIONID'
      Origin = 'TRANSMITTERS.ADMINISTRATIONID'
      Size = 4
    end
    object ibqTxOueryAREA_RETR_NAME: TIBStringField
      FieldName = 'AREA_RETR_NAME'
      Origin = 'AREA.NAME'
      Size = 32
    end
    object ibqTxOueryCITY_RETR_NAME: TIBStringField
      FieldName = 'CITY_RETR_NAME'
      Origin = 'CITY.NAME'
      Size = 32
    end
    object ibqTxOueryTXRETR_SOUND_CARRIER_PRIMARY: TFloatField
      FieldName = 'TXRETR_SOUND_CARRIER_PRIMARY'
      Origin = 'TRANSMITTERS.SOUND_CARRIER_PRIMARY'
    end
    object ibqTxOueryTXRETR_VIDEO_CARRIER: TFloatField
      FieldName = 'TXRETR_VIDEO_CARRIER'
      Origin = 'TRANSMITTERS.VIDEO_CARRIER'
    end
    object ibqTxOueryOPERATOR_ID: TIntegerField
      FieldName = 'OPERATOR_ID'
      Origin = 'TRANSMITTERS.OPERATOR_ID'
    end
    object ibqTxOueryOPER_MAME: TIBStringField
      FieldName = 'OPER_MAME'
      Origin = 'TELECOMORGANIZATION.NAME'
      Size = 32
    end
    object ibqTxOueryTYPERECEIVE_ID: TIntegerField
      FieldName = 'TYPERECEIVE_ID'
      Origin = 'TRANSMITTERS.TYPERECEIVE_ID'
    end
    object ibqTxOueryTREC_NAME: TIBStringField
      FieldName = 'TREC_NAME'
      Origin = 'TYPERECEIVE.NAME'
      Size = 16
    end
    object ibqTxOueryLEVELSIDERADIATION: TIntegerField
      FieldName = 'LEVELSIDERADIATION'
      Origin = 'TRANSMITTERS.LEVELSIDERADIATION'
    end
    object ibqTxOueryFREQSHIFT: TIntegerField
      FieldName = 'FREQSHIFT'
      Origin = 'TRANSMITTERS.FREQSHIFT'
    end
    object ibqTxOuerySUMMATORPOWERS: TSmallintField
      FieldName = 'SUMMATORPOWERS'
      Origin = 'TRANSMITTERS.SUMMATORPOWERS'
    end
    object ibqTxOueryAZIMUTHMAXRADIATION: TFloatField
      FieldName = 'AZIMUTHMAXRADIATION'
      Origin = 'TRANSMITTERS.AZIMUTHMAXRADIATION'
    end
    object ibqTxOuerySUMMATOFREQFROM: TFloatField
      FieldName = 'SUMMATOFREQFROM'
      Origin = 'TRANSMITTERS.SUMMATOFREQFROM'
    end
    object ibqTxOuerySUMMATORFREQTO: TFloatField
      FieldName = 'SUMMATORFREQTO'
      Origin = 'TRANSMITTERS.SUMMATORFREQTO'
    end
    object ibqTxOuerySUMMATORPOWERFROM: TFloatField
      FieldName = 'SUMMATORPOWERFROM'
      Origin = 'TRANSMITTERS.SUMMATORPOWERFROM'
    end
    object ibqTxOuerySUMMATORPOWERTO: TFloatField
      FieldName = 'SUMMATORPOWERTO'
      Origin = 'TRANSMITTERS.SUMMATORPOWERTO'
    end
    object ibqTxOuerySUMMATORMINFREQS: TFloatField
      FieldName = 'SUMMATORMINFREQS'
      Origin = 'TRANSMITTERS.SUMMATORMINFREQS'
    end
    object ibqTxOuerySUMMATORATTENUATION: TFloatField
      FieldName = 'SUMMATORATTENUATION'
      Origin = 'TRANSMITTERS.SUMMATORATTENUATION'
    end
    object ibqTxOueryARS_BAND_WIDTH: TFloatField
      FieldName = 'ARS_BAND_WIDTH'
      Origin = '(ARS.DEVIATION*2)'
    end
    object ibqTxOueryLATITUDE_SYMB: TStringField
      FieldKind = fkCalculated
      FieldName = 'LATITUDE_SYMB'
      Origin = #1057#1080#1084#1074#1086#1083#1100#1085#1086#1077' LATITUDE'
      Size = 13
      Calculated = True
    end
    object ibqTxOueryLONGITUTE_SYMB: TStringField
      FieldKind = fkCalculated
      FieldName = 'LONGITUDE_SYMB'
      Origin = #1057#1080#1084#1074#1086#1083#1100#1085#1086#1077' LONGITUDE'
      Size = 13
      Calculated = True
    end
    object ibqTxOuerySTAND_NAMESITE_ENG: TIBStringField
      FieldName = 'STAND_NAMESITE_ENG'
      Origin = 'STAND.NAMESITE_ENG'
      Size = 32
    end
    object ibqTxOueryCOUNTRY_CODE: TIBStringField
      FieldName = 'COUNTRY_CODE'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
    object ibqTxOueryLATITUDE_SYMB2: TStringField
      FieldKind = fkCalculated
      FieldName = 'LATITUDE_SYMB2'
      Origin = #1057#1080#1084#1074#1086#1083#1100#1085#1086#1077' LATITUDE, '#1086#1076#1085#1080' '#1094#1080#1092#1088#1099
      Size = 16
      Calculated = True
    end
    object ibqTxOueryLONGITUDE_SYMB2: TStringField
      FieldKind = fkCalculated
      FieldName = 'LONGITUDE_SYMB2'
      Origin = #1057#1080#1084#1074#1086#1083#1100#1085#1086#1077' LONGITUDE, '#1086#1076#1085#1080' '#1094#1080#1092#1088#1099
      Size = 16
      Calculated = True
    end
    object d: TIBStringField
      FieldName = 'TYPEOFFSET2'
      Origin = 'TRANSMITTERS.TYPEOFFSET'
      Size = 16
    end
    object ibqTxOueryCGI_CODE: TIBStringField
      FieldName = 'CGI_CODE'
      Origin = 'CARRIERGUARDINTERVAL.CODE'
      Size = 4
    end
    object ibqTxOueryPOLARIZATION2: TIBStringField
      FieldName = 'POLARIZATION2'
      Origin = 'TRANSMITTERS.POLARIZATION'
      FixedChar = True
      Size = 1
    end
    object ibqTxOueryDIRECTION2: TIBStringField
      FieldName = 'DIRECTION2'
      Origin = 'TRANSMITTERS.DIRECTION'
      FixedChar = True
      Size = 2
    end
    object ibqTxOueryDTS_NAMESYSTEM: TIBStringField
      FieldName = 'DTS_NAMESYSTEM'
      Origin = 'DIGITALTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object ibqTxOueryCOUNTRY_CODE2: TIBStringField
      FieldName = 'COUNTRY_CODE2'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
    object ibqTxOueryEPR_SOUND_HOR_PRIMARY_DBW: TFloatField
      FieldName = 'EPR_SOUND_HOR_PRIMARY_DBW'
      Origin = 'TX.EPR_SOUND_HOR_PRIMARY + 30'
    end
    object ibqTxOueryEPR_SOUND_VERT_PRIMARY_DBW: TFloatField
      FieldName = 'EPR_SOUND_VERT_PRIMARY_DBW'
      Origin = 'TX.EPR_SOUND_VERT_PRIMARY + 30'
    end
    object ibqTxOueryEPR_VIDEO_MAX_DBW: TFloatField
      FieldName = 'EPR_VIDEO_MAX_DBW'
      Origin = 'TX.EPR_VIDEO_MAX + 30'
    end
    object ibqTxOueryEPR_VIDEO_HOR_DBW: TFloatField
      FieldName = 'EPR_VIDEO_HOR_DBW'
      Origin = 'TX.EPR_VIDEO_HOR + 30'
    end
    object ibqTxOueryEPR_VIDEO_VERT_DBW: TFloatField
      FieldName = 'EPR_VIDEO_VERT_DBW'
      Origin = 'TX.EPR_VIDEO_VERT + 30'
    end
    object ibqTxOueryEPR_SOUND_MAX_PRIMARY_DBW: TFloatField
      FieldName = 'EPR_SOUND_MAX_PRIMARY_DBW'
      Origin = 'TX.EPR_SOUND_MAX_PRIMARY + 30'
    end
    object ibqTxOueryCH_FREQCARRIERNICAM: TFloatField
      FieldName = 'CH_FREQCARRIERNICAM'
      Origin = 'CHANNELS.FREQCARRIERNICAM'
    end
    object ibqTxOuerySC_ENUMVAL: TSmallintField
      FieldName = 'SC_ENUMVAL'
      Origin = 'SYSTEMCAST.ENUMVAL'
    end
    object ibqTxOuerySTATUS: TSmallintField
      FieldName = 'STATUS'
      Origin = 'TRANSMITTERS.STATUS'
    end
    object ibqTxOueryVIDEO_BAND: TFloatField
      FieldKind = fkCalculated
      FieldName = 'VIDEO_BAND'
      Origin = #1055#1086#1083#1086#1089#1072' '#1080#1079' VIDEO_EMISSION'
      Calculated = True
    end
    object ibqTxOuerySOUND_BAND: TFloatField
      FieldKind = fkCalculated
      FieldName = 'SOUND_BAND'
      Origin = #1055#1086#1083#1086#1089#1072' '#1080#1079' SOUND_EMISSION'
      Calculated = True
    end
    object ibqTxOueryATS_DESCR: TIBStringField
      FieldName = 'ATS_DESCR'
      Origin = 'ANALOGTELESYSTEM.DESCR'
      Size = 128
    end
    object ibqTxOueryARS_DESCR: TIBStringField
      FieldName = 'ARS_DESCR'
      Origin = 'ANALOGRADIOSYSTEM.DESCR'
      Size = 128
    end
    object ibqTxOueryDTS_DESCR: TIBStringField
      FieldName = 'DTS_DESCR'
      Origin = 'DIGITALTELESYSTEM.DESCR'
      Size = 128
    end
    object ibqTxOueryLICENSE_RFR_DATETO: TDateField
      FieldName = 'LICENSE_RFR_DATETO'
      Origin = 'LICENSE.DATETO'
    end
    object ibqTxOueryEMC_CONCL_NUM: TIBStringField
      FieldName = 'EMC_CONCL_NUM'
      Origin = 'TRANSMITTERS.EMC_CONCL_NUM'
      Size = 16
    end
    object ibqTxOueryEMC_CONCL_FROM: TDateField
      FieldName = 'EMC_CONCL_FROM'
      Origin = 'TRANSMITTERS.EMC_CONCL_FROM'
    end
    object ibqTxOueryEMC_CONCL_TO: TDateField
      FieldName = 'EMC_CONCL_TO'
      Origin = 'TRANSMITTERS.EMC_CONCL_TO'
    end
    object ibqTxOueryNR_REQ_NO: TIBStringField
      FieldName = 'NR_REQ_NO'
      Origin = 'TRANSMITTERS.NR_REQ_NO'
      Size = 16
    end
    object ibqTxOueryNR_REQ_DATE: TDateField
      FieldName = 'NR_REQ_DATE'
      Origin = 'TRANSMITTERS.NR_REQ_DATE'
    end
    object ibqTxOueryNR_CONCL_NO: TIBStringField
      FieldName = 'NR_CONCL_NO'
      Origin = 'TRANSMITTERS.NR_CONCL_NO'
      Size = 16
    end
    object ibqTxOueryNR_CONCL_DATE: TDateField
      FieldName = 'NR_CONCL_DATE'
      Origin = 'TRANSMITTERS.NR_CONCL_DATE'
    end
    object ibqTxOueryNR_APPL_NO: TIBStringField
      FieldName = 'NR_APPL_NO'
      Origin = 'TRANSMITTERS.NR_APPL_NO'
      Size = 16
    end
    object ibqTxOueryNR_APPL_DATE: TDateField
      FieldName = 'NR_APPL_DATE'
      Origin = 'TRANSMITTERS.NR_APPL_DATE'
    end
    object ibqTxOueryOP_AGCY: TIBStringField
      FieldName = 'OP_AGCY'
      Origin = 'TRANSMITTERS.OP_AGCY'
      Size = 64
    end
    object ibqTxOueryADDR_CODE: TIBStringField
      FieldName = 'ADDR_CODE'
      Origin = 'TRANSMITTERS.ADDR_CODE'
      Size = 64
    end
    object ibqTxOueryGND_COND: TFloatField
      FieldName = 'GND_COND'
      Origin = 'TRANSMITTERS.GND_COND'
    end
    object ibqTxOueryMAX_COORD_DIST: TFloatField
      FieldName = 'MAX_COORD_DIST'
      Origin = 'TRANSMITTERS.MAX_COORD_DIST'
    end
    object ibqTxOueryDOC_CURRENT_TIME: TDateTimeField
      FieldName = 'DOC_CURRENT_TIME'
      Required = True
    end
    object ibqTxOueryASSOCIATED_ADM_ALLOT_ID: TIBStringField
      FieldName = 'ASSOCIATED_ADM_ALLOT_ID'
      Origin = 'TRANSMITTERS.ASSOCIATED_ADM_ALLOT_ID'
    end
    object ibqTxOueryDOC_CURRENT_DATE: TDateTimeField
      FieldName = 'DOC_CURRENT_DATE'
      Required = True
    end
    object ibqTxOueryLATITUDE: TFloatField
      FieldName = 'LATITUDE'
      Origin = 'STAND.LATITUDE'
    end
    object ibqTxOueryLONGITUDE2: TFloatField
      FieldName = 'LONGITUDE2'
      Origin = 'STAND.LONGITUDE'
    end
    object ibqTxOueryLATITUDE2: TFloatField
      FieldName = 'LATITUDE2'
      Origin = 'STAND.LATITUDE'
    end
    object ibqTxOueryLONGITUDE: TFloatField
      FieldName = 'LONGITUDE'
      Origin = 'STAND.LONGITUDE'
    end
    object ibqTxOueryNAME: TStringField
      FieldKind = fkCalculated
      FieldName = 'NAME'
      Size = 64
      Calculated = True
    end
    object ibqTxOueryLASTNAME: TStringField
      FieldKind = fkCalculated
      FieldName = 'LASTNAME'
      Size = 128
      Calculated = True
    end
    object ibqTxOueryTEL: TStringField
      FieldKind = fkCalculated
      FieldName = 'TEL'
      Size = 30
      Calculated = True
    end
    object ibqTxOueryIDENT: TStringField
      FieldKind = fkCalculated
      FieldName = 'IDENT'
      Size = 64
      Calculated = True
    end
    object ibqTxOueryPOST: TStringField
      FieldKind = fkCalculated
      FieldName = 'POST'
      Size = 64
      Calculated = True
    end
    object ibqTxOueryNR_LICENSE_NUMLICENSE: TStringField
      DisplayWidth = 256
      FieldKind = fkCalculated
      FieldName = 'NR_LICENSE_NUMLICENSE'
      Size = 100
      Calculated = True
    end
    object ibqTxOueryNR_LICENSE_DATEFROM: TStringField
      FieldKind = fkCalculated
      FieldName = 'NR_LICENSE_DATEFROM'
      Size = 256
      Calculated = True
    end
    object ibqTxOueryNR_LICENSE_DATETO: TStringField
      DisplayWidth = 256
      FieldKind = fkCalculated
      FieldName = 'NR_LICENSE_DATETO'
      Size = 150
      Calculated = True
    end
  end
end

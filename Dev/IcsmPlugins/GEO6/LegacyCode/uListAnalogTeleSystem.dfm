inherited frmListAnalogTeleSystem: TfrmListAnalogTeleSystem
  Tag = 4
  Caption = #1057#1080#1089#1090#1077#1084#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1058#1041
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1057#1080#1089#1090#1077#1084#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1058#1041
  end
  inherited trList: TIBTransaction
    Active = True
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from ANALOGTELESYSTEM'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into ANALOGTELESYSTEM'
      '  (ID, NAMESYSTEM, NUMBERLINES, CHANNELBAND, VIDEOBAND, '
      'SEPARATEVIDEOSOUND1, '
      '   VESTIGIALBAND, VIDEOMODULATION, SOUND1MODULATION, '
      'SOUND2SYSTEM, SEPARATEVIDEOSOUND2, ENUMVAL, FREQUENCYGRID_ID, '
      'DESCR)'
      'values'
      '  (:ID, :NAMESYSTEM, :NUMBERLINES, :CHANNELBAND, :VIDEOBAND, '
      ':SEPARATEVIDEOSOUND1, '
      '   :VESTIGIALBAND, :VIDEOMODULATION, :SOUND1MODULATION, '
      ':SOUND2SYSTEM, '
      '   :SEPARATEVIDEOSOUND2, :ENUMVAL, :FREQUENCYGRID_ID, :DESCR)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAMESYSTEM,'
      '  NUMBERLINES,'
      '  CHANNELBAND,'
      '  VIDEOBAND,'
      '  SEPARATEVIDEOSOUND1,'
      '  VESTIGIALBAND,'
      '  VIDEOMODULATION,'
      '  SOUND1MODULATION,'
      '  SOUND2SYSTEM,'
      '  SEPARATEVIDEOSOUND2,'
      '  ENUMVAL, '
      '  FREQUENCYGRID_ID,'
      '  DESCR'
      'from ANALOGTELESYSTEM '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, NAMESYSTEM, NUMBERLINES, CHANNELBAND, VIDEOBAND, SEPA' +
        'RATEVIDEOSOUND1, VESTIGIALBAND, VIDEOMODULATION, SOUND1MODULATIO' +
        'N, SOUND2SYSTEM, SEPARATEVIDEOSOUND2, ENUMVAL, FREQUENCYGRID_ID,' +
        ' DESCR'
      'from ANALOGTELESYSTEM'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update ANALOGTELESYSTEM'
      'set'
      '  NAMESYSTEM = :NAMESYSTEM,'
      '  NUMBERLINES = :NUMBERLINES,'
      '  CHANNELBAND = :CHANNELBAND,'
      '  VIDEOBAND = :VIDEOBAND,'
      '  SEPARATEVIDEOSOUND1 = :SEPARATEVIDEOSOUND1,'
      '  VESTIGIALBAND = :VESTIGIALBAND,'
      '  VIDEOMODULATION = :VIDEOMODULATION,'
      '  SOUND1MODULATION = :SOUND1MODULATION,'
      '  SOUND2SYSTEM = :SOUND2SYSTEM,'
      '  SEPARATEVIDEOSOUND2 = :SEPARATEVIDEOSOUND2,'
      '  ENUMVAL = :ENUMVAL,'
      '  FREQUENCYGRID_ID = :FREQUENCYGRID_ID,'
      '  DESCR = :DESCR'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'ANALOGTELESYSTEM.ID'
      Required = True
      Visible = False
    end
    object dstListNAMESYSTEM: TIBStringField
      Alignment = taCenter
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAMESYSTEM'
      Origin = 'ANALOGTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object dstListNUMBERLINES: TSmallintField
      DisplayLabel = 'C'#1090#1088#1086#1082#1080
      DisplayWidth = 7
      FieldName = 'NUMBERLINES'
      Origin = 'ANALOGTELESYSTEM.NUMBERLINES'
    end
    object dstListCHANNELBAND: TFloatField
      DisplayLabel = #1057#1084#1091#1075#1072' '#1082#1072#1085#1072#1083#1072
      FieldName = 'CHANNELBAND'
      Origin = 'ANALOGTELESYSTEM.CHANNELBAND'
      DisplayFormat = '0.000'
    end
    object dstListVIDEOBAND: TFloatField
      DisplayLabel = #1057#1084#1091#1075#1072' '#1074#1110#1076#1077#1086
      FieldName = 'VIDEOBAND'
      Origin = 'ANALOGTELESYSTEM.VIDEOBAND'
      DisplayFormat = '0.000'
    end
    object dstListSEPARATEVIDEOSOUND1: TFloatField
      DisplayLabel = #1047#1089#1091#1074' '#1079#1074#1091#1082#1091
      FieldName = 'SEPARATEVIDEOSOUND1'
      Origin = 'ANALOGTELESYSTEM.SEPARATEVIDEOSOUND1'
      DisplayFormat = '0.000'
    end
    object dstListVESTIGIALBAND: TFloatField
      DisplayLabel = #1054#1089#1090#1072#1090#1086#1095#1085#1072' '#1096#1080#1088#1080#1085#1072
      FieldName = 'VESTIGIALBAND'
      Origin = 'ANALOGTELESYSTEM.VESTIGIALBAND'
      DisplayFormat = '0.000'
    end
    object dstListVIDEOMODULATION: TIBStringField
      DisplayLabel = #1052#1086#1076'. '#1074#1110#1076#1077#1086
      DisplayWidth = 8
      FieldName = 'VIDEOMODULATION'
      Origin = 'ANALOGTELESYSTEM.VIDEOMODULATION'
      Size = 16
    end
    object dstListSOUND1MODULATION: TIBStringField
      DisplayLabel = #1052#1086#1076'. '#1079#1074#1091#1082#1091
      FieldName = 'SOUND1MODULATION'
      Origin = 'ANALOGTELESYSTEM.SOUND1MODULATION'
      Size = 8
    end
    object dstListSOUND2SYSTEM: TIBStringField
      DisplayLabel = #1057#1080#1089#1090#1077#1084#1072' '#1074#1090#1086#1088'. '#1079#1074#1091#1082#1072
      FieldName = 'SOUND2SYSTEM'
      Origin = 'ANALOGTELESYSTEM.SOUND2SYSTEM'
      Size = 8
    end
    object dstListSEPARATEVIDEOSOUND2: TFloatField
      DisplayLabel = #1047#1089#1091#1074' '#1074#1090#1086#1088'. '#1079#1074#1091#1082#1091
      FieldName = 'SEPARATEVIDEOSOUND2'
      Origin = 'ANALOGTELESYSTEM.SEPARATEVIDEOSOUND2'
      DisplayFormat = '0.000'
    end
    object dstListDESCR: TIBStringField
      DisplayLabel = #1054#1087#1080#1089#1072#1085#1080#1077
      DisplayWidth = 40
      FieldName = 'DESCR'
      Size = 128
    end
    object dstListENUMVAL: TSmallintField
      DisplayLabel = #1050#1086#1085#1089#1090
      DisplayWidth = 3
      FieldName = 'ENUMVAL'
      Origin = 'ANALOGTELESYSTEM.ENUMVAL'
      Required = True
    end
    object dstListFREQUENCYGRID_ID: TIntegerField
      DisplayLabel = #1057#1110#1090#1082#1072' '#1082#1072#1085#1072#1083#1110#1074
      DisplayWidth = 10
      FieldName = 'FREQUENCYGRID_ID'
      Origin = 'ANALOGTELESYSTEM.FREQUENCYGRID_ID'
      Required = True
      Visible = False
    end
    object dstListGRIDNAME: TStringField
      DisplayLabel = #1057#1110#1090#1082#1072' '#1082#1072#1085#1072#1083#1110#1074
      FieldKind = fkLookup
      FieldName = 'GRIDNAME'
      LookupDataSet = dstGrids
      LookupKeyFields = 'ID'
      LookupResultField = 'NAME'
      KeyFields = 'FREQUENCYGRID_ID'
      Size = 4
      Lookup = True
    end
  end
  object dsGrids: TDataSource
    DataSet = dstGrids
    Left = 256
    Top = 56
  end
  object dstGrids: TIBDataSet
    Database = dmMain.dbMain
    Transaction = trList
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAME from FREQUENCYGRID')
    Active = True
    Left = 296
    Top = 56
    object dstGridsID: TIntegerField
      FieldName = 'ID'
      Origin = 'FREQUENCYGRID.ID'
      Required = True
    end
    object dstGridsNAME: TIBStringField
      FieldName = 'NAME'
      Origin = 'FREQUENCYGRID.NAME'
      Required = True
      Size = 4
    end
  end
end

inherited frmListChannel: TfrmListChannel
  Tag = 10
  Left = 468
  Top = 167
  Caption = #1050#1072#1085#1072#1083#1080' / '#1095#1072#1089#1090#1086#1090#1080
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1050#1072#1085#1072#1083#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1058#1041
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from CHANNELS'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into CHANNELS'
      
        '  (ID, NAMECHANNEL, FREQUENCYGRID_ID, FMSOUNDCARRIERSECOND, FREQ' +
        'CARRIERNICAM, '
      '   FREQCARRIERSOUND, FREQCARRIERVISION, FREQFROM, FREQTO)'
      'values'
      
        '  (:ID, :NAMECHANNEL, :FREQUENCYGRID_ID, :FMSOUNDCARRIERSECOND, ' +
        ':FREQCARRIERNICAM, '
      '   :FREQCARRIERSOUND, :FREQCARRIERVISION, :FREQFROM, :FREQTO)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  FREQUENCYGRID_ID,'
      '  NAMECHANNEL,'
      '  FREQFROM,'
      '  FREQTO,'
      '  FREQCARRIERVISION,'
      '  FREQCARRIERSOUND,'
      '  FMSOUNDCARRIERSECOND,'
      '  FREQCARRIERNICAM'
      'from CHANNELS '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, NAMECHANNEL, FREQUENCYGRID_ID, FMSOUNDCARRIERSECOND, ' +
        'FREQCARRIERNICAM, FREQCARRIERSOUND, FREQCARRIERVISION, FREQFROM,' +
        ' FREQTO'
      'from CHANNELS'
      'where FREQUENCYGRID_ID = :GRP_ID'
      'ORDER BY 1')
    ModifySQL.Strings = (
      'update CHANNELS'
      'set'
      '  NAMECHANNEL = :NAMECHANNEL,'
      '  FREQUENCYGRID_ID = :FREQUENCYGRID_ID,'
      '  FMSOUNDCARRIERSECOND = :FMSOUNDCARRIERSECOND,'
      '  FREQCARRIERNICAM = :FREQCARRIERNICAM,'
      '  FREQCARRIERSOUND = :FREQCARRIERSOUND,'
      '  FREQCARRIERVISION = :FREQCARRIERVISION,'
      '  FREQFROM = :FREQFROM,'
      '  FREQTO = :FREQTO'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'CHANNELS.ID'
      Required = True
      Visible = False
    end
    object dstListNAMECHANNEL: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAMECHANNEL'
      Origin = 'CHANNELS.NAMECHANNEL'
      Size = 4
    end
    object dstListFREQFROM: TFloatField
      DisplayLabel = #1044#1110#1072#1087#1072#1079#1086#1085' '#1086#1090
      FieldName = 'FREQFROM'
      Origin = 'CHANNELS.FREQFROM'
      DisplayFormat = '0.000'
    end
    object dstListFREQTO: TFloatField
      DisplayLabel = #1044#1110#1072#1087#1072#1079#1086#1085' '#1076#1086
      FieldName = 'FREQTO'
      Origin = 'CHANNELS.FREQTO'
      DisplayFormat = '0.000'
    end
    object dstListFREQCARRIERVISION: TFloatField
      DisplayLabel = #1053#1077#1089#1091#1095#1072' '#1042#1110#1076#1077#1086
      FieldName = 'FREQCARRIERVISION'
      Origin = 'CHANNELS.FREQCARRIERVISION'
      DisplayFormat = '0.000'
    end
    object dstListFREQCARRIERSOUND: TFloatField
      DisplayLabel = #1053#1077#1089#1091#1095#1072' '#1079#1074#1091#1082
      FieldName = 'FREQCARRIERSOUND'
      Origin = 'CHANNELS.FREQCARRIERSOUND'
      DisplayFormat = '0.000'
    end
    object dstListFMSOUNDCARRIERSECOND: TFloatField
      DisplayLabel = 'dual FM '
      FieldName = 'FMSOUNDCARRIERSECOND'
      Origin = 'CHANNELS.FMSOUNDCARRIERSECOND'
      DisplayFormat = '0.000'
    end
    object dstListFREQCARRIERNICAM: TFloatField
      DisplayLabel = 'NICAM '
      FieldName = 'FREQCARRIERNICAM'
      Origin = 'CHANNELS.FREQCARRIERNICAM'
      DisplayFormat = '0.000'
    end
    object dstListFREQUENCYGRID_ID: TIntegerField
      FieldName = 'FREQUENCYGRID_ID'
      Origin = 'CHANNELS.FREQUENCYGRID_ID'
      Required = True
      Visible = False
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select FREQUENCYGRID_ID from CHANNELS where ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'select ID, NAME from FREQUENCYGRID')
  end
end

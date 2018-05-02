inherited frmListRadioService: TfrmListRadioService
  Tag = 30
  Caption = #1056#1072#1076#1110#1086#1089#1083#1091#1078#1073#1080
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1056#1072#1076#1110#1086#1089#1083#1091#1078#1073#1080
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from RADIOSERVICE'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into RADIOSERVICE'
      '  (ID, CODE, NAME, FREQFROM, FREQTO, DESCRIPTION)'
      'values'
      '  (:ID, :CODE, :NAME, :FREQFROM, :FREQTO, :DESCRIPTION)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAME,'
      '  DESCRIPTION,'
      '  CODE,'
      '  FREQFROM,'
      '  FREQTO'
      'from RADIOSERVICE '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, CODE, NAME, FREQFROM, FREQTO, DESCRIPTION '
      'from RADIOSERVICE'
      'ORDER BY 4')
    ModifySQL.Strings = (
      'update RADIOSERVICE'
      'set'
      '  CODE = :CODE,'
      '  NAME = :NAME,'
      '  FREQFROM = :FREQFROM,'
      '  FREQTO = :FREQTO,'
      '  DESCRIPTION = :DESCRIPTION'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'RADIOSERVICE.ID'
      Required = True
      Visible = False
    end
    object dstListCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'RADIOSERVICE.CODE'
      Size = 4
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'RADIOSERVICE.NAME'
      Size = 64
    end
    object dstListFREQFROM: TFloatField
      DisplayLabel = #1042#1110#1076
      FieldName = 'FREQFROM'
      Origin = 'RADIOSERVICE.FREQFROM'
      DisplayFormat = '0.000'
    end
    object dstListFREQTO: TFloatField
      DisplayLabel = #1044#1086
      FieldName = 'FREQTO'
      Origin = 'RADIOSERVICE.FREQTO'
      DisplayFormat = '0.000'
    end
    object dstListDESCRIPTION: TIBStringField
      DisplayLabel = #1054#1087#1080#1089
      FieldName = 'DESCRIPTION'
      Origin = 'RADIOSERVICE.DESCRIPTION'
      Size = 64
    end
  end
end

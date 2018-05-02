inherited frmListCity: TfrmListCity
  Tag = 11
  Left = 281
  Top = 164
  Caption = #1053#1072#1089#1077#1083#1077#1085#1110' '#1087#1091#1085#1082#1090#1080
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1053#1072#1089#1077#1083#1077#1085#1110' '#1087#1091#1085#1082#1090#1080
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from CITY'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into CITY'
      '  (ID, NAME, DISTRICT_ID, AREA_ID)'
      'values'
      '  (:ID, :NAME, :DISTRICT_ID, :AREA_ID)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  DISTRICT_ID,'
      '  NAME,'
      '  AREA_ID,'
      '  COUNTRY_ID'
      'from CITY '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, NAME, DISTRICT_ID, AREA_ID'
      'from CITY'
      'where DISTRICT_ID = :GRP_ID'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update CITY'
      'set'
      '  NAME = :NAME,'
      '  DISTRICT_ID = :DISTRICT_ID,'
      '  AREA_ID = :AREA_ID'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'CITY.ID'
      Required = True
      Visible = False
    end
    object dstListNAME2: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'CITY.NAME'
      Size = 32
    end
    object dstListDISTRICT_ID: TIntegerField
      FieldName = 'DISTRICT_ID'
      Origin = 'CITY.DISTRICT_ID'
      Visible = False
    end
    object dstListAREA_ID: TIntegerField
      FieldName = 'AREA_ID'
      Origin = 'CITY.AREA_ID'
      Visible = False
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select DISTRICT_ID, AREA_ID, COUNTRY_ID from CITY'
      'where ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'select ID, NAME from COUNTRY order by NAME'
      
        'select ID, NAME from AREA where COUNTRY_ID = :GRP_ID order by NA' +
        'ME'
      
        'select ID, NAME from DISTRICT where AREA_ID = :GRP_ID order by N' +
        'AME')
  end
end

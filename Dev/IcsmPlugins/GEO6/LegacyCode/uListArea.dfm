inherited frmListArea: TfrmListArea
  Tag = 5
  Left = 479
  Top = 220
  Width = 451
  Caption = #1056#1077#1075#1110#1086#1085#1080
  PixelsPerInch = 96
  TextHeight = 13
  inherited tbrList: TToolBar
    Width = 443
  end
  inherited panList: TPanel
    Width = 319
    Caption = #1056#1077#1075#1110#1086#1085#1080
    inherited dgrList: TDBGrid
      Width = 319
      Columns = <
        item
          Expanded = False
          FieldName = 'NAME'
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'NUMREGION'
          Visible = True
        end>
    end
    inherited stPath: TStaticText
      Width = 319
    end
    inherited panSearch: TPanel
      Width = 319
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from AREA'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into AREA'
      '  (ID, COUNTRY_ID, NAME, NUMREGION)'
      'values'
      '  (:ID, :COUNTRY_ID, :NAME, :NUMREGION)')
    RefreshSQL.Strings = (
      'select A.ID, A.NAME, NUMREGION, COUNTRY_ID, C.NAME COUNTRY_NAME'
      'from AREA A left outer join COUNTRY C on (A.COUNTRY_ID = C.ID)'
      'where'
      '  A.ID = :ID')
    SelectSQL.Strings = (
      'select A.ID, A.NAME, NUMREGION, COUNTRY_ID, C.NAME COUNTRY_NAME'
      'from AREA A left outer join COUNTRY C on (A.COUNTRY_ID = C.ID)'
      'where COUNTRY_ID = :GRP_ID'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update AREA'
      'set'
      '  COUNTRY_ID = :COUNTRY_ID,'
      '  NAME = :NAME,'
      '  NUMREGION = :NUMREGION'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'AREA.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'AREA.NAME'
      Size = 32
    end
    object dstListCOUNTRY_ID: TIntegerField
      Tag = 14
      FieldName = 'COUNTRY_ID'
      Origin = 'AREA.COUNTRY_ID'
      Required = True
      Visible = False
    end
    object dstListCOUNTRY_NAME: TIBStringField
      Tag = 3
      DisplayLabel = #1050#1088#1072#1111#1085#1072
      FieldName = 'COUNTRY_NAME'
      Origin = 'COUNTRY.NAME'
      Visible = False
      Size = 32
    end
    object dstListNUMREGION: TIBStringField
      DisplayLabel = #1053#1086#1084#1077#1088
      FieldName = 'NUMREGION'
      Origin = 'AREA.NUMREGION'
      Size = 4
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select COUNTRY_ID '
      'from AREA'
      'where ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'select ID, NAME from COUNTRY order by NAME')
  end
end

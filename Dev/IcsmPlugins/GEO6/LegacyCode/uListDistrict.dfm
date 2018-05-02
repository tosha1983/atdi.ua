inherited frmListDistrict: TfrmListDistrict
  Tag = 18
  Left = 335
  Top = 189
  Width = 427
  Caption = #1056#1072#1081#1086#1085#1080
  PixelsPerInch = 96
  TextHeight = 13
  inherited splTree: TSplitter
    Left = 185
  end
  inherited tbrList: TToolBar
    Width = 419
  end
  inherited panList: TPanel
    Left = 188
    Width = 231
    Caption = #1056#1072#1081#1086#1085#1080
    inherited dgrList: TDBGrid
      Width = 231
    end
    inherited stPath: TStaticText
      Width = 231
    end
  end
  inherited panTree: TPanel
    Width = 185
    inherited trvList: TTreeView
      Width = 185
    end
    inherited stxListQuantity: TStaticText
      Width = 185
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from DISTRICT'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into DISTRICT'
      '  (ID, NAME, AREA_ID)'
      'values'
      '  (:ID, :NAME, :AREA_ID)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  AREA_ID,'
      '  NAME'
      'from DISTRICT '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select d.ID, d.NAME, d.AREA_ID '
      'from DISTRICT d'
      'where d.AREA_ID = :GRP_ID'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update DISTRICT'
      'set'
      '  NAME = :NAME,'
      '  AREA_ID = :AREA_ID'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'DISTRICT.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'DISTRICT.NAME'
      Size = 32
    end
    object dstListAREA_ID: TIntegerField
      FieldName = 'AREA_ID'
      Origin = 'DISTRICT.AREA_ID'
      Required = True
      Visible = False
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select AREA_ID from DISTRICT'
      'where ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'select ID, NAME from COUNTRY order by NAME'
      
        'select ID, NAME from AREA where COUNTRY_ID = :GRP_ID order by NA' +
        'ME')
  end
end

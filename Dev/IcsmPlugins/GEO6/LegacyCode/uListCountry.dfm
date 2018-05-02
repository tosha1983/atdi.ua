inherited frmListCountry: TfrmListCountry
  Tag = 14
  Left = 329
  Top = 349
  Width = 792
  Caption = #1050#1088#1072#1111#1085#1080
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited tbrList: TToolBar
    Width = 784
  end
  inherited panList: TPanel
    Width = 784
    Caption = #1050#1088#1072#1111#1085#1080
    inherited dgrList: TDBGrid
      Width = 784
    end
    inherited panSearch: TPanel
      Width = 784
    end
  end
  inherited trList: TIBTransaction
    Active = True
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from COUNTRY'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into COUNTRY'
      '  (ID, NAME, CODE, DESCRIPTION)'
      'values'
      '  (:ID, :NAME, :CODE, :DESCRIPTION)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAME,'
      '  DESCRIPTION,'
      '  CODE'
      ', DEF_TVA_SYS, DEF_COLOR, DEF_FM_SYS, DEF_DVB_SYS'
      ''
      'from COUNTRY '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, NAME, CODE, DESCRIPTION, DEF_TVA_SYS, DEF_COLOR, DEF_' +
        'FM_SYS, DEF_DVB_SYS'
      'from COUNTRY'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update COUNTRY'
      'set'
      '  NAME = :NAME,'
      '  CODE = :CODE,'
      '  DESCRIPTION = :DESCRIPTION,'
      '  DEF_TVA_SYS = :DEF_TVA_SYS, '
      '  DEF_COLOR = :DEF_COLOR, '
      '  DEF_FM_SYS = :DEF_FM_SYS, '
      '  DEF_DVB_SYS = :DEF_DVB_SYS'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'COUNTRY.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'COUNTRY.NAME'
      Size = 32
    end
    object dstListCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
    object dstListDEF_TVA_SYS: TIntegerField
      FieldName = 'DEF_TVA_SYS'
      Origin = 'COUNTRY.DEF_TVA_SYS'
      Required = True
      Visible = False
    end
    object dstListDEF_COLOR: TIBStringField
      DisplayLabel = #1040#1058#1041' '#1082#1086#1083#1110#1088
      FieldName = 'DEF_COLOR'
      Origin = 'COUNTRY.DEF_COLOR'
      Size = 8
    end
    object dstListDEF_FM_SYS: TIntegerField
      FieldName = 'DEF_FM_SYS'
      Origin = 'COUNTRY.DEF_FM_SYS'
      Required = True
      Visible = False
    end
    object dstListDEF_DVB_SYS: TIntegerField
      FieldName = 'DEF_DVB_SYS'
      Origin = 'COUNTRY.DEF_DVB_SYS'
      Required = True
      Visible = False
    end
    object dstListDEF_TVA_NAME: TIBStringField
      DisplayLabel = #1040#1058#1041' '#1089#1080#1089#1090#1077#1084#1072
      FieldKind = fkLookup
      FieldName = 'DEF_TVA_NAME'
      LookupDataSet = dstTvaLookup
      LookupKeyFields = 'ID'
      LookupResultField = 'NAMESYSTEM'
      KeyFields = 'DEF_TVA_SYS'
      Size = 4
      Lookup = True
    end
    object dstListDEF_FM_NAME: TIBStringField
      DisplayLabel = #1040#1056#1052' '#1089#1080#1089#1090#1077#1084#1072
      FieldKind = fkLookup
      FieldName = 'DEF_FM_NAME'
      LookupDataSet = dstFmLookup
      LookupKeyFields = 'ID'
      LookupResultField = 'CODSYSTEM'
      KeyFields = 'DEF_FM_SYS'
      Size = 8
      Lookup = True
    end
    object dstListDEF_DVB_NAME: TIBStringField
      DisplayLabel = #1062#1058#1041' '#1089#1080#1089#1090#1077#1084#1072
      FieldKind = fkLookup
      FieldName = 'DEF_DVB_NAME'
      LookupDataSet = dstDvbLookup
      LookupKeyFields = 'ID'
      LookupResultField = 'NAMESYSTEM'
      KeyFields = 'DEF_DVB_SYS'
      Size = 4
      Lookup = True
    end
    object dstListDESCRIPTION: TIBStringField
      DisplayLabel = #1055#1088#1080#1084#1110#1090#1082#1072
      FieldName = 'DESCRIPTION'
      Origin = 'COUNTRY.DESCRIPTION'
      Size = 32
    end
  end
  object dstTvaLookup: TIBDataSet
    Database = dmMain.dbMain
    Transaction = trList
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAMESYSTEM from ANALOGTELESYSTEM'
      'order by 2')
    Active = True
    Left = 164
    Top = 144
    object dstTvaLookupID: TIntegerField
      FieldName = 'ID'
      Origin = 'ANALOGTELESYSTEM.ID'
      Required = True
    end
    object dstTvaLookupNAMESYSTEM: TIBStringField
      FieldName = 'NAMESYSTEM'
      Origin = 'ANALOGTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
  end
  object dstFmLookup: TIBDataSet
    Database = dmMain.dbMain
    Transaction = trList
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, CODSYSTEM from ANALOGRADIOSYSTEM'
      'order by 2')
    Active = True
    Left = 204
    Top = 144
    object dstFmLookupID: TIntegerField
      FieldName = 'ID'
      Origin = 'ANALOGRADIOSYSTEM.ID'
      Required = True
    end
    object dstFmLookupCODSYSTEM: TIBStringField
      FieldName = 'CODSYSTEM'
      Origin = 'ANALOGRADIOSYSTEM.CODSYSTEM'
      Size = 8
    end
  end
  object dstDvbLookup: TIBDataSet
    Database = dmMain.dbMain
    Transaction = trList
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, NAMESYSTEM from DIGITALTELESYSTEM'
      'order by 2')
    Active = True
    Left = 244
    Top = 144
    object dstDvbLookupID: TIntegerField
      FieldName = 'ID'
      Origin = 'DIGITALTELESYSTEM.ID'
      Required = True
    end
    object dstDvbLookupNAMESYSTEM: TIBStringField
      FieldName = 'NAMESYSTEM'
      Origin = 'DIGITALTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
  end
end

inherited frmListTelecomOrganization: TfrmListTelecomOrganization
  Tag = 36
  Left = 299
  Top = 156
  Caption = #1040#1076#1084#1110#1085#1110#1089#1090#1088#1072#1094#1110#1111' '#1079#1074#39#1103#1079#1082#1091
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1040#1076#1084#1110#1085#1110#1089#1090#1088#1072#1094#1110#1111' '#1079#1074#39#1103#1079#1082#1091
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from TELECOMORGANIZATION'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into TELECOMORGANIZATION'
      
        '  (ID, CODE, NAME, PHONE, MAIL, COUNTRY_ID, ADDRESS, COORDDOCUME' +
        'NT)'
      'values'
      
        '  (:ID, :CODE, :NAME, :PHONE, :MAIL, :COUNTRY_ID, :ADDRESS, :COO' +
        'RDDOCUMENT)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CODE,'
      '  NAME,'
      '  ADDRESS,'
      '  MAIL,'
      '  PHONE,'
      '  COUNTRY_ID,'
      '  COORDDOCUMENT'
      'from TELECOMORGANIZATION '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select T.ID, T.CODE, T.NAME, T.PHONE, T.MAIL, T.COUNTRY_ID, C.CO' +
        'DE C_CODE, T.ADDRESS,  T.COORDDOCUMENT'
      
        'from TELECOMORGANIZATION T left outer join COUNTRY C on (T.COUNT' +
        'RY_ID = C.ID)'
      'order by 3')
    ModifySQL.Strings = (
      'update TELECOMORGANIZATION'
      'set'
      '  CODE = :CODE,'
      '  NAME = :NAME,'
      '  PHONE = :PHONE,'
      '  MAIL = :MAIL,'
      '  COUNTRY_ID = :COUNTRY_ID,'
      '  ADDRESS = :ADDRESS,'
      '  COORDDOCUMENT = :COORDDOCUMENT'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'TELECOMORGANIZATION.ID'
      Required = True
      Visible = False
    end
    object dstListCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'TELECOMORGANIZATION.CODE'
      Size = 4
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'TELECOMORGANIZATION.NAME'
      Size = 16
    end
    object dstListCOUNTRY_ID: TIntegerField
      FieldName = 'COUNTRY_ID'
      Origin = 'TELECOMORGANIZATION.COUNTRY_ID'
      Required = True
      Visible = False
    end
    object dstListC_CODE: TIBStringField
      DisplayLabel = #1050#1088#1072#1111#1085#1072
      DisplayWidth = 6
      FieldKind = fkLookup
      FieldName = 'C_CODE'
      LookupDataSet = IBDataSet1
      LookupKeyFields = 'ID'
      LookupResultField = 'CODE'
      KeyFields = 'COUNTRY_ID'
      Origin = 'COUNTRY.CODE'
      Size = 4
      Lookup = True
    end
    object dstListCOORDDOCUMENT: TSmallintField
      Alignment = taCenter
      DisplayLabel = #1040#1074#1090#1086#1059#1079#1075
      DisplayWidth = 7
      FieldName = 'COORDDOCUMENT'
      Origin = 'TELECOMORGANIZATION.COORDDOCUMENT'
      OnGetText = dstListCOORDDOCUMENTGetText
      OnSetText = dstListCOORDDOCUMENTSetText
    end
    object dstListMAIL: TIBStringField
      DisplayLabel = 'e-mail'
      FieldName = 'MAIL'
      Origin = 'TELECOMORGANIZATION.MAIL'
      Size = 32
    end
    object dstListPHONE: TIBStringField
      DisplayLabel = #1058#1077#1083#1077#1092#1086#1085
      FieldName = 'PHONE'
      Origin = 'TELECOMORGANIZATION.PHONE'
      Size = 16
    end
    object dstListADDRESS: TIBStringField
      DisplayLabel = #1040#1076#1088#1077#1089#1072
      DisplayWidth = 32
      FieldName = 'ADDRESS'
      Origin = 'TELECOMORGANIZATION.ADDRESS'
      Size = 64
    end
  end
  object IBDataSet1: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      'select ID, CODE from COUNTRY  order by 2')
    Left = 152
    Top = 112
    object IBDataSet1ID: TIntegerField
      FieldName = 'ID'
      Origin = 'COUNTRY.ID'
      Required = True
    end
    object IBDataSet1CODE: TIBStringField
      FieldName = 'CODE'
      Origin = 'COUNTRY.CODE'
      Size = 4
    end
  end
end

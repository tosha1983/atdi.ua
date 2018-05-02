inherited frmListAnalogRadioSystem: TfrmListAnalogRadioSystem
  Tag = 3
  Caption = #1057#1080#1089#1090#1077#1084#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1056#1052
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1057#1080#1089#1090#1077#1084#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1056#1052
  end
  inherited trList: TIBTransaction
    Active = True
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from ANALOGRADIOSYSTEM'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into ANALOGRADIOSYSTEM'
      '  (ID, CODSYSTEM, TYPESYSTEM, MODULATION, DEVIATION, '
      'DESCR, ENUMVAL)'
      'values'
      '  (:ID, :CODSYSTEM, :TYPESYSTEM, :MODULATION, :DEVIATION, '
      ':DESCR, :ENUMVAL)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CODSYSTEM,'
      '  TYPESYSTEM,'
      '  MODULATION,'
      '  DEVIATION,'
      '  DESCR,'
      '  ENUMVAL'
      'from ANALOGRADIOSYSTEM '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, CODSYSTEM, TYPESYSTEM, MODULATION, DEVIATION, DESCR, ' +
        'ENUMVAL'
      'from ANALOGRADIOSYSTEM'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update ANALOGRADIOSYSTEM'
      'set'
      '  CODSYSTEM = :CODSYSTEM,'
      '  TYPESYSTEM = :TYPESYSTEM,'
      '  MODULATION = :MODULATION,'
      '  DEVIATION = :DEVIATION,'
      '  ENUMVAL = :ENUMVAL,'
      '  DESCR = :DESCR'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'ANALOGRADIOSYSTEM.ID'
      Required = True
      Visible = False
    end
    object dstListCODSYSTEM: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      DisplayWidth = 10
      FieldName = 'CODSYSTEM'
      Origin = 'ANALOGRADIOSYSTEM.CODSYSTEM'
    end
    object dstListTYPESYSTEM: TIBStringField
      DisplayLabel = #1058#1080#1087
      FieldName = 'TYPESYSTEM'
      Origin = 'ANALOGRADIOSYSTEM.TYPESYSTEM'
      Size = 8
    end
    object dstListMODULATION: TIBStringField
      DisplayLabel = #1052#1086#1076#1091#1083#1103#1094#1110#1103
      FieldName = 'MODULATION'
      Origin = 'ANALOGRADIOSYSTEM.MODULATION'
      Size = 8
    end
    object dstListDEVIATION: TFloatField
      DisplayLabel = #1044#1077#1074#1110#1072#1094#1110#1103
      FieldName = 'DEVIATION'
      Origin = 'ANALOGRADIOSYSTEM.DEVIATION'
    end
    object dstListDESCR: TIBStringField
      DisplayLabel = #1054#1087#1080#1089#1072#1085#1080#1077
      DisplayWidth = 40
      FieldName = 'DESCR'
      Size = 128
    end
    object dstListENUMVAL: TIntegerField
      DisplayLabel = #1050#1086#1085#1089#1090
      DisplayWidth = 4
      FieldName = 'ENUMVAL'
      Origin = 'ANALOGRADIOSYSTEM.ENUMVAL'
      Required = True
    end
  end
end

inherited frmListEquipment: TfrmListEquipment
  Tag = 20
  Caption = #1059#1089#1090#1072#1090#1082#1091#1074#1072#1085#1085#1103
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1059#1089#1090#1072#1090#1082#1091#1074#1072#1085#1085#1103
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from EQUIPMENT'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into EQUIPMENT'
      '  (ID, TYPEEQUIPMENT, NAME, MANUFACTURE)'
      'values'
      '  (:ID, :TYPEEQUIPMENT, :NAME, :MANUFACTURE)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  TYPEEQUIPMENT,'
      '  NAME,'
      '  MANUFACTURE'
      'from EQUIPMENT '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, TYPEEQUIPMENT, NAME, MANUFACTURE '
      'from EQUIPMENT'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update EQUIPMENT'
      'set'
      '  TYPEEQUIPMENT = :TYPEEQUIPMENT,'
      '  NAME = :NAME,'
      '  MANUFACTURE = :MANUFACTURE'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'EQUIPMENT.ID'
      Required = True
      Visible = False
    end
    object dstListTYPEEQUIPMENT: TIBStringField
      DisplayLabel = #1058#1080#1087
      DisplayWidth = 40
      FieldName = 'TYPEEQUIPMENT'
      Origin = 'EQUIPMENT.TYPEEQUIPMENT'
      Size = 64
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'EQUIPMENT.NAME'
      Size = 32
    end
    object dstListMANUFACTURE: TIBStringField
      DisplayLabel = #1042#1080#1088#1086#1073#1085#1080#1082
      FieldName = 'MANUFACTURE'
      Origin = 'EQUIPMENT.MANUFACTURE'
      Size = 32
    end
  end
end

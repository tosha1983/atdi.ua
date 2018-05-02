inherited frmListBank: TfrmListBank
  Tag = 6
  Caption = #1041#1072#1085#1082#1080
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1041#1072#1085#1082#1080
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from BANK'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into BANK'
      '  (ID, NAME, MFO, ADDRESS)'
      'values'
      '  (:ID, :NAME, :MFO, :ADDRESS)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAME,'
      '  MFO,'
      '  ADDRESS'
      'from BANK '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, NAME, MFO, ADDRESS from BANK'
      'order by 2')
    ModifySQL.Strings = (
      'update BANK'
      'set'
      '  NAME = :NAME,'
      '  MFO = :MFO,'
      '  ADDRESS = :ADDRESS'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'BANK.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      DisplayWidth = 32
      FieldName = 'NAME'
      Origin = 'BANK.NAME'
      Size = 64
    end
    object dstListMFO: TIBStringField
      DisplayLabel = #1052#1060#1054
      FieldName = 'MFO'
      Origin = 'BANK.MFO'
      Size = 8
    end
    object dstListADDRESS: TIBStringField
      DisplayLabel = #1040#1076#1088#1077#1089#1072
      DisplayWidth = 32
      FieldName = 'ADDRESS'
      Origin = 'BANK.ADDRESS'
      Size = 64
    end
  end
end

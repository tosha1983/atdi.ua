inherited frmListTypeReceive: TfrmListTypeReceive
  Tag = 40
  Left = 629
  Top = 235
  Caption = #1058#1080#1087#1080' '#1087#1088#1080#1081#1086#1084#1091
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1058#1080#1087#1080' '#1087#1088#1080#1081#1086#1084#1091
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from TYPERECEIVE'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into TYPERECEIVE'
      '  (ID, NAME)'
      'values'
      '  (:ID, :NAME)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAME'
      'from TYPERECEIVE '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, NAME from TYPERECEIVE')
    ModifySQL.Strings = (
      'update TYPERECEIVE'
      'set'
      '  NAME = :NAME'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'TYPERECEIVE.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'TYPERECEIVE.NAME'
      Size = 16
    end
  end
end

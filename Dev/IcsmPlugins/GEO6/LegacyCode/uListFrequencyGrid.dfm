inherited frmListFrequencyGrid: TfrmListFrequencyGrid
  Tag = 47
  Caption = #1057#1110#1090#1082#1080' '#1082#1072#1085#1072#1083#1110#1074' / '#1095#1072#1089#1090#1086#1090
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1057#1110#1090#1082#1080' '#1082#1072#1085#1072#1083#1110#1074
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from FREQUENCYGRID'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into FREQUENCYGRID'
      '  (ID, NAME, DESCRIPTION)'
      'values'
      '  (:ID, :NAME, :DESCRIPTION)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAME,'
      '  DESCRIPTION'
      'from FREQUENCYGRID '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'SELECT fg.ID, fg.NAME, fg.DESCRIPTION'
      'FROM FREQUENCYGRID fg')
    ModifySQL.Strings = (
      'update FREQUENCYGRID'
      'set'
      '  NAME = :NAME,'
      '  DESCRIPTION = :DESCRIPTION'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'FREQUENCYGRID.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'NAME'
      Origin = 'FREQUENCYGRID.NAME'
      Required = True
      Size = 4
    end
    object dstListDESCRIPTION: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      DisplayWidth = 50
      FieldName = 'DESCRIPTION'
      Origin = 'FREQUENCYGRID.DESCRIPTION'
      Size = 256
    end
  end
end

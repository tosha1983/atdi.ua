inherited frmListDocTemplate: TfrmListDocTemplate
  Tag = 19
  Left = 278
  Top = 157
  Caption = #1064#1072#1073#1083#1086#1085#1080' '#1076#1086#1082#1091#1084#1077#1085#1090#1110#1074
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1064#1072#1073#1083#1086#1085#1080' '#1076#1086#1082#1091#1084#1077#1085#1090#1110#1074
  end
  inherited dstList: TIBDataSet
    AfterScroll = dstListAfterScroll
    OnNewRecord = dstListNewRecord
    DeleteSQL.Strings = (
      'delete from DOCUMENT'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into DOCUMENT'
      '  (ID, CODE, NAME, TEMPLATE, TEMPLATE2, TTYPE)'
      'values'
      '  (:ID, :CODE, :NAME, :TEMPLATE, :TEMPLATE2, :TTYPE)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CODE,'
      '  NAME,'
      '  TEMPLATE,'
      '  TEMPLATE2,'
      '  TTYPE'
      'from DOCUMENT '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'SELECT id, code, name, template, template2, tType FROM Document ' +
        'ORDER BY code')
    ModifySQL.Strings = (
      'update DOCUMENT'
      'set'
      '  CODE = :CODE,'
      '  NAME = :NAME,'
      '  TEMPLATE = :TEMPLATE,'
      '  TEMPLATE2 = :TEMPLATE2,'
      '  TTYPE = :TTYPE'
      'where'
      '  ID = :OLD_ID')
  end
end

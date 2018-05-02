inherited frmListOffsetCarryFreqTVA: TfrmListOffsetCarryFreqTVA
  Tag = 24
  Left = 289
  Top = 160
  Caption = #1058#1080#1087#1080' '#1047#1053#1063' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1058#1041
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1058#1080#1087#1080' '#1047#1053#1063' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1058#1041
  end
  inherited trList: TIBTransaction
    Active = True
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from OFFSETCARRYFREQTVA'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into OFFSETCARRYFREQTVA'
      '  (ID, CODEOFFSET, OFFSET, OFFSETLINES)'
      'values'
      '  (:ID, :CODEOFFSET, :OFFSET, :OFFSETLINES)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CODEOFFSET,'
      '  OFFSETLINES,'
      '  OFFSET'
      'from OFFSETCARRYFREQTVA '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, CODEOFFSET, OFFSET, OFFSETLINES '
      'from OFFSETCARRYFREQTVA')
    ModifySQL.Strings = (
      'update OFFSETCARRYFREQTVA'
      'set'
      '  CODEOFFSET = :CODEOFFSET,'
      '  OFFSET = :OFFSET,'
      '  OFFSETLINES = :OFFSETLINES'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'OFFSETCARRYFREQTVA.ID'
      Required = True
      Visible = False
    end
    object dstListCODEOFFSET: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODEOFFSET'
      Origin = 'OFFSETCARRYFREQTVA.CODEOFFSET'
      Size = 8
    end
    object dstListOFFSET: TIntegerField
      DisplayLabel = #1043#1094
      FieldName = 'OFFSET'
      Origin = 'OFFSETCARRYFREQTVA.OFFSET'
    end
    object dstListOFFSETLINES: TSmallintField
      DisplayLabel = #1051#1110#1085#1110#1111
      FieldName = 'OFFSETLINES'
      Origin = 'OFFSETCARRYFREQTVA.OFFSETLINES'
    end
  end
end

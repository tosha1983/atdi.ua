inherited frmListAccountCondition: TfrmListAccountCondition
  Tag = 1
  Left = 315
  Top = 206
  Width = 425
  Caption = #1054#1073#1083#1110#1082#1086#1074#1110' '#1089#1090#1072#1085#1080' '#1087#1077#1088#1077#1076#1072#1090#1095#1080#1082#1110#1074
  PixelsPerInch = 96
  TextHeight = 13
  inherited tbrList: TToolBar
    Width = 417
  end
  inherited panList: TPanel
    Width = 293
    Caption = #1054#1073#1083#1110#1082#1086#1074#1110' '#1089#1090#1072#1085#1080' '#1087#1077#1088#1077#1076#1072#1090#1095#1080#1082#1110#1074
    inherited dgrList: TDBGrid
      Width = 293
      Columns = <
        item
          Expanded = False
          FieldName = 'ID'
          Width = 45
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'CODE'
          Width = 33
          Visible = True
        end
        item
          Expanded = False
          FieldName = 'NAME'
          Visible = True
        end>
    end
    inherited stPath: TStaticText
      Width = 293
    end
    inherited panSearch: TPanel
      Width = 293
    end
  end
  inherited panTree: TPanel
    inherited trvList: TTreeView
      Items.Data = {
        02000000220000001400000015000000FFFFFFFFFFFFFFFF0000000000000000
        09C2EDF3F2F0B3F8EDB3230000001400000015000000FFFFFFFFFFFFFFFF0000
        0000000000000ACCB3E6EDE0F0EEE4EDB3}
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from ACCOUNTCONDITION'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into ACCOUNTCONDITION'
      '  (ID, TYPECONDITION, NAME, CODE)'
      'values'
      '  (:ID, :TYPECONDITION, :NAME, :CODE)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  TYPECONDITION,'
      '  NAME,'
      '  CODE'
      'from ACCOUNTCONDITION '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, CODE, TYPECONDITION, NAME '
      'from ACCOUNTCONDITION'
      'where TYPECONDITION = :GRP_ID'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update ACCOUNTCONDITION'
      'set'
      '  TYPECONDITION = :TYPECONDITION,'
      '  NAME = :NAME,'
      '  CODE = :CODE'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      DisplayWidth = 7
      FieldName = 'ID'
      Origin = 'ACCOUNTCONDITION.ID'
      ReadOnly = True
      Required = True
    end
    object dstListCODE: TIBStringField
      DisplayLabel = #1050#1086#1076
      FieldName = 'CODE'
      Origin = 'ACCOUNTCONDITION.CODE'
      Size = 4
    end
    object dstListTYPECONDITION: TSmallintField
      FieldName = 'TYPECONDITION'
      Origin = 'ACCOUNTCONDITION.TYPECONDITION'
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'ACCOUNTCONDITION.NAME'
      Size = 32
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select TYPECONDITION'
      'from ACCOUNTCONDITION'
      'where  ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'lazha')
  end
end

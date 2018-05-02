inherited frmListStreet: TfrmListStreet
  Tag = 34
  Left = 351
  Top = 175
  Width = 503
  Height = 365
  Caption = #1042#1091#1083#1080#1094#1110
  PixelsPerInch = 96
  TextHeight = 13
  inherited splTree: TSplitter
    Height = 313
  end
  inherited tbrList: TToolBar
    Width = 495
  end
  inherited panList: TPanel
    Width = 371
    Height = 313
    Caption = #1042#1091#1083#1080#1094#1110
    inherited dgrList: TDBGrid
      Width = 371
      Height = 296
    end
    inherited stPath: TStaticText
      Width = 371
    end
  end
  inherited panTree: TPanel
    Height = 313
    inherited trvList: TTreeView
      Height = 296
      AutoExpand = True
      Items.Data = {
        01000000200000001400000015000000FFFFFFFFFFFFFFFF0000000001000000
        07D3EAF0E0BFEDE0210000001400000015000000FFFFFFFFFFFFFFFF00000000
        0100000008CAE8BFE2F1FCEAE0250000001400000015000000FFFFFFFFFFFFFF
        FF00000000010000000CCCE8F0EEEDB3E2F1FCEAE8E921000000140000001500
        0000FFFFFFFFFFFFFFFF000000000000000008C1EEE3F3F1EBE0E2}
    end
    inherited stxListQuantity: TStaticText
      Top = 296
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from STREET'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into STREET'
      '  (ID, NAME, CITY_ID)'
      'values'
      '  (:ID, :NAME, :CITY_ID)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  CITY_ID,'
      '  NAME'
      'from STREET '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, NAME, CITY_ID'
      'from STREET'
      'where'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update STREET'
      'set'
      '  NAME = :NAME,'
      '  CITY_ID = :CITY_ID'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'STREET.ID'
      Required = True
      Visible = False
    end
    object dstListNAME: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAME'
      Origin = 'STREET.NAME'
      Size = 16
    end
    object dstListCITY_ID: TIntegerField
      FieldName = 'CITY_ID'
      Origin = 'STREET.CITY_ID'
      Required = True
      Visible = False
    end
  end
end

inherited frmListCoordDistance: TfrmListCoordDistance
  Tag = 12
  Left = 285
  Top = 197
  Width = 614
  Height = 348
  Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1110' '#1074#1110#1076#1089#1090#1072#1085#1110
  PixelsPerInch = 96
  TextHeight = 13
  inherited splTree: TSplitter
    Height = 296
  end
  inherited tbrList: TToolBar
    Width = 606
  end
  inherited panList: TPanel
    Width = 482
    Height = 296
    Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1094#1110#1081#1085#1110' '#1074#1110#1076#1089#1090#1072#1085#1110
    inherited dgrList: TDBGrid
      Width = 482
      Height = 279
    end
    inherited stPath: TStaticText
      Width = 482
    end
  end
  inherited panTree: TPanel
    Height = 296
    inherited trvList: TTreeView
      Height = 279
    end
    inherited stxListQuantity: TStaticText
      Top = 279
    end
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from COORDDISTANCE'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into COORDDISTANCE'
      
        '  (ID, EFFECTRADIATEPOWER, HEIGHTANTENNA, OVERLAND, OVERCOLDSEA,' +
        ' '
      'OVERWARMSEA, '
      '   GENERALLYSEA20, GENERALLYSEA40, GENERALLYSEA60, '
      'GENERALLYSEA80, GENERALLYSEA100, '
      '   MEDITERRANEANSEA20, MEDITERRANEANSEA40, MEDITERRANEANSEA60, '
      'MEDITERRANEANSEA80, '
      '   MEDITERRANEANSEA100, SYSTEMCAST_ID)'
      'values'
      '  (:ID, :EFFECTRADIATEPOWER, :HEIGHTANTENNA, :OVERLAND, '
      ':OVERCOLDSEA, :OVERWARMSEA, '
      '   :GENERALLYSEA20, :GENERALLYSEA40, :GENERALLYSEA60, '
      ':GENERALLYSEA80, '
      '   :GENERALLYSEA100, :MEDITERRANEANSEA20, :MEDITERRANEANSEA40, '
      ':MEDITERRANEANSEA60, '
      '   :MEDITERRANEANSEA80, :MEDITERRANEANSEA100, :SYSTEMCAST_ID)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  SYSTEMCAST_ID,'
      '  EFFECTRADIATEPOWER,'
      '  HEIGHTANTENNA,'
      '  OVERLAND,'
      '  OVERCOLDSEA,'
      '  OVERWARMSEA,'
      '  GENERALLYSEA20,'
      '  GENERALLYSEA40,'
      '  GENERALLYSEA60,'
      '  GENERALLYSEA80,'
      '  GENERALLYSEA100,'
      '  MEDITERRANEANSEA20,'
      '  MEDITERRANEANSEA40,'
      '  MEDITERRANEANSEA60,'
      '  MEDITERRANEANSEA80,'
      '  MEDITERRANEANSEA100'
      'from COORDDISTANCE '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, EFFECTRADIATEPOWER, HEIGHTANTENNA, OVERLAND, OVERCOLD' +
        'SEA, OVERWARMSEA, GENERALLYSEA20, GENERALLYSEA40, GENERALLYSEA60' +
        ', GENERALLYSEA80, GENERALLYSEA100, MEDITERRANEANSEA20, MEDITERRA' +
        'NEANSEA40, MEDITERRANEANSEA60, MEDITERRANEANSEA80, MEDITERRANEAN' +
        'SEA100, SYSTEMCAST_ID'
      'from COORDDISTANCE'
      'where SYSTEMCAST_ID = :GRP_ID'
      'ORDER BY 1, 2')
    ModifySQL.Strings = (
      'update COORDDISTANCE'
      'set'
      '  EFFECTRADIATEPOWER = :EFFECTRADIATEPOWER,'
      '  HEIGHTANTENNA = :HEIGHTANTENNA,'
      '  OVERLAND = :OVERLAND,'
      '  OVERCOLDSEA = :OVERCOLDSEA,'
      '  OVERWARMSEA = :OVERWARMSEA,'
      '  GENERALLYSEA20 = :GENERALLYSEA20,'
      '  GENERALLYSEA40 = :GENERALLYSEA40,'
      '  GENERALLYSEA60 = :GENERALLYSEA60,'
      '  GENERALLYSEA80 = :GENERALLYSEA80,'
      '  GENERALLYSEA100 = :GENERALLYSEA100,'
      '  MEDITERRANEANSEA20 = :MEDITERRANEANSEA20,'
      '  MEDITERRANEANSEA40 = :MEDITERRANEANSEA40,'
      '  MEDITERRANEANSEA60 = :MEDITERRANEANSEA60,'
      '  MEDITERRANEANSEA80 = :MEDITERRANEANSEA80,'
      '  MEDITERRANEANSEA100 = :MEDITERRANEANSEA100,'
      '  SYSTEMCAST_ID = :SYSTEMCAST_ID'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'COORDDISTANCE.ID'
      Required = True
      Visible = False
    end
    object dstListEFFECTRADIATEPOWER: TIntegerField
      DisplayLabel = #1045#1042#1055
      FieldName = 'EFFECTRADIATEPOWER'
      Origin = 'COORDDISTANCE.EFFECTRADIATEPOWER'
    end
    object dstListHEIGHTANTENNA: TIntegerField
      DisplayLabel = #1042#1080#1089#1086#1090#1072
      FieldName = 'HEIGHTANTENNA'
      Origin = 'COORDDISTANCE.HEIGHTANTENNA'
    end
    object dstListOVERLAND: TSmallintField
      DisplayLabel = #1047#1077#1084#1083#1103
      FieldName = 'OVERLAND'
      Origin = 'COORDDISTANCE.OVERLAND'
    end
    object dstListOVERCOLDSEA: TSmallintField
      DisplayLabel = #1061#1086#1083#1086#1076#1085#1077' '#1084#1086#1088#1077
      FieldName = 'OVERCOLDSEA'
      Origin = 'COORDDISTANCE.OVERCOLDSEA'
    end
    object dstListOVERWARMSEA: TSmallintField
      DisplayLabel = #1043#1072#1088#1103#1095#1077' '#1084#1086#1088#1077
      FieldName = 'OVERWARMSEA'
      Origin = 'COORDDISTANCE.OVERWARMSEA'
    end
    object dstListGENERALLYSEA20: TSmallintField
      DisplayLabel = #1052#1086#1088#1077' 20%'
      FieldName = 'GENERALLYSEA20'
      Origin = 'COORDDISTANCE.GENERALLYSEA20'
    end
    object dstListGENERALLYSEA40: TSmallintField
      DisplayLabel = #1052#1086#1088#1077' 40%'
      FieldName = 'GENERALLYSEA40'
      Origin = 'COORDDISTANCE.GENERALLYSEA40'
    end
    object dstListGENERALLYSEA60: TSmallintField
      DisplayLabel = #1052#1086#1088#1077' 60%'
      FieldName = 'GENERALLYSEA60'
      Origin = 'COORDDISTANCE.GENERALLYSEA60'
    end
    object dstListGENERALLYSEA80: TSmallintField
      DisplayLabel = #1052#1086#1088#1077' 80'
      FieldName = 'GENERALLYSEA80'
      Origin = 'COORDDISTANCE.GENERALLYSEA80'
    end
    object dstListGENERALLYSEA100: TSmallintField
      DisplayLabel = #1052#1086#1088#1077' 100%'
      FieldName = 'GENERALLYSEA100'
      Origin = 'COORDDISTANCE.GENERALLYSEA100'
    end
    object dstListMEDITERRANEANSEA20: TSmallintField
      DisplayLabel = #1042#1085'. '#1084#1086#1088#1077' 20%'
      FieldName = 'MEDITERRANEANSEA20'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA20'
    end
    object dstListMEDITERRANEANSEA40: TSmallintField
      DisplayLabel = #1042#1085'. '#1084#1086#1088#1077' 40%'
      FieldName = 'MEDITERRANEANSEA40'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA40'
    end
    object dstListMEDITERRANEANSEA60: TSmallintField
      DisplayLabel = #1042#1085'. '#1084#1086#1088#1077' 60%'
      FieldName = 'MEDITERRANEANSEA60'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA60'
    end
    object dstListMEDITERRANEANSEA80: TSmallintField
      DisplayLabel = #1042#1085'. '#1084#1086#1088#1077' 80%'
      FieldName = 'MEDITERRANEANSEA80'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA80'
    end
    object dstListMEDITERRANEANSEA100: TSmallintField
      DisplayLabel = #1042#1085'. '#1084#1086#1088#1077' 100%'
      FieldName = 'MEDITERRANEANSEA100'
      Origin = 'COORDDISTANCE.MEDITERRANEANSEA100'
    end
    object dstListSYSTEMCAST_ID: TIntegerField
      DefaultExpression = '0'
      FieldName = 'SYSTEMCAST_ID'
      Origin = 'COORDDISTANCE.SYSTEMCAST_ID'
      Required = True
    end
  end
  inherited sqlFindGrp: TIBSQL
    SQL.Strings = (
      'select SYSTEMCAST_ID from COORDDISTANCE'
      'where ID = :ID')
  end
  inherited sqlTree: TIBSQL
    SQL.Strings = (
      'select ID, CODE from SYSTEMCAST')
  end
end

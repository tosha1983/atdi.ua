object dmForms: TdmForms
  OldCreateOrder = False
  Left = 524
  Top = 132
  Height = 307
  Width = 504
  object ibqNamesPrograms: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select distinct(NAMEPROGRAMM), SC.ENUMVAL'
      'from TRANSMITTERS TR '
      'left outer join SYSTEMCAST SC on (SC.ID = TR.SYSTEMCAST_ID)')
    Left = 36
    Top = 8
  end
  object ibqNumSertificates: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select distinct(NUMSTANDCERTIFICATE) from TRANSMITTERS ')
    Left = 136
    Top = 8
  end
  object ibqListVideoEmission: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select distinct(VIDEO_EMISSION) from TRANSMITTERS')
    Left = 240
    Top = 8
  end
  object ibqListSoundEmission: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select distinct(SOUND_EMISSION_PRIMARY) '
      'from TRANSMITTERS')
    Left = 344
    Top = 8
  end
end

# ---------------------------------------------------------------------------
!if !$d(BCB)
BCB = $(MAKEDIR)\..
!endif

# ---------------------------------------------------------------------------
# IDE SECTION
# ---------------------------------------------------------------------------
# The following section of the project makefile is managed by the BCB IDE.
# It is recommended to use the IDE to change any of the values in this
# section.
# ---------------------------------------------------------------------------

VERSION = BCB.06.00
# ---------------------------------------------------------------------------
PROJECT = LISBCTxServer.dll
OBJFILES = LISBCTxServer.obj LISBCTxServer_TLB.obj LISBCTXIMPL.obj \
    LISBCTxServer_ATL.obj LISBCTxListImpl.obj TxAttribute.obj \
    LisBcObjectServerVersionImpl.obj LisBcDigAllotImpl.obj \
    C:\Progra~1\Borland\CBuilder6\Imports\lisbc_tlb.obj 
RESFILES = LISBCTxServer.res LISBCTxServer.tlb
MAINSOURCE = LISBCTxServer.cpp
RESDEPEN = $(RESFILES)
LIBFILES = 
IDLFILES = 
IDLGENFILES = 
LIBRARIES = vclnp.lib bcb2kaxserver.lib indy.lib dbxcds.lib dclocx.lib soaprtl.lib \
    bcbie.lib nmfast.lib inetdbxpress.lib inetdbbde.lib \
    dsnapcon.lib dsnapcrba.lib visualdbclx.lib teeqr.lib dss.lib tee.lib \
    teedb.lib teeui.lib qrpt.lib bdecds.lib vcldbx.lib bdertl.lib adortl.lib \
    vcldb.lib dbexpress.lib dsnap.lib cds.lib ibxpress.lib dbrtl.lib 
PACKAGES = vcl.bpi rtl.bpi 
SPARELIBS = vcl.lib rtl.lib dbrtl.lib ibxpress.lib cds.lib dsnap.lib dbexpress.lib \
    vcldb.lib adortl.lib bdertl.lib vcldbx.lib bdecds.lib qrpt.lib teeui.lib \
    teedb.lib tee.lib dss.lib teeqr.lib visualdbclx.lib dsnapcrba.lib \
    dsnapcon.lib inetdbbde.lib inetdbxpress.lib nmfast.lib \
    bcbie.lib soaprtl.lib dclocx.lib dbxcds.lib indy.lib bcb2kaxserver.lib
DEFFILE = 
OTHERFILES = 
# ---------------------------------------------------------------------------
DEBUGLIBPATH = $(BCB)\lib\debug
RELEASELIBPATH = $(BCB)\lib\release
USERDEFINES = 
SYSDEFINES = NO_STRICT;USING_ATLVCL;_ATL_NO_SECURITY;INC_ATL_HEADERS;_ATL_SINGLEUSE_INSTANCING;_ATL_MULTIUSE_INSTANCING;_ATL_SINGLE_THREADED;USEPACKAGES
INCLUDEPATH = ..;$(BCB)\include;$(BCB)\include\vcl;$(BCB)\include\atl;$(BCB)\Imports
LIBPATH = ..;$(BCB)\Projects\Lib;$(BCB)\lib\obj;$(BCB)\lib
WARNINGS= -w-par -w-pck
PATHCPP = .;C:\Progra~1\Borland\CBuilder6\Imports;
PATHASM = .;
PATHPAS = .;
PATHRC = .;
PATHOBJ = .;$(LIBPATH)
# ---------------------------------------------------------------------------
CFLAG1 = -WD -O2 -H=$(BCB)\precomphdr\bctx.csm -Hc -Vx -Ve -X- -a2 -b- -k- -vi -c \
    -tWM
IDLCFLAGS = -I.. -I$(BCB)\include -I$(BCB)\include\vcl -I$(BCB)\include\atl -I$(BCB)\Imports \
    -src_suffix cpp -boa
PFLAGS = -$Y- -$L- -$D- -$A8 -v -JPHNE -M
RFLAGS = 
AFLAGS = /mx /w2 /zn
LFLAGS = -D"" -aa -Tpd -x -Gn
# ---------------------------------------------------------------------------
ALLOBJ = c0d32.obj $(PACKAGES) sysinit.obj Memmgr.Lib $(OBJFILES)
ALLRES = $(RESFILES)
ALLLIB = $(LIBFILES) $(LIBRARIES) import32.lib cp32mt.lib
# ---------------------------------------------------------------------------


# ---------------------------------------------------------------------------
# MAKE SECTION
# ---------------------------------------------------------------------------
# This section of the project file is not used by the BCB IDE.  It is for
# the benefit of building from the command-line using the MAKE utility.
# ---------------------------------------------------------------------------

.autodepend
# ---------------------------------------------------------------------------
!if "$(USERDEFINES)" != ""
AUSERDEFINES = -d$(USERDEFINES:;= -d)
!else
AUSERDEFINES =
!endif

!if !$d(BCC32)
BCC32 = bcc32
!endif

!if !$d(CPP32)
CPP32 = cpp32
!endif

!if !$d(DCC32)
DCC32 = dcc32
!endif

!if !$d(TASM32)
TASM32 = tasm32
!endif

!if !$d(LINKER)
LINKER = ilink32
!endif

!if !$d(BRCC32)
BRCC32 = brcc32
!endif


# ---------------------------------------------------------------------------
!if $d(PATHCPP)
.PATH.CPP = $(PATHCPP)
.PATH.C   = $(PATHCPP)
!endif

!if $d(PATHPAS)
.PATH.PAS = $(PATHPAS)
!endif

!if $d(PATHASM)
.PATH.ASM = $(PATHASM)
!endif

!if $d(PATHRC)
.PATH.RC  = $(PATHRC)
!endif

!if $d(PATHOBJ)
.PATH.OBJ  = $(PATHOBJ)
!endif
# ---------------------------------------------------------------------------
$(PROJECT): $(OTHERFILES) $(IDLGENFILES) $(OBJFILES) $(RESDEPEN) $(DEFFILE)
    $(BCB)\BIN\$(LINKER) @&&!
    $(LFLAGS) -L$(LIBPATH) +
    $(ALLOBJ), +
    $(PROJECT),, +
    $(ALLLIB), +
    $(DEFFILE), +
    $(ALLRES)
!
# ---------------------------------------------------------------------------
.pas.hpp:
    $(BCB)\BIN\$(DCC32) $(PFLAGS) -U$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -O$(INCLUDEPATH) --BCB {$< }

.pas.obj:
    $(BCB)\BIN\$(DCC32) $(PFLAGS) -U$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -O$(INCLUDEPATH) --BCB {$< }

.cpp.obj:
    $(BCB)\BIN\$(BCC32) $(CFLAG1) $(WARNINGS) -I$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -n$(@D) {$< }

.c.obj:
    $(BCB)\BIN\$(BCC32) $(CFLAG1) $(WARNINGS) -I$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -n$(@D) {$< }

.c.i:
    $(BCB)\BIN\$(CPP32) $(CFLAG1) $(WARNINGS) -I$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -n. {$< }

.cpp.i:
    $(BCB)\BIN\$(CPP32) $(CFLAG1) $(WARNINGS) -I$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -n. {$< }

.asm.obj:
    $(BCB)\BIN\$(TASM32) $(AFLAGS) -i$(INCLUDEPATH:;= -i) $(AUSERDEFINES) -d$(SYSDEFINES:;= -d) $<, $@

.rc.res:
    $(BCB)\BIN\$(BRCC32) $(RFLAGS) -I$(INCLUDEPATH) -D$(USERDEFINES);$(SYSDEFINES) -fo$@ $<



# ---------------------------------------------------------------------------





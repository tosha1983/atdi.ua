using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Idwm
{
    static class Idwm
    {
        private const string DLL_NAME = @"idwm32d.dll";

        [DllImport(DLL_NAME)]
        public static extern void ALLMAP(ref int IVAL, [In,Out] float[,] ARRAY, ref int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void AP27AER(ref int IWANT1, ref int IWANT2, ref int ITYP, ref int MAXNR, [In,Out] StringBuilder NAMES, [In,Out] int[] NRCRD, [In,Out] float[] CRDARR, out int NR2, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void AP28RZ(ref float RLON, ref float RLAT, StringBuilder RAINZ);

        [DllImport(DLL_NAME)]
        public static extern void AP30ARZ(ref float RLON, ref float RLAT, StringBuilder RAINZ);

        [DllImport(DLL_NAME)]
        public static extern void AP30RZ(ref float RLON, ref float RLAT, StringBuilder RAINZ);

        [DllImport(DLL_NAME)]
        public static extern void CDGRAD4(StringBuilder COORD, out float LON, out float LAT);

        [DllImport(DLL_NAME)]
        public static extern void CIRAFZL(ref int IZONE, ref int ITYPE, ref int MAXCRD, [In,Out] float[,] CRDARR, out int NRCRD, out float TLON, out float TLAT, out int IREST, out int IER);

        [DllImport(DLL_NAME)]
        public static extern void CIRAFZP(ref float RLON, ref float RLAT, out int IZONE, out int IER);

        [DllImport(DLL_NAME)]
        public static extern void CNDMAP(ref int IVAL, [In,Out] float[,] ARRAY, ref int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void CRADDG4(ref float LON, ref float LAT, StringBuilder COORD);

        [DllImport(DLL_NAME)]
        public static extern void CRDCHK1(StringBuilder COORD, out bool VALID);

        [DllImport(DLL_NAME)]
        public static extern void CRDCHK2(StringBuilder COORD, out bool VALID);

        [DllImport(DLL_NAME)]
        public static extern void CRDUNPN(StringBuilder STRING, out float LON, out float LAT, out bool VALID, ref int N);

        [DllImport(DLL_NAME)]
        public static extern void GE75NZ(ref float RLON, ref float RLAT, out int INZ);

        [DllImport(DLL_NAME)]
        public static extern void GEO14SQ(ref int N, ref float LON, ref float LAT, ref float RADIUS, [In,Out] float[,] WND );

        [DllImport(DLL_NAME)]
        public static extern void GEOAAC2(ref float RLON, ref float RLAT, ref int IRANGE, ref int IAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] int[] IDIST, [In,Out] StringBuilder ALLVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOAAC3(ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] int[] IDIST, [In,Out] StringBuilder ALLVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOAAC8(StringBuilder ALLOT, ref float RLON, ref float RLAT, ref int IRANGE, ref int IAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] int[] IDIST, [In,Out] StringBuilder ALLVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOAAC9(StringBuilder ALLOT, ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] float[] RDIST, [In,Out] StringBuilder ALLVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOAAL(ref int ITYPE, StringBuilder ALLOT, [In,Out] float CRDARR, ref int MAXCRD, out int LINEID, out int NRCRD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOACW(ref float RLONLL, ref float RLATLL, ref float RLONUR, ref float RLATUR, ref int MAXCTY, [In,Out] StringBuilder CTYCOD, out int NOCTY, out bool ALL);

        [DllImport(DLL_NAME)]
        public static extern void GEOADP(ref float LON1, ref float LAT1, ref float LON2, ref float LAT2, ref int NRCRD, [In,Out] float CRDARR );

        [DllImport(DLL_NAME)]
        public static extern void GEOALC(ref float RLON, ref float RLAT, ref int IRANGE, ref int IAZIM, ref int IWC, ref float MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] int[] IDIST, [In,Out] StringBuilder CTYVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOALC3(ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int IWC, ref float MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] float[] RDIST, [In,Out] StringBuilder CTYVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOAXP (ref int IHOW, ref float DSTMN, ref int MXCRD, [In,Out] float[,] CRDARR, out int NRCRD );

        [DllImport(DLL_NAME)]
        public static extern void GEOC1L([In,Out] float[,] CRDLIN, ref int NRPNT, ref int IENDP, ref int MAXCROS, [In,Out] int[,] IPOSV2, [In,Out] float[,] CROSVEK, out int NCROSS, out bool ALL);

        [DllImport(DLL_NAME)]
        public static extern void GEOC2L([In,Out] float[,] CRDLN1, ref int NRPNT1, [In,Out] float[,] CRDLN2, ref int NRPNT2, ref int C180, ref int IEND, [In,Out] float[,] CROSVEK, ref int MAXCROS, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOC2LT([In,Out] float[,] CRDLN1, ref int NRPNT1, [In,Out] float[,] CRDLN2, ref int NRPNT2, ref int C180, ref int IEND, [In,Out] float[,] CROSVEK, ref int MAXCROS, out int CROSS, out int IREST, [In,Out] int[,] IPOSV2);

        [DllImport(DLL_NAME)]
        public static extern void GEOCAC2(ref float RLON, ref float RLAT, ref int IRANGE, ref int IAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] int[] IDIST, [In,Out] int[] CNDVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOCAC3(ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] float[] RDIST, [In,Out] int[] CNDVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOCCD(StringBuilder CTY1, StringBuilder CTY2, out float LON1, out float LAT1, out float LON2, out float LAT2, out int KM);

        [DllImport(DLL_NAME)]
        public static extern void GEOCCDC(StringBuilder CTYCOD, ref int MAXCTY, out int NOCTY, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOCCDI(ref int UNR);

        [DllImport(DLL_NAME)]
        public static extern void GEOCFD(out int N);

        [DllImport(DLL_NAME)]
        public static extern void GEOCIA([In,Out] float[,]CRDARR, ref int NRPNT, ref int IGCTY, StringBuilder IGCTYC, ref int MAXCTY, out int NOCTY, StringBuilder CTYCOD, out int IREST); 

        [DllImport(DLL_NAME)]
        public static extern void GEOCIEL(ref float EARTHR, ref float ORBRAD, ref float SATLON, ref float BLON, ref float BLAT, ref float RMAJ, ref float RMIN, ref float ORI, ref int MAXCTY, out int NOCTY, StringBuilder CTYCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOCIW(ref int ITYPE, [In,Out] float[,] CRDARR, ref int MAXCRD, out int NRCRD, StringBuilder ION, out bool NOMORE);

        [DllImport(DLL_NAME)]
        public static extern void GEOCLPS(ref float XKM, [In,Out] float[,] CRDLIN, ref int NRPNT, ref int MXNEAR, [In,Out] int[] IPOSV, [In,Out] float[] DISTV, out int NRNEAR, out bool ALL);

        [DllImport(DLL_NAME)]
        public static extern void GEOCRDN(ref int LUNCCD, ref int SORDER, StringBuilder CTY, ref int IDIST, ref int MAXCTY, StringBuilder CTYCOD,  [In,Out] int[] KMCTY, out int NOCTY, out int IREST );

        [DllImport(DLL_NAME)]
        public static extern void GEOCTYA(StringBuilder CTY, StringBuilder ADM);

        [DllImport(DLL_NAME)]
        public static extern void GEOCTYR(StringBuilder CTY, StringBuilder REGION);

        [DllImport(DLL_NAME)]
        public static extern void GEOCWI(ref int NRCTY, StringBuilder CTYVEK, out float RLONLL, out float RLATLL, out float RLONUR, out float RLATUR);

        [DllImport(DLL_NAME)]
        public static extern void GEODAA(ref float RLON, ref float RLAT, StringBuilder ALLOT, ref int IRANGE, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODAA1(ref float RLON, ref float RLAT, StringBuilder ALLOT, ref float RANGE, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODAA2(ref float RLON, ref float RLAT, StringBuilder ALLOT, ref int IRANGE, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODAA3(ref float RLON, ref float RLAT, StringBuilder ALLOT, ref float RANGE, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODAM(ref float RLON, ref float RLAT, ref int IRANGE, ref int IGALLO, StringBuilder IGALLC, ref int MAXALL, StringBuilder ALLPNT, out int NRALL, [In,Out] int[] IDIST, [In,Out] int[] IAZIM, StringBuilder ALLCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEODAM1(ref float RLON, ref float RLAT, ref float RANGE, ref int IGALLO, StringBuilder IGALLC, ref int MAXALL, StringBuilder ALLPNT, out int NOALL, [In,Out] float[] RDIST, [In,Out] float[] RAZIM, StringBuilder ALLCOD, out int IREST);
        
        [DllImport(DLL_NAME)]
        public static extern void GEODAM2(ref float RLON, ref float RLAT, ref int IRANGE, ref int IGALLO, StringBuilder IGALLC, ref int MAXALL, StringBuilder ALLPNT, out int NOALL, [In,Out] int[] IDIST, [In,Out] int[] IAZIM, StringBuilder ALLCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEODAM3(ref float RLON, ref float RLAT, ref float RANGE, ref int IGALLO, StringBuilder IGALLC, ref int MAXALL, StringBuilder ALLPNT, out int NOALL, [In,Out] float[] RDIST, [In,Out] float[] RAZIM, StringBuilder ALLCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEODBLP([In,Out] float[,] CRDLIN, ref int NRPNT, ref int MXDBLP, [In,Out] int[] IPOSV, out int NRDBLP, out bool ALL);

        [DllImport(DLL_NAME)]
        public static extern void GEODC(ref float RLON, ref float RLAT, StringBuilder CTY, ref int IRANGE, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODC1(ref float RLON, ref float RLAT, StringBuilder CTY, ref float RANGE, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODC9(ref float RLON, ref float RLAT, StringBuilder CTY, ref float RANGE, out float RDIST, out float RAZIM);
        
        [DllImport(DLL_NAME)]
        public static extern void GEODCC(ref float RLON, ref float RLAT, ref int IRANGE, ref int IGCTY, StringBuilder IGCTYC, out float CTYLON, out float CTYLAT, out int IDIST, out int IAZIM, StringBuilder CTYCOD);

        [DllImport(DLL_NAME)]
        public static extern void GEODCC1(ref float RLON, ref float RLAT, ref float RANGE, ref int IGCTY, StringBuilder IGCTYC, out float CTYLON, out float CTYLAT, out float RDIST, out float RAZIM, StringBuilder CTYCOD);

        [DllImport(DLL_NAME)]
        public static extern void GEODCM(ref float RLON, ref float RLAT, ref int IRANGE, ref int IGCTY, StringBuilder IGCTYC, ref int MAXCTY, out int  NOCTY, [In,Out] int[] IDIST, [In,Out] int[] IAZIM, StringBuilder CTYCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEODCM1(ref float RLON, ref float RLAT, ref float RANGE, ref int IGCTY, StringBuilder IGCTYC, ref int MAXCTY, out int  NOCTY, [In,Out] float[] RDIST, [In,Out] float[] RAZIM, StringBuilder CTYCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEODCM9(ref float RLON, ref float RLAT, ref float RANGE, ref int IGCTY, StringBuilder IGCTYC, ref int MAXCTY, out int  NOCTY, [In,Out] float[] RDIST, [In,Out] float[] RAZIM, StringBuilder CTYCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEODCW(ref float RLONLL, ref float RLATLL, ref float RLONUR, ref float RLATUR, ref float DENS, StringBuilder CTY);

        [DllImport(DLL_NAME)]
        public static extern void GEODGRD(StringBuilder COORD, out float LON, out float LAT);

        [DllImport(DLL_NAME)]
        public static extern void GEODST(ref float RLON1, ref float RLAT1, ref float RLON2, ref float RLAT2, out int IDIST);

        [DllImport(DLL_NAME)]
        public static extern float GEODST3(ref float RLON1, ref float RLAT1, ref float RLON2, ref float RLAT2);

        [DllImport(DLL_NAME)]
        public static extern void GEODSTR(ref float RLON1, ref float RLAT1, ref float RLON2, ref float RLAT2, out float RDIST);

        [DllImport(DLL_NAME)]
        public static extern void GEODTA(ref float RLON, ref float RLAT, ref int NRPNT, [In,Out] float[,] CRDARR, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODTA1(ref float RLON, ref float RLAT, [In,Out] float[,] WND, ref int NRPNT, [In,Out] float[,] CRDARR, out float RLONNR, out float RLATNR, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODTA3(ref float RLON, ref float RLAT, [In,Out] float[,] WND, ref int NRPNT, [In,Out] float[,] CRDARR, out float RLONNR, out float RLATNR, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODTL(ref float RLON, ref float RLAT, [In,Out] float[,] CRDARR, ref int NRPNT, out float RLONNR, out float RLATNR, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODW(ref float RLON, ref float RLAT, ref int IRANGE, StringBuilder CTY, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODW1(ref float RLON, ref float RLAT, ref float RANGE, StringBuilder CTY, out int RDIST, out int RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEODWA(ref float RLON, ref float RLAT, ref int IAZIM, ref int IRANGE, ref int IVAL, StringBuilder CTY, out int IDIST);

        [DllImport(DLL_NAME)]
        public static extern void GEODWA1(ref float RLON,ref float RLAT, ref float RAZIM, ref float RANGE, ref int IVAL, StringBuilder CTY, out float RDIST);

        [DllImport(DLL_NAME)]
        public static extern void GEODWA2(ref float RLON, ref float RLAT, ref int IAZIM, ref int IRANGE, ref int IVAL, StringBuilder CTY, out int IDIST);

        [DllImport(DLL_NAME)]
        public static extern void GEODWA3(ref float RLON, ref float RLAT, ref float RAZIM, ref float RANGE, ref int IVAL, StringBuilder CTY, out float RDIST);

        [DllImport(DLL_NAME)]
        public static extern void GEODWD(ref float RLONLL, ref float RLATLL, ref float RLONUR, ref float RLATUR, ref float DENS, ref int NRCTY, StringBuilder CTYVEK);

        [DllImport(DLL_NAME)]
        public static extern void GEOEEL(ref float SATLON, ref float ELEV, ref int MAXCTY, out int NOCTY,
            StringBuilder CTYCOD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOELEV(ref float EARTHR, ref float ORBRAD, ref float SATLON, ref float ELEV, ref int MAXCRD, [In,Out] float[,] CRDARR, out int NRCRD);

        [DllImport(DLL_NAME)]
        public static extern void GEOELLP(ref float EARTHR, ref float ORBRAD, ref float SATLON, ref float BLON, ref float BLAT, ref float RMAJ, ref float RMIN, ref float ORI, ref int MAXCRD, [In,Out] float[,] CRDARR, [In,Out] int[] IFLGV, out int NRCRD, out int NGAP);

        [DllImport(DLL_NAME)]
        public static extern void GEOEMA (ref float RLON, ref float RLAT, out int IEMA );

        [DllImport(DLL_NAME)]
        public static extern void GEOGCC3(ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, [In,Out] float[] RDIST, [In,Out] float[] CNDVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void GEOGCM(ref int IVAL, [In,Out] float[,] ARRAY, ref int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void GEOGCML(ref int ITYPE, [In,Out] float[,] WND, ref int MAXCRD, [In,Out] float[,] CRDARR, out int NRCRD, out int IREST, out float CNDV1, out float CNDV2, out bool NOMORE);

        [DllImport(DLL_NAME)]
        public static extern void GEOGCMS(ref int N, out int IER);

        [DllImport(DLL_NAME)]
        public static extern void GEOGCP(ref float RLON, ref float RLAT, out float GRCOND);

        [DllImport(DLL_NAME)]
        public static extern void GEOGCP2(ref float RLON, ref float RLAT, out float GRCOND);

        [DllImport(DLL_NAME)]
        public static extern void GEOGETF(ref int MXLEN, StringBuilder FNAME, out bool FOUND);

        [DllImport(DLL_NAME)]
        public static extern void GEOIAA(ref float RLON, ref float RLAT, StringBuilder ALLOT);

        [DllImport(DLL_NAME)]
        public static extern void GEOIAA2(ref float RLON, ref float RLAT, StringBuilder ALLOT);

        [DllImport(DLL_NAME)]
        public static extern void GEOINM(ref float RLON, ref float RLAT, out int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void GEOLDI(ref int ISW, ref float DMN, [In,Out] float[,] CRDARR, ref int NRPNT, out float DMI, out float DAV, out float DMX, out int IPOS);

        [DllImport(DLL_NAME)]
        public static extern void GEOLIW(ref int ITYPE, [In,Out] float[,] CRDARR, ref int MAXCRD, out int LINE, out int NRCRD, StringBuilder BCILR, out bool NOMORE);

        [DllImport(DLL_NAME)]
        public static extern void GEOMAP(ref int IVAL, [In,Out] float[,] ARRAY, ref int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void GEOMNYP([In,Out] float[,] CRDLIN,ref int NRPNT, ref int MXREP, [In,Out] int[,] IPOSV2, out int NRREP, out bool ALL);

        [DllImport(DLL_NAME)]
        public static extern void GEOPDAP(ref float RLON1, ref float RLAT1, ref float RDIST, ref float RAZIM, out float RLON2, out float RLAT2);

        [DllImport(DLL_NAME)]
        public static extern bool GEOPIA([In,Out] float[,] POINT,[In,Out] float[,] WND, [In,Out] float[,] CRDARR, ref int NRPNT);

        [DllImport(DLL_NAME)]
        public static extern bool GEOPIA2([In,Out] float[,] POINT, [In,Out] float[,] CRDARR, ref int NRPNT);

        [DllImport(DLL_NAME)]
        public static extern bool GEOPIM(ref float LON, ref float LAT);

        [DllImport(DLL_NAME)]
        public static extern void GEOPINF(StringBuilder PLAN, StringBuilder CTY, out int NN);

        [DllImport(DLL_NAME)]
        public static extern bool GEOPIW([In,Out] float[,] CRDARR, ref int NRPNT, [In,Out] float[,] WND);

        [DllImport(DLL_NAME)]
        public static extern void GEOPIW1([In,Out] float[,] CRDARR, ref int NRPNT, [In,Out] float[,] WND, out int ISEQ);

        [DllImport(DLL_NAME)]
        public static extern void GEOPIW2([In,Out] float[,] CRDARR, ref int NRPNT, [In,Out] float[,] WND, out int ISEQ);

        [DllImport(DLL_NAME)]
        public static extern void GEOPLC(ref float RLON, ref float RLAT, StringBuilder CTY);

        [DllImport(DLL_NAME)]
        public static extern void GEOPLC2(ref float RLON, ref float RLAT, StringBuilder CTY);

        [DllImport(DLL_NAME)]
        public static extern void GEOPPDA(ref float RLON1, ref float RLAT1, ref float RLON2, ref float RLAT2, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void GEOPRCP(ref float RLON1, ref float RLAT1, ref float RLON2, ref float RLAT2, out float RDIST, out int IZONES, StringBuilder PRCODVEK, [In,Out] float[] PRDVEK, [In,Out] float[] RATIOVEK, out int IER);

        [DllImport(DLL_NAME)]
        public static extern void GEOPRM(ref int IVAL, [In,Out] float[,] ARRAY, ref int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void GEOPRML(ref int ITYPE, [In,Out] float[,] WND, ref int MAXCRD, [In,Out] float[,] CRDARR, out int NRCRD, out int IREST, out bool NOMORE);

        [DllImport(DLL_NAME)]
        public static extern void GEOPRMS(ref int N, out int IER);

        [DllImport(DLL_NAME)]
        public static extern void GEOPRP(ref float RLON, ref float RLAT, StringBuilder PRCODE);

        [DllImport(DLL_NAME)]
        public static extern void GEOPRP2(ref float RLON, ref float RLAT, StringBuilder PRCODE);

        [DllImport(DLL_NAME)]
        public static extern void GEOR837(ref float RLON, ref float RLAT, StringBuilder RAINZ, [In,Out] float[] RPROB);

        [DllImport(DLL_NAME)]
        public static extern void GEORDDG(ref float LON, ref float LAT, StringBuilder COORD);

        [DllImport(DLL_NAME)]
        public static extern void GEORDXX(ref int ITYPE, [In,Out] float[,] CRDARR, ref int MAXCRD);

        [DllImport(DLL_NAME)]
        public static extern void GEORGN(ref float RLON, ref float RLAT, out int IRGN);

        [DllImport(DLL_NAME)]
        public static extern void GEOSET(ref int IOPT, ref int N);

        [DllImport(DLL_NAME)]
        public static extern void GEOSMLA(ref int MINALF, [In,Out] float[,] CRDLIN, ref int NRPNT, ref int MXSMLA, [In,Out] int[] IPOSV, out int NRSMLA, out bool ALL);

        [DllImport(DLL_NAME)]
        public static extern void GEOSQR(ref float LON, ref float LAT, ref float RADIUS, [In,Out] float[,] WND);

        [DllImport(DLL_NAME)]
        public static extern void GEOTLC(StringBuilder STRING);

        [DllImport(DLL_NAME)]
        public static extern void GEOTLCN(StringBuilder STRING, ref int N);

        [DllImport(DLL_NAME)]
        public static extern void GEOTUC(StringBuilder STRING);

        [DllImport(DLL_NAME)]
        public static extern void GEOTUCN(StringBuilder STRING, ref int N);

        [DllImport(DLL_NAME)]
        public static extern void GEOVA1(ref int IVAL, out int IRES);

        [DllImport(DLL_NAME)]
        public static extern void GEOVA2(ref int N, [In,Out] float[,] PNTCRD);

        [DllImport(DLL_NAME)]
        public static extern bool GEOWIW([In,Out] float[,] WND1, [In,Out] float[,] WND2); 

        [DllImport(DLL_NAME)]
        public static extern void GEOWND([In,Out] float[,] CRDARR, ref int NRPNT, [In,Out] float[,]  WND);

        [DllImport(DLL_NAME)]
        public static extern bool GEOWTIW([In,Out] float[,] WND1, [In,Out] float[,] WND2);

        [DllImport(DLL_NAME)]
        public static extern void IDWM(out int N);

        [DllImport(DLL_NAME)]
        public static extern void LCLAAC2(ref float RLON, ref float RLAT, ref int IRANGE, ref int IAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, out int IDIST, StringBuilder AREAVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLAAC3(ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, out int RDIST, StringBuilder AREAVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLAAC8(StringBuilder AREACOD, ref float RLON, ref float RLAT, ref int IRANGE, ref int IAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, out int IDIST, StringBuilder AREAVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLAAC9(StringBuilder AREACOD, ref float RLON, ref float RLAT, ref float RANGE, ref float RAZIM, ref int MAXCROS, [In,Out] float[,] CROSVEK, out float RDIST, StringBuilder AREAVEK, out int CROSS, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLDAM(ref float RLON, ref float RLAT, ref int IRANGE, ref int IGAREAO, StringBuilder IGAREAC, ref int MAXAREA, StringBuilder AREAPNT, out int NRAREA, [In,Out] int[] IDIST, [In,Out] int[] IAZIM, StringBuilder AREAVEK, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLDAM1(ref float RLON, ref float RLAT, ref float RANGE, ref int IGAREAO, StringBuilder IGAREAC, ref int MAXAREA, StringBuilder AREAPNT, out int NRAREA, [In,Out] float[] RDIST, [In,Out] float[] RAZIM, StringBuilder AREAVEK, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLDAM2(ref float RLON, ref float RLAT, ref int IRANGE, ref int IGAREAO, StringBuilder IGAREAC, ref int MAXAREA, StringBuilder AREAPNT, out int NRAREA, [In,Out] int[] IDIST, [In,Out] int[] IAZIM, StringBuilder AREAVEK, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLDAM3(ref float RLON, ref float RLAT, ref float RANGE, ref int IGAREAO, StringBuilder IGAREAC, ref int MAXAREA, StringBuilder AREAPNT, out int NRAREA, [In,Out] float[] RDIST, [In,Out] float[] RAZIM, StringBuilder AREAVEK, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLDLA(ref float RLON, ref float RLAT, StringBuilder AREACOD, ref int IRANGE, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void LCLDLA1(ref float RLON, ref float RLAT, StringBuilder AREACOD, ref float RANGE, out float RDIST, out float RAZIM);

        [DllImport(DLL_NAME)]
        public static extern void LCLDLA2(ref float RLON, ref float RLAT, StringBuilder AREACOD, ref int IRANGE, out int IDIST, out int IAZIM);

        [DllImport(DLL_NAME)]
        public static extern void LCLDLA3(ref float RLON, ref float RLAT, StringBuilder AREACOD, ref float RANGE, out float RDIST, out float RAZIM);
        
        [DllImport(DLL_NAME)]
        public static extern void LCLGAL(ref int ITYPE, StringBuilder AREACOD, [In,Out] float[,] CRDARR, ref int MAXCRD, out int LINEID, out int NRCRD, out int IREST);

        [DllImport(DLL_NAME)]
        public static extern void LCLILA(ref float RLON, ref float RLAT, StringBuilder AREACOD);

        [DllImport(DLL_NAME)]
        public static extern void LCLILA2(ref float RLON, ref float RLAT, StringBuilder AREACOD);

        [DllImport(DLL_NAME)]
        public static extern void LCLLIN(ref int ITYPE, [In,Out] float[,] CRDARR, ref int MAXCRD, ref int LINE, out int NRCRD, out bool FOUND);

        [DllImport(DLL_NAME)]
        public static extern void LCLMAP(ref int IVAL, [In,Out] float[,] ARRAY, ref int IRANGE);

        [DllImport(DLL_NAME)]
        public static extern void LCLNXT(ref int ITYPE,  [In,Out] float[,]CRDARR, ref int MAXCRD, out int LINE, out int NRCRD,  out bool NOMORE);

        [DllImport(DLL_NAME)]
        public static extern void RJ81NZ(ref float RLON, ref float RLAT, out int INZ);

        [DllImport(DLL_NAME)]
        public static extern void RJ88NZ(ref float RLON, ref float RLAT, StringBuilder CTYALL, out int INZ);

        [DllImport(DLL_NAME)]
        public static extern void WDBUNIT(ref int UNR);
    }
}

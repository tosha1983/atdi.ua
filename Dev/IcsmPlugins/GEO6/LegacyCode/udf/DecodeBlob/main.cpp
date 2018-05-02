#include <windows.h>
#include <stdlib.h>
#if TIME_WITH_SYS_TIME
# include <sys/time.h>
# include <time.h>
#else
# if HAVE_SYS_TIME_H
#  include <sys/time.h>
# else
#  include <time.h>
# endif
#endif
#include <string.h>
//#include <math.h>
#include "ibase.h"
//#include <SysUtils.hpp>
#include <strutils.hpp>

#define BADVAL -9999L
#define MYBUF_LEN 15		/* number of chars to get for */

#include <sstream>
#include <iomanip>
using namespace std;

//#include "main.h"

extern "C" __declspec(dllexport) char* blob_as_str(BLOBCALLBACK blob, long* mode);
extern "C" __declspec(dllexport) double* get_blob_item(BLOBCALLBACK blob, long* index);
extern "C" __declspec(dllexport) double* _get_max_blob_item(BLOBCALLBACK blob);
extern "C" __declspec(dllexport) double* _get_normalized_blob_item(BLOBCALLBACK blob, long* index);
extern "C" __declspec(dllexport) const char* _get_format_string(BLOBCALLBACK blob);


//---------------------------------------------------------------------------
int WINAPI DllEntryPoint(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    return 1;
}
//---------------------------------------------------------------------------
extern "C" __declspec(dllexport)
char*  blob_as_str(BLOBCALLBACK blob, long* mode)
{
	char *buf, *cur_pos;

	unsigned short length, actual_length;
    unsigned short cur_length = 0;

	if (!blob || !blob->blob_handle)
    {
		return NULL;
    }
	//length = blob->blob_max_segment + 1L;
	length = blob->blob_total_length + 1L;
	cur_pos = buf = (char*)malloc(length);

    /*
    if (*m > *n || *m < 1L || *n < 1L)
		return "";
    */
	//if (blob->blob_total_length < (long)*m)
	//	return "";

	//begin = *m;				/* beginning position */

	//if (blob->blob_total_length < (long)*n)
	//	end = blob->blob_total_length;	/* ending position */
	//else
	//	end = *n;

	/* Limit the return string to 255 bytes */
	//if (end - begin + 1L > 255L)
	//	end = begin + 254L;
	//q = buffer;

    while ((*blob->blob_get_segment) (blob->blob_handle, cur_pos, length, &actual_length))
    {
        cur_pos += actual_length;
        cur_length += actual_length;
		buf[cur_length] = '\0';
    }

    int prec = (mode != NULL) ? *mode : 0;
    if (prec <= 0) prec = 0;
    
    double* dbuff = (double*)buf;
    stringstream oss;
    for (unsigned int i = 0; i < cur_length / sizeof(double); i++)
    {
        oss << " " << fixed << setprecision(prec) << dbuff[i];
    }

	free(buf);

    string str = oss.str();
    unsigned short resbuflen = str.size();
    if (resbuflen > 0)
    {
        char* resbuf = (char*)malloc(resbuflen + 2);
        strncpy(resbuf + 2, str.data(), resbuflen);
        unsigned short* size_placeholder = (unsigned short*)resbuf;
        *size_placeholder = resbuflen;
        return resbuf;
    } else {
        return NULL;
    }
}

//---------------------------------------------------------------------------
extern "C" __declspec(dllexport)
double* _get_blob_item(BLOBCALLBACK blob, long* index)
{
	char *buf, *cur_pos;

	unsigned short length, actual_length;
    unsigned short cur_length = 0;

	if (!blob || !blob->blob_handle)
    {
		return NULL;
    }
	//length = blob->blob_max_segment + 1L;
	length = blob->blob_total_length + 1L;
	cur_pos = buf = (char*)malloc(length);

    /*
    if (*m > *n || *m < 1L || *n < 1L)
		return "";
    */
	//if (blob->blob_total_length < (long)*m)
	//	return "";

	//begin = *m;				/* beginning position */

	//if (blob->blob_total_length < (long)*n)
	//	end = blob->blob_total_length;	/* ending position */
	//else
	//	end = *n;

	/* Limit the return string to 255 bytes */
	//if (end - begin + 1L > 255L)
	//	end = begin + 254L;
	//q = buffer;

    while ((*blob->blob_get_segment) (blob->blob_handle, cur_pos, length, &actual_length))
    {
        cur_pos += actual_length;
        cur_length += actual_length;
		buf[cur_length] = '\0';
    }

    int prec = 0;

    double* dbuff = (double*)buf;
    stringstream oss;
    for (unsigned int i = 0; i < cur_length / sizeof(double); i++)
    {
        oss << " " << fixed << setprecision(prec) << dbuff[i];
    }
    string str = oss.str();
    unsigned short resbuflen = str.size();
    if (resbuflen > 0)
    {
        double res = dbuff[*index];
        free(buf);
        return &res;
    } else {
        free(buf);
        return NULL;
    }
}
//---------------------------------------------------------------------------
extern "C" __declspec(dllexport)
double* _get_normalized_blob_item(BLOBCALLBACK blob, long* index)
{
	char *buf, *cur_pos;

	unsigned short length, actual_length;
    unsigned short cur_length = 0;

	if (!blob || !blob->blob_handle)
    {
		return NULL;
    }
	//length = blob->blob_max_segment + 1L;
	length = blob->blob_total_length + 1L;
	cur_pos = buf = (char*)malloc(length);

    /*
    if (*m > *n || *m < 1L || *n < 1L)
		return "";
    */
	//if (blob->blob_total_length < (long)*m)
	//	return "";

	//begin = *m;				/* beginning position */

	//if (blob->blob_total_length < (long)*n)
	//	end = blob->blob_total_length;	/* ending position */
	//else
	//	end = *n;

	/* Limit the return string to 255 bytes */
	//if (end - begin + 1L > 255L)
	//	end = begin + 254L;
	//q = buffer;

    while ((*blob->blob_get_segment) (blob->blob_handle, cur_pos, length, &actual_length))
    {
        cur_pos += actual_length;
        cur_length += actual_length;
		buf[cur_length] = '\0';
    }

    int prec = 0;

    double* dbuff = (double*)buf;
    stringstream oss;
    unsigned int i = 0;
    for ( ;i < cur_length / sizeof(double); i++)
    {
        oss << " " << fixed << setprecision(prec) << dbuff[i];
    }

    string str = oss.str();
    unsigned short resbuflen = str.size();
    if (resbuflen > 0)
    {
        double max = dbuff[0];
        for(unsigned int j = 1; j < i; ++j)
        {
            if(dbuff[j] > max)
                max = dbuff[j];
        }
        double res = max - dbuff[*index];//NULL;  free(buf);
        //*res =
        return &res;
    } else {
        free(buf);
        return NULL;
    }
}

//---------------------------------------------------------------------------
extern "C" double* __declspec(dllexport)
 _get_max_blob_item(BLOBCALLBACK blob)
{
	char *buf, *cur_pos;

	unsigned short length, actual_length;
    unsigned short cur_length = 0;

	if (!blob || !blob->blob_handle)
    {
		return NULL;
    }
	//length = blob->blob_max_segment + 1L;
	length = blob->blob_total_length + 1L;
	cur_pos = buf = (char*)malloc(length);

    /*
    if (*m > *n || *m < 1L || *n < 1L)
		return "";
    */
	//if (blob->blob_total_length < (long)*m)
	//	return "";

	//begin = *m;				/* beginning position */

	//if (blob->blob_total_length < (long)*n)
	//	end = blob->blob_total_length;	/* ending position */
	//else
	//	end = *n;

	/* Limit the return string to 255 bytes */
	//if (end - begin + 1L > 255L)
	//	end = begin + 254L;
	//q = buffer;

    while ((*blob->blob_get_segment) (blob->blob_handle, cur_pos, length, &actual_length))
    {
        cur_pos += actual_length;
        cur_length += actual_length;
		buf[cur_length] = '\0';
    }

    int prec = 0;

    double* dbuff = (double*)buf;
    stringstream oss;
    unsigned int i = 0;
    for ( ;i < cur_length / sizeof(double); i++)
    {
        oss << " " << fixed << setprecision(prec) << dbuff[i];
    }
    
    string str = oss.str();
    unsigned short resbuflen = str.size();
    if (resbuflen > 0)
    {
        double max = dbuff[0];
        for(unsigned int j = 1; j < i; ++j)
        {
            if(dbuff[j] > max)
                max = dbuff[j];
        }
        free(buf);
        return &max;
    } else {
        free(buf);
        return NULL;
    }


}

//---------------------------------------------------------------------------

extern "C" __declspec(dllexport) 
const char*  _get_format_string(BLOBCALLBACK blob)
{
  	char *buf, *cur_pos;

	unsigned short length, actual_length;
    unsigned short cur_length = 0;

	if (!blob || !blob->blob_handle)
    {
		return NULL;
    }
	length = blob->blob_total_length + 1L;
	cur_pos = buf = (char*)malloc(length);

    while ((*blob->blob_get_segment) (blob->blob_handle, cur_pos, length, &actual_length))
    {
        cur_pos += actual_length;
        cur_length += actual_length;
		buf[cur_length] = '\0';
    }

    int prec = 0;
    double* dbuff = (double*)buf;
    stringstream oss;
    unsigned int i = 0;
    for (; i < cur_length / sizeof(double); i++)
    {
        oss << " " << fixed << setprecision(prec) << dbuff[i];
    }

   //	free(buf);

    string str = oss.str();
    unsigned short resbuflen = str.size();
   if (resbuflen > 0)
    {
        double max = dbuff[0];
        string resstr = "";
        for(unsigned int j = 1; j < i; ++j)
        {
            if(dbuff[j] > max)
                max = dbuff[j];
        }
         for(unsigned int j = 0; j < i; ++j)
        {
            double fl = max - dbuff[j];
            char temp[10];
            sprintf(temp, "%.3f ", fl);
            resstr += temp;
        }
    /*    resbuflen = resstr.size();
        char* resbuf = (char*)malloc(resbuflen);
        strncpy(resbuf, resstr.data(), resbuflen);
        unsigned short* size_placeholder = (unsigned short*)resbuf;
        *size_placeholder = resbuflen;     */
        free(buf);
    //    return resbuf;
        return resstr.c_str();
    } else {
        free(buf);
        return NULL;
    }         
}
//---------------------------------------------------------------------------


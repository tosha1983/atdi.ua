#include "stdafx.h"
#include <string.h>
#include <math.h>
#include<TCHAR.H>
#include <atldbcli.h>
#include <atlstr.h>  


namespace ManagedNetKey {
	typedef int(*funcptrnetkey)(char* softname, char* exedate);
	public  ref class  NetKey
	{
	public:
		int GetToken(char* softname, char* exedate);
		~NetKey();
	};
	

	int NetKey::GetToken(char* softname, char* exedate)
	{
		int value = 0;
		HINSTANCE hGetProcIDDLL = LoadLibrary(_T("netkey.dll"));
		if (hGetProcIDDLL != NULL) {
			FARPROC pnetkey = GetProcAddress(hGetProcIDDLL, "GetToken");
			if (pnetkey != 0)
				value = ((funcptrnetkey)pnetkey)(softname, exedate);
			
		}
		FreeLibrary(hGetProcIDDLL);
		return value;
	}

	NetKey::~NetKey()
	{
		
	}

}
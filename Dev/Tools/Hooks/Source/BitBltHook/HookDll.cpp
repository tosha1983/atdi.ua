//Хук для функции BitBlt
#include <Windows.h>
#define SIZE 6
typedef BOOL(WINAPI *pBitBlt) (HDC, UINT, UINT, UINT,UINT,HDC,UINT,UINT,DWORD dwRop);
BOOL WINAPI HookBitBlt(HDC, UINT, UINT, UINT, UINT, HDC, UINT, UINT, DWORD dwRop);
void BeginRedirect(LPVOID);

pBitBlt pOrigBitBltAddress = NULL;
BYTE oldBytes[SIZE] = { 0 };
BYTE JMP[SIZE] = { 0 };
DWORD oldProtect, myProtect = PAGE_EXECUTE_READWRITE;
BOOL APIENTRY DllMain(HMODULE hModule,	DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		pOrigBitBltAddress = (pBitBlt)GetProcAddress(GetModuleHandleW(L"gdi32.dll"), "BitBlt");
		if (pOrigBitBltAddress != NULL)
		{
			BeginRedirect(HookBitBlt);
		}
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}


void BeginRedirect(LPVOID newFunction)
{
	BYTE tempJMP[SIZE] = { 0xE9, 0x90, 0x90, 0x90, 0x90, 0xC3 };
	memcpy(JMP, tempJMP, SIZE);
	DWORD JMPSize = ((DWORD)newFunction - (DWORD)pOrigBitBltAddress - 5);
	VirtualProtect((LPVOID)pOrigBitBltAddress, SIZE, PAGE_EXECUTE_READWRITE, &oldProtect);
	memcpy(oldBytes, pOrigBitBltAddress, SIZE);
	memcpy(&JMP[1], &JMPSize, 4);
	memcpy(pOrigBitBltAddress, JMP, SIZE);
	VirtualProtect((LPVOID)pOrigBitBltAddress, SIZE, oldProtect, NULL);
}

BOOL WINAPI HookBitBlt(HDC hdcDest,UINT nXDest, UINT nYDest, UINT nWidth, UINT nHeight,HDC hdcSrc, UINT nXSrc,  UINT nYSrc,  DWORD dwRop )
{
	VirtualProtect((LPVOID)pOrigBitBltAddress, SIZE, myProtect, NULL);
	memcpy(pOrigBitBltAddress, oldBytes, SIZE);
	BOOL retValue = true;
	memcpy(pOrigBitBltAddress, JMP, SIZE);
	VirtualProtect((LPVOID)pOrigBitBltAddress, SIZE, oldProtect, NULL);
	return retValue;
}

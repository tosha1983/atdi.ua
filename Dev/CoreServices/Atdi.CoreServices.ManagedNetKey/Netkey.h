
#ifdef __cplusplus
extern "C" {
#endif

typedef struct {
	char softname[20];		
	BOOL IsKeyValid;
} KeyData;


#if defined(DLLCOMPIL)
	#define DLLUSE _declspec(dllexport)
#else
	#define DLLUSE _declspec(dllimport)
#endif

DLLUSE BOOL KeyLogon(KeyData*);
DLLUSE void KeyLogoff();
DLLUSE unsigned short ReadKeyCell(unsigned short);
DLLUSE int UpdateRemoteKey();
DLLUSE void KeyMgmt(long); 
DLLUSE int GetToken(char* softname, char* exedate);
DLLUSE BOOL CheckToken(int Token_Id);
DLLUSE void ReleaseToken(int Token_Id);


#ifdef __cplusplus
}
#endif

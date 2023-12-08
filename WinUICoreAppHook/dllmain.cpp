#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <mutex>
#include <map>

#include "./Detours/include/detours.h"

#if defined(_M_AMD64)
#pragma comment(lib, "./Detours/x64/detours.lib")
#elif defined(_M_ARM64)
#pragma comment(lib, "../Detours/ARM64/detours.lib")
#elif defined(_M_IX86)
#pragma comment(lib, "../Detours/X86/detours.lib")
#endif

std::mutex xamlKeyMtx;
std::map<HKEY, bool> xamlKeyMap;

LSTATUS(APIENTRY* oldRegOpenKeyExW)(HKEY, LPCWSTR, DWORD, REGSAM, PHKEY) = RegOpenKeyExW;
LSTATUS APIENTRY NewRegOpenKeyExW(HKEY hkey, LPCWSTR lpSubKey, DWORD ulOptions, REGSAM samDesired, PHKEY phkResult)
{
	LSTATUS result = oldRegOpenKeyExW(hkey, lpSubKey, ulOptions, samDesired, phkResult);

	if (hkey == HKEY_LOCAL_MACHINE && !_wcsicmp(lpSubKey, L"Software\\Microsoft\\WinUI\\XAML"))
	{
		std::lock_guard<std::mutex> lock(xamlKeyMtx);
		if (result == ERROR_FILE_NOT_FOUND)
		{
			xamlKeyMap.emplace((HKEY)INVALID_HANDLE_VALUE, false);
			*phkResult = (HKEY)INVALID_HANDLE_VALUE;
			result = ERROR_SUCCESS;
		}
		else if (result == ERROR_SUCCESS)
		{
			xamlKeyMap.emplace(*phkResult, true);
		}
	}
	return result;
}

LSTATUS(APIENTRY* oldRegCloseKey)(HKEY hKey) = RegCloseKey;
LSTATUS APIENTRY NewRegCloseKey(HKEY hKey)
{
	bool isXamlKey;
	bool isRealKey = false;
	{
		std::lock_guard<std::mutex> lock(xamlKeyMtx);
		auto pos = xamlKeyMap.find(hKey);
		isXamlKey = pos != xamlKeyMap.end();
		if (isXamlKey)
		{
			isRealKey = pos->second;
			xamlKeyMap.erase(pos);
		}
	}

	LSTATUS result;
	if (isXamlKey)
	{
		if (isRealKey)
		{
			// real key
			result = oldRegCloseKey(hKey);
		}
		else
		{
			// simulated key
			result = ERROR_SUCCESS;
		}
	}
	else
	{
		if (hKey == INVALID_HANDLE_VALUE)
		{
			result = ERROR_INVALID_HANDLE;
		}
		else
		{
			result = oldRegCloseKey(hKey);
		}
	}
	return result;
}

LSTATUS(APIENTRY* oldRegQueryValueExW)(HKEY hKey, LPCWSTR lpValueName, LPDWORD lpReserved, LPDWORD lpType, LPBYTE lpData, LPDWORD lpcbData) = RegQueryValueExW;
LSTATUS APIENTRY NewRegQueryValueExW(HKEY hKey, LPCWSTR lpValueName, LPDWORD lpReserved, LPDWORD lpType, LPBYTE lpData, LPDWORD lpcbData)
{
	LSTATUS result;
	if (lpValueName != NULL && !_wcsicmp(lpValueName, L"EnableUWPWindow"))
	{
		bool isXamlKey;
		bool isRealKey = false;
		{
			std::lock_guard<std::mutex> lock(xamlKeyMtx);
			auto pos = xamlKeyMap.find(hKey);
			isXamlKey = pos != xamlKeyMap.end();
			if (isXamlKey)
			{
				isRealKey = pos->second;
			}
		}

		if (isXamlKey)
		{
			if (isRealKey)
			{
				// real key
				result = oldRegQueryValueExW(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);
				if (result == ERROR_SUCCESS && lpData != NULL)
				{
					*lpData = 1;
				}
				else if (result == ERROR_FILE_NOT_FOUND)
				{
					if (lpData == NULL && lpcbData != NULL)
					{
						*lpcbData = sizeof(DWORD);
						result = ERROR_SUCCESS;
					}
					else if (lpData != NULL && lpcbData != NULL)
					{
						if (*lpcbData >= sizeof(DWORD))
						{
							*lpData = 1;
							result = ERROR_SUCCESS;
						}
						else
						{
							result = ERROR_MORE_DATA;
						}
					}
				}
			}
			else
			{
				// simulated key
				result = ERROR_FILE_NOT_FOUND;
				if (lpData == NULL && lpcbData != NULL)
				{
					*lpcbData = sizeof(DWORD);
					result = ERROR_SUCCESS;
				}
				else if (lpData != NULL && lpcbData != NULL)
				{
					if (*lpcbData >= sizeof(DWORD))
					{
						*lpData = 1;
						result = ERROR_SUCCESS;
					}
					else
					{
						result = ERROR_MORE_DATA;
					}
				}
			}
		}
		else
		{
			result = oldRegQueryValueExW(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);
		}
	}
	else
	{
		result = oldRegQueryValueExW(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);
	}
	return result;
}

void StartHook()
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((PVOID*)&oldRegOpenKeyExW, NewRegOpenKeyExW);
	DetourAttach((PVOID*)&oldRegCloseKey, NewRegCloseKey);
	DetourAttach((PVOID*)&oldRegQueryValueExW, NewRegQueryValueExW);
	DetourTransactionCommit();
}

void EndHook()
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach((PVOID*)&oldRegOpenKeyExW, NewRegOpenKeyExW);
	DetourDetach((PVOID*)&oldRegCloseKey, NewRegCloseKey);
	DetourDetach((PVOID*)&oldRegQueryValueExW, NewRegQueryValueExW);
	DetourTransactionCommit();
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_THREAD_ATTACH:
		StartHook();
		break;
	case DLL_THREAD_DETACH:
		EndHook();
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}
// testSendWindow.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
HWND iResult = NULL;
BOOL __stdcall enumWindow(HWND hwnd, LPARAM args)
{
	HWND f1 = FindWindowEx(hwnd, 0, NULL, (LPCTSTR)args);
	if (f1 == NULL)
		return true;
	else
	{
		iResult = f1;
		return false;
	}
}
HWND MyFindWindowEx(HWND hwnd, LPCTSTR lpszWindow, bool bChild);
int main()
{
	HMODULE   hDllInst = LoadLibrary(_T("ClipboardQQSender.dll"));
	if (hDllInst)
	{
		typedef   VOID(  *SENDFUNC)(HWND);
		SENDFUNC Paste = NULL;   //   youFuntionNameAlias   函数别名 
		Paste = (SENDFUNC)GetProcAddress
			(hDllInst, "Paste");
		//   youFuntionName   在DLL中声明的函数名 
		if (Paste)
		{
			//Paste((HWND)724026);
		}

		
		//PostMessage((HWND)724026, WM_IME_CHAR, (LPARAM)_T('测'), 0);
		//PostMessage((HWND)724026, WM_IME_CHAR, (LPARAM)_T('试'), 0);
		Sleep(200);
		//keybd_event(VK_MENU, 0x38, 0, 0);
		PostMessage((HWND)724026, WM_SYSKEYDOWN, VK_MENU, 0x20380001);
		PostMessage((HWND)724026, WM_SYSKEYDOWN, 0x53, 0x201F0001);
		Sleep(200);
		//PostMessage((HWND)724026, WM_SYSCHAR, 0x73, 0x201F0001);
		PostMessage((HWND)724026, WM_SYSKEYUP, 0x53, 0xC01F0001);
		Sleep(200);
		PostMessage((HWND)724026, WM_KEYUP, VK_MENU, 0xC0380001);
		//keybd_event(VK_MENU, 0x38, KEYEVENTF_KEYUP, 0);
		//SendMessage((HWND)724026, WM_CHAR, 10, 0x011C0001);
		//PostMessage((HWND)724026, WM_SYSKEYUP, 0x53, 0x201F0001);
		//SendMessage((HWND)724026, WM_KEYUP, VK_CONTROL, 0x011D0001);

		/*
		SENDFUNC submit = (SENDFUNC)GetProcAddress
			(hDllInst, "Sumbit");
		if (submit)
		{
			submit((HWND)724026);
		}
		*/
		FreeLibrary(hDllInst);
	}
	//SendMessage((HWND)0x140BA6, WM_SETTEXT, NULL, (LPARAM)_T("测试"));
	//MyFindWindowEx((HWND)0x140BA6, _T("Edit"), TRUE);
	//HWND EditNumHwnd = ::FindWindowEx((HWND)0x140BA6, NULL, NULL, _T("QQEdit"));
	//PostMessage((HWND)0x140BA6, WM_KEYDOWN, VK_RETURN, 0);
	//EnumChildWindows((HWND)0x7043E, enumWindow, 0);
	return 0;
}
/// <summary>
/// 查找窗体上控件句柄
/// </summary>
/// <param name="hwnd">父窗体句柄</param>
/// <param name="lpszWindow">控件标题(Text)</param>
/// <param name="bChild">设定是否在子窗体中查找</param>
/// <returns>控件句柄，没找到返回IntPtr.Zero</returns>
HWND MyFindWindowEx(HWND hwnd, LPCTSTR lpszWindow, bool bChild)
{
	// 首先在父窗体上查找控件
	iResult = FindWindowEx(hwnd, 0, NULL, lpszWindow);
	// 如果找到直接返回控件句柄
	if (iResult != NULL) return iResult;

	// 如果设定了不在子窗体中查找
	if (!bChild) return iResult;

	// 枚举子窗体，查找控件句柄
	int i = EnumChildWindows(
		hwnd,
		enumWindow,
		(LPARAM)lpszWindow);
	// 返回查找结果
	return iResult;
}




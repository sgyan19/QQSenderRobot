// testQQRobot.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "testQQRobot.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// 唯一的应用程序对象

CWinApp theApp;

using namespace std;


void sendStringToClipboard(HWND hwnd, CString source)
{
	if (OpenClipboard(hwnd))
	{
		HGLOBAL clipbuffer;
		char * buffer;
		EmptyClipboard();
		clipbuffer = GlobalAlloc(GMEM_DDESHARE, source.GetLength() + 1);
		buffer = (char*)GlobalLock(clipbuffer);
		strcpy_s(buffer, source.GetLength() + 1, source.GetBuffer());
		GlobalUnlock(clipbuffer);
		SetClipboardData(CF_TEXT, clipbuffer);
		CloseClipboard();
		source.ReleaseBuffer(-1);
	}
}

void sendClipboardMessage(CWnd *pwnd)
{
	keybd_event(VK_CONTROL, 0x1D, 0, 0);
	pwnd->PostMessage(WM_KEYDOWN, 0x00000056, 0x002F0001);
	Sleep(500);
	pwnd->PostMessage(WM_KEYUP, 0x00000056, 0xC02F0001);
	pwnd->PostMessage(WM_KEYDOWN, 0x0000000D, 0x001C0001);
	Sleep(500);
	pwnd->PostMessage(WM_KEYUP, 0x0000000D, 0xC01C0001);
	keybd_event(VK_CONTROL, 0x1D, KEYEVENTF_KEYUP, 0);
}


int main()
{
    int nRetCode = 0;

    HMODULE hModule = ::GetModuleHandle(nullptr);

    if (hModule != nullptr)
    {
        // 初始化 MFC 并在失败时显示错误
        if (!AfxWinInit(hModule, nullptr, ::GetCommandLine(), 0))
        {
            // TODO: 更改错误代码以符合您的需要
            wprintf(L"错误: MFC 初始化失败\n");
            nRetCode = 1;
        }
        else
        {
            // TODO: 在此处为应用程序的行为编写代码。
			HWND hw = (HWND)0x00150060;
			CWnd* pQQWnd;
			pQQWnd = CDialog::FromHandle(hw);
			sendStringToClipboard(hw, CString("本消息由robot 发送"));
			sendClipboardMessage(pQQWnd);
        }
    }
    else
    {
        // TODO: 更改错误代码以符合您的需要
        wprintf(L"错误: GetModuleHandle 失败\n");
        nRetCode = 1;
    }

    return nRetCode;
}

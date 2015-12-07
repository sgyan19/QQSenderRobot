// testQQRobot2.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <windows.h>
void sendStringToClipboard(HWND hwnd, char const *source,int len)
{
	if (OpenClipboard(hwnd))
	{
		HGLOBAL clipbuffer;
		char * buffer;
		EmptyClipboard();
		clipbuffer = GlobalAlloc(GMEM_DDESHARE, len);
		buffer = (char*)GlobalLock(clipbuffer);
		strcpy_s(buffer,100, source);
		GlobalUnlock(clipbuffer);
		SetClipboardData(CF_TEXT, clipbuffer);
		CloseClipboard();
	}
}

void sendClipboardMessage(HWND hwnd)
{
	keybd_event(VK_CONTROL, 0x1D, 0, 0);
	PostMessage(hwnd ,WM_KEYDOWN, 0x00000056, 0x002F0001);
	Sleep(500);
	PostMessage(hwnd,WM_KEYUP, 0x00000056, 0xC02F0001);
	PostMessage(hwnd,WM_KEYDOWN, 0x0000000D, 0x001C0001);
	Sleep(500);
	PostMessage(hwnd,WM_KEYUP, 0x0000000D, 0xC01C0001);
	keybd_event(VK_CONTROL, 0x1D, KEYEVENTF_KEYUP, 0);
}

int main()
{
	HWND hw = (HWND)0x00150060;
	char buffer[100];
	sprintf_s(buffer,100,"机器人转发信息");
	sendStringToClipboard(hw, buffer,100);
	while (1)
	{
		Sleep(5000);
		sendClipboardMessage(hw);
	}

    return 0;
}

